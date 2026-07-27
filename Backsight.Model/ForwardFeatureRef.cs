namespace Backsight.Model;

/// <summary>
/// A reference to a feature that has not yet been created. This is utilized by code
/// that handles the forward-references that might be encountered when loading data
/// originating from the old CEdit system.
/// </summary>
class ForwardFeatureRef : ForwardRef
{
    /// <summary>
    /// The object that makes the forward-reference (not null).
    /// </summary>
    internal IFeatureRef ReferenceFrom { get; private set; }

    /// <summary>
    /// The internal ID that has been persisted for the field (relating to a feature
    /// that has not been created yet).
    /// </summary>
    internal InternalIdValue InternalId { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForwardFeatureRef"/> class.
    /// </summary>
    /// <param name="referenceFrom">The object that makes the forward-reference (not null).</param>
    /// <param name="field">The ID of the persistent field.</param>
    /// <param name="iid">The internal ID that has been persisted for the field (relating to a feature
    /// that has not been created yet).</param>
    /// <exception cref="ArgumentNullException">If <paramref name="referenceFrom"/> is not defined.</exception>
    internal ForwardFeatureRef(IFeatureRef referenceFrom, DataField field, InternalIdValue iid)
        : base(field)
    {
        if (referenceFrom == null)
            throw new ArgumentNullException();

        ReferenceFrom = referenceFrom;
        InternalId = iid;
    }

    /// <summary>
    /// Attempts to resolves this forward reference.
    /// </summary>
    /// <param name="mapModel">The map model that should now contain the relevant features.</param>
    internal override void Resolve(CadastralMapModel mapModel)
    {
        Feature f = mapModel.Find<Feature>(InternalId);
        if (f == null)
            throw new ApplicationException("Cannot locate forward reference " + InternalId);

        ReferenceFrom.ApplyFeatureRef(Field, f);
    }
}