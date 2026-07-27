namespace Backsight.Model.Operations;

/// <summary>
/// The changes to a specific edit (included as part of the <see cref="UpdateOperation"/> class).
/// </summary>
class RevisedEdit : IPersistent
{
    /// <summary>
    /// The edit being updated (not null).
    /// </summary>
    readonly Operation m_Edit;

    /// <summary>
    /// Information about the update (not null). 
    /// </summary>
    UpdateItemCollection m_Changes;

    /// <summary>
    /// Initializes a new instance of the <see cref="RevisedEdit"/> class.
    /// </summary>
    /// <param name="revisedEdit">The edit being updated (not null). Must implement
    /// <see cref="IRevisable"/>.</param>
    /// <exception cref="ArgumentNullException">If either <paramref name="edit"/> or
    /// <paramref name="changes"/> is null.</exception>
    /// <exception cref="ArgumentException">If <paramref name="revisedEdit"/> does not
    /// implement <see cref="IRevisable"/>.</exception>
    internal RevisedEdit(Operation revisedEdit, UpdateItemCollection changes)
        : base()
    {
        if (revisedEdit == null || changes == null)
            throw new ArgumentNullException();

        if (!(revisedEdit is IRevisable))
            throw new ArgumentException();

        m_Changes = changes;
        m_Edit = revisedEdit;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RevisedEdit"/> class
    /// using the data read from persistent storage.
    /// </summary>
    /// <param name="editDeserializer">The mechanism for reading back content.</param>
    internal RevisedEdit(EditDeserializer editDeserializer)
    {
        InternalIdValue id = editDeserializer.ReadInternalId(DataField.RevisedEdit);
        m_Edit = editDeserializer.MapModel.FindOperation(id);
        m_Changes = (m_Edit as IRevisable).ReadUpdateItems(editDeserializer);
    }

    /// <summary>
    /// Exchanges changes with the revised edit.
    /// </summary>
    internal void ApplyChanges()
    {
        ((IRevisable)m_Edit).ExchangeData(m_Changes);
    }

    /// <summary>
    /// The edit being updated (not null). Must implement <see cref="IRevisable"/>.
    /// </summary>
    internal Operation RevisedOperation => m_Edit;

    /// <summary>
    /// Information about the update (not null). 
    /// </summary>
    internal UpdateItemCollection Changes
    {
        get => m_Changes;
        set => m_Changes = value;
    }

    /// <summary>
    /// Writes the content of this instance to a persistent storage area.
    /// </summary>
    /// <param name="editSerializer">The mechanism for storing content.</param>
    public void WriteData(EditSerializer editSerializer)
    {
        editSerializer.WriteInternalId(DataField.RevisedEdit, new InternalIdValue(m_Edit.EditSequence));
        (m_Edit as IRevisable).WriteUpdateItems(editSerializer, m_Changes);
    }
}