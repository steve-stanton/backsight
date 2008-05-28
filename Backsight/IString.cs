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
using System.Drawing;

using Backsight.Geometry;

namespace Backsight
{
    public interface IString
    {
        /// <summary>
        /// The text string.
        /// </summary>
        string Text { get; }

        /// <summary>
        /// Clockwise rotation from horizontal
        /// </summary>
        IAngle Rotation { get; }

        /// <summary>
        /// The position of the top-left corner of the string.
        /// </summary>
        PointGeometry Position { get; }

        /// <summary>
        /// A closed outline that surrounds the string.
        /// </summary>
        IPosition[] Outline { get; }

        /// <summary>
        /// Creates the font used to present the string.
        /// </summary>
        /// <param name="display">The display on which the string will be displayed</param>
        /// <returns>The corresponding font (may be null if the font is too small to be drawn)</returns>
        Font CreateFont(ISpatialDisplay display);
    }
}
