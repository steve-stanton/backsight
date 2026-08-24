using System.Collections.Generic;
using Backsight.Model;

namespace Backsight.Map.Editor.Mapping;

/// <written by="Steve Stanton" on="02-OCT-2006" />
/// <summary>
/// Methods for drawing geometry with a specific style.
/// </summary>
/// TODO: Is this obsolete?
interface IDrawStyle
{
    void Render(MapCanvas display, IPosition position);
/*
    /// <summary>
    /// Draws a point as a plus sign.
    /// </summary>
    /// <param name="display">The display to draw to</param>
    /// <param name="position">The position of the center of the point</param>
    void RenderPlus(MapCanvas display, IPosition position);

    /// <summary>
    /// Draws a point as a triangle.
    /// </summary>
    /// <param name="display">The display to draw to</param>
    /// <param name="position">The position of the center of the point</param>
    void RenderTriangle(MapCanvas display, IPosition position);

    /// <summary>
    /// Draws an icon
    /// </summary>
    /// <param name="display">The display to draw to</param>
    /// <param name="position">The position for the center of the icon</param>
    /// <param name="icon">The icon to display</param>
    void Render(MapCanvas display, IPosition position, Icon icon);
*/
    /// <summary>
    /// Draws a circular arc
    /// </summary>
    /// <param name="display">The display to draw to</param>
    /// <param name="arc">The circular arc</param>
    void Render(MapCanvas display, IClockwiseCircularArcGeometry arc);
/*
    /// <summary>
    /// Draws a text string (annotation)
    /// </summary>
    /// <param name="display">The display to draw to</param>
    /// <param name="text">The item of text</param>
    void Render(MapCanvas display, IString text);

    /// <summary>
    /// Draws a circle
    /// </summary>
    /// <param name="display">The display to draw to</param>
    /// <param name="center">The position of the center of the circle</param>
    /// <param name="radius">The radius of the circle, in meters on the ground</param>
    void Render(MapCanvas display, IPosition center, double radius);

    /// <summary>
    /// Fills a polygon, possibly including holes.
    /// </summary>
    /// <param name="display">The display to draw to</param>
    /// <param name="outlines">The outlines of one or more closed shapes. The first
    /// array corresponds to the outline of the enclosing polygon, while the
    /// remaining arrays correspond to islands.</param>
    void Render(MapCanvas display, IPosition[][] outlines);
    */
/*
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
    */
}
