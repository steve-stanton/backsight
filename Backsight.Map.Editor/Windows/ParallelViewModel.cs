using System;
using System.Diagnostics;
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
        Terminal1 = new ParallelTerminalViewModel(this);
        Terminal2 = new ParallelTerminalViewModel(this);
    }

    internal bool MouseDown(IPosition p)
    {
        return CurrentPageIndex switch
        {
            0 => Offset.PickPoint(p),
            1 => Terminal1.PickLine(p),
            2 => Terminal2.PickLine(p),
            _ => false
        };
    }

    internal void MouseMove(IPosition p)
    {
        if (CurrentPageIndex == 0)
            Offset.MouseMove(p);

        // We don't (currently) auto-highlight terminal lines, although that would be consistent
    }

    private PageModel GetCurrentPageModel()
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
        GetCurrentPageModel().OnBack();
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

    protected override bool ValidateCurrentPage()
    {
        return GetCurrentPageModel().ValidatePage();
    }

    /// <summary>
    /// Attempts to find a position on a line that has been selected as a terminal for the parallel.
    /// </summary>
    /// <param name="line">The line (if any) that has been selected.</param>
    /// <returns>The intersection (if any). In cases where the line intersects the parallel
    /// more than once, you get an arbitrary intersection.</returns>
    internal IPosition? FindTerminalPosition(LineFeature? line)
    {
        if (line is null)
            return null;
        
        // Assume this is called only by the ParallelTerminalViewModel for the current wizard page
        var parpos = CurrentPageIndex == 1 ? Offset.StartPosition : Offset.EndPosition; 
        return ReferenceLine.GetIntersect(parpos, line);
    }

    /// <summary>
    /// Attempts to find a line that is close to one end of the parallel line.
    /// </summary>
    /// <returns>The closest line (if any), together with the position (if any) where the parallel
    /// line intersects it.</returns>
    internal (LineFeature?, IPosition?) FindTerminal()
    {
        // This should only be called by the ParallelTerminalViewModel for the current wizard page
        // so a parallel position should have been defined
        var parpos = CurrentPageIndex == 1 ? Offset.StartPosition : Offset.EndPosition;
        Debug.Assert(parpos is not null);

        // Get the offset to the parallel (on the mapping plane)
        var offset = Offset.GetPlanarDistance(parpos);

        // Search for the line closest to the parallel point, using a tolerance that's either half
        // half the offset, or half a centimetre at the current draw scale (whichever is smaller)
        var mapScale = (Tool.ViewModel as IMapEditorViewModel).MapScale;
        ILength tol = new Length(Math.Min(offset * 0.5, mapScale * 0.005));
        var line = Tool.Store.Model.QueryClosest(parpos, tol, SpatialType.Line) as LineFeature;

        if (line is null)
            return (null, null);
        
        // Return the line along with the position of any intersection
        var p = ReferenceLine.GetIntersect(parpos, line);
        return (line, p);
    }

    /// <summary>
    /// The position on a terminal line for the current wizard page (null if a good terminal line
    /// has not been selected, or some other wizard page is currently shown).
    /// </summary>
    internal IPosition? CurrentTerminalPosition => CurrentPageIndex switch
    {
        1 => Terminal1.Position,
        2 => Terminal2.Position,
        _ => null
    };
}