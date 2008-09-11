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

namespace Ntx
{
    /// <summary>
    /// A two- or three-dimensional position (on a map projection).
    /// </summary>
    public class Position
    {
        #region Class data

        /// <summary>
        /// The X-position
        /// </summary>
        double m_Easting;

        /// <summary>
        /// The Y-position
        /// </summary>
        double m_Northing;

        /// <summary>
        /// The Z-position. Meaningful only if <c>m_Is3D</c> is true (it will be
        /// 0.0 if it's not a 3D position).
        /// </summary>
        float m_Elevation;

        /// <summary>
        /// Is this a 3D position? If true, the value of <c>m_Elevation</c> should
        /// be meaningful.
        /// </summary>
        bool m_Is3D;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new 2D position at (0N,0E).
        /// </summary>
        internal Position() : this(0.0, 0.0)
        {
        }

        /// <summary>
        /// Creates a new 2D position at the specified position.
        /// </summary>
        /// <param name="x">The easting of the position</param>
        /// <param name="y">The northing of the position</param>
        internal Position(double x, double y)
        {
            m_Easting = x;
            m_Northing = y;
            m_Elevation = 0.0F;
            m_Is3D = false;
        }

        /// <summary>
        /// Creates a new 3D position at the specified position.
        /// </summary>
        /// <param name="x">The easting of the position</param>
        /// <param name="y">The northing of the position</param>
        /// <param name="z">The elevation of the position</param>
        internal Position(double x, double y, float z)
        {
            m_Easting = x;
            m_Northing = y;
            m_Elevation = z;
            m_Is3D = true;
        }

        #endregion

        /// <summary>
        /// The easting (x-value) of this position.
        /// </summary>
        public double Easting
        {
            get { return m_Easting; }
        }

        /// <summary>
        /// The northing (y-value) of this position.
        /// </summary>
        public double Northing
        {
            get { return m_Northing; }
        }

        /// <summary>
        /// The elevation (z-value) of this position (0.0 if the <see cref="Is3D"/>
        /// property is false).
        /// </summary>
        float Elevation
        {
            get { return m_Elevation; }
        }

        /// <summary>
        /// Is this a 3D position?
        /// </summary>
        bool Is3D
        {
            get { return m_Is3D; }
        }

        /// <summary>
        /// Does this position coincide with the specified position?
        /// </summary>
        /// <param name="that">The position to compare with</param>
        /// <returns>True if the positions match.</returns>
        /// <remarks>
        /// This uses an arbitrary point match tolerance of 1 micron on the ground,
        /// assuming that no-one stores NTX data to a better resolution than that.
        /// </remarks>
        bool IsAt(Position that)
        {
            return Math.Abs(this.m_Easting - that.m_Easting) < 0.000001 &&
                   Math.Abs(this.m_Northing - that.m_Northing) < 0.000001;
        }

    }
}
