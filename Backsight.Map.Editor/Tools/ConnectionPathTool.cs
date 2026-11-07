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

    //internal PointFeature? StartPoint => _dialog.Model.Terminals.SelectedStart; 
    //internal PointFeature? EndPoint => _dialog.Model.Terminals.SelectedEnd;

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
        var model = _dialog.Model;
        var start = model.Terminals.SelectedStart;
        var end = model.Terminals.SelectedEnd;
        var entryString = model.Entry.Text;
        
        Console.WriteLine(entryString);

        return base.Finish();
    }

    internal override void Render(MapCanvas canvas)
    {
        var model = _dialog.Model;

        var pointStyle = new PaintStyle
        {
            Color = SKColors.Aqua,
            Style = SKPaintStyle.Fill,
        };

        var startPoint = model.Terminals.SelectedStart ?? model.Terminals.PickingStart;
        var endPoint = model.Terminals.SelectedEnd ?? model.Terminals.PickingEnd;

        if (startPoint is not null)
            canvas.DrawPoint(startPoint, pointStyle with { Color = SKColors.Blue });

        if (endPoint is not null)
            canvas.DrawPoint(endPoint, pointStyle with { Color = SKColors.Aqua });
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