using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Backsight.Map.Editor.Models;
using CommunityToolkit.Mvvm.Input;

namespace Backsight.Map.Editor.Windows;

public partial class StartupViewModel : DialogViewModel
{
    private readonly string _lastMap;
    private readonly IMapEditorModel _model;
    private readonly IDialogService _dialogService;
    private string? _mapName;
    
    /// <summary>
    /// The name of the map that should be opened (null if a map has not been specified).
    /// </summary>
    internal string? MapName => _mapName;
    
    public StartupViewModel(IMapEditorModel model, IDialogService dialogService)
    {
        _lastMap = GlobalUserSetting.Read("LastMap");
        _model = model;
        _dialogService = dialogService;
    }

    public string OpenLastText => String.IsNullOrEmpty(_lastMap) ? "Open last map" : "Open " + _lastMap;

    private bool CanOpenLastMap => !String.IsNullOrEmpty(_lastMap);
    
    [RelayCommand(CanExecute = nameof(CanOpenLastMap))]
    private void OpenLastMap()
    {
        StartWithMap(_lastMap);
    }

    void StartWithMap(string mapName)
    {
        _mapName = mapName;
        Ok();
    }

    [RelayCommand]
    private async Task OpenMap()
    {
        var vm = new OpenMapViewModel(_model.MapRepository);
        var dialog = new OpenMapWindow(vm);
        var result = await _dialogService.ShowDialog(dialog);

        if (result == DialogResult.OK)
        {
            var mapName = vm.SelectedMapName;
            Debug.Assert(mapName is not null);
            StartWithMap(mapName);
        }
    }
    
    [RelayCommand]
    private async Task CreateMap()
    {
        var vm = new NewMapViewModel(_model);
        var dialog = new NewMapWindow(vm);
        var result = await _dialogService.ShowDialog(dialog);

        if (result == DialogResult.OK)
        {
            var mapName = vm.MapName;
            Debug.Assert(mapName is not null);
            StartWithMap(mapName);
        }
    }
}