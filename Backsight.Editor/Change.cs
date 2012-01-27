// <remarks>
// Copyright 2012 - Steve Stanton. This file is part of Backsight
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
    /// <written by="Steve Stanton" on="23-JAN-2012"/>
    /// <summary>
    /// Any sort of change
    /// </summary>
    class Change : IPersistent
    {
        #region Class data

        /// <summary>
        /// The sequence number of this change (starts at 1 for a new project, always increasing).
        /// </summary>
        readonly uint m_Sequence;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Change"/> class.
        /// </summary>
        /// <param name="editSequence">The sequence number of this change (greater than zero).</param>
        /// <exception cref="ArgumentException">If the supplied sequence number is zero.</exception>
        protected Change(uint editSequence)
        {
            if (editSequence == 0)
                throw new ArgumentException();

            m_Sequence = editSequence;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Change"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="ed">The mechanism for reading back content.</param>
        protected Change(EditDeserializer ed)
        {
            m_Sequence = ed.ReadUInt32(DataField.Id);
        }

        #endregion

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public virtual void WriteData(EditSerializer editSerializer)
        {
            editSerializer.WriteUInt32(DataField.Id, m_Sequence);
        }

        /// <summary>
        /// Change sequence number.
        /// </summary>
        public uint EditSequence
        {
            get { return m_Sequence; }
        }
    }
}
