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

using Backsight.Geometry;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="06-FEB-2008" />
    /// <summary>
    /// A collection of positions defining a closed shape (without any islands).
    /// The last position must coincide with the first position. At least 3 positions.
    /// </summary>
    class ClosedShape : LineStringGeometry
    {
        #region Class data

        /// <summary>
        /// The extent of the closed shape (since it's quite likely to be required on
        /// a frequent basis, it's retained here only to save on computation time).
        /// </summary>
        readonly IWindow m_Extent;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>ClosedShape</c> using the supplied positions.
        /// </summary>
        /// <param name="edge">The positions defining the edge of the closed shape</param>
        internal ClosedShape(IPosition[] edge)
            : base(edge)
        {
            // Don't use the supplied IPosition array, since individual positions may have
            // been rounded to the nearest micron.
            IPointGeometry[] data = this.Data;
            if (data.Length < 3)
                throw new ArgumentException("Not enough positions for a closed shape");

            IPointGeometry last = data[data.Length-1];
            if (!data[0].IsCoincident(last))
                throw new ArgumentException("Closed shape is not closed");

            m_Extent = new Window(this.Data);
        }

        #endregion

        /// <summary>
        /// The extent of the closed shape. This is a stored value (cheap to access).
        /// </summary>
        public override IWindow Extent
        {
            get { return m_Extent; }
        }

        /// <summary>
        /// Checks whether this closed shape overlaps (encloses) a point.
        /// </summary>
        /// <param name="point">The position to check</param>
        /// <returns>True if the shape overlaps the point</returns>
        internal bool IsOverlap(IPointGeometry point)
        {
            return (m_Extent.IsOverlap(point) && Geom.IsPointInClosedShape(this.Data, point));
        }

        /// <summary>
        /// Checks whether this closed shape overlaps a line
        /// </summary>
        /// <param name="shape">Positions defining a closed outline (the last position
        /// must coincide with the first position). At least 3 positions.</param>
        /// <param name="shapeWindow">The extent of <paramref name="shape"/></param>
        /// <param name="line">The line to compare with the shape</param>
        /// <returns>True if the shape overlaps the line</returns>
        internal bool IsOverlap(LineGeometry line)
        {
            // Get the window of the line
            IWindow lwin = line.Extent;

            // The two windows have to overlap.
            if (!m_Extent.IsOverlap(lwin))
                return false;

            // If either end of the line falls inside this shape, we've got overlap.
            IPointGeometry p = line.Start;
            if (m_Extent.IsOverlap(p) && Geom.IsPointInClosedShape(this.Data, p))
                return true;

            p = line.End;
            if (m_Extent.IsOverlap(p) && Geom.IsPointInClosedShape(this.Data, p))
                return true;

            // The only thing left is a possible interesection between
            // the shape and the line.
            return IsIntersect(line);
        }

        /// <summary>
        /// Checks whether this closed shape intersects (overlaps) a line.
        /// This assumes that a window-window overlap has already been checked for.
        /// </summary>
        /// <param name="line">The line to compare with the shape</param>
        /// <returns>True if intersection found.</returns>
        bool IsIntersect(LineGeometry line)
        {
            //IntersectionResult xres = new IntersectionResult(line);
            //return (xres.IntersectMultiSegment(this) > 0);

            IntersectionResult xres = new IntersectionResult(line);

            // Intersect each segment of this shape with the line.
            IPointGeometry[] data = this.Data;
            for (int i=1; i<data.Length; i++)
            {
                LineSegmentGeometry thisSeg = new LineSegmentGeometry(data[i-1], data[i]);
                if (xres.IntersectSegment(thisSeg) > 0)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Does this closed shape overlap another closed shape?
        /// </summary>
        /// <param name="that">The closed shape to compare with</param>
        /// <returns>True if the shapes overlap</returns>
        /// <remarks>It should be more efficient to make <c>this</c> closed shape the
        ///	smaller one.</remarks>
        internal bool IsOverlap(ClosedShape that)
        {
            // The window of this shape must overlap the window of the other shape
            if (!m_Extent.IsOverlap(that.Extent))
                return false;

	        // If any position in this shape falls inside the other one, we've got overlap.
            foreach (IPointGeometry p in this.Data)
            {
                if (that.IsOverlap(p))
                    return true;
            }

            // NONE of the positions in this shape fall inside the
            // other shape. So check if any segment intersects the other shape.

            // To try to maximize efficiency, compare the shape that
            // has the larger spatial extent. That way, we should be
            // able to eliminate the max number of segments with a
            // simple segment-window test.

            double areaThis = this.Extent.Width * this.Extent.Height;
            double areaThat = that.Extent.Width * that.Extent.Height;

            if (areaThis > areaThat)
                return this.IsIntersect(that);
            else
                return that.IsIntersect(this);
        }

        /// <summary>
        /// Does any edge of this closed shape intersect the edge of another shape.
        /// This assumes that a window-window overlap has already been checked for.
        /// </summary>
        /// <param name="that">The closed shape to compare with (preferably the one
        /// with the smaller spatial extent).
        /// </param>
        /// <returns>True if the edges of the shapes intersect</returns>
        bool IsIntersect(ClosedShape that)
        {
            IPointGeometry[] data = this.Data;

            for (int i=1; i<data.Length; i++)
            {
                if (that.IsOverlap(data[i-1], data[i]))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Does any edge of this closed shape intersect a line segment?
        /// </summary>
        /// <param name="ps">The start of the segment</param>
        /// <param name="pe">The end of the segment</param>
        /// <returns>True if any edge of this shape intersects the segment</returns>
        bool IsOverlap(IPointGeometry ps, IPointGeometry pe)
        {
            Window segwin = new Window(ps, pe);
            if (!m_Extent.IsOverlap(segwin))
                return false;

            // Define the test segment.
            ITerminal ts = new FloatingTerminal(ps);
            ITerminal te = new FloatingTerminal(pe);
            SegmentGeometry seg = new SegmentGeometry(ts, te);

            return IsIntersect(seg);
        }
    }
}
