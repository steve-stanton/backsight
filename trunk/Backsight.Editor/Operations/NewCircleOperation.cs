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
    /// <written by="Steve Stanton" on="01-DEC-1998" was="CeNewCircle" />
    /// <summary>
    /// Operation to add a new circle
    /// </summary>
    [Serializable]
    class NewCircleOperation : NewLineOperation
    {
        #region Class data

        /// <summary>
        /// Point at the center of the circle.
        /// </summary>
        PointFeature m_Center;

        /// <summary>
        /// The radius of the circle (either a <see cref="Distance"/> object,
        /// or an <see cref="OffsetPoint"/> that sits on the circumference of
        /// the circle.
        /// </summary>
        Observation m_Radius;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor creates a <c>NewCircleOperation</c> with
        /// everything set to null.
        /// </summary>
        internal NewCircleOperation()
        {
            m_Center = null;
            m_Radius = null;
        }

        #endregion

        /// <summary>
        /// Point at the center of the circle.
        /// </summary>
        internal PointFeature Center
        {
            get { return m_Center; }
        }

        /// <summary>
        /// The radius of the circle (either a <see cref="Distance"/> object,
        /// or an <see cref="OffsetPoint"/> that sits on the circumference of
        /// the circle.
        /// </summary>
        internal Observation Radius
        {
            get { return m_Radius; }
        }

        /// <summary>
        /// Return true, to indicate that this edit can be corrected.
        /// </summary>
        internal bool CanCorrect
        {
            get { return true; }
        }

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get { return "Add new circle"; }
        }

        /// <summary>
        /// The unique identifier for this edit.
        /// </summary>
        internal override EditingActionId EditId
        {
            get { return EditingActionId.NewCircle; }
        }

        /// <summary>
        /// Rollback this operation.
        /// </summary>
        /// <returns></returns>
        internal override bool Undo()
        {
            // Delete observed radius.
            m_Radius.OnRollback(this);
            m_Radius = null;

            // Cut direct refs made by this operation.
            if (m_Center != null)
                m_Center.CutOp(this);

            // Rollback the base class (mark the circle for undo).
            return base.Undo();
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

            // Get the circle that has changed.
            ArcFeature line = (this.Line as ArcFeature);
            if (line==null)
                throw new RollforwardException(this, "NewCircleOperation.Rollforward - Unexpected line type.");
            ArcGeometry curve = (ArcGeometry)line.LineGeometry;

            Circle circle = (Circle)curve.Circle;

            // Rollforward the base class (does nothing)
            return base.Rollforward();

            // Get the new radius (on the ground).
            ILength rad = m_Radius.GetDistance(m_Center);
            if (rad.Meters < Constants.TINY)
                throw new RollforwardException(this, "NewCircleOperation.Rollforward - New radius is too small.");

            // Mark the circle as moved (force CleanEdit to re-intersect).
            // Actually, I think this should do nothing, seeing how circle
            // construction lines are supposed to always be non-topolgical.
            line.IsMoved = true;

	        // If the underlying circle was created by this op, update
	        // it so that it has the correct center and radius.
	        // If the circle previously existed, make a new one (if there
	        // isn't one there already).

            if (Object.ReferenceEquals(circle.Creator, this))
                circle.MoveCircle(m_Center, rad);
            else
            {
                // Is there a suitable circle where we're going to? If not, create a new one.
                // If a new circle gets created, this will reference the center point to the circle.
                circle = MapModel.AddCircle(m_Center, rad);
            }

            // Update the arc geometry. The BC=EC must move to
            // fall on the updated circle. For circles where the
            // radius was defined using an offset point, we move
            // to there. Otherwise we want a point at the top of
            // the circle.

            if (m_Radius is OffsetPoint)
            {
                OffsetPoint offset = (m_Radius as OffsetPoint);
                PointFeature start = offset.Point;

                // Alter the arc (the complete circle) so it starts at
                // (and ends) at the offset position.
                //line.ChangeEnds(start, start);
            }
            else
            {
                // Get the current BC/EC position.
            }
        }
        /*
	else {

		CeLocation* pStart = (CeLocation*)pCurve->GetpStart();

		// Get the new start location for the curve.
		newstart = CeVertex( m_pCentre->GetEasting()
						   , m_pCentre->GetNorthing()+radm );

		// If the current location somehow existed before this
		// operation (highly unlikely, although it COULD be
		// an old offset point I guess), create a new location.
		// Otherwise shift it.

		if ( pStart->GetpCreator()==this )
			pStart->Move(newstart);
		else {
			pStart = (CeLocation*)pMap->AddLocation(newstart);
			pCurve->ChangeEnds(*pStart,*pStart);
		}
	}
         */

        /// <summary>
        /// Adds references to existing features referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        public override void AddReferences()
        {
            m_Center.AddOp(this);
            m_Radius.AddReferences(this);
        }

        /// <summary>
        /// Executes this new circle operation.
        /// </summary>
        /// <param name="center">The point at the center of the circle.</param>
        /// <param name="radius">The radius of the circle (either a <see cref="Distance"/> object,
        /// or an <see cref="OffsetPoint"/> that sits on the circumference of the circle.</param>
        internal void Execute(PointFeature center, Observation radius)
        {
            // Remember the center point.
            m_Center = center;

            // Get the radius, in meters on the ground.
            ILength rad = radius.GetDistance(center);
            if (rad.Meters < Constants.TINY)
                throw new Exception("NewCircleOperation.Execute - Radius is too close to zero.");

            // If the radius was specified as an offset point, make the circle
            // start at that point.
            OffsetPoint offset = (radius as OffsetPoint);
            PointFeature start = (offset == null ? null : offset.Point);

            // Add a circle to the map.
            CadastralMapModel map = MapModel;
            ArcFeature arc = map.AddCompleteCircularArc(center, rad, start, this);

            // Record the new arc in the base class.
            SetNewLine(arc);

            // Save the observed radius
            m_Radius = radius;

            // Peform standard completion steps
            Complete();
        }

        /// <summary>
        /// Checks whether this operation makes reference to a specific feature.
        /// </summary>
        /// <param name="feat">The feature to check for.</param>
        /// <returns>True if this edit depends on (contains a reference to) the supplied feature</returns>
        bool HasReference(Feature feat)
        {
            if (Object.ReferenceEquals(m_Center, feat))
                return true;

            if (m_Radius.HasReference(feat))
                return true;

            return false;
        }

        /// <summary>
        /// Corrects this operation.
        /// </summary>
        /// <param name="center">The point at the center of the circle.</param>
        /// <param name="radius">The radius of the circle (either a <see cref="Distance"/> object,
        /// or an <see cref="OffsetPoint"/> that sits on the circumference of the circle.</param>
        /// <returns>True if operation updated ok.</returns>
        bool Correct(PointFeature center, Observation radius)
        {
            // If the center point has changed, cut the reference to this
            // operation from the old point, and change it so the
            // operation is referenced from the new center.
            if (!Object.ReferenceEquals(m_Center, center))
            {
                m_Center.CutOp(this);
                m_Center = center;
                m_Center.AddOp(this);
            }

            // If the old radius observation refers to an offset point, cut
            // the reference that the point has to this op. If nothing has
            // changed, the reference will be re-inserted when the
            // observation is re-saved below.
            CutOffsetRef(m_Radius);

            // Get rid of the previously defined observation, and replace
            // with the new one (we can't necessarily change the old one
            // because we may have changed the type of observation).

            m_Radius.OnRollback(this);
            m_Radius = radius;
            m_Radius.AddReferences(this);

            return true;
        }

        /// <summary>
        /// The features created by this editing operation.
        /// </summary>
        internal override Feature[] Features
        {
            get
            {
                // The point at the BC (and EC) of the arc may have been created by this
                // operation, or may be a previously existing point used to define the radius.

                PointFeature p = this.Line.StartPoint;

                if (Object.ReferenceEquals(p.Creator, this))
                    return new Feature[] { this.Line, p };
                else                
                    return new Feature[] { this.Line };
            }
        }
    }
}
