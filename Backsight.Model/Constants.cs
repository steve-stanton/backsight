namespace Backsight.Model;

public class Constants : MathConstants
{
    /// <summary>
    /// Resolution of data (1 micron)
    /// </summary>
    public const double XYRES = 0.000001;

    /// <summary>
    /// Tolerance for intersections. This is established by supposing that any location has a
    /// circle of uncertainty that has a 1 micron radius. This means that a segment has a corridor
    /// of uncertainty that is up to 1.414 units wide. When dealing with 2 line segments, it may
    /// be possible that the uncertainty is compounded to give us 2.828 units (I'm no mathematician,
    /// maybe it doesn't). Then we add on a bit for luck to give us 3 microns.
    /// </summary>
    public const double XYTOL = 0.000003;

    /// <summary>
    /// The tolerance squared (handy during calculations)
    /// </summary>
    public const double XYTOLSQ = XYTOL*XYTOL;
}