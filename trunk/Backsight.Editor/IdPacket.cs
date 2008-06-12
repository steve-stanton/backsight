/// <remarks>
/// Copyright 2008 - Steve Stanton. This file is part of Backsight
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

using Backsight.Editor.Database;

namespace Backsight.Editor
{
    /// <summary>
    /// A packet of user-perceived IDs.
    /// </summary>
    /// <remarks>This class should hopefully replace <c>IdRange</c></remarks>
    class IdPacket
    {
        #region Class data

        /// <summary>
        /// The ID group that contains this packet
        /// </summary>
        readonly IdGroup m_Group;

        /// <summary>
        /// The database information about the allocation
        /// </summary>
        readonly IdAllocation m_Allocation;

        /// <summary>
        /// References to allocated IDs. The  first element corresponds to m_LowestId,
        /// while the last corresponds to m_HighestId. When created, all ID references
        /// will be null. They get defined only when a user makes use of ID. When that
        /// happens, the reference is defined at the appropriate place in the array,
        /// m_NumUsed is incremented, and m_NumFree is decremented.
        /// </summary>
        readonly FeatureId[] m_Ids;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>IdPacket</c>
        /// </summary>
        /// <param name="group">The group that will contain this packet (not null)</param>
        /// <param name="alloc">The allocation associated with the group (not null)</param>
        /// <exception cref="ArgumentNullException">If either parameter is null</exception>
        /// <exception cref="ArgumentException">If the allocation does not refer to the supplied group</exception>
        internal IdPacket(IdGroup group, IdAllocation alloc)
        {
            if (group==null || alloc==null)
                throw new ArgumentNullException();

            if (group.Id != alloc.GroupId)
                throw new ArgumentException();

            m_Group = group;
            m_Allocation = alloc;
            m_Ids = new FeatureId[m_Allocation.Size];
        }

        #endregion

        /// <summary>
        /// Check if this ID packet refers to a specific feature ID.
        /// </summary>
        /// <param name="fid">The feature ID to check for.</param>
        /// <returns>True if the feature ID was found.</returns>
        internal bool IsReferredTo(FeatureId fid)
        {
            if (fid==null)
                return false;
            else
                return (GetIndex(fid)>=0);
        }


        /// <summary>
        /// Returns the array index for a specific feature ID.
        /// </summary>
        /// <param name="fid">The ID we want the index for.</param>
        /// <returns>The index value, or -1 if the ID was not found.</returns>
        int GetIndex(FeatureId fid)
        {
            return Array.IndexOf<FeatureId>(m_Ids, fid);
        }

        /// <summary>
        /// Deletes an ID that this packet points to. This nulls out the reference to the ID,
        /// and should be called ONLY if the feature ID is being eliminated as a result of undo.
        /// </summary>
        /// <param name="fid">The feature ID to remove. At call, it should be inactive (not
        /// referring to anything).</param>
        /// <returns>True if ID reference has been nulled out.</returns>
        internal bool DeleteId(FeatureId fid)
        {
            // Confirm the ID is inactive.
            if (!fid.IsInactive)
                throw new Exception("IdPacket.DeleteId - ID is still in use");

            // Get the array index of the ID.
            int index = GetIndex(fid);
            if (index < 0)
                return false;

            m_Ids[index] = null;
            return true;
        }

        /// <summary>
        /// The lowest value in the allocation
        /// </summary>
        public int Min
        {
            get { return m_Allocation.LowestId; }
        }

        /// <summary>
        /// The highest value in the allocation
        /// </summary>
        public int Max
        {
            get { return m_Allocation.HighestId; }
        }

        /// <summary>
        /// The number of IDs in the allocation
        /// </summary>
        internal int Size
        {
            get { return m_Allocation.Size; }
        }

        /// <summary>
        /// The number of IDs that have been used
        /// </summary>
        internal int NumUsed
        {
            get { return m_Allocation.NumUsed; }
        }
    }
}
