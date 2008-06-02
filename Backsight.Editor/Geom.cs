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

namespace Backsight.Editor
{
    public class Geom : BasicGeom
    {
        
        /// <summary>
        /// Gets the position ratio on a line for a point that intersects the line at 90 degrees.
        /// The perpendicular point does not necessarily lie on the line.
        /// 
        /// Most of this code is scabbed from <c>GetPerpendicular</c>.
        /// </summary>
        /// <param name="x">The point to drop to the vector.</param>
        /// <param name="y"></param>
        /// <param name="xs">Start of vector.</param>
        /// <param name="ys"></param>
        /// <param name="xe">End of vector.</param>
        /// <param name="ye"></param>
        /// <returns></returns>
        internal static double GetPositionRatio(double x, double y, double xs, double ys, double xe, double ye)
        {
	        double dxseg,dyseg;	// Deltas of the vector
	        double dxpt,dypt;	// Deltas of point to start of vector
	        double lensq;		// Length squared of vector
	        double posrat;		// Position ratio

	        dxseg = xe-xs;
	        dyseg = ye-ys;

	        dxpt = x-xs;
	        dypt = y-ys;

            // Get position ratio of the perpendicular point.
	        lensq = dxseg*dxseg + dyseg*dyseg;
	        posrat = ((dxseg*dxpt)+(dyseg*dypt)) / lensq;

	        return posrat;
        }

        /// <summary>
        /// Finds the intersection (if any) between 2 line segments. 
        /// </summary>
        /// <param name="xk">Start of segment KL</param>
        /// <param name="yk"></param>
        /// <param name="xl">End of segment KL</param>
        /// <param name="yl"></param>
        /// <param name="xm">Start of segment MN</param>
        /// <param name="ym"></param>
        /// <param name="xn">End of segment MN</param>
        /// <param name="yn"></param>
        /// <param name="xi">The intersection (if any).</param>
        /// <param name="yi"></param>
        /// <param name="online">TRUE if the intersection must lie on both lines (default)</param>
        /// <returns>
        /// The type of intersection:
        /// 
        /// 0  =>   no intersection (in that case, the supplied parameters for the intersection are
        ///         left unchanged). Segments that are parallel (e.g. coincident) will return this value.
        ///	-1 =>   clean intersection
        ///	
        /// Virtual intersections may also be returned if the intersection must lie ON the line. A virtual
        /// intersect does not technically exist. However, if either line was projected by up to 0.1mm on
        /// the ground, you'd have a intersection.
        /// 
        /// 1 => K returned
        ///	2 => L returned
        ///	3 => M returned
        ///	4 => N returned
        /// </returns>
        public static int CalcIntersect( double xk, double yk
			                           , double xl, double yl
				                       , double xm, double ym
				                       , double xn, double yn
				                       , out double xi
				                       , out double yi
				                       , bool online )
        {
            xi = yi = 0.0;

            // Get deltas of the 2 line segments.
            double xlk = xl-xk;
            double ylk = yl-yk;
            double xnm = xn-xm;
            double ynm = yn-ym;
            double xmk = xm-xk;
            double ymk = ym-yk;

            double det = xnm*ylk - ynm*xlk;

            // Check if lines are parallel.
            if ( Math.Abs(det) < Constants.TINY )
                return 0;

            // The infinite lines intersect somewhere. Get position ratios.
            // and return if intersection is not within the line segments.
	        double detinv = 1.0/det;

            /*
	            Relax the position ratio test a little bit (see code below) ...

	            double s = (xnm*ymk - ynm*xmk)*detinv;
	            if ( online && (s<0.0 || s>1.0) ) return false;

	            double t = (xlk*ymk - ylk*xmk)*detinv;
	            if ( online && (t<0.0 || t>1.0) ) return false;

                // Get the intersection.
	            xi = xk + xlk*s;
	            yi = yk + ylk*s;
	            return true;
            */

	        if ( online )
            {
		        // Arbitrary values that should pick up any intersection
		        // close to an end point (given that we'll take an end only
		        // if it's in the micron range).

		        const double MINRAT = -0.0001;
		        const double MAXRAT =  1.0001;

		        double s = (xnm*ymk - ynm*xmk)*detinv;
		        if ( s<MINRAT || s>MAXRAT ) return 0;

		        double t = (xlk*ymk - ylk*xmk)*detinv;
		        if ( t<MINRAT || t>MAXRAT ) return 0;

		        // If the intersection is definitely on the line, just return
		        // the intersection. Otherwise return it only if it's within
		        // the coordinate precision.

		        if ( s>=0.0 && s<=1.0 && t>=0.0 && t<=1.0 )
                {
			        xi = xk + xlk*s;
			        yi = yk + ylk*s;
			        return -1;
		        }

		        // Technically, the intersection does not exist. However, if
		        // it falls within tolerance of any end point, return that
		        // position as the intersection.

        		const double tol = Constants.XYRES;

		        double xt = xk + xlk*s;
		        double yt = yk + ylk*s;

		        if ( Math.Abs(xt-xk) < tol && Math.Abs(yt-yk) < tol )
                {
			        xi = xk;
			        yi = yk;
			        return 1;
		        }

		        if ( Math.Abs(xt-xl) < tol && Math.Abs(yt-yl) < tol )
                {
			        xi = xl;
			        yi = yl;
			        return 2;
		        }

		        if ( Math.Abs(xt-xm) < tol && Math.Abs(yt-ym) < tol )
                {
			        xi = xm;
			        yi = ym;
			        return 3;
		        }

		        if ( Math.Abs(xt-xn) < tol && Math.Abs(yt-yn) < tol )
                {
			        xi = xn;
			        yi = yn;
			        return 4;
		        }

		        return 0;
	        }
	        else
            {
		        double s = (xnm*ymk - ynm*xmk)*detinv;
		        xi = xk + xlk*s;
		        yi = yk + ylk*s;
		        return -1;
	        }
        }

