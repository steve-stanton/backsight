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

namespace Backsight.Editor
{
	/// <written by="Steve Stanton" on="06-NOV-1997" />
    /// <summary>
    /// A bearing is an angle taken from a point with respect to grid north.
    /// </summary>
    class BearingDirection : Direction
    {
        #region Class data

        /// <summary>
        /// Angle from grid north, in range [0,2*PI].
        /// </summary>
        private readonly IAngle m_Observation;

        /// <summary>
        /// The point which the bearing was taken.
        /// </summary>
        private readonly PointFeature m_From;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="from">The point which the bearing was taken from.</param>
        /// <param name="observation">The observed bearing. If this is outwith the range
        /// [0,2PI), the value stored will be fixed so that it is in the expected range.
        /// </param>
        internal BearingDirection(PointFeature from, IAngle observation)
        {
            double a = observation.Radians;
            m_Observation = new RadianValue(Direction.Normalize(a));
            m_From = from;
        }

        #endregion

        internal override IAngle Bearing
        {
            get { return m_Observation; }
        }

        internal override IAngle Observation
        {
            get { return m_Observation; }
        }

        internal override PointFeature From
        {
            get { return m_From; }
        }

        /// <summary>
        /// Performs actions when the operation that uses this observation is marked
        /// for deletion as part of its rollback function. This cuts any reference from any
        /// previously existing feature that was cross-referenced to the operation (see
        /// calls made to AddOp).
        /// </summary>
        /// <param name="op">The operation that makes use of this observation.</param>
        internal override void OnRollback(Operation op)
        {
            CutReferences(op);
            base.OnRollback(op);
        }

        /// <summary>
        /// Cuts references to an operation that are made by the features this
        /// direction refers to.
        /// </summary>
        /// <param name="op">The operation that should no longer be referred to.</param>
        internal override void CutReferences(Operation op)
        {
            if (m_From!=null)
                m_From.CutOp(op);
        }

        internal override DirectionType DirectionType
        {
            get { return DirectionType.Bearing; }
        }

        /// <summary>
        /// Relational equality test. 
        /// </summary>
        /// <param name="that">The bearing to compare with</param>
        /// <returns>True if the supplied bearing has the same from-point, and the
        /// same observed value.</returns>
        public bool Equals(BearingDirection that)
        {
            if (that==null)
                return false;

            return (Object.ReferenceEquals(this.m_From, that.m_From) &&
                    Math.Abs(this.m_Observation.Radians - that.m_Observation.Radians) < Constants.TINY);
        }

        /// <summary>
        /// Checks whether this observation makes reference to a specific feature.
        /// </summary>
        /// <param name="feature">The feature to check for.</param>
        /// <returns>True if this direction refers to the feature</returns>
        internal override bool HasReference(Feature feature)
        {
            if (Object.ReferenceEquals(m_From, feature))
                return true;

            return base.HasReference(feature);
        }

        /// <summary>
        /// Writes the content of this class. This is called by <see cref="WriteElement"/>
        /// after the class type (xsi:type) has been written, and after any attributes
        /// and elements that are part of the base class. Derived classes should override
        /// and call this implementation up front.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteContent(XmlContentWriter writer)
        {
            base.WriteContent(writer);
            writer.WriteString("From", m_From.DataId);
            writer.WriteElement(m_Observation, "Observation");
        }
    }
}
