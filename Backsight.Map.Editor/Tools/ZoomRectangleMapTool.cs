using System;
using Avalonia.Media;
using Backsight.Map.Editor.Windows;
using Rectangle = Avalonia.Controls.Shapes.Rectangle;

namespace Backsight.Map.Editor.Tools;

internal class ZoomRectangleMapTool : MapDisplayTool
{
    /// <summary>
    /// The anchor position  of the rectangle
    /// </summary>
    IPosition? _anchor;

    /// <summary>
    /// Rectangle corresponding to the current corners, in screen units.
    /// </summary>
    readonly Rectangle _screenRect;

    internal ZoomRectangleMapTool(MapEditorViewModel viewModel)
        : base(viewModel)
    {
        _screenRect = new Rectangle
        {
            Stroke = Brushes.Magenta,
            StrokeThickness = 2,
            StrokeDashArray = [2, 4]
        };
    }

    //public override int Id => (int)DisplayToolId.ZoomRectangle;

    internal override bool Start()
    {
        ViewModel.MapCursor = EditingCursors.ZoomRectangleCursor;
        ViewModel.OverlayChildren.Add(_screenRect);
        return true;
    }
    
    private protected override bool Finish()
    {
        ViewModel.OverlayChildren.Remove(_screenRect);
        return base.Finish();
    }
    
    internal override void Escape()
    {
        ViewModel.OverlayChildren.Remove(_screenRect);
        base.Escape();
    }

    /// <summary>
    /// Handles a left click on the map by remembering the anchor point for a zoom by rectangle.
    /// </summary>
    /// <param name="p">The position where the left click occurred.</param>
    /// <param name="b">The mouse button that was clicked (ignored).</param>
    internal override void MouseDown(IPosition p, MouseButton b)
    {
        _anchor = p;
    }

    /// <summary>
    /// Handles a mouse move event by updating the latest point defining a zoom by rectangle.
    /// </summary>
    /// <param name="p">The updated position</param>
    /// <param name="b">The mouse button that was clicked (ignored).</param>
    internal override void MouseMove(IPosition p, MouseButton b)
    {
        if (_anchor is null)
            return;

        var p1 = ViewModel.WorldToScreen(_anchor);
        var p2 = ViewModel.WorldToScreen(p);

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
    internal override void MouseUp(IPosition p, MouseButton b)
    {
        if (_anchor is not null)
            ViewModel.ZoomTo(new Window(_anchor, p));
        
        Finish();
    }
}