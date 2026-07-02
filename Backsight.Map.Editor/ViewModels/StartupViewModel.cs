using System;
using CommunityToolkit.Mvvm.Input;

namespace Backsight.Map.Editor.ViewModels;

public partial class StartupViewModel : ViewModelBase
{
    public event EventHandler<string>? CloseRequested;
    
    private readonly string _lastMap;
    
    public StartupViewModel()
    {
        _lastMap = GlobalUserSetting.Read("LastMap");
    }

    public string OpenLastText => String.IsNullOrEmpty(_lastMap) ? "Open last map" : "Open " + _lastMap;

    private bool CanOpenLastMap => !String.IsNullOrEmpty(_lastMap);
    
    [RelayCommand(CanExecute = nameof(CanOpenLastMap))]
    private void OpenLastMap()
    {
        CloseRequested?.Invoke(this, _lastMap);
    }

    [RelayCommand]
    private void OpenMap()
    {
        CloseRequested?.Invoke(this, "OpenMap");
    }
    
    [RelayCommand]
    private void CreateMap()
    {
        CloseRequested?.Invoke(this, "CreateMap");
    }

    [RelayCommand]
    private void Exit()
    {
        CloseRequested?.Invoke(this, "");
    }
}