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
    /// The geometry for a line.
    /// </summary>
    public interface ILineGeometry
    {
        /// <summary>
        /// The position of the start of the line.
        /// </summary>
        IPointGeometry Start { get; }

        /// <summary>
        /// The position of the end of the line.
        /// </summary>
        IPointGeometry End { get; }

        /// <summary>
        /// The length of the line (on the map projection).
        /// </summary>
        ILength Length { get; }

        /// <summary>
        /// The spatial extent of the line.
        /// </summary>
        IWindow Extent { get; }

        /// <summary>
        /// The shortest distance from this line to the specified position.
        /// </summary>
        /// <param name="point">The position of interest</param>
        /// <returns>The perpendicular distance from the position to the line (if
        /// the perpendicular isn't on the line, you'll get the distance to the
        /// closest end point)</returns>
        ILength Distance(IPosition point);
    }
}
