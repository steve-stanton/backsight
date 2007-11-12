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

using Backsight.Geometry;
using System.Diagnostics;

namespace Backsight.Editor
{
    //- LineSegment - LineSegment
    //- LineSegment - MultiLineSegment
    //- LineSegment - CircularArc
    //- LineSegment - Circle
    //- MultiLineSegment - MultiLineSegment
    //- MultiLineSegment - CircularArc
    //- MultiLineSegment - Circle
    // CircularArc - CircularArc
    // CircularArc - Circle
    //- Circle - Circle

    static class IntersectionHelper
    {
        internal static uint Intersect(IntersectionResult results, ILineSegmentGeometry ab, ILineSegmentGeometry pq)
        {
            return Intersect(results, ab.Start, ab.End, pq.Start, pq.End);
        }

        internal static uint Intersect( IntersectionResult results
                                      , IPointGeometry a
                                      , IPointGeometry b
                                      , IPointGeometry p
                                      , IPointGeometry q )
        {
            // 04-APR-2003: This isn't supposed to happen, but if we've somehow
            // got a null segment, it NEVER intersects anything.
            if (a.IsCoincident(b))
                return 0;

            // If the segment EXACTLY meets either end of the other segment,
            // it gets handled seperately.
            if (a.IsCoincident(p) || a.IsCoincident(q))
                return EndIntersect(results, p, q, a, b);

            if (b.IsCoincident(p) || b.IsCoincident(q))
                return EndIntersect(results, p, q, b, a);

            // Return if the windows don't overlap.
            Window winab = new Window(a, b);
            Window winpq = new Window(p, q);
            if (!winab.IsOverlap(winpq))
                return 0;

            // Check whether this line is completely coincident with the other one.
            // double tolsq = Constants.XYTOLSQ;
            double tolsq = (Constants.XYRES*Constants.XYRES);

            uint xcase = 0;
            if (PointGeometry.IsCoincidentWith(a, p, q, tolsq))
                xcase |= 0x08;
            if (PointGeometry.IsCoincidentWith(b, p, q, tolsq))
                xcase |= 0x04;

            if (xcase==12) // bits 1100
            {
                results.Append(a, b);
                return 1;
            }

            // Check the end points of the other line to this one.
            if (PointGeometry.IsCoincidentWith(p, a, b, tolsq))
                xcase |= 0x02;
            if (PointGeometry.IsCoincidentWith(q, a, b, tolsq))
                xcase |= 0x01;

            // Return intersections. Note that in cases 3,7,11, the intersections
            // are not necessarily ordered with respect to THIS segment.

            switch (xcase)
            {
                case 0:
                    {
                        // Try to get a simple intersection. Do not accept
                        // virtual intersections, since they should have been
                        // trapped via the calculation of the xcase.

                        double xi, yi;
                        int xcode = Geom.CalcIntersect(a.X, a.Y, b.X, b.Y, p.X, p.Y, q.X, q.Y, out xi, out yi, true);

                        if (xcode < 0)
                        {
                            results.Append(xi, yi, 0.0);
                            return 1;
                        }

                        return 0;
                    }

                case 1:
                    {
                        results.Append(q);
                        return 1;
                    }

                case 2:
                    {
                        results.Append(p);
                        return 1;
                    }

                case 3:
                    {
                        results.Append(p, q);	// order?
                        return 1;
                    }

                case 4:
                    {
                        results.Append(b);
                        return 1;
                    }

                case 5:
                    {
                        results.Append(q, b);
                        return 1;
                    }

                case 6:
                    {
                        results.Append(p, b);
                        return 1;
                    }

                case 7:
                    {
                        results.Append(p, q);	// order?
                        return 1;
                    }

                case 8:
                    {
                        results.Append(a);
                        return 1;
                    }

                case 9:
                    {
                        results.Append(a, q);
                        return 1;
                    }

                case 10:
                    {
                        results.Append(a, p);
                        return 1;
                    }

                case 11:
                    {
                        results.Append(p, q);	// order?
                        return 1;
                    }

            } // end switch

            throw new Exception("LineSegmentFeature.Intersect - Unexpected case");
        }

        /// <summary>
        /// Intersects this segment with another segment, where at least one end of the
        /// segment exactly coincides with one end of the other segment. 
        /// </summary>
        /// <param name="xsect">The intersection results.</param>
        /// <param name="start">The start of the other segment.</param>
        /// <param name="end">The end of the other segment.</param>
        /// <param name="xend">The end of THIS segment that coincides with one end
        /// of the other one (the geometry for either m_Start or m_End).</param>
        /// <param name="othend">The other end of THIS segment (may or may not coincide
        /// with one end of the other segment).</param>
        /// <returns>The number of intersections (always 1).</returns>
        static uint EndIntersect ( IntersectionResult xsect
                                 , IPointGeometry start
                                 , IPointGeometry end
                                 , IPointGeometry xend
                                 , IPointGeometry othend )
        {
            // If the other end of this segment coincides with either end
            // of the other segment, we've got a total graze.
            if (othend.IsCoincident(start) || othend.IsCoincident(end))
            {
                xsect.Append(start, end);
                return 1;
            }

            // Get the locations that define the longer segment, together
            // with the location that is different from the exactly matching end.

            IPointGeometry startLong;
            IPointGeometry endLong;
            IPointGeometry test;

            if (Geom.DistanceSquared(xend, othend) < Geom.DistanceSquared(start, end))
            {
                test = othend;
                startLong = start;
                endLong = end;
            }
            else
            {
                startLong = xend;
                endLong = othend;

                if (xend.IsCoincident(start))
                    test = end;
                else
                    test = start;
            }

            // If it is coincident (to within the resolution) AND the
            // position ratio of the perpendicular point is ON this
            // segment, it's a graze.

            double tolsq = (Constants.XYRES * Constants.XYRES);

            if (PointGeometry.IsCoincidentWith(test, startLong, endLong, tolsq))
            {
                double prat = Geom.GetPositionRatio(test.X
                                                   , test.Y
                                                   , startLong.X
                                                   , startLong.Y
                                                   , endLong.X
                                                   , endLong.Y);

                if (prat>0.0 && prat<1.0)
                {
                    xsect.Append(xend, test);
                    return 1;
                }
            }

            // ONE intersection at the end that exactly matches.
            xsect.Append(xend);
            return 1;
        }

