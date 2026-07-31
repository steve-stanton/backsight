namespace Backsight.Model;

/// <written by="Steve Stanton" on="12-JUN-07" />
/// <summary>
/// Something that is dependent on the position of instance(s) of <c>Feature</c>.
/// Each feature involved should be cross-referenced to the dependent feature.
/// </summary>
interface IFeatureDependent
{
    /// <summary>
    /// Performs any processing that needs to be done just before the position of
    /// a referenced feature is changed.
    /// </summary>
    /// <param name="f">The feature that is about to be moved  - something that
    /// the <c>IFeatureDependent</c> is dependent on (not null).</param>
    /// <param name="context">The context in which the move is being made (not null).</param>
    void OnFeatureMoving(Feature f, UpdateEditingContext context);

    /// <summary>
    /// Obtains referenced features where position is required by this dependent.
    /// </summary>
    /// <returns>The referenced features (never null, but may be an empty array).</returns>
    /// <remarks>
    /// Re-consider the method name. Some edits refer to features, but those references
    /// have no bearing on the creation of any new features (e.g. MovePolygonPositionOperation).
    /// The relevance in terms of new features is what's really important.
    /// </remarks>
    Feature[] GetRequiredFeatures();

    /// <summary>
    /// The edit that created this dependent.
    /// </summary>
    Operation Creator { get; }
}