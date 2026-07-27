namespace Backsight.Model;

/// <written by="Steve Stanton" on="29-OCT-2007" />
/// <summary>
/// Topology for a section of line.
/// Base class for <see cref="SectionDivider"/> and <see cref="SectionOverlap"/>.
/// </summary>
/// <seealso cref="LineTopology"/>
/// <seealso cref="SectionTopologyList"/>
abstract class SectionTopology : ISection, IDivider
{
    /// <summary>
    /// The line this topological section coincides with. The geometry for this feature
    /// may be an instance of <see cref="SectionGeometry"/> (consequently, the geometry
    /// for this <c>SectionTopology</c> object may be a section on a section).
    /// </summary>
    readonly LineFeature m_Line;

    /// <summary>
    /// The start position for the topological section.
    /// </summary>
    readonly ITerminal m_From;

    /// <summary>
    /// The end position for the topological section.
    /// </summary>
    readonly ITerminal m_To;

    /// <summary>
    /// Creates a new <c>SectionTopology</c>
    /// </summary>
    /// <param name="line">The line this topological section partially coincides with.</param>
    /// <param name="from">The start position for the topological section.</param>
    /// <param name="to">The end position for the topological section.</param>
    protected SectionTopology(LineFeature line, ITerminal from, ITerminal to)
    {
        m_Line = line;
        m_From = from;
        m_To = to;
    }

    /// <summary>
    /// The polygon ring to the left of this section of line (null if not yet determined).
    /// </summary>
    public abstract Ring? Left { get; set; } // IDivider

    /// <summary>
    /// The polygon ring to the right of this section of line (null if not yet determined).
    /// </summary>
    public abstract Ring? Right { get; set; } // IDivider

    /// <summary>
    /// The line the section partially coincides with.
    /// </summary>
    public LineFeature Line => m_Line;

    /// <summary>
    /// The start position for the section.
    /// </summary>
    public ITerminal From => m_From;

    /// <summary>
    /// The end position for the section.
    /// </summary>
    public ITerminal To => m_To;

    /// <summary>
    /// The geometry of the section of the line feature associated with this topology.
    /// </summary>
    // Note that the geometry associated with the boundary line may be an instance
    // of SectionGeometry (in that case, we need to return a section on a section).
    public LineGeometry LineGeometry => m_Line.LineGeometry.SectionBase.Section(this); // IIntersectable, IDivider

    /// <summary>
    /// Implements <see cref="IDivider"/> method by returning <c>false</c>,
    /// indicating that this topology is not involved in any sort of overlap.
    /// The <see cref="SectionOverlap"/> class overrides.
    /// </summary>
    public virtual bool IsOverlap => false;

    /// <summary>
    /// Implements <see cref="IDivider"/> method by returning <c>true</c>,
    /// indicating that this topology is visible.
    /// </summary>
    public virtual bool IsVisible => true;

    /// <summary>
    /// Override returns a string for use in debugging.
    /// </summary>
    /// <returns>A sting indicating the internal ID of the line involved, plus the two terminals.</returns>
    public override string ToString()
    {
        return $"Section on line {m_Line.InternalId} from {m_From}-{m_To}";
    }
}