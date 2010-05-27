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
using System.Collections.Generic;

using Backsight.Environment;
using Backsight.Editor.Observations;
using Backsight.Editor.UI;

namespace Backsight.Editor.Operations
{
	/// <written by="Steve Stanton" on="08-DEC-1998" />
    /// <summary>
    /// Operation to add a single sideshot.
    /// </summary>
    /// <remarks>It was originally planned to also provide a RadialStakeout
    /// operation, that would add a whole bunch of sideshots, but there has
    /// been no need for that so far.</remarks>
    class RadialOperation : Operation, IRecallable, IRevisable
    {
        #region Class data

        // Observations ...

        /// <summary>
        /// The direction (could contain an offset).
        /// </summary>
        private Direction m_Direction;

        /// <summary>
        /// The length of the sideshot arm (either a <c>Distance</c> or
        /// an <c>OffsetPoint</c>).
        /// </summary>
        private Observation m_Length;

        // Creations ...

        /// <summary>
        /// The point at the end of the sideshot arm.
        /// </summary>
        private PointFeature m_To;

        /// <summary>
        /// The line (if any) that was added to correspond to the sideshot arm.
        /// </summary>
        private LineFeature m_Line;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for use during deserialization.
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        /// <param name="sequence">The sequence number of the edit within the session (specify 0 if
        /// a new sequence number should be reserved). A non-zero value is specified during
        /// deserialization from the database.</param>
        internal RadialOperation(Session s, uint sequence)
            : base(s, sequence)
        {
            SetInitialValues();
        }

        /// <summary>
        /// Creates a new <c>RadialOperation</c> as part of an editing session.
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        internal RadialOperation(Session s)
            : this(s, 0)
        {
        }

        #endregion

        /// <summary>
        /// Initializes class data with default values
        /// </summary>
        void SetInitialValues()
        {
            m_Direction = null;
            m_Length = null;
            m_To = null;
            m_Line = null;
        }

        /// <summary>
        /// The point that the sideshot was observed from (the origin of
        /// the observed direction).
        /// </summary>
        internal PointFeature From
        {
            get { return (m_Direction==null ? null : (PointFeature)m_Direction.From); }
        }

        /// <summary>
        /// The point created at the end of the sideshot arm.
        /// </summary>
        internal PointFeature Point
        {
            get { return m_To; }
            set { m_To = value; }
        }

        /// <summary>
        /// The line (if any) that was added to correspond to the sideshot arm.
        /// </summary>
        internal LineFeature Line
        {
            get { return m_Line; }
            set { m_Line = value; }
        }

        /// <summary>
        /// The length of the sideshot arm (either a <c>Distance</c> or
        /// an <c>OffsetPoint</c>).
        /// </summary>
        internal Observation Length
        {
            get { return m_Length; }
        }

        /// <summary>
        /// The direction (could contain an offset).
        /// </summary>
        internal Direction Direction
        {
            get { return m_Direction; }
        }

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get { return "Sideshot"; }
        }

        internal override EditingActionId EditId
        {
            get { return EditingActionId.Radial; }
        }

        /// <summary>
        /// Records the input parameters for this edit.
        /// </summary>
        /// <param name="extendLine">The line that's being extended.</param>
        /// <param name="isFromEnd">True if extending from the end | False from the start.</param>
        /// <param name="length">The length of the extension.</param>
        internal void SetInput(Direction dir, Observation length)
        {
            m_Direction = dir;
            m_Length = length;
        }

        /// <summary>
        /// Executes this operation.
        /// </summary>
        /// <param name="dir">The direction of the sideshot (includes the from-point). Not null.</param>
        /// <param name="length"></param>
        /// <param name="pointId"></param>
        /// <param name="lineType"></param>
        /// <returns>True if operation executed ok.</returns>
        internal bool Execute(Direction dir, Observation length, IdHandle pointId, IEntity lineType)
        {
            // Calculate the position of the sideshot point.
            IPosition to = RadialUI.Calculate(dir, length);
            if (to==null)
                throw new Exception("Cannot calculate position of sideshot point.");

            // Save the observations.
            SetInput(dir, length);

            // Add the sideshot point to the map.
            CadastralMapModel map = CadastralMapModel.Current;
            m_To = map.AddPoint(to, pointId.Entity, this);

            // Associate the new point with the specified ID (if any).
            pointId.CreateId(m_To);

            // If a line entity has been supplied, add a line too.
            if (lineType==null)
                m_Line = null;
            else
                m_Line = map.AddLine(dir.From, m_To, lineType, this);

            // Peform standard completion steps
            Complete();
            return true;
        }

        /*
//	@mfunc	Execute this operation WITHOUT attaching an IdHandle.
//	@rdesc	TRUE if operation executed ok.
LOGICAL CeRadial::Execute	( const CeDirection& dir
							, const CeObservation& length
							, const CeEntity* const pPointType
							, const CeEntity* const pLineType ) {

	// Calculate the position of the sideshot point.
	CeVertex to;
	if ( !CuiRadial::Calculate(&dir,&length,to) ) {
		ShowMessage("Cannot calculate position of sideshot point.");
		return FALSE;
	}

	// Save the observations.
	m_pDirection = (CeDirection*)dir.Save();
	m_pLength = length.Save();

	// Add the sideshot point to the map.
	CeMap* pMap = CeMap::GetpMap();
	LOGICAL isold;
	m_pTo = pMap->AddPoint(to,pPointType,isold);
	if ( !m_pTo ) return FALSE;

	// If a line entity has been supplied, add a line too.
	if ( pLineType ) {
		const CePoint* const pFrom = dir.GetpFrom();
		CeVertex from(*pFrom);
		m_pArc = pMap->AddArc(from,to,pLineType);
	}
	else
		m_pArc = 0;

	// Add any direct feature references made by this operation.
	AddReferences();

	// Clean up the map.
	Intersect();
	pMap->CleanEdit();

	return TRUE;

} // end of Execute
*/

