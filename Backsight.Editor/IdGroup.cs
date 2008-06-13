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

using Backsight.Environment;
using Backsight.Data;
using Backsight.Editor.Database;

namespace Backsight.Editor
{
	/// <written by="Steve Stanton" on="14-DEC-1998" />
    /// <summary>
    /// An ID group, corresponding to a row that was selected from the IdGroup table.
    /// </summary>
    class IdGroup : IdGroupFacade
    {
        #region Class data

        /// <summary>
        /// Any ID packets allocated for this group.
        /// </summary>
        readonly List<IdPacket> m_Packets;
        
        /// <summary>
        /// Any IDs that have been reserved (but not yet allocated).
        /// </summary>
        readonly List<uint> m_ReservedIds;        

        #endregion

        #region Constructors

        internal IdGroup (IIdGroup data) : base(data)
        {
            m_Packets = new List<IdPacket>();
            m_ReservedIds = new List<uint>();
        }

        #endregion

        /// <summary>
        /// Any ID packets allocated for this group.
        /// </summary>
        internal IdPacket[] IdPackets
        {
            get { return m_Packets.ToArray(); }
        }

        /// <summary>
        /// Determines whether this group is the "owner" of an ID range. It owns the
        /// range if the group's range of IDs entirely encloses it.
        /// </summary>
        /// <param name="range">The ID range to check.</param>
        /// <returns>True if this group owns the specified ID range.</returns>
        /*
        internal bool IsOwnerOf(IdRange range)
        {
	        return (range.Min >= this.LowestId && range.Max <= this.HighestId);
        }
        */

        /// <summary>
        /// Tries to associate an ID range with this group. This will only succeed if
        /// the group "owns" the range.
        /// </summary>
        /// <param name="range">The ID range to add.</param>
        /// <returns>True if this group has taken possession of the range.</returns>
        /*
        internal bool AddIdRange(IdRange range)
        {
            if (this.IsOwnerOf(range))
            {
                m_IdRanges.Add(range);
                return true;
            }

            return false;
        }
        */

        /// <summary>
        /// Gets a new allocation for this ID group.
        /// </summary>
        /// <param name="announce">Should the allocation be announced to the user?</param>
        /// <returns>The number of ranges that were added.</returns>
        internal uint GetAllocation(bool announce)
        {
            string announcement = "Allocating extra IDs:" + System.Environment.NewLine + System.Environment.NewLine;

            // The new ranges will go in the current map.
            //List<IdRange> ranges = CadastralMapModel.Current.IdRanges;

        	// Grab a hold of info we'll need to stick into the IdAllocation table.
            Job curjob = EditingController.Current.Job;
            User curuser = Session.CurrentSession.User;
            DateTime curtime = DateTime.Now;

	        int ntoget = PacketSize;   // How many do we need to allocate?
	        int minid=0;               // The low end of a new range.
	        int maxid=0;               // The high end of a new range.
	        uint nadd=0;               // The number of ranges added.

            Transaction.Execute(delegate
            {
                // Create ranges based on what's in the free list. We keep creating until we've got
                // the packet size demanded by this group. To keep things simple, just grab all rows
                // in the IdFree table for this group.
                IdFree[] freeTab = IdFree.FindByGroupId(this.Id);

                // If we didn't find ANYTHING, initialize the database with the complete
                // range of this group.
                if (freeTab.Length==0)
                {
                    IdFree row = IdFree.Insert(this);
                    freeTab = new IdFree[] { row };
                }

                foreach (IdFree row in freeTab)
                {
                    if (ntoget<=0)
                        break;

                    // How many IDs are free in the current range?
                    int nfree = (row.HighestId - row.LowestId + 1);

                    // If it's got MORE than enough, grab the portion we need, and that's us done
                    if (nfree > ntoget)
                    {
                        minid = row.LowestId;
                        maxid = row.LowestId + ntoget - 1;
                        row.LowestId = maxid+1;
                        ntoget = 0;
                    }
                    else
                    {
                        // Grab the complete range and delete the row.
                        minid = row.LowestId;
                        maxid = row.HighestId;
                        row.Delete();
                        ntoget -= nfree;
                    }

                    // If the packet we found is contiguous with an existing
                    // packet, append to that packet. Otherwise add a new packet.

                    bool isExtension = false;
                    for (int i=0; i<m_Packets.Count && !isExtension; i++)
                    {
                        IdPacket oldPacket = m_Packets[i];
                        isExtension = oldPacket.Extend(minid, maxid);
                    }

                    if (!isExtension)
                    {
                        // Create a new range object and append it to this group and to the map.
                        //IdRange range = new IdRange((uint)minid, (uint)maxid, HasCheckDigit, KeyFormat);
                        //m_IdRanges.Add(range);
                        //ranges.Add(range);

                        // Insert a row into the IdAllocation table.
                        IdAllocation ida = IdAllocation.Insert(this, minid, maxid, curjob, curuser, curtime, 0);

                        // Remember it as part of this group
                        AddIdPacket(ida);
                    }

                    // Increment the number of ranges we've added (extensions
                    // count as additions too).
                    nadd++;

                    // Append to any announcement
                    if (announce)
                        announcement += String.Format("{0}-{1}{2}", minid, maxid, System.Environment.NewLine);
                }
            });

            // If the user should be informed, list out any ranges we created.
            if (announce)
                MessageBox.Show(announcement);

            // Did we get everything we needed?
            if (ntoget>0)
                MessageBox.Show("Could not obtain a complete allocation.");

	        return nadd;
        }

