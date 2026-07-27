using System;
using Backsight.Map.Editor.Models;

namespace Backsight.Map.Editor.ViewModels;

public interface IMapEditorViewModel
{
    string? CurrentMapName { get; }
    void OpenMap(string mapName);
}

// Responsible for:
// 1. expose visible spatial objects
// 2. selection state
// 3. styling decisions or style keys
// 4. commands
// 5. viewport state

// should probably implement IProvider
public partial class MapEditorViewModel : ViewModelBase, IMapEditorViewModel
{
    private readonly IMapEditorModel _model;
    private string? _currentMapName;

    public MapEditorViewModel() : this(new DesignMapEditorModel())
    {
    }

    public MapEditorViewModel(IMapEditorModel model)
    {
        _model = model;
    }

    public string? CurrentMapName
    {
        get => _currentMapName;
        private set
        {
            if (SetProperty(ref _currentMapName, value))
                OnPropertyChanged(nameof(WindowTitle));
        }
    }

    public string WindowTitle =>
        String.IsNullOrWhiteSpace(CurrentMapName) ? "Map Editor" : CurrentMapName;

    public void OpenMap(string mapName)
    {
        if (String.IsNullOrWhiteSpace(mapName))
            throw new ArgumentException("Map name cannot be empty.", nameof(mapName));

        _model.OpenMap(mapName);
        CurrentMapName = mapName;
    }
}