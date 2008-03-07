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
using System.Diagnostics;

namespace Backsight
{
	/// <written by="Steve Stanton" on="01-NOV-2006" />
    /// <summary>
    /// Basic geometric calculations that you can seldom do without.
    /// </summary>
    public class BasicGeom
    {
        /// <summary>
        /// Returns the minimum distance squared between a point and a line.
        /// </summary>
        /// <param name="line">The line to process</param>
        /// <param name="point">The point of interest</param>
        /// <returns>The smallest distance (squared)</returns>
        public static double MinDistanceSquared(IPosition[] line, IPosition point)
        {
            double x = point.X;
            double y = point.Y;

            double xs = line[0].X;
            double ys = line[0].Y;

            // Get the distance (squared) from the point of interest to
            // the first point in the line.
            double dx = xs-x;
            double dy = ys-y;
            double mind = (dx*dx + dy*dy);

            // Loop through the line segments, looking for something closer
            for (int seg=1; seg<line.Length; seg++)
            {
                double xe = line[seg].X;
                double ye = line[seg].Y;

                // Get closest distance to this segment
                double dist = DistanceSquared(x, y, xs, ys, xe, ye);

                // Update minimum is we got a smaller distance. If we get
                // zero, break from loop, since it won't get any better.
                if (dist<mind)
                {
                    mind = dist;
                    if (mind < Double.Epsilon) //Constants.TINY)
                        return 0.0;
                }

                xs = xe;
                ys = ye;
            }

            return mind;
        }

        /// <summary>
        /// Returns the distance between a pair of positions
        /// </summary>
        public static double Distance(IPosition a, IPosition b)
        {
            return Math.Sqrt(DistanceSquared(a, b));
        }

        /// <summary>
        /// Returns the distance (squared) between a pair of positions
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double DistanceSquared(IPosition a, IPosition b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return (dx*dx+dy*dy);
        }
        /// <summary>
        /// Finds the perpendicular distance (squared) from a point to a vector. If the perpendicular point
        /// does not lie ON the vector, the distance to the closest end is returned.
        /// </summary>
        /// <param name="x">The point to check against the vector.</param>
        /// <param name="y"></param>
        /// <param name="xs">Start of vector.</param>
        /// <param name="ys"></param>
        /// <param name="xe">End of vector.</param>
        /// <param name="ye"></param>
        /// <returns></returns>
        public static double DistanceSquared(double x, double y, double xs, double ys, double xe, double ye)
        {
            // Get the perpendicular point
            double xp, yp;
            GetPerpendicular(x, y, xs, ys, xe, ye, out xp, out yp);

            //	Calculate the distance to return
            double dxseg = x - xp;
            double dyseg = y - yp;

            return (dxseg*dxseg + dyseg*dyseg);
        }

        /// <summary>
        /// Finds the location of a point which is perpendicular to a segment. If the perpendicular point does
        /// not lie ON the vector, the location refers to the closest end of the segment.
        /// </summary>
        /// <param name="x">The point to check against the vector.</param>
        /// <param name="y"></param>
        /// <param name="xs">Start of vector.</param>
        /// <param name="ys"></param>
        /// <param name="xe">End of vector.</param>
        /// <param name="ye"></param>
        /// <param name="xp">The located point.</param>
        /// <param name="yp"></param>
        public static void GetPerpendicular(double x, double y, double xs, double ys, double xe, double ye,
                                                out double xp, out double yp)
        {
            double dxseg, dyseg; // Deltas of the vector
            double dxpt, dypt;	// Deltas of point to start of vector
            double lensq;		// Length squared of vector
            double posrat;		// Position ratio

            dxseg = xe-xs;
            dyseg = ye-ys;

            dxpt = x-xs;
            dypt = y-ys;

            // Get position ratio of the perpendicular point.
            lensq = dxseg*dxseg + dyseg*dyseg;
            posrat = ((dxseg*dxpt)+(dyseg*dypt)) / lensq;

            // If the perpendicular point falls ON the vector, use that point.
            // Otherwise use the end of the vector which is closest to the
            // perpendicular.

            if (posrat>=0.0 && posrat<=1.0)
            {
                xp = xs + (posrat*dxseg);
                yp = ys + (posrat*dyseg);
            }
            else if (posrat<0.0)
            {
                xp = xs;
                yp = ys;
            }
            else
            {
                xp = xe;
                yp = ye;
            }
        }

