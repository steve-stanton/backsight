using System;
using System.Threading.Tasks;
using Backsight.Map.Editor.Models;
using CommunityToolkit.Mvvm.Input;

namespace Backsight.Map.Editor.Windows;

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
    private async Task OpenMap(Avalonia.Controls.Window owner)
    {
        var dialog = new OpenMapWindow()
        {
            DataContext = new OpenMapViewModel(_model.MapRepository)
        };
        
        var mapName = await dialog.ShowDialog<string>(owner);
        
        if (!String.IsNullOrEmpty(mapName))
            CloseRequested?.Invoke(this, mapName);
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