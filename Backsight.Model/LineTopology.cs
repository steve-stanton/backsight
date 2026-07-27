namespace Backsight.Model;

/// <written by="Steve Stanton" on="29-OCT-2007" />
/// <summary>
/// Topology that relates to a complete line.
/// Base class for <see cref="LineDivider"/> and <see cref="LineOverlap"/>.
/// </summary>
/// <seealso cref="SectionTopology"/>
abstract class LineTopology : Topology, IDivider
{
    /// <summary>
    /// Creates a new <c>LineTopology</c> that relates to a complete line.
    /// Base class for <see cref="LineDivider"/> and <see cref="LineOverlap"/>.
    /// </summary>
    /// <param name="line">The line the topology relates to.</param>
    /// <seealso cref="SectionTopology"/>
    protected LineTopology(LineFeature line)
        : base(line)
    {
    }

    /// <summary>
    /// The polygon ring to the left of the line (null if not yet determined).
    /// </summary>
    public abstract Ring? Left { get; set; } // IDivider

    /// <summary>
    /// The polygon ring to the right of the line (null if not yet determined).
    /// </summary>
    public abstract Ring? Right { get; set; } // IDivider

    /// <summary>
    /// The geometry of the line that is associated with this topology.
    /// </summary>
    public LineGeometry LineGeometry => Line.LineGeometry; // IIntersectable, IDivider

    /// <summary>
    /// The position of the start of this divider (coincides with the start
    /// of the associated line)
    /// </summary>
    public ITerminal From => Line.StartPoint;

    /// <summary>
    /// The position of the end of this divider (coincides with the end
    /// of the associated line)
    /// </summary>
    public ITerminal To => Line.EndPoint;

    /// <summary>
    /// The divider at the start of the associated line
    /// </summary>
    internal override IDivider FirstDivider => this;

    /// <summary>
    /// The divider at the end of the associated line.
    /// </summary>
    internal override IDivider LastDivider => this;

    /// <summary>
    /// Returns an enumerator that identifies this instance as the one and only divider
    /// in this topology.
    /// </summary>
    /// <returns>This</returns>
    public override IEnumerator<IDivider> GetEnumerator()
    {
        yield return this;
    }

    public override string ToString()
    {
        return $"Line={Line.InternalId} L={Left?.ToString() ?? "n/a"} R={Right?.ToString() ?? "n/a"}";
    }

    /// <summary>
    /// Implements <see cref="IDivider"/> method by returning <c>false</c>,
    /// indicating that this topology is not involved in any sort of overlap.
    /// The <see cref="LineOverlap"/> class overrides.
    /// </summary>
    public virtual bool IsOverlap => false;

    /// <summary>
    /// Implements <see cref="IDivider"/> method by returning <c>true</c>,
    /// indicating that this topology is visible.
    /// </summary>
    public bool IsVisible => true;

    /// <summary>
    /// Performs any processing when the line associated with this topology
    /// is being de-activated.  This should mark adjacent polygons for deletion, and
    /// remove line references from any intersections.
    /// </summary>
    internal override void OnLineDeactivation()
    {
        // Mark adjacent polygons for deletion
        MarkPolygons(this);

        // Don't need to do anything about intersections, since LineTopology relates
        // to a complete line, and complete lines must terminate on concrete PointFeature
        // objects.
    }
}