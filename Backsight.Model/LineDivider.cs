namespace Backsight.Model;

/// <written by="Steve Stanton" on="29-OCT-2007" />
/// <summary>
/// Topology for a line that separates a pair of polygons.
/// </summary>
/// <seealso cref="SectionDivider"/>
class LineDivider : LineTopology
{
    /// <summary>
    /// The polygon ring to the left of this divider.
    /// </summary>
    Ring? m_Left;

    /// <summary>
    /// The polygon ring to the right of this divider.
    /// </summary>
    Ring? m_Right;

    /// <summary>
    /// Creates a new <c>Divider</c> that relates to the specified line.
    /// </summary>
    /// <param name="line">The line the topology relates to.</param>
    internal LineDivider(LineFeature line)
        : base(line)
    {
        m_Left = m_Right = null;
    }

    public override Ring? Left // IDivider
    {
        get => m_Left;
        set => m_Left = value;
    }

    public override Ring? Right // IDivider
    {
        get => m_Right;
        set => m_Right = value;
    }
}