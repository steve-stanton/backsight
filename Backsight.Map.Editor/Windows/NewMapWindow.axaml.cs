using System;
using Avalonia.Interactivity;

namespace Backsight.Map.Editor.Windows;

public partial class NewMapWindow : Avalonia.Controls.Window
{
    public NewMapWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        this.FocusByTabIndex(0);
    }

    private NewMapViewModel? ViewModel => DataContext as NewMapViewModel;

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