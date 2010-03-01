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
        readonly double m_Observation;

        /// <summary>
        /// The point which the bearing was taken.
        /// </summary>
        readonly PointFeature m_From;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for use during deserialization
        /// </summary>
        /// <param name="op">The editing operation utilizing the observation</param>
        /// <param name="t">The serialized version of this observation</param>
        internal BearingDirection(Operation op, BearingType t)
            : base(op, t)
        {
            if (!RadianValue.TryParse(t.Value, out m_Observation))
                throw new Exception("BearingDirection - Cannot parse 'Value' attribute");

            m_From = op.MapModel.Find<PointFeature>(t.From);
        }

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
            m_Observation = Direction.Normalize(a);
            m_From = from;
        }

        #endregion

        internal override IAngle Bearing
        {
            get { return new RadianValue(m_Observation); }
        }

        internal override double ObservationInRadians
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
                    Math.Abs(this.m_Observation - that.m_Observation) < Constants.TINY);
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
        /// Returns an object that represents this observation, and that can be serialized using
        /// the <c>XmlSerializer</c> class.
        /// </summary>
        /// <returns>The serializable version of this observation</returns>
        internal override ObservationType GetSerializableObservation()
        {
            BearingType t = new BearingType();
            SetSerializableObservation(t);
            t.From = m_From.DataId;
            t.Value = RadianValue.AsShortString(m_Observation);
            return t;
        }
    }
}