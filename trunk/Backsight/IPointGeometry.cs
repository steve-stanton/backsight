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
	/// <written by="Steve Stanton" on="01-NOV-2006" />
    /// <summary>
    /// The geometry for a point.
    /// </summary>
    public interface IPointGeometry : IPosition
    {
        /// <summary>
        /// Is this point at the same position as another point.
        /// </summary>
        /// <param name="p">The point to compare with</param>
        /// <returns>True if the positions are identical</returns>
        /// <remarks>
        /// When dealing with implementations that involve floating point numbers,
        /// the comparison will probably involve some sort of tolerance (perhaps
        /// an appropriate epsilon value, or perhaps a value that is consistent
        /// with the resolution of an associated map model). If that is the case,
        /// you may prefer to be explicit about the tolerance used by making use
        /// of the <c>IPosition.IsAt</c> method.
        /// </remarks>
        bool IsCoincident(IPointGeometry p);

        /// <summary>
        /// The X position of this point.
        /// </summary>
        ILength Easting { get; }

        /// <summary>
        /// The Y position of this point.
        /// </summary>
        ILength Northing { get; }
    }
}
