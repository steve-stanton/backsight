using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Backsight.Map.Editor.Mapping;
using Backsight.Map.Editor.Windows;
using Backsight.Model;
using Backsight.Model.Operations;
using SkiaSharp;

namespace Backsight.Map.Editor.Tools;

internal class NewLineTool : CommandTool
{
    private readonly Avalonia.Controls.Shapes.Line _line;

    /// <summary>
    /// The point at the start of the new line.
    /// </summary>
    private PointFeature? _start;

    /// <summary>
    /// The point the mouse is currently close to (may end up being either the start or
    /// the end of the new line).
    /// </summary>
    private PointFeature? _currentPoint;

    /// <summary>
    /// The last mouse position
    /// </summary>
    private IPointGeometry? _end;

    /// <summary>
    /// Should intersections be shown?
    /// </summary>
    private readonly bool _showIntersections;

    internal NewLineTool(MapEditorViewModel viewModel, PointFeature? start)
        : base(viewModel, EditingActionId.NewLine)
    {
        _start = start;
        _currentPoint = start;

        _line = new Avalonia.Controls.Shapes.Line
        {
            Stroke = Brushes.Magenta,
            StrokeThickness = 2,
        };

        _showIntersections =
            viewModel.ArePointsDrawn &&
            viewModel.Settings?.IntersectionsDrawn == true && 
            Store.DefaultLineType.IsPolygonBoundaryValid;
    }

    private protected PointFeature? StartPoint => _start;

    /// <summary>
    /// The last mouse position (may or may not correspond to a point feature).
    /// </summary>
    private protected IPointGeometry? LastMousePosition
    {
        get => _end;
        set => _end = value;
    }

    internal override bool Run()
    {
        ViewModel.OverlayChildren.Add(_line);
        
        // Ensure any initial selection has been cleared (if the user clicks in space
        // to cancel the line-add command, the selection needs to be clear... really?)
        //ViewModel.ClearSelection();

        ViewModel.MapCursor = EditingCursors.PenCursor;
        return true;
    }

    internal override void MouseMove(IPosition p, MouseButton b)
    {
        base.MouseMove(p, b);
        
        // Try to find a point at the current position
        ILength size = new Length(Store.Settings.PointHeight * 0.5);
        var pt  = Store.Model.QueryClosest(p, size, SpatialType.Point) as PointFeature;

        if (pt is null)
        {
            _end = PointGeometry.Create(p);
            _currentPoint = null;
        }
        else
        {
            _end = pt;
            _currentPoint = IsPointValid(pt) ? pt : null;
        }

        if (_start is not null && _end.IsCoincident(_start))
        {
            _end = null;
            return;
        }
        
        var geom = GetIntersectGeometry();
        if (geom is null)
            return;
        
        UpdateDisplay(geom);

        if (_showIntersections)
        {
            var xf = new IntersectionFinder(Store.Model.Index, geom, false);
            var points = new List<Rectangle>();

            foreach (var x in xf.Intersections.SelectMany(x => x.Intersections))
            {
                if (x.P1 is not null)
                    points.Add(CreateRectangle(x.P1));
            }

            var oldPoints = ViewModel.OverlayChildren.OfType<Rectangle>();
            ViewModel.OverlayChildren.RemoveAll(oldPoints);
            ViewModel.OverlayChildren.AddRange(points);
        }
    }

    /// <summary>
    /// Checks whether a selected point is valid for the current line.
    /// </summary>
    /// <param name="p"></param>
    /// <returns>True if the specified point can be used as a line endpoint.</returns>
    private protected virtual bool IsPointValid(PointFeature p) => _start is null || !ReferenceEquals(p, _start);

    private protected virtual void UpdateDisplay(Model.LineGeometry geom)
    {
        var (p1x, p1y) = ViewModel.WorldToScreen(geom.Start);
        var (p2x, p2y) = ViewModel.WorldToScreen(geom.End);

        _line.StartPoint = new Avalonia.Point(p1x, p1y);
        _line.EndPoint = new Avalonia.Point(p2x, p2y);
        _line.InvalidateVisual();
    }

    internal override bool MouseDown(IPosition p, MouseButton b)
    {
        // Cancel the new line if there is no point selected.
        if (_currentPoint is null)
        {
            Abort();
            return true;
        }

        // If we don't have the first point yet, remember the start location.
        // Otherwise remember the end point & add the line.
        AppendToLine(_currentPoint);
        
        return true;
    }

    /// <summary>
    /// Appends a point to the new line. If it's the second point, the new line
    /// will be added to the map.
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    /// <remarks>The NewArcTool should override</remarks>
    protected virtual bool AppendToLine(PointFeature p)
    {
        // If the start point is not defined, just remember it.
        if (_start is null)
        {
            _start = p;
            return true;
        }

        // Confirm the point is different from the start.
        if (p.IsCoincident(_start))
        {
            //MessageBox.Show("End point cannot match the start point.");
            return false;
        }

        // Add the new line.
        AddNewLine(p);

        Finish();
        return true;
    }

    /// <summary>
    /// Adds a new line segment feature.
    /// </summary>
    /// <param name="end"></param>
    private protected virtual void AddNewLine(PointFeature end)
    {
        if (_start is null)
            throw new InvalidOperationException("Start point not defined.");
        
        var op = new NewSegmentOperation(Store);
        op.Execute(_start, end);
    }
    
    /// <summary>
    /// Geometry that can be used to detect intersections with the map
    /// </summary>
    /// <returns>The geometry for the new line (null if insufficient information has been specified)</returns>
    private protected virtual Model.LineGeometry? GetIntersectGeometry()
    {
        if (_start is null || _end is null)
            return null;

        ITerminal endTerm = new FloatingTerminal(_end);
        return new SegmentGeometry(_start, endTerm);
    }

    private Rectangle CreateRectangle(IPosition p)
    {
        var offset = ViewModel.Model.Store.Settings.PointHeight * 0.5;
        var topLeft = new Position(p.X - offset, p.Y + offset);
        var bottomRight = new Position(p.X + offset, p.Y - offset);
        
        var (left, top) = ViewModel.WorldToScreen(topLeft);
        var (right, bottom) = ViewModel.WorldToScreen(bottomRight);

        return new Rectangle
        {
            Width = right - left,
            Height = bottom - top,
            Fill = Brushes.Transparent,
            Stroke = Brushes.Magenta,
            StrokeThickness = 2.0,
            RenderTransform = new TranslateTransform(left, top),
        };
    }

    internal override void Render(MapCanvas canvas)
    {
        if (_start is not null)
        {
            canvas.DrawPoint(_start, new PaintStyle
            {
                Color = SKColors.Blue,
                Style = SKPaintStyle.Fill
            });
        }

        if (_currentPoint is not null)
        {
            canvas.DrawPoint(_currentPoint, new PaintStyle
            {
                Color = SKColors.Aqua,
                Style = SKPaintStyle.Fill
            });
        }
    }
    
    public override void Dispose()
    {
        ViewModel.OverlayChildren.Clear();
        base.Dispose();
    }
}