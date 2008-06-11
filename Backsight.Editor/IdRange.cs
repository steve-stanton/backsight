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
using Backsight.Editor.Database;

namespace Backsight.Editor
{
	/// <written by="Steve Stanton" on="14-DEC-1998" />
    /// <summary>
    /// Persistent class that records a range of allocated IDs. The <c>CadastralMapModel</c> class
    /// maintains a list of references to all the <c>IdRange</c> objects in a map.
    /// </summary>
    public class IdRange
    {
        #region Class data

        // m_LowestId and m_HighestId are assigned at the time a range is
        // allocated, and may be trimmed when the range is released.

        // The lowest ID is the primary key into the IdAllocation table.

        /// <summary>
        /// The lowest ID in the range.
        /// </summary>
        uint m_LowestId;

        /// <summary>
        /// The highest ID in the range.
        /// </summary>
        uint m_HighestId;

        // The allocated size of the pointer array.
        // If the range has been released, this may
        // be LESS than the value returned by GetSize().
        //UINT4 m_NumAlloc;

        /// <summary>
        /// The number of IDs that have been used (not necessarily sequential in the array). 
        /// Corresponds to the number of non-null references in the m_Ids array.
        /// </summary>
        uint m_NumUsed;

        /// <summary>
        /// The number of IDs that are currently free. This is the total number of null
        /// references in m_Ids PLUS the number of references to inactive IDs.
        /// </summary>
        uint m_NumFree;

        /// <summary>
        /// References to allocated IDs. The  first element corresponds to m_LowestId,
        /// while the last corresponds to m_HighestId. When created, all ID references
        /// will be null. They get defined only when a user makes use of ID. When that
        /// happens, the reference is defined at the appropriate place in the array,
        /// m_NumUsed is incremented, and m_NumFree is decremented.
        /// </summary>
        FeatureId[] m_Ids;

        /// <summary>
        /// True if any unused portion of the allocation has been returned to the
        /// central database. When this is done, the m_Ids array will be squashed
        /// up to eliminate any null references.
        /// </summary>
        bool m_IsReleased;

        /// <summary>
        /// True if a check digit should be appended to keys produced for IDs in this range.
        /// </summary>
        bool m_IsCheckDigit;

