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
    
    /// <summary>
    /// The position perpendicular to the reference line where the parallel starts (or the
    /// radial position if the reference line is a circular arc).
    /// </summary>
    internal IPosition? Parallel1 { get; private set; }

    /// <summary>
    /// The position perpendicular to the reference line where the parallel ends (or the
    /// radial position if the reference line is a circular arc).
    /// </summary>
    internal IPosition? Parallel2 { get; private set; }

    /// <summary>
    /// The position of the parallel where it intersects the first terminal line (if any).
    /// </summary>
    internal IPosition? OnTerminal1 { get; private set; }

    /// <summary>
    /// The position of the parallel where it intersects the second terminal line (if any).
    /// </summary>
    internal IPosition? OnTerminal2 { get; private set; }

    internal ParallelViewModel(ParallelTool tool, LineFeature referenceLine)
    {
        Tool = tool;
        ReferenceLine = referenceLine;
        Offset = new ParallelOffsetViewModel(this);
        Terminal1 = new ParallelTerminalViewModel(this);
        Terminal2 = new ParallelTerminalViewModel(this);
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

    internal void OnOffsetChanged(Observation? offset)
    {
        // Attempt to calculate the parallel based on the offset alone
        if (offset is null)
        {
            Parallel1 = Parallel2 = null;
        }
        else
        {
            ReferenceLine.CalculateParallel(offset, out var start, out var end);
            Parallel1 = start;
            Parallel2 = end;
        }

        // The ParallelTerminalViewModel currently nulls out the selected terminal line if the
        // user goes back to the offset page of the wizard. At that point, the two terminals
        // held here should become n/a. That being the case, ensure they are in fact undefined.
        
        OnTerminal1 = OnTerminal2 = null;

        Tool.RefreshMapDisplay();
    }

    /// <summary>
    /// Handles any change to a terminal line selected by the user.
    /// </summary>
    /// <param name="sender">The view model where the change has been made.</param>
    internal void OnTerminalChanged(ParallelTerminalViewModel sender)
    {
        var terminal = sender.SelectedLine;
        if (terminal is null)
        {
            if (ReferenceEquals(sender, Terminal1))
                OnTerminal1 = null;
            else
                OnTerminal2 = null;
        }
        else
        {
            // TODO: Attempt to calculate the terminal position
            
        }
        
        Tool.RefreshMapDisplay();
    }

    protected override bool ValidateCurrentPage()
    {
        var pageModel = GetCurrentPageModel() as PageModel;
        return pageModel?.ValidatePage() ?? true;
    }

}