        /// <summary>
        /// Releases unused portions of allocations that belong to this group (actually,
        /// it just trims the allocation).
        /// </summary>
        /*
        internal void ReleaseAll()
        {
            List<IdRange> result = new List<IdRange>(m_IdRanges.Count);

            foreach(IdRange range in m_IdRanges)
            {
                // Get the range to release. If that doesn't end up releasing everything,
                // remember that the range is still relevant.

                int nLeft = range.Release(this);
                if (nLeft>0)
                    result.Add(range);
            }

            // Hold on to the modified range list if at least one range was
            // entirely released.
            if (result.Count != m_IdRanges.Count)
                m_IdRanges = result;

	        // Any reserved IDs are no good any more.
	        m_ReservedIds.Clear();
        }
        */

        /// <summary>
        /// Releases the unused portions of a specific ID range that belongs to this group.
        /// </summary>
        /// <param name="minid">The lowest ID in the range.</param>
        /// <param name="maxid">The highest ID in the range.</param>
        /// <returns>True if the range was found and successfully released (it may have
        /// already been released). False if the range could not be found (an error message
        /// is displayed), or the release failed for some reason.</returns>
        /*
        internal bool Release(uint minid, uint maxid)
        {
	        // Try to find the range to release.
            IdRange range = FindRange(minid, maxid);
	        if (range==null)
            {
                string errmsg = String.Format("Cannot find ID range {0}-{1}", minid, maxid);
                MessageBox.Show(errmsg);
                return false;
            }

	        // Ensure that any reserved IDs in the specified range have been discarded.
            m_ReservedIds.RemoveAll(delegate(uint id) { return (id>=minid && id<=maxid); });

	        // Release the range.
	        int nleft = range.Release(this);

	        // If that leaves us with NOTHING, the range has already
	        // removed itself from the map's list. Remove it from our own list too.
        	if (nleft==0)
		        CutRange(range);

        	return (nleft>=0);
        }
        */

        /// <summary>
        /// Returns the ID range object that has a specific range.
        /// </summary>
        /// <param name="minid">The lowest ID in the range.</param>
        /// <param name="maxid">The highest ID in the range.</param>
        /// <returns>The ID range, or null if not found.</returns>
        /*
        IdRange FindRange(uint minid, uint maxid)
        {
            return m_IdRanges.Find(delegate(IdRange range) { return (range.Min==minid && range.Max==maxid); });
        }
        */

        /// <summary>
        /// Returns the ID range object (belonging to this group) that encloses a specific ID.
        /// </summary>
        /// <param name="id">The ID to find.</param>
        /// <returns>The ID range, or null if not found.</returns>
        /*
        IdRange FindRange(uint id)
        {
            return m_IdRanges.Find(delegate(IdRange range) { return (range.Min<=id && range.Max>=id); });
        }
        */

