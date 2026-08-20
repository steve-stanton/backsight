using System;
using System.Linq;
using Backsight.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Backsight.Map.Editor.Windows;

public partial class OpenMapViewModel : ViewModelBase
{
    public event EventHandler<string>? CloseRequested;
    public string[] MapNames { get; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OkCommand))]
    private string? _selectedMapName;

    public OpenMapViewModel(IMapRepository mapRepository)
    {
        MapNames = mapRepository.FindAllMapNames().ToArray();
    }
    
    [RelayCommand(CanExecute=nameof(CanExecuteOk))]
    private void Ok()
    {
        CloseRequested?.Invoke(this, SelectedMapName!);
    }
    
    private bool CanExecuteOk()
    {
        return SelectedMapName is not null;
    }
    
    [RelayCommand]
    private void Cancel()
    {
        CloseRequested?.Invoke(this, "");
    }
}
