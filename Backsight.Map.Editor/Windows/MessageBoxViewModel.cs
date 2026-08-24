using CommunityToolkit.Mvvm.ComponentModel;

namespace Backsight.Map.Editor.Windows;

public partial class MessageBoxViewModel(string message, string heading = "Note") : DialogViewModel
{
    [ObservableProperty] public string _heading = heading;
    [ObservableProperty] public string _message = message;

    protected override bool CanExecuteOk()
    {
        return true;
    }
}