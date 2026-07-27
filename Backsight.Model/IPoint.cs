namespace Backsight.Model;

/// <written by="Steve Stanton" on="22-JUN-07" />
/// <summary>
/// A spatial object that refers to a point in space.
/// </summary>
public interface IPoint : IMapObject
{
    /// <summary>
    /// The geometry for this point.
    /// </summary>
    IPointGeometry Geometry { get; }
}