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
    class NewPointOperation : Operation, IRecallable
    {
        #region Class data

        /// <summary>
        /// The created feature
        /// </summary>
        PointFeature m_NewPoint;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NewPointOperation"/> class.
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        /// <param name="sequence">The sequence number of the edit within the session (specify 0 if
        /// a new sequence number should be reserved). A non-zero value is specified during
        /// deserialization from the database.</param>
        internal NewPointOperation(Session s, uint sequence)
            : base(s, sequence)
        {
        }

        #endregion

        public PointFeature Point
        {
            get { return m_NewPoint; }
        }

        internal void SetNewPoint(PointFeature p)
        {
            m_NewPoint = p;
        }

        internal override Feature[] Features
        {
            get
            {
                if (m_NewPoint==null)
                    return new Feature[0];
                else
                    return new Feature[] { m_NewPoint }; }
        }

        /// <summary>
        /// Executes the new point operation.
        /// </summary>
        /// <param name="vtx">The position of the new point.</param>
        /// <param name="pointId">The ID (and entity type) to assign to the new point</param>
        internal void Execute(IPosition vtx, IdHandle pointId)
        {
            // Add a point on the model
            m_NewPoint = MapModel.AddPoint(vtx, pointId.Entity, this);

            // Give the new point the specified ID.
            pointId.CreateId(m_NewPoint);

            // Peform standard completion steps
            Complete();
        }

        internal override EditingActionId EditId
        {
            get { return EditingActionId.NewPoint; }
        }

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get { return "Add point"; }
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
            Rollback(m_NewPoint);
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

            // nothing to do

            // Rollforward the base class.
	        return base.OnRollforward();
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
