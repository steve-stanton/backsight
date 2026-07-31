using System.Diagnostics;

namespace Backsight.Model.Operations;

/// <written by="Steve Stanton" on="22-DEC-1997" was="CeDeletion" />
/// <summary>
/// Operation to delete features. When a feature gets deleted, it doesn't disappear,
/// and it doesn't get garbage collected. It just gets marked as deleted, and will
/// be retained as part of the map history.
/// </summary>
class DeletionOperation : Operation, IFeatureRefArray
{
    /// <summary>
    /// The features that were deleted.
    /// </summary>
    Feature[] m_Deletions;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeletionOperation"/> class that refers to nothing.
    /// </summary>
    /// <param name="store">The map store the features are part of.</param>
    /// <param name="deletions">The features to be deleted (expected to be at least one).</param>
    internal DeletionOperation(IMapStore store, Feature[] deletions)
        : base(store)
    {
        m_Deletions = deletions;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeletionOperation"/> class
    /// using the data read from persistent storage.
    /// </summary>
    /// <param name="editDeserializer">The mechanism for reading back content.</param>
    internal DeletionOperation(EditDeserializer editDeserializer)
        : base(editDeserializer)
    {
        try
        {
            m_Deletions = editDeserializer.ReadFeatureRefArray<Feature>(this, DataField.Delete);

            // Deactivate features (means they will never make it into the spatial index, and
            // any lines will be invisible as far as intersection tests are concerned).
            DeserializationFactory dff = new DeserializationFactory(this);
            ProcessFeatures(dff);
        }

        catch (Exception ex)
        {
            throw new ApplicationException("Error loading edit " + this.EditSequence, ex);
        }
    }

    /// <summary>
    /// The user-perceived title for this operation is "Deletion"
    /// </summary>
    public override string Name => "Deletion";

    /// <summary>
    /// The features created by this editing operation.
    /// </summary>
    /// <returns>An empty array</returns>
    internal override Feature[] Features => [];

    /// <summary>
    /// The deleted features
    /// </summary>
    internal Feature[] Deletions => m_Deletions;

    /// <summary>
    /// The unique identifier for this edit.
    /// </summary>
    internal override EditingActionId EditId => EditingActionId.Deletion;

    /// <summary>
    /// Rollback this operation (occurs when a user undoes the last edit).
    /// </summary>
    internal override void Undo()
    {
        OnRollback();

        // Restore everything that was deleted.
        foreach(Feature f in m_Deletions)
            f.Restore();
    }

    /// <summary>
    /// Executes this operation. Before calling this function, you must make at
    /// least one call to <see cref="AddDeletion"/>.
    /// </summary>
    internal void Execute()
    {
        // Confirm that at least one call to AddDeletion has been made.
        if (m_Deletions==null)
            throw new Exception("Deletion.Execute - Nothing to delete.");

        // TODO: This should probably be done as part of the DeletionUI class (and make m_Deletions readonly)
        // Stick the features that were explicitly noted into the complete list
        List<Feature> all = new List<Feature>(m_Deletions);

        // Loop through the features, checking for point features that
        // have attached lines.
        foreach (Feature f in m_Deletions)
        {
            if (f is PointFeature && f.HasDependents)
            {
                // Go through incident lines, checking to see it they're in the
                // deletions list. If not, remember them in the extras list.
                PointFeature p = (f as PointFeature);
                foreach (IFeatureDependent fd in p.Dependents)
                {
                    if (fd is LineFeature)
                    {
                        LineFeature line = (fd as LineFeature);
                            
                        // Ignore lines that pass THROUGH the point (we only want to remove
                        // lines that terminate at the point)
                        if (line.StartPoint == f || line.EndPoint == f)
                        {
                            if (!line.IsUndoing && !all.Contains(line))
                                all.Add(line);
                        }
                    }
                }
            }
        }

        // Refresh the list if we added in anything extra
        if (all.Count > m_Deletions.Length)
            m_Deletions = all.ToArray();

        FeatureFactory ff = new FeatureFactory(this);
        base.Execute(ff);

        //// Mark the features as deleted
        //foreach (Feature f in m_Deletions)
        //    f.IsInactive = true;

        //Complete();
    }

    /// <summary>
    /// Performs data processing that involves creating or retiring spatial features.
    /// Newly created features will not have any definition for their geometry - a
    /// subsequent call to <see cref="CalculateGeometry"/> is needed to to that.
    /// </summary>
    /// <param name="ff">The factory class for generating any spatial features</param>
    internal override void ProcessFeatures(FeatureFactory ff)
    {
        foreach (Feature f in m_Deletions)
        {
            // TESTING - allow nulls for now
            // Array elements could be null if the deletion had a forward ref originating from CEdit
            if (f != null)
                ff.Deactivate(f);

            //if (f != null)
            //    ff.Deactivate(f);
        }
    }

    /// <summary>
    /// The number of features deleted by this edit.
    /// </summary>
    /// <remarks>This property is used by <see cref="SessionForm"/> to show
    /// the number of feature involved in each edit.</remarks>
    public override uint FeatureCount => (uint)m_Deletions.Length;

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
        return m_Deletions;
    }

    /// <summary>
    /// Adds references to existing features referenced by this operation (including features
    /// that are indirectly referenced by observation classes).
    /// <para/>
    /// This override does nothing. A deletions operates directly on the referenced features
    /// (sets a special flag bit).
    /// </summary>
    private protected override void AddReferences()
    {
    }

    /// <summary>
    /// Writes the content of this instance to a persistent storage area.
    /// </summary>
    /// <param name="editSerializer">The mechanism for storing content.</param>
    public override void WriteData(EditSerializer editSerializer)
    {
        base.WriteData(editSerializer);
        editSerializer.WriteFeatureRefArray<Feature>(DataField.Delete, m_Deletions.ToArray());
    }

    public void ApplyFeatureRefArray(DataField field, ForwardRefArrayItem[] featureRefs)
    {
        Debug.Assert(field == DataField.Delete);

        foreach (var item in featureRefs)
        {
            if (item.ArrayIndex < 0 || item.ArrayIndex >= m_Deletions.Length)
                throw new IndexOutOfRangeException();

            m_Deletions[item.ArrayIndex] = item.Feature;

            // As DeserializationFactory.Deactivate...
            if (item.Feature is LineFeature line)
                line.RemoveTopology();

            item.Feature.IsInactive = true;
        }
    }
}