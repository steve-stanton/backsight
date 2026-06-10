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

namespace Backsight;

/// <written by="Steve Stanton" on="08-JAN-2007" />
/// <summary>
/// A selection of spatial objects
/// </summary>
public interface ISpatialSelection
{
    /// <summary>
    /// The number of items in the selection
    /// </summary>
    int Count { get; }

    /// <summary>
    /// The one and only item in this selection (null if the selection is empty, or
    /// it contains more than one item).
    /// </summary>
    ISpatialObject? SingleOrDefault { get; }

    /// <summary>
    /// The items in the selection
    /// </summary>
    IEnumerable<ISpatialObject> Items { get; }

    /// <summary>
    /// Draws the content of this selection
    /// </summary>
    /// <param name="mapDisplay">The display to draw to</param>
    void Draw(IMapDisplay mapDisplay);
}