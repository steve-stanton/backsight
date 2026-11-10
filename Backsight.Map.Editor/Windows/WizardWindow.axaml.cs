using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace Backsight.Map.Editor.Windows;

internal partial class WizardWindow : DialogWindow<WizardViewModel>
{
    private int _previousPageIndex;
    
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
            UpdateTransitionDirection();
            UpdateCurrentPage();
            _previousPageIndex = ViewModel.CurrentPageIndex;
        }
    }

    private void UpdateTransitionDirection()
    {
        if (PagePresenter is not null)
        {
            bool isForward = ViewModel.CurrentPageIndex >= _previousPageIndex;
            var slide = new PageSlide(TimeSpan.FromMilliseconds(200), PageSlide.SlideAxis.Horizontal);
            PagePresenter.PageTransition = isForward ? slide : new ReverseTransition(slide);
        }
    }
    
    private void UpdateCurrentPage()
    {
        if (ViewModel.CurrentPageIndex >= 0 && ViewModel.CurrentPageIndex < Pages.Count)
        {
            var page = Pages[ViewModel.CurrentPageIndex];
            PagePresenter.Content = page;

            // Defer focus because Avalonia doesn't produce the visual tree until the
            // ContentPresenter materializes stuff
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                { FindControlByTabIndex(page, 0)?.Focus(); },  Avalonia.Threading.DispatcherPriority.Loaded);
            
            (page.DataContext as PageModel)?.OnNavigatedTo();
        }
        else
        {
            PagePresenter.Content = null;
        }
    }

    /// <summary>
    /// Recursively searches the visual tree for a control with the given TabIndex.
    /// </summary>
    private Control? FindControlByTabIndex(Visual root, int tabIndex)
    {
        if (root is Control ctrl && ctrl.TabIndex == tabIndex)
            return ctrl;

        foreach (var child in root.GetVisualChildren())
        {
            var result = FindControlByTabIndex(child, tabIndex);
            if (result is not null)
                return result;
        }

        return null;
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

/// <summary>
/// Wraps an <see cref="IPageTransition"/> and inverts the forward/backward direction.
/// </summary>
/// <remarks>
/// See <a href="https://docs.avaloniaui.net/docs/graphics-animation/page-transitions"/>
/// </remarks>
internal sealed class ReverseTransition : IPageTransition
{
    private readonly IPageTransition _inner;

    public ReverseTransition(IPageTransition inner)
    {
        _inner = inner;
    }

    public System.Threading.Tasks.Task Start(Avalonia.Visual? from, Avalonia.Visual? to, bool forward, System.Threading.CancellationToken cancellationToken)
    {
        return _inner.Start(from, to, !forward, cancellationToken);
    }
}