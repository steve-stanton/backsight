using System.Collections.Generic;
using Backsight.Model;

namespace Backsight.Map.Editor;

/// <written by="Steve Stanton" on="05-FEB-2008" />
/// <summary>
/// Query spatial index to locate spatial objects that overlap a closed shape. For use
/// when selecting features.
/// </summary>
internal class FindOverlapsQuery
{
    /// <summary>
    /// The closed shape defining the search area.
    /// </summary>
    private readonly ClosedShape m_ClosedShape;

    /// <summary>
    /// The overlaps found so far.
    /// </summary>
    private readonly List<IMapObject> m_Result = new(100);

    /// <summary>
    /// Overlapping points
    /// </summary>
    private readonly List<PointFeature> m_Points = new(100);

    /// <summary>
    /// Creates a new <c>FindOverlapsQuery</c> (and executes it). The result of the query
    /// can then be obtained through the <c>Result</c> property.
    /// </summary>
    /// <param name="index">The spatial index to search</param>
    /// <param name="closedShape">The closed shape defining the search area.</param>
    /// <param name="spatialType">The type of objects to look for.</param>
    internal FindOverlapsQuery(IMapIndex index, IPosition[] closedShape, SpatialType spatialType)
    {
        m_ClosedShape = new ClosedShape(closedShape);

        // If we are looking for points or lines, locate points that overlap. Note that
        // if the user does not actually want points in the result, we still do a point
        // search, since it helps with the selection of lines.
        if (spatialType.HasFlag(SpatialType.Point) || spatialType.HasFlag(SpatialType.Line))
        {
            index.QueryWindow(m_ClosedShape.Extent, SpatialType.Point, OnPointFound);

            // Remember the points in the result if the caller wants them
            if (spatialType.HasFlag(SpatialType.Point))
                m_Result.AddRange([.. m_Points]);
        }

        // Find lines (this automatically includes lines connected to the points we just found)
        if (spatialType.HasFlag(SpatialType.Line))
            index.QueryWindow(m_ClosedShape.Extent, SpatialType.Line, OnLineFound);

        // Find any overlapping text
        if (spatialType.HasFlag(SpatialType.Text))
            index.QueryWindow(m_ClosedShape.Extent, SpatialType.Text, OnTextFound);

        m_Result.TrimExcess();
    }

    /// <summary>
    /// Delegate that's called whenever the index finds a point that's inside the query window
    /// </summary>
    /// <param name="item">The item to process (expected to be some sort of <c>PointFeature</c>)</param>
    /// <returns>True (always), indicating that the query should continue.</returns>
    private bool OnPointFound(IMapObject item)
    {
        if (item is PointFeature p && m_ClosedShape.IsOverlap(p))
            m_Points.Add(p);

        return true;
    }

    /// <summary>
    /// Delegate that's called whenever the index finds a line with a window that overlaps the query window
    /// </summary>
    /// <param name="item">The item to process (expected to be some sort of <c>LineFeature</c>)</param>
    /// <returns>True (always), indicating that the query should continue.</returns>
    private bool OnLineFound(IMapObject item)
    {
        if (item is not LineFeature line)
            return true;

        // If either line end point is in the list of selected points, just accept the line.
        // Otherwise check whether the closed shape overlaps (or intersects) the line.
        if (m_Points.Contains(line.StartPoint) ||
            m_Points.Contains(line.EndPoint) ||
            m_ClosedShape.IsOverlap(line.LineGeometry))
            m_Result.Add(line);

        return true;
    }

    /// <summary>
    /// Delegate that's called whenever the index finds text with a window that overlaps the query window
    /// </summary>
    /// <param name="item">The item to process (expected to be some sort of <c>TextFeature</c>)</param>
    /// <returns>True (always), indicating that the query should continue.</returns>
    private bool OnTextFound(IMapObject item)
    {
        if (item is not TextFeature text)
            return true;

        // Get the outline for the text
        IPosition[] outline = text.TextGeometry.Outline;

        // If any corner of the text outline is inside the closed shape (or any edge of
        // the outline intersects), remember the text in the results
        ClosedShape cs = new ClosedShape(outline);
        if (cs.IsOverlap(m_ClosedShape))
            m_Result.Add(text);

        return true;
    }

    /// <summary>
    /// The result of the query (may be an empty list).
    /// </summary>
    internal List<IMapObject> Result => m_Result;
}