using System;
using System.Diagnostics;
using Backsight.Environment;
using Backsight.Map.Editor.Mapping;
using Backsight.Map.Editor.Models;
using Backsight.Map.Editor.Windows;
using Backsight.Model;
using Backsight.Model.Observations;
using Backsight.Model.Operations;
using SkiaSharp;
using Direction = Backsight.Model.Observations.Direction;

namespace Backsight.Map.Editor.Tools;

internal class RadialTool : CommandTool
{
    private readonly RadialWindow _dialog;
    
    internal RadialTool(MapEditorViewModel viewModel, PointFeature fromPoint)
        : base(viewModel, EditingActionId.Radial)
    {
        _dialog = new RadialWindow(this, fromPoint);
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
        UpdateUI? up = this.Update;

        if (up is not null)
        {
            RadialOperation pop = (up.GetOp() as RadialOperation);
            if (pop==null)
            {
                MessageBox.Show("RadialUI.DialFinish - Unexpected edit type.");
                return false;
            }

            // Get info from the dialog.
            Direction dir = m_Dialog.Direction;
            Observation len = m_Dialog.Length;

            // The direction and length must both be defined.
            if (dir==null || len==null)
            {
                MessageBox.Show("Missing parameters for sideshot update.");
                return false;
            }

            // Remember the changes as part of the UI object (the original edit remains
            // unchanged for now)
            UpdateItemCollection changes = pop.GetUpdateItems(dir, len);
            if (!up.AddUpdate(pop, changes))
                return false;
        }
        else
        {
        */

        var model = _dialog.ViewModel;
        var dir = GetDirection();
        var len = model.GetLengthObservation();

        if (dir is null || len is null)
            return false;
        
        var op = new RadialOperation(dir, len);

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
            Color = SKColors.Aqua,
            Style = SKPaintStyle.Fill,
        };

        // Fill the from-point
        canvas.DrawPoint(model.From, pointStyle);
        
        // Fill the backsight (if there is one)
        var backsight = model.SelectedBacksight ?? model.PickingBacksight;
        if (backsight is not null)
            canvas.DrawPoint(backsight, pointStyle with { Color = SKColors.Blue });

        // Fill any parallel points
        if (model.Parallel1 is not null)
            canvas.DrawPoint(model.Parallel1, pointStyle with { Color = SKColors.DeepPink });
        if (model.Parallel2 is not null)
            canvas.DrawPoint(model.Parallel2, pointStyle with { Color = SKColors.DeepPink });
        if (model.PickingParallel is not null)
            canvas.DrawPoint(model.PickingParallel, pointStyle with { Color = SKColors.DeepPink });

        // Length defined by offset point
        var lengthOffset = model.LengthOffset ?? model.PickingOffset;
        if (lengthOffset is not null)
            canvas.DrawPoint(lengthOffset, pointStyle with { Color = SKColors.LightGreen });
        /*

        if (m_Offset!=null)
            DrawIfDefined(m_Offset.Point, view, style, Color.Gray);
            */
        
        var dir = GetDirection();
        if (dir is not null)
        {
            IPosition from = dir.StartPosition;

            var lineStyle = new PaintStyle
            {
                Color = SKColors.Magenta,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2
            };

            var len = model.GetLengthObservation();
            if (len is null)
            {
                IPosition to = BasicGeom.Polar(from, dir.Bearing.Radians, canvas.GetDiagonalLength());
                canvas.DrawLine(from, to, lineStyle with { Dashed = true });
            }
            else
            {
                IPosition to = RadialOperation.Calculate(dir, len);
                canvas.DrawLine(from, to, lineStyle with { Dashed = !model.WantLine });
                canvas.DrawPoint(to, pointStyle with { Color = SKColors.Magenta });
            }
        }
    }

    internal override bool MouseDown(IPosition p, MouseButton b)
    {
        return _dialog.ViewModel.PickPoint(p);
    }

    internal override void MouseMove(IPosition p, MouseButton b)
    {
        _dialog.ViewModel.MouseMove(p);
    }

    private Direction? GetDirection()
    {
        var model = _dialog.ViewModel;

        if (model.IsParallel)
        {
            var p1 = model.Parallel1;
            var p2 = model.Parallel2;

            if (p1 is null || p2 is null)
                return null;
            
            return new ParallelDirection(model.From, p1, p2);
        }

        if (!TryParseAngle(out var angleInRadians))
            return null;
        
        if (angleInRadians > 0.0 && !model.IsClockwise)
            angleInRadians = -angleInRadians;

        var angle = new RadianValue(angleInRadians);
        var backsight = model.SelectedBacksight;
        
        if (backsight is null)
        {
            // No backsight, so we have a bearing
            return new BearingDirection(model.From, angle);
        }
        
        // It could be either have a regular angle or a deflection.
        if (IsDeflectionAngle())
            return new DeflectionDirection(backsight, model.From, angle);

        return new AngleDirection(backsight, model.From, angle);
    }
    
    private bool IsDeflectionAngle()
    {
        var angle = _dialog.ViewModel.AngleText;
        return angle is not null && angle.ToUpper().Contains("D");
    }
    
    /// <summary>
    /// Parses an explicitly entered angle. 
    /// </summary>
    /// <param name="angleInRadians">The angle in radians (zero when no result).</param>
    /// <returns>True if direction parses ok.</returns>
    private bool TryParseAngle(out double angleInRadians)
    {
        angleInRadians = 0.0;
        
        // Get the entered string.
        var angle = _dialog.ViewModel.AngleText?.Trim();
        if (String.IsNullOrEmpty(angle))
            return false;
        
        // No angle if selecting parallel points and nothing has been specified
        if (angle.StartsWith("..."))
            return false;
        
        // No angle if selecting parallel points (assumes convention of using a + character as
        // a prefix for point IDs)
        if (angle.StartsWith("+"))
            return false;

        // Strip out any "D" (indicating a deflection)
        int dindex = angle.IndexOf('D', StringComparison.InvariantCultureIgnoreCase);
        if (dindex >= 0)
            angle = angle.Remove(dindex, 1);

        // Validate entered angle.
        if (!RadianValue.TryParse(angle, out angleInRadians))
            return false;

        return true;
    }
}