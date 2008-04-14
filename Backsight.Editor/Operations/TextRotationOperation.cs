/// <remarks>
/// Copyright 2008 - Steve Stanton. This file is part of Backsight
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
    /// <written by="Bob Bruce" on="29-JUN-1998" was="CeSetLabelRotation" />
    /// <summary>
    /// Edit to define the default angle for subsequently added text.
    /// </summary>
    [Serializable]
    class TextRotationOperation : Operation
    {
        #region Class Data

        /// <summary>
        /// The new default rotation (as a clockwise rotation from horizontal axis (+X), in radians)
        /// </summary>
        double m_Rotation;

        /// <summary>
        /// The previous rotation (used for undo)
        /// </summary>
        double m_PrevRotation;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>TextRotationOperation</c> with everything set to zero.
        /// </summary>
        internal TextRotationOperation()
        {
            m_Rotation = m_PrevRotation = 0.0;
        }

        #endregion

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get { return "Set rotation angle for text"; }
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
            get { return new Feature[0]; }
        }

        /// <summary>
        /// The unique identifier for this edit.
        /// </summary>
        internal override EditingActionId EditId
        {
            get { return EditingActionId.SetLabelRotation; }
        }

        /// <summary>
        /// Rollback this operation (occurs when a user undoes the last edit).
        /// </summary>
        /// <returns>True if operation was rolled back ok</returns>
        internal override bool Undo()
        {
            MapModel.DefaultTextRotation = m_PrevRotation;
            return true;
        }

        /// <summary>
        /// Rollforward this edit in response to some sort of update.
        /// </summary>
        /// <returns>True if operation has been re-executed successfully</returns>
        internal override bool Rollforward()
        {
            // Rollforward the base class.
            return base.OnRollforward();
        }

        /// <summary>
        /// Adds references to existing features referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        public override void AddReferences()
        {
            // Do nothing
        }

        /// <summary>
        /// Executes this edit
        /// </summary>
        /// <param name="p1">The first point to be used in orienting the labels.</param>
        /// <param name="p2">The second point to be used in orienting the labels.</param>
        internal void Execute(IPosition p1, IPosition p2)
        {
            m_PrevRotation = MapModel.DefaultTextRotation;

            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;
            m_Rotation = Math.Atan2(dx, dy);

            // Text should always read right-to-left
            if (m_Rotation< 0.0)
                m_Rotation += MathConstants.PI;

            m_Rotation -= MathConstants.PIDIV2;
            if (m_Rotation < 0.0)
                m_Rotation += MathConstants.PIMUL2;

            MapModel.DefaultTextRotation = m_Rotation;
        }
    }
}
