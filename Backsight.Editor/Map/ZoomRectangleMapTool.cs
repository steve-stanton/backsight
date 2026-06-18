using Avalonia.Media;
using Backsight.Editor.Forms;
using Rectangle = Avalonia.Controls.Shapes.Rectangle;

namespace Backsight.Editor.Map;

class ZoomRectangleMapTool : MapWindowTool
{
    /// <summary>
    /// The anchor position  of the rectangle
    /// </summary>
    IPosition? _anchor;

    /// <summary>
    /// Rectangle corresponding to the current corners, in screen units.
    /// </summary>
    readonly Rectangle _screenRect;

    internal ZoomRectangleMapTool(MapWindow mapWindow)
        : base(mapWindow)
    {
        _screenRect = new Rectangle
        {
            Stroke = Brushes.Magenta,
            StrokeThickness = 2,
        };
    }

    public override int Id => (int)DisplayToolId.ZoomRectangle;

    public override bool Start()
    {
        MapWindow.SetCursor(EditingCursors.MagnifyingGlassCursor);
        MapWindow.MapOverlay.Children.Add(_screenRect);
        return true;
    }
    
    public override bool Finish()
    {
        MapWindow.MapOverlay.Children.Remove(_screenRect);
        return base.Finish();
    }
    
    public override void Escape()
    {
        MapWindow.MapOverlay.Children.Remove(_screenRect);
        base.Escape();
    }

    /// <summary>
    /// Handles a left click on the map by remembering the anchor point for a zoom by rectangle.
    /// </summary>
    /// <param name="p">The position where the left click occurred.</param>
    /// <param name="b">The mouse button that was clicked (ignored).</param>
    public override void MouseDown(IPosition p, MouseButton b)
    {
        _anchor = p;
    }

    /// <summary>
    /// Handles a mouse move event by updating the latest point defining a zoom by rectangle.
    /// </summary>
    /// <param name="p">The updated position</param>
    /// <param name="b">The mouse button that was clicked (ignored).</param>
    public override void MouseMove(IPosition p, MouseButton b)
    {
        if (_anchor is null)
            return;

        var p1 = MapWindow.WorldToScreen(_anchor);
        var p2 = MapWindow.WorldToScreen(p);

        var top = Math.Min(p1.Y, p2.Y);
        var left = Math.Min(p1.X, p2.X);
        var bottom = Math.Max(p1.Y, p2.Y);
        var right = Math.Max(p1.X, p2.X);

        _screenRect.Width = right - left;
        _screenRect.Height = bottom - top;
        _screenRect.RenderTransform = new TranslateTransform(left, top);

        _screenRect.InvalidateVisual();
    }

    /// <summary>
    /// Finishes off a zoom by rectangle
    /// </summary>
    public override void MouseUp(IPosition p, MouseButton b)
    {
        if (_anchor is not null)
            MapWindow.ZoomTo(new Window(_anchor, p));
        
        Finish();
    }
}