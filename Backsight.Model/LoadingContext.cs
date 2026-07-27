namespace Backsight.Model;

/// <summary>
/// Context indicating that the map model is being loaded.
/// </summary>
class LoadingContext : EditingContext
{
    /// <summary>
    /// Remembers a modification to the position of a point.
    /// </summary>
    /// <param name="p"></param>
    internal override void RegisterChange(PointFeature p)
    {
        // Do nothing
    }
}