using System;
using System.Diagnostics;
using Backsight.Map.Editor.Mapping;
using Backsight.Map.Editor.Windows;
using Backsight.Model;
using SkiaSharp;

namespace Backsight.Map.Editor.Tools;

internal class ConnectionPathTool : CommandTool
{
    private readonly PathTerminalsWindow _terminals;
    private PathWindow? _path;

    internal PointFeature? StartPoint => _terminals.ViewModel.SelectedStart; 
    internal PointFeature? EndPoint => _terminals.ViewModel.SelectedEnd;

    internal ConnectionPathTool(MapEditorViewModel viewModel)
        : base(viewModel, EditingActionId.Path)
    {
        _terminals = new PathTerminalsWindow(this);
    }

    internal override bool Run()
    {
        // Display the path terminals window as a modeless dialog
        ViewModel.Show(_terminals);
        return true;
    }

    protected override bool Finish()
    {
        /*
        if (StartPoint is null)
        {
            var model = _terminals.ViewModel;
            StartPoint = model.SelectedStart;
            EndPoint = model.SelectedEnd;
            
            Debug.Assert(StartPoint is not null);
            Debug.Assert(EndPoint is not null);
            
            // Display the path data entry screen as a modal dialog... but needs to be async
            _path = new PathWindow(this);
            ViewModel.Show(_path);
        }
        else
        {
            
        }
        */
        Console.WriteLine("Path connection finished");
        return base.Finish();
    }

    internal override void Render(MapCanvas canvas)
    {
        var model = _terminals.ViewModel;

        var pointStyle = new PaintStyle
        {
            Color = SKColors.Aqua,
            Style = SKPaintStyle.Fill,
        };

        var startPoint = model.SelectedStart ?? model.PickingStart;
        var endPoint = model.SelectedEnd ?? model.PickingEnd;

        if (startPoint is not null)
            canvas.DrawPoint(startPoint, pointStyle with { Color = SKColors.Blue });

        if (endPoint is not null)
            canvas.DrawPoint(endPoint, pointStyle with { Color = SKColors.Aqua });
    }

    internal override bool MouseDown(IPosition p, MouseButton b)
    {
        return _terminals.ViewModel.PickPoint(p);
    }

    internal override void MouseMove(IPosition p, MouseButton b)
    {
        _terminals.ViewModel.MouseMove(p);
    }
}