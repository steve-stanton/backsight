namespace Backsight.Model;

/// <written by="Steve Stanton" on="23-OCT-2007" />
/// <summary>
/// Dumb implementation of <see cref="ITerminal"/> that has no incident polygon dividers
/// (i.e. it always floats in space).
/// </summary>
/// <remarks>This class exists only because I need to detect intersections while a
/// new line is in the process of getting added. To do that using the <c>IntersectionFinder</c>
/// class, I need to pass in an instance of <c>LineGeometry</c>, and to create that, I need
/// two instances of <c>ITerminal</c>.</remarks>
public class FloatingTerminal : PointGeometry, ITerminal
{
    /// <summary>
    /// Creates a new <c>FloatingTerminal</c> at the specified position (rounded off to
    /// the nearest micron)
    /// </summary>
    /// <param name="p">The position of the terminal</param>
    public FloatingTerminal(IPosition p)
        : base(p)
    {
    }

    /// <summary>
    /// Creates a new <c>FloatingTerminal</c> at the specified position (rounded off to
    /// the nearest micron)
    /// </summary>
    /// <param name="x">The easting of the terminal, in meters on the ground.</param>
    /// <param name="y">The northing of the terminal, in meters on the ground.</param>
    internal FloatingTerminal(double x, double y)
        : base(x, y)
    {
    }

    /// <summary>
    /// Returns null, indicating that no polygon dividers start or end at this terminal.
    /// </summary>
    public IDivider[]? IncidentDividers() => null;
}