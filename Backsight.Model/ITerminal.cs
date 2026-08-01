namespace Backsight.Model;

/// <written by="Steve Stanton" on="05-JUL-2007" />
/// <summary>
/// A position at one end of a polygon divider. Implemented by
/// the <see cref="PointFeature"/> and <see cref="Intersection"/> classes.
/// </summary>
public interface ITerminal : IPointGeometry
{
    /// <summary>
    /// The dividers that start or end at the terminal. If a divider
    /// starts and also ends at the terminal, it should appear in the
    /// returned array just once.
    /// </summary>
    IDivider[]? IncidentDividers();
}