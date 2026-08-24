using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Backsight.Map.Editor.Windows;

public abstract class DialogWindow : Avalonia.Controls.Window
{
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
    
    protected TViewModel ViewModel => _viewModel;
    
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        this.FocusByTabIndex(0);
        _viewModel.CloseRequested += OnCloseRequested;
    }

    private void OnCloseRequested(object? sender, DialogResult result)
    {
        Console.WriteLine("DialogWindow close requested");
        Result = result;
        Close(result);
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        Console.WriteLine("DialogWindow closing");
        _viewModel.CloseRequested -= OnCloseRequested;
    }
}