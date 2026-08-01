namespace Backsight.Model;

/// <summary>
/// Something that keeps track of changes arising from edits.
/// Base class for <see cref="UpdateEditingContext"/> and <see cref="LoadingContext"/>
/// </summary>
public abstract class EditingContext
{
    /// <summary>
    /// Remembers a modification to the position of a point.
    /// </summary>
    /// <param name="point">The point that is about to be modified</param>
    internal abstract void RegisterChange(PointFeature p);
}