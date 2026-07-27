namespace Backsight.Model;

/// <summary>
/// Flag bits for the <c>Operation</c> class
/// </summary>
[Flags]
enum OperationFlag : byte
{
    /// <summary>
    /// Operation has been "touched" by a change. Set either by SetTouch(), or via Touch().
    /// </summary>
    Touched=0x02,

    /// <summary>
    /// Operation still needs to be calculated. Used to determine the
    /// order in which calls to <see cref="Operation.CalculateGeometry"/>
    /// should be made.
    /// </summary>
    ToCalculate=0x04,
}