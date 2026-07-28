using System;
using Backsight.Map.Editor.Models;

namespace Backsight.Map.Editor.ViewModels;

public interface IMapEditorViewModel
{
    string? CurrentMapName { get; }
    void OpenMap(string mapName);
    
    /// <summary>
    /// Closes any map that is currently open.
    /// </summary>
    /// <returns>True if a map was closed, or false if there was no open map.</returns>
    bool CloseMap();
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

    /// <inheritdoc />
    /// <exception cref="ArgumentException">Map name cannot be empty.</exception>
    public void OpenMap(string mapName)
    {
        if (String.IsNullOrWhiteSpace(mapName))
            throw new ArgumentException("Map name cannot be empty.", nameof(mapName));

        _model.OpenMap(mapName);
        CurrentMapName = mapName;
    }

    /// <inheritdoc />
    public bool CloseMap()
    {
        if (CurrentMapName is null)
            return false;
        
        if (_model.RequiresSave)
        {
            // TODO: Prompt if changes need to be saved
            Console.WriteLine("Map changes were not saved");
        }
        
        CurrentMapName = null;
        _model.CloseMap();
        return true;
    }
}