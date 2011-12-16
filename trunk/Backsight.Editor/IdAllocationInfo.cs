// <remarks>
// Copyright 2011 - Steve Stanton. This file is part of Backsight
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

namespace Backsight.Editor
{
    /// <summary>
    /// Information about an ID allocation that has been made to a user for a specific editing job.
    /// </summary>
    public class IdAllocationInfo
    {
        #region Class data

        /// <summary>
        /// The unique ID of the ID group associated with this allocation
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// The lowest value in the allocation (this is the primary key)
        /// </summary>
        public int LowestId { get; set; }

        /// <summary>
        /// The highest value in the allocation
        /// </summary>
        public int HighestId { get; set; }

        /// <summary>
        /// The ID of the job that the allocation is for
        /// </summary>
        public int JobId { get; set; }

        /// <summary>
        /// The ID of the user who made the allocation
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// When was the allocation inserted into the database?
        /// </summary>
        public DateTime TimeAllocated { get; set; }

        /// <summary>
        /// The number of IDs that have been used
        /// </summary>
        //public int NumUsed { get; set; }

        /// <summary>
        /// The number of IDs in this allocation
        /// </summary>
        internal int Size
        {
            get { return (HighestId - LowestId + 1); }
        }

        #endregion
    }
}
