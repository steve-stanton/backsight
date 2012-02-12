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
        internal ImportOperation()
            : base()
        {
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
            m_Data = editDeserializer.ReadPersistentArray<Feature>(DataField.Features);
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

        /// <summary>
        /// Rollback this operation (occurs when a user undoes the last edit).
        /// </summary>
        /// <returns>True if operation was rolled back ok</returns>
        internal override bool Undo()
        {
            base.OnRollback();

        	// Get rid of the features that were created.
            foreach (Feature f in m_Data)
                Rollback(f);

            return true;
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
            editSerializer.WritePersistentArray<Feature>(DataField.Features, this.Features);
        }
    }
}
