using System;
using Avalonia.Media;
using Backsight.Geometry;
using Backsight.Map.Editor.Mapping;
using Backsight.Map.Editor.Windows;
using Backsight.Model;
using Backsight.Model.Observations;
using SkiaSharp;

namespace Backsight.Map.Editor.Tools;

internal class LineExtensionTool : CommandTool
{
    private readonly LineExtensionWindow _dialog;

    internal LineExtensionTool(MapEditorViewModel viewModel, LineFeature extendLine)
        : base(viewModel, EditingActionId.LineExtend)
    {
        _dialog = new LineExtensionWindow(this, extendLine);
    }

    internal override bool Run()
    {
        ViewModel.Show(_dialog);
        return false;
    }

    protected override bool Finish()
    {
        Console.WriteLine("Finishing LineExtensionTool with extension: " + _dialog.ViewModel.Length);
        return base.Finish();
    }

    internal override void Render(MapCanvas canvas)
    {
        var model = _dialog.ViewModel;

        var pointStyle = new PaintStyle
        {
            Color = SKColors.Magenta,
            Style = SKPaintStyle.Fill,
        };
        
        // Fill the from-point
        var from = model.IsExtendFromEnd ? model.Line.EndPoint : model.Line.StartPoint;
        canvas.DrawPoint(from, pointStyle);
        
        var geom = GetExtension();
        if (geom is null)
            return;

        // Draw a filled point at the end of the extension
        canvas.DrawPoint(geom.End, pointStyle);
        
        // Draw the extension (dashed if the user doesn't want to add a line)
        var lineStyle = new PaintStyle
        {
            Color = SKColors.Magenta,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2f,
            Dashed = !model.WantLine
        };
        canvas.DrawLine(geom, lineStyle);
    }

    private ILineGeometry? GetExtension()
    {
        var model = _dialog.ViewModel;

        if (model.Length is null)
            return null;
        
        var unitType = Store.Settings.EntryUnit;
        var entryUnit = DistanceUnit.GetUnit(unitType);
        var length = new Distance(Decimal.ToDouble(model.Length.Value), entryUnit);
        
        if (model.Line is ArcFeature arc)
        {
            arc.CalculateExtension(model.IsExtendFromEnd,
                length,
                out IPosition? start,
                out IPosition? end,
                out IPosition? center,
                out bool iscw);

            if (start is not null && end is not null && center is not null)
                return new CircularArcGeometry(PointGeometry.Create(center), start, end, iscw);
        }
        else
        {
            model.Line.CalculateExtension(model.IsExtendFromEnd,
                length,
                out IPosition? start,
                out IPosition? end);

            if (start is not null && end is not null)
                return new LineSegmentGeometry(start, end);
        }

        return null;
    }
    
    public override void Dispose()
    {
        // TODO: Not used?
        ViewModel.OverlayChildren.Clear();
        base.Dispose();
    }
}