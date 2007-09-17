/// <remarks>
/// Copyright 2007 - Steve Stanton. This file is part of Backsight
///
/// Backsight is free software; you can redistribute it and/or modify it under the terms
/// of the GNU Lesser General Public License as published by the Free Software Foundation;
/// either version 3 of the License, or (at your option) any later version.
///
/// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
/// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU Lesser General Public License for more details.
///
/// You should have received a copy of the GNU Lesser General Public License
/// along with this program. If not, see <http://www.gnu.org/licenses/>.
/// </remarks>

using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;

using Backsight.Data;
using Backsight.Data.BacksightDataSetTableAdapters;
using System.Diagnostics;
using System.Data.SqlClient;
using Backsight.Environment;

namespace Backsight.Editor
{
	/// <written by="Steve Stanton" on="16-DEC-1998" />
    /// <summary>
    /// Management of ID assignment. One of these objects forms part of the
    /// <c>CadastralEditController</c> class. It is responsible for maintaining a
    /// connection to the external database that holds ID info. It acts as a
    ///	server for dishing out IDs.
    /// </summary>
    public class IdManager : IDisposable
    {
        #region Statics

        private static IdManager s_Manager;

        internal static IdManager Current
        {
            get { return s_Manager; }
            private set { s_Manager = value; }
        }

        #endregion


        #region Class data

        // Connection to the database.
        //private IIdDatabase m_DB;

        /// <summary>
        /// The ID groups in the database.
        /// </summary>
        private IdGroup[] m_IdGroups;

        /// <summary>
        /// Association of ID info with those entity types that can be assigned an ID.
        /// </summary>
        private IdEntity[] m_IdEntities;

        /// <summary>
        /// True if messages reporting that feature IDs cannot be assigned will be
        /// displayed.
        /// </summary>
        private bool m_DisplayIDMessage;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        internal IdManager()
        {
            IdManager.Current = this;

	        GetGroups();
	        GetEntities();

            // Default is to display ID Allocation Error Messages
            m_DisplayIDMessage = true;
        }

        #endregion

        internal IdGroup[] IdGroups
        {
            get { return m_IdGroups; }
        }

        internal bool DisplayIDMessage
        {
            get { return m_DisplayIDMessage; }
            private set { m_DisplayIDMessage = value; }
        }

        public void Dispose()
        {
	        Close();

            if (Object.ReferenceEquals(s_Manager, this))
                s_Manager = null;
        }

        /// <summary>
        /// Reserves the next available ID for a given entity type.
        /// </summary>
        /// <param name="idh">The ID handle to fill in.</param>
        /// <param name="ent">The entity type to search for.</param>
        /// <param name="id">The specific ID to reserve (specify 0 to get the next available ID).</param>
        /// <returns>True if the ID handle was filled in successfully.</returns>
        internal bool ReserveId(IdHandle idh, IEntity ent, uint id)
        {
            // Ensure the ID handle is free.
            if (!idh.FreeId())
            {
                MessageBox.Show("IdManager.ReserveId - Cannot free old ID");
                return false;
            }

            // The entity type MUST be specified.
            if (ent==null)
                return false;

            // Search the array of ID entities looking for a match.
            IdEntity idEnt = Array.Find<IdEntity>(m_IdEntities,
                delegate(IdEntity e) { return (e.Entity.Name == ent.Name); });

            // Return if we did not find a match (entity type cannot be
            // assigned an ID).
            if (idEnt==null)
                return false;

            // Increment the number of hits on the 
            // Get the ID entity to do the allocation.
            bool isReserved = idEnt.ReserveId(idh, id);

            // Sort the ID entities so that the most frequently accessed entities come
            // first in the list.
            Array.Sort<IdEntity>(m_IdEntities,
                delegate(IdEntity a, IdEntity b) { return b.NumHit.CompareTo(a.NumHit); });

            return isReserved;
        }

