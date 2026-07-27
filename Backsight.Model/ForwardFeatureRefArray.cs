namespace Backsight.Model;

/// <summary>
/// A forward reference that relates to an array of items.
/// </summary>
class ForwardFeatureRefArray : ForwardRef
{
    /// <summary>
    /// The object that makes the forward-reference (not null).
    /// </summary>
    IFeatureRefArray ReferenceFrom { get; set; }

    ForwardRefArrayItem[] Items { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForwardFeatureRefArray"/> class.
    /// </summary>
    /// <param name="referenceFrom">The object that makes the forward-reference (not null).</param>
    /// <param name="field">The ID of the persistent array field.</param>
    /// <param name="items">The items that need to be resolved.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="referenceFrom"/> is not defined.</exception>
    internal ForwardFeatureRefArray(IFeatureRefArray referenceFrom, DataField field, ForwardRefArrayItem[] items)
        : base(field)
    {
        if (referenceFrom == null || items == null)
            throw new ArgumentNullException();

        if (items.Length == 0)
            throw new ArgumentException();

        ReferenceFrom = referenceFrom;
        Items = items;
    }

    internal override void Resolve(CadastralMapModel mapModel)
    {
        foreach (ForwardRefArrayItem item in Items)
        {
            item.Feature = mapModel.Find<Feature>(item.InternalId);
            if (item.Feature == null)
                throw new ApplicationException("Cannot locate forward reference " + item.InternalId);
        }

        ReferenceFrom.ApplyFeatureRefArray(Field, Items);
    }
}