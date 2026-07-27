namespace Backsight.Model;

/// <written by="Steve Stanton" on="25-OCT-2007" />
/// <summary>
/// Something that can be intersected.
/// </summary>
interface IIntersectable
{
    /// <summary>
    /// The geometry involved in the intersection calculation
    /// </summary>
    LineGeometry LineGeometry { get; }
}