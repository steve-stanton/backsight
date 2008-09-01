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
using System.Collections.Generic;

using Backsight.Environment;

namespace Backsight
{
    /// <summary>
    /// The data model for geographic space (typically some sort of map)
    /// </summary>
    public interface ISpatialModel : IExpandablePropertyItem
    {
        /// <summary>
        /// Draws this model on the specified display
        /// </summary>
        /// <param name="display">The display to render to</param>
        /// <param name="style">The display style to use</param>
        void Render(ISpatialDisplay display, IDrawStyle style);

        /// <summary>
        /// Is this model currently empty?
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// The ground extent of the data in the model (null if the model is empty)
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

        /// <summary>
        /// The coordinate system for model positions.
        /// </summary>
        ISpatialSystem SpatialSystem { get; }
    }
}
