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


namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="15-MAY-2009" />
    /// <summary>
    /// An edit representing a revision to an earlier edit.
    /// </summary>
    /// <remarks>Updates can change the observations that were specified for a previous edit, but it
    /// must not create any new spatial features.
    /// <para/>
    /// The observations mentioned in an update can refer to features that were created AFTER the
    /// revised edit, so long as the referenced feature is not dependent on the revised edit.
    /// </remarks>
    class UpdateOperation : Operation
    {
        #region Class data

        /// <summary>
        /// The edit being updated (not null).
        /// </summary>
        readonly Operation m_Edit;

        /// <summary>
        /// Information about the update (not null). 
        /// </summary>
        UpdateItemCollection m_Changes;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateOperation"/> class.
        /// </summary>
        /// <param name="revisedEdit">The edit being updated (not null). Must implement
        /// <see cref="IRevisable"/>.</param>
        /// <exception cref="ArgumentNullException">If either <paramref name="edit"/> or
        /// <paramref name="changes"/> is null.</exception>
        /// <exception cref="ArgumentException">If <paramref name="revisedEdit"/> does not
        /// implement <see cref="IRevisable"/>.</exception>
        internal UpdateOperation(Operation revisedEdit, UpdateItemCollection changes)
            : base()
        {
            if (revisedEdit == null || changes == null)
                throw new ArgumentNullException();

            if (!(revisedEdit is IRevisable))
                throw new ArgumentException();

            m_Changes = changes;
            m_Edit = revisedEdit;
        }

        #endregion

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get { return m_Edit.Name + " (update)"; }
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
        /// <returns>True if operation was rolled back ok</returns>
        internal override bool Undo()
        {
            // Copy the original stuff back to the edit
            ApplyChanges();

            // Rework the map model ... TODO - does the update need to be already removed
            // from the model? (see Session.Rollback). Should Undo return bool or throw
            // exception?
            UpdateEditingContext uec = new UpdateEditingContext(this);
            uec.Recalculate();

            return true;
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
            return m_Changes.GetReferences();
        }

        /// <summary>
        /// Obtains the edits that must be completed before this edit.
        /// </summary>
        /// <returns>An array holding the revised edit.</returns>
        internal override Operation[] GetRequiredEdits()
        {
            return new Operation[] { m_Edit };
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
        /// Exchanges changes with the revised edit.
        /// </summary>
        internal void ApplyChanges()
        {
            ((IRevisable)m_Edit).ExchangeData(m_Changes);
        }

        /// <summary>
        /// The edit being updated (not null). Must implement <see cref="IRevisable"/>.
        /// </summary>
        internal Operation RevisedEdit
        {
            get { return m_Edit; }
        }

        /// <summary>
        /// Information about the update (not null). 
        /// </summary>
        internal UpdateItemCollection Changes
        {
            get { return m_Changes; }
            set { m_Changes = value; }
        }

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public override void WriteData(EditSerializer editSerializer)
        {
            base.WriteData(editSerializer);

            editSerializer.WriteInternalId(DataField.RevisedEdit, m_Edit.InternalId);
            (m_Edit as IRevisable).WriteUpdateItems(editSerializer, m_Changes);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateOperation"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal UpdateOperation(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
            InternalIdValue id = editDeserializer.ReadInternalId(DataField.RevisedEdit);
            m_Edit = editDeserializer.MapModel.FindOperation(id);
            m_Changes = (m_Edit as IRevisable).ReadUpdateItems(editDeserializer);
        }
    }
}
