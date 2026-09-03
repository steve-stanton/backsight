namespace Backsight.Model;

/// <summary>
/// Container for a collection of maps.
/// TODO: Consider this as the container for all maps, with a separate interface called IMapStorage for each map.
/// IMapStorage would specify access to the settings, saving changes... anything to do with a specific map.
/// Can IMapStore not already fill that role? An instance gets returned from a call to OpenMap. It should
/// hold a reference to the IMapRepository that it came from, and should already know its map name.
/// </summary>
public interface IMapRepository
{
    /// <summary>
    /// Retrieves the names of all maps in this repository.
    /// </summary>
    /// <returns>The names of all maps.</returns>
    IEnumerable<string> FindAllMapNames();

    /// <summary>
    /// Reads map preferences from a settings file (creates it if it doesn't already exist).
    /// </summary>
    /// <param name="mapName">The name of the map.</param>
    /// <returns>The map preferences</returns>
    MapSettings GetMapSettings(string mapName);
    
    /// <summary>
    /// Saves any changes to map preferences.
    /// </summary>
    /// <param name="mapName">The name of the map.</param>
    /// <param name="settings">The settings to be saved.</param>
    void SaveMapSettings(string mapName, MapSettings settings);

    /// <summary>
    /// Creates a new map. If this completes without any exception, you can
    /// call <see cref="OpenMap"/> to work with it.
    /// </summary>
    /// <param name="mapName">The user-perceived name for the map.</param>
    /// <param name="settings">The settings to be saved.</param>
    void CreateMap(string mapName, MapSettings settings);

    /// <summary>
    /// Checks whether a map name refers to an existing map.
    /// </summary>
    /// <param name="mapName">The name of the map to check.</param>
    /// <returns>True if the map appears to exist.</returns>
    bool CanOpen(string mapName);

    /// <summary>
    /// Opens an existing map.
    /// </summary>
    /// <param name="mapName">The name of the map to open.</param>
    /// <returns>The corresponding map data.</returns>
    /// <remarks>The map name may be case-sensitive, depending on the machine where the app is running.</remarks>
    IMapStore OpenMap(string mapName);
    
    /// <summary>
    /// Closes a map, discarding any unsaved changes.
    /// </summary>
    /// <param name="store">A map store previously returned by <see cref="OpenMap"/>.</param>
    void CloseMap(IMapStore store);
    
    /// <summary>
    /// Records a change to a map.
    /// </summary>
    /// <param name="mapName">The name of the map.</param>
    /// <param name="change">The change to record.</param>
    /// <param name="itemCount">The number of items involved in the change (at least 1).</param>
    /// <typeparam name="T">The type of change.</typeparam>
    /// <remarks>The change will remain pending until it is saved via a call to <see cref="IMapStore.SaveChanges"/>.</remarks>   
    void RecordChange<T>(string mapName, T change, uint itemCount) where T : Change;
    
    /// <summary>
    /// Removes a change to a map.
    /// </summary>
    /// <param name="mapName">The name of the map.</param>
    /// <param name="change">The change to remove.</param>
    /// <param name="itemCount">The number of items involved in the change (at least 1).</param>
    /// <typeparam name="T">The type of change.</typeparam>
    /// <returns>True if the change was removed. False if the change could not be found.</returns>
    bool RemoveChange<T>(string mapName, T change, uint itemCount) where T : Change;

    /// <summary>
    /// Deletes changes made to a map since the last savepoint.
    /// </summary>
    /// <param name="store">The store to modify.</param>
    /// <returns>The number of changes that were removed.</returns>
    uint RemoveChanges(IMapStore store);
}
