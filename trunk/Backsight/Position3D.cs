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
    /// <written by="Steve Stanton" on="27-NOV-2007" />
    /// <summary>
    /// A 3D position
    /// </summary>
    public class Position3D : Position, IEditPosition3D
    {
        #region Class data

        /// <summary>
        /// The elevation for the position.
        /// </summary>
        double m_Z;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>Position3D</c> at the specified XYZ.
        /// </summary>
        /// <param name="x">The easting of the position</param>
        /// <param name="y">The northing of the position</param>
        /// <param name="z">The elevation of the position</param>
        public Position3D(double x, double y, double z)
            : base(x,y)
        {
            m_Z = z;
        }

        #endregion

        /// <summary>
        /// The elevation for the position.
        /// </summary>
        public double Z
        {
            get { return m_Z; }
            set { m_Z = value; }
        }
    }
}