        /// <summary>
        /// Returns the ID packet (belonging to this group) that encloses a specific ID.
        /// </summary>
        /// <param name="id">The ID to find.</param>
        /// <returns>The ID packet, or null if not found.</returns>
        IdPacket FindPacket(uint id)
        {
            return m_Packets.Find(delegate(IdPacket p) { return (p.Min <= id && p.Max >= id); });
        }

        /// <summary>
        /// Cuts a specific ID range from this group. This function is called by IdGroup.Release in
        /// situations where there was nothing left in the range.
        /// </summary>
        /// <param name="range">The range to cut.</param>
        /// <returns>True if range was cut. False if it could not be found (considered to be an
        /// error, leading to a message).</returns>
        /*
        bool CutRange(IdRange range)
        {
            bool isRemoved = m_IdRanges.Remove(range);
            if (!isRemoved)
                MessageBox.Show("IdGroup.CutRange - Could not find ID range.");
            return isRemoved;
        }
        */

        /// <summary>
        /// Arbitrarily removes all ID ranges that have been associated with this group.
        /// IdManager calls this function whenever a new map is opened for editing.
        /// </summary>
        /*
        internal void KillRanges()
        {
	        m_IdRanges.Clear();
	        m_ReservedIds.Clear();
        }
        */

        /// <summary>
        /// Reserves an ID belonging to this ID group.
        /// </summary>
        /// <param name="idh">The ID handle to define.</param>
        /// <param name="id">The specific ID to reserve. Specify 0 for the next
        /// available ID (in that case, an additional allocation will be made if
        /// necessary).</param>
        /// <returns>True if the ID was reserved ok. False if it is already reserved (or
        /// an allocation could not be obtained).</returns>
        internal bool ReserveId(IdHandle idh, uint id)
        {
	        if (id>0)
            {
        		// Find the packet that contains the specified ID.
                IdPacket packet = FindPacket(id);
                if (packet == null)
                {
                    MessageBox.Show("IdGroup.ReserveId - Wrong ID group.");
			        return false;
		        }

		        // Get the ID handle to reserve the ID.
                return idh.ReserveId(packet, idh.Entity, id);
	        }
	        else
            {
        		// Find the packet that contains the next available ID.
                IdPacket packet = FindNextAvail();

		        // If we didn't find anything, ask the ID manager to make
		        // a new allocation (most of the work actually gets passed
		        // back to IdGroup.GetAllocation).
                if (packet == null)
                {
                    IdManager man = CadastralMapModel.Current.IdManager;
			        if (man.GetAllocation(this, true)==0)
                        return false;
                    packet = FindNextAvail();
                    if (packet == null)
                        return false;
		        }

                // Get the next ID from the packet.
                uint nextid = packet.ReserveId();
		        if (nextid==0)
                {
                    MessageBox.Show("IdGroup.ReserveId - Range did not have any free IDs.");
			        return false;
		        }

                return idh.Define(packet, idh.Entity, nextid);
	        }
        }

        /// <summary>
        /// Reserves a specific ID in this ID group. This function is called by IdRange.ReserveId,
        /// which makes an initial check to confirm that the ID is available as far as it is concerned.
        /// </summary>
        /// <param name="id">The ID to reserve.</param>
        /// <returns>True if the ID was reserved ok. False if it is already reserved.</returns>
        internal bool ReserveId(uint id)
        {
	        // Return if the specified ID is already reserved.
	        if (IsReserved(id))
            {
		        MessageBox.Show("IdGroup.ReserveId = ID previously reserved");
		        return false;
	        }

	        // Remember the specified ID in the reserve list.
	        m_ReservedIds.Add(id);
	        return true;
        }

        /// <summary>
        /// Finds the first ID packet that contains the next available ID.
        /// </summary>
        /// <returns>The ID packet (if any) that contains the next available ID.</returns>
        IdPacket FindNextAvail()
        {
            // We assume that packets are ordered so that the earliest
            // packets come first. We COULD scan from the end of the list,
	        // but that would complicate it a bit, seeing how we'd have
            // to go back to the first packet that does NOT have an
            // available ID. In any case, the chain of ID packet shouldn't
	        // be excessively long.

            return m_Packets.Find(delegate(IdPacket p) { return p.HasAvail(); });
        }

