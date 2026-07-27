namespace Backsight.Model;

/// <summary>
/// A reference to something that has not yet been created. This is utilized by code
/// that handles the forward-references that might be encountered when loading data
/// originating from the old CEdit system.
/// </summary>
/// <remarks>This is the base class for <see cref="ForwardFeatureRef"/> and <see cref="ForwardFeatureRefArray"/></remarks>
abstract class ForwardRef
{
    /// <summary>
    /// The ID of the persistent field.
    /// </summary>
    internal DataField Field { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForwardFeatureRef"/> class.
    /// </summary>
    /// <param name="field">The ID of the persistent field.</param>
    internal ForwardRef(DataField field)
    {
        Field = field;
    }

    /// <summary>
    /// Attempts to resolves this forward reference.
    /// </summary>
    /// <param name="mapModel">The map model that should now contain the relevant features.</param>
    internal abstract void Resolve(CadastralMapModel mapModel);
}