        internal static uint Intersect(IntersectionResult result, ILineSegmentGeometry seg, IMultiSegmentGeometry line)
        {
            uint nx=0;

            IPointGeometry a = seg.Start;
            IPointGeometry b = seg.End;
            IPointGeometry[] data = line.Data;

            for (int i=1; i<data.Length; i++)
                nx += Intersect(result, a, b, data[i-1], data[i]);

            return nx;
        }

        internal static uint Intersect(IntersectionResult result, ILineSegmentGeometry seg, ICircularArcGeometry arc)
        {
            if (CircularArcGeometry.IsCircle(arc))
                return Intersect(result, seg, arc.Circle);
            else
                return Intersect(result, seg.Start, seg.End, arc.First, arc.Second, arc.Circle);
        }

        /// <summary>
        /// Intersects a line segment with the data describing a clockwise curve.
        /// </summary>
        /// <param name="result">The intersection results.</param>
        /// <param name="a">The start of the line segment</param>
        /// <param name="b">The end of the line segment</param>
        /// <param name="start">The start of the clockwise arc.</param>
        /// <param name="end">The end of the clockwise arc.</param>
        /// <param name="circle">The circle on which the arc lies.</param>
        /// <returns></returns>
        static uint Intersect
            ( IntersectionResult result
            , IPointGeometry a
            , IPointGeometry b
            , IPointGeometry start
            , IPointGeometry end
            , ICircleGeometry circle )
        {
            // If the segment exactly meets either end of the curve, it gets handled seperately.
            if (a.IsCoincident(start) || a.IsCoincident(end))
                return EndIntersect(result, start, end, circle, a, b);

            if (b.IsCoincident(start) || b.IsCoincident(end))
                return EndIntersect(result, start, end, circle, b, a);

            // Get circle definition.
            IPointGeometry centre = circle.Center;
            double radius = circle.Radius.Meters;

            // Get up to 2 intersections with the circle.
            IPosition x1, x2;
            bool isGraze;
            uint nx = GetXCircle(a, b, centre, radius, out x1, out x2, out isGraze);

            // Return if no intersections with the circle.
            if (nx==0)
                return 0;

            // If an intersection is really close to the end of an arc, force it to be there.
            if (Geom.DistanceSquared(start, x1) < Constants.XYTOLSQ)
                x1 = start;
            else if (Geom.DistanceSquared(end, x1) < Constants.XYTOLSQ)
                x1 = end;

            if (nx==2)
            {
                if (Geom.DistanceSquared(start, x2) < Constants.XYTOLSQ)
                    x2 = start;
                else if (Geom.DistanceSquared(end, x2) < Constants.XYTOLSQ)
                    x2 = end;
            }

            // Determine whether the intersections fall within the sector
            // that's defined by the clockwise curve (grazes need a bit
            // more handling, so do that after we see how to do the non-
            // grazing case.

	        if ( !isGraze )
            {
                PointGeometry xloc1 = PointGeometry.Create(x1);
		        double lintol = Constants.XYTOL / radius;

		        // If we only got one intersect, and it's out of sector, that's us done.
		        if (nx==1)
                {
                    if (!BasicGeom.IsInSector(xloc1, centre, start, end, lintol))
                        return 0;

			        result.Append(x1);
			        return 1;
		        }

		        // Two intersections with the circle ...

		        if (BasicGeom.IsInSector(xloc1, centre, start, end, lintol))
                    result.Append(x1);
		        else
			        nx--;

		        PointGeometry xloc2 = PointGeometry.Create(x2);

		        if (BasicGeom.IsInSector(xloc2, centre, start, end, lintol))
                    result.Append(x2);
		        else
			        nx--;

		        return nx;
	        }

            // That leaves us with the case where this segment grazes the
            // circle. GetXCircle is always supposed to return nx==2 in that
            // case (they're also supposed to be arranged in the same
            // direction as this segment).
            Debug.Assert(nx==2);

            // Get the clockwise angle subtended by the circular arc. Add
            // on a tiny bit to cover numerical comparison problems.
            Turn reft = new Turn(centre, start);
            double maxangle = reft.GetAngle(end).Radians + Constants.TINY;

            // Get the clockwise angles to both intersections.
            double a1 = reft.GetAngle(x1).Radians;
            double a2 = reft.GetAngle(x2).Radians;

            // No graze if both are beyond the arc sector.
            if (a1>maxangle && a2>maxangle)
                return 0;

            // If both intersects are within sector, the only thing to watch
            // out for is a case where the graze apparently occupies more
            // than half a circle. In that case, we've actually got 2 grazes.

            if (a1<maxangle &&  a2<maxangle)
            {
                if (a2>a1 && (a2-a1)>Constants.PI)
                {
                    result.Append(x1, start);
                    result.Append(end, x2);
                    return 2;
                }

                if (a1>a2 && (a1-a2)>Constants.PI)
                {
                    result.Append(start, x2);
                    result.Append(x1, end);
                    return 2;
                }

                // So it's just a regular graze.
                result.Append(x1, x2);
                return 1;
            }

            // That's covered the cases where both intersects are either
            // both in sector, or both out. Return the intersection that's
            // in sector as a simple intersection (NOT a graze).

            // This is a cop-out. We should really try to figure out which
            // portion of the curve is grazing, but the logic for doing so
            // is surprisingly complex, being subject to a variety of special
            // cases. By returning the intersect as a simple intersect, I'm
            // hoping it covers most cases.

            if (a1 < maxangle)
                result.Append(x1);
            else
                result.Append(x2);

            return 1;
        }

