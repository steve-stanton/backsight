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
        UpdateItem[] m_Changes;

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
        /// <exception cref="ArgumentNullException">If either <paramref name="edit"/> or
        /// <paramref name="changes"/> is null.</exception>
        /// <exception cref="ArgumentException">If <paramref name="changes"/> is an
        /// empty array.</exception>
        internal UpdateOperation(Session s, uint sequence, Operation edit, UpdateItem[] changes)
            : base(s, sequence)
        {
            if (edit == null || changes == null)
                throw new ArgumentNullException();

            if (changes.Length == 0)
                throw new ArgumentException("Empty change list");

            if (!(edit is IRevisable))
                throw new ArgumentException("Edit is not tagged as revisable");

            m_Edit = edit;
            m_Changes = changes;
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
        /// <param name="uc">The context in which editing revisions are being made (not null).
        /// Used to hold a record of any positional changes.</param>
        /// <returns>
        /// True if operation has been re-executed successfully
        /// </returns>
        internal override bool Rollforward(UpdateContext uc)
        {
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
        }

        /// <summary>
        /// Obtains the features that are referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        /// <returns>The referenced features (never null, but may be an empty array).</returns>
        public override Feature[] GetRequiredFeatures()
        {
            List<Feature> result = new List<Feature>();

            foreach (object o in m_Changes)
            {
                IFeatureDependent fd = (o as IFeatureDependent);
                if (fd != null)
                    result.AddRange(fd.GetRequiredFeatures());
            }

            return result.ToArray();
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
            IRevisable r = (IRevisable)m_Edit;
            m_Changes = r.ExchangeData(m_Changes);
        }

        /// <summary>
        /// The edit being updated (not null). Must implement <see cref="IRevisable"/>.
        /// </summary>
        internal Operation RevisedEdit
        {
            get { return m_Edit; }
        }
    }
}
