// <remarks>
// Copyright 2008 - Steve Stanton. This file is part of Backsight
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
        /// happens, the reference is defined at the appropriate place in the array.
        /// </summary>
        NativeId[] m_Ids;

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
            m_Ids = new NativeId[m_Allocation.Size];
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
        /// Returns the array index for a specific raw ID.
        /// </summary>
        /// <param name="id">The ID we want the index for.</param>
        /// <returns>The index value, or -1 if the ID is out of range.</returns>
        int GetIndex(uint id)
        {
            // Confirm that the specified id is in range.
            if (id < Min || id > Max)
                return -1;

            // Return the index position.
            return (int)(id - Min);
        }

        /// <summary>
        /// Deletes an ID that this packet points to. This nulls out the reference to the ID,
        /// and should be called ONLY if the feature ID is being eliminated as a result of undo.
        /// </summary>
        /// <param name="fid">The feature ID to remove. At call, it should be inactive (not
        /// referring to anything).</param>
        /// <returns>True if ID reference has been nulled out.</returns>
        internal bool DeleteId(NativeId fid)
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
            //get { return m_Allocation.NumUsed; }
            get
            {
                int result = 0;

                for (int i=0; i<m_Ids.Length; i++)
                {
                    if (m_Ids[i] != null)
                        result++;
                }

                return result;
            }
        }
        /// <summary>
        /// The number of IDs that are currently free. This is the total number of null
        /// references in m_Ids PLUS the number of references to inactive IDs.
        /// </summary>
        uint GetNumFree()
        {
            uint result = 0;

            foreach (FeatureId fid in m_Ids)
            {
                if (fid == null || fid.IsInactive)
                    result++;
            }

            return result;
        }

        /// <summary>
        /// The ID group that contains this packet
        /// </summary>
        internal IdGroup IdGroup
        {
            get { return m_Group; }
        }

        /// <summary>
        /// Reserves a specific ID in this packet.
        /// </summary>
        /// <param name="id">The raw ID to reserve</param>
        /// <returns>True if the ID has been reserved successfully.</returns>
        internal bool ReserveId(uint id)
        {
            // Get the index of the specified ID.
            int index = GetIndex(id);
            if (index < 0)
                throw new Exception("IdPacket.ReserveId - Index out of range");

            // Confirm that the ID is not already in use. It MAY have
            // a defined ID pointer, but if it's inactive, that's ok.
            if (m_Ids[index] != null)
            {
                if (!m_Ids[index].IsInactive)
                    throw new Exception("IdPacket.ReserveId - ID already used");
            }

            // Get the group to reserve the ID.
            return m_Group.ReserveId(id);
        }

        /// <summary>
        /// Reserves the next available ID in this packet (if any).
        /// </summary>
        /// <returns>The reserved ID (null if an ID could not be obtained).</returns>
        internal uint ReserveId()
        {
            // Return if the packet has been completely used up.
            uint numFree = GetNumFree();
            if (numFree == 0)
                return 0;

            // If the packet contains any inactive IDs, we will need to check for them.
            int numnull = Size - NumUsed;
            bool hasInactive = (numFree > numnull);

            // Scan though the array of ID pointers, looking for a free slot.
            // If we find one, get the group to confirm that it's ok to
            // reserve it (if not, keep going).

            for (uint i = 0; i < m_Ids.Length; i++)
            {
                if (m_Ids[i] == null)
                {
                    uint keynum = (uint)Min + i;
                    if (m_Group.ReserveId(keynum))
                        return keynum;
                }
                else
                {
                    if (hasInactive && m_Ids[i].IsInactive)
                    {
                        uint keynum = (uint)Min + i;
                        if (m_Group.ReserveId(keynum))
                            return keynum;
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// Loads a list with all the IDs that are available for this packet.
        /// </summary>
        /// <param name="avail">The list to load.</param>
        /// <returns>The number of IDs that were added to the array.</returns>
        internal uint GetAvailIds(List<uint> avail)
        {
            // Return if the packet has been completely used up.
            uint numFree = GetNumFree();
            if (numFree == 0)
                return 0;

            // If the range contains any inactive IDs, we will need to check for them.
            int numnull = Size - NumUsed;
            bool hasInactive = (numFree > numnull);

            uint navail = 0; // Nothing found so far.

            // Check if the group has any reserved IDs in THIS range.

            uint nres = m_Group.GetReserveCount((uint)Min, (uint)Max);
            if (nres == 0)
            {
                // No reserves, so just scan though the array of ID pointers,
                // appending every free slot to the return array.

                for (uint i = 0, keynum = (uint)Min; i < m_Ids.Length; i++, keynum++)
                {
                    if (m_Ids[i] == null)
                    {
                        avail.Add(keynum);
                        navail++;
                    }
                    else
                    {
                        if (hasInactive && m_Ids[i].IsInactive)
                        {
                            avail.Add(keynum);
                            navail++;
                        }
                    }
                }
            }
            else
            {
                // Get the reserves
                List<uint> reserves = new List<uint>((int)nres);
                m_Group.GetReserveIds((uint)Min, (uint)Max, reserves);

                // As above, scan through the array of ID pointers, looking
                // for a free slot. But don't add in any IDs that have been
                // reserved.

                for (uint i = 0, keynum = (uint)Min; i < m_Ids.Length; i++, keynum++)
                {
                    uint tnum = 0;
                    if (m_Ids[i] != null)
                    {
                        if (hasInactive && m_Ids[i].IsInactive)
                            tnum = keynum;
                    }
                    else
                        tnum = keynum;

                    if (tnum != 0)
                    {
                        bool isReserved = false;

                        for (int j = 0; j < nres && !isReserved; j++)
                        {
                            if (keynum == reserves[j])
                                isReserved = true;
                        }

                        if (!isReserved)
                        {
                            avail.Add(keynum);
                            navail++;
                        }
                    }
                }
            }

            return navail;
        }

        /// <summary>
        /// Checks whether this packet can be extended with an additional range.
        /// </summary>
        /// <param name="minid">The low end of the proposed extension.</param>
        /// <param name="maxid">The high end of the proposed extension.</param>
        /// <returns>True if the packet has been extended.</returns>
        internal bool Extend(int minid, int maxid)
        {
            // The beginning of the extension MUST follow the existing range.
            if (Max + 1 != minid)
                return false;

            // The extension must be valid!
            if (maxid < minid)
                return false;

            // Update the allocation recorded in the database. We do not
            // worry about the timestamp, or who made the allocation.
            if (m_Allocation.UpdateHighestId(maxid) != 1)
                return false;

            // Extend the allocation of pointers that we have.
            uint nextra = (uint)(maxid - minid + 1);
            uint nalloc = (uint)(m_Ids.Length + nextra);
            NativeId[] newIds = new NativeId[nalloc];

            // Copy over what we had (the rest contains nulls)
            Array.Copy(m_Ids, newIds, m_Ids.Length);

            // Replace the old array with the new one
            m_Ids = newIds;
            return true;
        }

        /// <summary>
        /// Checks whether this packet is available for generating one additional ID.
        /// </summary>
        /// <returns>True if the packet contains an available ID.</returns>
        internal bool HasAvail()
        {
            // Return if nothing is free.
            uint numFree = GetNumFree();
            if (numFree == 0)
                return false;

            // If the packet contains any inactive IDs, we will need to check for them.
            int numnull = Size - NumUsed;
            bool hasInactive = (numFree > numnull);

            // Scan the array looking for a free slot.

            for (uint i = 0; i < m_Ids.Length; i++)
            {
                if (m_Ids[i] == null)
                {
                    uint keynum = (uint)Min + i;
                    if (!m_Group.IsReserved(keynum))
                        return true;
                }
                else
                {
                    if (hasInactive && m_Ids[i].IsInactive)
                    {
                        uint keynum = (uint)Min + i;
                        if (!m_Group.IsReserved(keynum))
                            return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Restores an ID pointer that this range points to. This confirms that this
        /// packet really does point to the ID and, if so, the number of free IDs will
        /// be decremented.
        /// <para/>
        /// This function undoes a call to FreeId, and is called when a user-perceived
        /// deletion is being rolled back.
        /// </summary>
        /// <param name="fid">The feature ID to restore.</param>
        /// <returns>True if ID pointer was found.</returns>
        /// <remarks>TODO: This method now does nothing, since m_NumFree is no longer
        /// noted explicitly. Should look into calls. However, the other thing to also
        /// note is that FreeId is no longer called, so perhaps other things are amiss
        /// in the undo logic.</remarks>
        internal bool RestoreId(FeatureId fid)
        {
            // TODO: No need to do anything, since m_NumFree is no longer 
            /*
            // Get the array index of the ID.
            int index = GetIndex(fid);

            // Return if not found.
            if (index < 0)
                throw new Exception("IdPacket.RestoreId - ID not found");

            // Decrement the number of free slots (DON'T accidentally
            // decrement past zero, because that's a BIG number).
            if (m_NumFree>0)
                m_NumFree--;
            */
            return true;
        }

        /// <summary>
        /// Creates a new feature ID.
        /// </summary>
        /// <param name="id">The ID to create.</param>
        /// <returns>The created feature ID.</returns>
        internal FeatureId CreateId(uint id)
        {
            // Confirm that the specified ID falls within this range.
            if (id<(uint)Min || id>(uint)Max)
                throw new Exception("IdPacket.CreateId - New ID is not in range!");

            // Confirm that the packet does not already refer to an active ID.
            int index = (int)id - Min;
            bool reuse = false;
            NativeId fid = m_Ids[index];

            if (fid!=null)
            {
                if (fid.IsInactive)
                    reuse = true;
                else
                    throw new Exception("IdPacket.CreateId - ID slot has already been used.");
            }

            // If we're not re-using an old ID, create one.
            if (!reuse)
            {
                string keystr;      // The key string

                // Try to get a numeric key.
                uint keyval = m_Group.GetNumericKey(id);

                if (keyval!=0)
                {
                    // If we got one, we still need to represent it as a string (for
                    // the FeatureId constructor). However, we need to let the
                    // constructor know that it's ok to convert it back to numeric!

                    keystr = keyval.ToString();
                }
                else
                {
                    // Format the ID using the key format (+ possible check digit)
                    keystr = m_Group.FormatId(id);
                }

                // Create the new ID
                fid = new NativeId(m_Group, id);
                m_Ids[index] = fid;
            }

            return fid;
        }

        /// <summary>
        /// Records an ID if it belongs to this ID packet
        /// </summary>
        /// <param name="nid">The ID that may belong to this packet</param>
        /// <returns>True if the supplied ID has been recorded as part of
        /// this packet</returns>
        internal bool SetId(NativeId nid)
        {
            int index = GetIndex(nid.RawId);
            if (index < 0)
                return false;

            m_Ids[index] = nid;
            return true;
        }

        /// <summary>
        /// Does this ID packet enclose the supplied raw ID?
        /// </summary>
        /// <param name="rawId">The ID value to check</param>
        /// <returns>True if the ID is between the allocation min/max</returns>
        internal bool Contains(uint rawId)
        {
            return (Min <= rawId && rawId <= Max);
        }
    }
}
