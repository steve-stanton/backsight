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

namespace Backsight.Geometry
{
	/// <written by="Steve Stanton" on="14-JUN-2007" />
    /// <summary>
    /// The geometry for a circular arc.
    /// </summary>
    public class CircularArcGeometry : ILineGeometry, ICircularArcGeometry
    {
        #region Class data

        /// <summary>
        /// The location of the beginning of curve.
        /// </summary>
        private readonly IPointGeometry m_BC;

        /// <summary>
        /// The location of the end of curve.
        /// </summary>
        private readonly IPointGeometry m_EC;

        /// <summary>
        /// Definition of the circle that the curve lies on.
        /// </summary>
        private readonly ICircleGeometry m_Circle;

        /// <summary>
        /// True if curve is directed clockwise from BC to EC.
        /// </summary>
        private bool m_IsClockwise;

        #endregion

        #region Constructors

        public CircularArcGeometry(ICircleGeometry circle, IPosition bc, IPosition ec, bool isClockwise)
        {
            m_Circle = circle;
            m_BC = PointGeometry.Create(bc);
            m_EC = PointGeometry.Create(ec);
            m_IsClockwise = isClockwise;
        }

        public CircularArcGeometry(ICircleGeometry circle, IPointGeometry bc, IPointGeometry ec, bool isClockwise)
        {
            m_Circle = circle;
            m_BC = bc;
            m_EC = ec;
            m_IsClockwise = isClockwise;
        }

        /// <summary>
        /// Creates a new <c>CircularArcGeometry</c> where the radius of the circle is defined as
        /// distance from the BC to the supplied circle center.
        /// </summary>
        /// <param name="center">The center of the circle</param>
        /// <param name="bc">The start of the arc (the circle passes through this point)</param>
        /// <param name="ec">The end of the arc (assumed to coincide with the circle)</param>
        /// <param name="isClockwise">Is the arc directed clockwise?</param>
        public CircularArcGeometry(IPointGeometry center, IPosition bc, IPosition ec, bool isClockwise)
            : this(new CircleGeometry(center, new Length(BasicGeom.Distance(center, bc))), bc, ec, isClockwise)
        {
        }

        public CircularArcGeometry(ICircularArcGeometry g)
        {
            m_Circle = g.Circle;
            m_BC = g.BC;
            m_EC = g.EC;
            m_IsClockwise = g.IsClockwise;
        }

        #endregion

        public ICircleGeometry Circle
        {
            get { return m_Circle; }
        }

        public IPointGeometry BC
        {
            get { return m_BC; }
        }

        /// <summary>
        /// The same as the <c>BC</c> property (implements ILineGeometry)
        /// </summary>
        public IPointGeometry Start
        {
            get { return m_BC; }
        }

        public IPointGeometry EC
        {
            get { return m_EC; }
        }

        /// <summary>
        /// The same as the <c>EC</c> property (implements ILineGeometry)
        /// </summary>
        public IPointGeometry End
        {
            get { return m_EC; }
        }

        public bool IsClockwise
        {
            get { return m_IsClockwise; }
            set { m_IsClockwise = value; }
        }

        public void Render(ISpatialDisplay display, IDrawStyle style)
        {
            CircularArcGeometry.Render(this, display, style);
        }

        public static void Render(ICircularArcGeometry g, ISpatialDisplay display, IDrawStyle style)
        {
            style.Render(display, (IClockwiseCircularArcGeometry)g);
        }

        public IWindow Extent
        {
            get { return CircularArcGeometry.GetExtent(this); }
        }

