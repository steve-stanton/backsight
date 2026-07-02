using System;
using Backsight.Map.Editor.ViewModels;

namespace Backsight.Map.Editor.Views;

public partial class StartupWindow : Avalonia.Controls.Window
{
    public StartupWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
        Closed += OnClosed;
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