        /// <summary>
        /// Frees an ID that was previously reserved.
        /// </summary>
        /// <param name="id">The ID to free.</param>
        /// <returns>True if the ID was freed. False if it could not be found.</returns>
        internal bool FreeId(uint id)
        {
        	int index = FindReservedId(id);
	        if (index >= 0)
            {
                m_ReservedIds.RemoveAt(index);
        		return true;
	        }

	        return false;
        }

        /// <summary>
        /// Checks whether a specific ID has been reserved.
        /// </summary>
        /// <param name="id">The ID to check.</param>
        /// <returns>True if the ID is reserved.</returns>
        public bool IsReserved(uint id)
        {
            return (FindReservedId(id)>=0);
        }

        /// <summary>
        /// Tries to get the index of an ID that may have been reserved.
        /// </summary>
        /// <param name="id">The ID to find.</param>
        /// <returns>List index of the reserved ID (refers to m_ReservedIds). A value less than
        /// zero means the ID is not reserved.</returns>
        int FindReservedId(uint id)
        {
            return m_ReservedIds.FindIndex(delegate(uint r) { return r==id; });
        }

        /// <summary>
        /// Creates a new feature ID. This function is called by IdHandle.CreateId. The supplied
        /// ID and range should have been defined via a prior call to ReserveId.
        /// </summary>
        /// <param name="id">The ID key to use. Must be listed in this group as a reserved ID
        /// (if not, an error message will be issued).</param>
        /// <param name="packet">The ID packet that the ID belongs to.</param>
        /// <param name="feature">The feature which the new ID is to be assigned to.</param>
        /// <returns>The created feature ID (null on error).</returns>
        internal FeatureId CreateId(uint id, IdPacket packet, Feature feature)
        {
	        // Confirm that the specified ID was previously reserved (if not,
	        // see if it has already been created).
        	int index = FindReservedId(id);
	        if (index < 0)
            {
                string msg = String.Format("ID {0} d was not properly reserved.", id);
                MessageBox.Show(msg);
                return null;
            }

        	// Get the range to create the ID.
            FeatureId fid = packet.CreateId(id, feature);

	        // If we successfully created the ID, clear the reserve status.
	        if (fid!=null)
                m_ReservedIds.RemoveAt(index);

	        return fid;
        }

        /// <summary>
        /// Loads a list with all the IDs that are available for this group.
        /// </summary>
        /// <param name="avail">The list to load.</param>
        /// <returns>The number of IDs that were added to the list.</returns>
        internal uint GetAvailIds(List<uint> avail)
        {
	        // Loop through the ID packets that have already been allocated,
	        // adding free IDs into the array.

        	uint nid=0;
            foreach(IdPacket packet in m_Packets)
                nid += packet.GetAvailIds(avail);

        	return nid;
        }

        /// <summary>
        /// Counts the number of reserved IDs within a specific range.
        /// </summary>
        /// <param name="minid">The low end of the range.</param>
        /// <param name="maxid">The high end of the range.</param>
        /// <returns>The number of reserved IDs in the specified range.</returns>
        public uint GetReserveCount(uint minid, uint maxid)
        {
        	uint nres=0;

            foreach(uint id in m_ReservedIds)
            {
        		if (minid<=id && id<=maxid)
                    nres++;
	        }

	        return nres;
        }

        /// <summary>
        /// Loads a list with the reserved IDs that fall within a specific range.
        /// </summary>
        /// <param name="minid">The low end of the range.</param>
        /// <param name="maxid">The high end of the range.</param>
        /// <param name="reserves">The list to load. May be allocated with an initial
        /// capacity as returned by a call to GetReserveCount (using the same range).</param>
        /// <returns>The number of reserved IDs added to the list.</returns>
        public uint GetReserveIds(uint minid, uint maxid, List<uint> reserves)
        {
        	uint nres=0;

            foreach(uint id in m_ReservedIds)
            {
                if (minid<=id && id<=maxid)
                {
                    reserves.Add(id);
                    nres++;
                }
            }

        	return nres;
        }

