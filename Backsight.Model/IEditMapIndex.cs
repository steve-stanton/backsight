namespace Backsight.Model;

/// <summary>
/// A spatial index that can be edited.
/// </summary>
public interface IEditMapIndex : IMapIndex
{
    /// <summary>
    /// Adds a spatial object into the index
    /// </summary>
    /// <param name="o">The object to add to the index</param>
    void Add(IMapObject o);

    /// <summary>
    /// Removes a spatial object from the index
    /// </summary>
    /// <param name="o">The object to remove from the index</param>
    /// <returns>True if object removed. False if it couldn't be found.</returns>
    bool Remove(IMapObject o);
}