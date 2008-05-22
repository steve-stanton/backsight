/// <remarks>
/// Copyright 2008 - Steve Stanton. This file is part of Backsight
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
using System.Xml.Serialization;

namespace Backsight.Xml
{
    /// <summary>
    /// Data class for the <see cref="TextGeometry"/> class.
    /// </summary>
    [XmlType("TextGeometry")]
    [XmlRoot(Namespace = "Backsight")]
    public class TextGeometryData
    {
        public int FontId;
        public float Width;
        public float Height;
        public double Rotation;
        public PointGeometryData Position;
    }
}
