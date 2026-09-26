using System;
using Avalonia.Media;
using Backsight.Environment;
using Backsight.Geometry;
using Backsight.Map.Editor.Mapping;
using Backsight.Map.Editor.Models;
using Backsight.Map.Editor.Windows;
using Backsight.Model;
using Backsight.Model.Observations;
using Backsight.Model.Operations;
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
/*
        // If we are doing an update, alter the original operation.
        UpdateUI? up = this.Update;

        if (up is not null)
        {
            // Get the original operation.
            LineExtensionOperation pop = (up.GetOp() as LineExtensionOperation);
            if (pop==null)
            {
                MessageBox.Show("LineExtensionUI.DialFinish - Unexpected edit type.");
                return false;
            }

            // Remember the changes as part of the UI object (the original edit remains
            // unchanged for now)
            UpdateItemCollection changes = pop.GetUpdateItems(m_Dialog.IsExtendFromEnd, m_Dialog.Length);
            if (!up.AddUpdate(pop, changes))
                return false;
        }
        else
        {
            // Get info from the dialog
            m_IsExtendFromEnd = m_Dialog.IsExtendFromEnd;
            m_Length = m_Dialog.Length;
            IdHandle idh = m_Dialog.PointId;
            CadastralMapModel map = CadastralMapModel.Current;
            m_LineType = (m_Dialog.WantLine ? map.DefaultLineType : null);

            // Execute the edit
            LineExtensionOperation op = null;

            try
            {
                op = new LineExtensionOperation(m_ExtendLine, m_IsExtendFromEnd, m_Length);
                op.Execute(idh, m_LineType);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace, ex.Message);
                return false;
            }
        }
*/
        var model = _dialog.ViewModel;
        var distance = ToDistance(model.Length);

        if (distance is null)
            return false;
        
        var op = new LineExtensionOperation(
            model.Line,
            model.IsExtendFromEnd,
            distance);

        IEntity? lineType = model.WantLine ? Store.DefaultLineType : null;
        IEntity pointType = model.SelectedPointType;
        var idh = new IdHandle(WorkingSession);

        try
        {
            DisplayId? id = model.SelectedPointId;
            if (id is not null)
                idh.ReserveId(id.Packet, pointType, id.RawId);
                
            op.Execute(idh, pointType, lineType);
        }
        catch
        {
            idh.DiscardReservedId();
            throw;
        }
  
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

    private Distance? ToDistance(decimal? length)
    {
        if (length is null)
            return null;
        
        var unitType = Store.Settings.EntryUnit;
        var entryUnit = DistanceUnit.GetUnit(unitType);
        return new Distance(Decimal.ToDouble(length.Value), entryUnit);
    }
    
    private ILineGeometry? GetExtension()
    {
        var model = _dialog.ViewModel;

        var length = ToDistance(model.Length);
        if (length is null)
            return null;
        
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