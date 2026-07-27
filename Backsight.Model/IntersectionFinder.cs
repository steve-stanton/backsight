namespace Backsight.Model;

/// <written by="Steve Stanton" on="13-FEB-1998" was="CeXObject" />
/// <summary>
/// Detects intersections of a line with the map. The things that it
/// intersects are held in a series of <see cref="IntersectionResult"/> objects.
/// </summary>
class IntersectionFinder
{
    /// <summary>
    /// The thing being intersected
    /// </summary>
    IIntersectable m_Line;

    /// <summary>
    /// The things that are intersected
    /// </summary>
    List<IntersectionResult> m_Intersects;

    /// <summary>
    /// Default constructor.
    /// </summary>
    IntersectionFinder()
    {
        m_Line = null;
        m_Intersects = new List<IntersectionResult>();
    }

    /// <summary>
    /// Creates a new <c>IntersectionFinder</c> for the specified line feature.
    /// Use this constructor when intersecting something that has already been added to
    /// the map model. This ensures that the line is not intersected with itself.
    /// </summary>
    /// <param name="line">The line feature to intersect.</param>
    /// <param name="wantEndEnd">Specify true if you want end-to-end intersections in the results.</param>
    internal IntersectionFinder(LineFeature line, bool wantEndEnd)
    {
        m_Line = line;
        var index = line.MapModel.Index;
        m_Intersects = new FindIntersectionsQuery(index, line, wantEndEnd).Result;
    }

    /// <summary>
    /// Creates a new <c>IntersectionFinder</c> for the specified geometry.
    /// Use this constructor when intersecting geometry that has been created ad-hoc.
    /// </summary>
    /// <param name="index">The map index to use for intersection queries.</param>
    /// <param name="geom">The geometry to intersect.</param>
    /// <param name="wantEndEnd">Specify true if you want end-to-end intersections in the results.</param>
    internal IntersectionFinder(IMapIndex index, LineGeometry geom, bool wantEndEnd)
    {
        m_Line = geom;
        m_Intersects = new FindIntersectionsQuery(index, geom, wantEndEnd).Result;
    }

    internal uint Count => (uint)m_Intersects.Count;

    /// <summary>
    /// The list of things that <c>Geometry</c> intersects
    /// </summary>
    internal IList<IntersectionResult> Intersections => m_Intersects;

    /// <summary>
    /// The thing being intersected
    /// </summary>
    internal IIntersectable Intersector => m_Line;

    /// <summary>
    /// Appends intersection info to this object.
    /// </summary>
    /// <param name="xsect">The intersection info to append.</param>
    void Append(IntersectionResult xsect)
    {
        m_Intersects.Add(xsect);
    }

    /// <summary>
    /// Checks whether intersection results graze anything.
    /// </summary>
    internal bool IsGrazing
    {
        get
        {
            foreach(IntersectionResult r in m_Intersects)
            {
                if (r.IsGrazing)
                    return true;
            }

            return false;
        }
    }

    /// <summary>
    /// Checks whether any intersection refers to a position that requires a split
    /// on the primitive that THIS object refers to. This does not count the primitives
    /// that we actually intersected with.
    ///
    /// The result is true if a split is required. If this intersection object was for
    /// a point or a circle, the result will always be FALSE.
    /// </summary>
    internal bool IsSplitNeeded
    {
        get
        {
            // Go through each object we intersected with, looking for an intersection
            // that does not occur at the ends of the line primitive.

            foreach(IntersectionResult r in m_Intersects)
            {
                if (r.IsSplitOn(m_Line.LineGeometry))
                    return true;
            }

            return false;
        }
    }
}