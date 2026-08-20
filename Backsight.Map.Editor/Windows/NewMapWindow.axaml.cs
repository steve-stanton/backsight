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
            ViewModel.RequestClose += OnRequestClose;
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        if (ViewModel is not null)
            ViewModel.RequestClose -= OnRequestClose;
    }

    private void OnRequestClose()
    {
        // Close with the name that was entered for the map (should be blank if the user cancelled)
        Close(ViewModel?.MapName);
    }    
}