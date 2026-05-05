namespace Backsight;

/// <summary>
/// A map display.
/// </summary>
public interface IMapDisplay
{
    /// <summary>
    /// Renders a spatial object on the map display.
    /// </summary>
    /// <param name="o">The item to be displayed.</param>
    void Draw(ISpatialObject o);
    
    /// <summary>
    /// Draws a circle.
    /// </summary>
    /// <param name="center">The position of the center of the circle</param>
    /// <param name="radius">The radius of the circle, in meters on the ground</param>
    void DrawCircle(IPosition center, double radius);

    /// <summary>
    /// Draws a point as a filled square.
    /// </summary>
    /// <param name="position">The position of the center of the point</param>
    void DrawPoint(IPosition position);

    /// <summary>
    /// Draws a circular arc.
    /// </summary>
    /// <param name="arc">The arc to display.</param>
    void DrawArc(IClockwiseCircularArcGeometry arc);
    
    /// <summary>
    /// Draws a simple line segment.
    /// </summary>
    /// <param name="start">The position for the start of the line.</param>
    /// <param name="end">The position for the end of the line.</param>
    void DrawSegment(IPosition start, IPosition end);
    
    /// <summary>
    /// Draws a line consisting of multiple segments.
    /// </summary>
    /// <param name="line">The positions defining the line (expected to be at least two positions).</param>
    void DrawMultiSegment(IEnumerable<IPosition> line);
    
    /// <summary>
    /// The spatial extent of the map display.
    /// </summary>
    IWindow Extent { get; }

    /// <summary>
    /// Attempts to immediately paint the content of any display buffer (meant to provide
    /// the user with feedback during protracted draws). 
    /// </summary>
    void PaintNow();
}