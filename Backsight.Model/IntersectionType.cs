namespace Backsight.Model;

/// <summary>
/// Flag bits denoting line-line intersection relationships.
/// </summary>
/// <see>IntersectionData</see>
[Flags]
enum IntersectionType : ushort
{
    TouchStart  = 0x0001,       // Touch at start of line
    TouchEnd    = 0x0002, 	    // Touch at end of line
    TouchOther  = 0x0004,       // Touch at some intermediate position
    GrazeStart  = 0x0010,       // Graze at start of line
    GrazeEnd    = 0x0020,       // Graze at end of line
    GrazeOther  = 0x0040,       // Graze along interior portion of line
    GrazeTotal  = 0x0080,       // Total graze
}