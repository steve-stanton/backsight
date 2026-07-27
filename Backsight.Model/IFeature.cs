using Backsight.Environment;

namespace Backsight.Model;

/// <summary>
/// Basic information about a feature
/// </summary>
/// <remarks>
/// Implemented by <see cref="Feature"/> and <see cref="FeatureStub"/> 
/// </remarks>
interface IFeature
{
    /// <summary>
    /// The editing operation that created the feature (never null).
    /// </summary>
    Operation Creator { get; }

    /// <summary>
    /// The internal ID of this feature (holds the 1-based creation sequence
    /// of this feature within the project that created it).
    /// </summary>
    InternalIdValue InternalId { get; }

    /// <summary>
    /// The type of real-world object that the feature corresponds to.
    /// </summary>
    IEntity EntityType { get; }

    /// <summary>
    /// The user-perceived ID (if any) for the feature. This is the ID that
    /// is used to associate the feature with any miscellaneous attributes
    /// that may be held in a database.
    /// </summary>
    FeatureId FeatureId { get; }
}