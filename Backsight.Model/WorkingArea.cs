namespace Backsight.Model;

/// <written by="Steve Stanton" on="14-SEP-2006" />
/// <summary>The center and scale used for a map display</summary>
/// <param name="CenterX">The easting at the center of the map display.</param>
/// <param name="CenterY">The northing at the center of the map display.</param>
/// <param name="MapScale">The scale denominator for the map.</param>
public readonly record struct WorkingArea(double CenterX, double CenterY, double MapScale)
{
    internal WorkingArea(IWindow extent, double mapScale)
        : this(extent.Center.X, extent.Center.Y, mapScale)
    {
    }
}