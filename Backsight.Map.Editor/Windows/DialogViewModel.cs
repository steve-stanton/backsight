using System;
using CommunityToolkit.Mvvm.Input;

namespace Backsight.Map.Editor.Windows;

public abstract partial class DialogViewModel : ViewModelBase
{
    /// <summary>
    /// Event indicating that the dialog needs to be closed (usually when a user
    /// clicks the OK or Cancel button).
    /// </summary>
    /// <remarks>
    /// The view needs to listen for this event, and respond to it by closing itself
    /// (e.g. see <see cref="DialogWindow"/>).
    /// </remarks>
    public event EventHandler<DialogResult>? CloseRequested;

    [RelayCommand(CanExecute=nameof(CanExecuteOk))]
    protected void Ok()
    {
        RequestClose(DialogResult.OK);
    }
    
    protected virtual bool CanExecuteOk()
    {
        return true;
    }

    [RelayCommand(CanExecute=nameof(CanExecuteCancel))]
    protected void Cancel()
    {
        RequestClose(DialogResult.Cancel);
    }
    
    protected virtual bool CanExecuteCancel()
    {
        return true;
    }

    protected void RequestClose(DialogResult result)
    {
        CloseRequested?.Invoke(this, result);
    }
}