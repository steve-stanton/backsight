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
            return Array.Find<IdGroup>(m_IdGroups, g => g.Id == groupId);
        }

        /// <summary>
        /// Attempts to find the ID group that encloses a specific raw ID
        /// </summary>
        /// <param name="rawId">The raw ID of interest</param>
        /// <returns>The corresponding ID group (null if not found)</returns>
        internal IdGroup FindGroupByRawId(uint rawId)
        {
            return Array.Find<IdGroup>(m_IdGroups, g => (uint)g.LowestId <= rawId && rawId <= (uint)g.HighestId);
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
            idh.FreeReservedId();

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
                    IdGroup entGroup = Array.Find<IdGroup>(groups, g => g.Id==gid);
                    Debug.Assert(entGroup!=null);
                    result.Add(e.Id, entGroup);
                }
            }

            return result;
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
                IdPacket p = g.GetAllocation(false);
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
        /// Discards any IDs that may have been reserved (but which are no longer needed). This
        /// should be called in situations where a user cancels from a data entry dialog.
        /// </summary>
        internal void FreeAllReservedIds()
        {
            foreach (IdGroup group in m_IdGroups)
                group.FreeAllReservedIds();
        }
	}
}
