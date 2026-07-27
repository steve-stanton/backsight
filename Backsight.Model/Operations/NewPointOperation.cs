using Backsight.Environment;

namespace Backsight.Model.Operations;

class NewPointOperation : Operation, IRecallable, IRevisable
{
    /// <summary>
    /// The created feature
    /// </summary>
    PointFeature? m_NewPoint;

    /// <summary>
    /// Initializes a new instance of the <see cref="NewPointOperation"/> class.
    /// </summary>
    /// <param name="store">The map store this operation is part of.</param>
    internal NewPointOperation(IMapStore store)
        : base(store)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NewPointOperation"/> class
    /// using the data read from persistent storage.
    /// </summary>
    /// <param name="editDeserializer">The mechanism for reading back content.</param>
    internal NewPointOperation(EditDeserializer editDeserializer)
        : base(editDeserializer)
    {
        m_NewPoint = editDeserializer.ReadPersistent<PointFeature>(DataField.Point);
    }

    public PointFeature Point => m_NewPoint;

    internal void SetNewPoint(PointFeature p)
    {
        m_NewPoint = p;
    }

    internal override Feature[] Features
    {
        get
        {
            if (m_NewPoint is null)
                return [];
            else
                return [m_NewPoint]; }
    }

    /// <summary>
    /// Executes the new point operation.
    /// </summary>
    /// <param name="vtx">The position of the new point.</param>
    /// <param name="e">The entity type for the point (not null)</param>
    /// <param name="pointId">The ID to assign to the new point</param>
    internal void Execute(IPosition vtx, IEntity e, FeatureId pointId)
    {
        // Add a point on the model
        m_NewPoint = MapModel.AddPoint(vtx, e, this);

        // Give the new point the specified ID (point the ID to the feature & vice versa)
        if (pointId != null)
            pointId.Add(m_NewPoint);

        // Peform standard completion steps
        Complete();
    }

    internal override EditingActionId EditId => EditingActionId.NewPoint;

    /// <summary>
    /// A user-perceived title for this operation.
    /// </summary>
    public override string Name => "Add point";

    /// <summary>
    /// Rollback this operation (occurs when a user undoes the last edit).
    /// </summary>
    internal override void Undo()
    {
        base.OnRollback();
        Rollback(m_NewPoint);
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
    /// Obtains update items for a revised version of this edit
    /// (for later use with <see cref="ExchangeData"/>).
    /// </summary>
    /// <param name="newPosition">The revised position</param>
    /// <returns>The items representing the change (may be subsequently supplied to
    /// the <see cref="ExchangeUpdateItems"/> method).</returns>
    internal UpdateItemCollection GetUpdateItems(Position newPosition)
    {
        UpdateItemCollection result = new UpdateItemCollection();
        result.AddItem<double>(DataField.X, m_NewPoint.Easting.Meters, newPosition.X);
        result.AddItem<double>(DataField.Y, m_NewPoint.Northing.Meters, newPosition.Y);
        return result;
    }

    /// <summary>
    /// Writes updates for an editing operation to a persistent storage area.
    /// </summary>
    /// <param name="editSerializer">The mechanism for storing content.</param>
    /// <param name="data">The collection of changes to write</param>
    public void WriteUpdateItems(EditSerializer editSerializer, UpdateItemCollection data)
    {
        data.WriteItem<double>(editSerializer, DataField.X);
        data.WriteItem<double>(editSerializer, DataField.Y);
    }

    /// <summary>
    /// Reads back updates made to an editing operation.
    /// </summary>
    /// <param name="editDeserializer">The mechanism for reading back content.</param>
    /// <returns>The changes made to the edit</returns>
    public UpdateItemCollection ReadUpdateItems(EditDeserializer editDeserializer)
    {
        UpdateItemCollection result = new UpdateItemCollection();
        result.ReadItem<double>(editDeserializer, DataField.X);
        result.ReadItem<double>(editDeserializer, DataField.Y);
        return result;
    }

    /// <summary>
    /// Exchanges any previously generated update items (this is currently done
    /// by <see cref="NewPointForm.GetUpdateItems"/>).
    /// </summary>
    /// <param name="data">The update data to apply to the edit (modified to
    /// hold the values that were previously defined for the edit)</param>
    public override void ExchangeData(UpdateItemCollection data)
    {
        // Do nothing! This method is here because it is specified by the IRevisable
        // interface. But, while the NewPointOperation can be updated, it is unlike
        // other edits because we are explicitly changing a position (rather than
        // changing observations that are used to calculate a position).

        // Data exchange ordinarily happens before anything is moved, because
        // it may alter the calculation sequence. While explicitly changing a
        // position will not alter the sequence, it will move the point a little
        // too soon (the UpdateEditingContext.Recalculate method is responsible
        // for removing stuff from the spatial index, re-calculating geometry,
        // and re-adding to the spatial index). Ok, we could try fixing up the
        // index as part of this method. But if we do that, the old geometry
        // would not get remembered as part of the UpdateEditingContext (which
        // gets utilized in the event of a rollback).

        // To get around all this, the data exchange will be deferred until
        // CalculateGeometry is called (see implementation below).

        // ...the only problem with the above is that during deserialization, data
        // exchange occurs at the moment the update is encountered in the deserialization
        // stream. Subsequently, CalculateGeometry will be called with a null editing
        // context, so the update info will not be available. So do the exchange if
        // the model is just being loaded.

        if (MapModel.WorkingSession is null)
            ApplyUpdateItems(null, data);
    }

    /// <summary>
    /// Performs the data processing associated with this editing operation.
    /// </summary>
    /// <param name="ctx">The context in which the geometry is being calculated.</param>
    internal override void CalculateGeometry(EditingContext ctx)
    {
        // We only have to do stuff when processing an update (see comments in ExchangeData).

        if (ctx is UpdateEditingContext)
        {
            UpdateEditingContext uec = (ctx as UpdateEditingContext);
            ApplyUpdateItems(ctx, uec.UpdateSource.GetChanges(this));
        }
    }

    /// <summary>
    /// Applies changes to this editing operation.
    /// </summary>
    /// <param name="ctx">The editing context (null if the model is being deserialized)</param>
    /// <param name="data">The changes to apply</param>
    void ApplyUpdateItems(EditingContext ctx, UpdateItemCollection data)
    {
        double x = data.ExchangeValue<double>(DataField.X, m_NewPoint.Easting.Meters);
        double y = data.ExchangeValue<double>(DataField.Y, m_NewPoint.Northing.Meters);
        var pg = new PointGeometry(x, y);
        m_NewPoint.ApplyPointGeometry(ctx, pg);
    }

    /// <summary>
    /// Writes the content of this instance to a persistent storage area.
    /// </summary>
    /// <param name="editSerializer">The mechanism for storing content.</param>
    public override void WriteData(EditSerializer editSerializer)
    {
        base.WriteData(editSerializer);
        editSerializer.WritePersistent<PointFeature>(DataField.Point, m_NewPoint);
    }
}