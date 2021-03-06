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
	/// <written by="Steve Stanton" on="06-NOV-1997" />
    /// <summary>
    /// A bearing is an angle taken from a point with respect to grid north.
    /// </summary>
    class BearingDirection : Direction, IFeatureRef
    {
        #region Class data

        /// <summary>
        /// Angle from grid north, in range [0,2*PI].
        /// </summary>
        RadianValue m_Observation;

        /// <summary>
        /// The point from which the bearing was taken.
        /// </summary>
        PointFeature m_From;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BearingDirection"/> class.
        /// </summary>
        internal BearingDirection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BearingDirection"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal BearingDirection(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
            m_From = editDeserializer.ReadFeatureRef<PointFeature>(this, DataField.From);
            m_Observation = editDeserializer.ReadRadians(DataField.Value);
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
            m_Observation = new RadianValue(Direction.Normalize(a));
            m_From = from;
        }

        #endregion

        internal override IAngle Bearing
        {
            get { return m_Observation; }
        }

        internal override double ObservationInRadians
        {
            get { return m_Observation.Radians; }
        }

        internal void SetObservationInRadians(double value)
        {
            m_Observation = new RadianValue(value);
        }

        internal override PointFeature From
        {
            get { return m_From; }
            set { m_From = value; }
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
        /// Obtains the features that are referenced by this observation.
        /// </summary>
        /// <returns>The referenced features (never null, but may be an empty array).</returns>
        internal override Feature[] GetReferences()
        {
            List<Feature> result = new List<Feature>(base.GetReferences());
            result.Add(m_From);
            return result.ToArray();
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
                    Math.Abs(this.m_Observation.Value - that.m_Observation.Value) < Constants.TINY);
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
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public override void WriteData(EditSerializer editSerializer)
        {
            base.WriteData(editSerializer);

            editSerializer.WriteFeatureRef<PointFeature>(DataField.From, m_From);
            editSerializer.WriteRadians(DataField.Value, m_Observation);
        }

        /// <summary>
        /// Ensures that a persistent field has been associated with a spatial feature.
        /// </summary>
        /// <param name="field">A tag associated with the item</param>
        /// <param name="feature">The feature to assign to the field (not null).</param>
        /// <returns>
        /// True if a matching field was processed. False if the field is not known to this
        /// class (may be known to another class in the type hierarchy).
        /// </returns>
        public bool ApplyFeatureRef(DataField field, Feature feature)
        {
            if (field == DataField.From)
            {
                m_From = (PointFeature)feature;
                return true;
            }

            return false;
        }
    }
}
