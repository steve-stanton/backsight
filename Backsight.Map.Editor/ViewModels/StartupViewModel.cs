using System;
using System.Threading.Tasks;
using Backsight.Database;
using Backsight.Map.Editor.Models;
using Backsight.Map.Editor.Windows;
using CommunityToolkit.Mvvm.Input;

namespace Backsight.Map.Editor.ViewModels;

public partial class StartupViewModel : ViewModelBase
{
    public event EventHandler<string>? CloseRequested;
    
    private readonly string _lastMap;
    private readonly IMapEditorModel _model;
    
    public StartupViewModel(IMapEditorModel model)
    {
        _lastMap = GlobalUserSetting.Read("LastMap");
        _model = model;
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
    private async Task CreateMap(Avalonia.Controls.Window owner)
    {
        var dialog = new NewMapWindow
        {
            DataContext = new NewMapViewModel(_model)
        };
        var newMapName = await dialog.ShowDialog<string>(owner);
        
        if (!String.IsNullOrEmpty(newMapName))
            CloseRequested?.Invoke(this, newMapName);
    }

    [RelayCommand]
    private void Exit()
    {
        CloseRequested?.Invoke(this, "");
    }
}