        /// <summary>
        /// Intersects a line segment with a clockwise arc, where at least one end of the
        /// segment exactly coincides with one end of the arc.
        /// </summary>
        /// <param name="xsect">The intersection results.</param>
        /// <param name="bc">The start of the clockwise arc.</param>
        /// <param name="ec">The end of the clockwise arc.</param>
        /// <param name="circle">The circle on which the arc lies.</param>
        /// <param name="xend">The end of the segment that coincides with one end of the arc.</param>
        /// <param name="othend">The other end of the segment (may or may not coincide
        /// with one end of the arc).</param>
        /// <returns>The number of intersections (either 1 or 2).</returns>
        static uint EndIntersect ( IntersectionResult xsect
                                 , IPointGeometry bc
                                 , IPointGeometry ec
                                 , ICircleGeometry circle
                                 , IPointGeometry xend
                                 , IPointGeometry othend)
        {
            // If the other end is INSIDE the circle, the matching end
            // location is the one and only intersection. Allow a tolerance
            // that is consistent with the resolution of data (3 microns).
            IPointGeometry centre = circle.Center;
            double radius = circle.Radius.Meters;
            double minrad = (radius-Constants.XYTOL);
            double minrsq = (minrad*minrad);
            double othdsq = Geom.DistanceSquared(othend, centre);

            if (othdsq < minrsq)
            {
                xsect.Append(xend);
                return 1;
            }

            // If the other end of the segment also coincides with either
            // end of the curve, the segment is a chord of the circle that
            // the curve lies on. However, if it's REAL close, we need to
            // return it as a graze ...

	        if (othend.IsCoincident(bc) || othend.IsCoincident(ec))
            {
		        // Get the midpoint of the chord.
                IPosition midseg = Position.CreateMidpoint(xend, othend);

		        // If the distance from the centre of the circle to the
		        // midpoint is REAL close to the curve, we've got a graze.

		        if (Geom.DistanceSquared(midseg, centre) > minrsq)
                {
			        xsect.Append(xend, othend);
			        return 1;
		        }

		        // Two distinct intersections.
		        xsect.Append(xend);
		        xsect.Append(othend);
		        return 2;
	        }

            // If the other end is within tolerance of the circle, project
            // it to the circle and see if it's within the curve's sector.
            // If not, it's not really an intersect.

            double maxrad = (radius+Constants.XYTOL);
            double maxrsq = (maxrad*maxrad);

	        if ( othdsq < maxrsq )
            {
		        // Check if the angle to the other end is prior to the EC.
                // If not, it's somewhere off the curve.
                Turn reft = new Turn(centre, bc);
		        if (reft.GetAngle(othend).Radians > reft.GetAngle(ec).Radians)
                {
			        xsect.Append(xend);
			        return 1;
		        }

		        // And, like above, see if the segment grazes or not ...

		        // Get the midpoint of the chord.
                IPosition midseg = Position.CreateMidpoint(xend, othend);

		        // If the distance from the centre of the circle to the
		        // midpoint is REAL close to the curve, we've got a graze.

		        if (Geom.DistanceSquared(midseg, centre) > minrsq)
                {
			        xsect.Append(xend, othend);
			        return 1;
		        }

		        // Two distinct intersections.
		        xsect.Append(xend);
		        xsect.Append(othend);
		        return 2;
	        }

            // That leaves us with the other end lying somewhere clearly
            // outside the circle. However, we could still have a graze.

            // Make sure the BC/EC are EXACTLY coincident with the circle (they
            // may have been rounded off if the curve has been intersected
            // with a location that's within tolerance of the circle).

            IPosition trueBC, trueEC;
            Position.GetCirclePosition(bc, centre, radius, out trueBC);
            Position.GetCirclePosition(ec, centre, radius, out trueEC);

            // As well as the end of the segment that meets the curve.
            IPosition trueXend = (xend.IsCoincident(bc) ? trueBC : trueEC);

            // Intersect the segment with the complete circle (this does
            // NOT return an intersection at the other end, even if it is
            // really close ... we took care of that above).

            // The intersection must lie ON the segment (not sure about this though).

            IPosition othvtx = othend;
            IPosition x1, x2;
            bool isTangent;
	        uint nx = Geom.IntersectCircle(centre, radius, trueXend, othvtx, out x1, out x2, out isTangent, true);

            // If we got NOTHING, that's unexpected, since one end exactly coincides
            // with the circle. In that case, just return the matching end (NOT
            // projected to the true position).
            if (nx==0)
            {
                xsect.Append(xend);
                return 1;
            }

            // If we got 2 intersections, pick the one that's further away than the matching end.
            if (nx==2 && Geom.DistanceSquared(x2, trueXend) > Geom.DistanceSquared(x1, trueXend))
                x1 = x2;

            // That leaves us with ONE intersection with the circle ... now
            // confirm that it actually intersects the arc!

            Turn refbc = new Turn(centre, trueBC);
            double eangle = refbc.GetAngle(trueEC).Radians;
            double xangle = refbc.GetAngle(x1).Radians;

            if (xangle > eangle)
            {
                xsect.Append(xend);
                return 1;
            }

            // Get the midpoint of the segment that connects the intersection to the true end.
            IPosition midx = Position.CreateMidpoint(trueXend, x1);

            // If the midpoint does NOT graze the circle, we've got 2 distinct intersections.
            // 25-NOV-99: Be realistic about it (avoid meaningless sliver polygons that are
            // less than 0.1mm wide on the ground).

            // if ( midx.DistanceSquared(centre) < minrsq ) {
            double rdiff = Geom.Distance(midx, centre) - radius;
            if (Math.Abs(rdiff) > 0.0001)
            {
                xsect.Append(xend);
                xsect.Append(x1);
                return 2;
            }

            // We've got a graze, but possibly one that can be ignored(!). To
            // understand the reasoning here, bear in mind that lines get cut
            // only so that network topology can be formed. To do that, 2
            // orientation points are obtained for the lines incident on xend.
            // For the segment, the orientation point is the other end of the
            // segment. For the curve, it's a position 5 metres along the
            // curve (or the total curve length if it's not that long). So
            // if the graze is closer than the point that will be used to
            // get the orientation point, we can ignore the graze, since it
            // does not provide any useful info.

            // Given that it's a graze, assume that it's ok to work out
            // the arc distance as if it was straight.
            double dsqx = Geom.DistanceSquared(trueXend, x1);

            // If it's closer than 4m (allow some leeway, seeing how we've
            // just done an approximation), ignore the intersection. If it's
            // actually between 4 and 5 metres, it shouldn't do any harm
            // to make a split there (although it's kind of redundant).
            if (dsqx < 16.0)
            {
                xsect.Append(xend);
                return 1;
            }

            // It's a graze.
        	//CeVertex vxend(xend);	// wants a vertex
            xsect.Append(xend, x1);
            return 1;
        }

