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
using Backsight.Editor.Xml;


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
        /// The features that were imported (null until the <see cref="Execute"/> method
        /// is called).
        /// </summary>
        Feature[] m_Data;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for use during deserialization
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        /// <param name="t">The serialized version of this instance</param>
        internal ImportOperation(Session s, ImportData t)
            : base(s, t)
        {
            m_Data = new Feature[t.Feature.Length];
            for (int i=0; i<m_Data.Length; i++)
            {
                FeatureData f = t.Feature[i];
                m_Data[i] = f.LoadFeature(this);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Import"/> class
        /// that refers to nothing.
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        internal ImportOperation(Session s)
            : base(s)
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

        internal override Feature[] Features
        {
            get { return m_Data; }
        }

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

        public override void AddReferences()
        {
            // No direct references
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
        /// <param name="uc">The context in which editing revisions are being made (not null).
        /// Used to hold a record of any positional changes.</param>
        /// <returns>True if operation has been re-executed successfully</returns>
        internal override bool Rollforward(UpdateContext uc)
        {
            // Return if this operation has not been marked as changed.
            if (!IsChanged)
                return base.OnRollforward();

            // Nothing to do

            return true;
        }

        /// <summary>
        /// Returns an object that represents this edit, and that can be serialized using
        /// the <c>XmlSerializer</c> class.
        /// <returns>The serializable version of this edit</returns>
        internal override OperationData GetSerializableEdit()
        {
            return new ImportData(this);
            //ImportData t = new ImportData();
            //base.SetSerializableEdit(t);

            //FeatureData[] features = new FeatureData[m_Data.Length];
            //for (int i = 0; i < features.Length; i++)
            //{
            //    features[i] = m_Data[i].GetSerializableFeature();
            //}

            //t.Feature = features;
            //return t;
        }

        /// <summary>
        /// Performs the data processing associated with this editing operation.
        /// </summary>
        internal override void RunEdit()
        {
            // Could calculate the radius of circles created by the import. However, this
            // is already done by the LineFeature constructor that accepts an ArcType.
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
