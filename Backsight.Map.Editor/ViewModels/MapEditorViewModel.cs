using System;
using Backsight.Map.Editor.Models;

namespace Backsight.Map.Editor.ViewModels;

public interface IMapEditorViewModel
{
    /// <summary>
    /// The data for a map display.
    /// </summary>
    Mapsui.Map MapData { get; }
    
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

// should probably implement IProvider (or delegate to something that does)
/// <summary>
/// An implementation of a view model for <see cref="Backsight.Map.Editor.Views.MapEditorWindow"/>.
/// </summary>
public partial class MapEditorViewModel : ViewModelBase, IMapEditorViewModel
{
    /// <summary>
    /// The application model.
    /// </summary>
    private readonly IMapEditorModel _model;
    
    /// <summary>
    /// The map data for the map display.
    /// </summary>
    /// <remarks>
    /// This acts like a helper that feeds a Mapsui map control that should be present inside
    /// the map editor view. The map control should automatically pick up changes made via this
    /// instance, so it acts kind of like an inner view model.
    /// <para/>
    /// Meanwhile, the <c>MapEditorViewModel</c> class as a whole is expected to expose only those
    /// properties that the enclosing <c>MapEditorWindow</c> can bind to.
    /// </remarks>
    private readonly Mapsui.Map _mapData;

    public MapEditorViewModel() : this(new DesignMapEditorModel())
    {
    }

    public MapEditorViewModel(IMapEditorModel model)
    {
        _model = model;
        _mapData = new Mapsui.Map();
    }

    /// <inheritdoc />
    Mapsui.Map IMapEditorViewModel.MapData => _mapData;
    
    public string? CurrentMapName
    {
        get;
        private set
        {
            if (SetProperty(ref field, value))
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