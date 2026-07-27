namespace Backsight.Model;

/// <summary>
/// Flag bits that apply to each distance along a <see cref="Leg"/>
/// in a connection path.
/// </summary>
/// <seealso cref="PathOperation"/>
[Flags]
enum LegItemFlag : byte
{
    /// <summary>
    /// No flag
    /// </summary>
    Null = 0x00,

    /// <summary>
    /// Miss-connect (no line)
    /// </summary>
    MissConnect = 0x01,

    /// <summary>
    /// Omit point (no line, no point)
    /// </summary>
    OmitPoint = 0x02,

    /// <summary>
    /// Angle at start of a straight leg is a deflection.
    /// This switch will be set ONLY for the first span in a straight leg.
    /// </summary>
    //Deflection = 0x04,

    /// <summary>
    /// Miss-connect should be replaced with a new line upon rollforward.
    /// </summary>
    NewLine = 0x08,

    /// <summary>
    /// Leg is staggered, and this is the first face. 
    /// This switch will be set ONLY for the first span in a leg.
    /// </summary>
    //Face1 = 0x10,

    /// <summary>
    /// Leg is staggered, and this is the second face (the leg follows
    /// the first face in the enclosing <c>PathOperation</c> object).
    /// This switch will be set ONLY for the first span in a leg.
    /// </summary>
    //Face2 = 0x20,
}