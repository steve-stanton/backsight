namespace Backsight.Model.Index;

/// <written by="Steve Stanton" on="20-DEC-2006" />
/// <summary>
/// Some sort of positional range (where the range is expressed as an unsigned 64-bit integer).
/// </summary>
struct RangeValue
{
    /// <summary>
    /// The axis the range relates to.
    /// </summary>
    readonly Dimension m_Dimension;

    /// <summary>
    /// The low end of the range (inclusive).
    /// </summary>
    ulong m_Min;

    /// <summary>
    /// The high end of the range (inclusive).
    /// </summary>
    ulong m_Max;

    /// <summary>
    /// Creates a new <c>Range</c>
    /// </summary>
    /// <param name="d">The positional dimension the range refers to</param>
    /// <param name="a">One end of the range (either the min or the max)</param>
    /// <param name="b">The other end of the range (either the min or the max)</param>
    internal RangeValue(Dimension d, ulong a, ulong b)
    {
        m_Dimension = d;

        if (a<b)
        {
            m_Min = a;
            m_Max = b;
        }
        else
        {
            m_Min = b;
            m_Max = a;
        }
    }

    /// <summary>
    /// Creates a new <c>Range</c>, converting the supplied values into unsigned space.
    /// </summary>
    /// <param name="d">The positional dimension the range refers to</param>
    /// <param name="a">One end of the range (either the min or the max)</param>
    /// <param name="b">The other end of the range (either the min or the max)</param>
    internal RangeValue(Dimension d, long a, long b)
        : this(d, MapIndex.ToUnsigned(a), MapIndex.ToUnsigned(b))
    {
    }

    /// <summary>
    /// Checks whether a pair of ranges overlap
    /// </summary>
    /// <param name="that">The range to compare with this one</param>
    /// <returns>True if the ranges refer to the same positional dimension and they overlap or touch.
    /// False if they refer to different dimensions, or they don't overlap.
    /// </returns>
    internal bool IsOverlap(RangeValue that)
    {
        if (this.m_Dimension!=that.m_Dimension)
            return false;

        // The "range" may actually be a point
        if (that.m_Min==that.m_Max)
            return (this.m_Min<=that.m_Min && that.m_Max<=this.m_Max);

        return (Math.Max(this.m_Min, that.m_Min) <= Math.Min(this.m_Max, that.m_Max));
    }

    /// <summary>
    /// The axis the range relates to.
    /// </summary>
    internal Dimension Dimension => m_Dimension;

    /// <summary>
    /// Override displays the range as fixed-width hexadecimal.
    /// </summary>
    public override string ToString()
    {
        return $"[Min={m_Min:X016} Max={m_Max:X016}]";
    }

    /// <summary>
    /// The low end of the range (inclusive).
    /// </summary>
    internal ulong Min => m_Min;

    /// <summary>
    /// The high end of the range (inclusive).
    /// </summary>
    internal ulong Max => m_Max;

    /// <summary>
    /// The size of this range (inclusive of one end only).
    /// </summary>
    internal ulong Size => m_Max - m_Min;

    /// <summary>
    /// Increase both ends of this range by the specified amount.
    /// </summary>
    /// <param name="delta">The amount to add to both ends of the range</param>
    internal void Increase(ulong delta)
    {
        m_Min += delta;
        m_Max += delta;
    }
}