        internal static uint Intersect(IntersectionResult result, ILineSegmentGeometry seg, ICircleGeometry circle)
        {
            return Intersect(result, seg.Start, seg.End, circle.Center, circle.Radius.Meters);
        }

        /// <summary>
        /// Intersects a line segment with a circle
        /// </summary>
        /// <param name="result"></param>
        /// <param name="a">The start of the line segment</param>
        /// <param name="b">The end  of the line segment</param>
        /// <param name="c">The center of the circle</param>
        /// <param name="r">The radius of the circle</param>
        /// <returns></returns>
        static uint Intersect(IntersectionResult result, IPointGeometry a, IPointGeometry b, IPointGeometry c, double r)
        {
            // Get intersections (if any). Return if nothing.
            IPosition x1, x2;
            bool isGraze;
            uint nx = GetXCircle(a, b, c, r, out x1, out x2, out isGraze);
            if (nx==0)
                return 0;

            // If we got just one intersection, it's a simple intersect (even
            // if it's a tangent to the circle).
            if (nx==1)
            {
                result.Append(x1);
                return 1;
            }

            // That leaves us with two intersections, which may or may
            // not be grazing the circle.

            if (isGraze)
            {
                result.Append(x1, x2);
                return 1;
            }

            result.Append(x1);
            result.Append(x2);
            return 2;
        }

        /// <summary>
        /// Intersects a line segment with the data describing a circle. The resultant
        /// intersections will lie ON the segment somewhere (not on a projection of it).
        /// 
        /// That said, if an end of this segment is within tolerance of the circle (but
        /// not necessarily intersecting), that end will be regarded as an intersection.
        /// Furthermore, if an end is within tolerance of an intersection, that end will
        /// also be regarded as an intersection (as opposedto the "true" intersection).
        /// </summary>
        /// <param name="vs">The start of the line segment</param>
        /// <param name="ve">The end of the line segment</param>
        /// <param name="centre">Position of the circle's centre.</param>
        /// <param name="radius">Radius of the circle.</param>
        /// <param name="x1">First intersection (if any).</param>
        /// <param name="x2">Second intersection (if any).</param>
        /// <param name="isGraze">Does intersection graze the circle? (defined only if
        /// 2 intersections are returned).</param>
        /// <returns>The number of intersections found (0, 1, or 2). If 2 intersections
        /// are returned, they will be ordered in the same direction as this segment.
        /// </returns>
        static uint GetXCircle( IPosition vs
                              , IPosition ve
                              , IPosition centre
                              , double radius
                              , out IPosition x1
                              , out IPosition x2
                              , out bool isGraze )
        {
            // Initialize return variables.
            x1 = x2 = null;
            isGraze = false;

            // If the start and end of this segment are well inside the circle, there's no intersection.
            double minrad = radius - Constants.XYTOL;
            double minrsq = minrad * minrad;
            double sdsq = Geom.DistanceSquared(centre, vs);
            double edsq = Geom.DistanceSquared(centre, ve);
            if (sdsq<minrsq && edsq<minrsq)
                return 0;

            // Check whether the ends of this segment are really close to the circle.
            double maxrad = radius + Constants.XYTOL;
            double maxrsq = maxrad * maxrad;
            bool smatch = (sdsq>minrsq && sdsq<maxrsq);
            bool ematch = (edsq>minrsq && edsq<maxrsq);

            // If BOTH ends of this segment are really close, treat it
            // as an intersection at both ends. It's a graze if the
            // midpoint of the segment is within tolerance of the circle.
	        if (smatch && ematch)
            {
		        // The intersections EXACTLY match the ends of the segment.
		        x1 = vs;
		        x2 = ve;

		        // Check if it's a graze
                IPosition vm = Position.CreateMidpoint(vs, ve);
		        double mdsq = Geom.DistanceSquared(centre, vm);
		        isGraze = (mdsq>minrsq && mdsq<maxrsq);

		        return 2;
	        }

            bool isTangent;		// Is intersect a tangent to circle?
            uint nx;			// The number of intersections.

            // If ONE of the ends is really close to the circle ...
            if (smatch || ematch)
            {
                // Project the close end so that it coincides exactly with the circle.
                IPosition vp, vq;

                if (smatch)
                {
                    Position.GetCirclePosition(vs, centre, radius, out vp);
                    vq = ve;
                }
                else
                {
                    vq = vs;
                    Position.GetCirclePosition(ve, centre, radius, out vp);
                }

                // Intersect the adjusted segment with the circle. Any intersection(s)
                // MUST lie strictly ON the segment.
                nx = Geom.IntersectCircle(centre, radius, vp, vq, out x1, out x2, out isTangent, true);

                // If we got NOTHING, we've had a bit of roundoff, so go with a
                // simple intersection at the end of this segment that was really close.
                if (nx==0)
                {
                    if (smatch)
                        x1 = vs;
                    else
                        x1 = ve;

                    return 1;
                }

                // If one intersection, maybe we also had roundoff, so check
                // if it's anywhere really close to the matching end (the one
                // we projected to). If so, use the corresponding end of this
                // segment instead.

                if (nx==1)
                {
                    if (Geom.DistanceSquared(vp, x1) < Constants.XYTOLSQ)
                    {
                        if (smatch)
                            x1 = vs;
                        else
                            x1 = ve;

                        return 1;
                    }

        			// Treat as two intersections (arranged in the direction along the segment).
			        nx = 2;

			        if ( smatch )
                    {
				        x2 = x1;
				        x1 = vs;
			        }
			        else 
				        x2 = ve;

			        // And drop through to the grazing test at the end ...
                }
                else
                {
                    // Arrange the two intersections so they're ordered in
                    // the direction of this segment.
                    if (Geom.DistanceSquared(vs, x2) < Geom.DistanceSquared(vs, x1))
                    {
                        IPosition temp = x1;
                        x1 = x2;
                        x2 = temp;
                    }

                    // If we have a match at the start, and the first intersect
                    // is really close to it, use the start of segment. Similarly
                    // compare the 2nd intersect to the end of this segment.

                    if (smatch && Geom.DistanceSquared(vp, x1) < Constants.XYTOLSQ)
                        x1 = vs;

                    if (ematch && Geom.DistanceSquared(vp, x2) < Constants.XYTOLSQ)
                        x2 = ve;
                }
            }
            else
            {
                // That leaves us with the case where neither the start or the
                // end of this segment are really close to the circle. So just
                // do a straightforward intersection with the circle.

                nx = Geom.IntersectCircle(centre, radius, vs, ve, out x1, out x2, out isTangent, true);

                // Nothing to do if no intersections.
                if (nx==0)
                    return 0;

                // If we got just one intersection, and it's really close to
                // either end of this segment, use that position instead.
                if (nx==1)
                {
                    if (Geom.DistanceSquared(vs, x1) < Constants.XYTOLSQ)
                        x1 = vs;
                    else if (Geom.DistanceSquared(ve, x1) < Constants.XYTOLSQ)
                        x1 = ve;

                    return 1;
                }

                // We got two intersections, so initially arrange them in the
                // direction of this segment, and check whether they're
                // really close to the ends of this segment.

                if (Geom.DistanceSquared(vs, x2) < Geom.DistanceSquared(vs, x1))
                {
                    IPosition temp = x1;
			        x1 = x2;
			        x2 = temp;
		        }

                if (Geom.DistanceSquared(vs, x1) < Constants.XYTOLSQ)
                    x1 = vs;

                if (Geom.DistanceSquared(ve, x2) < Constants.XYTOLSQ)
                    x2 = ve;
            }

            // We got 2 intersections, so see if they make a graze.
            IPosition xm = Position.CreateMidpoint(x1, x2);
            double xmdsq = Geom.DistanceSquared(centre, xm);
            isGraze = (xmdsq>minrsq && xmdsq<maxrsq);
            return 2;
        }

