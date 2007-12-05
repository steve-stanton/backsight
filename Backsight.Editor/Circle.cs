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
using System.Collections.Generic;
using System.Diagnostics;

using Backsight.Geometry;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="19-SEP-1997" />
    /// <summary>
    /// The definition of a circle (as used by the Cadastral Editor).
    /// </summary>
    /// <seealso cref="Backsight.Geometry.CircleGeometry"/>
    [Serializable]
    class Circle : ISpatialObject, ICircleGeometry, IFeatureDependent
    {
        #region Class data

        /// <summary>
        /// The radius of the circle.
        /// </summary>
        private ILength m_Radius;
        //private readonly Offset m_Radius;

        /// <summary>
        /// The center of the circle. This may be quite remote from the main body of the map.
        /// </summary>
        private readonly PointFeature m_Center;

        /// <summary>
        /// The arcs that coincide with the perimeter of this circle.
        /// </summary>
        private readonly List<ArcFeature> m_Arcs;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>Circle</c> with the specified center and radius.
        /// </summary>
        /// <param name="center">The point at the center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        internal Circle(PointFeature center, ILength radius)
        {
            m_Center = center;
            m_Radius = radius;
            m_Arcs = new List<ArcFeature>();
        }

        #endregion

        #region ISpatialObject Members

        public SpatialType SpatialType
        {
            get { return SpatialType.Line; }
        }

        public void Render(ISpatialDisplay display, IDrawStyle style)
        {
            CircleGeometry.Render(this, display, style);
        }

        public IWindow Extent
        {
            get { return CircleGeometry.GetExtent(this); }
        }

        public ILength Distance(IPosition point)
        {
            return CircleGeometry.Distance(this, point);
        }

        #endregion

        #region IFeatureDependent Members

        public void OnPreMove(Feature f)
        {
            CadastralMapModel map = f.MapModel;
            IEditSpatialIndex index = map.EditingIndex;
            index.Remove(this);
        }

        public void OnPostMove(Feature f)
        {
            CadastralMapModel map = f.MapModel;
            IEditSpatialIndex index = map.EditingIndex;
            index.Add(this);
        }

        public void AddReferences()
        {
            m_Center.AddReference(this);
        }

        #endregion

        #region ICircleGeometry Members

        public IPointGeometry Center
        {
            get { return m_Center; }
        }

        public ILength Radius
        {
            get { return m_Radius; }
        }

        #endregion

        // Not sure about this. Should it be disallowed?
        internal void ChangeRadius(ArcFeature arc, ILength newRadius)
        {
            if (Math.Abs(m_Radius.Meters - newRadius.Meters) > Constants.TINY)
            {
                OnPreMove(arc);
                m_Radius = newRadius;
                OnPostMove(arc);
            }
        }

        /// <summary>
        /// Associates an arc with this circle.
        /// </summary>
        /// <param name="arc">The arc that coincides with the perimeter of this circle (must
        /// already be cross-referenced to this circle)</param>
        /// <exception cref="ArgumentException">If the specified arc does not already
        /// refer to this circle.</exception>
        internal void AddArc(ArcFeature arc)
        {
            if (arc.Circle != this)
                throw new ArgumentException();

            if (!m_Arcs.Contains(arc))
                m_Arcs.Add(arc);
        }

        /// <summary>
        /// The arcs attached to this circle.
        /// </summary>
        internal ArcFeature[] Arcs
        {
            get { return m_Arcs.ToArray(); }
        }

        /// <summary>
        /// Removes an arc from this circle. This might be called if the operation that
        /// created the arc is getting rolled back. Another possible scenario is where
        /// the arc is being moved to coincide with a different circle.
        /// </summary>
        /// <param name="arc">The arc that no longer references this circle.</param>
        /// <returns>True if the specified arc was removed. False if it wasn't referenced.</returns>
        internal bool RemoveArc(ArcFeature arc)
        {
            return m_Arcs.Remove(arc);
        }

        /// <summary>
        /// Returns a point feature that sits at the center of this circle
        /// </summary>
        /// <param name="op">The operation that must be the creator of the centre
        /// point. Specify null (the default) if the creator doesn't matter.</param>
        /// <param name="onlyActive">True (the default) if the point has to be active.
        /// Specify false if inactive points are ok too.</param>
        /// <returns>The centre point (null if no such point).</returns>
        internal PointFeature GetCenter(Operation op, bool onlyActive)
        {
            if (m_Center==null)
                return null;

            if (op==null)
                return m_Center;

            return (m_Center.Creator==op ? m_Center : null);
            /*
             * TODO?
             * 
            if (op==null)
                return m_Center.GetPoint(null, null, onlyActive);
            else
                return m_Center.GetPoint(op, onlyActive);
             */
        }

        /// <summary>
        /// The point at the center of this circle.
        /// </summary>
        internal PointFeature CenterPoint
        {
            get { return m_Center; }
        }

        /// <summary>
        /// The operation that created this circle is the operation that created
        /// the first arc associated with the circle.
        /// </summary>
        internal Operation Creator
        {
            // Perhaps this should scan the list, since the first element may not
            // necessarily be the earliest edit (alternatively, modify AddArc to
            // ensure edit order is maintained).
            get { return (m_Arcs.Count==0 ? null : m_Arcs[0].Creator); }
        }

        /// <summary>
        /// Returns the area between a specific quadrant of a circle, and the Y-axis.
        /// By convention, the area of the north-west and south-west quadrants are
        /// returned as negative values, while the other two are positive (assuming
        /// that the circle is to the right of the Y-axis). This same convention is
        /// followed by <c>QuadVertex::GetCurveArea</c>.
        /// </summary>
        /// <param name="quadrant">The desired quadrant</param>
        /// <returns>The area (in square meters on the (projected) ground)</returns>
        internal double GetQuadrantArea(Quadrant quadrant)
        {
            double rm = m_Radius.Meters;

            if (quadrant==Quadrant.NE || quadrant==Quadrant.SE)
                return (rm * (m_Center.X + rm*MathConstants.PIDIV4));
            else
        		return -(rm * (m_Center.X - rm*MathConstants.PIDIV4));
        }

        /// <summary>
        /// Gets the most easterly position for this circle.
        /// </summary>
        /// <returns>The most easterly position</returns>
        internal IPosition GetEastPoint()
        {
            return new Position(m_Center.X + m_Radius.Meters, m_Center.Y);
        }

        /// <summary>
        /// Inserts this circle into the supplied index.
        /// </summary>
        /// <param name="index">The spatial index to add to (should be an instance of
        /// <see cref="EditingIndex"/>)</param>
        internal void AddToIndex(IEditSpatialIndex index)
        {
            EditingIndex cx = (index as EditingIndex);
            Debug.Assert(cx!=null);
            cx.AddCircle(this);
        }
    }
}