        /// <summary>
        /// Corrects this operation. This just changes the info defining the op, and
        /// marks the operation as changed.
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="length"></param>
        /// <returns>True if changes are ok.</returns>
        internal bool Correct(Direction dir, Observation length)
        {
            // Confirm that the sideshot point can be re-calculated.
            IPosition to = RadialUI.Calculate(dir, length);
            if (to==null)
                throw new ArgumentException("Cannot update position of sideshot point.");

            // Cut the references made by the direction object. If nothing
            // has changed, the references will be re-inserted when the
            // direction is re-saved below.
            if (m_Direction!=null)
                m_Direction.OnRollback(this);

            if (m_Length!=null)
                m_Length.OnRollback(this);

            m_Direction = dir;
            m_Direction.AddReferences(this);
            m_Length = length;
            m_Length.AddReferences(this);

            return true;
        }

        /// <summary>
        /// Find the observed length of a line that was created by this operation.
        /// </summary>
        /// <param name="line">The line to find</param>
        /// <returns>The observed length of the line (null if this operation doesn't
        /// reference the specified line)</returns>
        internal override Distance GetDistance(LineFeature line)
        {
            // If the length of the sideshot arm was specified as an
            // entered distance (as opposed to an offset point), return
            // a reference to it.

            if (Object.ReferenceEquals(line, m_Line))
                return (m_Length as Distance);
            else
                return null;
        }

        /// <summary>
        /// The features that were created by this operation.
        /// </summary>
        internal override Feature[] Features
        {
            get
            {
                List<Feature> result = new List<Feature>(2);

                if (m_To!=null)
                    result.Add(m_To);

                if (m_Line!=null)
                    result.Add(m_Line);

                return result.ToArray();
            }
        }

/*
//	@mfunc	Check whether this operation makes reference to
//			a specific feature.
//	@parm	The feature to check for.
LOGICAL CeRadial::HasReference ( const CeFeature* const pFeat ) const {

	if ( m_pDirection && m_pDirection->HasReference(pFeat) ) return TRUE;
	if ( m_pLength && m_pLength->HasReference(pFeat) ) return TRUE;

	return FALSE;

} // end of HasReference
        */

        /*
//	@mfunc	Draw observed angles recorded as part of this op.
//	@parm	The point the observation must be made from.
//			Specify 0 for all points.
//	@parm	The view to draw in.
//	@parm	Display context for the draw.
//	@parm	The ground window for the draw (0 for no check).
void CeRadial::DrawAngles ( const CePoint* const pFrom
						  , CeView* view
						  , CDC* pDC
						  , const CeWindow* const pWin ) const {

	// Skip this op if it has an update.
	if ( GetpLatest() ) return;

	// Draw the direction.
	m_pDirection->DrawAngle(pFrom,view,pDC,pWin,m_pTo);

} // end of DrawAngles
         */

        /*
//	@mfunc	Draw observed angles recorded as part of this op.
//	@parm	The point the observation must be made from.
//			Specify 0 for all points.
//	@parm	The thing we're drawing to.
void CeRadial::DrawAngles ( const CePoint* const pFrom
						  , CeDC& gdc ) const {

	// Skip this op if it has an update.
	if ( GetpLatest() ) return;

	// Draw the direction.
	m_pDirection->DrawAngle(pFrom,gdc,m_pTo);

} // end of DrawAngles
         */

        /*
//	@mfunc	Create transient CeMiscText objects for any observed
//			angles that are part of this operation.
//
//			The caller is responsible for deleting the text
//			objects once done with them.
//
//	@parm	List of pointers to created text objects (appended to).
//	@parm	Should lines be produced too?
//	@parm	The associated point that this op must reference.
void CeRadial::CreateAngleText ( CPtrList& text
							   , const LOGICAL wantLinesToo
							   , const CePoint* const pFrom ) const {

	// Skip this op if it has an update.
	if ( GetpLatest() ) return;

	// Get the direction to do it.
	m_pDirection->CreateAngleText(text,wantLinesToo,pFrom,m_pTo);

} // end of CreateAngleText
         */

        public override void AddReferences()
        {
            m_Direction.AddReferences(this);
            m_Length.AddReferences(this);
        }

        internal override bool Undo()
        {
            base.OnRollback();

            // Delete observations.
            m_Direction.OnRollback(this);
            m_Direction = null;

            m_Length.OnRollback(this);
            m_Length = null;

            // Mark sideshot point for deletion
            Rollback(m_To);

            // If we created a line, mark it as well
            Rollback(m_Line);

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

        	// Re-calculate the position of the sideshot point.
            IPosition to = RadialUI.Calculate(m_Direction, m_Length);
            if (to==null)
                throw new RollforwardException(this, "Cannot re-calculate position of sideshot point.");

        	// Move the sideshot point.
            m_To.MovePoint(uc, to);

            // Rollforward the base class.
            return base.OnRollforward();
        }

        /// <summary>
        /// Calculates the position of the sideshot point.
        /// </summary>
        /// <returns>The calculated position</returns>
        IPosition Calculate()
        {
            return RadialUI.Calculate(m_Direction, m_Length);
        }

        /// <summary>
        /// Performs the data processing associated with this editing operation.
        /// </summary>
        internal override void RunEdit()
        {
            IPosition p = Calculate();
            PointGeometry pg = PointGeometry.Create(p);
            m_To.PointGeometry = pg;
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
