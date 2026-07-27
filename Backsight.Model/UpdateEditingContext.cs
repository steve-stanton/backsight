using Backsight.Model.Operations;

namespace Backsight.Model
{
    /// <summary>
    /// Changes arising while the a <see cref="CadastralMapModel"/> is being updated (handling
    /// revised information represented by the <see cref="UpdateOperation"/> class).
    /// </summary>
    class UpdateEditingContext : EditingContext
    {
        /// <summary>
        /// The operation holding the changes that are being propagated (not null).
        /// </summary>
        readonly UpdateOperation m_Update;

        /// <summary>
        /// The edits that have been processed via a call to <see cref="Recalculate"/>.
        /// </summary>
        readonly List<Operation> m_RecalculatedEdits;

        /// <summary>
        /// Changes made to the position of point features. The key is the ID of the feature.
        /// The value could conceivably be null.
        /// </summary>
        readonly Dictionary<PointFeature, PointGeometry> m_Changes;

        /// <summary>
        /// Are changes being reverted?
        /// </summary>
        bool m_IsReverting;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateEditingContext"/> class.
        /// </summary>
        /// <param name="uop">The operation holding the changes that are being propagated (not null).</param>
        internal UpdateEditingContext(UpdateOperation uop)
        {
            m_Update = uop;
            m_RecalculatedEdits = new List<Operation>();
            m_Changes = new Dictionary<PointFeature, PointGeometry>();
            m_IsReverting = false;
        }

        /// <summary>
        /// Remembers a modification to the position of a point.
        /// </summary>
        /// <param name="point">The point that is about to be changed</param>
        internal override void RegisterChange(PointFeature point)
        {
            if (m_IsReverting)
            {
                if (m_Changes.ContainsKey(point))
                {
                    // Don't remove from the index. When reverting, the call comes from RevertChanges, which loops
                    // through the items in m_Changes. Can't remove now without screwing up the enumeration. The
                    // m_Changes will be cleared at the end of the loop.
                    //m_Changes.Remove(point);
                    HandlePointMoving(point);
                }
            }
            else
            {
                if (!m_Changes.ContainsKey(point))
                {
                    m_Changes.Add(point, point.PointGeometry);
                    HandlePointMoving(point);
                }
            }
        }

        /// <summary>
        /// Performs processing when a point is about to be moved.
        /// </summary>
        /// <param name="point">The point that is about to move</param>
        void HandlePointMoving(PointFeature point)
        {
            // Remove the point from the spatial index.
            point.RemoveIndex();

            // Remove all dependent spatial objects from the index as well (usually
            // incident lines, but could also be circles)

            // Convert the supplied List to an array, since we may cut refs in
            // the loop below.
            if (point.Dependents != null)
            {
                IFeatureDependent[] deps = point.Dependents.ToArray();

                if (deps != null)
                {
                    // IFeatureDependent is implemented by Feature, Circle, and Operation.
                    // For features and circles, this will remove them from the spatial index (for
                    // line features, any polygon topology will also be marked for a rebuild).
                    // For operations, OnFeatureMoving does nothing.

                    foreach (IFeatureDependent fd in deps)
                        fd.OnFeatureMoving(point, this);
                }
            }
        }

        /// <summary>
        /// Recalculates geometry to account for the update. This should be called after
        /// <see cref="UpdateOperation.ApplyChanges"/> has been called.
        /// <para/>
        /// In a situation where you need to append an additional edit, you should first call
        /// <see cref="RevertChanges"/>, append the edit by calling <see cref="UpdateSource.AddRevisedEdit"/>,
        /// then call this method.
        /// </summary>
        /// <exception cref="RollforwardException">If geometry for an edit could not be re-calculated (in that
        /// case, any subsequent edits that need to be re-calculated will be ignored). In this situation,
        /// The spatial index may be in an indeterminate state (lines added subsequent to the problem edit
        /// may not be evident on a draw).</exception>
        internal void Recalculate()
        {
            // Obtain the calculation sequence (which may have changed as a result of applying the update).
            Operation[] edits = m_Update.MapModel.GetCalculationSequence();

            // The revised edits are the only edit that needs to be re-calculated initially.
            Operation[] revOps = m_Update.RevisedOperations;
            foreach (Operation o in revOps)
                o.ToCalculate = true;

            // Locate the earliest edit in the calculation sequence
            int startIndex = edits.Length;
            foreach (Operation o in revOps)
            {
                int oIndex = Array.IndexOf(edits, o);
                startIndex = Math.Min(oIndex, startIndex);
            }

            // Check all subsequent edits to see which ones also need to be reworked
            for (int i = startIndex+1; i < edits.Length; i++)
            {
                // If any required edit is going to be recalculated, this edit will need to be done too
                Operation currentEdit = edits[i];
                Operation[] req = currentEdit.GetRequiredEdits();

                if (Array.Exists<Operation>(req, t => t.ToCalculate))
                    currentEdit.ToCalculate = true;
            }

            // Recalculate geometry
            foreach (Operation op in edits)
            {
                if (op.ToCalculate)
                    Recalculate(op);
            }

            // At this stage, a call to CadastralMapModel.CleanEdit is needed.
        }

        /// <summary>
        /// Recalculates the geometry for an edit.
        /// </summary>
        /// <param name="op">The edit to recalculate</param>
        /// <exception cref="RollforwardException">If geometry for the edit could not be re-calculated</exception>
        void Recalculate(Operation op)
        {
            m_RecalculatedEdits.Add(op);

            // At this point, I initially used Operation.RemoveFromIndex to take the edit's
            // created feature out of the spatial index (so that they could be re-added after
            // the call to CalculateGeometry). However, there's a problem with lines connected
            // to moved points - when we go on to "recalculate" the edit that added the line
            // it would no longer have access to the original position, so could not remove
            // from the index at that stage.
            //
            // To get around that, lines get removed from the spatial index when terminal
            // points are moved. Each edit's CalculateGeometry method is expected to apply
            // calculated geometry by calling PointFeature.ApplyPointGeometry, which passes
            // on the change to EditingContext.RegisterChange.

            try
            {
                // Re-calculate the geometry for created features
                op.CalculateGeometry(this);
                op.ToCalculate = false;

                // Re-index
                op.AddToIndex();
            }

            catch (Exception ex)
            {
                throw new RollforwardException(op, ex.Message);
            }
        }

        /// <summary>
        /// Reverts all changes recorded as part of this editing context.
        /// </summary>
        internal void RevertChanges()
        {
            if (m_IsReverting)
                throw new InvalidOperationException("Changes are already being undone");

            try
            {
                m_IsReverting = true;

                foreach (Operation op in m_RecalculatedEdits)
                    op.RemoveFromIndex();

                foreach (KeyValuePair<PointFeature, PointGeometry> kvp in m_Changes)
                    kvp.Key.ApplyPointGeometry(this, kvp.Value);

                foreach (Operation op in m_RecalculatedEdits)
                    op.AddToIndex();

                // Clear the stuff just used to undo things
                m_RecalculatedEdits.Clear();
                m_Changes.Clear();

                // Push back the original data
                m_Update.RevertChanges();
            }

            finally
            {
                m_IsReverting = false;
            }
        }

        /// <summary>
        /// The operation holding the changes that are being propagated (not null).
        /// </summary>
        internal UpdateOperation UpdateSource => m_Update;
    }
}
