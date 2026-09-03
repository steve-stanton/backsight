using CommunityToolkit.Mvvm.Input;

namespace Backsight.Map.Editor.Windows;

public partial class SaveChangesViewModel : DialogViewModel
{
    public string Message { get; }

    internal SaveChangesViewModel(string message)
    {
        Message = message;
    }

    [RelayCommand]
    private void Yes()
    {
        RequestClose(DialogResult.Yes);
    }

    [RelayCommand]
    private void No()
    {
        RequestClose(DialogResult.No);
    }
}