        public static IWindow GetExtent(ICircularArcGeometry g)
        {
            // If the curve is a complete circle, just define it the easy way.
            if (g.BC.IsCoincident(g.EC))
                return CircleGeometry.GetExtent(g.Circle);

            IPosition bcp = g.BC;
            IPosition ecp = g.EC;
            IPosition centre = g.Circle.Center;

            // Initialize the window with the start location.
            Window win = new Window(bcp);

            // Expand using the end location
            win.Union(ecp);

            // If the curve is completely within one quadrant, we're done.
            QuadVertex bc = new QuadVertex(centre, bcp);
            QuadVertex ec = new QuadVertex(centre, ecp);

            Quadrant qbc = bc.Quadrant;
            Quadrant qec = ec.Quadrant;

            if (qbc == qec)
            {
                if (g.IsClockwise)
                {
                    if (bc.GetTanAngle() < ec.GetTanAngle())
                        return win;
                }
                else
                {
                    if (ec.GetTanAngle() < bc.GetTanAngle())
                        return win;
                }
            }

            // Get the window of the circle
            IWindow circle = CircleGeometry.GetExtent(g.Circle);

            // If the curve is anticlockwise, switch the quadrants for BC & EC
            if (!g.IsClockwise)
            {
                Quadrant temp = qbc;
                qbc = qec;
                qec = temp;
            }

            // Expand the window, depending on which quadrants the start &
            // end points fall in. The lack of breaks in the inner switches
            // is intentional (e.g. if start & end both fall in Quadrant.NorthEast,
            // the window we want is the complete circle, having checked above
            // for the case where the arc is JUST in Quadrant.NorthEast).

            // Define do-nothing values for the Union's below
            double wx = win.Min.X;
            double wy = win.Min.Y;

            if (qbc==Quadrant.NE)
            {
                switch (qec)
                {
                    case Quadrant.NE:
                        win.Union(wx, circle.Max.Y);
                        goto case Quadrant.NW;
                    case Quadrant.NW:
                        win.Union(circle.Min.X, wy);
                        goto case Quadrant.SW;
                    case Quadrant.SW:
                        win.Union(wx, circle.Min.Y);
                        goto case Quadrant.SE;
                    case Quadrant.SE:
                        win.Union(circle.Max.X, wy);
                        break;
                }
            }
            else if (qbc==Quadrant.SE)
            {
                switch (qec)
                {
                    case Quadrant.SE:
                        win.Union(circle.Max.X, wy);
                        goto case Quadrant.NE;
                    case Quadrant.NE:
                        win.Union(wx, circle.Max.Y);
                        goto case Quadrant.NW;
                    case Quadrant.NW:
                        win.Union(circle.Min.X, wy);
                        goto case Quadrant.SW;
                    case Quadrant.SW:
                        win.Union(wx, circle.Min.Y);
                        break;
                }
            }
            else if (qbc==Quadrant.SW)
            {
                switch (qec)
                {
                    case Quadrant.SW:
                        win.Union(wx, circle.Min.Y);
                        goto case Quadrant.SE;
                    case Quadrant.SE:
                        win.Union(circle.Max.X, wy);
                        goto case Quadrant.NE;
                    case Quadrant.NE:
                        win.Union(wx, circle.Max.Y);
                        goto case Quadrant.NW;
                    case Quadrant.NW:
                        win.Union(circle.Min.X, wy);
                        break;
                }
            }
            else if (qbc==Quadrant.NW)
            {
                switch (qec)
                {
                    case Quadrant.NW:
                        win.Union(circle.Min.X, wy);
                        goto case Quadrant.SW;
                    case Quadrant.SW:
                        win.Union(wx, circle.Min.Y);
                        goto case Quadrant.SE;
                    case Quadrant.SE:
                        win.Union(circle.Max.X, wy);
                        goto case Quadrant.NE;
                    case Quadrant.NE:
                        win.Union(wx, circle.Max.Y);
                        break;
                }
            }

            return win;
        }

        public ILength Distance(IPosition p)
        {
            return CircularArcGeometry.GetDistance(this, p);
        }

        public static ILength GetDistance(ICircularArcGeometry g, IPosition p)
        {
            return new Length(Math.Sqrt(MinDistanceSquared(g, p)));
        }

        static double MinDistanceSquared(ICircularArcGeometry g, IPosition p)
        {
            // If the position lies in the arc sector, the minimum distance
            // is given by the distance to the circle. Otherwise the minimum
            // distance is the distance to the closest end of the arc.

            if (IsInSector(g, p, 0.0))
            {
                double dist = BasicGeom.Distance(p, g.Circle.Center);
                double radius = g.Circle.Radius.Meters;
                return (dist-radius)*(dist-radius);
            }
            else
            {
                double d1 = BasicGeom.DistanceSquared(p, g.BC);
                double d2 = BasicGeom.DistanceSquared(p, g.EC);
                return Math.Min(d1, d2);
            }
        }