        /// <summary>
        /// Loads all the ID groups known to the database.
        /// </summary>
        void GetGroups()
        {
        	// Get rid of any previously selected groups.
        	m_IdGroups = null;

            IIdGroup[] groups = EnvironmentContainer.Current.IdGroups;
            List<IdGroup> result = new List<IdGroup>(groups.Length);

            foreach(IIdGroup group in groups)
            {
                if (group.Id != 0)
                    result.Add(CreateIdGroup(group));
            }

            m_IdGroups = result.ToArray();
        }

        /// <summary>
        /// Creates an ID group using a row selected from the IdGroup table.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        IdGroup CreateIdGroup(IIdGroup group)
        {
            IdGroup result = new IdGroup(group);
            return result;
            // Confirm that the format does not generate any numeric digits.
            // If it does, issue a warning and use the default format.
            /*
                        if ( m_KeyFormat.GetLength() ) {

                            CHARS keystr[32];
                            sprintf(keystr,(LPCTSTR)m_KeyFormat,0);
                            UINT4 slen = strlen(keystr);
                            LOGICAL ok = TRUE;

                            for ( UINT4 i=0; i<slen && ok; i++ ) {
                                CHARS charval = keystr[i];
                                if ( isdigit(charval) && charval!='0' ) ok = FALSE;
                            }

                            if ( !ok ) {
                                CString errmsg;
                                errmsg.Format( "%s '%s' %s\n%s"
                                    , "The key format string", m_KeyFormat, "generates extraneous numeric digits."
                                    , "The default numeric key format will be used instead.");
                                ShowMessage(errmsg);
                                m_KeyFormat = _T("%d");
                            }
                        }
                        else
                            m_KeyFormat = _T("%d");
                     */
        }

        /// <summary>
        /// Loads all the ID entities known to the database.
        /// </summary>
        void GetEntities()
        {
            Debug.Assert(m_IdGroups!=null); // GetGroups should have been called first

	        // Get rid of any previously selected ID-entity associations.
        	m_IdEntities = null;

            // This formerly grabbed stuff from an old 'IdEntities' table that held
            // the name of an ID group & the name of an entity type. However, I don't
            // understand why that was necessary (apart from the possibility that it
            // was a kludge to avoid the need for modifying an existing database table).
            // So unless some reason becomes apparent, simplify things by looking at
            // defined entity types. From what I can tell, each entity type can be
            // associated with just 1 ID group (or 0).

            // I think the issue was the fact that the entity types were previously
            // stored as part of each CED file (not represented whatsoever in the
            // database). IdEntities formed the only bridge between the IdGroup
            // table and the external entity definitions (which is why it would
            // contain the name of the entity type rather than an ID).

            IEntity[] ents = EnvironmentContainer.Current.EntityTypes;
            List<IdEntity> idEnts = new List<IdEntity>(ents.Length);
            foreach (IEntity e in ents)
            {
                IIdGroup idg = e.IdGroup;
                int gid = (idg==null ? 0 : idg.Id);

                if (gid != 0)
                {
                    IdGroup entGroup = Array.Find<IdGroup>(m_IdGroups,
                        delegate(IdGroup g) { return (g.Id==gid); });
                    Debug.Assert(entGroup!=null);
                    IdEntity idEnt = new IdEntity(entGroup, e);
                    idEnts.Add(idEnt);
                }
            }

            m_IdEntities = idEnts.ToArray();
        }

        /// <summary>
        /// Performs initialization that is required each time a different map is
        /// opened for editing.
        /// </summary>
        /// <param name="map">The map to do the setup for (null to reset).</param>
        internal void MapOpen(CadastralMapModel map)
        {
            // Cross-reference each ID group with the applicable ID ranges.
            SetGroupRanges(map);
        }

        /// <summary>
        /// Resets stuff when a map is closed.
        /// </summary>
        void MapClose()
        {
            MapOpen(null);
        }

