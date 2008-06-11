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
using System.Diagnostics;
using System.Data.SqlClient;

using Backsight.Data;
using Backsight.Data.BacksightDataSetTableAdapters;
using Backsight.Environment;

namespace Backsight.Editor
{
	/// <written by="Steve Stanton" on="16-DEC-1998" />
    /// <summary>
    /// Management of ID assignment. One of these objects forms part of the
    /// <c>EditingController</c> class. It is responsible for maintaining a
    /// connection to the external database that holds ID info. It acts as a
    ///	server for dishing out IDs.
    /// </summary>
    class IdManager
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

        /// <summary>
        /// The ID groups in the database.
        /// </summary>
        readonly IdGroup[] m_IdGroups;

        /// <summary>
        /// Index of the ID groups for each entity type. The key is the ID of the entity type,
        /// the values are elements in the <c>m_IdGroups</c> array.
        /// </summary>
        readonly Dictionary<int, IdGroup> m_EntityGroups;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        internal IdManager()
        {
            IdManager.Current = this;

	        m_IdGroups = GetGroups();
            m_EntityGroups = GetEntityGroups(m_IdGroups);
        }

        #endregion

        /// <summary>
        /// The ID groups in the database.
        /// </summary>
        internal IdGroup[] IdGroups
        {
            get { return m_IdGroups; }
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

            // Get the ID group to make the reservation
            IdGroup g = GetGroup(ent);
            if (g==null)
                return false;
            else
                return g.ReserveId(idh, id);
        }

        /// <summary>
        /// Loads all the ID groups known to the database.
        /// </summary>
        IdGroup[] GetGroups()
        {
            IIdGroup[] groups = EnvironmentContainer.Current.IdGroups;
            List<IdGroup> result = new List<IdGroup>(groups.Length);

            foreach(IIdGroup group in groups)
            {
                if (group.Id != 0)
                {
                    IdGroup idg = new IdGroup(group);
                    result.Add(idg);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Generates an index that cross-references each entity type with its corresponding
        /// ID group.
        /// </summary>
        /// <param name="groups">The ID groups to index</param>
        /// <returns>Index of the ID groups for each entity type. The key is the ID of the entity type,
        /// the values are elements in the <paramref name="groups"/> array.</returns>
        Dictionary<int, IdGroup> GetEntityGroups(IdGroup[] groups)
        {
            IEntity[] ents = EnvironmentContainer.Current.EntityTypes;
            Dictionary<int, IdGroup> result = new Dictionary<int, IdGroup>(ents.Length);

            foreach (IEntity e in ents)
            {
                IIdGroup idg = e.IdGroup;
                if (idg!=null && idg.Id>0)
                {
                    int gid = idg.Id;
                    IdGroup entGroup = Array.Find<IdGroup>(groups,
                        delegate(IdGroup g) { return (g.Id==gid); });
                    Debug.Assert(entGroup!=null);
                    result.Add(e.Id, entGroup);
                }
            }

            return result;
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
        /// Returns the ID group that corresponds to a specific entity type.
        /// </summary>
        /// <param name="ent">The entity type to find.</param>
        /// <returns>The matching group (null if no such group)</returns>
        internal IdGroup GetGroup(IEntity ent)
        {
            return (ent==null ? null : m_EntityGroups[ent.Id]);
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