        /// <summary>
        /// Finds the intersection of 2 bearings.
        /// </summary>
        /// <param name="x1">X-position for 1st bearing.</param>
        /// <param name="y1">Y-position for 1st bearing.</param>
        /// <param name="b1">First bearing, in radians.</param>
        /// <param name="x2">X-position for 2nd bearing.</param>
        /// <param name="y2">Y-position for 2nd bearing.</param>
        /// <param name="b2">Second bearing, in radians.</param>
        /// <param name="xi">X-position of intersection (if any).</param>
        /// <param name="yi">Y-position of intersection (if any).</param>
        /// <returns>True if intersection was defined (if not, the supplied parameters
        /// for the intersection are left unchanged).
        /// </returns>
        static bool Intersect( double x1, double y1, double b1
				             , double x2, double y2, double b2
				             , ref double xi
				             , ref double yi )
        {
            // The equation of each line is given in parametric form as:
            //
            //		x = xo + f * r
            //		y = yo + g * r
            //
            // where	xo,yo is the from point
            // and		f = sin(bearing)
            // and		g = cos(bearing)
            // and		r is the position ratio

            double cosb1 = Math.Cos(b1);
            double sinb1 = Math.Sin(b1);
            double cosb2 = Math.Cos(b2);
            double sinb2 = Math.Sin(b2);

            double f1g2 = sinb1 * cosb2;
            double f2g1 = sinb2 * cosb1;
            double det = f2g1 - f1g2;

            // Check whether lines are parallel.
            if (Math.Abs(det) < Constants.TINY)
                return false;

            // Work out the intersection.

	        double dx = x2 - x1;
	        double dy = y2 - y1;

	        double prat = ( sinb2*dy - cosb2*dx ) / det;

	        xi = x1 + sinb1*prat;
	        yi = y1 + cosb1*prat;

	        return true;
        }

