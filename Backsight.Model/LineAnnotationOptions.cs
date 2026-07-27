namespace Backsight.Model;

/// <summary>
/// Options relating to line annotation (as noted by <see cref="LineAnnotationStyle"/>)
/// </summary>
[Flags]
enum LineAnnotationOptions : byte
{
    /// <summary>
    /// Should the adjusted length of lines be displayed?
    /// (the units used for the display is determined via another display preference)
    /// </summary>
    ShowAdjustedLengths = 0x01,

    /// <summary>
    /// Should the observed length of lines be displayed?
    /// (the units used for the display is determined via another display preference)
    /// </summary>
    ShowObservedLengths = 0x02,

    /// <summary>
    /// Should observed angles be displayed?
    /// </summary>
    ShowObservedAngles = 0x04,
}