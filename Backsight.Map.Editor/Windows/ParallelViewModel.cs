using System;
using Backsight.Map.Editor.Tools;
using Backsight.Model;

namespace Backsight.Map.Editor.Windows;

public class ParallelViewModel : WizardViewModel
{
    /// <summary>
    /// The data entry tool for generating parallel lines.
    /// </summary>
    internal ParallelTool Tool { get; }

    /// <summary>
    /// The reference line for the parallel.
    /// </summary>
    internal LineFeature ReferenceLine { get; }

    internal ParallelOffsetViewModel Offset { get; }
    internal ParallelTerminalViewModel Terminal1 { get; }
    internal ParallelTerminalViewModel Terminal2 { get; }

    internal ParallelViewModel(ParallelTool tool, LineFeature referenceLine)
    {
        Tool = tool;
        ReferenceLine = referenceLine;
        Offset = new ParallelOffsetViewModel(this);
        Terminal1 = new ParallelTerminalViewModel(this, false);
        Terminal2 = new ParallelTerminalViewModel(this, true);
    }

    internal bool MouseDown(IPosition p)
    {
        bool result = CurrentPageIndex switch
        {
            0 => Offset.PickPoint(p),
            1 => Terminal1.PickLine(p),
            2 => Terminal2.PickLine(p),
            _ => false
        };

        // TODO: Is this needed? What if the user didn't pick something?
        //if (result)
        //    Tool.RefreshMapDisplay();

        return result;
    }

    internal void MouseMove(IPosition p)
    {
        if (CurrentPageIndex == 0)
            Offset.MouseMove(p);

        // We don't (currently) auto-highlight terminal lines, although that would be consistent
    }

    private ViewModelBase GetCurrentPageModel()
    {
        return CurrentPageIndex switch
        {
            0 => Offset,
            1 => Terminal1,
            2 => Terminal2,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    protected override void Back()
    {
        // Wizard pages don't HAVE to extend PageModel, those that do may or may not provide
        // an implementation that does something

        (GetCurrentPageModel() as PageModel)?.OnBack();
        base.Back();
    }

    internal override bool HandleCloseRequested(DialogResult result)
    {
        if (result == DialogResult.OK)
        {
            // return if fails validation
            //Debug.Assert(CanExecuteOk());
            
            Tool.FinishDataEntry(this);
        }
        else
        {
            Tool.CancelDataEntry(this);
        }

        return true;
    }

}