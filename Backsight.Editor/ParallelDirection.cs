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
	/// <written by="Steve Stanton" on="13-NOV-1997" />
    /// <summary>
    /// A parallel is a direction that is defined in terms of a from-point, and
    /// a pair of points that the direction is parallel to. Parallel directions are
    /// always regarded as FIXED directions.
    /// </summary>
    class ParallelDirection : Direction
    {
        #region Class data

        /// <summary>
        /// The origin of the direction.
        /// </summary>
        private readonly PointFeature m_From;

        /// <summary>
        /// Point defining start of parallel.
        /// </summary>
        private readonly PointFeature m_Par1;

        /// <summary>
        /// Point defining end of parallel.
        /// </summary>
        private readonly PointFeature m_Par2;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="from">The point the direction is from.</param>
        /// <param name="par1">The first point in the definition of the parallel line.</param>
        /// <param name="par2">The second point defining the parallel line.</param>
        internal ParallelDirection(PointFeature from, PointFeature par1, PointFeature par2) : base()
        {
            m_From = from;
            m_Par1 = par1;
            m_Par2 = par2;
        }

        #endregion

        internal override PointFeature From
        {
            get { return m_From; }
        }

        internal PointFeature Start
        {
            get { return m_Par1; }
        }

        internal PointFeature End
        {
            get { return m_Par2; }
        }

        /// <summary>
        /// Returns the "observed" angle as a bearing. 
        /// </summary>
        internal override double ObservationInRadians
        {
            get { return this.Bearing.Radians; }
        }

        /// <summary>
        /// The bearing of this direction.
        /// </summary>
        internal override IAngle Bearing
        {
            get
            {
                // Return if either of the parallel points are undefined.
	            if (m_Par1==null || m_Par2==null)
                    return new RadianValue(0.0);

                return new RadianValue(Geom.BearingInRadians(m_Par1, m_Par2));
            }
        }

        internal override DirectionType DirectionType
        {
            get { return DirectionType.Parallel; }
        }

        /// <summary>
        /// Returns true if this parallel refers to the same point features as the
        /// supplied parellel. Stuff in the base class is irrelevant.
        /// </summary>
        /// <param name="that">The parallel to compare with.</param>
        /// <returns>True if the supplied parallel matches this one.</returns>
        public bool Equals(ParallelDirection that)
        {
            if (that==null)
                return false;

            return (Object.ReferenceEquals(this.m_From, that.m_From) &&
                    Object.ReferenceEquals(this.m_Par1, that.m_Par1) &&
                    Object.ReferenceEquals(this.m_Par2, that.m_Par2));
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

            if (m_Par1!=null)
                m_Par1.CutOp(op);

            if (m_Par2!=null)
                m_Par2.CutOp(op);
        }

        /// <summary>
        /// Checks whether this observation makes reference to a specific feature.
        /// </summary>
        /// <param name="feature">The feature to check for.</param>
        /// <returns>True if this direction refers to the feature</returns>
        internal override bool HasReference(Feature feature)
        {
            if (Object.ReferenceEquals(m_From, feature) ||
                Object.ReferenceEquals(m_Par1, feature) ||
                Object.ReferenceEquals(m_Par2, feature))
                return true;

            return base.HasReference(feature);
        }

        /// <summary>
        /// Writes the content of this class. This is called by
        /// <see cref="XmlContentWriter.WriteElement"/>
        /// after the element name and class type (xsi:type) have been written.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteContent(XmlContentWriter writer)
        {
            base.WriteContent(writer);
            writer.WriteString("From", m_From.DataId);
            writer.WriteString("Par1", m_Par1.DataId);
            writer.WriteString("Par2", m_Par2.DataId);
        }
    }
}
