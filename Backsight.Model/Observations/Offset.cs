namespace Backsight.Model.Observations;

/// <written by="Steve Stanton" on="13-NOV-1997" />
/// <summary>
/// An offset with respect to something else. This is the base class for
/// <see cref="OffsetDistance"/> and <see cref="OffsetPoint"/>.
/// </summary>
abstract class Offset : Observation
{
    /// <summary>
    /// Returns the offset distance with respect to a reference direction, in meters
    /// on the ground. Offsets to the left are returned as a negated value, while
    /// offsets to the right are positive values.
    /// </summary>
    /// <param name="dir">The direction that the offset was observed with respect to.</param>
    /// <returns>The signed offset distance, in meters on the ground</returns>
    internal abstract double GetMetric(Direction dir);

    /// <summary>
    /// The offset point (if this is an instance of <see cref="OffsetPoint"/>), or
    /// null for any other type of offset.
    /// </summary>
    internal abstract PointFeature Point { get; }

    /// <summary>
    /// Cuts references to an operation that are made by any features this offset refers to.
    /// </summary>
    /// <param name="op">The operation that should no longer be referred to.</param>
    internal abstract void CutRef(Operation op);
}