using System;
using System.Windows.Input;

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
    /// An item that draws a dividing line between groups of commands.
    /// </summary>
    public static ContextMenuItem Separator { get; } = new(String.Empty, null);

    /// <summary>
    /// Is this item a dividing line rather than a command?
    /// </summary>
    public bool IsSeparator => Command is null;
}
