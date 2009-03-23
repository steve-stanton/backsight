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

using Backsight.Editor.Xml;


namespace Backsight.Editor.Observations
{
	/// <written by="Steve Stanton" on="13-NOV-1997" />
    /// <summary>
    /// An offset that is expressed in the form of a distance observation.
    /// </summary>
    class OffsetDistance : Offset
    {
        #region Class data

        /// <summary>
        /// The offset
        /// </summary>
        Distance m_Offset;

        /// <summary>
        /// True if the offset is left of the object that acts as the reference
        /// for the offset.
        /// </summary>
        bool m_IsLeft;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for use during deserialization
        /// </summary>
        /// <param name="op">The editing operation utilizing the observation</param>
        /// <param name="t">The serialized version of this observation</param>
        internal OffsetDistance(Operation op, OffsetDistanceType t)
            : base(op, t)
        {
            m_Offset = new Distance(op, t.Distance);
            m_IsLeft = t.Left;
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
        internal OffsetDistance(OffsetDistance copy) : base(copy)
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
        }

        /// <summary>
        /// Returns the offset distance with respect to a reference direction, in meters
        /// on the ground. Offsets to the left are returned as a negated value, while
        /// offsets to the right are positive values.
        /// </summary>
        /// <param name="dir">The direction that the offset was observed with respect to.</param>
        /// <returns>The (signed) offset distance, in meters.</returns>
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
            set { throw new Exception("The method or operation is not implemented."); }
        }

        /// <summary>
        /// Saves this offset distance.
        /// </summary>
        /// <returns></returns>
        /*
        internal override Observation Save()
        {
            //	Confirm that an operation is being saved.
            IOperation oper = (IOperation)SaveOp;
            if (oper==null)
                return null;

            return this;
        }
         */

        internal override void AddReferences(Operation op)
        {
            // nothing to do
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
        /// Writes the attributes of this class.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteAttributes(XmlContentWriter writer)
        {
            base.WriteAttributes(writer);
            writer.WriteBool("Left", m_IsLeft);
        }

        /// <summary>
        /// Writes any child elements of this class. This will be called after
        /// all attributes have been written via <see cref="WriteAttributes"/>.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteChildElements(XmlContentWriter writer)
        {
            base.WriteChildElements(writer);
            writer.WriteElement("Distance", m_Offset);
        }

        /// <summary>
        /// The string that will be used as the xsi:type for this edit
        /// </summary>
        public override string XmlTypeName
        {
            get { return "OffsetDistanceType"; }
        }
    }
}
