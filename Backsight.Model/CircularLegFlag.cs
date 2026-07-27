namespace Backsight.Model;

/// <written by="Steve Stanton" on="21-JAN-1998"/>
/// <summary>
/// Flag bits relating to the <see cref="CircularLeg"/> class.
/// </summary>
[Flags]
enum CircularLegFlag : byte
{
    /// <summary>
    /// Leg is a cul-de-sac
    /// </summary>
    CulDeSac = 0x01,

    /// <summary>
    /// Two angles were specified
    /// </summary>
    TwoAngles = 0x02,

    /// <summary>
    /// Counter-clockwise arc
    /// </summary>
    CounterClockwise = 0x04,
}