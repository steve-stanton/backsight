namespace Backsight.Model;

/// <summary>
/// Something that makes a reference to a spatial feature.
/// </summary>
/// <remarks>This interface was created to handle forward-references that might be
/// encountered when loading data that originates from the old CEdit system.</remarks>
interface IFeatureRef
{
    /// <summary>
    /// Ensures that a persistent field has been associated with a spatial feature.
    /// </summary>
    /// <param name="field">A tag associated with the item</param>
    /// <param name="feature">The feature to assign to the field (not null).</param>
    /// <returns>True if a matching field was processed. False if the field is not known to this
    /// class (may be known to another class in the type hierarchy).</returns>
    bool ApplyFeatureRef(DataField field, Feature feature);
}