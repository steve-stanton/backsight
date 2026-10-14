using System;
using Backsight.Geometry;
using Backsight.Map.Editor.Mapping;
using Backsight.Map.Editor.Windows;
using Backsight.Model;
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
        Console.WriteLine("finish");
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