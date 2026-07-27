using System.Diagnostics;

namespace Backsight.Model;

/// <written by="Steve Stanton" on="27-SEP-2007" />
/// <summary>
/// Query spatial index to obtain points coincident with a line.
/// </summary>
class FindPointsOnLineQuery
{
    /// <summary>
    /// The line of interest.
    /// </summary>
    private readonly LineGeometry m_Line;

    /// <summary>
    /// The search tolerance, in meters on the ground (expected to be greater than zero).
    /// </summary>
    private readonly double m_Tolerance;

    /// <summary>
    /// Should points that are coincident with the ends of the line be included in the results?
    /// </summary>
    private readonly bool m_WantEnds;

    /// <summary>
    /// The points found so far.
    /// </summary>
    private readonly List<PointFeature> m_Result = new();

    /// <summary>
    /// Creates a new <c>FindPointsOnLineQuery</c> (and executes it). The result of the query
    /// can then be obtained through the <c>Result</c> property.
    /// </summary>
    /// <param name="index">The spatial index to search</param>
    /// <param name="line">The line of interest.</param>
    /// <param name="wantEnds">Specify true if you want points coincident with the line ends.</param>
    /// <param name="tol">The search tolerance (expected to be greater than zero).</param>
    internal FindPointsOnLineQuery(IMapIndex index, LineGeometry line, bool wantEnds, ILength tol)
    {
        m_Line = line;
        m_Tolerance = tol.Meters;

        IWindow w = m_Line.Extent;
        index.QueryWindow(w, SpatialType.Point, OnQueryHit);
    }

    /// <summary>
    /// Delegate that's called whenever the index finds an object with an extent that
    /// overlaps the query window.
    /// </summary>
    /// <param name="item">The item to process (expected to be some sort of <c>PointFeature</c>)</param>
    /// <returns>True (always), indicating that the query should continue.</returns>
    private bool OnQueryHit(IMapObject item)
    {
        Debug.Assert(item is PointFeature);
        PointFeature p = (PointFeature)item;

        if (m_Line.Distance(p).Meters > m_Tolerance)
            return true;

        if (!m_WantEnds && (m_Line.Start.IsCoincident(p) || m_Line.End.IsCoincident(p)))
            return true;

        m_Result.Add(p);
        return true;
    }

    /// <summary>
    /// The result of the query (may be an empty list).
    /// </summary>
    internal List<PointFeature> Result => m_Result;
}