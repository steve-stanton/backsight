namespace Backsight.Model;

/// <written by="Steve Stanton" on="30-AUG-2007" />
/// <summary>
/// Defines the terminal positions for a section of a line.
/// </summary>
interface ISection
{
    /// <summary>
    /// The start position for the section.
    /// </summary>
    ITerminal From { get; }

    /// <summary>
    /// The end position for the section.
    /// </summary>
    ITerminal To { get; }
}