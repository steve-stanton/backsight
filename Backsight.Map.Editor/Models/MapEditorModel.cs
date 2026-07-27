using System;
using System.Collections.Generic;
using Backsight.Database;
using Backsight.Environment;
using Backsight.Model;
using Svg;

namespace Backsight.Map.Editor.Models;

public interface IMapEditorModel
{
    /// <summary>
    /// The name of the map that is currently being edited (blank if no map is open).
    /// </summary>
    string MapName { get; }

    /// <summary>
    /// Creates a new map. If this completes without any exception, you can
    /// call <see cref="OpenMap"/> to work with it.
    /// </summary>
    /// <param name="mapName">The user-perceived map name.</param>
    /// <param name="layer">The map layer.</param>
    void CreateMap(string mapName, ILayer layer);
    
    /// <summary>
    /// Opens a map for editing.
    /// </summary>
    /// <param name="mapName"></param>
    void OpenMap(string mapName);
    
    /// <summary>
    /// Closes the current map.
    /// </summary>
    void CloseMap();
}

public sealed class DesignMapEditorModel : IMapEditorModel
{
    public string MapName => "Design";

    public void CreateMap(string mapName, ILayer layer)
    {
    }

    public void OpenMap(string mapName)
    {
    }

    public void CloseMap()
    {
    }
}

public class MapEditorModel : IMapEditorModel
{
    private readonly IEnvironmentRepository _envRepo;
    private readonly IMapRepository _mapRepo;

    // The map (if any) that is currently being edited (null if a map has not been opened)
    // This implementation will allow for just the one map, but it should be viable to allow for
    // more than one map (it could be nice to see two adjacent maps at the same time)
    private IMapStore? _store;
    
    // The list of sessions should probably be part of the IMapStore implementation.
    // But it would be good to retain a list of unsaved operations here (leaving the store ONLY
    // for things that have been saved)
    //private List<Session> _sessions;
    
    /// <summary>
    /// Changes that have been recorded, but not yet saved as far as the user is concerned.
    /// </summary>
    //private List<Change> _changes = new();
    
    public MapEditorModel(IEnvironmentRepository envRepo, IMapRepository mapRepo)
    {
        _envRepo = envRepo;
        _mapRepo = mapRepo;
        
        _envRepo.Load();
    }

    /// <inheritdoc />
    public string MapName => _store?.Name ?? "";

    /// <inheritdoc />
    public void CreateMap(string mapName, ILayer layer)
    {
        if (String.IsNullOrWhiteSpace(mapName))
            throw new ArgumentException("Map name must be specified");

        if (_mapRepo.CanOpen(mapName))
            throw new ArgumentException("Map already exists");
        
        var settings = new MapSettings
        {
            ActiveLayer = layer.Id
        };
        
        // Initialize default entity types for the layer
        settings.GetDefaults(layer);

        // Save an empty map
        _mapRepo.CreateMap(mapName, settings);
    }
    
    /// <inheritdoc />
    public void OpenMap(string mapName)
    {
        if (String.IsNullOrWhiteSpace(mapName))
            throw new ArgumentException("Map name cannot be empty.");

        try
        {
            _store = _mapRepo.OpenMap(mapName);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <inheritdoc />
    public void CloseMap()
    {
        if (_store is null)
            throw new InvalidOperationException("No map is open.");

        _mapRepo.CloseMap(_store);
        _store = null;
    }

    /// <summary>
    /// Have all edits been saved?
    /// </summary>
    internal bool IsSaved => _store?.IsSaved ?? true;
}