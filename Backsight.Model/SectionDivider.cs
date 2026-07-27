namespace Backsight.Model;

/// <written by="Steve Stanton" on="29-OCT-2007" />
/// <summary>
/// Topology for a line section that seperates a pair of polygons.
/// </summary>
/// <seealso cref="LineDivider"/>
class SectionDivider : SectionTopology
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
    /// Creates a new <c>SectionDivider</c> that relates to the specified section of line,
    /// with undefined polygon rings on left and right.
    /// </summary>
    /// <param name="line">The line this topological section partially coincides with.</param>
    /// <param name="from">The start position for the topological section.</param>
    /// <param name="to">The end position for the topological section.</param>
    internal SectionDivider(LineFeature line, ITerminal from, ITerminal to)
        : base(line, from, to)
    {
        m_Left = m_Right = null;

        if (from is Intersection fi)
            fi.Add(line);
        else if (from is PointFeature fp)
            fp.AddReference(line);

        if (to is Intersection ti)
            ti.Add(line);
        else if (to is PointFeature tp)
            tp.AddReference(line);
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

    /// <summary>
    /// Implements <see cref="IDivider"/> method by returning <c>false</c> if this divider
    /// coincides with the trimmed portion of a line.
    /// </summary>
    public override bool IsVisible => Line.IsVisible(this);
}