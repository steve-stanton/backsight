using System.Diagnostics;

namespace Backsight.Model.Operations;

/// <summary>
/// Edit to subdivide a line at a specific point.
/// </summary>
/// <seealso cref="LineSubdivisionAtIntersectionOperation"/>
/// <remarks>
/// This makes it possible to split a line at an arbitrary point along any line (such as a
/// point that was added using <see cref="AttachPointOperation"/>.
/// <para/>
/// As things currently stand, it can also be used to subdivide a line at a point that was
/// added by an edit such as <see cref="LineSubdivisionAtIntersectionOperation"/>. But be aware
/// that this will lead to strange results in a situation where the intersection ceases to
/// exist because of updates to the geometry of the lines involved. Whereas the operation
/// to add a point at an intersection can still produce a position even if the intersection
/// has been lost, the same is not true if the other line involved is also subdivided. What
/// will happen is that the point will be dragged away, because it will continue to fall on
/// the other line. Consequently, something like a straight line segment that has been split
/// will no longer be straight.
/// <para/>
/// To be able to resolve that, we could perhaps add a second point feature whenever a line
/// gets split at an intersection. However, this would lead to confusion in a situation where
/// other edits are dependent on the position of the point (which point is being referred to?).
/// Alternatively, it could be regarded as a lower-class of point feature that cannot be
/// referenced by anything else. And, if the intersection is subsequently lost, the update
/// could still move that point to a new location that is distinct from the original point.
/// </remarks>
public class LineSubdivisionAtPointOperation : LineSubdivisionAtTerminal
{
    /// <summary>
    /// The point where the line was subdivided (between the 2 new lines).
    /// </summary>
    private readonly PointFeature m_Point;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="LineSubdivisionAtPointOperation"/> class
    /// when subdividing a line at an existing point.
    /// </summary>
    /// <param name="line">The line to be subdivided.</param>
    /// <param name="point">The point where the line is to be subdivided.</param>
    /// <remarks>
    /// The specified point feature must have a position that exactly coincides with the line.
    /// The easiest way to ensure this is to use a point attached to the line using the
    /// <see cref="AttachPointOperation"/>.
    /// </remarks>
    public LineSubdivisionAtPointOperation(LineFeature line, PointFeature point)
        : base(line)
    {
        m_Point = point;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LineSubdivisionAtPointOperation"/> class
    /// using the data read from persistent storage.
    /// </summary>
    /// <param name="editDeserializer">The mechanism for reading back content.</param>
    public LineSubdivisionAtPointOperation(EditDeserializer editDeserializer)
        : base(editDeserializer)
    {
        m_Point = editDeserializer.ReadFeatureRef<PointFeature>(DataField.Point);
        MakeSections(editDeserializer, m_Point);
    }
    
    /// <inheritdoc/>
    protected override PointFeature? SplitPoint => m_Point;

    /// <inheritdoc/>
    public override EditingActionId EditId => EditingActionId.LineSubdivisionAtPoint;

    /// <inheritdoc/>
    public override string Name => "Subdivide line at point";

    /// <inheritdoc/>
    protected override IEnumerable<Feature> ReferencedFeatures()
    {
        foreach (var feature in base.ReferencedFeatures())
            yield return feature;

        yield return m_Point;
    }
}