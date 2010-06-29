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
	/// <written by="Steve Stanton" on="14-DEC-2006" />
    /// <summary>
    /// Identifies a spatial object type (or possible combination of types).
    /// </summary>
    [Flags]
    public enum SpatialType : byte
    {
        /// <summary>
        /// Point features
        /// </summary>
        Point = 0x01,

        /// <summary>
        /// Line features
        /// </summary>
        Line = 0x02,

        /// <summary>
        /// Text features
        /// </summary>
        Text = 0x04,

        /// <summary>
        /// Polygons
        /// </summary>
        Polygon = 0x08,

        /// <summary>
        /// Any sort of spatial feature (excludes polygons).
        /// </summary>
        Feature = (Point | Line | Text),

        /// <summary>
        /// Any type of spatial object (things that implement <c>ISpatialObject</c>)
        /// </summary>
        /// <see>ISpatialObject</see>
        All = (Feature | Polygon),
    }
}
