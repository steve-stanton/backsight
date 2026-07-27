namespace Backsight.Model;

/// <written by="Steve Stanton" on="26-OCT-2007" />
/// <summary>
/// Flag bits relating to an <see cref="Intersection"/>
/// </summary>
[Flags]
enum IntersectionFlag : byte
{
    /// <summary>
    /// Intersection has been spatially indexed.
    /// </summary>
    Indexed = 0x01,
}