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

//using Backsight.

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="22-DEC-1997" was="CeDeletion" />
    /// <summary>
    /// Operation to delete features. When a feature gets deleted, it doesn't disappear,
    /// and it doesn't get garbage collected. It just gets marked as deleted, and will
    /// be retained as part of the map history.
    /// </summary>
    [Serializable]
    class DeletionOperation : Operation
    {
        #region Class data

        /// <summary>
        /// The features that were deleted.
        /// </summary>
        IPossibleList<Feature> m_Deletions;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>DeletionOperation</c> that refers to nothing.
        /// </summary>
        internal DeletionOperation()
        {
            m_Deletions = null;
        }

        #endregion

        /// <summary>
        /// The user-perceived title for this operation is "Deletion"
        /// </summary>
        internal override string Name
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
        /// Handles any intersections created by this operation (does nothing).
        /// </summary>
        internal override void Intersect()
        {
            // do nothing
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
        internal override bool Rollback()
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
            m_Deletions = (m_Deletions==null ? f : m_Deletions.Add(f));
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
            BasicList<Feature> all = new BasicList<Feature>(m_Deletions);

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
                            if (!line.IsDeleted && !all.Contains(line))
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
                f.IsDeleted = true;

            // Clean the map
            MapModel.CleanEdit();
            return true;
        }

        internal string ToXml()
        {
            return String.Empty;
        }
    }
}
