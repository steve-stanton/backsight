namespace Backsight.Model;

/// <written by="Steve Stanton" on="04-JUN-2007" />
/// <summary>
/// Query spatial index to get circles close to a specific position.
/// </summary>
class FindCirclesQuery
{
    /// <summary>
    /// The search position.
    /// </summary>
    private readonly IPosition m_Position;

    /// <summary>
    /// The search tolerance, in meters on the ground (expected to be greater than zero).
    /// </summary>
    private readonly double m_Tolerance;

    /// <summary>
    /// The circles found so far (may be an empty list).
    /// </summary>
    private readonly List<Circle> m_Result = new();

    /// <summary>
    /// Creates a new <c>FindCirclesQuery</c> (and executes it). The result of the query
    /// can then be obtained through the <c>Result</c> property.
    /// </summary>
    /// <param name="index">The spatial index to search</param>
    /// <param name="p">The search position.</param>
    /// <param name="tol">The search tolerance (expected to be greater than zero).</param>
    internal FindCirclesQuery(EditingIndex index, IPosition p, ILength tol)
    {
        m_Position = p;
        m_Tolerance = tol.Meters;

        // The query will actually involve a square window, not a circle.
        IWindow x = new Window(p, m_Tolerance * 2.0);
        index.FindCircles(x, OnQueryHit);
    }

    /// <summary>
    /// Delegate that's called whenever the index finds a line with an extent that
    /// overlaps the query window.
    /// </summary>
    /// <param name="item">The item to process (expected to be some sort of <c>IFeature</c>)</param>
    /// <returns>True (always), meaning the query should continue.</returns>
    private bool OnQueryHit(IMapObject item)
    {
        if (item is Circle c)
        {
            // Confirm the circle is truly within tolerance
            double rad = c.Radius;
            double dist = BasicGeom.Distance(c.Center, m_Position);
            if (Math.Abs(rad-dist) < m_Tolerance)
                m_Result.Add(c);
        }

        return true;
    }

    /// <summary>
    /// The result of the query (may be an empty list).
    /// </summary>
    internal List<Circle> Result => m_Result;
}