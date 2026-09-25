using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Backsight.Map.Editor.Windows;

/// <summary>
/// Base class for a view model that can be used with a wizard-style data entry dialog. 
/// </summary>
public abstract partial class WizardViewModel : DialogViewModel
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(BackCommand))]
    [NotifyCanExecuteChangedFor(nameof(NextCommand))]
    [NotifyCanExecuteChangedFor(nameof(FinishCommand))]
    [NotifyPropertyChangedFor(nameof(IsFirstPage))]
    [NotifyPropertyChangedFor(nameof(IsLastPage))]
    private int _currentPageIndex;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NextCommand))]
    [NotifyCanExecuteChangedFor(nameof(FinishCommand))]
    [NotifyPropertyChangedFor(nameof(IsLastPage))]
    private int _pageCount;

    [ObservableProperty]
    private string _title = "Wizard";

    public bool IsFirstPage => CurrentPageIndex == 0;
    public bool IsLastPage => PageCount > 0 && CurrentPageIndex == PageCount - 1;

    [RelayCommand(CanExecute = nameof(CanGoBack))]
    protected virtual void Back()
    {
        if (CurrentPageIndex > 0)
        {
            CurrentPageIndex--;
        }
    }

    protected virtual bool CanGoBack() => CurrentPageIndex > 0;

    [RelayCommand(CanExecute = nameof(CanGoNext))]
    protected virtual void Next()
    {
        if (CurrentPageIndex < PageCount - 1 && ValidateCurrentPage())
        {
            CurrentPageIndex++;
        }
    }

    protected virtual bool CanGoNext() => PageCount > 0 && CurrentPageIndex < PageCount - 1;

    [RelayCommand(CanExecute = nameof(CanFinish))]
    protected virtual void Finish()
    {
        if (ValidateCurrentPage())
        {
            RequestClose(DialogResult.OK);
        }
    }

    protected virtual bool CanFinish() => IsLastPage;

    /// <summary>
    /// Override to implement per-step validation before navigating forward or finishing.
    /// </summary>
    protected virtual bool ValidateCurrentPage() => true;
}