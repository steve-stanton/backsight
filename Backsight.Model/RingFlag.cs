namespace Backsight.Model;

/// <written by="Steve Stanton" on="20-JUL-1997" />
/// <summary>
/// Flag bits relating to a polygon ring.
/// </summary>
[Flags]
enum RingFlag : byte
{
    /// <summary>
    /// Ring is due for deletion.
    /// </summary>
    Deleted = 0x01,

    /// <summary>
    /// Island is floating
    /// </summary>
    Floating = 0x02,

    /// <summary>
    /// Ring to left of 1st arc (not used)
    /// </summary>
    Left = 0x04,

    /// <summary>
    /// Ring overlaps another ring in a theme
    /// </summary>
    /// <remarks>Hopefully obsolete</remarks>
    Overlap=0x08,

    /// <summary>
    /// Ring created system-generated lines in order to create itself.
    /// </summary>
    /// <remarks>Hopefully obsolete</remarks>
    LineOwner = 0x10,

    /// <summary>
    /// Ring has been spatially indexed.
    /// </summary>
    Indexed = 0x20,
}