        /// <summary>
        /// Intersects a circle with another circle.
        /// </summary>
        /// <param name="centre1">The centre of the 1st circle.</param>
        /// <param name="radius1">The radius of the 1st circle.</param>
        /// <param name="centre2">The centre of the 2nd circle.</param>
        /// <param name="radius2">The radius of the 2nd circle.</param>
        /// <param name="x1">The 1st intersection (if any).</param>
        /// <param name="x2">The 2nd intersection (if any).</param>
        /// <returns>The number of intersections found (0, 1, or 2).</returns>
        internal static uint IntersectCircles( IPosition centre1
				                             , double radius1
				                             , IPosition centre2
				                             , double radius2
				                             , out IPosition x1
				                             , out IPosition x2 )
        {
	        // Initialize both intersections.
            x1 = x2 = null;

	        // Pull out the XY's for the 2 centres.
	        double xc1 = centre1.X;
	        double yc1 = centre1.Y;
	        double xc2 = centre2.X;
	        double yc2 = centre2.Y;

	        // Get distance (squared) between the 2 centres.
	        double dx = xc2 - xc1;
	        double dy = yc2 - yc1;
	        double distsq = dx*dx + dy*dy;

	        // Return if the two circles have the same centre.
	        if ( distsq < Constants.TINY )
                return 0;

	        // We'll need the radii squared
	        double rsq1 = radius1 * radius1;
	        double rsq2 = radius2 * radius2;

	        // Now some mathematical magic I got out a book.
	        double delrsq = rsq2 - rsq1;
	        double sumrsq = rsq1 + rsq2;
	        double root = 2.0*sumrsq*distsq - distsq*distsq - delrsq*delrsq;

	        // Check for no intersection.
	        if ( root < Constants.TINY )
                return 0;

	        // We have at least one intersection.
	        double dstinv = 0.5 / distsq;
	        double scl = 0.5 - delrsq*dstinv;
	        double x = dx*scl + xc1;
	        double y = dy*scl + yc1;

	        // Is it tangential?
	        if ( root < Constants.TINY )
            {
		        x1 = new Position(x,y);
		        return 1;
	        }

	        // Figure out 2 intersections.
	        root = dstinv * Math.Sqrt(root);
	        double xfac = dx*root;
	        double yfac = dy*root;

            x1 = new Position(x-yfac, y+xfac);
            x2 = new Position(x+yfac, y-xfac);
	        return 2;
        }

        /// <summary>
        /// Intersects a line segment with a circle.
        /// </summary>
        /// <param name="centre">Position of the circle's centre.</param>
        /// <param name="radius">Radius of the circle.</param>
        /// <param name="start">Start of segment.</param>
        /// <param name="end">End of segment.</param>
        /// <param name="x1">First intersection (if any).</param>
        /// <param name="x2">Second intersection (if any).</param>
        /// <param name="istangent">Is segment a tangent to the circle? This can be true only if
        /// ONE intersection is returned.</param>
        /// <param name="online">TRUE if the intersection must lie ON the segment.</param>
        /// <returns>The number of intersections found (0, 1, or 2).</returns>
        internal static uint IntersectCircle( IPosition centre
                                            , double radius
 					                        , IPosition start
					                        , IPosition end
					                        , out IPosition ox1
					                        , out IPosition ox2
                                            , out bool istangent
					                        , bool online )
        {
        	// Initialize the return info.
            Position x1 = new Position(0, 0);
            Position x2 = new Position(0, 0);
            ox1 = x1;
            ox2 = x2;
            istangent = false;

            // Get the bearing of this segment.
            double bearing = Geom.BearingInRadians(start, end);

            // The equation of each line is given in parametric form as:
            //
            //		x = xo + f * r
            //		y = yo + g * r
            //
            // where	xo,yo is the from point
            // and		f = sin(bearing)
            // and		g = cos(bearing)
            // and		r is the distance along the line.

            double g = Math.Cos(bearing);
            double f = Math.Sin(bearing);

            double fsq = f*f;
            double gsq = g*g;

            double startx = start.X;
            double starty = start.Y;
            double dx = centre.X - startx;
            double dy = centre.Y - starty;

            double fygx = f*dy - g*dx;
            double root = radius*radius - fygx*fygx;

	        // Check for no intersection.
	        if ( root < -Constants.TINY ) return 0;

	        // We've either got 1 or 2 intersections ...

	        // If the intersection has to be ON the line, we'll need
	        // the length of the line segment.
	        double seglen=0.0;
	        if ( online )
                seglen = BasicGeom.Distance(start, end);

	        // Check for tangential intersection.

	        if ( root < Constants.TINY )
            {
		        double xdist = f*dx + g*dy;
		        if ( online && (xdist<0.0 || xdist>seglen) ) return 0;
		        x1.X = startx + f*xdist;
                x1.Y = starty + g*xdist;
		        istangent = true;
		        return 1;
	        }

	        // That leaves us with 2 intersections, although one or both of
	        // them may not actually fall on the segment.

	        double fxgy = f*dx + g*dy;
	        root = Math.Sqrt(root);
            double dist1 = (fxgy - root);
            double dist2 = (fxgy + root);

	        if ( online )
            {
		        uint nok=0;

		        if ( dist1>0.0 && dist1<seglen )
                {
			        x1.X = startx + f*dist1;
                    x1.Y = starty + g*dist1;
			        nok = 1;
		        }

		        if ( dist2>0.0 && dist2<seglen )
                {
			        if ( nok==0 )
                    {
				        x1.X = startx + f*dist2;
                        x1.Y = starty + g*dist2;
				        return 1;
			        }

			        x2.X = startx + f*dist2;
                    x2.Y = starty + g*dist2;
			        return 2;
		        }

		        return nok;
	        }

	        // Doesn't need to be ON the segment.

	        x1.X = startx + f*dist1;
            x1.Y = starty + g*dist1;

	        x2.X = startx + f*dist2;
            x2.Y = starty + g*dist2;

	        return 2;
        }

