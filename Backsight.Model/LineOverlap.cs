namespace Backsight.Model;

/// <written by="Steve Stanton" on="29-OCT-2007" />
/// <summary>
/// Topology for a line that completely overlaps another line (or possibly several
/// lines). In these situations, an overlap object is created to ensure that some
/// sort of topology can be associated with the line. However, it does not know
/// about adjacent polygons (you get back nulls, and an attempt to set them will
/// lead to an exception).
/// <para/>
/// This class does not currently hold any information about the lines that are
/// overlapped. It is assumed that the necessary information can be readily obtained
/// by searching the map model for the overlaps.
/// </summary>
/// <seealso cref="LineDivider"/>
class LineOverlap : LineTopology
{
    /// <summary>
    /// Creates a new <c>LineOverlap</c> that relates to the specified line.
    /// </summary>
    /// <param name="line">The line the overlap topology is for (<b>not</b>
    /// the line that is overlapped)</param>
    internal LineOverlap(LineFeature line)
        : base(line)
    {
    }

    /// <summary>
    /// The polygon ring to the left of the line. This implementation returns null (always).
    /// An attempt to set the polygon ring will lead to an <c>InvalidOperationException</c>.
    /// </summary>
    public override Ring? Left // IDivider
    {
        get => null;
        set => throw new InvalidOperationException();
    }

    /// <summary>
    /// The polygon ring to the right of the line. This implementation returns null (always).
    /// An attempt to set the polygon ring will lead to an <c>InvalidOperationException</c>.
    /// </summary>
    public override Ring? Right // IDivider
    {
        get => null;
        set => throw new InvalidOperationException();
    }

    /// <summary>
    /// Returns <c>true</c> (always), indicating that this divider represents
    /// some sort of overlap.
    /// </summary>
    public override bool IsOverlap => true; // IDivider
}