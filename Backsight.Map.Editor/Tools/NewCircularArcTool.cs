using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Media;
using Backsight.Geometry;
using Backsight.Map.Editor.Mapping;
using Backsight.Map.Editor.Windows;
using Backsight.Model;
using Backsight.Model.Operations;
using SkiaSharp;
using LineGeometry = Backsight.Model.LineGeometry;

namespace Backsight.Map.Editor.Tools;

internal class NewCircularArcTool : NewLineTool
{
    private readonly Avalonia.Controls.Shapes.Arc _arc;

    /// <summary>
    /// The search tolerance for finding circles.
    /// </summary>
    private ILength CircleTolerance { get; } = new Length(0.001);

    /// <summary>
    /// The circles incident on the start point.
    /// </summary>
    private readonly IReadOnlyCollection<Circle> _circles;

    /// <summary>
    /// True if user wants the short arc.
    /// </summary>
    private bool _isShortArc = true;

    /// <summary>
    /// The circle the new arc should coincide with
    /// </summary>
    Circle? _newArcCircle;

    /// <summary>
    /// The current arc geometry. This gets defined by the mouse move handler, used
    /// for draws, and for calculating intersect positions.
    /// </summary>
    ArcGeometry? _geom;

    internal NewCircularArcTool(MapEditorViewModel viewModel, PointFeature start, IReadOnlyCollection<Circle> circles)
        : base(viewModel, start)
    {
        if (circles.Count == 0)
            throw new ArgumentException("No circles incident on start point");

        _circles = circles;
        _arc = new Avalonia.Controls.Shapes.Arc
        {
            Stroke = Brushes.Magenta,
            StrokeThickness = 3,
        };
    }

    internal override bool Run()
    {
        Debug.Assert(StartPoint is not null);
        ViewModel.OverlayChildren.Add(_arc);
        return base.Run();
    }

    /// <inheritdoc/>
    /// <remarks>
    /// This override confirms that the point is not the start point,
    /// and that it is coincident with one of the circles passing through the start point.
    /// </remarks>
    private protected override bool IsPointValid(PointFeature p)
    {
        if (ReferenceEquals(p, StartPoint))
            return false;
        
        // Confirm the point is coincident with one of the circles
        // TODO: Should a tolerance even be allowed?
        return _circles.Any(c => c.Distance(p).Meters < CircleTolerance.Meters);
    }

    internal override void Render(MapCanvas canvas)
    {
        var circleStyle = new PaintStyle
        {
            Color = SKColors.Magenta,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2f,
            Dashed = true
        };

        foreach (var circle in _circles)
            canvas.DrawCircle(circle, circleStyle);

        // Render the start point (and maybe the end point)
        base.Render(canvas);
    }

    /// <summary>
    /// Adds a new circular arc feature.
    /// </summary>
    /// <param name="end"></param>
    private protected override void AddNewLine(PointFeature end)
    {
        // It might be better to figure out the _newArcCircle at this point. As it is,
        // this assumes that the value has been defined via the last call to UpdateDisplay
        // (which should ensure that the circle used agrees with the map preview in a
        // situation where two or more circles are involved)
        Debug.Assert(_newArcCircle is not null);
        Debug.Assert(ViewModel.Store is not null);
        Debug.Assert(StartPoint is not null);

        var op = new NewArcOperation(ViewModel.Store);
        op.Execute(StartPoint, end, _newArcCircle, _isShortArc);
    }

    private protected override void UpdateDisplay(LineGeometry geom)
    {
        var arc = geom as ArcGeometry ?? throw new ArgumentException("Expected an arc geometry");

        // Remember the circle that the geometry is based on (it SHOULD be one of the circles
        // we know about, assuming that IsPointValid is ued as expected)
        _newArcCircle = _circles.FirstOrDefault(c => ReferenceEquals(c, arc.Circle));
        
        var extent = ToScreenRect(arc.Circle);

        // It seems you need to use attached properties to define the position of the arc
        _arc.SetValue(Canvas.LeftProperty, extent.Left);
        _arc.SetValue(Canvas.TopProperty, extent.Top);
        
        _arc.Height = extent.Height;
        _arc.Width = extent.Width;
        _arc.StartAngle = arc.StartBearingInRadians * MathConstants.RADTODEG - 90.0;
        _arc.SweepAngle = arc.SweepAngleInRadians * MathConstants.RADTODEG;
        _arc.InvalidateVisual();
    }

    private SKRect ToScreenRect(ICircleGeometry circle)
    {
        var cx = CircleGeometry.GetExtent(circle);
        var (left, bottom) = ViewModel.WorldToScreen(cx.Min);
        var (right, top) = ViewModel.WorldToScreen(cx.Max);
        return new SKRect((float)left, (float)top, (float)right, (float)bottom);
    }        

    /// <summary>
    /// Geometry that can be used to detect intersections with the map.
    /// </summary>
    /// <returns>The geometry for the new line (null if insufficient information has been specified)</returns>
    private protected override LineGeometry? GetIntersectGeometry()
    {
        Debug.Assert(StartPoint is not null);

        IPointGeometry? end = LastMousePosition;
        if (end is null)
            return null;

        // Find the circle that is closest to the end (only looking at the valid circles
        // that are within a 1mm tolerance). Return null geometry if none of the circles
        // are within reach.

        Circle bestCircle = _circles.First();
        double bestdist = bestCircle.Distance(end).Meters;

        foreach (var c in _circles.Skip(1))
        {
            double dist = c.Distance(end).Meters;
            if (dist < bestdist)
            {
                bestCircle = c;
                bestdist = dist;
            }
        }

        // The best circle may be quite distant from the mouse position. Continue with it
        // anyway so that map preview can at least display a projection onto that circle.

        // Project the end point ON to the circle.
        IPosition cirend = CircleGeometry.GetClosestPosition(bestCircle, end);

        // Get the clockwise angle from the start to the current end point.
        IPointGeometry center = bestCircle.Center;
        Turn sturn = new Turn(center, StartPoint);
        double angle = sturn.GetAngleInRadians(cirend);

        // Figure out which direction the curve should go, depending
        // on whether the user wants the short arc or the long one.
        bool iscw = angle < MathConstants.PI ? _isShortArc : !_isShortArc;

        // Create the required geometry
        ITerminal ec = new FloatingTerminal(cirend);
        var result = new ArcGeometry(bestCircle, StartPoint, ec, iscw);
        
        // It's possible the end position doesn't exactly match the end location
        // determined by base.MouseMove (this should ensure the arc renders correctly)
        LastMousePosition = result.EC;

        return result;
    }

    internal override IReadOnlyList<ContextMenuItem>? CreateContextMenu() =>
    [
        new("Short Arc", () => { _isShortArc = true; }, isChecked: _isShortArc),
        new("Long Arc", () => { _isShortArc = false; }, isChecked: !_isShortArc),
        ContextMenuItem.Separator,
        new("Cancel", Escape)
    ];
}