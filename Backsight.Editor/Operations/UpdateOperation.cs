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

using Backsight.Editor.Observations;


namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="15-MAY-2009" />
    /// <summary>
    /// An edit representing a revision to an earlier edit.
    /// </summary>
    class UpdateOperation : Operation
    {
        #region Class data

        /// <summary>
        /// The edit being updated (not null). Must implement <see cref="IRevisable"/>.
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
        /// <param name="s">The session the operation should be referred to (the session itself
        /// is not modified until the editing operation is saved to the database).</param>
        /// <param name="sequence">The sequence number of the edit within the session (specify 0 if
        /// a new sequence number should be reserved). A non-zero value is specified during
        /// deserialization from the database.</param>
        /// <param name="revisedEdit">The edit being updated (not null). Must implement
        /// <see cref="IRevisable"/>.</param>
        /// <exception cref="ArgumentNullException">If either <paramref name="edit"/> or
        /// <paramref name="changes"/> is null.</exception>
        /// <exception cref="ArgumentException">If <paramref name="revisedEdit"/> does not
        /// implement <see cref="IRevisable"/>.</exception>
        internal UpdateOperation(Session s, uint sequence, Operation revisedEdit, UpdateItemCollection changes)
            : base(s, sequence)
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
        /// Finds the observed length of a line that was created by this operation.
        /// </summary>
        /// <param name="line">The line to find</param>
        /// <returns>Null (always)</returns>
        internal override Distance GetDistance(LineFeature line)
        {
            return null;
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

            // TODO: Anything else to do?

            return true;
        }

        /// <summary>
        /// Rollforward this edit in response to some sort of update.
        /// </summary>
        /// <returns>
        /// True if operation has been re-executed successfully
        /// </returns>
        internal override bool Rollforward()
        {
            throw new NotImplementedException();
            //return true;
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
            m_Changes.ExchangeData((IRevisable)m_Edit);
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

            editSerializer.Writer.WriteString("RevisedEdit", m_Edit.DataId);

            UpdateItem[] items = m_Changes.ToArray();
            /*

            // Re-express update items using *Data objects
            UpdateItem[] items = op.Changes.ToArray();
            UpdateItem[] dataItems = new UpdateItem[items.Length];

            for (int i = 0; i < items.Length; i++)
            {
                object o = items[i].Value;

                if (o is Feature)
                    o = (o as Feature).DataId;
                else if (o is Observation)
                    o = DataFactory.Instance.ToData<ObservationData>((Observation)o);

                dataItems[i] = new UpdateItem(items[i].Name, o);
            }

            // The root node always identifies an array of UpdateItem
            //this.Changes = new YamlSerializer().Serialize(dataItems);
             */
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateOperation"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal UpdateOperation(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
            throw new NotImplementedException();
        }
    }
}