        /// <summary>
        /// Checks if a point is to the right of a line defined by a pair of other points.
        /// Warning: This has not been tested.
        /// </summary>
        /// <param name="p1">The 1st point of the reference line</param>
        /// <param name="p2">The 2nd point of the reference line</param>
        /// <param name="pt">The point to check</param>
        /// <returns>True if the point to check is to the right of the reference line</returns>
        static bool IsRightOf(IPosition p1, IPosition p2, IPosition pt)
        {
            return IsRightOf(p1.X, p1.Y, p2.X, p2.Y, pt.X, pt.Y);
        }

        /// <summary>
        /// Checks if a point is to the right of a line defined by a pair of other points.
        /// Warning: This has not been tested.
        /// </summary>
        /// <param name="x1">The 1st XY of the reference line</param>
        /// <param name="y1"></param>
        /// <param name="x2">The 2nd XY of the reference line</param>
        /// <param name="y2"></param>
        /// <param name="xt">The XY to check</param>
        /// <param name="yt"></param>
        /// <returns>True if the point to check is to the right of the reference line</returns>
        static bool IsRightOf(double x1, double y1, double x2, double y2, double xt, double yt)
        {
            double dx = x2-x1;
            if (Math.Abs(dx) < Constants.TINY)
                dx = 0.0;

            double dy = y2-y1;
            if (Math.Abs(dy) < Constants.TINY)
                dx = 0.0;

            double d = Math.Sqrt(dx*dx + dy*dy);
            if (Math.Abs(d) < Constants.TINY)
                return false;

            return ((dy/d)*(xt-x1) - (dx/d)*(yt-y1)) > 0.0;
        }

        /// <summary>
        /// Checks an array of positions that purportedly correspond to a circular arc, to see
        /// whether the data is directed clockwise or not. No checks are made to confirm that
        /// the data really does correspond to a circular curve.
        /// </summary>
        /// <param name="pts">Positions on circular arc</param>
        /// <param name="center">The position of the centre of the circle that the curve lies on</param>
        /// <returns>True if the data is ordered clockwise.</returns>
        public static bool IsClockwise(IPosition[] pts, IPosition center)
        {
            if (pts.Length<2)
                return false;

            // To determine the direction, we must locate two successive
            // vertices that lie in the same quadrant (with respect to the
            // circle centre).

            QuadVertex start = new QuadVertex(center, pts[0]);
            QuadVertex end;

            for (int i=1; i<pts.Length; i++, start=end)
            {
                // Pick up the position at the end of the line segment
                end = new QuadVertex(center, pts[i]);

                // If both ends of the segment are in the same quadrant,
                // see which one comes first. The result tells us whether
                // the curve is clockwise or not.

                if (start.Quadrant == end.Quadrant)
                    return (start.GetTanAngle() < end.GetTanAngle() ? true : false);
            }

            // Something has gone wrong if we got here!
            Debug.Assert(1==2);
            return true;
        }

