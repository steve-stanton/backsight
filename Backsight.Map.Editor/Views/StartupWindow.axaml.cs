using System;
using Avalonia.Interactivity;
using Backsight.Map.Editor.ViewModels;
using Backsight.Map.Editor.Windows;

namespace Backsight.Map.Editor.Views;

public partial class StartupWindow : Avalonia.Controls.Window
{
    public StartupWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
        Closed += OnClosed;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        this.FocusByTabIndex(0);
    }

    private StartupViewModel? ViewModel => DataContext as StartupViewModel;
    
    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (ViewModel is not null)
            ViewModel.CloseRequested += OnCloseRequested;
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        if (ViewModel is not null)
            ViewModel.CloseRequested -= OnCloseRequested;
    }

    private void OnCloseRequested(object? sender, string result)
    {
        Close(result);
    }
}