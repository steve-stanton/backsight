namespace Backsight.Model;

/// <summary>
/// The various sorts of items that may appear in a path description.
/// </summary>
enum PathItemType
{
    /// <summary>
    /// Undefined type
    /// </summary>
    Null = 0,

    /// <summary>
    /// Some sort of angle
    /// </summary>
    Angle = 1,

    /// <summary>
    /// Angle at BC
    /// </summary>
    BcAngle = 10,

    /// <summary>
    /// BeginningOfCurve
    /// </summary>
    BC = 20,

    /// <summary>
    /// Central angle
    /// </summary>
    CentralAngle = 30,

    /// <summary>
    /// Counter-clockwise arc indicator
    /// </summary>
    CounterClockwise = 40,

    /// <summary>
    /// Deflection angle
    /// </summary>
    Deflection = 45,

    /// <summary>
    /// Distance value
    /// </summary>
    Distance = 50,

    /// <summary>
    /// Angle at EC
    /// </summary>
    EcAngle = 60,

    /// <summary>
    /// EndOfCurve
    /// </summary>
    EC = 70,
		
    /// <summary>
    /// Miss-connect (no line)
    /// </summary>
    MissConnect = 80,

    /// <summary>
    /// Omit point (no line && no point)
    /// </summary>
    OmitPoint = 90,

    /// <summary>
    /// Radius for circular arc
    /// </summary>
    Radius = 100,

    /// <summary>
    /// Free-standing slash character
    /// </summary>
    Slash = 110,

    /// <summary>
    /// New default units for what follows
    /// </summary>
    Units = 120,

    /// <summary>
    /// Some sort of floating point value
    /// </summary>
    Value = 130,
}