        /// <summary>
        /// Checks whether a location falls in the sector lying between the BC and EC
        /// of a circular arc. The only condition is that the location has an appropriate
        /// bearing with respect to the bearings of the BC & EC (i.e. the location does not
        /// necessarily lie ON the arc).
        /// </summary>
        /// <param name="pos">The position to check</param>
        /// <param name="lintol">Linear tolerance, in meters on the ground (locations that are
        /// beyond the BC or EC by up to this much will also be regarded as "in-sector").</param>
        /// <returns>True if position falls in the sector defined by this arc</returns>
        public static bool IsInSector(ICircularArcGeometry g, IPosition pos, double lintol)
        {
            // If the arc is a complete circle, it's ALWAYS in sector.
            if (IsCircle(g))
                return true;

            // Express the curve's start & end points in local system,
            // ordered clockwise.
            IPointGeometry start = g.First;
            IPointGeometry end = g.Second;

            // Get the centre of the circular arc.
            IPosition centre = g.Circle.Center;

            // If we have a linear tolerance, figure out the angular equivalent.
            if (lintol > MathConstants.TINY)
            {
                double angtol = lintol/g.Circle.Radius.Meters;
                return BasicGeom.IsInSector(pos, centre, start, end, angtol);
            }
            else
                return BasicGeom.IsInSector(pos, centre, start, end, 0.0);
        }

        /// <summary>
        /// Does a circular arc represent a complete circle (with a
        /// BC that's coincident with the EC).
        /// </summary>
        public static bool IsCircle(ICircularArcGeometry g)
        {
            return g.BC.IsCoincident(g.EC);
        }

        public IAngle StartBearing
        {
            get { return CircularArcGeometry.GetStartBearing(this); }
        }

        public static IAngle GetStartBearing(ICircularArcGeometry g)
        {
            return BasicGeom.Bearing(g.Circle.Center, g.First);
        }

        public IAngle SweepAngle
        {
            get { return CircularArcGeometry.GetSweepAngle(this); }
        }

        public static IAngle GetSweepAngle(ICircularArcGeometry g)
        {
            IPosition f = g.First;
            IPosition s = g.Second;
            Turn t = new Turn(g.Circle.Center, f);
            return t.GetAngle(s);
        }

        public IPointGeometry First
        {
            get { return CircularArcGeometry.GetFirstPosition(this); }
        }

        /// <summary>
        /// The first position of an arc, when reckoned clockwise.
        /// </summary>
        public static IPointGeometry GetFirstPosition(ICircularArcGeometry g)
        {
            return (g.IsClockwise ? g.BC : g.EC);
        }

        public IPointGeometry Second
        {
            get { return CircularArcGeometry.GetSecondPosition(this); }
        }

        /// <summary>
        /// The second position of an arc, when reckoned clockwise.
        /// </summary>
        public static IPointGeometry GetSecondPosition(ICircularArcGeometry g)
        {
            return (g.IsClockwise ? g.EC : g.BC);
        }

        /// <summary>
        /// The length of the line (on the map projection). (implements ILineGeometry)
        /// </summary>
        public ILength Length
        {
            get { return GetLength(this, null); }
        }

        // If the specified position isn't actually on the arc, the length is to the
        // position when it's projected onto the arc (i.e. the perpendicular position)
        public static ILength GetLength(ICircularArcGeometry g, IPosition asFarAs)
        {
            ICircleGeometry circle = g.Circle;
            double radius = circle.Radius.Meters;

            if (asFarAs==null)
                return new Length(radius * g.SweepAngle.Radians);

            // Express the position of the BC in a local coordinate system.
            IPosition c = circle.Center;
            QuadVertex bc = new QuadVertex(c, g.BC, radius);

            // Calculate the clockwise angle to the desired point.
            QuadVertex to = new QuadVertex(c, asFarAs, radius);
            double ang = to.Bearing.Radians - bc.Bearing.Radians;
            if (ang<0.0)
                ang += MathConstants.PIMUL2;

            // If the curve is actually anti-clockwise, take the complement.
            if (!g.IsClockwise)
                ang = MathConstants.PIMUL2 - ang;

            return new Length(radius * ang);
        }