        /// <summary>
        /// Key-format. This should always be defined to be a non-null value, even
        /// if it's just the default "{0}" format.
        /// </summary>
        string m_KeyFormat;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        IdRange()
        {
            SetNoRange();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="lowest">The low end of the range (inclusive).</param>
        /// <param name="highest">The high end of the range (inclusive).</param>
        /// <param name="isCheckDigit">Should a check digit be appended to IDs in the range?</param>
        /// <param name="keyFormat">Any specialized key format.</param>
        internal IdRange(uint lowest, uint highest, bool isCheckDigit, string keyFormat)
        {
            // Confirm highest really is higher!
            if (highest < lowest)
                throw new ArgumentException("IdRange - Inverted range");

            m_LowestId = lowest;
            m_HighestId = highest;
            m_NumUsed = 0;
            m_NumFree = this.Size;
            m_Ids = new FeatureId[m_NumFree]; // all null references
            m_IsReleased = false;
            m_IsCheckDigit = isCheckDigit;
            m_KeyFormat = keyFormat.Trim();

            // If we didn't get a key format, use the default.
            if (m_KeyFormat.Length==0)
                m_KeyFormat = "{0}";
        }

        #endregion

        /// <summary>
        /// Sets everything to null values. Should be called only by constructors,
        /// because it does not check if the object already refers to stuff.
        /// </summary>
        void SetNoRange()
        {
            m_LowestId = 0;
            m_HighestId = 0;
            m_NumUsed = 0;
            m_NumFree = 0;
            m_Ids = null;
            m_IsReleased = false;
            m_IsCheckDigit = false;
            m_KeyFormat = String.Empty;
        }

        public uint Min
        {
            get { return m_LowestId; }
        }

        public uint Max
        {
            get { return m_HighestId; }
        }

        internal bool IsReleased
        {
            get { return m_IsReleased; }
        }

	    internal uint NumUsed
        {
            get { return m_NumUsed; }
        }

	    uint NumFree
        {
            get { return m_NumFree; }
        }

	    string KeyFormat
        {
            get { return m_KeyFormat; }
        }

        bool IsCheckDigit
        {
            get { return m_IsCheckDigit; }
        }

        internal uint Size
        {
            get { return (m_HighestId-m_LowestId+1); }
        }

        /// <summary>
        /// Determines whether this range is enclosed by the range of an ID group.
        /// </summary>
        /// <param name="group">The ID group to check.</param>
        /// <returns>True if the group's range encloses this range.</returns>
        bool IsEnclosedBy(IdGroup group)
        {
            return (group.LowestId<=m_LowestId && group.HighestId>=m_HighestId);
        }

        /// <summary>
        /// Releases any unused portion of this ID range.
        /// </summary>
        /// <param name="group">The group that the range is part of (may be updated if
        /// the unused portion is not contiguous).</param>
        /// <returns>The number of ID left in the range. A value less than zero means
        /// an error occurred.</returns>
        internal int Release(IdGroup group)
        {
	        // Return if this range has already been released.
	        if (m_IsReleased)
                return (int)m_NumUsed;

	        // If NOTHING in the range has ever been used (i.e. ALL ID
	        // pointers are null)
	        if (m_NumUsed==0)
            {
                int nrows = 0;

                Transaction.Execute(delegate
                {
                    // Delete the row from the IdAllocation table.
                    nrows = IdAllocation.Delete((int)m_LowestId);

                    // If we didn't delete anything, perhaps the row was
                    // successfully deleted on a previous attempt, so plough on
                    // (only exit if an error occurred).
                    if (nrows<0)
                        return;

                    // Insert a new row into the IdFree table.
                    IdFree.Insert(group, (int)m_LowestId, (int)m_HighestId);
                    nrows = 1;
                });

                if (nrows<0)
                    return -1;

                // Remove the IdRange object from the map.
                List<IdRange> ranges = CadastralMapModel.Current.IdRanges;
                if (!ranges.Remove(this))
                    throw new Exception("Could not find deleted ID range in the map.");

		        // Mark the range as released.
		        m_IsReleased = true;
	        }
	        else
            {
		        // Insert free ranges into the IdFree table.
		        if (InsertFreeIds(group)<0)
                    return -1;

		        // Handle the used range(s). This will update the IdAllocation
		        // table (and may insert additional rows if the used IDs are
		        // not contiguous).
		        if (InsertUsedIds(group)<0)
                    return -1;
	        }

	        // Return the number of used IDs we have left (if any).
	        return (int)m_NumUsed;
        }

        /// <summary>
        /// Gets the range of IDs that have actually been used at any point in time
        /// (including IDs that may currently be inactive).
        /// </summary>
        /// <param name="minid">The lowest used ID (0 if nothing was used).</param>
        /// <param name="maxid">The highest used ID (0 if nothing was used).</param>
        void GetMinMax(out uint minid, out uint maxid)
        {
	        minid = maxid = 0;

	        // Return if all references are null.
	        if (m_NumUsed==0)
                return;

	        // If the range has already been released, or the number of
	        // non-zero pointers equals the complete size of the range,
	        // just return the min-max of the range.

	        if (m_IsReleased || m_NumUsed==this.Size)
            {
		        minid = m_LowestId;
		        maxid = m_HighestId;
		        return;
	        }

	        // Scan the array to find the smallest and largest IDs that are not null.

            for (uint i=0; i<m_Ids.Length; i++)
            {
                if (m_Ids[i]!=null)
                {
                    if (minid==0)
                        minid = m_LowestId+i;

                    maxid = m_LowestId+i;
                }
            }
		}

        /// <summary>
        /// Inserts any free ID ranges into the database.
        /// </summary>
        /// <param name="group">The ID group associated with this range.</param>
        /// <returns>The number of ranges that were inserted (-1 on error).</returns>
        int InsertFreeIds(IdGroup group)
        {
	        int nfree=0;        // The number of inserts made

            Transaction.Execute(delegate
            {
                uint start = 0;       // Index of the start of free range
                uint end = 0;         // Index of the end of free range

                for (; GetNextFree(ref start, ref end); start = end + 1)
                {
                    uint minid = m_LowestId + start;
                    uint maxid = m_LowestId + end;
                    IdFree.Insert(group, (int)minid, (int)maxid);
                    nfree++;
                }
            });

	        return nfree;
        }

        /// <summary>
        /// Gets the next free range (if any). This function is used by <c>InsertFreeIds</c>
        /// when it needs to obtain any IDs to return to the database.
        /// 
        /// In this context, only those ID slots which are null are considered to be "free".
        /// Any slots that refer to inactive IDs are not free, since they may be needed
        /// if the user decides to rollback a user-perceived deletion.
        /// </summary>
        /// <param name="start">The index of the first ID to check. Updated to refer
        /// to the start of the next free range (if any).</param>
        /// <param name="end">The index of the end of the next free range (if any).</param>
        /// <returns>True if a free range was found. False if no additional free ranges were
        /// found (in that case, the start and end index values are invalid).</returns>
        bool GetNextFree(ref uint start, ref uint end)
        {
        	bool isFound=false;	// Set to true as soon as a free slot is found.

	        for (uint i=start; i<m_Ids.Length; i++)
            {
		        // If the current ID is used, but we have a defined
		        // free range, return what we previously found.
		        if (m_Ids[i]!=null && isFound)
                    return true;

		        // If the current ID is not used, see whether we have
		        // started a free range or not. If we have, just update
		        // the end of the free range.

		        if (m_Ids[i]==null)
                {
			        if (!isFound)
                    {
				        start = i;
				        isFound = true;
			        }
			        end = i;
		        }
	        }

	        // If we didn't find ANY free slots, define invalid index values as the free range.
	        if (!isFound)
                start = end = (uint)m_Ids.Length;

        	return isFound;
        }

        /// <summary>
        /// Obtains a string that you should be able to use when inserting into the database.
        /// This currently does the same as the old CeTime::Format method, which may not be
        /// compatible with the db! This needs to be re-visited (the fact that the DateTime
        /// needs to be converted into a string is the issue -- the software using this method
        /// should probably be using SQL parameters).
        /// </summary>
        string DatabaseTimeString(DateTime time)
        {
            return time.ToString("dd-MMM-yyyy HH:mm:ss");
        }

        /// <summary>
        /// Inserts any used ID ranges into the database. The used ranges may not be contiguous.
        /// </summary>
        /// <param name="group">The ID group.</param>
        /// <returns>The number of used ranges that were found (-1 on error).</returns>
        int InsertUsedIds(IdGroup group)
        {
	        string sql;         // SQL statement
	        int nused=0;        // The number of used ranges found.
	        uint totused=0;     // The total number of used IDs found.
	        uint start=0;       // Index of the start of used range.
	        uint end=0;         // Index of the end of used range.
	        int nrows;          // Row count.
	        uint firstmin=0;    // The lowest ID of the 1st used range.
	        uint firstmax=0;    // The highest ID of the 1st used range.
	        IdRange last=this;  // The last range inserted into the group.
	        uint i;             // Loop counter

	        // Get info we'll need if we have to insert extra ranges ...
            CadastralMapModel map = CadastralMapModel.Current;
            List<IdRange> mapRanges = map.IdRanges;

            Job curjob = EditingController.Current.Job;
            User curuser = Session.CurrentSession.User;
            DateTime curtime = DateTime.Now;    // The current time
            string fname = Path.GetFileNameWithoutExtension(map.Name);

        	// While more used ranges
	        for (; GetNextUsed(ref start, ref end); start=end+1)
            {
		        // Get the ID range.
		        uint minid = m_LowestId + start;
		        uint maxid = m_LowestId + end;

		        // Increment the number of used ranges, and update the
		        // total number of used IDs.
		        nused++;
		        totused += (maxid-minid+1);

		        // If this is the first used range, just remember the range
		        // for now. Otherwise create an additional ID range, insert
		        // it into the database, tell the group and the map.

		        if (nused==1)
                {
			        firstmin = minid;
			        firstmax = maxid;
		        }
		        else
                {
			        // Create a new range.
                    IdRange newRange = new IdRange(minid, maxid, m_IsCheckDigit, m_KeyFormat);

			        // And make it look released.
			        newRange.m_NumUsed = (maxid-minid+1);
			        newRange.m_NumFree = 0;
			        newRange.m_IsReleased = true;

			        // Copy over the ID pointers from this range.
			        for (i=0; i<m_NumUsed; i++)
				        newRange.m_Ids[i] = m_Ids[start+i];

        			// Insert the new range into the database.
                    IdAllocation.Insert(group, (int)minid, (int)maxid, curjob, curuser, curtime,
                                          (int)newRange.m_NumUsed);

                    // Insert into the group's list of ID ranges.
			        newRange.InsertAfter(last, group);

        			// Insert into the map's list of ID ranges.
                    int pos = mapRanges.IndexOf(last);
                    if (pos<0)
                    {
                        MessageBox.Show("IdRange.Release - Could not find map's ID range.");
                        return -1;
                    }
                    mapRanges.Insert(pos+1, newRange);

			        // Remember the new range as the range we last inserted.
			        last = newRange;
		        }
	        }

	        Debug.Assert(firstmin>0);
	        Debug.Assert(totused==m_NumUsed);

	        // Issue message in non-debug runs as well, as the error may not be immediately obvious.
	        if (firstmin==0 || totused!=m_NumUsed)
            {
		        MessageBox.Show("IdRange.InsertUsedIds - Check count failed");
		        return -1;
	        }

	        // Allocate a new array for the IDs in the first used group.
	        m_NumUsed = (firstmax-firstmin+1);
            FeatureId[] newIds = new FeatureId[m_NumUsed];

        	// Get the index of the first ID to copy.
	        start = (firstmin-m_LowestId);

	        // Copy over the ID references.
	        for (i=0; i<m_NumUsed; i++)
            {
		        newIds[i] = m_Ids[start+i];
                Debug.Assert(newIds[i]!=null);
            }

        	// Update the IdAllocation table to refer to the first used range.
            sql = String.Format("update IdAllocation set LowestId={0}, HighestId={1}, NumUsed={2} where LowestId={3}"
                                    , firstmin, firstmax, m_NumUsed, m_LowestId);
            SqlCommand cmd = new SqlCommand(sql, AdapterFactory.GetConnection().Value);
            nrows = cmd.ExecuteNonQuery();
	        if (nrows!=1)
                return -1;

	        // Replace the original array of references with those that relate to the first used group.
            m_Ids = newIds;

        	// Record the new min/max for the initial used group.
	        m_LowestId = firstmin;
	        m_HighestId = firstmax;

	        // The range has been released.
	        m_NumFree = 0;
	        m_IsReleased = true;

	        return nused;
        }

        /// <summary>
        /// Gets the next range of used IDs (if any). This means ID slots that point
        /// to a feature ID object.
        /// </summary>
        /// <param name="start">The index of the first ID to check. Updated to refer
        /// to the start of the next used range (if any).</param>
        /// <param name="end">The index of the end of the next used range (if any).</param>
        /// <returns>True if a used range was found. False if no additional used ranges were
        /// found (in that case, the start and end index values are invalid).</returns>
        bool GetNextUsed(ref uint start, ref uint end)
        {
        	bool isFound = false;	// Set to true as soon as a used slot is found.

	        for (uint i=start; i<m_Ids.Length; i++ )
            {
		        // If the current ID is un-used, but we have a used range, return what we previously found.
		        if (m_Ids[i]==null && isFound)
                    return true;

		        // If the current ID is used, see whether we have started a used range or not. If we
                // have, just update the end of the used range.

		        if (m_Ids[i]!=null)
                {
			        if (!isFound)
                    {
				        start = i;
				        isFound = true;
			        }
			        end = i;
		        }
        	}

	        // If we didn't find ANY used IDs, define invalid index values as the used range.
	        if (!isFound)
		        start = end = (uint)m_Ids.Length;

        	return isFound;
        }

        /// <summary>
        /// Inserts this range after a specific range that's already associated with an ID group.
        /// </summary>
        /// <param name="range">The range to insert after.</param>
        /// <param name="group">The ID group to insert into.</param>
        /// <returns>True if inserted ok.</returns>
        bool InsertAfter(IdRange range, IdGroup group)
        {
            return group.InsertAfter(range, this);
        }

        /// <summary>
        /// Kills any unused space in the array of IDs. This should be called only
        /// when unused IDs are being released back to the database.
        /// </summary>
        void Compress()
        {
	        // There's nothing to do if the range has been released,
	        // or the usage exactly fits the range. Note that in the
	        // second case, it is possible that some of the ID pointers
	        // refer to inactive IDs (i.e. m_NumFree could be > 0).
	        if (m_IsReleased || m_NumUsed==this.Size)
                return;

	        // If NOTHING has been used (all pointers are null), just toast the lot.
	        if (m_NumUsed==0)
            {
        		m_Ids = null;
        		m_NumFree = 0;
		        return;
	        }
	
	        // Allocate a new array
            FeatureId[] newIds = new FeatureId[m_NumUsed];

	        // Go through the existing array, copying over any defined pointers.

	        uint ncopy=0;			// The number copied so far

	        for (uint i=0; i<m_Ids.Length; i++)
            {
		        if (m_Ids[i]!=null)
                {
			        newIds[ncopy] = m_Ids[i];
			        ncopy++;
		        }
	        }

	        // Confirm that the number of pointers we copied is in agreement
	        // with the usage count. If not, issue a warning message and fix it.

        	if (ncopy!=m_NumUsed)
            {
                string msg = String.Format("IdRange.Compress - ID range usage count set to {0} (was {1})"
			                                , ncopy, m_NumUsed);
                MessageBox.Show(msg);
        		m_NumUsed = ncopy;
	        }

	        // Toast the original array and replace with the new one.
        	m_Ids = newIds;
        }

        /// <summary>
        /// Checks whether this range can be extended with an additional range.
        /// </summary>
        /// <param name="db">The database the range is recorded in.</param>
        /// <param name="minid">The low end of the proposed extension.</param>
        /// <param name="maxid">The high end of the proposed extension.</param>
        /// <param name="keyFormat">The key format for the range.</param>
        /// <returns>True if the range has been extended.</returns>
        internal bool Extend(int minid, int maxid, string keyFormat)
        {
	        // Ranges that have been released cannot be extended.
	        if (m_IsReleased)
                return false;

	        // The beginning of the extension MUST follow the existing range.
	        if (m_HighestId+1 != minid)
                return false;

	        // The extension must be valid!
	        if (maxid<minid)
                return false;

	        // The key formats must be identical.
	        if (keyFormat!=null && m_KeyFormat==null)
                return false;

	        if (keyFormat==null && m_KeyFormat!=null)
                return false;

            if (String.Compare(m_KeyFormat, keyFormat)!=0)
                return false;

	        // Update the allocation recorded in the database. We do not
	        // worry about the timestamp, or who made the allocation.
            if (IdAllocation.UpdateHighestId((int)m_LowestId, maxid)!=1)
                return false;

	        // And update the max this object knows about!
	        m_HighestId = (uint)maxid;

	        // Extend the allocation of pointers that we have.
	        uint nextra = (uint)(maxid-minid+1);
	        uint nalloc = (uint)(m_Ids.Length + nextra);
            FeatureId[] newIds = new FeatureId[nalloc];

	        // Copy over what we had (the rest contains nulls)
            Array.Copy(m_Ids, newIds, m_Ids.Length);

        	// Replace the old array with the new one
        	m_Ids = newIds;

	        // Update the number of free slots.
	        m_NumFree += nextra;
            return true;
        }

        /// <summary>
        /// Checks whether this range is available for generating one additional ID.
        /// </summary>
        /// <param name="group">The ID group that this range belongs to (has the final
        /// say-so as to whether a slot is available or not).</param>
        /// <returns>True if the range contains an available ID.</returns>
        internal bool IsAvail(IdGroup group)
        {
	        // Released ranges are never available.
	        if (m_IsReleased)
                return false;

	        // Return if nothing is free.
	        if (m_NumFree==0)
                return false;

	        // If the range contains any inactive IDs, we will need to check for them.
	        uint numnull = this.Size - m_NumUsed;
	        bool hasInactive = (m_NumFree > numnull);

	        // Scan the array looking for a free slot.

	        for (uint i=0; i<m_Ids.Length; i++ )
            {
		        if (m_Ids[i]==null)
                {
			        uint keynum = m_LowestId + i;
			        if (!group.IsReserved(keynum))
                        return true;
		        }
		        else 
                {
			        if (hasInactive && m_Ids[i].IsInactive)
                    {
				        uint keynum = m_LowestId + i;
				        if (!group.IsReserved(keynum))
                            return true;
			        }
		        }
	        }

	        return false;
        }

        /// <summary>
        /// Reserves the next available ID in this range (if any).
        /// </summary>
        /// <param name="group">The ID group to which this range belongs.</param>
        /// <returns>The reserved ID (null if an ID could not be obtained).</returns>
        internal uint ReserveId(IdGroup group)
        {
	        // Return if this range has already been released.
	        if (m_IsReleased)
                return 0;

	        // Return if the range has been completely used up.
	        if (m_NumFree==0)
                return 0;

	        // If the range contains any inactive IDs, we will need to check for them.
	        uint numnull = this.Size - m_NumUsed;
	        bool hasInactive = (m_NumFree > numnull);

	        // Scan though the array of ID pointers, looking for a free slot.
	        // If we find one, get the group to confirm that it's ok to
	        // reserve it (if not, keep going).

	        for (uint i=0; i<m_Ids.Length; i++)
            {
		        if (m_Ids[i]==null)
                {
			        uint keynum = m_LowestId + i;
			        if (group.ReserveId(keynum))
                        return keynum;
		        }
		        else 
                {
			        if (hasInactive && m_Ids[i].IsInactive)
                    {
				        uint keynum = m_LowestId + i;
				        if (group.ReserveId(keynum))
                            return keynum;
			        }
		        }
	        }

	        return 0;
        }

        /// <summary>
        /// Reserves a specific ID in this range.
        /// </summary>
        /// <param name="group">The ID group to which this range belongs. This will
        /// be modified to include the reserved ID (if successful).</param>
        /// <param name="id"></param>
        /// <returns>True if the ID has been reserved successfully.</returns>
        internal bool ReserveId(IdGroup group, uint id)
        {
	        // Never reserve an ID in a released range.
	        if (m_IsReleased)
            {
		        MessageBox.Show("IdRange.ReserveId - Attempt to reserve ID in released range");
		        return false;
	        }

	        // Get the index of the specified ID.
	        int index = GetIndex(id);
	        if (index<0) 
            {
		        MessageBox.Show("IdRange.ReserveId - Index out of range");
		        return false;
	        }

	        // Confirm that the ID is not already in use. It MAY have
	        // a defined ID pointer, but if it's inactive, that's ok.
	        if (m_Ids[index]!=null)
            {
		        if (!m_Ids[index].IsInactive)
                {
			        MessageBox.Show("IdRange.ReserveId - ID already used");
			        return false;
		        }
	        }

	        // Get the group to reserve the ID.
	        return group.ReserveId(id);
        }

        /// <summary>
        /// Returns the array index for a specific raw ID. This function does NOT work
        /// for ranges that have been released.
        /// </summary>
        /// <param name="id">The ID we want the index for.</param>
        /// <returns>The index value, or -1 if the ID is out of range.</returns>
        int GetIndex(uint id)
        {
            // If the range has been released, we can't get the index
            // (well, we could, but it would involve formatting the
            // specified ID and scanning the array).
            if (m_IsReleased)
                return -1;

            // Confirm that the specified id is in range.
            if (id<m_LowestId || id>m_HighestId)
                return -1;

            // Return the index position.
            return (int)(id-m_LowestId);
        }

        /// <summary>
        /// Returns the array index for a specific feature ID.
        /// </summary>
        /// <param name="fid">The ID we want the index for.</param>
        /// <returns>The index value, or -1 if the ID was not found.</returns>
        int GetIndex(FeatureId fid)
        {
            for (int i=0; i<m_Ids.Length; i++)
            {
                if (Object.ReferenceEquals(m_Ids[i], fid))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Creates a new feature ID.
        /// </summary>
        /// <param name="id">The ID to create.</param>
        /// <param name="feature">The feature to assign the ID to.</param>
        /// <returns>The created feature ID (null on error).</returns>
        internal FeatureId CreateId(uint id, Feature feature)
        {
	        // This range should NOT be already released.
	        if (m_IsReleased)
            {
		        MessageBox.Show("IdRange.CreateId - Cannot assign ID (ID range has been released.");
		        return null;
	        }

	        // Confirm that the specified ID falls within this range.
	        if (id<m_LowestId || id>m_HighestId)
            {
		        MessageBox.Show("IdRange.CreateId - New ID is not in range!");
		        return null;
	        }

	        // Confirm that the feature does not already have an ID.
	        if (feature.Id!=null)
            {
		        MessageBox.Show("IdRange.CreateId - Feature already has an ID.");
		        return null;
	        }

	        // Confirm that the range does not already refer to an active ID.
	        int index = (int)(id - m_LowestId);
	        bool reuse = false;
	        FeatureId fid = m_Ids[index];

	        if (fid!=null)
            {
		        if (fid.IsInactive)
			        reuse = true;
		        else
                {
			        MessageBox.Show("IdRange.CreateId - ID slot has already been used.");
			        return null;
		        }
	        }

	        // If we're not re-using an old ID, create one.
	        if (!reuse)
            {
		        string keystr;      // The key string
		        bool isNumeric;     // Should FeatureId try to make it numeric?

		        // Try to get a numeric key.
		        uint keyval = GetNumericKey(id);

		        if (keyval!=0)
                {
			        // If we got one, we still need to represent it as a string (for
			        // the FeatureId constructor). However, we need to let the
			        // constructor know that it's ok to convert it back to numeric!

			        keystr = keyval.ToString();
			        isNumeric = true;
		        }
		        else
                {
			        // Format the ID using the key format.
			        keystr = String.Format(m_KeyFormat, id);

			        // If a check digit is required, work it out and append it.
			        if (m_IsCheckDigit)
                    {
				        string extra = Key.GetCheckDigit(id).ToString();
				        keystr += extra;
			        }

			        // Remember that it's not to be converted to numeric.
			        isNumeric = false;
		        }

		        // Create the new ID (and point it to the feature).
                fid = new FeatureId(feature, keystr, isNumeric);

		        // Remember the new ID and increment the usage count.
		        m_Ids[index] = fid;
		        m_NumUsed++;
        	}
	        else
            {
		        // Refer the old ID to the feature.
		        fid.AddReference(feature);
	        }

	        // Point the feature back to the ID (not foreign).
	        feature.SetId(fid, false);

	        // Decrement the number of free IDs.
	        m_NumFree--;
	        return fid;
        }

        /// <summary>
        /// Tries to produce a numeric key.
        /// </summary>
        /// <param name="id">The raw ID to use (without any check digit).</param>
        /// <returns>The numeric key value (0 if a numeric key cannot be generated).</returns>
        uint GetNumericKey(uint id)
        {
	        // If a check digit is required, and the raw ID exceeds 200
	        // million, it means that the numeric key wouldn't fit in
	        // a 32-bit value.
	        if (m_IsCheckDigit && id > 200000000)
                return 0;

	        // If the key format is not completely numeric, we can't do it.
	        if (!IsNumericKey())
                return 0;

	        // If a check digit is not required, the supplied raw ID is
	        // the numeric key we want to store.
	        if (!m_IsCheckDigit)
                return id;

	        // Work out the check digit and append it to the raw ID.
	        uint checkdig = Key.GetCheckDigit(id);
	        return (id*10 + checkdig);
        }

        /// <summary>
        /// Checks if this range has numeric keys. This means that if the raw ID (without
        /// any check digit) is formatted, does it produce a right-justified string that
        /// contains just numeric digits.
        /// 
        /// Note that a numeric key may actually need to be stored as a string if a check
        /// digit causes the resulting number to exceed 2 billion.
        /// </summary>
        /// <returns>True if the keys are numeric.</returns>
        bool IsNumericKey()
        {
            // Use the key format string to parse the value "1"
            string str = String.Format(m_KeyFormat, 1);

            // If we got just 1 character, it's numeric.
            return (str.Length==1);
        }

        /// <summary>
        /// Loads a list with all the IDs that are available for this range.
        /// </summary>
        /// <param name="avail">The list to load.</param>
        /// <param name="group">The group this range is part of.</param>
        /// <returns>The number of IDs that were added to the array.</returns>
        internal uint GetAvailIds(List<uint> avail, IdGroup group)
        {
	        // Return if this range has already been released.
	        if (m_IsReleased)
                return 0;

	        // Return if the range has been completely used up.
	        if (m_NumFree==0)
                return 0;

	        // If the range contains any inactive IDs, we will need to check for them.
	        uint numnull = this.Size  -m_NumUsed;
	        bool hasInactive = (m_NumFree > numnull);

	        uint navail = 0; // Nothing found so far.

	        // Check if the group has any reserved IDs in THIS range.

	        uint nres = group.GetReserveCount(m_LowestId, m_HighestId);
        	if (nres==0)
            {
		        // No reserves, so just scan though the array of ID pointers,
		        // appending every free slot to the return array.

		        for (uint i=0, keynum=m_LowestId; i<m_Ids.Length; i++, keynum++)
                {
			        if (m_Ids[i]==null)
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
        		group.GetReserveIds(m_LowestId, m_HighestId, reserves);

		        // As above, scan through the array of ID pointers, looking
		        // for a free slot. But don't add in any IDs that have been
		        // reserved.

		        for (uint i=0, keynum=m_LowestId; i<m_Ids.Length; i++, keynum++)
                {
			        uint tnum = 0;
			        if (m_Ids[i]!=null)
                    {
				        if (hasInactive && m_Ids[i].IsInactive)
                            tnum = keynum;
			        }
			        else
				        tnum = keynum;

			        if (tnum!=0)
                    {
				        bool isReserved = false;

				        for (int j=0; j<nres && !isReserved; j++)
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
        /// Formats an ID number the way this range likes to see it.
        /// </summary>
        /// <param name="id">The ID to format (may not actually lie within the
        /// limits of this range).</param>
        /// <returns>The formatted result.</returns>
        internal string FormatId(uint id)
        {
            // Apply the format assuming no check digit.
            string key = String.Format(m_KeyFormat, id);

            // If there really is a check digit, work it out and append it.
            if (m_IsCheckDigit)
            {
                uint cd = Key.GetCheckDigit(id);
                key += cd.ToString();
            }

            return key;
        }

        /// <summary>
        /// Check if this ID range refers to a specific feature ID.
        /// </summary>
        /// <param name="fid">The feature ID to check for.</param>
        /// <returns>True if the feature ID was found.</returns>
        internal bool IsReferredTo(FeatureId fid)
        {
            return (GetIndex(fid)>=0);
        }

        /// <summary>
        /// Tries to find a previously created feature ID.
        /// </summary>
        /// <param name="id">The raw ID to find.</param>
        /// <returns>The corresponding feature ID (if found).</returns>
        FeatureId FindId(uint id)
        {
	        // If this range has not been released, just get the array
	        // index and return whatever is there.
	        if (!m_IsReleased)
            {
		        int index = GetIndex(id);
		        if (index<0)
			        return null;
		        else
			        return m_Ids[index];
	        }

	        // The range has been released. Confirm the specified ID is in this range.
	        if (id<m_LowestId || id>m_HighestId)
                return null;

	        // If we used up ALL the IDs in the released range, the
	        // answer is given by the index position.
	        if (m_NumUsed == this.Size)
            {
		        int index = (int)(id-m_LowestId);
		        return m_Ids[index];
	        }
	        else
            {
		        // The range had some gaps in it, so we'll have to match
		        // up the keys. To start with, format the specified ID
		        // the way this range likes it.

                string key = FormatId(id);

		        // Go through each ID looking for a matching key.
		        for (uint i=0; i<m_NumUsed; i++)
                {
			        FeatureId fid = m_Ids[i];
			        if (fid!=null && key==fid.FormattedKey)
                        return fid;
		        }

		        return null; // not found
	        }
        }

        /// <summary>
        /// Deletes an ID pointer that this range points to. This nulls out the ID pointer,
        /// and should be called ONLY if the feature ID is about to be physically deleted
        /// as a result of rollback.
        /// 
        /// Note that IDs may need to be nulled out, even if the range has been released.
        /// However, IDs of this sort will never be re-used, since the function(s) that
        /// return available IDs should always skip released ranges.
        /// 
        /// Belay the above comment. You CAN'T zero out the pointer, or delete the ID object!
        /// If you do that, and the ID had been previously re-used, we'd have a prior feature
        /// still pointing to it. So if you went to restore that, it would not find the ID in
        /// the ID range, leading to an error. But the main issue is that we could have a
        /// pointer to deleted memory, which is a definite no-no!
        /// </summary>
        /// <param name="fid">The feature ID to remove. At call, it should be inactive (not
        /// referring to anything).</param>
        /// <returns>True if ID pointer has been nulled out.</returns>
        internal bool DeleteId(FeatureId fid)
        {
            // Confirm the ID is inactive.
            if (!fid.IsInactive)
            {
                MessageBox.Show("IdRange.DeleteId - ID is still in use");
                return false;
            }

            // Get the array index of the ID.
            int index = GetIndex(fid);

            // Return if not found.
            if (index < 0)
                return false;

            // Null out the pointer ... NO (see comment up top).
            // m_Ids[index] = null;
            // m_NumUsed--;

            // Increment the number of free slots.
            m_NumFree++;

            return true;
        }

        /// <summary>
        /// Frees an ID pointer that this range points to. This confirms that this range
        /// really does point to the ID and, if so, the number of free IDs will be incremented.
        /// 
        /// This function is like DeleteId, except that it should be used in situations where
        /// the associated feature is in the process of being deactived. This makes it possible
        /// to restore the ID if a user-perceived deletion needs to be rolled back.
        /// </summary>
        /// <param name="fid">The feature ID to free. At call, it should be inactive (not
        /// referring to anything).</param>
        /// <returns>True if ID pointer was found.</returns>
        bool FreeId(FeatureId fid)
        {
	        // Confirm the ID is inactive.
	        if (!fid.IsInactive)
            {
		        MessageBox.Show("IdRange.FreeId - ID is still in use");
		        return false;
	        }

	        // Get the array index of the ID.
	        int index = GetIndex(fid);

	        // Return if not found.
	        if (index < 0)
                return false;

	        // Increment the number of free slots.
	        m_NumFree++;	
        	return true;
        }

        /// <summary>
        /// Restores an ID pointer that this range points to. This confirms that this
        /// range really does point to the ID and, if so, the number of free IDs will
        /// be decremented.
        /// 
        /// This function undoes a call to FreeId, and is called when a user-perceived
        /// deletion is being rolled back.
        /// </summary>
        /// <param name="fid">The feature ID to restore.</param>
        /// <returns>True if ID pointer was found.</returns>
        internal bool RestoreId(FeatureId fid)
        {
            // Get the array index of the ID.
            int index = GetIndex(fid);

            // Return if not found.
            if (index < 0)
            {
                MessageBox.Show("IdRange.RestoreId - ID not found");
                return false;
            }

            // Decrement the number of free slots (DON'T accidentally
            // decrement past zero, because that's a BIG number).
            if (m_NumFree>0)
                m_NumFree--;

            return true;
        }
    }
}
