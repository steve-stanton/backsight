using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia;

namespace Backsight.Map.Editor.Windows;

public interface IDialogService
{
    Task ShowMessageBox(string message, string heading = "Message", Avalonia.Controls.Window? owner = null);
    Task<DialogResult> ShowDialog(DialogWindow window);
    void Show(DialogWindow window);
}

internal class DialogService : IDialogService //, IDisposable
{
    private readonly Avalonia.Controls.Window _topLevel;

    //private readonly List<DialogWindow> _modelessDialogs = new();

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

    async Task IDialogService.ShowMessageBox(string message, string heading, Avalonia.Controls.Window? owner)
    {
        var messageBox = new MessageBoxWindow(message, heading);
        await messageBox.ShowDialog<DialogResult>(owner ?? _topLevel);
    }
    
    async Task<DialogResult> IDialogService.ShowDialog(DialogWindow window)
    {
        return await window.ShowDialog<DialogResult>(_topLevel);
    }

    void IDialogService.Show(DialogWindow window)
    {
        //_modelessDialogs.Add(window);
        //window.Closed += OnModelessDialogClosed;
        window.Show(_topLevel);
    }

    /*
    private void OnModelessDialogClosed(object? sender, EventArgs e)
    {
        var dialog = sender as DialogWindow;
        Debug.Assert(dialog is not null);
        
        bool isRemoved = _modelessDialogs.Remove(dialog);
        Debug.Assert(isRemoved);
        
        Console.WriteLine($"Modeless dialog {dialog.GetType().Name} closed with result: {dialog.Result}");
    }

    public void Dispose()
    {
        _topLevel.Closed -= OnModelessDialogClosed;

        foreach (var dialog in _modelessDialogs)
            dialog.Close();
    }
    */
}