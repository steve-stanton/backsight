using System.Diagnostics;

namespace Backsight.Model.Operations;

/// <summary>
/// Edit to subdivide a line at an intersection with another line.
/// </summary>
/// <remarks>
/// The ability to subdivide a line with a self-intersection (e.g. a multi-segment that turns
/// back on itself) is not currently supported.
/// </remarks>
public class LineSubdivisionAtIntersectionOperation : LineSubdivisionAtTerminal
{
    /// <summary>
    /// The intersecting line where the subdivision occurs.
    /// </summary>
    private readonly LineFeature m_IntersectingLine;

    /// <summary>
    /// The position ratio (on the subdivided line) of the intersection (as it was when
    /// the subdivision was originally performed).
    /// </summary>
    /// <remarks>
    /// Map updates can shift the geometry of any number of dependent map features.
    /// These shifts will generally be small, so it is reasonable to expect that the
    /// intersection of two lines will continue to exist following an update, but at
    /// a slightly different location.
    /// <para/>
    /// This is easily done when the lines involved are simple line segments. But
    /// when dealing with multi-segments or circular arcs, there may be more than one
    /// intersection. In that situation, the position ratio will be used to identify
    /// the closest intersection.
    /// <para/>
    /// It is also conceivable that there may be no intersection at all following an
    /// update. In that case, the position ratio alone will be used to establish the
    /// position of the split point. In that scenario, the result will be the same
    /// as that produced by the <see cref="AttachPointOperation"/>.
    /// </remarks>
    private readonly uint m_PositionRatio;

    /// <summary>
    /// The point feature created at the intersection.
    /// </summary>
    private PointFeature? m_NewPoint;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="LineSubdivisionAtIntersectionOperation"/> class
    /// when subdividing a line at an intersection with another line.
    /// </summary>
    /// <param name="line">The line to be subdivided.</param>
    /// <param name="intersectingLine">Another intersecting line.</param>
    /// <param name="positionRatio">The position ratio of the intersection point (with respect to
    /// the line that is being subdivided).</param>
    /// <exception cref="NotSupportedException">Thrown if the line to be subdivided is self-intersecting.</exception>
    public LineSubdivisionAtIntersectionOperation(
        LineFeature line,
        LineFeature intersectingLine,
        uint positionRatio) : base(line)
    {
        // This just adds a bit of complexity, but it should be possible with some extra checks
        if (ReferenceEquals(line, intersectingLine))
            throw new NotSupportedException("Subdivision of a self-intersecting line is not supported.");
        
        m_IntersectingLine = intersectingLine;
        m_PositionRatio = positionRatio;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LineSubdivisionAtIntersectionOperation"/> class
    /// using the data read from persistent storage.
    /// </summary>
    /// <param name="editDeserializer">The mechanism for reading back content.</param>
    public LineSubdivisionAtIntersectionOperation(EditDeserializer editDeserializer)
        : base(editDeserializer)
    {
        var stub = editDeserializer.ReadPersistent<FeatureStub>(DataField.NewPoint);
        m_NewPoint = new PointFeature(stub, null);
        MakeSections(editDeserializer, m_NewPoint);

        m_PositionRatio = editDeserializer.ReadUInt32(DataField.PositionRatio);
        m_IntersectingLine = editDeserializer.ReadFeatureRef<LineFeature>(DataField.IntersectingLine);
    }

    protected override PointFeature? SplitPoint => m_NewPoint;
    
    public override void WriteData(EditSerializer editSerializer)
    {
        base.WriteData(editSerializer);

        editSerializer.WriteUInt32(DataField.PositionRatio, m_PositionRatio);
        editSerializer.WriteFeatureRef(DataField.IntersectingLine, m_IntersectingLine);
    }

    /// <inheritdoc/>
    public override string Name => "Subdivide line at intersection";

    /// <inheritdoc/>
    internal override EditingActionId EditId => EditingActionId.LineSubdivisionAtIntersection;

    /// <inheritdoc/>
    protected override IEnumerable<Feature> ReferencedFeatures()
    {
        foreach (var feature in base.ReferencedFeatures())
            yield return feature;

        yield return m_IntersectingLine;
    }

    /// <inheritdoc/>
    protected override IEnumerable<Feature> CreatedFeatures()
    {
        foreach (var feature in base.CreatedFeatures())
            yield return feature;

        if (m_NewPoint is not null)
            yield return m_NewPoint;
    }

    /// <inheritdoc/>
    public override void Execute(FeatureFactory ff)
    {
        ff.AddFeatureDescription(DataField.NewPoint, new FeatureStub(this, MapStore.DefaultPointType, null));
        base.Execute(ff);
    }

    /// <inheritdoc/>
    internal override void ProcessFeatures(FeatureFactory ff)
    {
        m_NewPoint = ff.CreatePointFeature(DataField.NewPoint);
        base.ProcessFeatures(ff);
    }

    /// <inheritdoc/>
    internal override void CalculateGeometry(EditingContext ctx)
    {
        Debug.Assert(m_NewPoint is not null);
        
        IPosition p = Calculate();
        var pg = PointGeometry.Create(p);
        m_NewPoint.ApplyPointGeometry(ctx, pg);
    }

    private IPosition Calculate()
    {
        // Figure out the position of the intersection using the position ratio
        var defaultResult = CalculateUsingPositionRatio();
        
        // That's us done if it coincides with the intersecting line (which should be the case
        // unless an update subsequently shifted things.
        var d = m_IntersectingLine.Distance(defaultResult);
        if (d.Meters < Constants.XYTOL)
            return defaultResult;

        // Don't intersect a line with itself.
        if (ReferenceEquals(BaseLine, m_IntersectingLine))
            throw new ApplicationException("Cannot intersect a line with itself.");

        // Intersect this line with the other one.
        var xres = new IntersectionResult(m_IntersectingLine);
        uint nx = BaseLine.LineGeometry.Intersect(xres);
        
        // Simple case (and assume it's not a grazing intersection)
        if (nx == 1)
        {
            var result = xres.Intersections.Single().P1;
            if (result is not null)
                return result;
        }
        
        // If we aren't able to determine anything at all, we'll default to
        // a position defined by the position ratio
        if (nx <= 1)
            return defaultResult;
        
        // Identify the intersection that's closest to the position defined via the position ratio
        LineGeometry geom = BaseLine.LineGeometry;
        IPosition? closest = null;
        double delta = geom.GetLength(null).Meters;
        double defaultDelta = geom.GetLength(defaultResult).Meters;
        
        foreach (var intersection in xres.Intersections)
        {
            if (intersection.P1 is not null)
            {
                var xd = Math.Abs(geom.GetLength(intersection.P1).Meters - defaultDelta);
                if (xd < delta)
                {
                    closest = intersection.P1;
                    delta = xd;
                }
            }

            if (intersection.P2 is not null)
            {
                var xd = Math.Abs(geom.GetLength(intersection.P2).Meters - defaultDelta);
                if (xd < delta)
                {
                    closest = intersection.P2;
                    delta = xd;
                }
            }
        }

        return closest ?? defaultResult;
    }

    IPosition CalculateUsingPositionRatio()
    {
        // Get the distance to the attached point
        double dist = PositionRatio.GetDistance(BaseLine, m_PositionRatio);

        // Get the position for the point
        IPosition xpos;
        if (BaseLine.LineGeometry.GetPosition(new Length(dist), out xpos))
            return xpos;

        throw new Exception("Unable to calculate position of intersection point");
    }
}