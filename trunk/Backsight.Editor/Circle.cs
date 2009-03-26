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
using System.Diagnostics;

using Backsight.Geometry;
using Backsight.Editor.Xml;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="19-SEP-1997" />
    /// <summary>
    /// The definition of a circle
    /// </summary>
    /// <seealso cref="Backsight.Geometry.CircleGeometry"/>
    class Circle : Content, ISpatialObject, ICircleGeometry, IFeatureDependent
    {
        #region Class data

        /// <summary>
        /// The radius of the circle, in meters
        /// </summary>
        private double m_Radius;

        /// <summary>
        /// The center of the circle. This may be quite remote from the main body of the map.
        /// </summary>
        private PointFeature m_Center;

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
        /// <param name="radius">The radius of the circle, in meters</param>
        internal Circle(PointFeature center, double radius)
        {
            m_Center = center;
            m_Radius = radius;
            m_Arcs = new List<ArcFeature>();
        }

        /// <summary>
        /// Constructor for use during deserialization
        /// </summary>
        /// <param name="op">The editing operation creating the circle</param>
        /// <param name="t">The serialized version of this circle</param>
        internal Circle(Operation op, CircleType t)
        {
            m_Center = op.MapModel.Find<PointFeature>(t.Center);
            m_Radius = t.Radius;
            m_Arcs = new List<ArcFeature>();

            // Not sure whether this is the best place to do this (seems inconsistent
            // that the other constructor doesn't do it, though I can see the sense if
            // we need to create adhoc circles for some reason).
            m_Center.AddReference(this);
        }

        #endregion

        #region ISpatialObject Members

        /// <summary>
        /// Value denoting the spatial object type.
        /// </summary>
        public SpatialType SpatialType
        {
            get { return SpatialType.Line; }
        }

        /// <summary>
        /// Draws this object on the specified display
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The drawing style</param>
        public void Render(ISpatialDisplay display, IDrawStyle style)
        {
            CircleGeometry.Render(this, display, style);
        }

        /// <summary>
        /// The spatial extent of this object.
        /// </summary>
        public IWindow Extent
        {
            get { return CircleGeometry.GetExtent(this); }
        }

        /// <summary>
        /// Calculates the distance from the perimeter of this circle to the specified position.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public ILength Distance(IPosition point)
        {
            return CircleGeometry.Distance(this, point);
        }

        #endregion

        #region IFeatureDependent Members

        /// <summary>
        /// Performs any processing that needs to be done just before the position of
        /// a referenced feature is changed.
        /// </summary>
        /// <param name="f">The feature that is about to be changed (a feature that
        /// the <c>IFeatureDependent</c> is dependent on)</param>
        /// <returns>
        /// True if the feature was removed from spatial index. False if the
        /// spatial index does not exist.
        /// </returns>
        public bool OnPreMove(Feature f)
        {
            IEditSpatialIndex index = f.MapModel.EditingIndex;
            if (index == null)
                return false;

            index.Remove(this);
            return true;
        }

        /// <summary>
        /// Performs any processing that needs to be done after the position of
        /// a referenced feature has been changed.
        /// </summary>
        /// <param name="f">The feature that has just been changed (a feature that
        /// the <c>IFeatureDependent</c> is dependent on)</param>
        /// <returns>
        /// True if the feature was removed from spatial index. False if the
        /// spatial index does not exist.
        /// </returns>
        public bool OnPostMove(Feature f)
        {
            IEditSpatialIndex index = f.MapModel.EditingIndex;
            if (index == null)
                return false;

            index.Add(this);
            return true;
        }

        /// <summary>
        /// Adds references to the features that this dependent is dependent on.
        /// </summary>
        public void AddReferences()
        {
            m_Center.AddReference(this);
        }

        #endregion

        #region ICircleGeometry Members

        /// <summary>
        /// The position of the center of the circle.
        /// </summary>
        public IPointGeometry Center
        {
            get { return m_Center; }
        }

        /// <summary>
        /// The radius of the circle, in meters
        /// </summary>
        public double Radius
        {
            get { return m_Radius; }
            internal set { m_Radius = value; }
        }

        #endregion

        // Not sure about this. Should it be disallowed?
        internal void ChangeRadius(ArcFeature arc, double newRadius)
        {
            if (Math.Abs(m_Radius - newRadius) > Constants.TINY)
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
            if (quadrant==Quadrant.NE || quadrant==Quadrant.SE)
                return (m_Radius * (m_Center.X + m_Radius * MathConstants.PIDIV4));
            else
                return -(m_Radius * (m_Center.X - m_Radius * MathConstants.PIDIV4));
        }

        /// <summary>
        /// Gets the most easterly position for this circle.
        /// </summary>
        /// <returns>The most easterly position</returns>
        internal IPosition GetEastPoint()
        {
            return new Position(m_Center.X + m_Radius, m_Center.Y);
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

        /// <summary>
        /// Obtains a list of circles that exist in two lists
        /// </summary>
        /// <param name="a">The first list</param>
        /// <param name="b">The second list</param>
        /// <returns>The circles that exist in both lists (the test is based simply
        /// on reference equality)</returns>
        internal static List<Circle> GetCommonCircles(List<Circle> a, List<Circle> b)
        {
            List<Circle> result = new List<Circle>();

            foreach (Circle c in a)
            {
                if (b.Contains(c))
                    result.Add(c);
            }

            return result;
        }

        /// <summary>
        /// Updates the definition of this circle.
        /// </summary>
        /// <param name="center">The point at the center of the circle.</param>
        /// <param name="radius">The radius of the circle, on the ground, in meters</param>
        internal void MoveCircle(PointFeature center, double radius)
        {
            // Remove this circle (and attached arcs) from the spatial index
            foreach (ArcFeature a in m_Arcs)
                a.PreMove();

            m_Center.MapModel.EditingIndex.RemoveCircle(this);

            // If the center location is changing then ensure that
            // the old location no longer refers to this circle,
            // and ensure the new one does.

            // 08-DEC-99: Note that there appears to be inconsistency
            // in the cross-referencing of the center location to the
            // circle. Some places seem to do it, while other don't.
            // The extra reference shouldn't hurt, and at some stage
            // in the future, it would be good to enforce this.

            if (!Object.ReferenceEquals(m_Center, center))
            {
                m_Center.CutReference(this);
                m_Center = center;
                m_Center.AddReference(this);
            }

            // Change the radius
            m_Radius = radius;

            // Re-index this circle and any attached arcs (this should also
            // mark the operations that created the arcs for rollforward).
            m_Center.MapModel.EditingIndex.AddCircle(this);
            foreach (ArcFeature a in m_Arcs)
                a.PostMove();
        }

        /// <summary>
        /// Checks whether this circle is referenced to arcs that terminaye at
        /// a specific point. This excludes arcs that correspond to the whole circle.
        /// </summary>
        /// <param name="p">The point to look for</param>
        /// <returns>True if an incident arc was found.</returns>
        internal bool HasArcsAt(PointFeature p)
        {
            // A location to check has to be specified!
            if (p==null)
                return false;

            // Loop through each arc (including inactive ones).
            foreach (ArcFeature a in m_Arcs)
            {
                // Skip if the arc represents the whole circle.
                if (a.Geometry.IsCircle)
                    continue;

                // Check whether either end of the arc coincides with
                // the check location.
                if (p.IsCoincident(a.StartPoint) || p.IsCoincident(a.EndPoint))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Writes the attributes of this class.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteAttributes(XmlContentWriter writer)
        {
            // When writing out the radius, round if off to 10 decimal places (to
            // avoid strings with spurious trailing digits, like "237.13320000000002")

            writer.WriteFeatureReference("Center", m_Center);
            writer.WriteDouble("Radius", Math.Round(m_Radius, 10));
        }

        /// <summary>
        /// Defines the attributes of this content
        /// </summary>
        /// <param name="reader">The reading tool</param>
        public override void ReadAttributes(XmlContentReader reader)
        {
            m_Center = reader.ReadFeatureByReference<PointFeature>("Center");
            m_Radius = reader.ReadDouble("Radius");
        }

        /// <summary>
        /// The first arc associated with this circle (null if no arcs are currently
        /// associated with this circle).
        /// </summary>
        internal ArcFeature FirstArc
        {
            get { return (m_Arcs.Count > 0 ? m_Arcs[0] : null); }
        }
    }
}
