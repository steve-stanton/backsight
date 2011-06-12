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
    /// <written by="Steve Stanton" on="03-JUL-2007" />
    /// <summary>
    /// Essential coordinate system methods required by Backsight.
    /// </summary>
    public interface ISpatialSystem
    {
        /// <summary>
        /// A name for the coordinate system.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The EPSG number for the system (0 if not known).
        /// </summary>
        int EpsgNumber { get; }

        /// <summary>
        /// The WKT that defines the coordinate system.
        /// </summary>
        string GetWellKnownText();

        /// <summary>
        /// The mean elevation , in meters
        /// </summary>
        ILength MeanElevation { get; set; }

        /// <summary>
        /// Geoid separation, in meters
        /// </summary>
        ILength GeoidSeparation { get; set; }

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