        internal static uint Intersect(IntersectionResult result, IMultiSegmentGeometry line1, IMultiSegmentGeometry line2)
        {
	        uint nx=0;
            IPointGeometry[] pa = line1.Data;
            IPointGeometry[] pb = line2.Data;

	        for (int i=1; i<pa.Length; i++)
            {
                IPointGeometry a = pa[i-1];
                IPointGeometry b = pa[i];

                for(int j=1; j<pb.Length; j++)
                    nx += Intersect(result, a, b, pb[j-1], pb[j]);
	        }

	        return nx;
        }

        internal static uint Intersect(IntersectionResult results, IMultiSegmentGeometry line, ICircularArcGeometry arc)
        {
	        uint nx=0;

            IPointGeometry[] segs = line.Data;
            IPointGeometry f = arc.First;
            IPointGeometry s = arc.Second;
            ICircleGeometry circle = arc.Circle;

	        for (int i=1; i<segs.Length; i++)
                nx += Intersect(results, segs[i-1], segs[i], f, s, circle);

	        return nx;
        }

        internal static uint Intersect(IntersectionResult results, IMultiSegmentGeometry line, ICircleGeometry circle)
        {
	        uint nx=0;

            IPointGeometry[] segs = line.Data;
            IPointGeometry c = circle.Center;
            double r = circle.Radius.Meters;

            for (int i=1; i<segs.Length; i++)
                nx += Intersect(results, segs[i-1], segs[i], c, r);

	        return nx;
        }

        /// <summary>
        /// Intersects a pair of circular arcs.
        /// </summary>
        /// <param name="results">Where to stick the results</param>
        /// <param name="a">The first arc</param>
        /// <param name="b">The second arc</param>
        /// <returns></returns>
        internal static uint Intersect(IntersectionResult results, ICircularArcGeometry ab, ICircularArcGeometry pq)
        {
            // Special handling if the two arcs share the same circle
            if (CircleGeometry.IsCoincident(ab.Circle, pq.Circle, Constants.XYRES))
                return ArcIntersect(results, ab, pq);

            // Arcs that meet exactly end to end get handled seperately.
            IPointGeometry a = ab.First;
            IPointGeometry b = ab.Second;
            IPointGeometry p = pq.First;
            IPointGeometry q = pq.Second;

            if (a.IsCoincident(p) || a.IsCoincident(q))
                return EndIntersect(results, ab, pq, true);

            if (b.IsCoincident(p) || b.IsCoincident(q))
                return EndIntersect(results, ab, pq, false);
            
            // Intersect the circle for the two arcs
            IPosition x1, x2;
            uint nx = Intersect(ab.Circle, pq.Circle, out x1, out x2);

            // Return if the circles don't intersect.
            if (nx==0)
                return 0;

            // Remember the intersection(s) if they fall in BOTH curve sectors.

            IPointGeometry thiscen = ab.Circle.Center;
            IPointGeometry othrcen = pq.Circle.Center;

            if (nx==1)
            {
                // If we got 1 intersection, it may be VERY close to the end
                // points. Make sure we also consider the precise check made up top.
                // Otherwise check if the intersection is in both sectors.

                IPointGeometry loc = PointGeometry.Create(x1); // rounded to nearest micron

                if (Geom.IsInSector(loc, thiscen, a, b, 0.0) &&
                    Geom.IsInSector(loc, othrcen, p, q, 0.0))
                {
                    results.Append(loc);
                    return 1;
                }

                return 0;
            }
            else
            {
                // Two intersections. They are valid if they fall within the arc's sector.
                // Again, make sure we consider any precise end-point check made above.

                uint nok=0;
                IPointGeometry loc1 = PointGeometry.Create(x1);
                IPointGeometry loc2 = PointGeometry.Create(x2);

                if (Geom.IsInSector(loc1, thiscen, a, b, 0.0) &&
                    Geom.IsInSector(loc1, othrcen, p, q, 0.0))
                {
                    results.Append(loc1);
                    nok++;
                }

                if (Geom.IsInSector(loc2, thiscen, a, b, 0.0) &&
                    Geom.IsInSector(loc2, othrcen, p, q, 0.0))
                {
                    results.Append(loc2);
                    nok++;
                }

                return nok;
            }
        }

