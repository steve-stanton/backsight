using System;
using System.Diagnostics;
using Backsight.Map.Editor.Windows;

namespace Backsight.Map.Editor.Tools;

/// <summary>
/// View model for a command tool that involves some sort of window.
/// </summary>
internal abstract class ToolViewModel<T> : DialogViewModel where T : CommandTool
{
    internal T Tool { get; }
    
    protected ToolViewModel(T tool)
    {
        Tool = tool;
    }
    
    internal override bool HandleCloseRequested(DialogResult result)
    {
        Console.WriteLine("Requesting close with result: " + result);

        if (result == DialogResult.OK)
        {
            Debug.Assert(CanExecuteOk());
            Tool.FinishDataEntry(this);
        }
        else
        {
            Tool.CancelDataEntry(this);
        }

        return true;
    }
    
}