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
using System.Windows.Forms;

using Backsight.Data;
using Backsight.Environment;

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
        /// The highest ID allocated to this group.
        /// </summary>
        int m_MaxAllocatedId;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IdGroup"/> class.
        /// </summary>
        /// <param name="data">The environment definition for the ID group.</param>
        internal IdGroup (IIdGroup data)
            : base(data)
        {
            m_Packets = new List<IdPacket>();
            m_MaxAllocatedId = data.MaxUsedId;
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
        /// Gets a new allocation for this ID group.
        /// </summary>
        /// <param name="announce">Should the allocation be announced to the user?</param>
        /// <returns>Information about the allocated range.</returns>
        internal IdPacket GetAllocation(bool announce)
        {
            IdPacket result = null;

            IDataServer ds = EditingController.Current.DataServer;
            if (ds == null)
                throw new ApplicationException("Database not available");

            ds.RunTransaction(delegate
            {
                // May be best to remove this from IIdGroup! -- DO want to be able to retrieve the value
                // currently stored in the database.
                int oldMaxUsedId = m_MaxAllocatedId;
                int newMaxUsedId = (oldMaxUsedId == 0 ? LowestId + PacketSize - 1 : oldMaxUsedId + PacketSize);

                /*
                // The following should be covered by the implementation of the ID server (for a personal ID server,
                // there should be nothing to do).
                string sql = String.Format("UPDATE [ced].[IdGroups] SET [MaxUsedId]={0} WHERE [GroupId]={1} AND [MaxUsedId]={2}",
                                                newMaxUsedId, Id, oldMaxUsedId);
                int nRows = ds.ExecuteNonQuery(sql);

                if (nRows != 1)
                    throw new ApplicationException("Allocation failed");
                */

                // Remember the allocation as as part of this group
                IdAllocation alloc = new IdAllocation()
                {
                    GroupId = this.Id,
                    LowestId = newMaxUsedId - PacketSize + 1,
                    HighestId = newMaxUsedId,
                };

                result = AddIdPacket(alloc);
                m_MaxAllocatedId = newMaxUsedId;

                // Write event data for the allocation
                EditingController.Current.Project.WriteChange(alloc);
            });

            // If the user should be informed, list out any ranges we created.
            if (announce && result != null)
            {
                string announcement = String.Format("Allocating extra IDs: {0}-{1}", result.Min, result.Max);
                MessageBox.Show(announcement);
            }

            return result;
        }

        /// <summary>
        /// Returns the ID packet (belonging to this group) that encloses a specific ID.
        /// </summary>
        /// <param name="id">The ID to find.</param>
        /// <returns>The ID packet, or null if not found.</returns>
        internal IdPacket FindPacket(uint id)
        {
            return m_Packets.Find(p => p.Min <= id && p.Max >= id);
            //return m_Packets.Find(delegate(IdPacket p) { return (p.Min <= id && p.Max >= id); });
        }

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
			        GetAllocation(true);
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

            return m_Packets.Find(p => p.HasAvail());
        }

        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// Associates an ID allocation with this group
        /// </summary>
        /// <param name="a">The allocation associated with this group</param>
        /// <returns>The ID packet corresponding to the supplied allocation</returns>
        internal IdPacket AddIdPacket(IdAllocation a)
        {
            IdPacket result = new IdPacket(this, a);
            m_Packets.Add(result);
            m_MaxAllocatedId = Math.Max(m_MaxAllocatedId, a.HighestId);
            return result;
        }

        /// <summary>
        /// Exhaustive search for the ID packet that refers to a specific ID.
        /// </summary>
        /// <param name="fid">The ID to search for</param>
        /// <returns>The packet that contains the specified object (null if not found)</returns>
        internal IdPacket FindPacket(NativeId fid)
        {
            uint rawId = fid.RawId;

            foreach (IdPacket p in m_Packets)
            {
                if (p.Contains(rawId))
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
                uint cd = NativeId.GetCheckDigit(id);
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
            uint checkdig = NativeId.GetCheckDigit(id);
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


        /// <summary>
        /// Loads a list with all the IDs that are available for this ID group.
        /// </summary>
        /// <returns>The raw IDs in this group that are available (may be an empty array).</returns>
        internal uint[] GetAvailIds()
        {
            List<uint> result = new List<uint>();

            foreach (IdPacket p in m_Packets)
                p.GetAvailIds(result);

            return result.ToArray();
        }

        /// <summary>
        /// Discards any IDs that may have been reserved (but which are no longer needed). This
        /// should be called in situations where a use cancels from a data entry dialog.
        /// </summary>
        internal void FreeAllReservedIds()
        {
            foreach (IdPacket p in m_Packets)
                p.FreeAllReservedIds();
        }
    }
}
