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
    /// <written by=Steve Stanton" on="01-AUG-2007" />
    /// <summary>
    /// The geometry for a circular arc.
    /// </summary>
    [Serializable]
    class ArcGeometry : LineGeometry, ICircularArcGeometry, ISectionBase
    {
        #region Class data

        /// <summary>
        /// Definition of the circle that this arc lies on.
        /// </summary>
        private Circle m_Circle;

        /// <summary>
        /// True if curve is directed clockwise from BC to EC.
        /// </summary>
        private bool m_IsClockwise;

        #endregion

        #region Constructors

        internal ArcGeometry(Circle c, ITerminal bc, ITerminal ec, bool isClockwise )
            : base(bc, ec)
        {
            m_Circle = c;
            m_IsClockwise = isClockwise;
        }

        #endregion

        #region ICircularArcGeometry Members

        public IPointGeometry BC
        {
            get { return Start; }
        }

        public IPointGeometry EC
        {
            get { return End; }
        }

        public bool IsClockwise
        {
            get { return m_IsClockwise; }
            set { m_IsClockwise = value; }
        }

        #endregion

        public override ILength Length
        {
            get { return CircularArcGeometry.GetLength(this, null); }
        }

        /// <summary>
        /// The circle the arc falls on.
        /// </summary>
        public ICircleGeometry Circle
        {
            get { return m_Circle; }
        }

        /// <summary>
        /// The bearing from the center of circle to the start of the arc
        /// </summary>
        public IAngle StartBearing
        {
            get { return CircularArcGeometry.GetStartBearing(this); }
        }

        /// <summary>
        /// The angular length of the arc
        /// </summary>
        public IAngle SweepAngle
        {
            get { return CircularArcGeometry.GetSweepAngle(this); }
        }

        public IPointGeometry First
        {
            get { return CircularArcGeometry.GetFirstPosition(this); }
        }

        public IPointGeometry Second
        {
            get { return CircularArcGeometry.GetSecondPosition(this); }
        }

        public override IWindow Extent
        {
            get { return CircularArcGeometry.GetExtent(this); }
        }

        public override ILength Distance(IPosition point)
        {
            return CircularArcGeometry.GetDistance(this, point);
        }

        internal override uint IntersectSegment(IntersectionResult results, ILineSegmentGeometry that)
        {
            return IntersectionHelper.Intersect(results, that, (ICircularArcGeometry)this);
        }

        internal override uint IntersectMultiSegment(IntersectionResult results, IMultiSegmentGeometry that)
        {
            return IntersectionHelper.Intersect(results, that, (ICircularArcGeometry)this);
        }

        internal override uint IntersectArc(IntersectionResult results, ICircularArcGeometry that)
        {
            return IntersectionHelper.Intersect(results, that, (ICircularArcGeometry)this);
        }

        internal override uint IntersectCircle(IntersectionResult results, ICircleGeometry that)
        {
            return IntersectionHelper.Intersect(results, (ICircularArcGeometry)this, that);
        }

        /// <summary>
        /// Gets the position that is a specific distance from the start of this line.
        /// </summary>
        /// <param name="dist">The distance from the start of the line.</param>
        /// <param name="result">The position found</param>
        /// <returns>True if the distance is somewhere ON the line. False if the distance
        /// was less than zero, or more than the line length (in that case, the position
        /// found corresponds to the corresponding terminal point).</returns>
        internal override bool GetPosition(ILength distance, out IPosition result)
        {
            // Check for invalid distances.
            double d = distance.Meters;
            if (d<0.0)
            {
                result = Start;
                return false;
            }

            double clen = Length.Meters; // Curve length
            if (d>clen)
            {
                result = End;
                return false;
            }

            // Check for limiting values (if you don't do this, minute
            // roundoff at the BC & EC can lead to spurious locations).
            // (although it's possible to use TINY here, use 1 micron
            // instead, since we can't represent position any better
            // than that).

            if (d<0.000001)
            {
                result = Start;
                return true;
            }

            if (Math.Abs(d-clen)<0.000001)
            {
                result = End;
                return true;
            }

            // Get the bearing of the BC
            ICircleGeometry circle = Circle;
            IPosition c = circle.Center;
            double radius = circle.Radius.Meters;
            double bearing = Geom.Bearing(c, BC).Radians;

            // Add the angle that subtends the required distance (or
            // subtract if the curve goes anti-clockwise).
            if (m_IsClockwise)
                bearing += (d/radius);
            else
                bearing -= (d/radius);

            // Figure out the required point from the new bearing.
            result = Geom.Polar(c, bearing, radius);
            return true;
        }

        // If the specified position isn't actually on the arc, the length is to the
        // position when it's projected onto the arc (i.e. the perpendicular position)
        internal override ILength GetLength(IPosition asFarAs)
        {
            return CircularArcGeometry.GetLength(this, asFarAs);
        }

        /// <summary>
        /// Gets the orientation point for a line. This is utilized to form
        /// network topology at the ends of a topological line.
        /// </summary>
        /// <param name="fromStart">True if the orientation from the start of the line is
        /// required. False to get the end orientation.</param>
        /// <param name="crvDist">Orientation distance for circular arcs (if a value of
        /// zero (or less) is specified, a "reasonable" orientation distance will be used -
        /// currently 5 metres on the ground).
        /// </param>
        /// <returns>The orientation point.</returns>
        internal override IPosition GetOrient(bool fromStart, double crvDist)
        {
            // Point to the BC or EC, depending on what we are orienting.
            IPosition loc = (fromStart ? Start : End);

            // Get the bearing of the location from the center of the circular arc
            IPointGeometry centre = Circle.Center;
            double bearing = Geom.Bearing(centre, loc).Radians;

            // Get the distance to the orientation point. It should not be
            // any further than the length of the arc. We use a fixed
            // orientation distance of 5 metres on the ground for now.

            double dist;
            if (crvDist < Constants.TINY)
                dist = Math.Min(5.0, Length.Meters);
            else
                dist = Math.Min(crvDist, Length.Meters);

            // If we are coming from the end of the arc, use a negated
            // distance to get the direction right.
            if (!fromStart)
                dist = -dist;

            // Add the angle that subtends the orientation distance (or
            // subtract if the arc goes anti-clockwise).
            double radius = Circle.Radius.Meters;

            if (m_IsClockwise)
                bearing += (dist/radius);
            else
                bearing -= (dist/radius);

            // Figure out the orientation point from the new bearing.
            return Geom.Polar(centre, bearing, radius);
        }

        internal override void Render(ISpatialDisplay display, IDrawStyle style)
        {
            // At scales larger than 1:1000, it may be advisable to draw an approximation (since
            // the drawing methods tend to reveal small non-existent glitches at larger scales)...

            // Draw an approximation of the curve (such that the chord-to-circumference
            // distance does not exceed 0.2mm at draw scale).
            /*
            ILength tol = new Length(display.MapScale * 0.0002);
            IPointGeometry[] pts = CircularArcGeometry.GetApproximation(this, tol);
            new LineStringGeometry(pts).Render(display, style);
             */
            CircularArcGeometry.Render(this, display, style);
        }

        /// <summary>
        /// The line geometry that corresponds to a section of this line.
        /// </summary>
        /// <param name="s">The required section</param>
        /// <returns>The corresponding geometry for the section (<b>not</b> an instance of
        /// <see cref="SectionGeometry"/>)</returns>
        public LineGeometry Section(ISection s) // implements ISectionBase
        {
            return new ArcGeometry(m_Circle, s.From, s.To, m_IsClockwise);
        }

        /// <summary>
        /// Gets geometric info for this geometry. For use during the formation
        /// of <c>Polygon</c> objects.
        /// </summary>
        /// <param name="window">The window of the geometry</param>
        /// <param name="area">The area (in square meters) between the geometry and the Y-axis.</param>
        /// <param name="length">The length of the geometry (in meters on the (projected) ground).</param>
        internal override void GetGeometry(out IWindow win, out double area, out double length)
        {
            win = Extent;
            length = Length.Meters;
            area = Area;
        }

        /// <summary>
        /// The area between this arc and the Y-axis.
        /// </summary>
        double Area
        {
            get
            {
                double area;

	            // If the arc is a complete circle, the area is the area
	            // to the left of the two western quadrants.
                if (IsCircle)
                {
                    area = m_Circle.GetQuadrantArea(Quadrant.NW) +
                           m_Circle.GetQuadrantArea(Quadrant.SW);
                    return (m_IsClockwise ? area : -area);
                }

                // Express the BC & EC as quadrant coordinates, ordered clockwise.
                IPosition center = m_Circle.Center;
                double radius = m_Circle.Radius.Meters;
                QuadVertex start, end;

                if (m_IsClockwise)
                {
		            start = new QuadVertex(center, BC, radius);
		            end   = new QuadVertex(center, EC, radius);
	            }
	            else
                {
		            end   = new QuadVertex(center, BC, radius);
		            start = new QuadVertex(center, EC, radius);
	            }

                // Note start and end quadrants.
                Quadrant qs = start.Quadrant;
                Quadrant qe = end.Quadrant;

                // If the start and the end are in the same quadrant, and the
                // arc does not go all the way round, the area is the area
                // for the second point, less the area for the first point.

                if (qs==qe && start.GetTanAngle()<end.GetTanAngle())
                {
		            area = end.CurveArea - start.CurveArea;
                    return (m_IsClockwise ? area : -area);
	            }

                // Initialize total area with the portion from the start
                // point, to the end of the quadrant it falls in.

                double totarea = m_Circle.GetQuadrantArea(qs) - start.CurveArea;
	            if (!m_IsClockwise)
                    totarea = -totarea;

                // Step through any quadrants prior to the end point's quadrant.
	            if (m_IsClockwise)
                {
		            for ( Quadrant quad=QuadVertex.NextQuadrant(qs);
			              quad!=qe;
			              totarea += m_Circle.GetQuadrantArea(quad),
                          quad=QuadVertex.NextQuadrant(quad))
                        ;
	            }
	            else
                {
                    for (Quadrant quad=QuadVertex.NextQuadrant(qs);
			              quad!=qe;
			              totarea -= m_Circle.GetQuadrantArea(quad),
                          quad=QuadVertex.NextQuadrant(quad))
                        ;
	            }

                // Add on the curve area for the quadrant that contains the end point.
            	area = end.CurveArea;
	            if (m_IsClockwise)
		            totarea += area;
	            else
		            totarea -= area;

	            return totarea;
            }
        }

        /// <summary>
        /// Does this curve represents a complete circle?
        /// </summary>
        bool IsCircle
        {
            get { return BC.IsCoincident(EC); }
        }

        /// <summary>
        /// Gets the most easterly position for this arc.
        /// </summary>
        /// <returns>The most easterly position</returns>
        internal override IPosition GetEastPoint()
        {
            // If the arc is a complete circle, just work it out the easy way.
            if (IsCircle)
                return m_Circle.GetEastPoint();

            // Determine where the east point is. It could be either the
            // BC, the EC, or the most easterly point of the circle.

            IPosition start = First;
            IPosition end = Second;
            char choice = (start.X > end.X ? 'S' : choice = 'E');

            // But hold on. If the arc passes from the northeast to southeast
            // quadrant, we want the east point of the circle.

            IPosition c = m_Circle.Center;
            QuadVertex vs = new QuadVertex(c, start);
            QuadVertex ve = new QuadVertex(c, end);

            Quadrant qs = vs.Quadrant;
            Quadrant qe = ve.Quadrant;

            // If the BC & EC are in the same quadrant, the only way the
            // circle point can apply is if it goes all the way round.
            if (!(qs==qe && vs.GetTanAngle()<ve.GetTanAngle()))
            {
                // Determine whether the circle point applies.
                switch (qs)
                {
                    case Quadrant.NE:
                    {
                        choice='C';
                        break;
                    }

                    case Quadrant.SE:
                        break;

                    case Quadrant.SW:
                    {
                        if (qe==Quadrant.SE)
                            choice = 'C';

                        break;
                    }

                    case Quadrant.NW:
                    {
                        if (qe!=Quadrant.NE)
                            choice = 'C';

                        break;
                    }
                }
            }

            if (choice=='S')
                return start;
            else if (choice=='E')
                return end;

            Debug.Assert(choice=='C');
            return m_Circle.GetEastPoint();            
        }


        /// <summary>
        /// Determines which side of a line a horizontal line segment lies on.
        /// Used in point in polygon.
        /// </summary>
        /// <param name="hr">The horizontal line segment</param>
        /// <returns>Code indicating the position of the horizontal segment with respect to this line.
        /// Side.Left if the horizontal segment is to the left of this line; Side.Right if to the
        /// right of this line; Side.Unknown if the side cannot be determined (this line is
        /// horizontal).
        /// </returns>
        internal override Side GetSide(HorizontalRay hr)
        {
            // Express the end of the horizontal in a local system with
            // the same origin as the circle this arc lies on.
            IPosition center = m_Circle.Center;
            QuadVertex pos = new QuadVertex(center, hr.End);
            Quadrant quad = pos.Quadrant;

            // Assign the side code, assuming the arc is clockwise.
            Side result = (quad==Quadrant.NE || quad==Quadrant.SE ? Side.Right : Side.Left);

            // If the horizontal is apparently to the right of the curve,
            // but the location in question coincides with the extreme
            // north point of the circle, reverse the side (it's OK in
            // the extreme south).

            if (result==Side.Right)
            {
                double maxnorth = center.Y + Circle.Radius.Meters;
                if (Math.Abs(maxnorth - hr.Y) < Constants.TINY)
                    result = Side.Left;
            }

            // If the arc actually goes anti-clockwise, reverse the side.
            if (!m_IsClockwise)
                result = (result==Side.Left ? Side.Right : Side.Left);

            return result;
        }

        /// <summary>
        /// Cuts back a horizontal line segment to the closest intersection with this line.
        /// Used in point in polygon.
        /// </summary>
        /// <param name="s">Start of horizontal segment.</param>
        /// <param name="e">End of segment (will be modified if segment intersects this line)</param>
        /// <param name="status">Return code indicating whether an error has arisen (returned
        /// as 0 if no error).</param>
        /// <returns>True if the horizontal line was cut back.</returns>
        internal override bool GetCloser(IPointGeometry s, ref PointGeometry e, out uint status)
        {
            status = 0;

            // Get the window of the circle that this curve lies on.
            IWindow cwin = m_Circle.Extent;

            // Get a window for the horizontal segment.
            IWindow segwin = new Window(s, e);

            // Return if the windows don't overlap.
            if (!cwin.IsOverlap(segwin))
                return false;

            // Return if the the segment is to the west of the circle's window.
            if (segwin.Max.X < cwin.Min.X)
                return false;

            // Represent the horizontal segment in a class of its own
            HorizontalRay hseg = new HorizontalRay(s, e.X-s.X);
            if (!hseg.IsValid)
            {
                status = 1;
                return false;
            }

            // Locate up to two intersections of the horizontal segment with the circle.
            IPosition x1=null;
            IPosition x2=null;
            uint nx = hseg.Intersect(m_Circle, ref x1, ref x2);

            // Return if no intersections
            if (nx==0)
                return false;

            // Return if this arc curve is a complete circle.
            if (this.IsCircle)
                return false;

            // Check whether the first intersection lies on this arc. If
            // so, update the end of segment, and return.
            if (CircularArcGeometry.IsInSector(this, x1, 0.0))
            {
                e = new PointGeometry(x1);
                return true;
            }

            // If we got two intersections with the circle, check the
            // second intersection as well.
            if (nx==2 && CircularArcGeometry.IsInSector(this, x2, 0.0))
            {
                e = new PointGeometry(x2);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Loads a list of positions with data for this line.
        /// </summary>
        /// <param name="positions">The list to append to</param>
        /// <param name="reverse">Should the data be appended in reverse order?</param>
        /// <param name="wantFirst">Should the first position be appended? (last if <paramref name="reverse"/> is true)</param>
        /// <param name="arcTol">Tolerance for approximating circular arcs (the maximum chord-to-circumference distance).</param>
        internal override void AppendPositions(List<IPosition> positions, bool reverse, bool wantFirst, ILength arcTol)
        {
            IPointGeometry[] approx = CircularArcGeometry.GetApproximation(this, arcTol);
            MultiSegmentGeometry.AppendPositions(approx, positions, reverse, wantFirst);
        }

        internal override uint Intersect(IntersectionResult results)
        {
            return results.IntersectArc(this);
        }
    }
}
