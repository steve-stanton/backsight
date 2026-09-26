using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Backsight.Map.Editor.Windows;

public abstract class DialogWindow : Avalonia.Controls.Window
{
    /// <summary>
    /// The dialog service used to display this dialog (null if this dialog is not currently shown).
    /// </summary>
    internal IDialogService? DialogService { get; set; }
    
    public DialogResult Result { get; protected set; } = DialogResult.None;
}

public abstract class DialogWindow<TViewModel> : DialogWindow where TViewModel : DialogViewModel
{
    private readonly TViewModel _viewModel;

    protected DialogWindow(TViewModel viewModel)
    {
        _viewModel = viewModel;
        DataContext = viewModel;
    }
    
    internal TViewModel ViewModel => _viewModel;
    
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        this.FocusByTabIndex(0);
        _viewModel.CloseRequested += OnCloseRequested;
    }

    private void OnCloseRequested(object? sender, DialogResult result)
    {
        Console.WriteLine("OnCloseRequested: " + result);
        Result = result;
        Close(result);
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        Console.WriteLine("Closing dialog window with result: " + Result);
        
        // Force cancellation if the window was closed in an unexpected way (e.g. the user
        // might have clicked the [x] in a Windows title bar)

        if (Result == DialogResult.None)
        {
            Result = DialogResult.Cancel;
            ViewModel.HandleCloseRequested(Result);
        }
        
        DialogService?.OnClosing(this);
        _viewModel.CloseRequested -= OnCloseRequested;
    }
}