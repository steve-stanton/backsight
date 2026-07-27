using System.Diagnostics;

namespace Backsight.Model;

/// <written by="Steve Stanton" on="27-SEP-2007" />
/// <summary>
/// Query spatial index to obtain a point that exactly coincides with a position.
/// </summary>
class FindPointQuery
{
    /// <summary>
    /// The position of interest.
    /// </summary>
    private readonly IPointGeometry m_Point;

    /// <summary>
    /// The points found (null if nothing found).
    /// </summary>
    private PointFeature? m_Result;

    /// <summary>
    /// Creates a new <c>FindPointQuery</c> (and executes it). The result of the query
    /// can then be obtained through the <c>Result</c> property.
    /// </summary>
    /// <param name="index">The spatial index to search</param>
    /// <param name="point">The position of interest</param>
    internal FindPointQuery(IMapIndex index, IPointGeometry p)
    {
        m_Point = p;
        m_Result = null;
        IWindow w = new Window(p, p);
        index.QueryWindow(w, SpatialType.Point, OnQueryHit);
    }

    /// <summary>
    /// Delegate that's called whenever the index finds an object with an extent that
    /// overlaps the query window.
    /// </summary>
    /// <param name="item">The item to process (expected to be some sort of <c>PointFeature</c>)</param>
    /// <returns>True if the query should continue. False if a coincident point has been found.</returns>
    private bool OnQueryHit(IMapObject item)
    {
        Debug.Assert(item is PointFeature);

        PointFeature p = (PointFeature)item;
        if (p.Geometry.IsCoincident(m_Point))
        {
            m_Result = p;
            return false;
        }

        return true;
    }

    /// <summary>
    /// The result of the query (null if a coincident point could not be found).
    /// </summary>
    internal PointFeature? Result => m_Result;
}