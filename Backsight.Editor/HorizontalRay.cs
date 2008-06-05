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

namespace Backsight.Editor
{
	/// <written by="Steve Stanton" on="04-AUG-1997" was="CeHSegment" />
    /// <summary>
    /// A horizontal line segment, directed from the west to the east. Used when
    /// performing point in polygon tests.
    /// </summary>
    class HorizontalRay
    {
        #region Class data

        readonly IPointGeometry m_Start;
        double m_EndX;

        #endregion

        #region Constructors

        internal HorizontalRay(IPosition start, double distance)
        {
            if (distance < 0.0)
                throw new ArgumentOutOfRangeException();

            m_Start = PointGeometry.Create(start);
            m_EndX = start.X + distance;
        }

        #endregion

        internal IPointGeometry Start
        {
            get { return m_Start; }
        }

        internal IPointGeometry End
        {
            get { return new PointGeometry(m_EndX, m_Start.Y); }
        }

        internal double StartX
        {
            get { return m_Start.X; }
        }

        internal double EndX
        {
            get { return m_EndX; }
        }

        internal double Y
        {
            get { return m_Start.Y; }
        }

        /// <summary>
        /// Intersects this horizontal ray with a vector.
        /// 
        /// If the vector is partially coincident with the ray, you get the
        /// intersection which is closest to the start of the vector.
        /// 
        /// This function does NOT do any quick checks before doing those
        /// nasty floating point calculations. Do get some modicum of efficiency,
        /// do it yourself prior to calling this function!
        /// </summary>
        /// <param name="vs">Start of vector</param>
        /// <param name="ve">End of vector</param>
        /// <param name="xsect">Any intersection (only defined if there is one)</param>
        /// <returns>True if an intersection was found</returns>
        internal bool Intersect(IPosition vs, IPosition ve, ref IPosition xsect)
        {
            // Ensure that an intersection is feasible
	        if (Math.Max(vs.Y,ve.Y) < this.Y || Math.Min(vs.Y,ve.Y) > this.Y)
                return false;

            double vsx = vs.X;
	        double vsy = vs.Y;
	        double vex = ve.X;
	        double vey = ve.Y;

            // Get the deltas of the vector
	        double dx = vex - vsx;
	        double dy = vey - vsy;

            // If the vector is also horizontal
	        if (Math.Abs(dy) < Constants.TINY )
            {
                // Return if there is no overlap (we already know that the Y's match).

		        double minx = Math.Min(vsx, vex);
                if (minx > m_EndX)
                    return false;

		        double maxx = Math.Max(vsx, vex);
		        if (maxx < this.StartX)
                    return false;

                // Get the intersection closest to the start of the horizontal segment.

		        if (minx < m_Start.X)
                    xsect = new Position(this.StartX, this.Y);
                else
                    xsect = new Position(minx, this.Y);

        		return true;
            }

            // Get the delta between the start of the vector and the horizontal segment,
            // and figure out the intersection
		    double sdy = this.Y - vsy;
		    double xi = ((sdy*dx)/dy) + vsx;

            // Check whether the intersection is ON the horizontal segment
		    if (xi>=this.StartX && xi<=this.EndX)
            {
                xsect = new Position(xi, this.Y);
                return true;
    		}

            return false;
        }

        /// <summary>
        /// Intersects this ray with a circle. This will return 0, 1, or 2 intersections.
        /// If two intersections are found, they will be ordered so that the first one
        /// is the one to the west.
        /// </summary>
        /// <param name="circle">The circle to intersect with.</param>
        /// <param name="x1">First intersection (if any).</param>
        /// <param name="x2">Second intersection (if any).</param>
        /// <returns>The number of intersections found.</returns>
        internal uint Intersect(Circle circle, ref IPosition x1, ref IPosition x2)
        {
            // Get northing of the ray with respect to the circle centre.
	        double dy = this.Y - circle.Center.Y;

            // No intersections if the delta-Y exceeds the radius
            double radius = circle.Radius;
	        if (Math.Abs(dy) > radius)
                return 0;

            // Substitute the delta-Y in the equation of the circle.
	        double dist = Math.Sqrt(radius*radius - dy*dy);

            // No intersections so far.
	        uint nx = 0;

            // If the western intersection falls on this segment, remember it's location.

	        double minx = this.StartX;
	        double maxx = this.EndX;
	        double cx = circle.Center.X;
	        double x = cx-dist;

	        if (x>=minx && x<=maxx)
            {
		        x1 = new Position(x, this.Y);
		        nx++;
	        }

            // Similarly for the eastern intersection.

        	x = cx+dist;
	        if (x>=minx && x<=maxx)
            {
		        if ( nx==0 )
			        x1 = new Position(x, this.Y);
		        else
			        x2 = new Position(x, this.Y);

		        nx++;
	        }

            return nx;
        }

        /// <summary>
        /// Gets the side code for a horizontal segment which is incident on a node,
        /// given any one the boundaries incident on the node.
        /// </summary>
        /// <param name="id">The divider we know about (incident on node).</param>
        /// <param name="isStart">True if it's the start of <c>ib</c>.</param>
        /// <param name="od">The divider the returned side refers to</param>
        /// <returns>Side.Left if the segment is to the left of <c>ob</c>, Side.Right
        /// if segment to the right.</returns>
        internal Side GetSide(IDivider id, bool isStart, out IDivider od)
        {
            ConnectionFinder connect = new ConnectionFinder(id, isStart, this);
            od = connect.Next;
            return (connect.IsStart ? Side.Right : Side.Left);
        }

        /// <summary>
        /// Confirms that this object is valid. It MUST be horizontal, and the initial point
        /// must be west of the second point.
        /// </summary>
        /// <returns></returns>
        internal bool IsValid
        {
            get { return (m_EndX - m_Start.X) >= 0.0; }
        }

        public override string  ToString()
        {
            return String.Format("{0}N,{1}E -> {2}E", m_Start.Y, m_Start.X, m_EndX);
        }
    }
}