        /// <summary>
        /// Finds the signed perpendicular distance from a point to a vector that is expressed in terms of
        /// a bearing, and a position that the vector passes through. If the returned distance is less than zero,
        /// it means the point is to the left of the line.
        ///
        /// The perpendicular point does not necessarily lie on the line that starts at the specified point.
        /// </summary>
        /// <param name="x">X-value that line passes through.</param>
        /// <param name="y">Y-value that line passes through.</param>
        /// <param name="bearing">Bearing of the line, in radians.</param>
        /// <param name="xoff">X-value of offset point.</param>
        /// <param name="yoff">Y-value of offset point.</param>
        /// <returns></returns>
        public static double SignedDistance(double x, double y, double bearing, double xoff, double yoff)
        {
            //	The specified line comes in its parametric form:
            //
            //		X = x + ft
            //		Y = y + gt
            //
            //	where	f = sin(bearing)
            //	and		g = cos(bearing)
            //	and		t = position ratio
            //
            //	What we want to do is think of it in its implicit
            //	form:
            //
            //		aX + bY + c = 0
            //
            //	which is:
            //
            //		-gX + fY + (xg -yf) = 0
            //
            //	If we make the start of the line (x,y) the origin of the
            //	coordinate system, this simplifies to:
            //
            //		-gX + fY = 0 = gX - fY (multiply through by -1)
            //
            //	Also, if we treat the line as a unit vector (i.e.
            //	normalized), getting the signed distance is just:
            //
            //		dsigned = a*xlocal + b*ylocal
            //				= g*xlocal - f*ylocal

            return (Math.Cos(bearing)*(xoff-x) - Math.Sin(bearing)*(yoff-y));
        }

        /// <summary>
        /// Calculates the bearing of a position.
        /// </summary>
        /// <param name="origin">The origin (from point)</param>
        /// <param name="pos">The position to determine the bearing for</param>
        /// <returns>The bearing to the position.</returns>
        public static IAngle Bearing(IPosition origin, IPosition pos)
        {
            return new QuadVertex(origin, pos).Bearing;
        }

        /// <summary>
        /// Checks whether a location falls in the sector lying between two points on a 
        /// clockwise circular arc. The only condition is that the location has an appropriate
        /// bearing with respect to the bearings of the BC & EC (i.e. the location does not
        /// necessarily lie ON the arc).
        /// </summary>
        /// <param name="pos">The position to check</param>
        /// <param name="centre">The centre of the circular arc</param>
        /// <param name="start">The first point of the arc (reckoned clockwise)</param>
        /// <param name="end">The second point of the arc (reckoned clockwise)</param>
        /// <param name="angtol">Angular tolerance (locations that are beyond the BC or EC
        /// by up to this much will also be regarded as "in-sector").</param>
        /// <returns>True if position falls in the sector defined by the arc</returns>
        public static bool IsInSector( IPosition pos
                                     , IPosition centre
                                     , IPosition start
                                     , IPosition end
                                     , double angtol)
        {
            // Work out the clockwise angle, using the direction from
            // the centre to the start of sector as the reference
            // bearing.

            // 18-OCT-99: I think the following only finds close matches
            // at the start of the curve, since the angular tolerance
            // is compared with respect to the reference bearing.

            //	CeTurn turn(centre,start);
            //	if ( turn.GetAngle(*this,angtol) <= turn.GetAngle(end,angtol) )
            //		return TRUE;
            //	else
            //		return FALSE;

            /*
	            Don't know what's wrong with the following ...

            // Define a reference bearing through THIS location.
            CeTurn turn(centre,*this);

            // Get the angle to the BC & check for an exact match.
            FLOAT8 sAngle = turn.GetAngle(start,angtol);
            if ( sAngle<TINY ) return TRUE;

            // Same for the EC.
            FLOAT8 eAngle = turn.GetAngle(end,angtol);
            if ( eAngle<TINY ) return TRUE;

            // This location is in sector if the angle to the EC
            // is less than the angle to the BC.
            return (eAngle<sAngle);
            */

            // Check if definitely in sector.

            Turn turn = new Turn(centre, start);
            double aThis = turn.GetAngle(pos).Radians;
            double aEnd  = turn.GetAngle(end).Radians;
            if (aThis <= aEnd)
                return true;

            Debug.Assert(aThis>=0.0 && aThis<=MathConstants.PIMUL2);
            Debug.Assert(aEnd>=0.0 && aEnd<=MathConstants.PIMUL2);

            if (angtol > MathConstants.TINY)
            {
                // Check for match at the start of the curve.
                if (aThis<angtol || Math.Abs(aThis-MathConstants.PIMUL2)<angtol)
                    return true;

                // And at the end of the curve.
                if (Math.Abs(aThis-aEnd)<angtol)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a position based on its polar coordinates
        /// </summary>
        /// <param name="origin">The origin for the position</param>
        /// <param name="bearing">The bearing (in radians)</param>
        /// <param name="distance">The distance (in meters)</param>
        /// <returns></returns>
        public static IPosition Polar(IPosition origin, double bearing, double distance)
        {
            double x = origin.X + distance * Math.Sin(bearing);
            double y = origin.Y + distance * Math.Cos(bearing);
            return new Position(x, y);
        }

        /// <summary>
        /// Rotates a position by a clockwise angle.
        /// </summary>
        /// <param name="origin">The position to rotate around.</param>
        /// <param name="point">The position that needs to be rotated</param>
        /// <param name="rotation">The clockwise rotation (less than zero for a
        /// counter-clockwise rotation.</param>
        /// <returns></returns>
        public static IPosition Rotate(IPosition origin, IPosition point, IAngle rotation)
        {
            // What's the distance between the point we're rotating and the origin?
            double dist = Distance(origin, point);

            // Get the bearing of the rotated point with respect to the origin.
            double newbearing = Bearing(origin, point).Radians + rotation.Radians;

            // Figure out the new position.
            return Polar(origin, newbearing, dist);
        }
    }
}
