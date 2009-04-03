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
using Backsight.Editor.Xml;

namespace Backsight.Editor.Operations
{
    class NewPointOperation : Operation
    {
        #region Class data

        /// <summary>
        /// The created feature
        /// </summary>
        PointFeature m_NewPoint;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for use during deserialization
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        /// <param name="t">The serialized version of this instance</param>
        internal NewPointOperation(Session s, NewPointType t)
            : base(s, t)
        {
            m_NewPoint = new PointFeature(this, t.Point);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NewPointOperation"/> class
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        internal NewPointOperation(Session s)
            : base(s)
        {
            m_NewPoint = null;
        }

        #endregion

        public PointFeature Point
        {
            get { return m_NewPoint; }
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
        /// <returns>True if operation executed ok.</returns>
        public bool Execute(IPosition vtx, IdHandle pointId)
        {
            // Add a point on the current editing layer.
            CadastralMapModel map = CadastralMapModel.Current;
            PointFeature newPoint = map.AddPoint(vtx, pointId.Entity, this);

            // Give the new point the specified ID.
            pointId.CreateId(newPoint);

            m_NewPoint = newPoint;

            // Peform standard completion steps
            Complete();
            return true;
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
        /// Rollforward this operation.
        /// </summary>
        /// <returns>True on success</returns>
        internal override bool Rollforward()
        {
            // Return if this operation has not been marked as changed.
            if (!IsChanged)
                return base.OnRollforward();

            // nothing to do

            // Rollforward the base class.
	        return base.OnRollforward();
        }

        /// <summary>
        /// Returns an object that represents this edit, and that can be serialized using
        /// the <c>XmlSerializer</c> class.
        /// <returns>The serializable version of this edit</returns>
        internal override OperationType GetSerializableEdit()
        {
            NewPointType t = new NewPointType();
            t.Id = this.DataId;
            t.Point = (PointType)m_NewPoint.GetSerializableFeature();
            return t;
        }

        /// <summary>
        /// Calculates the geometry for any features created by this edit.
        /// </summary>
        internal override void CalculateGeometry()
        {
            // Nothing to do
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
