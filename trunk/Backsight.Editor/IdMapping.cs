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

namespace Backsight.Editor
{
    /// <summary>
    /// Mapping that relates an internal ID to the raw ID for an instance of <see cref="NativeId"/>
    /// </summary>
    class IdMapping : IPersistent
    {
        #region Class data

        /// <summary>
        /// The internal ID.
        /// </summary>
        readonly InternalIdValue m_InternalId;

        /// <summary>
        /// The raw value for an associated instance of <see cref="NativeId"/>.
        /// </summary>
        readonly uint m_RawId;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IdMapping"/> class.
        /// </summary>
        /// <param name="internalId">The internal id.</param>
        /// <param name="rawId">The raw value for an associated instance of <see cref="NativeId"/>.</param>
        internal IdMapping(InternalIdValue internalId, uint rawId)
        {
            m_InternalId = internalId;
            m_RawId = rawId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdMapping"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal IdMapping(EditDeserializer editDeserializer)
        {
            m_InternalId = editDeserializer.ReadInternalId(DataField.Id);
            m_RawId = editDeserializer.ReadUInt32(DataField.Key);
        }

        #endregion

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public void WriteData(EditSerializer editSerializer)
        {
            editSerializer.WriteInternalId(DataField.Id, m_InternalId);
            editSerializer.WriteUInt32(DataField.Key, m_RawId);
        }

        /// <summary>
        /// The internal ID.
        /// </summary>
        internal InternalIdValue InternalId
        {
            get { return m_InternalId; }
        }

        /// <summary>
        /// The raw value for an associated instance of <see cref="NativeId"/>.
        /// </summary>
        internal uint RawId
        {
            get { return m_RawId; }
        }
    }
}
