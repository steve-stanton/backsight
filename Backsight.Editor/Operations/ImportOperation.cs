// <remarks>
// Copyright 2007 - Steve Stanton. This file is part of Backsight
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
    /// <summary>
    /// Editing operation that transfers data from a <see cref="FileImportSource"/> to
    /// the current map model.
    /// </summary>
    class ImportOperation : Operation
    {
        #region Class data

        /// <summary>
        /// The features that were imported.
        /// </summary>
        Feature[] m_Data;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportOperation"/> class.
        /// </summary>
        /// <param name="s">The session the operation should be referred to (the session itself
        /// is not modified until the editing operation is saved to the database).</param>
        /// <param name="sequence">The sequence number of the edit within the session (specify 0 if
        /// a new sequence number should be reserved). A non-zero value is specified during
        /// deserialization from the database.</param>
        internal ImportOperation(Session s, uint sequence)
            : base(s, sequence)
        {
            m_Data = null;
        }

        #endregion

        /// <summary>
        /// Executes this import by loading data from the supplied source.
        /// </summary>
        /// <param name="source">The data source to use for the import</param>
        internal void Execute(FileImportSource source)
        {
            m_Data = source.Load(this);
            Complete();
        }

        /// <summary>
        /// The features created by this editing operation (may be an empty array, but
        /// never null).
        /// </summary>
        internal override Feature[] Features
        {
            get { return m_Data; }
        }

        /// <summary>
        /// Records the features created by this edit (for use during deserialization).
        /// </summary>
        /// <param name="data">The features created by this import.</param>
        internal void SetFeatures(Feature[] data)
        {
            if (data == null)
                throw new ArgumentNullException();

            m_Data = data;
        }

        /// <summary>
        /// The unique identifier for this edit.
        /// </summary>
        internal override EditingActionId EditId
        {
            get { return EditingActionId.DataImport; }
        }

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get { return "Data import"; }
        }

        internal override Distance GetDistance(LineFeature line)
        {
            return null; // nothing to do
        }

        internal override bool Undo()
        {
            base.OnRollback();

        	// Get rid of the features that were created.
            foreach (Feature f in m_Data)
                Rollback(f);

            return true;
        }

        /// <summary>
        /// Rollforward this edit in response to some sort of update.
        /// </summary>
        /// <returns>True if operation has been re-executed successfully</returns>
        internal override bool Rollforward()
        {
            throw new NotImplementedException();
            /*
            // Return if this operation has not been marked as changed.
            if (!IsChanged)
                return base.OnRollforward();

            // Nothing to do

            return true;
             */
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
    }
}
