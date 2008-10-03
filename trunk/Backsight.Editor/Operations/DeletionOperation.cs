/// <remarks>
/// Copyright 2007 - Steve Stanton. This file is part of Backsight
///
/// Backsight is free software; you can redistribute it and/or modify it under the terms
/// of the GNU Lesser General Public License as published by the Free Software Foundation;
/// either version 3 of the License, or (at your option) any later version.
///
/// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
/// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU Lesser General Public License for more details.
///
/// You should have received a copy of the GNU Lesser General Public License
/// along with this program. If not, see <http://www.gnu.org/licenses/>.
/// </remarks>

using System;
using System.Collections.Generic;

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="22-DEC-1997" was="CeDeletion" />
    /// <summary>
    /// Operation to delete features. When a feature gets deleted, it doesn't disappear,
    /// and it doesn't get garbage collected. It just gets marked as deleted, and will
    /// be retained as part of the map history.
    /// </summary>
    class DeletionOperation : Operation
    {
        #region Class data

        /// <summary>
        /// The features that were deleted.
        /// </summary>
        List<Feature> m_Deletions;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>DeletionOperation</c> that refers to nothing.
        /// </summary>
        public DeletionOperation()
        {
            m_Deletions = null;
        }

        #endregion

        /// <summary>
        /// The user-perceived title for this operation is "Deletion"
        /// </summary>
        public override string Name
        {
            get { return "Deletion"; }
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
        /// The features created by this editing operation.
        /// </summary>
        /// <returns>An empty array</returns>
        internal override Feature[] Features
        {
            get { return new Feature[0]; }
        }

        /// <summary>
        /// The deleted features
        /// </summary>
        internal Feature[] Deletions
        {
            get
            {
                if (m_Deletions==null)
                    return new Feature[0];

                List<Feature> result = new List<Feature>(m_Deletions.Count);
                result.AddRange(m_Deletions);
                return result.ToArray();
            }
        }

        /// <summary>
        /// The unique identifier for this edit.
        /// </summary>
        internal override EditingActionId EditId
        {
            get { return EditingActionId.Deletion; }
        }

        /// <summary>
        /// Adds references to existing features referenced by this operation.
        /// <para/>
        /// In the case of deletions, the deleted features are <b>not</b> cross-referenced to
        /// the deletion operation. Instead, a special flag bit gets set in each feature.
        /// This is perhaps a bit inconsistent.
        /// </summary>
        public override void AddReferences()
        {
            // do nothing
        }

        /// <summary>
        /// Rollback this operation (occurs when a user undoes the last edit).
        /// </summary>
        /// <returns>True if operation was rolled back ok</returns>
        internal override bool Undo()
        {
            base.OnRollback();

            // Restore everything that was deleted.
            foreach(Feature f in m_Deletions)
                f.Restore();

        	return true;
        }

        /// <summary>
        /// Rollforward this edit in response to some sort of update.
        /// </summary>
        /// <returns>True if operation has been re-executed successfully</returns>
        internal override bool Rollforward()
        {
            // Return if this operation has not been marked as changed.
            if (!IsChanged)
                return base.OnRollforward();

            // There's nothing to do for a deletion.
            
	        // Rollforward the base class.
	        return base.OnRollforward();
        }

        /// <summary>
        /// Adds an additional feature to the things to be deleted.
        /// </summary>
        /// <param name="f">The feature to add to the deletions list</param>
        internal void AddDeletion(Feature f)
        {
            if (m_Deletions==null)
                m_Deletions = new List<Feature>(1);

            m_Deletions.Add(f);
        }

        /// <summary>
        /// Executes this operation. Before calling this function, you must make at
        /// least one call to <see cref="AddDeletion"/>.
        /// </summary>
        /// <returns>True if operation succeeded.</returns>
        internal bool Execute()
        {
            // Confirm that at least one call to AddDeletion has been made.
            if (m_Deletions==null)
                throw new Exception("Deletion.Execute - Nothing to delete.");

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
                            if (!line.IsUndoing && !all.Contains(line))
                                all.Add(line);
                        }
                    }
                }
            }

            // Refresh the list if we added in anything extra
            if (all.Count > m_Deletions.Count)
                m_Deletions = all;

            // Mark the features as deleted
            foreach (Feature f in m_Deletions)
                f.IsInactive = true;

            Complete();
            return true;
        }

        /// <summary>
        /// Writes the content of this class. This is called by
        /// <see cref="XmlContentWriter.WriteElement"/>
        /// after the element name and class type (xsi:type) have been written.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteContent(XmlContentWriter writer)
        {
            writer.WriteFeatureReferenceArray("IdArray", "Id", m_Deletions.ToArray());
        }

        /// <summary>
        /// Loads the content of this class. This is called by
        /// <see cref="XmlContentReader"/> during deserialization from XML (just
        /// after the default constructor has been invoked).
        /// </summary>
        /// <param name="reader">The reading tool</param>
        public override void ReadContent(XmlContentReader reader)
        {
            base.ReadContent(reader);
            Feature[] dels = reader.ReadFeatureReferenceArray<Feature>("IdArray", "Id");
            m_Deletions = new List<Feature>(dels);

            // Mark the features as deleted and remove from spatial index
            //EditingIndex index = reader.Model.EditingIndex;
            foreach (Feature f in dels)
            {
                f.Deactivate();
                //index.Remove(f);
            }
        }

        /// <summary>
        /// The number of features deleted by this edit.
        /// </summary>
        /// <remarks>This property is used by <see cref="SessionForm"/> to show
        /// the number of feature involved in each edit.</remarks>
        public override uint FeatureCount
        {
            get { return (uint)m_Deletions.Count; }
        }
    }
}
