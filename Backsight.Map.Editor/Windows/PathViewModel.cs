using System;
using Avalonia.Media;
using Backsight.Map.Editor.Tools;
using Backsight.Model.Operations;

namespace Backsight.Map.Editor.Windows;

internal class PathViewModel : WizardViewModel
{
    internal ConnectionPathTool Tool { get; }
    internal PickPointViewModel Start { get; }
    internal PickPointViewModel End { get; }
    internal PathEntryViewModel Entry { get; }
    internal PathOperation? Recall { get; }

    internal PathViewModel(ConnectionPathTool tool, PathOperation? recall)
        : base(tool)
    {
        Tool = tool;
        Recall = recall;
        Start = new PickPointViewModel(Tool, Colors.Blue, "Pick the starting point from the map");
        End = new PickPointViewModel(Tool, Colors.Aqua, "Pick the finishing point from the map");
        Entry = new PathEntryViewModel(this, recall);
    }

    protected override bool ValidateCurrentPage()
    {
        return GetCurrentPageModel().ValidatePage();
    }

    private PageModel GetCurrentPageModel()
    {
        return CurrentPageIndex switch
        {
            0 => Start,
            1 => End,
            2 => Entry,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    internal bool MouseDown(IPosition p)
    {
        var pointSelected = (GetCurrentPageModel() as PickPointViewModel)?.PickPoint(p);
        if (pointSelected == true)
            Next();

        return true;
    }

    internal void MouseMove(IPosition p)
    {
        (GetCurrentPageModel() as PickPointViewModel)?.MouseMove(p);
    }

    internal Model.PointFeature? StartPoint => Start.SelectedPoint;
    internal Model.PointFeature? EndPoint => End.SelectedPoint;
    
    protected override void Back()
    {
        Console.WriteLine("path Back");
        GetCurrentPageModel().OnBack();
        base.Back();
    }

    protected override void Next()
    {
        Console.WriteLine("path Next");
        GetCurrentPageModel().OnNext();
        base.Next();
    }
}