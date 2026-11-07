using System;
using Backsight.Map.Editor.Tools;
using Backsight.Model.Operations;

namespace Backsight.Map.Editor.Windows;

public class PathViewModel : WizardViewModel
{
    internal ConnectionPathTool Tool { get; }
    internal PathTerminalsViewModel Terminals { get; }
    internal PathEntryViewModel Entry { get; }
    internal PathOperation? Recall { get; }

    internal PathViewModel(ConnectionPathTool tool, PathOperation? recall)
    {
        Tool = tool;
        Recall = recall;
        Terminals = new PathTerminalsViewModel(this);
        Entry = new PathEntryViewModel(this);
    }

    protected override bool ValidateCurrentPage()
    {
        return GetCurrentPageModel().ValidatePage();
    }

    private PageModel GetCurrentPageModel()
    {
        return CurrentPageIndex switch
        {
            0 => Terminals,
            1 => Entry,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    internal bool MouseDown(IPosition p)
    {
        if (CurrentPageIndex == 0)
            return Terminals.PickPoint(p);
        else
            return false;
    }

    internal void MouseMove(IPosition p)
    {
        if (CurrentPageIndex == 0)
            Terminals.MouseMove(p);
    }
}