using System.Diagnostics;

namespace Backsight.Model;

/// <written by="Steve Stanton" on="28-AUG-2007" />
/// <summary>
/// Query spatial index to obtain the polygon (if any) that encloses a point.
/// This class assumes that polygon topology is completely up to date.
/// </summary>
/// <seealso cref="FindIslandContainerQuery"/>
class FindPointContainerQuery
{
    /// <summary>
    /// The position you want the container for.
    /// </summary>
    readonly IPointGeometry m_Point;

    /// <summary>
    /// The enclosing polygon (null if nothing has been found).
    /// </summary>
    Polygon? m_Result;

    /// <summary>
    /// Candidates that we'll come back to.
    /// </summary>
    /// <remarks>
    /// Things like an unclosed street network can have MANY islands, and checking whether the position falls
    /// inside any of them is a bit laborious. If we hit this, we'll comes back to these candidates if we
    /// can't find an easy match.
    /// </remarks>
    List<Polygon>? m_Candidates;

    /// <summary>
    /// Creates a new <c>FindPointContainerQuery</c> (and executes it). The result of the query
    /// can then be obtained through the <c>Result</c> property.
    /// </summary>
    /// <param name="index">The spatial index to search</param>
    /// <param name="point">The position you want the container for</param>
    internal FindPointContainerQuery(IMapIndex index, IPointGeometry p)
    {
        m_Point = p;
        m_Result = null;
        IWindow w = new Window(p, p);
        index.QueryWindow(w, SpatialType.Polygon, OnQueryHit);

        // If we didn't get a result, but we skipped some candidates, check them now.
        if (m_Result is null && m_Candidates is not null)
        {
            // If NONE of the polygon's islands enclose the search position, that's
            // the result we want.

            foreach (Polygon cand in m_Candidates)
            {
                Debug.Assert(cand.HasAnyIslands);

                if (!cand.HasIslandEnclosing(m_Point))
                {
                    m_Result = cand;
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Delegate that's called whenever the index finds an object with an extent that
    /// overlaps the query window.
    /// </summary>
    /// <param name="item">The item to process (expected to be some sort of <c>Ring</c>)</param>
    /// <returns>True if the query should continue. False if the enclosing polygon has been found.</returns>
    private bool OnQueryHit(IMapObject item)
    {
        // We're only interested in real polygons (not islands)
        if (item is not Polygon p)
            return true;

        // The window of the polygon has to overlap.
        if (!p.Extent.IsOverlap(m_Point))
            return true;

        // Skip if it doesn't enclose the search position
        if (!p.IsRingEnclosing(m_Point))
            return true;

        // If the polygon contains any islands, remember the polygon
        // for a further look.

        if (p.HasAnyIslands)
        {
            if (m_Candidates is null)
                m_Candidates = new List<Polygon>(1);

            m_Candidates.Add(p);
            return true;
        }

        m_Result = p;
        return false;
    }

    /// <summary>
    /// The result of the query (null if no features were found within the query region).
    /// </summary>
    internal Polygon? Result => m_Result;
}