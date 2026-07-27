using System.Diagnostics;

namespace Backsight.Model;

/// <written by="Steve Stanton" on="08-SEP-2007" />
/// <summary>
/// Query spatial index to allow for cleanup after any sort of edit.
/// </summary>
class CleanupQuery
{
    /// <summary>
    /// The model that's being cleaned.
    /// </summary>
    readonly CadastralMapModel m_Model;

    /// <summary>
    /// Features that have been moved (and which have not been marked for deletion).
    /// </summary>
    readonly List<Feature> m_Moves;

    /// <summary>
    /// Items that have been deleted (either instances of <see cref="Feature"/> or <see cref="Ring"/>).
    /// </summary>
    readonly List<IMapObject> m_Deletions;

    /// <summary>
    /// Window corresponding to data that has been marked for deletion.
    /// </summary>
    readonly Window m_UpdateWindow;

    /// <summary>
    /// Creates a new <c>CleanupQuery</c> and executes it.
    /// </summary>
    /// <param name="model">The model to clean</param>
    internal CleanupQuery(CadastralMapModel model)
    {
        m_Model = model ?? throw new ArgumentNullException();
        m_UpdateWindow = new Window();
        m_Deletions = new List<IMapObject>(100);
        m_Moves = new List<Feature>(100);

        EditingIndex index = model.EditingIndex;

        // Cleanup features
        index.QueryWindow(null, SpatialType.Feature, CleanupFeature);

        // Cleanup polygons
        index.QueryWindow(null, SpatialType.Polygon, CleanupPolygon);

        // Remove stuff from spatial index if it's been deleted
        foreach (IMapObject o in m_Deletions)
        {
            m_UpdateWindow.Union(o.Extent);

            if (o is Feature)
                index.RemoveFeature((Feature)o);
            else if (o is Ring)
                index.Remove(o);
            else
                throw new ApplicationException("Unexpected data type: " + o.GetType().Name);
        }
    }

    /// <summary>
    /// Features that have been moved.
    /// </summary>
    internal List<Feature> Moves => m_Moves;

    /// <summary>
    /// Delegate that's called whenever the index finds a feature
    /// </summary>
    /// <param name="item">The item to process</param>
    /// <returns>True (always), indicating that the query should continue.</returns>
    private bool CleanupFeature(IMapObject item)
    {
        Debug.Assert(item is Feature);
        Feature f = (Feature)item;

        if (f.IsInactive)
            m_Deletions.Add(f);
        else if (f.IsMoved)
            m_Moves.Add(f);

        f.Clean();
        return true;
    }

    /// <summary>
    /// Delegate that's called whenever the index finds a polygon
    /// </summary>
    /// <param name="item">The item to process</param>
    /// <returns>True (always), indicating that the query should continue.</returns>
    private bool CleanupPolygon(IMapObject item)
    {
        Debug.Assert(item is Ring);
        Ring r = (Ring)item;

        if (r.IsDeleted)
            m_Deletions.Add(r);

        r.Clean();
        return true;
    }
}