        /// <summary>
        /// Cross-references each ID group with the ID ranges that are stored in a specific map.
        /// </summary>
        /// <param name="map">The map to do the setup for.</param>
        void SetGroupRanges(CadastralMapModel map)
        {
            // Eliminate any ID ranges that may be known to each ID group.
            foreach (IdGroup g in m_IdGroups)
                g.KillRanges();

            // Return if there is no map.
            if (map==null)
                return;

            // Associate each group with the ID ranges that are already
            // known to the map.

            List<IdRange> ranges = map.IdRanges;
            foreach (IdRange r in ranges)
            {
                // Loop through the ID groups to find the group that encloses the range.
                IdGroup group = Array.Find<IdGroup>(m_IdGroups,
                    delegate(IdGroup g) { return g.IsOwnerOf(r); });
                Debug.Assert(group!=null);
                bool rangeAdded = group.AddIdRange(r);
                Debug.Assert(rangeAdded==true);
            }
        }

        /// <summary>
        /// Closes down this ID manager. 
        /// </summary>
        /// <devnote>This may no longer be necessary. The comment in the original C++ code was:
        /// This disconnects from the database and deletes all info that was obtained from there.
        ///
        /// Note that I was originally doing this stuff in the destructor. However, it appears
        /// that MFC does not call the destructor when the app is closed down, so the database
        /// connection is not gracefully closed down. To get around that, this function is now
        /// called by CEditDoc::OnCloseDocument().
        /// </devnote>
        void Close()
        {
            /*
            // Disconect from the database.
            m_DB.Close();

            // Get rid of the arrays that we loaded from the database.

            delete[] m_IdGroups;
            delete[] m_IdEntities;

            m_IdGroups = 0;
            m_IdEntities = 0;
            m_NumGroup = 0;
            m_NumEntity = 0;
             */
        }

        /// <summary>
        /// Gets a new allocation for a specific ID group.
        /// </summary>
        /// <param name="group">The group to get the allocation for.</param>
        /// <param name="announce">Should the new range be announced to the user? Default=TRUE.
        /// The only time when it's not appropriate to announce is when the user is explicitly
        /// allocating ID ranges.</param>
        /// <returns>The number of ranges that were added.</returns>
        internal uint GetAllocation(IdGroup group, bool announce)
        {
            // I assume that the specified group is actually
            // one of the groups known to this ID manager.

            // Let the group do the work.
            return group.GetAllocation(announce);
        }

        /// <summary>
        /// Gets a new allocation for every ID group. This function is used only when the
        /// user is explicitly allocating ID ranges.
        /// </summary>
        /// <returns>The number of ranges that were added.</returns>
        internal uint GetAllocation()
        {
	        uint nadd=0;

            foreach (IdGroup g in m_IdGroups)
                nadd += GetAllocation(g, false);

	        return nadd;
        }

        /// <summary>
        /// Releases the unused portion of all ID ranges known to all ID groups.
        /// </summary>
        internal void Release()
        {
            // Tell each group to truncate all ID ranges.
            foreach (IdGroup g in m_IdGroups)
                g.ReleaseAll();
        }

        /// <summary>
        /// Releases the unused portion of a specific ID range known to a specific ID group.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="minid"></param>
        /// <param name="maxid"></param>
        /// <returns>True if the range was found and successfully released (it may have already
        /// been released). False if the range could not be found (an error message is displayed),
        /// or the release failed for some reason.</returns>
        /*
        bool Release(IdGroup group, uint minid, uint maxid)
        {
            // Formerly passed down the database connection known to CeIdManager,
            // so could probably do without this method.

            return group.Release(minid, maxid);
        }
         */

        /// <summary>
        /// Returns a specific ID group
        /// </summary>
        /// <param name="index">The array index of the group.</param>
        /// <returns>The group at the specified array index (null if out of range).</returns>
        IdGroup GetGroup(uint index)
        {
            if (index < m_IdGroups.Length)
                return m_IdGroups[index];
            else
                return null;
        }

