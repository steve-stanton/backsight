using System;
using Backsight.Database;
using Backsight.Environment;
using Backsight.Model;

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
    /// <param name="mapName">The name of the map to be opened.</param>
    /// <remarks>
    /// The model should only support one open map. An exception should be thrown if
    /// you attempt to open a 2nd map.
    /// </remarks>
    void OpenMap(string mapName);
    
    /// <summary>
    /// Closes the current map (or does nothing if a map has not been opened).
    /// </summary>
    /// <param name="saveChanges">Should any unsaved changes be saved?</param>
    void CloseMap(bool saveChanges);
    
    /// <summary>
    /// The extent of the current map (null if there is no map, or the map is empty).
    /// </summary>
    IWindow? Extent { get; }
    
    /// <summary>
    /// The underlying store for the map that is currently open (null if there is no map).
    /// </summary>
    IMapStore? Store { get; }
    
    /// <summary>
    /// The repository containing details about the operating environment.
    /// </summary>
    IEnvironmentRepository Environment { get; }
    
    /// <summary>
    /// The repository for holding maps.
    /// </summary>
    IMapRepository MapRepository { get; }
}

public sealed class DesignMapEditorModel : IMapEditorModel
{
    public string MapName => "Design";
    public void CreateMap(string mapName, ILayer layer) {}
    public void OpenMap(string mapName) {}
    public void CloseMap(bool saveChanges) {}
    public IWindow? Extent => null;
    public IMapStore? Store => null;
    public IEnvironmentRepository Environment => new EmptyRepository();
    public IMapRepository MapRepository => null!;
}

public class MapEditorModel : IMapEditorModel
{
    private readonly IEnvironmentRepository _envRepo;
    private readonly IMapRepository _mapRepo;

    // The map (if any) that is currently being edited (null if a map has not been opened)
    // This implementation will allow for just the one map, but it should be viable to allow for
    // more than one map (it could be nice to see two adjacent maps at the same time)
    private IMapStore? _store;
    
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
    /// <exception cref="InvalidOperationException">Map already opened.</exception>
    public void OpenMap(string mapName)
    {
        // Disallow an attempt to open more than one map
        if (_store is not null)
            throw new InvalidOperationException("Map already opened.");
        
        if (String.IsNullOrWhiteSpace(mapName))
            throw new ArgumentException("Map name cannot be empty.");

        try
        {
            // Deserialize from the repo
            _store = _mapRepo.OpenMap(mapName);

            // Initialize geometry etc
            _store.Model.Load();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <inheritdoc />
    public void CloseMap(bool saveChanges)
    {
        if (_store is null)
            return;

        // If the user decided NOT to save changes, get rid of any change entries that follow the
        // last savepoint (this may or may not include the NewSessionEvent record)
        if (!saveChanges)
            _mapRepo.RemoveChanges(_store);
        
        // Finalize the map repository. If the editing session contains at least one change, the MapsDirectory
        // implementation will combine the associated data files into a single file, finishing off with an
        // EndSessionEvent record. If there are no changes, the MapsDirectory implementation will simply
        // delete the file that holds the NewSessionEvent record.
        _mapRepo.CloseMap(_store);
        
        _store = null;
    }
    
    /// <inheritdoc />
    public IWindow? Extent
    {
        get
        {
            if (_store is null)
                return null;
            
            var extent = _store.Model.Extent;
            return extent.IsEmpty ? null : extent;
        }
    }
    
    /// <inheritdoc />
    public IMapStore? Store => _store;
    
    /// <inheritdoc />
    public IEnvironmentRepository Environment => _envRepo;
    
    /// <inheritdoc />
    public IMapRepository MapRepository => _mapRepo;
}