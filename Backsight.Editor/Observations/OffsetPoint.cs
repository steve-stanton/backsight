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
using System.Diagnostics;

namespace Backsight.Editor.Observations
{
	/// <written by="Steve Stanton" on="13-NOV-1997" />
    /// <summary>
    /// An offset that is defined by the location of a specific point feature.
    /// 
    /// When you create an instance, the point feature involved is not referred to any
    /// editing operation (that should be done when(if) the operation is saved, via a call
    /// to <c>AddReferences</c>).
    /// </summary>
    class OffsetPoint : Offset, IFeatureRef
    {
        #region Class data

        /// <summary>
        /// The point that defines the offset position (the actual distance to it
        /// depends on some reference object not known to this class).
        /// </summary>
        PointFeature m_Point;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OffsetPoint"/> class.
        /// </summary>
        internal OffsetPoint()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OffsetPoint"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal OffsetPoint(EditDeserializer editDeserializer)
        {
            m_Point = editDeserializer.ReadFeatureRef<PointFeature>(this, DataField.Point);
        }

        /// <summary>
        /// Constructor for an offset at the specified point.
        /// </summary>
        /// <param name="point">The offset point</param>
        internal OffsetPoint(PointFeature point)
        {
            m_Point = point;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="copy">The offset to copy</param>
        internal OffsetPoint(OffsetPoint copy)
        {
            m_Point = copy.m_Point;
        }

        #endregion

        /// <summary>
        /// The easting of the offset point (0 if the point is undefined).
        /// </summary>
        double Easting
        {
            get { return (m_Point==null ? 0.0 : m_Point.X); }
        }

        /// <summary>
        /// The northing of the offset point (0 if the point is undefined).
        /// </summary>
        double Northing
        {
            get { return (m_Point==null ? 0.0 : m_Point.Y); }
        }

        /// <summary>
        /// Obtains the features that are referenced by this observation.
        /// </summary>
        /// <returns>The referenced features (never null, but may be an empty array).</returns>
        internal override Feature[] GetReferences()
        {
            return new Feature[] { m_Point };
        }

        /// <summary>
        /// Returns the offset distance with respect to a reference direction, in meters
        /// on the ground. Offsets to the left are returned as a negated value, while
        /// offsets to the right are positive values.
        /// </summary>
        /// <param name="dir">The direction that the offset was observed with respect to.</param>
        /// <returns>The signed offset distance, in meters on the ground</returns>
        internal override double GetMetric(Direction dir)
        {
            // Return offset of zero if there is no offset point.
	        if (m_Point==null)
                return 0.0;

            // Get the origin of the direction & it's bearing. This gives
            // us the info we need to express the equation of the line
            // in parametric form.

	        PointFeature from = dir.From;
        	double x = from.X;
	        double y = from.Y;
            double bearing = dir.Bearing.Radians;

            // Get the position of the offset point.
        	double xoff = m_Point.X;
	        double yoff = m_Point.Y;

            // Get the signed perpendicular distance from the offset
            // point to the reference direction.
	        return BasicGeom.SignedDistance(x, y, bearing, xoff, yoff);
        }

        /// <summary>
        /// The offset point.
        /// </summary>
        internal override PointFeature Point
        {
	        get { return m_Point; }
        }

        /// <summary>
        /// Checks whether this offset makes reference to a specific feature.
        /// </summary>
        /// <param name="feat">The feature to check for.</param>
        /// <returns>True if the offset point is the same instance as the supplied feature</returns>
        internal override bool HasReference(Feature feat)
        {
            return Object.ReferenceEquals(m_Point, feat);
        }

        /// <summary>
        /// Performs actions when the operation that uses this observation is marked
        /// for deletion as part of its rollback function. This cuts any reference from any
        /// previously existing feature that was cross-referenced to the operation (see
        /// calls made to <c>AddOp</c>).
        /// </summary>
        /// <param name="oper">The operation that makes use of this observation.</param>
        internal override void OnRollback(Operation oper)
        {
            Debug.Assert(oper!=null);
            if (m_Point!=null)
                m_Point.CutOp(oper);
        }

        /// <summary>
        /// Cuts references to an operation that are made by the features this offset refers to.
        /// </summary>
        /// <param name="op">The operation that should no longer be referred to.</param>
        internal override void CutRef(Operation op)
        {
        	if (m_Point!=null)
                m_Point.CutOp(op);
        }

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public override void WriteData(EditSerializer editSerializer)
        {
            editSerializer.WriteFeatureRef<PointFeature>(DataField.Point, m_Point);
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
            if (field == DataField.Point)
            {
                m_Point = (PointFeature)feature;
                return true;
            }

            return false;
        }
    }
}
