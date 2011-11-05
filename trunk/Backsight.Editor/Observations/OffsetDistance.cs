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


namespace Backsight.Editor.Observations
{
	/// <written by="Steve Stanton" on="13-NOV-1997" />
    /// <summary>
    /// An offset that is expressed in the form of a distance observation.
    /// </summary>
    class OffsetDistance : Offset, IPersistent
    {
        #region Class data

        /// <summary>
        /// The offset
        /// </summary>
        Distance m_Offset;

        /// <summary>
        /// True if the offset is left of the object that acts as the reference for the offset.
        /// </summary>
        bool m_IsLeft;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OffsetDistance"/> class.
        /// </summary>
        internal OffsetDistance()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OffsetDistance"/> class.
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="reader">The reading stream (positioned ready to read the first data value).</param>
        internal OffsetDistance(IEditReader reader)
        {
            ReadData(reader, out m_Offset, out m_IsLeft);
        }

        /// <summary>
        /// Creates an <c>OffsetDistance</c> using the supplied values
        /// </summary>
        /// <param name="dist">The offset distance</param>
        /// <param name="isLeft">Is the offset to the left of the object that acts as
        /// the reference for the offset?</param>
        internal OffsetDistance(Distance dist, bool isLeft)
        {
            m_Offset = dist;
            m_IsLeft = isLeft;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="copy">The offset to copy</param>
        internal OffsetDistance(OffsetDistance copy)
        {
            m_Offset = new Distance(copy.m_Offset);
            m_IsLeft = copy.m_IsLeft;
        }

        #endregion

        /// <summary>
        /// Relational equality test. 
        /// </summary>
        /// <param name="that">The offset to compare with</param>
        /// <returns>True if the offset distance (and direction) match (the units used
        /// to express the distance may be different).</returns>
        public bool Equals(OffsetDistance that)
        {
            return (m_IsLeft == that.m_IsLeft &&
                    Math.Abs(m_Offset.Meters - that.m_Offset.Meters) < Constants.TINY);
        }

        /// <summary>
        /// Marks offset to the left. 
        /// </summary>
        internal void SetLeft()
        {
            m_IsLeft = true;
        }

        /// <summary>
        /// Marks offset to the right.
        /// </summary>
        internal void SetRight()
        {
            m_IsLeft = false;
        }

        /// <summary>
        /// Is the offset to the right of whatever.
        /// </summary>
        internal bool IsRight
        {
            get { return !m_IsLeft; }
        }

        /// <summary>
        /// The offset observation.
        /// </summary>
        internal Distance Offset
        {
            get { return m_Offset; }
            set { m_Offset = value; }
        }

        /// <summary>
        /// Returns the offset distance with respect to a reference direction, in meters
        /// on the ground. Offsets to the left are returned as a negated value, while
        /// offsets to the right are positive values.
        /// </summary>
        /// <param name="dir">The direction that the offset was observed with respect to.</param>
        /// <returns>The signed offset distance, in meters on the ground.</returns>
        internal override double GetMetric(Direction dir)
        {
            // In the case of an offset distance, the reference direction
            // is not actually required to get the offset (it is required
            // as a parameter only because the base class defines a pure
            // virtual this way).

            return this.Meters;
        }

        /// <summary>
        /// Returns the offset distance, in meter on the ground. Offsets to the left are
        /// returned as a negated value, while offsets to the right are positive values.
        /// </summary>
        /// <returns></returns>
        double Meters
        {
            get
            {
                if (m_IsLeft)
                    return -m_Offset.Meters;
                else
                    return m_Offset.Meters;
            }
        }

        /// <summary>
        /// Associated point is always null for an offset distance.
        /// </summary>
        internal override PointFeature Point
        {
            get { return null; }
        }

        internal override void CutRef(Operation op)
        {
            // nothing to do
        }

        /// <summary>
        /// Checks whether this offset makes reference to a specific feature.
        /// </summary>
        /// <param name="feat">The feature to check for.</param>
        /// <returns>True if the offset distance references the supplied feature</returns>
        internal override bool HasReference(Feature feature)
        {
            return m_Offset.HasReference(feature);
        }

        /// <summary>
        /// Obtains the features that are referenced by this observation.
        /// </summary>
        /// <returns>The referenced features (never null, but may be an empty array).</returns>
        internal override Feature[] GetReferences()
        {
            return m_Offset.GetReferences();
        }

        /// <summary>
        /// Performs actions when the operation that uses this observation is marked
        /// for deletion as part of its rollback function. This cuts any reference from any
        /// previously existing feature that was cross-referenced to the operation (see
        /// calls made to <c>AddOp</c>).
        /// </summary>
        /// <param name="oper">The operation that makes use of this observation.</param>
        internal override void OnRollback(Operation op)
        {
            // nothing to do
        }

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="writer">The mechanism for storing content.</param>
        public override void WriteData(IEditWriter writer)
        {
            writer.WriteObject<Distance>("Offset", m_Offset);
            writer.WriteBool("Left", m_IsLeft);
        }

        /// <summary>
        /// Reads data that was previously written using <see cref="WriteData"/>
        /// </summary>
        /// <param name="reader">The reader for loading data values</param>
        /// <param name="offset">The offset</param>
        /// <param name="isLeft">Is the offset to the left of the object that acts as the reference for the offset.</param>
        static void ReadData(IEditReader reader, out Distance offset, out bool isLeft)
        {
            offset = reader.ReadObject<Distance>("Offset");
            isLeft = reader.ReadBool("Left");
        }
    }
}
