using System.Diagnostics;

namespace Backsight.Model.Operations;

/// <summary>
/// Edit to subdivide a line at a specific point.
/// </summary>
/// <seealso cref="LineSubdivisionAtIntersectionOperation"/>
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
    internal override EditingActionId EditId => EditingActionId.LineSubdivisionAtPoint;

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