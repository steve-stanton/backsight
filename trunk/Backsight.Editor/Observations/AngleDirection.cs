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
    /// An "angle" is an angle taken from a point, with respect to a backsight
    /// that provides the reference orientation.
    /// </summary>
    class AngleDirection : Direction
    {
        #region Class data

        /// <summary>
        /// The angle in radians. A negated value indicates an anticlockwise angle.
        /// </summary>
        readonly double m_Observation;

        /// <summary>
        /// The backsight point.
        /// </summary>
        readonly PointFeature m_Backsight;

        /// <summary>
        /// The occupied station.
        /// </summary>
        readonly PointFeature m_From;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for use during deserialization
        /// </summary>
        /// <param name="op">The editing operation utilizing the observation</param>
        /// <param name="t">The serialized version of this observation</param>
        internal AngleDirection(Operation op, AngleType t)
            : base(op, t)
        {
            if (!RadianValue.TryParse(t.Value, out m_Observation))
                throw new Exception("AngleDirection - Cannot parse 'Value' attribute");

            m_Backsight = op.MapModel.Find<PointFeature>(t.Backsight);
            m_From = op.MapModel.Find<PointFeature>(t.From);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="backsight">The backsight point.</param>
        /// <param name="occupied">The occupied station.</param>
        /// <param name="observation">The angle to an observed point, measured with respect
        /// to the reference orientation defined by the backsight. Positive values indicate
        /// a clockwise rotation & negated values for counter-clockwise.</param>
        internal AngleDirection(PointFeature backsight, PointFeature occupied, IAngle observation)
        {
            m_Backsight = backsight;
            m_From = occupied;
            m_Observation = observation.Radians;
        }

        #endregion

        internal override double ObservationInRadians
        {
            get { return m_Observation; }
        }

        /// <summary>
        /// The angle as a bearing
        /// </summary>
        internal override IAngle Bearing
        {
            get
            {
                // Get the bearing to the backsight
                double bb = Geom.BearingInRadians(m_From, m_Backsight);

                // Add on the observed angle, and restrict to [0,2*PI]
                double a = bb + m_Observation;
                return new RadianValue(Direction.Normalize(a));
            }
        }

        internal override PointFeature From
        {
            get { return m_From; }
        }

        internal PointFeature Backsight
        {
            get { return m_Backsight; }
        }

        internal override DirectionType DirectionType
        {
            get { return DirectionType.Angle; }
        }

        internal override void AddReferences(Operation op)
        {
            base.AddReferences(op);

            if (m_From!=null)
                m_From.AddOp(op);

            if (m_Backsight!=null)
                m_Backsight.AddOp(op);
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

            if (m_Backsight!=null)
                m_Backsight.CutOp(op);
        }

        /// <summary>
        /// Relational equality test. 
        /// </summary>
        /// <param name="that">The angle to compare with</param>
        /// <returns>True if the supplied angle has the same from-point & backsight,
        /// and the same observed value.</returns>
        public bool Equals(AngleDirection that)
        {
            if (that==null)
                return false;

            return (Object.ReferenceEquals(this.m_From, that.m_From) &&
                    Object.ReferenceEquals(this.m_Backsight, that.m_Backsight) &&
                    Math.Abs(this.m_Observation - that.m_Observation) < Constants.TINY);
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
        /// Checks whether this observation makes reference to a specific feature.
        /// </summary>
        /// <param name="feature">The feature to check for.</param>
        /// <returns>True if this direction refers to the feature</returns>
        internal override bool HasReference(Feature feature)
        {
            if (Object.ReferenceEquals(m_From, feature) ||
                Object.ReferenceEquals(m_Backsight, feature))
                return true;

            return base.HasReference(feature);
        }

        /// <summary>
        /// Writes the attributes of this class.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteAttributes(XmlContentWriter writer)
        {
            base.WriteAttributes(writer);
            writer.WriteFeatureReference("Backsight", m_Backsight);
            writer.WriteFeatureReference("From", m_From);
            writer.WriteAngle("Value", m_Observation);
        }

        /// <summary>
        /// The string that will be used as the xsi:type for this edit
        /// </summary>
        public override string XmlTypeName
        {
            get { return "AngleType"; }
        }

        /// <summary>
        /// Returns an object that represents this observation, and that can be serialized using
        /// the <c>XmlSerializer</c> class.
        /// </summary>
        /// <returns>The serializable version of this observation</returns>
        internal override ObservationType GetSerializableObservation()
        {
            AngleType t = new AngleType();
            SetSerializableObservation(t);
            t.Backsight = m_Backsight.DataId;
            t.From = m_From.DataId;
            t.Value = RadianValue.AsShortString(m_Observation);
            return t;
        }
    }
}
