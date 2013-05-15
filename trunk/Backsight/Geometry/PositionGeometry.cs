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

namespace Backsight.Geometry
{
    /// <summary>
    /// Implementation of point geometry based on the <see cref="Position"/> class.
    /// </summary>
    /// <remarks>This class is provided for use by sibling classes in this project.
    /// </remarks>
    public class PositionGeometry : Position, IPointGeometry
    {
        #region Class data

        // None

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>PositionGeometry</c> from the supplied position (or casts
        /// the supplied position if it's already an instance of <c>PositionGeometry</c>).
        /// </summary>
        /// <param name="p">The position the geometry should correspond to</param>
        /// <returns>A newly created <c>PositionGeometry</c> instance, or the supplied
        /// position if it's already an instance of <c>PositionGeometry</c></returns>
        public static PositionGeometry Create(IPosition p)
        {
            if (p is PositionGeometry)
                return (p as PositionGeometry);
            else
                return new PositionGeometry(p.X, p.Y);
        }

        /// <summary>
        /// Creates a new <c>PositionGeometry</c> at the specified position.
        /// (not rounded in any way).
        /// </summary>
        /// <param name="x">The easting of the point, in meters on the ground.</param>
        /// <param name="y">The northing of the point, in meters on the ground.</param>
        public PositionGeometry(double x, double y)
            : base(x, y)
        {
        }

        #endregion

        #region IPointGeometry Members

        /// <summary>
        /// Is this point at the same position as another point.
        /// </summary>
        /// <param name="p">The point to compare with</param>
        /// <returns>True if the positions are identical (to the nearest micron)</returns>
        public bool IsCoincident(IPointGeometry p)
        {
            return this.IsAt(p, 0.000001); // 1 micron is close enough
        }

        /// <summary>
        /// The X position of this point.
        /// </summary>
        public ILength Easting
        {
            get { return new Length(X); }
        }

        /// <summary>
        /// The Y position of this point.
        /// </summary>
        public ILength Northing
        {
            get { return new Length(Y); }
        }

        #endregion
    }
}
