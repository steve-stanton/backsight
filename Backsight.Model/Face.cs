namespace Backsight.Model;

/// <written by="Steve Stanton" on="16-MAY-1999" />
/// <summary>
///	A polygon ring divider that is associated with a facing direction. This is a transient
///	class that is utilized when a new polygon is being created.
/// </summary>
class Face : IEquatable<Face>
{
    /// <summary>
    /// What divider?
    /// </summary>
    readonly IDivider m_Divider;

    /// <summary>
    /// Is it facing left?
    /// </summary>
    readonly bool m_IsLeft;

    /// <summary>
    /// Creates a new <c>Face</c> for the specified divider.
    /// </summary>
    internal Face(IDivider d, bool isLeft)
    {
        m_Divider = d;
        m_IsLeft = isLeft;
    }

    /// <summary>
    /// The divider involved.
    /// </summary>
    internal IDivider Divider => m_Divider;

    /// <summary>
    /// Is the polygon involved to the left of the divider?
    /// </summary>
    internal bool IsLeft => m_IsLeft;

    public bool Equals(Face that)
    {
        return ReferenceEquals(this.m_Divider, that.m_Divider) && this.m_IsLeft == that.m_IsLeft;
    }
}