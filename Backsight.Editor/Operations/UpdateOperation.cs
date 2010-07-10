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
        /// The edit being updated (not null)
        /// </summary>
        readonly Operation m_Edit;

        /// <summary>
        /// Information about the update (not null)
        /// </summary>
        //readonly UpdateData m_Update;

        #endregion

        #region Constructors

        internal UpdateOperation(Session s, uint sequence)
            : base(s, sequence)
        {
        }

        /// <summary>
        /// Constructor for use during deserialization.
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        /// <param name="t">The serialized version of this instance</param>
        /*
        internal UpdateOperation(Session s, UpdateData t)
            : base(s, t)
        {
            m_Edit = s.MapModel.FindOperation(t.RevisedEdit);
            m_Update = t;

            if (m_Edit == null)
                throw new ArgumentException("Cannot locate original edit "+t.RevisedEdit);
        }
        */

        #endregion

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get { return m_Edit.Name + " (update)"; }
        }

        internal override Distance GetDistance(LineFeature line)
        {
            return null;
        }

        internal override Feature[] Features
        {
            get { return new Feature[0]; }
        }

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
            return false;
        }

        internal override bool Rollforward(UpdateContext uc)
        {
            return true;
        }

        public override void AddReferences()
        {
        }

        internal override void CalculateGeometry()
        {
        }

        internal override LineFeature GetPredecessor(LineFeature line)
        {
            return null;
        }
    }
}