        /// <summary>
        /// Returns the ID range object that refers to a specific feature ID.
        /// </summary>
        /// <param name="fid">The feature ID to find.</param>
        /// <returns>The ID range, or null if not found.</returns>
        /*
        internal IdRange FindRange(FeatureId fid)
        {
	        // If the feature ID is null, it doesn't belong to anything.
	        if (fid==null)
                return null;

            return m_IdRanges.Find(delegate(IdRange range) { return range.IsReferredTo(fid); });
        }
         */

        /// <summary>
        /// Inserts an additional ID range into this group.
        /// </summary>
        /// <param name="after">The range to insert after.</param>
        /// <param name="insert">The range to insert.</param>
        /// <returns>True if inserted ok.</returns>
        /*
        internal bool InsertAfter(IdRange after, IdRange insert)
        {
        	// Find the range we're supposed to insert after.
            int index = m_IdRanges.IndexOf(after);
            if (index<0)
            {
		        MessageBox.Show("IdGroup.InsertAfter - Cannot find ID range");
                return false;
            }

	        // Do the insert.
            m_IdRanges.Insert(index+1, insert);
            return true;
        }
        */

        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// Associates an ID allocation with this group
        /// </summary>
        /// <param name="a">The allocation associated with this group</param>
        internal void AddIdPacket(IdAllocation a)
        {
            m_Packets.Add(new IdPacket(this, a));
        }

        /// <summary>
        /// Exhaustive search for the ID packet that refers to a specific ID. This method
        /// should only be called in situations where something has gone astray.
        /// </summary>
        /// <param name="fid">The ID to search for</param>
        /// <returns>The packet that contains the specified object (null if not found)</returns>
        internal IdPacket FindPacket(FeatureId fid)
        {
            foreach (IdPacket p in m_Packets)
            {
                if (p.IsReferredTo(fid))
                    return p;                
            }

            return null;
        }

        /// <summary>
        /// Formats an ID number the way this ID group likes to see it.
        /// </summary>
        /// <param name="id">The raw ID to format (may not actually lie within the
        /// limits of this packet).</param>
        /// <returns>The formatted result.</returns>
        internal string FormatId(uint id)
        {
            // Apply the format assuming no check digit.
            string key = String.Format(KeyFormat, id);

            // If there really is a check digit, work it out and append it.
            if (HasCheckDigit)
            {
                uint cd = Key.GetCheckDigit(id);
                key += cd.ToString();
            }

            return key;
        }

        /// <summary>
        /// Tries to produce a numeric key.
        /// </summary>
        /// <param name="id">The raw ID to use (without any check digit).</param>
        /// <returns>The numeric key value (0 if a numeric key cannot be generated).</returns>
        internal uint GetNumericKey(uint id)
        {
            // If a check digit is required, and the raw ID exceeds 200
            // million, it means that the numeric key wouldn't fit in
            // a 32-bit value.
            if (HasCheckDigit && id > 200000000)
                return 0;

            // If the key format is not completely numeric, we can't do it.
            if (!IsNumericKey())
                return 0;

            // If a check digit is not required, the supplied raw ID is
            // the numeric key we want to store.
            if (!HasCheckDigit)
                return id;

            // Work out the check digit and append it to the raw ID.
            uint checkdig = Key.GetCheckDigit(id);
            return (id*10 + checkdig);
        }

        /// <summary>
        /// Checks if this ID group has numeric keys. This means that if the raw ID (without
        /// any check digit) is formatted, does it produce a right-justified string that
        /// contains just numeric digits.
        /// <para/>
        /// Note that a numeric key may actually need to be stored as a string if a check
        /// digit causes the resulting number to exceed 2 billion.
        /// </summary>
        /// <returns>True if the keys are numeric.</returns>
        bool IsNumericKey()
        {
            // Use the key format string to parse the value "1"
            string str = String.Format(KeyFormat, 1);

            // If we got just 1 character, it's numeric.
            return (str.Length==1);
        }
    }
}
