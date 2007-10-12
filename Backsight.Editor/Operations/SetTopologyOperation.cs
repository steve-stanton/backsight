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


namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="30-MAR-1999" was="CeSetTopology" />
    /// <summary>
    /// Edit that toggles the topological status of a line.
    /// </summary>
    [Serializable]
    class SetTopologyOperation : Operation
    {
        #region Class data

        /// <summary>
        /// The line altered by the edit.
        /// </summary>
        readonly LineFeature m_Line;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>SetTopologyOperation</c> that refers to the specified line.
        /// </summary>
        /// <param name="line">The line to change</param>
        internal SetTopologyOperation(LineFeature line)
        {
            if (line==null)
                throw new ArgumentNullException();

            m_Line = line;
        }

        #endregion

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get { return "Polygon boundary status"; }
        }

        /// <summary>
        /// Finds the observed length of a line that was created by this operation.
        /// </summary>
        /// <param name="line">The line to find</param>
        /// <returns>Null (always), since this edit doesn't create any lines.</returns>
        internal override Distance GetDistance(LineFeature line)
        {
            return null;
        }

        /// <summary>
        /// The features created by this editing operation (an empty array).
        /// </summary>
        internal override Feature[] Features
        {
            get { return new Feature[0]; }
        }

        /// <summary>
        /// Handles any intersections created by this operation.
        /// </summary>
        internal override void Intersect()
        {
            // See LineFeature.SwitchTopology (??)
        }

        /// <summary>
        /// The unique identifier for this edit.
        /// </summary>
        internal override EditingActionId EditId
        {
            get { return EditingActionId.SetTopology; }
        }

        /// <summary>
        /// Adds references to existing features referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
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
            m_Line.SwitchTopology();
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

            // Nothing to do

            // Rollforward the base class.
            return base.OnRollforward();
        }

        /// <summary>
        /// Executes the edit.
        /// </summary>
        internal void Execute()
        {
            m_Line.SwitchTopology();
            Complete();
        }
    }
}
