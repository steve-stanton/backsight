namespace Backsight.Model;

/// <written by="Steve Stanton" on="30-OCT-2007" />
/// <summary>
/// An <c>IDivider</c> is a line (or a portion of a line) that divides in two senses:
/// 1. It divides a pair of polygon rings.
/// 2. It is capable of dividing itself at a series of intersections.
/// <para/>
/// Implemented by <see cref="LineTopology"/> and <see cref="SectionTopology"/>
/// </summary>
/// <seealso cref="DividerObject"/>
public interface IDivider : IIntersectable, IExpandablePropertyItem
{
    /// <summary>
    /// The line the divider is associated with (the divider may cover only a portion
    /// of this line).
    /// </summary>
    LineFeature Line { get; }

    /// <summary>
    /// The start position for the divider.
    /// </summary>
    ITerminal From { get; }

    /// <summary>
    /// The end position for the divider.
    /// </summary>
    ITerminal To { get; }

    /// <summary>
    /// The polygon ring on the left of the divider (may be null)
    /// </summary>
    Ring? Left { get; set; }

    /// <summary>
    /// The polygon ring on the right of the divider (may be null)
    /// </summary>
    Ring? Right { get; set; }

    /// <summary>
    /// Does this divider represent some sort of overlap? If so, the
    /// divider is regarded only as a placeholder - the left and right polygon
    /// rings will be null (always), and will lead to an exception on an attempt
    /// to set them.
    /// </summary>
    bool IsOverlap { get; }

    /// <summary>
    /// Is this divider visible? (when dealing with trimmed dangles, dividers may
    /// be marked invisible).
    /// </summary>
    bool IsVisible { get; }
}