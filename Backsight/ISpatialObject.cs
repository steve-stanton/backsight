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

namespace Backsight
{
	/// <written by="Steve Stanton" on="14-SEP-2006" />
    /// <summary>
    /// Some sort of spatial object.
    /// </summary>
    /// <seealso cref="SpatialType"/>
    public interface ISpatialObject
    {
        /// <summary>
        /// Value denoting the spatial object type.
        /// </summary>
        SpatialType SpatialType { get; }

        /// <summary>
        /// Draws this object on the specified display
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The drawing style</param>
        void Render(ISpatialDisplay display, IDrawStyle style);

        /// <summary>
        /// The spatial extent of this object.
        /// </summary>
        IWindow Extent { get; }

        /// <summary>
        /// The shortest distance between this object and the specified position.
        /// </summary>
        /// <param name="point">The position of interest</param>
        /// <returns>The shortest distance between the specified position and this object</returns>
        ILength Distance(IPosition point);
    }
}
