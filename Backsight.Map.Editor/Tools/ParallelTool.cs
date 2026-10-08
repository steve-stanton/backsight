using System;
using System.Diagnostics;
using Backsight.Geometry;
using Backsight.Map.Editor.Mapping;
using Backsight.Map.Editor.Windows;
using Backsight.Model;
using Backsight.Model.Operations;
using SkiaSharp;

namespace Backsight.Map.Editor.Tools;

internal class ParallelTool : CommandTool
{
    private readonly ParallelWindow _dialog;

    /// <summary>
    /// The reference line for the parallel.
    /// </summary>
    private readonly LineFeature _referenceLine;
    
    internal ParallelTool(MapEditorViewModel viewModel, LineFeature referenceLine)
        : base(viewModel, EditingActionId.Parallel)
    {
        _referenceLine = referenceLine;
        _dialog = new ParallelWindow(this, referenceLine);
    }

    internal override bool Run()
    {
        ViewModel.Show(_dialog);
        return true;
    }

    protected override bool Finish()
    {
        /*
        // If we are doing an update, remember the changes
        if (up!=null)
        {
            // Get the original operation.
            ParallelLineOperation op = (ParallelLineOperation)up.GetOp();
            if (op==null)
                throw new Exception("ParallelLineUI.DialFinish - Unexpected edit type.");

            // Note the offset (it SHOULD be defined)
            Observation offset = null;

            if (m_Offset != null)
                offset = m_Offset;
            else if (m_OffsetPoint != null)
                offset = new OffsetPoint(m_OffsetPoint);

            Debug.Assert(offset != null);

            // Remember the changes as part of the UI object (the original edit remains
            // unchanged for now)

            UpdateItemCollection changes = op.GetUpdateItems(m_Line, offset, m_TermLine1, m_TermLine2, m_IsReversed);
            if (!up.AddUpdate(op, changes))
                return false;
        }
        else
        {
        */
        
        var model = _dialog.Model;
        var offset = model.Offset.GetOffsetObservation();
        var terminal1 = model.Terminal1.SelectedLine;
        var terminal2 = model.Terminal2.SelectedLine;
        var isArcReversed = model.Offset.UsingOtherArc;
        
        Debug.Assert(offset is not null);
        
        var op = new ParallelLineOperation(_referenceLine, offset, terminal1, terminal2, isArcReversed);
        op.Execute();

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

        // Draw the parallel line
        IPosition? start = model.Terminal1.Position ?? model.Offset.StartPosition;
        IPosition? end = model.Terminal2.Position ?? model.Offset.EndPosition;
        RenderParallel(canvas, start, end);

        // Draw the start and end points too
        if (start is not null)
            canvas.DrawPoint(start, pointStyle);
        
        if (end is not null)
            canvas.DrawPoint(end, pointStyle);
        
        // Draw any offset point specified via the offset page
        var pt = model.Offset.OffsetPoint ?? model.Offset.PickingOffsetPoint;
        if (pt is not null)
            canvas.DrawPoint(pt, pointStyle with { Color = SKColors.LightGreen });
        
        if (model.Terminal1.SelectedLine is not null)
            canvas.Highlight(model.Terminal1.SelectedLine);

        if (model.Terminal2.SelectedLine is not null)
            canvas.Highlight(model.Terminal2.SelectedLine);
        
        if (model.CurrentTerminalPosition is not null)
            canvas.DrawPoint(model.CurrentTerminalPosition, pointStyle with { Color = SKColors.Yellow });
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
            if (_dialog.Model.Offset.UsingOtherArc)
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

        return true;
    }

    internal override void MouseMove(IPosition p, MouseButton b)
    {
        _dialog.Model.MouseMove(p);
    }
}