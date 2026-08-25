using System;
using System.Threading.Tasks;

namespace Backsight.Map.Editor.Windows;

public interface IDialogService
{
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
    /// <param name="dock">Display the dialog in a docked panel of the main window.</param>
    void Show(DialogWindow window, bool dock = false);
    
    /// <summary>
    /// Called when a dialog window is closing.
    /// </summary>
    /// <param name="window">The window that is being closed.</param>
    void OnClosing(DialogWindow window);
}

internal static class IDialogServiceEx
{
    extension(IDialogService dialogService)
    {
        /// <summary>
        /// Displays a message box.
        /// </summary>
        /// <param name="message">The message to show in the body of the box.</param>
        /// <param name="heading">The heading of a group box that surrounds the message.</param>
        internal async Task ShowMessageBox(string message, string heading = "Message")
        {
            var messageBox = new MessageBoxWindow(message, heading);
            await dialogService.ShowDialog(messageBox);
        }
    }
}

internal class DialogService : IDialogService
{
    private readonly MapEditorWindow _mainWindow;

    /// <summary>
    /// The dialog window that is currently docked in the main window.
    /// </summary>
    /// <remarks>
    /// Only one dialog window can be docked at a time.
    /// </remarks>
    private DialogWindow? _dockedWindow;

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
        _mainWindow = mainWindow;
    }

    /// <inheritdoc />
    async Task<DialogResult> IDialogService.ShowDialog(DialogWindow window)
    {
        window.DialogService = this;
        return await window.ShowDialog<DialogResult>(_mainWindow);
    }

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">Another dialog is already docked.</exception>
    void IDialogService.Show(DialogWindow window, bool dock)
    {
        if (dock && _dockedWindow is not null)
            throw new InvalidOperationException("Another dialog is already docked.");

        window.DialogService = this;
        
        if (dock)
        {
            _dockedWindow = window;
            _mainWindow.SetDockPanel(window);
        }
        else
        {
            window.Show(_mainWindow);
        }
    }

    /// <inheritdoc />
    void IDialogService.OnClosing(DialogWindow window)
    {
        if (ReferenceEquals(_dockedWindow, window))
        {
            Console.WriteLine("Docked window is closing with result: " + window.Result);
            _mainWindow.ClearDockPanel();
            _dockedWindow = null;
        }

        window.DialogService = null;
    }
}