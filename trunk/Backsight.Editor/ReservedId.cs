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
    /// <summary>
    /// An ID that has been reserved for use as part of an edit that is currently being entered.
    /// </summary>
    class ReservedId //: FeatureId
    {
        #region Class data

        /// <summary>
        /// The ID packet for holding the reserved ID (not null).
        /// </summary>
        readonly IdPacket m_Packet;

        /// <summary>
        /// The raw ID value within the packet.
        /// </summary>
        readonly uint m_RawId;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReservedId"/> class.
        /// </summary>
        /// <param name="packet">The ID packet for holding the reserved ID (not null)</param>
        /// <param name="rawId">The raw ID value within the packet</param>
        /// <exception cref="ArgumentNullException">If the specified ID packet is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the supplied raw ID is not enclosed by the packet</exception>
        internal ReservedId(IdPacket packet, uint rawId)
        {
            if (packet == null)
                throw new ArgumentNullException();

            if (!packet.Contains(rawId))
                throw new ArgumentOutOfRangeException();

            m_Packet = packet;
            m_RawId = rawId;
        }

        #endregion

        /// <summary>
        /// The ID packet for holding the reserved ID (not null).
        /// </summary>
        internal IdPacket Packet
        {
            get { return m_Packet; }
        }

        /// <summary>
        /// The raw ID value within the packet.
        /// </summary>
        internal uint RawId
        {
            get { return m_RawId; }
        }
    }
}
