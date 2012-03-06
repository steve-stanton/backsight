// <remarks>
// Copyright 2009 - Steve Stanton. This file is part of Backsight
//
// Backsight is free software; you can redistribute it and/or modify it under the terms
// of the GNU Lesser General Public License as published by the Free Software Foundation;
// either version 3 of the License, or (at your option) any later version.
//
// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
// </remarks>

using System;
using System.Collections.Generic;

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="15-MAY-2009" />
    /// <summary>
    /// An edit representing a revision to one (or more) editing operations.
    /// </summary>
    /// <remarks>Updates can change the observations that were specified for previous edits, but it
    /// must not create any new spatial features.
    /// <para/>
    /// An observation mentioned in an update can refer to a feature that was created AFTER the
    /// revised edit, so long as the referenced feature is not dependent on the revised edit.
    /// </remarks>
    class UpdateOperation : Operation
    {
        #region Class data

        /// <summary>
        /// The editing operations that were changed
        /// </summary>
        RevisedEdit[] m_Revisions;

        /// <summary>
        /// Have the revisions been used to modify the original edits?
        /// </summary>
        bool m_IsApplied;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateOperation"/> class
        /// that involves changes to more than one editing operation.
        /// </summary>
        /// <param name="revisions">The editing operations that were changed.</param>
        internal UpdateOperation(RevisedEdit[] revisions)
            : base()
        {
            if (revisions == null || revisions.Length == 0)
                throw new ArgumentException();

            m_Revisions = revisions;
            m_IsApplied = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateOperation"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal UpdateOperation(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
            if (editDeserializer.IsNextField(DataField.RevisedEdits))
            {
                m_Revisions = editDeserializer.ReadPersistentArray<RevisedEdit>(DataField.RevisedEdits);
            }
            else
            {
                RevisedEdit rev = new RevisedEdit(editDeserializer);
                m_Revisions = new RevisedEdit[] { rev };
            }

            m_IsApplied = false;
        }

        #endregion

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get
            {
                if (m_Revisions.Length == 1)
                    return m_Revisions[0].RevisedOperation.Name + " (update)";
                else
                    return String.Format("Update to {0} editing operations", m_Revisions.Length);
            }
        }

        /// <summary>
        /// The features created by this editing operation (may be an empty array, but
        /// never null).
        /// </summary>
        /// <value>An empty array</value>
        internal override Feature[] Features
        {
            get { return new Feature[0]; }
        }

        /// <summary>
        /// The unique identifier for this edit.
        /// </summary>
        /// <value>EditingActionId.Update</value>
        internal override EditingActionId EditId
        {
            get { return EditingActionId.Update; }
        }

        /// <summary>
        /// Rollback this operation (occurs when a user undoes the last edit).
        /// </summary>
        internal override void Undo()
        {
            // Undoing involves re-exchanging the data for each revised edit, then re-calculating.

            // The update edit should have been removed already from the map model (see Session.Rollback),
            // so the re-calculate call done below will not attempt to re-run this update again.

            // Hitting a RollforwardException at this stage would be quite unexpected, indicating a basic
            // application logic error, so do not attempt to cover that eventuality.

            RevertChanges();
            UpdateEditingContext uec = new UpdateEditingContext(this);
            uec.Recalculate();
        }

        /// <summary>
        /// Adds references to existing features referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// <para/>
        /// This override does nothing. In the case of updates, the references should be
        /// made from the edit that was revised. That should occur when update items are
        /// applied to the original edit using <see cref="IRevisable.ExchangeData"/>.
        /// </summary>
        public override void AddReferences()
        {
            // TODO: What about updates that change an observation so that something else is
            // referenced? I think the handling of references should be getting done when
            // the revised edit's ExchangeData method is called.
        }

        /// <summary>
        /// Obtains the features that are referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        /// <returns>The referenced features (never null, but may be an empty array).</returns>
        public override Feature[] GetRequiredFeatures()
        {
            if (m_Revisions.Length == 1)
                return m_Revisions[0].Changes.GetReferences();

            // Initialize with the features required by the first revision
            List<Feature> result = new List<Feature>();
            result.AddRange(m_Revisions[0].Changes.GetReferences());

            // Append features referenced by subsequent revisions (excluding duplicates)
            for (int i = 1; i < m_Revisions.Length; i++)
            {
                Feature[] fa = m_Revisions[i].Changes.GetReferences();
                foreach (Feature f in fa)
                {
                    if (!result.Contains(f))
                        result.Add(f);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Obtains the edits that must be completed before this edit.
        /// </summary>
        /// <returns>An array holding the revised edit.</returns>
        internal override Operation[] GetRequiredEdits()
        {
            Operation[] result = new Operation[m_Revisions.Length];

            for (int i = 0; i < result.Length; i++)
                result[i] = m_Revisions[i].RevisedOperation;

            return result;
        }

        /// <summary>
        /// Attempts to locate a superseded (inactive) line that was the parent of
        /// a specific line.
        /// </summary>
        /// <param name="line">The line of interest</param>
        /// <returns>
        /// Null (always)
        /// </returns>
        internal override LineFeature GetPredecessor(LineFeature line)
        {
            return null;
        }

        /// <summary>
        /// Ensures that revised editing operations have been reverted to their original state.
        /// </summary>
        internal void RevertChanges()
        {
            if (m_IsApplied)
            {
                foreach (RevisedEdit re in m_Revisions)
                    re.ApplyChanges(); // apply again to undo

                m_IsApplied = false;
            }
        }

        /// <summary>
        /// Ensures that changes have been applied to the revised editing operations.
        /// </summary>
        internal void ApplyChanges()
        {
            if (!m_IsApplied)
            {
                foreach (RevisedEdit re in m_Revisions)
                    re.ApplyChanges();

                m_IsApplied = true;
            }
        }

        /// <summary>
        /// The editing operations that are associated with the elements of the <see cref="RevisedEdits"/> array.
        /// </summary>
        internal Operation[] RevisedOperations
        {
            get
            {
                var result = new Operation[m_Revisions.Length];

                for (int i = 0; i < result.Length; i++)
                    result[i] = m_Revisions[i].RevisedOperation;

                return result;
            }
        }

        /// <summary>
        /// Obtains the changes for a specific editing operation.
        /// </summary>
        /// <param name="op">The edit of interest</param>
        /// <returns>The corresponding changes (null if not found)</returns>
        internal UpdateItemCollection GetChanges(Operation op)
        {
            RevisedEdit rev = Array.Find<RevisedEdit>(m_Revisions, r => r.RevisedOperation == op);
            return (rev == null ? null : rev.Changes);
        }

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public override void WriteData(EditSerializer editSerializer)
        {
            base.WriteData(editSerializer);

            if (m_Revisions.Length == 1)
                m_Revisions[0].WriteData(editSerializer);
            else
                editSerializer.WritePersistentArray<RevisedEdit>(DataField.RevisedEdits, m_Revisions);
        }

        /// <summary>
        /// Remembers an additional revision is part of this operation.
        /// </summary>
        /// <param name="rev">The additional revision to append</param>
        /// <remarks>This method should be called only by the <see cref="UpdateUI"/> class
        /// in a situation where the user is fixing up a rollforward problem.</remarks>
        internal void AddRevisedEdit(RevisedEdit rev)
        {
            Array.Resize<RevisedEdit>(ref m_Revisions, m_Revisions.Length + 1);
            m_Revisions[m_Revisions.Length - 1] = rev;
        }
    }
}
