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

using Backsight.Environment;

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="22-SEP-1998" was="CeGetControl" />
    /// <summary>
    /// Import control points.
    /// </summary>
    class GetControlOperation : Operation
    {
        #region Class data

        /// <summary>
        /// The point features that were added.
        /// </summary>
        readonly List<PointFeature> m_Features;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        internal GetControlOperation()
        {
            m_Features = new List<PointFeature>();
        }

        #endregion

        /// <summary>
        /// Return true, to indicate that this edit can be corrected.
        /// </summary>
        internal bool CanCorrect
        {
            get { return true; }
        }

        /// <summary>
        /// Attempts to locate a superseded (inactive) line that was the parent of
        /// a specific line.
        /// </summary>
        /// <param name="line">The line of interest</param>
        /// <returns>The superseded line that the line of interest was derived from. Null if
        /// this edit did not create the line of interest.</returns>
        internal LineFeature GetPredecessor(LineFeature line)
        {
            // This edit doesn't supersede anything
            return null;
        }

        /// <summary>
        /// Checks whether this operation makes reference to a specific feature.
        /// </summary>
        /// <param name="feat">The feature to check for.</param>
        /// <returns>True if this edit depends on (contains a reference to) the supplied feature</returns>
        bool HasReference(Feature feat)
        {
            return false;
        }

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get { return "Import control"; }
        }

        /// <summary>
        /// Finds the observed length of a line that was created by this operation.
        /// </summary>
        /// <param name="line">The line to find</param>
        /// <returns>The observed length of the line (null if this operation doesn't
        /// reference the specified line)</returns>
        internal override Distance GetDistance(LineFeature line)
        {
            return null;
        }

        /// <summary>
        /// The features created by this editing operation.
        /// </summary>
        internal override Feature[] Features
        {
            get { return m_Features.ToArray(); }
        }

        /// <summary>
        /// The unique identifier for this edit.
        /// </summary>
        internal override EditingActionId EditId
        {
            get { return EditingActionId.GetControl; }
        }

        /// <summary>
        /// Adds references to existing features referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        public override void AddReferences()
        {
            // nothing to do
        }

        /// <summary>
        /// Rollback this operation (occurs when a user undoes the last edit).
        /// </summary>
        /// <returns>True if operation was rolled back ok</returns>
        internal override bool Undo()
        {
            base.OnRollback();

            // Mark for deletion each created point
            foreach (PointFeature p in m_Features)
                Rollback(p);

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

            // Nothing to do, since the import data comes in with
            // absolute positions ...

            // Rollforward the base class.
            return base.OnRollforward();
        }

        /// <summary>
        /// Imports a control point. Once all control points have been imported, a call
        /// to <see cref="Execute"/> should be made.
        /// </summary>
        /// <param name="control">The control point to import.</param>
        /// <param name="ent">The entity type for the point.</param>
        /// <returns>True if a point was imported</returns>
        /// <remarks>May want to revisit this, since it's possible the CEdit implementation
        /// ignored control points if a point at the same position already existed. This
        /// implementation always adds.</remarks>
        internal bool Import(ControlPoint control, IEntity ent)
        {
            // Save the control point in the map.
            PointFeature point = control.Save(ent, this);
            if (point==null)
                return false;

            // If the point did not previously exist, remember the point.
	        if (point.Creator==this)
                m_Features.Add(point);

	        return true;
        }

        /// <summary>
        /// "Execute" this operation. It actually just does normal cleanup steps performed by
        /// all operations. Prior to call, all the data must have already been added via calls
        /// to <see cref="Import"/>
        /// </summary>
        /// <remarks>This should probably accept an array of control points to process (do away
        /// with separate <c>Import</c> method)</remarks>
        internal void Execute()
        {
            Complete();
        }

        /// <summary>
        /// The number of point features that were created.
        /// </summary>
        int Count
        {
            get { return m_Features.Count; }
        }
    }
}