        /// <summary>
        /// Returns a position at the midpoint of a pair of positions
        /// </summary>
        /// <param name="p">First position</param>
        /// <param name="q">Second position</param>
        /// <returns>The midpoint</returns>
        public static IPosition MidPoint(IPosition p, IPosition q)
        {
            double dx = q.X - p.X;
            double dy = q.Y - p.Y;
            return new Position(p.X + dx*0.5, p.Y + dy*0.5);
        }

        /// <summary>
        /// Checks whether a closed shape encloses a point
        /// </summary>
        /// <param name="shape">Positions defining a closed outline (the last position
        /// must coincide with the first position). At least 3 positions.</param>
        /// <param name="testPosition">The position to compare with the shape</param>
        /// <returns>True if the shape encloses the test position</returns>
        public static bool IsOverlap(IPosition[] shape, IPosition testPosition)
        {
            // Return if the test position doesn't fall within the window
            // of the shape
            Window win = new Window(shape);
            if (!win.IsOverlap(testPosition))
                return false;

            // Do point in polygon test
            return IsPointInClosedShape(shape, testPosition);
        }

        /// <summary>
        /// Performs a point in polygon test. It is assumed that the test position falls within
        /// the window of the supplied shape. Generally speaking, you will want to call
        /// <c>Geom.IsOverlap</c> method instead of this one.
        /// </summary>
        /// <devnote>
        /// This may not be the most robust function going. It was written to see whether
        /// a position falls within the outline of a text string, so the shapes it's
        /// been tested with are pretty simple.
        /// </devnote>
        /// <was>CeVertexList::IsOverlap</was>
        /// <param name="shape">Positions defining a closed outline (the last position
        /// must coincide with the first position). At least 3 positions.</param>
        /// <param name="testPosition">The position to compare with the shape</param>
        /// <returns>True if the shape encloses the test position</returns>
        public static bool IsPointInClosedShape(IPosition[] shape, IPosition testPosition)
        {
            Debug.Assert(shape.Length>=3);
            Debug.Assert(shape[0].IsAt(shape[shape.Length-1], 0.0));

            // Get the XY of the test point.
            double x = testPosition.X;
            double y = testPosition.Y;

            // Count the number of intersections with the closed shape. If the count is odd,
            // the position is inside.

            double x1 = shape[0].X;
            double y1 = shape[0].Y;

        	double x2,y2;			// End of segment
	        uint nx=0;				// Number of intersections

            for (uint i=1; i<shape.Length; i++, x1=x2, y1=y2)
            {
                // Pick up the end of the segment & get the window of the segment.
                x2 = shape[i].X;
                y2 = shape[i].Y;

                double ymin = Math.Min(y1, y2);
                double ymax = Math.Max(y1, y2);

                // Ignore horizontal segments.
		        if ((ymax-ymin) < Double.Epsilon)
                    continue;

                // The test point has to be within the Y-swath of the
                // current segment's Y-swath.
		        if (y>ymin && y<ymax)
                {
                    // Get the X-position where a horizontal line intersects the test segment.
                    // If it's to the east of the test point, increment the intersect count.

			        double dx = x2-x1;
			        double dy = y2-y1;

			        double dyi = y-y1;
			        double dxi = (dyi * dx)/dy;

			        double xi = x1 + dxi;
			        if (xi>x)
                        nx++;
		        }
            }

	        // The test point is enclosed if the count is odd.
	        return ((nx%2)==1);
        }

        /// <summary>
        /// Returns a code indicating the relative position of a location with respect to
        /// a rectangular window (the Cohen & Sutherland clipping algorithm).
        /// </summary>
        /// <param name="p">The position of interest</param>
        /// <param name="sw">South-west corner of window</param>
        /// <param name="ne">North-east corner of window</param>
        /// <returns></returns>
        internal static byte GetPositionCode(IPointGeometry p, IPointGeometry sw, IPointGeometry ne)
        {
            byte code = 0;
            long e = p.Easting.Microns;

            if (e < sw.Easting.Microns) // p west of sw
                code |= 0x01;
            else if (e > ne.Easting.Microns) // p east of ne
                code |= 0x02;

            long n = p.Northing.Microns;
            if (n < sw.Northing.Microns) // p south of sw
                code |= 0x04;
            else if (n > ne.Northing.Microns) // p north of ne
                code |= 0x08;

            return code;
        }
    }
}
