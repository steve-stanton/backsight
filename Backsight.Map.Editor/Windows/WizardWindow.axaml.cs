using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Avalonia.Controls;

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

    public WizardWindow(WizardViewModel viewModel) : base(viewModel)
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
            // Ensure the page shares the single wizard DataContext
            page.DataContext = ViewModel;
            PagePresenter.Content = page;
        }
        else
        {
            PagePresenter.Content = null;
        }
    }
}