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

namespace Backsight.Editor
{
    interface ICoordinateSystem : ISpatialSystem, IExpandablePropertyItem
    {
        // This isn't exactly generic because it currently defines only those
        // methods the CadastralEditor needs to work with UTM.

        double MeanElevation { get; set;  }
        double GeoidSeparation { get; }
        byte Zone { get; }
        string Ellipsoid { get; }
        string Projection { get; }

        /// <summary>
        /// Converts a projected position into geographic
        /// </summary>
        /// <param name="p">The XY position to convert</param>
        /// <returns>The corresponding geographic position (longitude is X, latitude is Y)</returns>
        IPosition GetGeographic(IPosition p);

        /// <summary>
        /// Obtains a scale factor (multiplier) that may be applied to ground distances,
        /// to reduce them to the mapping projection.
        /// </summary>
        /// <param name="start">The starting XY position</param>
        /// <param name="end">The terminating XY position</param>
        /// <returns>The scale multiplier for converting ground distances</returns>
        double GetLineScaleFactor(IPosition start, IPosition end);

        /// <summary>
        /// Calculates the area of a closed shape on the ground.
        /// </summary>
        /// <param name="v">The positions defining the shape, on the map projection</param>
        /// <returns>The true ground area (in the units of the coordinate system)</returns>
        double GetGroundArea(IPosition[] v);
    }
}