        /// <summary>
        /// Generates an approximation of a circular arc.
        /// </summary>
        /// <param name="tol">The maximum chord-to-circumference distance.</param>
        /// <returns></returns>
        public static IPointGeometry[] GetApproximation(ICircularArcGeometry g, ILength tol)
        {
            // Get info about the circle the curve lies on.
            IPosition center = g.Circle.Center;
            double radius = g.Circle.Radius.Meters;

            // Determine the change in bearing which will satisfy the specified tolerance
            // (if no tolerance has been specified, arbitrarily use a tolerance of 1mm on the ground).
            double tolm = (tol.Meters > Double.Epsilon ? tol.Meters : 0.001);
            double dbear = Math.Acos((radius-tolm)/radius);

            IPointGeometry start = g.BC;
            IPointGeometry end = g.EC;
            bool iscw = g.IsClockwise;

            // Get the total angle subtended by the curve.
            Turn reft = new Turn(center, start);
            double totang = reft.GetAngle(end).Radians; // clockwise
            if (!iscw)
                totang = MathConstants.PIMUL2 - totang;

            // Figure out how many positions we'll generate
            int nv = (int)(totang/dbear); // truncate
            Debug.Assert(nv>=0);

            // Handle special case of very short arc.
            if (nv==0)
                return new IPointGeometry[] { start, end };

            // Sign the delta-bearing the right way.
	        if (!iscw)
                dbear = -dbear;

            // Get the initial bearing to the first position along the curve.
	        double curbear = reft.Bearing.Radians + dbear;

            // Append positions along the length of the curve.
            List<IPointGeometry> result = new List<IPointGeometry>(nv);
            result.Add(start);

            for (int i=0; i<nv; i++, curbear+=dbear)
            {
                IPosition p = BasicGeom.Polar(center, curbear, radius);
                result.Add(PointGeometry.Create(p));
            }

            result.Add(end);
            return result.ToArray();
        }

        /// <summary>
        /// Gets the position that is a specific distance from the start of this line.
        /// </summary>
        /// <param name="distance">The distance from the start of the line.</param>
        /// <param name="result">The position found</param>
        /// <returns>True if the distance is somewhere ON the line. False if the distance
        /// was less than zero, or more than the line length (in that case, the position
        /// found corresponds to the corresponding terminal point).</returns>
        public bool GetPosition(ILength distance, out IPosition result)
        {
            return GetPosition(this, distance, out result);
        }

        /// <summary>
        /// Gets the position that is a specific distance from the start of a circular arc.
        /// </summary>
        /// <param name="g">The geometry for the circular arc</param>
        /// <param name="distance">The distance from the start of the arc.</param>
        /// <param name="result">The position found</param>
        /// <returns>True if the distance is somewhere ON the arc. False if the distance
        /// was less than zero, or more than the arc length (in that case, the position
        /// found corresponds to the corresponding terminal point).</returns>
        public static bool GetPosition(ICircularArcGeometry g, ILength distance, out IPosition result)
        {
            // Check for invalid distances.
            double d = distance.Meters;
            if (d<0.0)
            {
                result = g.BC;
                return false;
            }

            double clen = g.Length.Meters; // Arc length
            if (d>clen)
            {
                result = g.EC;
                return false;
            }

            // Check for limiting values (if you don't do this, minute
            // roundoff at the BC & EC can lead to spurious locations).
            // (although it's possible to use TINY here, use 1 micron
            // instead, since we can't represent position any better
            // than that).

            if (d<0.000001)
            {
                result = g.BC;
                return true;
            }

            if (Math.Abs(d-clen)<0.000001)
            {
                result = g.EC;
                return true;
            }

            // Get the bearing of the BC
            ICircleGeometry circle = g.Circle;
            IPosition c = circle.Center;
            double radius = circle.Radius.Meters;
            double bearing = BasicGeom.Bearing(c, g.BC).Radians;

            // Add the angle that subtends the required distance (or
            // subtract if the curve goes anti-clockwise).
            if (g.IsClockwise)
                bearing += (d/radius);
            else
                bearing -= (d/radius);

            // Figure out the required point from the new bearing.
            result = BasicGeom.Polar(c, bearing, radius);
            return true;
        }
    }
}