        static uint ArcIntersect(IntersectionResult results, ICircularArcGeometry ab, ICircularArcGeometry pq)
        {
            // Arcs that meet exactly end to end get handled seperately.
            IPointGeometry a = ab.First;
            IPointGeometry b = ab.Second;
            IPointGeometry p = pq.First;
            IPointGeometry q = pq.Second;

            if (a.IsCoincident(p) || a.IsCoincident(q))
                //return ArcEndIntersect(results, pq.Circle, p, q, a, b);
                return ArcEndIntersect(results, pq.Circle, p, q, a, b, true);

            if (b.IsCoincident(p) || b.IsCoincident(q))
                //return ArcEndIntersect(results, pq.Circle, p, q, b, a);
                return ArcEndIntersect(results, pq.Circle, p, q, a, b, false);

            return ArcIntersect(results, pq.Circle, p, q, a, b);
        }


        /// <summary>
        /// Intersects 2 clockwise arcs that coincide with the perimeter of a circle.
        /// </summary>
        /// <param name="xsect">Intersection results.</param>
        /// <param name="circle">The circle the arcs coincide with</param>
        /// <param name="bc1">BC for 1st arc.</param>
        /// <param name="ec1">EC for 1st arc.</param>
        /// <param name="bc2">BC for 2nd arc.</param>
        /// <param name="ec2">EC for 2nd arc.</param>
        /// <returns>The number of intersections (0, 1, or 2). If non-zero, the intersections
        /// will be grazes.</returns>
        static uint ArcIntersect
            ( IntersectionResult xsect
            , ICircleGeometry circle
            , IPointGeometry bc1
            , IPointGeometry ec1
            , IPointGeometry bc2
            , IPointGeometry ec2 )
        {
            // Define the start of the clockwise arc as a reference line,
            // and get the clockwise angle to it's EC.
            Turn reft = new Turn(circle.Center, bc1);
            double sector = reft.GetAngle(ec1).Radians;

            // Where do the BC and EC of the 2nd curve fall with respect
            // to the 1st curve's arc sector?
            double bcang = reft.GetAngle(bc2).Radians;
            double ecang = reft.GetAngle(ec2).Radians;

            if (bcang<sector)
            {
                if (ecang<bcang)
                {
                    xsect.Append(bc2, ec1);
                    xsect.Append(bc1, ec2);
                    return 2;
                }

                if (ecang<sector)
                    xsect.Append(bc2, ec2);
                else
                    xsect.Append(bc2, ec1);

                return 1;
            }

            // The BC of the 2nd curve falls beyond the sector of the 1st ...
            // so we can't have any graze if the EC is even further on.
            if (ecang>bcang)
                return 0;

            // One graze ...

            if (ecang<sector)
                xsect.Append(bc1, ec2);
            else
                xsect.Append(bc1, ec1);

            return 1;
        }

        /// <summary>
        /// Intersect two circles. If there are two intersections,
        /// they are arranged so that the one with the lowest bearing comes first.
        /// </summary>
        /// <param name="a">The 1st circle</param>
        /// <param name="b">The 2nd circle</param>
        /// <param name="x1">The 1st intersection (if any).</param>
        /// <param name="x2">The 2nd intersection (if any).</param>
        /// <returns>The number of intersections found.</returns>
        static uint Intersect(ICircleGeometry a, ICircleGeometry b, out IPosition x1, out IPosition x2)
        {
            // Get the intersections (if any).
            uint nx = Geom.IntersectCircles(a.Center, a.Radius.Meters, b.Center, b.Radius.Meters, out x1, out x2);

            // Return if 0 or 1 intersection.
            if (nx<2)
                return nx;

            // Arrange the intersections so that the one with the lowest bearing comes first.
            QuadVertex q1x1 = new QuadVertex(a.Center, x1);
            QuadVertex q1x2 = new QuadVertex(a.Center, x2);
            QuadVertex q2x1 = new QuadVertex(b.Center, x1);
            QuadVertex q2x2 = new QuadVertex(b.Center, x2);

            double b1x1 = q1x1.Bearing.Radians;
            double b1x2 = q1x2.Bearing.Radians;
            double b2x1 = q2x1.Bearing.Radians;
            double b2x2 = q2x2.Bearing.Radians;

            // Switch if the min bearing to the 2nd intersection is actually
            // less than the min bearing to the 1st intersection.

            if (Math.Min(b1x2, b2x2) < Math.Min(b1x1, b2x1))
            {
                IPosition temp = x1;
                x1 = x2;
                x2 = temp;
            }

            return 2;
        }

