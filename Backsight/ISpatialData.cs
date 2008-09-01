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
    /// A data source for a map.
    /// </summary>
    public interface ISpatialData
    {
        /// <summary>
        /// A name for the data source (may be null).
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Draws the content of the map on a display.
        /// </summary>
        /// <param name="display">The display on which to draw</param>
        /// <param name="style">The display style to use</param>
        void Render(ISpatialDisplay display, IDrawStyle style);

        /// <summary>
        /// Is the map currently empty?
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// The ground extent of the map.
        /// </summary>
        IWindow Extent { get; }

        /// <summary>
        /// Locates the object closest to a specific position.
        /// </summary>
        /// <param name="p">The search position</param>
        /// <param name="radius">The search radius</param>
        /// <param name="types">The type(s) of object to look for</param>
        /// <returns>The closest object of the requested type (null if nothing found)</returns>
        ISpatialObject QueryClosest(IPosition p, ILength radius, SpatialType types);
    }
}
