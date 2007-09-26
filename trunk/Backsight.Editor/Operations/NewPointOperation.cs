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
    [Serializable]
    class NewPointOperation : Operation
    {
        #region Class data

        /// <summary>
        /// The created feature
        /// </summary>
        private PointFeature m_NewPoint;

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

            // Handle any intersections the new point has with the map.
            Intersect();
            map.CleanEdit();

            return true;
        }

        internal override void Intersect()
        {
            // Nothing to do
        }

        internal override EditingActionId EditId
        {
            get { return EditingActionId.NewPoint; }
        }

        internal override string Name
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
    }
}
