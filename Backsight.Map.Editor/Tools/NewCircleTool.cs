using System;
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