namespace Backsight.Model.Operations;

/// <written by="Bob Bruce" on="29-JUN-1998" was="CeSetLabelRotation" />
/// <summary>
/// Edit to define the default angle for subsequently added text.
/// </summary>
class TextRotationOperation : Operation
{
    /// <summary>
    /// The new default rotation (as a clockwise rotation from horizontal axis (+X), in radians)
    /// </summary>
    double m_Rotation;

    /// <summary>
    /// The previous rotation (used for undo)
    /// </summary>
    double m_PrevRotation;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextRotationOperation"/> class
    /// </summary>
    /// <param name="store">The map store this operation is part of.</param>
    internal TextRotationOperation(IMapStore store)
        : base(store)
    {
        m_Rotation = m_PrevRotation = 0.0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TextRotationOperation"/> class
    /// using the data read from persistent storage.
    /// </summary>
    /// <param name="editDeserializer">The mechanism for reading back content.</param>
    internal TextRotationOperation(EditDeserializer editDeserializer)
        : base(editDeserializer)
    {
        RadianValue value = editDeserializer.ReadRadians(DataField.Value);
        m_Rotation = value.Radians;
    }

    /// <summary>
    /// A user-perceived title for this operation.
    /// </summary>
    public override string Name => "Set rotation angle for text";

    /// <summary>
    /// The features created by this editing operation.
    /// </summary>
    internal override Feature[] Features => new Feature[0];

    /// <summary>
    /// The unique identifier for this edit.
    /// </summary>
    internal override EditingActionId EditId => EditingActionId.SetLabelRotation;

    /// <summary>
    /// Rollback this operation (occurs when a user undoes the last edit).
    /// </summary>
    internal override void Undo()
    {
        MapModel.DefaultTextRotation = m_PrevRotation;
    }

    /// <summary>
    /// Executes this edit
    /// </summary>
    /// <param name="p1">The first point to be used in orienting the labels.</param>
    /// <param name="p2">The second point to be used in orienting the labels.</param>
    internal void Execute(IPosition p1, IPosition p2)
    {
        m_PrevRotation = MapModel.DefaultTextRotation;

        double dx = p2.X - p1.X;
        double dy = p2.Y - p1.Y;
        m_Rotation = Math.Atan2(dx, dy);

        // Text should always read right-to-left
        if (m_Rotation< 0.0)
            m_Rotation += MathConstants.PI;

        m_Rotation -= MathConstants.PIDIV2;
        if (m_Rotation < 0.0)
            m_Rotation += MathConstants.PIMUL2;

        MapModel.DefaultTextRotation = m_Rotation;
        Complete();
    }

    /// <summary>
    /// Calculates the geometry for any spatial features that were created by
    /// this editing operation.
    /// </summary>
    /// <param name="ctx">The context in which the geometry is being calculated.</param>
    /// <remarks>This is currently used only during deserialization from the database.
    /// By changing the default rotation here (rather than in <c>ProcessFeatures</c>),
    /// it gets assigned at the proper slot in the editing sequence.
    /// </remarks>
    internal override void CalculateGeometry(EditingContext ctx)
    {
        // Remember the current default
        m_PrevRotation = MapModel.DefaultTextRotation;

        // Remember the new rotation as part of the map model
        MapModel.DefaultTextRotation = m_Rotation;
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
    /// The new default rotation (as a clockwise rotation from horizontal axis (+X), in radians)
    /// </summary>
    internal double RotationInRadians
    {
        get => m_Rotation;
        set => m_Rotation = value;
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
        editSerializer.WriteRadians(DataField.Value, new RadianValue(m_Rotation));
    }
}