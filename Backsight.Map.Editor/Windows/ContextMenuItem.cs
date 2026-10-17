using System;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace Backsight.Map.Editor.Windows;

/// <summary>
/// Describes one entry in a context menu that a view model wants to display.
/// </summary>
/// <param name="Header">The text for the menu item (not used for a separator).</param>
/// <param name="Command">The command to execute when the item is picked (null for a separator).</param>
/// <param name="IsChecked">Should a checkmark appear alongside the menu item?</param>
/// <remarks>
/// This lets a view model decide the content of a context menu without any dependency on the
/// UI toolkit. It's up to the view to turn these items into menu controls.
/// </remarks>
public record ContextMenuItem(string Header, ICommand? Command, bool IsChecked = false)
{
    /// <summary>
    /// Creates a context menu item from an Action delegate.
    /// </summary>
    public ContextMenuItem(string header, Action execute, bool isChecked = false)
        : this(header, new RelayCommand(execute), isChecked)
    {
    }

    /// <summary>
    /// Creates a context menu item from a synchronous Action delegate with a guard condition.
    /// </summary>
    public ContextMenuItem(string header, Action execute, Func<bool> canExecute, bool isChecked = false)
        : this(header, new RelayCommand(execute, canExecute), isChecked)
    {
    }
    
    /// <summary>
    /// Creates a context menu item from an asynchronous Func&lt;Task&gt; delegate.
    /// </summary>
    public ContextMenuItem(string header, Func<Task> execute, bool isChecked = false)
        : this(header, new AsyncRelayCommand(execute), isChecked)
    {
    }
    
    /// <summary>
    /// Creates a context menu item from an asynchronous Func&lt;Task&gt; delegate with a guard condition.
    /// </summary>
    public ContextMenuItem(string header, Func<Task> execute, Func<bool> canExecute, bool isChecked = false)
        : this(header, new AsyncRelayCommand(execute, canExecute), isChecked)
    {
    }
    
    /// <summary>
    /// An item that draws a dividing line between groups of commands.
    /// </summary>
    public static ContextMenuItem Separator { get; } = new(String.Empty, (ICommand?)null);

    /// <summary>
    /// Is this item a dividing line rather than a command?
    /// </summary>
    public bool IsSeparator => Command is null;
}
