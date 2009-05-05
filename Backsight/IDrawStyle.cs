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
using System.Drawing;

namespace Backsight
{
	/// <written by="Steve Stanton" on="02-OCT-2006" />
    /// <summary>
    /// Methods for drawing geometry with a specific style.
    /// </summary>
    public interface IDrawStyle
    {
        /// <summary>
        /// Draws a line
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="line">The positions defining the line (expected to be at
        /// least two positions)</param>
        void Render(ISpatialDisplay display, IPosition[] line);

        /// <summary>
        /// Draws a point as a filled square.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="position">The position of the center of the point</param>
        void Render(ISpatialDisplay display, IPosition position);

        /// <summary>
        /// Draws a point as a plus sign.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="position">The position of the center of the point</param>
        void RenderPlus(ISpatialDisplay display, IPosition position);

        /// <summary>
        /// Draws a point as a triangle.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="position">The position of the center of the point</param>
        void RenderTriangle(ISpatialDisplay display, IPosition position);

        /// <summary>
        /// Draws an icon
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="position">The position for the center of the icon</param>
        /// <param name="icon">The icon to display</param>
        void Render(ISpatialDisplay display, IPosition position, Icon icon);

        /// <summary>
        /// Draws a circular arc
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="arc">The circular arc</param>
        void Render(ISpatialDisplay display, IClockwiseCircularArcGeometry arc);

        /// <summary>
        /// Draws a text string (annotation)
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="text">The item of text</param>
        void Render(ISpatialDisplay display, IString text);

        /// <summary>
        /// Draws a circle
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="center">The position of the center of the circle</param>
        /// <param name="radius">The radius of the circle, in meters on the ground</param>
        void Render(ISpatialDisplay display, IPosition center, double radius);

        /// <summary>
        /// Fills a polygon, possibly including holes.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="outlines">The outlines of one or more closed shapes. The first
        /// array corresponds to the outline of the enclosing polygon, while the
        /// remaining arrays correspond to islands.</param>
        void Render(ISpatialDisplay display, IPosition[][] outlines);

        /// <summary>
        /// The default height for point features (on the ground)
        /// </summary>
        ILength PointHeight { get; set; }

        /// <summary>
        /// The color used to fill things
        /// </summary>
        Color FillColor { get; set; }

        /// <summary>
        /// The fill for closed shapes
        /// </summary>
        IFill Fill { get; set; }

        /// <summary>
        /// The color used to draw lines
        /// </summary>
        Color LineColor { get; set; }

        /// <summary>
        /// The pen to use for drawing lines
        /// </summary>
        Pen Pen { get; set; }

        /// <summary>
        /// Is this a fixed style (meaning that color and fill should be retain their current values)
        /// </summary>
        bool IsFixed { get; set; }
    }
}
