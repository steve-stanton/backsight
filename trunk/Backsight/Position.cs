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

namespace Backsight
{
    /// <summary>
    /// A two-dimensional position on the ground
    /// </summary>
    public class Position : IEditPosition
    {
        #region Statics

        /// <summary>
        /// Creates a new <c>Position</c> at the midpoint of a pair of positions
        /// </summary>
        /// <param name="p">First point</param>
        /// <param name="q">Second point</param>
        /// <returns>The mid-position</returns>
        public static Position CreateMidpoint(IPosition p, IPosition q)
        {
            double x = p.X + 0.5 *(q.X - p.X);
            double y = p.Y + 0.5 *(q.Y - p.Y);
            return new Position(x, y);
        }
        #endregion

        #region Class data
        private double m_X;
        private double m_Y;
        #endregion

        #region Constructors

        /// <summary>
        /// Create a new <c>Position</c> at the specified XY.
        /// </summary>
        /// <param name="x">The easting of the position</param>
        /// <param name="y">The northing of the position</param>
        public Position(double x, double y)
        {
            m_X = x;
            m_Y = y;
        }

        public Position(IPosition p)
        {
            m_X = p.X;
            m_Y = p.Y;
        }

        #endregion

        public override string ToString()
        {
            return String.Format("{0:0.0000}N {1:0.0000}E", m_Y, m_X);
        }

        public double X
        {
            get { return m_X; }
            set { m_X = value; }
        }

        public double Y
        {
            get { return m_Y; }
            set { m_Y = value; }
        }

        public bool IsAt(IPosition p, double tol)
        {
            return Position.IsCoincident(this, p, tol);
        }

        public static bool IsCoincident(IPosition p, IPosition q, double tol)
        {
            if (p==null || q==null)
                return false;

            if (Math.Abs(p.X - q.X) > tol)
                return false;

            if (Math.Abs(p.Y - q.Y) > tol)
                return false;

            return true;
        }

        /// <summary>
        /// Projects a position onto a circle.
        /// </summary>
        /// <param name="p">The position to project</param>
        /// <param name="centre">The position of the centre of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="cpos">The projected position.</param>
        /// <returns>True if vertex projected ok. False if this vertex coincides EXACTLY
        /// with the centre of the circle (in that case, you get back the position of
        /// the centre).</returns>
        public static bool GetCirclePosition(IPosition p, IPosition centre, double radius, out IPosition cpos)
        {
            // Get the deltas of this vertex with respect to the centre of the circle.
            double dx = p.X - centre.X;
            double dy = p.Y - centre.Y;
            double dist = Math.Sqrt(dx*dx + dy*dy);

            // Return if the vertex is coincident with the centre.
	        if (dist<MathConstants.TINY)
            {
		        cpos = centre;
		        return false;
	        }

	        // Get the factor for projecting the position.
	        double factor = radius/dist;

            // Figure out the position on the circle.
            cpos = new Position(centre.X + dx*factor, centre.Y+ dy*factor);
            return true;
        }
    }
}
