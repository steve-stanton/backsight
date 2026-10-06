using System;
using Avalonia.Rendering.Composition.Animations;
using Backsight.Geometry;
using Backsight.Map.Editor.Mapping;
using Backsight.Map.Editor.Windows;
using Backsight.Model;
using SkiaSharp;

namespace Backsight.Map.Editor.Tools;

internal class ParallelTool : CommandTool
{
    private readonly ParallelWindow _dialog;

    /// <summary>
    /// The reference line for the parallel.
    /// </summary>
    private readonly LineFeature _referenceLine;
    
    /// <summary>
    /// The position perpendicular to the reference line where the parallel starts (or the
    /// radial position if the reference line is a circular arc).
    /// </summary>
    private IPosition? _parallel1;

    /// <summary>
    /// The position perpendicular to the reference line where the parallel ends (or the
    /// radial position if the reference line is a circular arc).
    /// </summary>
    private IPosition? _parallel2;

    /// <summary>
    /// The position of the parallel where it intersects the first terminal line (if any).
    /// </summary>
    private IPosition? _terminal1;

    /// <summary>
    /// The position of the parallel where it intersects the second terminal line (if any).
    /// </summary>
    private IPosition? _terminal2;
    
    internal ParallelTool(MapEditorViewModel viewModel, LineFeature referenceLine)
        : base(viewModel, EditingActionId.Parallel)
    {
        _referenceLine = referenceLine;
        _dialog = new ParallelWindow(this, referenceLine);
    }

    internal override bool Run()
    {
        var model = _dialog.Model;
        Console.WriteLine("running with model type: " + model.GetType().Name);

        ViewModel.Show(_dialog);
        return true;
    }

    protected override bool Finish()
    {
        Console.WriteLine("finishing parallel");
        return base.Finish();
    }

    internal override void Render(MapCanvas canvas)
    {
        var model = _dialog.Model;

        var pointStyle = new PaintStyle
        {
            Color = SKColors.Magenta,
            Style = SKPaintStyle.Fill,
        };

        var lineStyle = new PaintStyle
        {
            Color = SKColors.Magenta,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 4
        };
        
        canvas.DrawLine(_referenceLine.LineGeometry, lineStyle with { Color = SKColors.Blue });

        // Figure out the positions for the ends of the parallel line (if any) ...

        // Assume we already know both terminals.
        IPosition? start = _terminal1;
        IPosition? end = _terminal2;

        // If they weren't actually defined, use the parallel points instead.
        if (start is null)
            start = _parallel1;

        if (end is null)
            end = _parallel2;

        RenderParallel(canvas, start, end);

        var pt = model.Offset.OffsetPoint ?? model.Offset.PickingOffsetPoint;
        if (pt is not null)
            canvas.DrawPoint(pt, pointStyle with { Color = SKColors.LightGreen });

        if (model.Terminal1.SelectedLine is not null)
            canvas.Highlight(model.Terminal1.SelectedLine);

        if (model.Terminal2.SelectedLine is not null)
            canvas.Highlight(model.Terminal2.SelectedLine);
    }

    private void RenderParallel(MapCanvas canvas, IPosition? start, IPosition? end)
    {
        if (start is null || end is null)
            return;

        var lineStyle = new PaintStyle(SKColors.Magenta, StrokeWidth: 4);

        ArcGeometry? arc = _referenceLine.GetArcBase()?.Geometry;
        if (arc is not null)
        {
            bool iscw = arc.IsClockwise;

            // Reverse the direction if necessary
            // TODO: Should UsingOtherArc be an option of the offset page?
            if (_dialog.Model.Terminal2.UsingOtherArc)
                iscw = !iscw;

            // Draw the parallel line (the rest of the circle being dotted).
            var radius = BasicGeom.Distance(arc.Circle.Center, start);
            var circle = new CircleGeometry(arc.Circle.Center, radius);
            var parArc = new CircularArcGeometry(circle, start, end, iscw);
            canvas.DrawArc(parArc, lineStyle);
            
            parArc.IsClockwise = !parArc.IsClockwise;
            canvas.DrawArc(parArc, lineStyle with { Dashed = true });
        }
        else
        {
            // Assume the parallel is straight...
            
            // What's the max length of a diagonal crossing the entire screen?
            double maxdiag = canvas.GetDiagonalLength();

            // What's the bearing from the start to the end of the parallel?
            double bearing = BasicGeom.BearingInRadians(start, end);

            // Project to a point before the start end of the parallel, as well as a point after the end.
            IPosition before = BasicGeom.Polar(start, bearing + MathConstants.PI, maxdiag);
            IPosition after = BasicGeom.Polar(end, bearing, maxdiag);

            canvas.DrawLine(before, start, lineStyle with { Dashed = true });
            canvas.DrawLine(start, end, lineStyle);
            canvas.DrawLine(end, after, lineStyle with { Dashed = true });
        }
    }

    internal override bool MouseDown(IPosition p, MouseButton b)
    {
        var selectionChanged = _dialog.Model.MouseDown(p);

        if (selectionChanged)
            ViewModel.RefreshMapDisplay();

        return selectionChanged;
    }

    internal override void MouseMove(IPosition p, MouseButton b)
    {
        _dialog.Model.MouseMove(p);
    }

    internal void OnOffsetChanged(Observation? offset)
    {
        // Attempt to calculate the parallel based on the offset alone
        if (offset is null)
        {
            _parallel1 = _parallel2 = null;
        }
        else
        {
            _referenceLine.CalculateParallel(offset, out var start, out var end);
            _parallel1 = start;
            _parallel2 = end;
        }

        // The ParallelTerminalViewModel currently nulls out the selected terminal line if the
        // user goes back to the offset page of the wizard. At that point, the two terminals
        // held here should become n/a. That being the case, ensure they are in fact undefined.
        
        _terminal1 = _terminal2 = null;

        ViewModel.RefreshMapDisplay();
    }

    /// <summary>
    /// Handles any change to a terminal line selected by the user.
    /// </summary>
    /// <param name="sender">The view model where the change has been made.</param>
    internal void OnTerminalChanged(ParallelTerminalViewModel sender)
    {
        var terminal = sender.SelectedLine;
        if (terminal is null)
        {
            if (ReferenceEquals(sender, _dialog.Model.Terminal1))
                _terminal1 = null;
            else
                _terminal2 = null;
        }
        else
        {
            // TODO: Attempt to calculate the terminal position
            
        }
        
        ViewModel.RefreshMapDisplay();
    }

}