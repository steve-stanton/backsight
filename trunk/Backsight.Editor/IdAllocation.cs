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
    /// Information about an ID allocation that has been made to a user for a specific editing project.
    /// </summary>
    class IdAllocation : Change
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

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IdAllocation"/> class.
        /// </summary>
        internal IdAllocation()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdAllocation"/> class.
        /// </summary>
        /// <param name="ed">The mechanism for reading back content.</param>
        internal IdAllocation(EditDeserializer ed)
            : base(ed)
        {
            this.GroupId = ed.ReadInt32(DataField.GroupId);
            this.LowestId = ed.ReadInt32(DataField.LowestId);
            this.HighestId = ed.ReadInt32(DataField.HighestId);
        }

        #endregion

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="es">The mechanism for storing content.</param>
        public override void WriteData(EditSerializer es)
        {
            base.WriteData(es);

            es.WriteInt32(DataField.GroupId, this.GroupId);
            es.WriteInt32(DataField.LowestId, this.LowestId);
            es.WriteInt32(DataField.HighestId, this.HighestId);
        }

        /// <summary>
        /// The number of IDs in this allocation
        /// </summary>
        internal int Size
        {
            get { return (HighestId - LowestId + 1); }
        }
    }
}
