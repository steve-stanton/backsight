// <remarks>
// Copyright 2007 - Steve Stanton. This file is part of Backsight
//
// Backsight is free software; you can redistribute it and/or modify it under the terms
// of the GNU Lesser General Public License as published by the Free Software Foundation;
// either version 3 of the License, or (at your option) any later version.
//
// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
// </remarks>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

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
        /// Attempts to find the ID group with a specific ID
        /// </summary>
        /// <param name="groupId">The ID of the group to look for</param>
        /// <returns>The corresponding ID group (null if not found)</returns>
        internal IdGroup FindGroupById(int groupId)
        {
            return Array.Find<IdGroup>(m_IdGroups, delegate(IdGroup g)
            {
                return (g.Id == groupId);
            });
        }

        /// <summary>
        /// Attempts to find the ID group that encloses a specific raw ID
        /// </summary>
        /// <param name="rawId">The raw ID of interest</param>
        /// <returns>The corresponding ID group (null if not found)</returns>
        internal IdGroup FindGroupByRawId(uint rawId)
        {
            return Array.Find<IdGroup>(m_IdGroups, delegate(IdGroup g)
            {
                return ((uint)g.LowestId <= rawId && rawId <= (uint)g.HighestId);
            });
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
            IEnvironmentContainer ec = EnvironmentContainer.Current;
            if (ec == null)
                return new IdGroup[0];

            IIdGroup[] groups = ec.IdGroups;
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
            IEnvironmentContainer ec = EnvironmentContainer.Current;
            if (ec == null)
                return new Dictionary<int, IdGroup>();

            IEntity[] ents = ec.EntityTypes;
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
        /// Loads ID information for the specified project and user. This should be called
        /// after edits have been loaded into the map model.
        /// </summary>
        /// <param name="map">The loaded model</param>
        /// <param name="project">The project to load</param>
        /// <param name="user">The user who is doing the load</param>
        internal void Load(CadastralMapModel map, Project project, User user)
        {
            // Grab all defined allocations for the job and user
            //IdAllocation[] allocs = IdAllocation.FindByJobUser(job, user);
            //IdAllocation[] allocs = job.GetIdAllocations();
            throw new NotImplementedException();

            /*
            // Attach each allocation to its group
            foreach (IdAllocation a in allocs)
            {
                IdGroup g = Array.Find<IdGroup>(m_IdGroups, delegate(IdGroup t)
                                { return t.Id==a.GroupId; });
                if (g==null)
                    throw new Exception("Cannot locate ID group for allocation");

                g.AddIdPacket(a);
            }
             */
        }

        /// <summary>
        /// Gets a new allocation for a specific ID group.
        /// </summary>
        /// <param name="group">The group to get the allocation for.</param>
        /// <param name="announce">Should the new range be announced to the user? Default=TRUE.
        /// The only time when it's not appropriate to announce is when the user is explicitly
        /// allocating ID ranges.</param>
        /// <returns>Information about the allocated range.</returns>
        internal IdPacket GetAllocation(IdGroup group, bool announce)
        {
            // I assume that the specified group is actually
            // one of the groups known to this ID manager.

            // Let the group do the work.
            return group.GetAllocation(announce);
        }

        /// <summary>
        /// Gets a new allocation for every ID group. This function is used only when the
        /// user is explicitly allocating ID ranges (through the dialog that lists the allocations).
        /// </summary>
        /// <returns>Information about the allocations made.</returns>
        internal IdPacket[] GetAllocation()
        {
            List<IdPacket> allocs = new List<IdPacket>();

            foreach (IdGroup g in m_IdGroups)
            {
                IdPacket p = GetAllocation(g, false);
                if (p != null)
                    allocs.Add(p);
            }

            return allocs.ToArray();
        }

        /// <summary>
        /// Returns the ID group that corresponds to a specific entity type.
        /// </summary>
        /// <param name="ent">The entity type to find.</param>
        /// <returns>The matching group (null if no such group)</returns>
        internal IdGroup GetGroup(IEntity ent)
        {
            if (ent == null || ent.Id == 0)
                return null;

            IdGroup result;
            if (m_EntityGroups.TryGetValue(ent.Id, out result))
                return result;
            else
                return null;
        }

        /*
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
        */

        /// <summary>
        /// Gets rid of a pointer to a feature ID.
        /// </summary>
        /// <param name="fid">The ID to search for.</param>
        /// <param name="ent">The entity type for the ID.</param>
        /// <returns>True if the ID pointer was removed. False if it could not be found.</returns>
        internal bool DeleteId(NativeId fid, IEntity ent)
        {
        	IdPacket packet = null;		// Packet not found so far.

	        // Search the ID group that the ID SHOULD be in.
	        IdGroup group = GetGroup(ent);
            if (group != null)
                packet = group.FindPacket(fid);

	        // If we didn't find it, we must have been supplied the wrong
	        // entity type, so check every group we know about (we'll repeat
	        // the above, but who cares).
            if (packet == null)
            {
                foreach (IdGroup g in m_IdGroups)
                {
                    packet = g.FindPacket(fid);
                    if (packet != null)
                        break;
                }
	        }

            // Issue an error message if we STILL haven't found the packet.
            if (packet == null)
            {
		        string errmsg = String.Format("Unable to delete ID '{0}'", fid.FormattedKey);
                MessageBox.Show(errmsg);
		        return false;
	        }

	        // I SUPPOSE it could be a range that refers to an ID group
	        // that is now obsolete, so we COULD also check every range
	        // known to the map ... later

	        // Get the range to remove the pointer.
            return packet.DeleteId(fid);

	        // Note that the ID slot will only be re-used if the range
	        // has not been released. If the range HAS been released, the
	        // ID disappears into the ether, and will never come back.
        }

        /// <summary>
        /// Exhaustive search for the ID packet that refers to a specific ID. This method
        /// should only be called in situations where something has gone astray.
        /// </summary>
        /// <param name="fid">The ID to search for</param>
        /// <returns>The packet that contains the specified object (null if not found)</returns>
        internal IdPacket FindPacket(NativeId nid)
        {
            foreach (IdGroup g in m_IdGroups)
            {
                IdPacket p = g.FindPacket(nid);
                if (p!=null)
                    return p;
            }

            return null;
        }

        /// <summary>
        /// Records any ID utilized by a feature
        /// </summary>
        /// <param name="f">The feature to examine</param>
        /// <param name="hint">The ID packet that may well contain the ID in
        /// question (null for no hint)</param>
        /// <returns>The ID packet that actually contains the ID (null if not found)</returns>
        internal IdPacket AddUsedId(Feature f, IdPacket hint)
        {
            // We only care about features with native IDs
            NativeId nid = (f.FeatureId as NativeId);
            if (nid == null)
                return hint;

            if (hint!=null && hint.SetId(nid))
                return hint;

            // Try the ID group for the feature's entity type. If we can't find it
            // that way (we probably should), make an exhaustive search
            IdPacket p = null;
            IdGroup g;
            if (m_EntityGroups.TryGetValue(f.EntityType.Id, out g))
                p = g.FindPacket(nid);
            else
                p = FindPacket(nid);

            // Just in case...
            if (p == null)
                return null;

            bool set = p.SetId(nid);
            Debug.Assert(set);
            return p;
        }
	}
}
