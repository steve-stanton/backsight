using System;
using System.Diagnostics;
using Backsight.Geometry;
using Backsight.Map.Editor.Mapping;
using Backsight.Map.Editor.Windows;
using Backsight.Model;
using Backsight.Model.Observations;
using Backsight.Model.Operations;
using SkiaSharp;

namespace Backsight.Map.Editor.Tools;

internal class NewCircleTool : CommandTool
{
    private readonly NewCircleWindow _dialog;

    public NewCircleTool(MapEditorViewModel viewModel)
        : base(viewModel, EditingActionId.NewCircle)
    {
        _dialog = new NewCircleWindow(this);
    }

    internal override bool Run()
    {
        ViewModel.Show(_dialog);
        return true;
    }

    protected override bool Finish()
    {
        var model = _dialog.ViewModel;
        var center = model.SelectedCenter ?? throw new InvalidOperationException("Center not selected");
        var radius = model.SelectedRadius ?? throw new InvalidOperationException("Radius not selected");
        
        /*
        // If we are doing an update, alter the original operation.
        UpdateUI? pup = this.Update;

        if (pup is not null)
        {
            // Get the original operation.
            NewCircleOperation pop = (pup.GetOp() as NewCircleOperation);
            if (pop==null)
            {
                MessageBox.Show("NewCircleUI.DialFinish - Unexpected edit type.");
                return false;
            }

            pop.Correct(center, radius);
        }
        else
        {
        */
        
        // The old system allowed specification of radius via an entered distance,
        // but the distance had to be on the mapping plane. That was a bit of a hack
        // because conversion of a ground dimension depends on a scaling factor that
        // could potentially vary around the circumference of the circle. The new
        // system will only let you specify a radius via an offset point - the operation
        // will take an observed distance only for the sake of backward compatibility.
        
        var op = new NewCircleOperation(center, new OffsetPoint(radius));
        op.Execute();

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

        var center = model.SelectedCenter ?? model.PickingCenter;
        var radius = model.SelectedRadius ?? model.PickingRadius;
        
        if (center is not null)
            canvas.DrawPoint(center, pointStyle with { Color = SKColors.DarkGray });
        
        if (radius is not null)
            canvas.DrawPoint(radius, pointStyle with { Color = SKColors.LightGreen });

        if (center is not null && radius is not null)
        {
            var pc = center.Geometry;
            var pr = radius.Geometry;
            var circle = new CircleGeometry(pc, BasicGeom.Distance(pc, pr));
            var lineStyle = new PaintStyle
            {
                Color = SKColors.Magenta,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2,
                Dashed = true
            };
            
            canvas.DrawCircle(circle, lineStyle);
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
    
}