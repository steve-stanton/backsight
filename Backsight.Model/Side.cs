namespace Backsight.Model;

/// <written by="Steve Stanton" on="23-AUG-2007" />
/// <summary>
/// Indicates the side of some object of interest with respect to something else (the "subject").
/// </summary>
enum Side
{
    /// <summary>
    /// The side for the object of interest has not been determined.
    /// </summary>
    Unknown=0,

    /// <summary>
    /// The object of interest is to the left of the subject.
    /// </summary>
    Left = 1,

    /// <summary>
    /// The object of interest is to the right of the subject.
    /// </summary>
    Right=2,

    /// <summary>
    /// The object of interest is coincident with the subject.
    /// </summary>
    On = 3
}