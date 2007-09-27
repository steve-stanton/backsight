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
    /// <written by="Steve Stanton" on="01-AUG-2007" />
    /// <summary>
    /// Geometry for a line that connects two points.
    /// </summary>
    [Serializable]
    class SegmentGeometry : LineGeometry, ILineSegmentGeometry, ISectionBase
    {
        #region Class data

        // none

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>LineSegment</c> using the supplied points.
        /// </summary>
        /// <param name="start">The point at the start of the line segment.</param>
        /// <param name="end">The point at the end of the line segment.</param>
        internal SegmentGeometry(ITerminal start, ITerminal end)
            : base(start, end)
        {
        }

        #endregion

        public override ILength Length
        {
            get { return LineSegmentGeometry.GetLength(this); }
        }

        public override IWindow Extent
        {
            get { return LineSegmentGeometry.GetExtent(this); }
        }

        public override ILength Distance(IPosition point)
        {
            return LineSegmentGeometry.GetDistance(this, point);
        }

        internal override bool GetPosition(ILength dist, out IPosition pos)
        {
            return LineSegmentGeometry.GetPosition(this, dist.Meters, out pos);
        }
        internal override uint IntersectSegment(IntersectionResult results, ILineSegmentGeometry that)
        {
            return IntersectionHelper.Intersect(results, (ILineSegmentGeometry)this, that);
        }

        internal override uint IntersectMultiSegment(IntersectionResult results, IMultiSegmentGeometry that)
        {
            return IntersectionHelper.Intersect(results, (ILineSegmentGeometry)this, that);
        }

        internal override uint IntersectArc(IntersectionResult results, ICircularArcGeometry that)
        {
            return IntersectionHelper.Intersect(results, (ILineSegmentGeometry)this, that);
        }

        internal override uint IntersectCircle(IntersectionResult results, ICircleGeometry that)
        {
            return IntersectionHelper.Intersect(results, (ILineSegmentGeometry)this, that);
        }

        /// <summary>
        /// Calculates the distance from the start of this line to a specific position (on the map projection)
        /// </summary>
        /// <param name="asFarAs">Position on the line that you want the length to. Specify
        /// null for the length of the whole line.</param>
        /// <returns>The length. Less than zero if a position was specified and it is
        /// not on the line.</returns>
        internal override ILength GetLength(IPosition asFarAs)
        {
            if (asFarAs==null)
                return Length;
            else
                // assume it's on the line
                return new Length(Geom.Distance(Start, asFarAs));
        }

        internal override IPosition GetOrient(bool fromStart, double crvDist)
        {
            if (fromStart)
                return End;
            else
                return Start;
        }

        internal override void Render(ISpatialDisplay display, IDrawStyle style)
        {
            LineSegmentGeometry.Render(this, display, style);
        }

        /// <summary>
        /// The line geometry that corresponds to a section of this line.
        /// </summary>
        /// <param name="s">The required section</param>
        /// <returns>The corresponding geometry for the section (<b>not</b> an instance of
        /// <see cref="SectionGeometry"/>)</returns>
        public LineGeometry Section(ISection s) // implements ISectionBase
        {
            return new SegmentGeometry(s.From, s.To);
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
            IPosition start = Start;
            IPosition end = End;

            // Define the window.
            win = new Window(start, end);

            // Define line length
            length = BasicGeom.Distance(start, end);

            // Define area to the left of the line.
            // Uses the mid-X of the segment to get the area left (signed).
            // If the line is directed up the way, it contributes a negative
            // area. If directed down, it contributes a positive area. So,
            // if flat, it contributes nothing.
            double dy = start.Y - end.Y;
            area = dy * 0.5 * (start.X + end.X);
        }

        /// <summary>
        /// Gets the most easterly position of this segment. If both positions have the same easting,
        /// one of them will be picked arbitrarily.
        /// </summary>
        /// <returns>The most easterly position</returns>
        internal override IPosition GetEastPoint()
        {
            if (Start.Easting.Microns > End.Easting.Microns)
                return Start;
            else
                return End;
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
            // The end of the horizontal segment should be coincident with this segment.
            IPosition e = hr.End;
            if (this.Distance(e).Meters > Constants.XYTOL)
                return Side.Unknown;

            // What's the northing of the horizontal segment guy?
            double hy = hr.Y;

            // Scan the remainder of the line, looking for a point which is
            // off the horizontal. If below the horizontal, it's to the right
            // of the line (& above=>left).

            double y = End.Y;
            if ((hy-y) > Constants.TINY) // y<hy
                return Side.Right;
            if ((y-hy) > Constants.TINY) // y>hy
                return Side.Left;

            // If we didn't find any suitable points, try traversing back down
            // the line, and adopt the reverse logic for figuring out the side.

            y = Start.Y;
            if ((y-hy) > Constants.TINY) // y>hy
                return Side.Right;
            if ((hy-y) > Constants.TINY) // y<hy
                return Side.Left;

            // The line is completely horizontal, so we have an intersection
            // at the start or the end of the line. This should have already
            // been caught, so we're fucked now.

            return Side.Unknown;
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

            // Remember the initial end of segment
            PointGeometry initEnd = new PointGeometry(e);

            // Represent the horizontal segment in a class of its own
            HorizontalRay hseg = new HorizontalRay(s, e.X-s.X);
            if (!hseg.IsValid)
            {
                status = 1;
                return false;
            }

            // Get relative position code for the start of the line. If it's
            // somewhere on the horizontal segment, cut the line back.
            byte scode = Geom.GetPositionCode(Start, s, e);
            if (scode==0)
                e = new PointGeometry(Start);

            // Get the position code for the end of the line segment
            byte ecode = Geom.GetPositionCode(End, s, e);

            // If it's coincident with the horizontal segment, cut the
            // line back. Otherwise see whether there is any potential
            // intersection to cut back to.

            if (ecode==0)
                e = new PointGeometry(End);
            else if ((scode & ecode)==0)
            {
                IPosition x = null;
                if (hseg.Intersect(Start, End, ref x))
                    e = new PointGeometry(x);
            }

            // Return flag to indicate whether we got closer or not.
            return (!e.IsCoincident(initEnd));
        }

        /// <summary>
        /// Loads a list of positions with data for this line.
        /// </summary>
        /// <param name="positions">The list to append to</param>
        /// <param name="reverse">Should the data be appended in reverse order?</param>
        /// <param name="wantFirst">Should the first position be appended? (last if <paramref name="reverse"/> is true)</param>
        /// <param name="arcTol">Tolerance for approximating circular arcs (not used)</param>
        internal override void AppendPositions(List<IPosition> positions, bool reverse, bool wantFirst, ILength arcTol)
        {
            if (reverse)
            {
                if (wantFirst)
                    positions.Add(End);

                positions.Add(Start);
            }
            else
            {
                if (wantFirst)
                    positions.Add(Start);

                positions.Add(End);
            }
        }

        internal override uint Intersect(IntersectionResult results)
        {
            return results.IntersectSegment(this);
        }
    }
}
