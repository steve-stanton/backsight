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
using System.ComponentModel;

namespace Backsight
{
    /// <summary>
    /// A const ground position
    /// </summary>
    /// <seealso cref="IEditPosition"/>
    [TypeConverter(typeof(StringConverter))]
    public interface IPosition
    {
        /// <summary>
        /// The easting value for this position.
        /// </summary>
        double X { get; }

        /// <summary>
        /// The northing value for this position.
        /// </summary>
        double Y { get; }

        /// <summary>
        /// Does this position coincide with another one?
        /// </summary>
        /// <param name="p">The position to compare with</param>
        /// <param name="tol">Tolerance to use for comparison</param>
        /// <returns>True if this position is at the same position (within
        /// the specified tolerance)</returns>
        bool IsAt(IPosition p, double tol);
    }
}
