using System.Threading.Tasks;

namespace Backsight.Map.Editor.Windows;

public interface IDialogService
{
    /// <summary>
    /// Displays a message box.
    /// </summary>
    /// <param name="message">The message to show in the body of the box.</param>
    /// <param name="heading">The heading of a group box that surrounds the message.</param>
    /// <param name="owner">The window that the message box is associated with (null to use
    /// the top-level window known to the dialog service).</param>
    Task ShowMessageBox(string message, string heading = "Message", Avalonia.Controls.Window? owner = null);

    /// <summary>
    /// Displays a modal dialog.
    /// </summary>
    /// <param name="window">The dialog window to show.</param>
    /// <returns>The way that the user closed the dialog.</returns>
    Task<DialogResult> ShowDialog(DialogWindow window);
    
    /// <summary>
    /// Displays a modeless dialog.
    /// </summary>
    /// <param name="window">The dialog window to show.</param>
    void Show(DialogWindow window);
}

internal class DialogService : IDialogService
{
    private readonly Avalonia.Controls.Window _topLevel;

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogService"/> class that
    /// uses the supplied window as the owner of all dialogs.
    /// </summary>
    /// <param name="owner">The window to use as the owner for sub-dialogs.</param>
    internal DialogService(DialogWindow owner)
    {
        _topLevel = owner;
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="DialogService"/> class that
    /// uses the application's main window as the owner of all dialogs.
    /// </summary>
    /// <param name="mainWindow">The main application window.</param>
    /// <remarks>
    /// This constructor should be used as part of dependency injection.
    /// </remarks>
    public DialogService(MapEditorWindow mainWindow)
    {
        _topLevel = mainWindow;
    }

    /// <inheritdoc />
    async Task IDialogService.ShowMessageBox(string message, string heading, Avalonia.Controls.Window? owner)
    {
        var messageBox = new MessageBoxWindow(message, heading);
        await messageBox.ShowDialog<DialogResult>(owner ?? _topLevel);
    }
    
    /// <inheritdoc />
    async Task<DialogResult> IDialogService.ShowDialog(DialogWindow window)
    {
        return await window.ShowDialog<DialogResult>(_topLevel);
    }

    /// <inheritdoc />
    void IDialogService.Show(DialogWindow window)
    {
        window.Show(_topLevel);
    }
}