        /// <summary>
        /// Intersects a pair of clockwise arcs, where one of the ends exactly coincides with the
        /// other arc. The two arcs are assumed to sit on different circles.
        /// </summary>
        /// <param name="xsect">The intersection results.</param>
        /// <param name="bc">The start of the other arc.</param>
        /// <param name="ec">The end of the other arc.</param>
        /// <param name="circle">The circle that the other arc sits on.</param>
        /// <param name="xend">The end of THIS arc that coincides with one end of the other one.</param>
        /// <param name="othend">The other end of THIS arc (may or may not coincide with one end of
        /// the other arc).</param>
        /// <param name="xCircle">The circle for THIS arc</param>
        /// <returns>The number of intersections (1 or 2).</returns>
        static uint EndIntersect
            ( IntersectionResult xsect
            , ICircularArcGeometry ab
            , ICircularArcGeometry pq
            , bool isFirstEndCoincident )
        {
            IPointGeometry bc = pq.First;
            IPointGeometry ec = pq.Second;
            ICircleGeometry circle = pq.Circle;
            IPointGeometry xend = (isFirstEndCoincident ? ab.First : ab.Second);
            IPointGeometry othend = (isFirstEndCoincident ? ab.Second : ab.First);
            ICircleGeometry xCircle = ab.Circle;

            // The two curves sit on different circles, so do a precise intersection of the circles.
            IPointGeometry c1 = xCircle.Center;
            IPointGeometry c2 = circle.Center;
            double r1 = xCircle.Radius.Meters;
            double r2 = circle.Radius.Meters;
            IPosition x1, x2;
            uint nx = Geom.IntersectCircles(c1, r1, c2, r2, out x1, out x2);

            // If we didn't get ANY intersections, that's a bit unusual
            // seeing how one of the ends matches. However, it's possible
            // due to roundoff of the end locations. So in that case, just
            // return a single intersection at the matching end.
            if (nx==0)
            {
                xsect.Append(xend);
                return 1;
            }

            // @devnote If we got 1 intersection (i.e. the 2 circles just
            // touch), you might be tempted to think that it must be close
            // to the matching end location. That is NOT the case if the
            // circles are big.

            // If we got 2 intersections, pick the one that's further away
            // than the matching end.
            if (nx==2 && Geom.DistanceSquared(x2, xend) > Geom.DistanceSquared(x1, xend))
                x1 = x2;

            // That leaves us with ONE intersection with the circle ... now
            // confirm that it actually intersects both curves!

            // Does it fall in the sector defined by the clockwise curve?

            IPointGeometry centre = c2;
            Turn reft = new Turn(centre, bc);
            double eangle = reft.GetAngle(ec).Radians;
            double xangle = reft.GetAngle(x1).Radians;
            if (xangle > eangle)
            {
                xsect.Append(xend);
                return 1;
            }

            // Does it fall in the sector defined by this curve (NO tolerance allowed).
            PointGeometry xloc = PointGeometry.Create(x1);
            bool isxthis = Geom.IsInSector(xloc, c1, ab.First, ab.Second, 0.0);
            if (!isxthis)
            {
                xsect.Append(xend);
                return 1;
            }

            // Get the midpoint of the segment that connects the intersection to the matching end.
            IPosition midx = Position.CreateMidpoint(xend, x1);

            // If the midpoint does NOT graze the circle, we've got 2 distinct intersections.

            // 25-NOV-99: Be realistic about it (avoid meaningless sliver
            // polygons that are less than 0.1mm wide on the ground). Also,
            // the old way used 'centre' which may refer to r1 OR r2, so
            // you would have got the correct result only half of the time!
            // if ( fabs(midx.Distance(centre) - r1) > XYTOL ) {

            double rdiff = Geom.Distance(midx, c1) - r1;
            if (Math.Abs(rdiff) > 0.0001)
            {
                xsect.Append(xend);
                xsect.Append(x1);
                return 2;
            }

            // We've got a graze, but possibly one that can be ignored(!). To
            // understand the reasoning here, bear in mind that lines get cut
            // only so that network topology can be formed. To do that, 2
            // orientation points are obtained for the lines incident on xend.
            // For curves, it's a position 5 metres along the curve (or the
            // total curve length if it's not that long). So if the graze is
            // closer than the point that will be used to get the orientation
            // point, we can ignore the graze, since it does not provide any
            // useful info.

            // Given that it's a graze, assume that it's ok to work out
            // the arc distance as if it was straight.
            double dsqx = Geom.DistanceSquared(xend, x1);

            // If it's closer than 4m (allow some leeway, seeing how we've
            // just done an approximation), ignore the intersection. If it's
            // actually between 4 and 5 metres, it shouldn't do any harm
            // to make a split there (although it's kind of redundant).
            if (dsqx < 16.0)
            {
                xsect.Append(xend);
                return 1;
            }

            // It's a graze.
            xsect.Append(xend, x1);
            return 1;
        }

        /// <summary>
        /// Intersects a pair of clockwise arcs that sit on the same circle, and where ONE of
        /// the end points exactly matches.
        /// </summary>
        /// <param name="xsect">The intersection results.</param>
        /// <param name="circle">The circle the arcs coincide with</param>
        /// <param name="bc1">The BC of the 1st arc</param>
        /// <param name="ec1">The EC of the 1st arc</param>
        /// <param name="bc2">The BC of the 2nd arc -- the matching end</param>
        /// <param name="ec2">The EC of the 2nd arc</param>
        /// <param name="isStartMatch">Specify <c>true</c> if <paramref name="bc2"/> matches an end
        /// point of the 1st arc. Specify <c>false</c> if <paramref name="ec2"/> matches an end
        /// point of the 1st arc.</param>
        /// <returns>The number of intersections (always 1).</returns>
        static uint ArcEndIntersect
            ( IntersectionResult xsect
            , ICircleGeometry circle
            , IPointGeometry bc1
            , IPointGeometry ec1
            , IPointGeometry bc2
            , IPointGeometry ec2
            , bool isStartMatch)
        {
            bool bmatch = bc1.IsCoincident(bc2);
            bool ematch = ec1.IsCoincident(ec2);

	        // If the two curves share the same BC or same EC
	        if (bmatch || ematch)
            {
		        // We've got some sort of graze ...

		        // Check for total graze.
		        if (bmatch && ematch)
			        xsect.Append(bc1, ec1);
		        else
                {
			        // We've therefore got a partial graze. 

			        // If the length of this arc is longer than the other one, the graze is over the
                    // length of the other one, and vice versa. Since the two curves share the same
			        // radius and direction, the comparison just involves a comparison of the clockwise
                    // angles subtended by the arcs.

                    IPointGeometry centre = circle.Center;
                    double ang1 = new Turn(centre, bc1).GetAngle(ec1).Radians;
                    double ang2 = new Turn(centre, bc2).GetAngle(ec2).Radians;
			        bool isThisGraze = (ang1 < ang2);

			        if (isThisGraze)
				        xsect.Append(bc1, ec1);
			        else
				        xsect.Append(bc2, ec2);
		        }
	        }
	        else
            {
		        // The only intersection is at the common end.
		        if (bc1.IsCoincident(bc2) || ec1.IsCoincident(bc2))
			        xsect.Append(bc2);
		        else
			        xsect.Append(ec2);
	        }

	        return 1;
        }

