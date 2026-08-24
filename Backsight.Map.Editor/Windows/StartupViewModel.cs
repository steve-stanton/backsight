using System;
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
    private async Task OpenMap(Avalonia.Controls.Window owner)
    {
        var dialog = new OpenMapWindow()
        {
            DataContext = new OpenMapViewModel(_model.MapRepository)
        };
        
        var mapName = await dialog.ShowDialog<string>(owner);

        if (!String.IsNullOrEmpty(mapName))
            StartWithMap(mapName);
    }
    
    [RelayCommand]
    private async Task CreateMap(Avalonia.Controls.Window owner)
    {
        var vm = new NewMapViewModel(_model);
        var dialog = new NewMapWindow(vm);
        var result = await dialog.ShowDialog<DialogResult>(owner);

        if (result == DialogResult.OK)
            StartWithMap(vm.MapName);
    }
}