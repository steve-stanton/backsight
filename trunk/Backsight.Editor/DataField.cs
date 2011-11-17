// <remarks>
// Copyright 2011 - Steve Stanton. This file is part of Backsight
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

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="17-NOV-2011"/>
    /// <summary>
    /// Numeric values that are used to identify the various data fields involved in
    /// the definition of edits.
    /// </summary>
    internal enum DataField : ushort
    {
        /// <summary>
        /// Easting
        /// </summary>
        X = 1,

        /// <summary>
        /// Northing
        /// </summary>
        Y = 2,

        /// <summary>
        /// Elevation
        /// </summary>
        Z = 3,

        /// <summary>
        /// Unique internal reference
        /// </summary>
        Id = 4,

        /// <summary>
        /// Reference to a point feature
        /// </summary>
        Point = 5,

        /// <summary>
        /// Reference to a line feature
        /// </summary>
        Line = 6,

        /// <summary>
        /// Reference to a text feature
        /// </summary>
        Text = 7,
    }
}
