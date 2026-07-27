namespace Backsight.Model;

/// <summary>
/// Something that processes an item in the index (for use with implementations
/// of the <c>IMapIndex.QueryWindow</c> method).
/// </summary>
/// <param name="item">An object associated with the spatial index</param>
/// <returns>True if the query should be continued. False if the query should be
/// terminated (e.g. a result may have been obtained).</returns>
public delegate bool ProcessItem(IMapObject item);

/// <summary>
/// Retrieval from a spatial index.
/// </summary>
public interface IMapIndex
{
    /// <summary>
    /// Locates the feature closest to a specific position. Ignores polygons.
    /// </summary>
    /// <param name="p">The search position</param>
    /// <param name="radius">The search radius</param>
    /// <param name="types">The type(s) of object to look for (if you include polygons as
    /// an applicable type, they will be quietly ignored).</param>
    /// <returns>The closest feature of the requested type (null if nothing found)</returns>
    IMapObject? QueryClosest(IPosition p, ILength radius, SpatialType types);

    /// <summary>
    /// Process items with a covering rectangle that overlaps a query window.
    /// </summary>
    /// <param name="extent">The extent of the query window (null for everything).</param>
    /// <param name="types">The type(s) of object to look for</param>
    /// <param name="itemHandler">The method that should be called for each query hit. A hit
    /// is defined as anything with a covering rectangle that overlaps the query window (this
    /// does not mean the hit actually intersects the window).</param>
    void QueryWindow(IWindow? extent, SpatialType types, ProcessItem itemHandler);
}