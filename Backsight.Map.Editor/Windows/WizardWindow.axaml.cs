using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Backsight.Map.Editor.Windows;

public partial class WizardWindow : DialogWindow<WizardViewModel>
{
    public ObservableCollection<UserControl> Pages { get; } = new();

    /// <summary>
    /// Design-time constructor.
    /// </summary>
    public WizardWindow() : base(null!)
    {
        InitializeComponent();
    }

    internal WizardWindow(WizardViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
        Pages.CollectionChanged += OnPagesCollectionChanged;
        viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnPagesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ViewModel.PageCount = Pages.Count;
        UpdateCurrentPage();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(WizardViewModel.CurrentPageIndex))
        {
            UpdateCurrentPage();
        }
    }

    private void UpdateCurrentPage()
    {
        if (ViewModel.CurrentPageIndex >= 0 && ViewModel.CurrentPageIndex < Pages.Count)
        {
            var page = Pages[ViewModel.CurrentPageIndex];
            PagePresenter.Content = page;
            (page.DataContext as PageModel)?.OnNavigatedTo();
        }
        else
        {
            PagePresenter.Content = null;
        }
    }
    
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        AdjustSizeToFitAllPages();
    }

    private void AdjustSizeToFitAllPages()
    {
        if (Pages.Count == 0)
            return;

        double maxPageWidth = 0;
        double maxPageHeight = 0;

        // Available constraint for measurement
        var infiniteSize = new Avalonia.Size(double.PositiveInfinity, double.PositiveInfinity);

        foreach (var page in Pages)
        {
            page.Measure(infiniteSize);
            var desired = page.DesiredSize;

            if (desired.Width > maxPageWidth)
                maxPageWidth = desired.Width;
            if (desired.Height > maxPageHeight)
                maxPageHeight = desired.Height;
        }

        // Account for header, separator, footer buttons, and Grid margins
        const double horizontalChromePadding = 40;
        const double verticalChromePadding = 180;

        Width = Math.Max(MinWidth, maxPageWidth + horizontalChromePadding);
        Height = Math.Max(MinHeight, maxPageHeight + verticalChromePadding);
    }
}