        internal static uint Intersect(IntersectionResult results, ICircularArcGeometry a, ICircleGeometry b)
        {
            if (CircularArcGeometry.IsCircle(a))
                return Intersect(results, a.Circle, b);
            else
                return Intersect(results, b, a.First, a.Second, a.Circle);
        }

        /// <summary>
        /// Intersects a circle with a clockwise arc
        /// </summary>
        /// <param name="results"></param>
        /// <param name="c"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="arcCircle"></param>
        /// <returns></returns>
        static uint Intersect(IntersectionResult xsect, ICircleGeometry c, IPointGeometry start, IPointGeometry end, ICircleGeometry arcCircle)
        {
        	// If the circles are IDENTICAL, we've got a graze.
            if (CircleGeometry.IsCoincident(c, arcCircle, Constants.XYRES))
            {
                xsect.Append(start, end);
                return 1;

            }

            // Intersect the 2 circles.
            IPosition x1, x2;
            uint nx = Intersect(c, arcCircle, out x1, out x2);

            // Return if no intersection.
	        if (nx==0)
                return 0;

            // Remember the intersection(s) if they fall in the
            // curve's sector. Use a tolerance which is based on
            // the circle with the smaller radius (=> bigger angular
            // tolerance).

            IPointGeometry centre;
            double minrad;

            if (c.Radius.Meters < arcCircle.Radius.Meters)
            {
                minrad = c.Radius.Meters;
                centre = c.Center;
            }
            else
            {
                minrad = arcCircle.Radius.Meters;
                centre = arcCircle.Center;
            }

            //	const FLOAT8 angtol = XYTOL/minrad;
            //	const FLOAT8 angtol = 0.00002/minrad;	// 20 microns
	        double angtol = 0.002/minrad;		// 2mm

            if (nx==1)
            {
        		IPointGeometry loc = PointGeometry.Create(x1);
                if (Geom.IsInSector(loc, centre, start, end, angtol))
                {
			        xsect.Append(loc);
			        return 1;
		        }

		        return 0;
            }
            else
            {
                // Two intersections. They are valid if they fall within the curve's sector.
                IPointGeometry loc1 = PointGeometry.Create(x1);
                IPointGeometry loc2 = PointGeometry.Create(x2);
                uint nok=0;

                if (Geom.IsInSector(loc1, centre, start, end, angtol))
                {
                    xsect.Append(loc1);
                    nok++;
                }

                if (Geom.IsInSector(loc2, centre, start, end, angtol))
                {
                    xsect.Append(loc2);
                    nok++;
                }

                return nok;
            }
        }

        internal static uint Intersect(IntersectionResult results, ICircleGeometry a, ICircleGeometry b)
        {
            IPointGeometry centre1 = a.Center;
            IPointGeometry centre2 = b.Center;
            double radius1 = a.Radius.Meters;
            double radius2 = b.Radius.Meters;

            // The following looks pretty similar to Geom.IntersectCircles (some tolerance values are
            // a bit more relaxed here).

            // Pull out the XY's for the 2 centres.
            double xc1 = centre1.X;
            double yc1 = centre1.Y;
            double xc2 = centre2.X;
            double yc2 = centre2.Y;

            // Get distance (squared) between the 2 centres.
            double dx = xc2 - xc1;
            double dy = yc2 - yc1;
            double distsq = dx*dx + dy*dy;

            //	If the two circles have the same centre, check whether
            //	the radii match (if so, we have an overlap case).

            if (distsq < Constants.TINY)
            {
                if (Math.Abs(radius1-radius2) < Constants.XYRES)
                {
                    double xi = xc1;
                    double yi = yc1 + radius1;
                    results.Append(xi, yi, xi, yi);
                    return 1;
                }
                else
                    return 0;
            }

            // We'll need the radii squared
            double rsq1 = radius1 * radius1;
            double rsq2 = radius2 * radius2;

            // Now some mathematical magic I got out a book.
            double delrsq = rsq2 - rsq1;
            double sumrsq = rsq1 + rsq2;
            double root = 2.0*sumrsq*distsq - distsq*distsq - delrsq*delrsq;

            //	Check for no intersection.
            if (root < Constants.TINY)
                return 0;

            // We have at least one intersection.
            double dstinv = 0.5 / distsq;
            double scl = 0.5 - delrsq*dstinv;
            double x = dx*scl + xc1;
            double y = dy*scl + yc1;

            //	Is it tangential?
            if (root < Constants.TINY)
            {
                results.Append(x, y, 0.0);
                return 1;
            }

            // Figure out 2 intersections.
            root = dstinv * Math.Sqrt(root);
            double xfac = dx*root;
            double yfac = dy*root;

            results.Append(x-yfac, y+xfac, 0.0);
            results.Append(x+yfac, y-xfac, 0.0);
            return 2;
        }
    }
}
