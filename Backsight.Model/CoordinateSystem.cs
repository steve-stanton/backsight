namespace Backsight.Model;

/// <summary>
/// Access to the coordinate system used for maps.
/// </summary>
static class CoordinateSystem
{
    internal static readonly ISpatialSystem DefaultSystem = new UtmSystem();
    
    /// <summary>
    /// Calculates a scale factor (multiplier) that may be applied to ground distances,
    /// to reduce them to the mapping projection.
    /// </summary>
    /// <param name="start">The starting XY position</param>
    /// <param name="end">The terminating XY position</param>
    /// <returns>The scale multiplier for converting ground distances</returns>
    internal static double GetLineScaleFactor(IPosition start, IPosition end)
    {
        return DefaultSystem.GetLineScaleFactor(start, end);
    }
}