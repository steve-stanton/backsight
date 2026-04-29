namespace Backsight;

/// <summary>
/// A keyboard selection. 
/// </summary>
/// <param name="KeyValue"></param>
/// <param name="Alt"></param>
/// <param name="Control"></param>
/// <param name="Shift"></param>
/// <remarks>This is intended as a substitute for <see cref="System.Windows.Forms.KeyEventArgs"/></remarks>
public readonly record struct KeySelection(
    int KeyValue,
    bool Alt,
    bool Control,
    bool Shift);