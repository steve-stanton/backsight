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

namespace Backsight.Editor
{
    interface ICoordinateSystem : ISpatialSystem, IExpandablePropertyItem
    {
        // This isn't exactly generic because it currently defines only those
        // methods the CadastralEditor needs to work with UTM.

        double MeanElevation { get; }
        double GeoidSeparation { get; }
        double ScaleFactor { get; }
        byte Zone { get; }
        string Ellipsoid { get; }
        string Projection { get; }

        void GetLatLong(IPosition p, out double latitude, out double longitude);
        double GetLineScaleFactor(IPosition start, IPosition end);
        double GetGroundArea(IPosition[] v);
    }
}
