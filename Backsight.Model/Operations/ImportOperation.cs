namespace Backsight.Model.Operations;

/// <summary>
/// Editing operation that transfers data from a <see cref="FileImportSource"/> to
/// the current map model.
/// </summary>
class ImportOperation : Operation
{
    /// <summary>
    /// A name that identifies the source of the data (e.g. a file name).
    /// </summary>
    string m_Source;

    /// <summary>
    /// The features that were imported.
    /// </summary>
    Feature[] m_Data;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImportOperation"/> class.
    /// </summary>
    /// <param name="store">The store this operation is part of.</param>
    internal ImportOperation(IMapStore store)
        : base(store)
    {
        m_Source = String.Empty;
        m_Data = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ImportOperation"/> class
    /// using the data read from persistent storage.
    /// </summary>
    /// <param name="editDeserializer">The mechanism for reading back content.</param>
    internal ImportOperation(EditDeserializer editDeserializer)
        : base(editDeserializer)
    {
        if (editDeserializer.IsNextField(DataField.Source))
            m_Source = editDeserializer.ReadString(DataField.Source);
        else
            m_Source = String.Empty;

        m_Data = editDeserializer.ReadPersistentArray<Feature>(DataField.Features);
    }

    /// <summary>
    /// Executes this import by loading data from the supplied source.
    /// </summary>
    /// <param name="source">The data source to use for the import</param>
    internal void Execute(FileImportSource source)
    {
        m_Source = source.FileName;
        m_Data = source.Load(this);
        Complete();
    }

    /// <summary>
    /// The features created by this editing operation (may be an empty array, but
    /// never null).
    /// </summary>
    internal override Feature[] Features => m_Data;

    /// <summary>
    /// The unique identifier for this edit.
    /// </summary>
    internal override EditingActionId EditId => EditingActionId.DataImport;

    /// <summary>
    /// A user-perceived title for this operation.
    /// </summary>
    public override string Name => "Data import";

    /// <summary>
    /// Rollback this operation (occurs when a user undoes the last edit).
    /// </summary>
    internal override void Undo()
    {
        base.OnRollback();

        // Get rid of the features that were created.
        foreach (Feature f in m_Data)
            Rollback(f);
    }

    /// <summary>
    /// Attempts to locate a superseded (inactive) line that was the parent of
    /// a specific line.
    /// </summary>
    /// <param name="line">The line of interest</param>
    /// <returns>Null (always), since this edit doesn't supersede any lines.</returns>
    internal override LineFeature GetPredecessor(LineFeature line)
    {
        return null;
    }

    /// <summary>
    /// Obtains the features that are referenced by this operation (including features
    /// that are indirectly referenced by observation classes).
    /// </summary>
    /// <returns>
    /// The referenced features (never null, but may be an empty array).
    /// </returns>
    public override Feature[] GetRequiredFeatures()
    {
        return new Feature[0];
    }

    /// <summary>
    /// Writes the content of this instance to a persistent storage area.
    /// </summary>
    /// <param name="editSerializer">The mechanism for storing content.</param>
    public override void WriteData(EditSerializer editSerializer)
    {
        base.WriteData(editSerializer);
        editSerializer.WriteString(DataField.Source, m_Source);
        editSerializer.WritePersistentArray<Feature>(DataField.Features, this.Features);
    }
}