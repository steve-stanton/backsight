// <remarks>
// Copyright 2010 - Steve Stanton. This file is part of Backsight
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
using Backsight.Editor.Observations;

namespace Backsight.Editor
{
    /// <summary>
    /// A distance that is portrayed as an annotation alongside a line.
    /// </summary>
    class AnnotatedDistance : Distance, IPersistent
    {
        #region Class data

        /// <summary>
        /// Should the annotation be flipped (displayed on the non-default side)?
        /// </summary>
        bool m_IsFlipped;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OffsetDistance"/> class.
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal AnnotatedDistance(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
            ReadData(editDeserializer, out m_IsFlipped);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnnotatedDistance"/> class.
        /// </summary>
        /// <param name="distance">The distance observation (not null)</param>
        /// <param name="isFlipped">Should the annotation be flipped (displayed on the
        /// non-default side)?</param>
        /// <exception cref="ArgumentNullException">If the supplied distance is null.</exception>
        internal AnnotatedDistance(Distance distance, bool isFlipped)
            : base(distance)
        {
            m_IsFlipped = isFlipped;
        }

        #endregion

        /// <summary>
        /// Should the annotation be flipped (displayed on the non-default side)?
        /// </summary>
        internal bool IsFlipped
        {
            get { return m_IsFlipped; }
            set { m_IsFlipped = value; }
        }

        /// <summary>
        /// Toggles the <see cref="IsFlipped"/> property.
        /// </summary>
        internal void ToggleIsFlipped()
        {
            m_IsFlipped = !m_IsFlipped;
        }

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public override void WriteData(EditSerializer editSerializer)
        {
            base.WriteData(editSerializer);
            editSerializer.Writer.WriteBool("Flipped", m_IsFlipped);
        }

        /// <summary>
        /// Reads data that was previously written using <see cref="WriteData"/>
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        /// <param name="isFlipped">Is the annotation flipped (displayed on the non-default side)?</param>
        static void ReadData(EditDeserializer editDeserializer, out bool isFlipped)
        {
            isFlipped = editDeserializer.Reader.ReadBool("Flipped");
        }
    }
}
