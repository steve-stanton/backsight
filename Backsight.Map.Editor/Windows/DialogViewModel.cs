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
        CloseRequested?.Invoke(this, PositiveResult);
    }
    
    protected virtual bool CanExecuteOk()
    {
        return true;
    }
    
    protected virtual DialogResult PositiveResult => DialogResult.OK;

    [RelayCommand(CanExecute=nameof(CanExecuteCancel))]
    protected void Cancel()
    {
        CloseRequested?.Invoke(this, NegativeResult);
    }
    
    protected virtual bool CanExecuteCancel()
    {
        return true;
    }
    
    protected virtual DialogResult NegativeResult => DialogResult.Cancel;
}