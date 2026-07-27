namespace Backsight.Model;

/// <summary>
/// Some sort of spatial object.
/// </summary>
public interface IMapObject
{
    /// <summary>
    /// Value denoting the spatial object type.
    /// </summary>
    SpatialType SpatialType { get; }

    /// <summary>
    /// The spatial extent of this object.
    /// </summary>
    IWindow Extent { get; }

    /// <summary>
    /// The shortest distance between this object and the specified position.
    /// </summary>
    /// <param name="point">The position of interest</param>
    /// <returns>The shortest distance between the specified position and this object</returns>
    ILength Distance(IPosition point);
}