        /// <summary>
        /// Returns the ID group that corresponds to a specific entity type.
        /// </summary>
        /// <param name="ent">The entity type to find.</param>
        /// <returns>The matching group (null if no such group)</returns>
        internal IdGroup GetGroup(IEntity ent)
        {
            // The entity type MUST be specified.
            if (ent==null)
                return null;

            IdEntity idEnt = Array.Find<IdEntity>(m_IdEntities,
                delegate(IdEntity i) { return (i.Entity.Name == ent.Name); });

            // Return if we did not find a match (entity type cannot be assigned an ID).
            if (idEnt==null)
                return null;

            // What ID group does the entity fall in?
            return idEnt.Group;
        }

        /// <summary>
        /// Loads a list with all the IDs that are available for a specific entity type.
        /// </summary>
        /// <param name="ent">The entity type to search for.</param>
        /// <param name="avail">The list to load.</param>
        /// <returns>The number of IDs that were added to the list.</returns>
        internal uint GetAvailIds(IEntity ent, List<uint> avail)
        {
            // Get the ID group for the specified entity type.
            IdGroup group = GetGroup(ent);
            if (group==null)
                return 0;

            // Get the group to load the available IDs.
            uint navail = group.GetAvailIds(avail);

            // If the group didn't have anything, allocate a new range,
            // announcing the fact to the user.
            if (navail==0)
            {
                group.GetAllocation(true);
                navail = group.GetAvailIds(avail);
            }

            return navail;
        }

        /// <summary>
        /// Gets rid of a pointer to a feature ID.
        /// </summary>
        /// <param name="fid">The ID to search for.</param>
        /// <param name="ent">The entity type for the ID.</param>
        /// <returns>True if the ID pointer was removed. False if it could not be found.</returns>
        internal bool DeleteId(FeatureId fid, IEntity ent)
        {
        	IdRange range = null;		// Range not found so far.

	        // Search the ID group that the ID SHOULD be in.
	        IdGroup group = GetGroup(ent);
	        if (group!=null)
                range = group.FindRange(fid);

	        // If we didn't find it, we must have been supplied the wrong
	        // entity type, so every group we know about (we'll repeat
	        // the above, but who cares).
	        if (range==null)
            {
                foreach (IdGroup g in m_IdGroups)
                {
                    range = g.FindRange(fid);
                    if (range!=null)
                        break;
                }
	        }

        	// Issue an error message if we STILL haven't found the range.
	        if (range==null)
            {
		        string errmsg = String.Format("Unable to delete ID '{0}'", fid.FormattedKey);
                MessageBox.Show(errmsg);
		        return false;
	        }

	        // I SUPPOSE it could be a range that refers to an ID group
	        // that is now obsolete, so we COULD also check every range
	        // known to the map ... later

	        // Get the range to remove the pointer.
	        return range.DeleteId(fid);

	        // Note that the ID slot will only be re-used if the range
	        // has not been released. If the range HAS been released, the
	        // ID disappears into the ether, and will never come back.
        }

        /// <summary>
        /// Handles the very first time the map is saved.
        /// </summary>
        void OnFirstSave()
        {
        	// The map SHOULD be defined.
            CadastralMapModel map = CadastralMapModel.Current;
            if (map==null)
                return;

        	// Get the name that has been assigned to the map.
            string fname = Path.GetFileNameWithoutExtension(map.Name);

	        // Go through any ID allocations already known to the map,
	        // and ensure the file name is stored in the IdAllocation
	        // table.

            List<IdRange> ranges = map.IdRanges;

            Transaction.Execute(delegate
            {
                foreach (IdRange range in ranges)
                {
                    string sql = String.Format("update IdAllocation set FileName='{0}' where LowestId={1}"
                                    , fname, range.Min);
                    SqlCommand cmd = new SqlCommand(sql, Transaction.Connection.Value);
                    int nRows = cmd.ExecuteNonQuery();
                    Debug.Assert(nRows==1);
                }
            });
        }
	}
}
