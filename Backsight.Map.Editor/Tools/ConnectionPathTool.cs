using System;
using System.Diagnostics;
using Backsight.Map.Editor.Mapping;
using Backsight.Map.Editor.Windows;
using Backsight.Model;
using Backsight.Model.Operations;
using SkiaSharp;

namespace Backsight.Map.Editor.Tools;

internal class ConnectionPathTool : CommandTool
{
    private readonly PathWindow _dialog;

    internal void SetCurrentPath(PathInfo path)
    {
        _currentPath = path;
        RefreshMapDisplay();
    }

    //private PointFeature? StartPoint => _dialog.Model.Terminals.SelectedStart; 
    //private PointFeature? EndPoint => _dialog.Model.Terminals.SelectedEnd;
    private PathInfo? _currentPath;

    internal ConnectionPathTool(MapEditorViewModel viewModel, Operation? recall)
        : base(viewModel, EditingActionId.Path)
    {
        _dialog = new PathWindow(this, recall as PathOperation);
    }

    internal override bool Run()
    {
        ViewModel.Show(_dialog);
        return true;
    }

    protected override bool Finish()
    {
        var model = _dialog.Model;
        var start = model.Start.SelectedPoint;
        var end = model.End.SelectedPoint;
        var entryString = model.Entry.Text;
        var unitType = ViewModel.Settings?.EntryUnit ?? DistanceUnitType.Meters;
        var defaultUnits = DistanceUnit.GetUnit(unitType);
        
        Debug.Assert(start is not null);
        Debug.Assert(end is not null);
        Debug.Assert(!string.IsNullOrWhiteSpace(entryString));

        var op = new PathOperation(start, end, entryString, defaultUnits);
        op.Execute();

        return base.Finish();
    }

    internal override void Render(MapCanvas canvas)
    {
        RenderCurrentPath(canvas);
        
        var model = _dialog.Model;

        var pointStyle = new PaintStyle
        {
            Color = SKColors.Aqua,
            Style = SKPaintStyle.Fill,
        };

        var startPoint = model.Start.SelectedPoint ?? model.Start.PickingPoint;
        var endPoint = model.End.SelectedPoint ?? model.End.PickingPoint;

        if (startPoint is not null)
            canvas.DrawPoint(startPoint, pointStyle with { Color = SKColors.Blue });

        if (endPoint is not null)
            canvas.DrawPoint(endPoint, pointStyle with { Color = SKColors.Aqua });
    }

    private void RenderCurrentPath(MapCanvas canvas)
    {
        if (_currentPath is null)
            return;
        
        // Do nothing if the scale factor is undefined (could be zero if the path terminals coincide)
        double sfac = _currentPath.ScaleFactor;
        if (Math.Abs(sfac) < MathConstants.TINY)
            return;
        
        // Initialize position to the start of the path.
        IPosition gotend = new Position(_currentPath.FromPoint);

        // Initial bearing is whatever the rotation is.
        double bearing = _currentPath.RotationInRadians;

        foreach (var leg in _currentPath.Legs)
        {
            // Include any angle specified at the start of the leg
            if (leg is StraightLeg sLeg)
                bearing = sLeg.AddStartAngle(bearing);

            // Determine exit bearing for circular leg (do it now, in case an extra leg complicates matters below)
            double exitBearing = bearing;
            if (leg is CircularLeg cLeg)
                exitBearing = cLeg.GetExitBearing(gotend, bearing, sfac);

            // Obtain geometry for each span and draw
            SpanInfo[] spans = leg.PrimaryFace.Spans;
            ILineGeometry[] sections = leg.GetSpanSections(gotend, bearing, sfac, spans);
            DrawSpans(canvas, spans, sections);
            
            /*
             This was formerly part of the old PathInfo class... should not be relevant
             for new connection paths
             
            // If we're dealing with the first face of a staggered leg, process the second face
            if (leg.AlternateFace != null)
            {
                spans = leg.AlternateFace.Spans;
                sections = leg.GetSpanSections(gotend, bearing, m_ScaleFactor, spans);
                DrawSpans(display, spans, sections);
            }
             */

            // Get to the end of the leg
            gotend = sections[^1].End;
            bearing = exitBearing;
        }
    }

    /// <summary>
    /// Renders the geometry for the spans along a leg.
    /// </summary>
    /// <param name="canvas">The display to draw to</param>
    /// <param name="spans">Information about each observed span</param>
    /// <param name="sections">The geometry that corresponds to each span</param>
    private void DrawSpans(MapCanvas canvas, SpanInfo[] spans, ILineGeometry[] sections)
    {
        Debug.Assert(spans.Length == sections.Length);

        var pointStyle = new PaintStyle
        {
            Color = SKColors.Magenta,
            Style = SKPaintStyle.Fill
        };
        var lineStyle = new PaintStyle
        {
            Color = SKColors.Magenta,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2
        };
        var dottedStyle = lineStyle with { Dashed = true };
        
        for (int i = 0; i < spans.Length; i++)
        {
            var style = spans[i].HasLine ? lineStyle : dottedStyle;

            ILineGeometry geom = sections[i];
            
            if (geom is ICircularArcGeometry arc)
                canvas.DrawArc(arc, style);
            else
                canvas.DrawLine(geom.Start, geom.End, style);

            if (spans[i].HasEndPoint)
                canvas.DrawPoint(geom.End, pointStyle);
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