using Backsight.Forms;

namespace Backsight.Editor.Forms;

public class MapDisplay : IMapDisplay
{
    private readonly MapControl _display;
    private readonly DrawStyle _style;

    public MapDisplay(MapControl display, DrawStyle style)
    {
        _display = display;
        _style = style;
    }

    public void Draw(ISpatialObject o)
    {
        o.Draw(this);
    }

    /// <inheritdoc cref="IMapDisplay.DrawCircle"/>
    public void DrawCircle(IPosition center, double radius)
    {
        _style.Render(_display, center, radius);
    }

    /// <inheritdoc cref="IMapDisplay.DrawPoint"/>
    public void DrawPoint(IPosition position)
    {
        _style.Render(_display, position);
    }

    /// <inheritdoc cref="IMapDisplay.DrawArc"/>
    public void DrawArc(IClockwiseCircularArcGeometry arc)
    {
        _style.Render(_display, arc);
    }

    /// <inheritdoc cref="IMapDisplay.DrawSegment"/>
    public void DrawSegment(IPosition start, IPosition end)
    {
        _style.Render(_display, start, end);
    }

    /// <inheritdoc cref="IMapDisplay.DrawMultiSegment"/>
    public void DrawMultiSegment(IEnumerable<IPosition> line)
    {
        _style.Render(_display, line);
    }

    /// <inheritdoc cref="IMapDisplay.Extent"/>
    public IWindow Extent => _display.Extent;

    /// <inheritdoc cref="IMapDisplay.PaintNow"/>
    public void PaintNow()
    {
        _display.PaintNow();
    }
    
    internal MapControl Display => _display;
    internal DrawStyle Style => _style;
}