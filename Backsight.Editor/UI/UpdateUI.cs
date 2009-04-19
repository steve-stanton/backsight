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
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

using Backsight.Editor.Operations;
using Backsight.Forms;
using Backsight.Editor.Forms;
using Backsight.Editor.Observations;

namespace Backsight.Editor.UI
{
    /// <written by="Steve Stanton" on="30-NOV-1999" was="CuiUpdate"/>
    /// <summary>
    /// User interface for updating editing operations
    /// </summary>
    class UpdateUI : SimpleCommandUI
    {
        #region Class data

        /// <summary>
        /// The update that is currently being executed.
        /// </summary>
        CommandUI m_Cmd;

        /// <summary>
        /// The feature currently selected for update.
        /// </summary>
        Feature m_Update;

        /// <summary>
        /// The number of undo markers that have been set.
        /// </summary>
        //uint m_NumUndo;

        /// <summary>
        /// Dependent operations.
        /// </summary>
        List<Operation> m_DepOps;

        /// <summary>
        /// The operation that rollforward has had problems re-executing.
        /// </summary>
        Operation m_Problem;

        /// <summary>
        /// Info about the current feature that's selected for update.
        /// </summary>
        UpdateForm m_Info;

        /// <summary>
        /// The last editing operation that was completed prior to the start of the update (null
        /// if the update was started at the very beginning of the session).
        /// </summary>
        readonly Operation m_LastEdit;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateUI"/> class.
        /// </summary>
        /// <param name="action">The action that initiated this command</param>
        public UpdateUI(IUserAction action)
            : base(action)
        {
            m_Update = null;
            m_Info = null;
            m_Cmd = null;
            //m_NumUndo = 0;
            m_DepOps = null;
            m_Problem = null;
            m_LastEdit = Session.WorkingSession.LastOperation;
        }

        #endregion

        public void Dispose()
        {
            /*
	// If a command is running(!), abort it now.
	if ( m_pCmd ) {
		m_pCmd->DialAbort(0);
		delete m_pCmd;
		m_pCmd = 0;
	}

	// Get rid of the dialog that shows info about the
	// current update feature.
	if ( m_pInfo ) {
		m_pInfo->DestroyWindow();
		delete m_pInfo;
		m_pInfo = 0;
	}

	// Get rid of dependent op list.
	delete m_pDepOps;
	m_pDepOps = 0;
             */
        }

        /// <summary>
        /// Updates a specific feature.
        /// </summary>
        /// <param name="update">The feature selected for update.</param>
        /// <returns>True if feature accepted for update. False if an update is already in progress.</returns>
        internal bool Run(Feature update)
        {
            // Return if we're currently updating something, or we've
            // hit a problem during rollforward.
            if (m_Cmd != null || m_Problem != null)
                return false;

            // If we prevously had something selected for update,
            // undo any drawing that we did for it.
            ErasePainting();

            // Remember the specified feature.
            m_Update = update;

            // If the info dialog has not already been displayed, display it now.
            if (m_Info == null)
            {
		        m_Info = new UpdateForm(this);
                m_Info.Show();
            }

            // Get the info window to display stuff about the
            // selected feature.
            m_Info.Display(m_Update);

            // Leave keyboard focus with the info dialog.
            m_Info.Focus();

            // Draw stuff.
            Draw();
            return true;
        }

        /*
//	@mfunc	Handle request to wrap things up.
void CuiUpdate::Finish ( void ) {

	// Erase stuff.
	Erase();

	// Force a redraw (for luck).
	m_View.InvalidateRect(0);

	// Get the base class to finish up (will destroy this
	// command).
	CuiCommand::FinishCommand();

} // end of Finish
         */

        /// <summary>
        /// Displays info about the edits that are dependent on the feature
        /// currently selected for update.
        /// </summary>
        internal void Dependencies()
        {
            // Get the operation that created the update feature.
            Operation pop = GetOp();
            if (pop == null)
                return;

            throw new NotImplementedException("UpdateUI.Dependencies");
        }
        /*
void CuiUpdate::Dependencies ( void ) {

	// Get a list of the impacted operations & features.
	CeObjectList feats;
	CeObjectList ops;
	CeMap::GetpMap()->Touch(*pop,feats,ops);

	// Display all the dependent features (includes the operation
	// that we're changing).
	Draw(feats,FALSE);

	// List the dependent operations.
	CdOpList dial(ops,"Dependent Operations");
	dial.DoModal();

	// Draw all the dependent features in their normal way
	// (except for features that are no longer active).
	Draw(feats,TRUE);

	// Ensure the original draw looks the same.
	Draw();

} // end of Dependencies
         */

        /// <summary>
        /// Returns the first predecessor (if any) for a specific feature.
        /// </summary>
        /// <param name="feat"></param>
        /// <returns>The predecessor (must be a line). Null if none.</returns>
        static LineFeature GetPredecessor(Feature feat)
        {
            LineFeature line = (feat as LineFeature);
            if (line == null)
                return null;
            else
                return line.GetPredecessor();
        }

        /// <summary>
        /// Displays info about any previous incarnations of the feature
        /// currently selected for update.
        /// </summary>
        internal void Predecessors()
        {
            // Return if there is no predecessor.
            LineFeature prevLine = GetPredecessor(m_Update);
            if (prevLine == null)
                return;
        }
        /*
void CuiUpdate::Predecessors ( void ) {

	const CeArc* pPrevArc = GetPredecessor(m_pUpdate);
	if ( !pPrevArc ) return;

	// Get a list of all the predecessor lines (breaking out as soon
	// as we hit something that has no creating op).

	CeObjectList prevarcs;

	for ( ;
		  pPrevArc;
		  pPrevArc = GetPredecessor(pPrevArc) ) {

		// Ensure it's not the result of a split.
		pPrevArc = pPrevArc->GetUserArc();

		// Break out if the arc does not have a creating operation.
		if ( !pPrevArc->GetpCreator() ) break;

		// Remember the arc.
		prevarcs.Append(pPrevArc);
	}

	CdOpList dial(prevarcs,"Predecessor Operations",TRUE,TRUE);
	if ( dial.DoModal()==IDOK ) {

		// Get the selected feature.
		pPrevArc = dynamic_cast<CeArc*>(dial.GetpFeature());

		// Refresh the dialog.
		if ( pPrevArc ) Run(*pPrevArc);
	}

} // end of Predecessors
         */

        /// <summary>
        /// Cancels all updates.
        /// </summary>
        internal void Cancel()
        {
            // No can do if an update is currently in progress!
            if (m_Cmd != null)
            {
                string msg = "You are in the middle of making an update." + System.Environment.NewLine;
                msg += "You must cancel that first.";
                MessageBox.Show(msg);
                return;
            }

            // If we've currently got a problem, treat the cancellation
            // request as a request to undo just the last update.
            if (m_Problem != null)
            {
                Undo();
                m_Problem = null;
                if (m_Update != null)
                    Run(m_Update);

                m_Info.OnFinishUpdate(null);
                ErasePainting();
            }
            else
            {
                // If we set any undo points, undo them all.
                UndoAll();

                FinishCommand();
            }
        }

        /*
//	@mfunc	Invoke the update dialog for the selected feature.
//
//////////////////////////////////////////////////////////////////////

void CuiUpdate::Update ( void ) {

	// Nothing to do if update feature is (somehow) undefined.
	if ( !m_pUpdate ) return;

	// Return if already updating something.
	if ( m_pCmd ) return;

	// Get the operation (if any) that created the feature and
	// re-run that operation.

	CeOperation* pop = m_pProblem;
	if ( !pop ) pop = m_pUpdate->GetpCreator();
	if ( !pop ) {
		AfxMessageBox("Specified feature is not associated with an edit.");
		return;
	}

	// Erase anything we've drawn (the command should be able
	// to draw stuff in it's own way, and that might conflict
	// with what this object does).
	Erase();

	// Get a list of the dependent operations.
	if ( !m_pDepOps )
		m_pDepOps = new CeObjectList();
	else
		m_pDepOps->Remove();

	CeObjectList feats;
	CeMap* pMap = CeMap::GetpMap();
	pMap->Touch(*pop,feats,*m_pDepOps);
	feats.Remove();

	// Run the update command. If it's left running, declare
	// a save point (so long as we're not responding to a problem
	// that has arisen during rollforward).
	if ( RunUpdate() ) {
		if ( m_pProblem==0 )
			SetUndoMarker();
	}

} // end of Update
         */

        /// <summary>
        /// Do any drawing that is specific to the current update.
        /// </summary>
        void Draw()
        {
            // Get the operation associated with the update feature (or
            // the current problem op)
            Operation op = GetOp();
            if (op==null)
                return;

            // Draw the features that were created by the operation that created the feature
            // selected for update.
            ISpatialDisplay display = ActiveDisplay;
            IDrawStyle style = Controller.Style(Color.Magenta);
            op.Render(display, style, true);

            // If the update feature is a line, ensure that it is highlighted the normal
            // way (so that the direction of the line is apparent).
            if (m_Update is LineFeature)
                m_Update.Render(display, Controller.HighlightStyle);
        }

        /*
//	@mfunc	Draw (or un-draw) all the features in a list.
//
//	@parm	The list of features.
//	@parm	Are we undo-ing an earlier draw?
//
///////////////////////////////////////////////////////////////////////

void CuiUpdate::Draw ( const CeObjectList& flist
					 , const LOGICAL isUndo ) const {

	// Reserve a DC.
	CClientDC dc(&m_View);
	m_View.OnPrepareDC(&dc);

	CeListIter loop(&flist);
	CeFeature* pFeat;
	COLORREF col;

	// If we're undoing an earlier draw, erase anything in the
	// list that is inactive.

	if ( isUndo ) {

		COLORREF bkcol = GetSysColor(COLOR_WINDOW);
		CPen bkpen(PS_SOLID,1,bkcol);
		CBrush bkbrush(bkcol);
		CPen* pOldPen = dc.SelectObject(&bkpen);
		CBrush* pOldBrush = dc.SelectObject(&bkbrush);

		for ( pFeat = (CeFeature*)loop.GetHead();
			  pFeat;
			  pFeat = (CeFeature*)loop.GetNext() ) {

			if ( pFeat->IsInactive() ) pFeat->Draw(&m_View,&dc);
		}

		dc.SelectObject(pOldPen);
		dc.SelectObject(pOldBrush );

		col = COL_BLACK;
	}
	else
		col = COL_MAGENTA;

	// Draw each feature (skipping inactive features if we're
	// undoing an earlier draw).

	CPen pen(PS_SOLID,1,col);
	CBrush brush(col);
	dc.SelectObject(&pen);
	dc.SelectObject(&brush);

	for ( pFeat = (CeFeature*)loop.GetHead();
		  pFeat;
		  pFeat = (CeFeature*)loop.GetNext() ) {

		if ( isUndo && pFeat->IsInactive() ) continue;

		pFeat->Draw(&m_View,&dc);
	}

} // end of Draw
         */

        /*
//	@mfunc	Run the update for the current update feature.
//
//	@rdesc	TRUE if a modeless command dialog has been started.
//
//////////////////////////////////////////////////////////////////////

LOGICAL CuiUpdate::RunUpdate ( void ) {

	// Get the operation that created the feature selected
	// for update.
	CeOperation* pop = GetOp();
	if ( !pop ) return FALSE;

	// There shouldn't already be a command running.
	if ( m_pCmd ) {
		ShowMessage("CuiUpdate::RunUpdate\nUpdate already running?");
		return FALSE;
	}

	switch ( pop->GetType() ) {

	case CEOP_ARC_EXTEND:
	{
		m_pCmd = new CuiArcExtend(ID_LINE_EXTEND,*this);
		break;
	}

	case CEOP_ARC_SUBDIVISION:
	{
		m_pCmd = new CuiArcSubdivision(ID_LINE_SUBDIVIDE,*this);
		break;
	}

	case CEOP_DIR_INTERSECT:
	{
		m_pCmd = new CuiIntersect(ID_INTERSECT_BB,*this);
		break;
	}

	case CEOP_DIRDIST_INTERSECT:
	{
		m_pCmd = new CuiIntersect(ID_INTERSECT_BD,*this);
		break;
	}

	case CEOP_DIRLINE_INTERSECT:
	{
		m_pCmd = new CuiIntersect(ID_INTERSECT_BL,*this);
		break;
	}

	case CEOP_DIST_INTERSECT:
	{
		m_pCmd = new CuiIntersect(ID_INTERSECT_DD,*this);
		break;
	}

	case CEOP_GET_CONTROL:
	{
		m_pCmd = new CuiNewPoint(ID_GETCONTROL,*this);
		break;
	}

	case CEOP_LINE_INTERSECT:
	{
		m_pCmd = new CuiIntersect(ID_INTERSECT_LL,*this);
		break;
	}

	case CEOP_NEW_CIRCLE:
	{
		m_pCmd = new CuiNewCircle(ID_LINE_CIRCLE,*this);
		break;
	}

	case CEOP_NEW_POINT:
	{
		m_pCmd = new CuiNewPoint(ID_POINT_NEW,*this);
		break;
	}

	case CEOP_PATH:
	{
		m_pCmd = new CuiPath(ID_PATH,*this);
		break;
	}

	case CEOP_PARALLEL:
	{
		m_pCmd = new CuiParallel(ID_LINE_PARALLEL,*this);
		break;
	}

	case CEOP_POINT_ON_LINE:
	{
		m_pCmd = new CuiPointOnLine(ID_POINT_ON_LINE,*this);
		break;
	}

	case CEOP_RADIAL:
	{
		m_pCmd = new CuiRadial(ID_POINT_SIDESHOT,*this);
		break;
	}

	} // end switch

	if ( m_pCmd ) {
		m_pCmd->Run();
		return TRUE;
	}

	ShowMessage("You cannot update the selected feature this way.");
	return FALSE;

} // end of RunUpdate
         */

        /// <summary>
        /// Returns the operation that created the feature that is currently selected for update.
        /// </summary>
        /// <returns>The operation (could conceivably be null).</returns>
        internal Operation GetOp()
        {
            if (m_Problem != null)
                return m_Problem;

            if (m_Update == null)
                return null;

            return (Operation)m_Update.Creator;
        }

        /// <summary>
        /// Handles completion of an editing command.
        /// </summary>
        /// <param name="cmd">The command that is finishing.</param>
        /// <returns>True if the command is one that was created by this update object.</returns>
        internal bool FinishCommand(CommandUI cmd)
        {
	        // Return if we somehow don't have an operation.
	        Operation pop = GetOp();
	        if (pop==null)
                return false;

	        // Were we just updating the absolute position of a point?
	        // If so, grab the new position before destroying the
	        // command (we need to do this because CeNewPoint::Rollforward
	        // cannot calculate a new absolute position).

            NewPointUI newPoint = (cmd as NewPointUI);
            PointFeature point = null;
            IPosition newpos = null;
	        if (newPoint!=null)
            {
                point = m_Update as PointFeature;
                Debug.Assert(point!=null);
                newpos = newPoint.Position;
	        }

        	// Delete the command.
	        if (!DeleteCommand(cmd))
                return false;

	        // Was the command run to fix a problem?
	        bool wasProblem = (m_Problem!=null);
	        m_Problem = null;

	        // If so, re-display the info for the originally selected op.
	        if ( wasProblem && m_Update!=null )
                Run(m_Update);

	        // If we're not responding to a rollforward problem, tell
	        // the map that rollforward is about to begin.
            CadastralMapModel map = CadastralMapModel.Current;
	        if (!wasProblem)
                map.StartRollforward();

	        // If a new absolute position has been defined for a point,
	        // just move it. Otherwise mark the modified operation as
	        // changed, so that rollforward will re-calculate stuff.

        	if (newpos!=null)
		        point.Move(newpos);
	        else
		        pop.OnMove(null);

	        // Propagate the change (breaking if an operation can no
	        // longer be calculated, which assigns m_Problem)
	        Rollforward(pop);

	        // Ensure info window is shown
	        m_Info.OnFinishUpdate(m_Problem);

	        // Force a redraw.
            this.Controller.RefreshAllDisplays();
	        return true;
        }

        /// <summary>
        /// Propogates changes, starting at the specified operation. If any problem arises,
        /// the draw window will be recentred on the problem area.
        /// </summary>
        /// <param name="op">The operation to start with</param>
        /// <returns>Any operation that caused a problem during rollfoward (this is also
        /// assigned to <c>m_Problem</c>)</returns>
        Operation Rollforward(Operation op)
        {
            throw new NotImplementedException("UpdateUI.Rollforward");
            /*
	        // Propagate the change (breaking if an operation can no longer be calculated).
            CadastralMapModel map = CadastralMapModel.Current;
	        m_Problem = map.Rollforward(op);

	        // Cleanup the map (if there's a problem, don't fix up the
	        // topology, since things may be screwed up royally).
	        if (m_Problem!=null)
		        map.MaintainTopology = false;
	        else
		        map.FinishRollforward();

	        // Clean up the map.
	        map.CleanEdit();

	        // If we switched off topology, switch it on again now just
	        // so we don't forget later. Also ensure that the problem
	        // is on screen.
	        if (m_Problem!=null)
            {
		        map.MaintainTopology = true;

		        // Get the view's current draw window
                ISpatialDisplay display = EditingController.Current.ActiveDisplay;
                IWindow drawin = display.Extent;

		        // If the problem isn't on screen, re-centre at the current scale
                IPosition center;
		        if (m_Problem.GetReCentre(drawin,center))
		        {
                    double scale = display.MapScale;
                    display.SetDraw(center,scale);
		        }
	        }

	        return m_Problem;
             */
        }

        /// <summary>
        /// Handles cancellation of an editing command.
        /// </summary>
        /// <param name="cmd">The command that is being cancelled.</param>
        /// <returns>True if the command is one that was created by this update object.</returns>
        internal bool AbortCommand(CommandUI cmd)
        {
	        // Return if the command wasn't started by this update.
	        if (!Object.ReferenceEquals(cmd, m_Cmd))
                return false;

	        // Delete the command.
            m_Cmd.Dispose();
            m_Cmd = null;

	        // Revert to the save point that was set when we started the command.

	        // SS 27-MAR-2003 -- In the case of a command that was
	        // invoked to fix a problem, DON'T undo. That has to
	        // wait until the user clicks the Undo button that's
	        // part of the m_Info dialog.

	        if (m_Problem==null)
        		Undo();

	        // Paint in our own way.
	        Draw();

	        // Ensure info window is shown
	        m_Info.OnAbortUpdate();
	        return true;
        }

        /// <summary>
        /// Runs this instance (implements the abstract method declared by the base class, but
        /// does nothing).
        /// </summary>
        /// <returns>False (always). To start an update, use <see cref="Run(Feature)"/></returns>
        internal override bool Run()
        {
            return false;
        }

        /// <summary>
        /// Override indicates that this command performs painting. This means that the
        /// controller will periodically call the <see cref="Paint"/> method (probably during
        /// idle time).
        /// </summary>
        internal override bool PerformsPainting
        {
            get { return true; }
        }

        /// <summary>
        /// Does any command-specific drawing.
        /// </summary>
        /// <param name="point">The specific point (if any) that the parent window has drawn. Not used.</param>
        internal override void Paint(PointFeature point)
        {
            if (m_Cmd == null)
                Draw();
            else
                m_Cmd.Paint(point);
        }

        /*
//	@mfunc	Handle mouse-move.
//	@parm	The new position of the mouse, in logical units.
void CuiUpdate::MouseMove ( const CPoint& lpt ) {

	if ( m_pCmd ) m_pCmd->MouseMove(lpt);

} // end of MouseMove
         */

        /*
//	@mfunc	Handle left mouse click.
//	@parm	The position where the left-click occurred, in logical
//			units.
void CuiUpdate::LButtonDown ( const CPoint& lpt ) {

	if ( m_pCmd ) m_pCmd->LButtonDown(lpt);

} // end of LButtonDown
         */

        /*
//	@mfunc	Handle left-up mouse click.
//	@parm	The position where the left-up occurred, in logical
//			units.
void CuiUpdate::LButtonUp ( const CPoint& lpt ) {

	if ( m_pCmd ) m_pCmd->LButtonUp(lpt);

} // end of LButtonUp
         */

        /*
//	@mfunc	Handle double-click.
//	@parm	The position where the double-click occurred, in
//			logical units.
void CuiUpdate::LButtonDblClick ( const CPoint& lpt ) {

	if ( m_pCmd )
		m_pCmd->LButtonDblClick(lpt);

} // end of LButtonDblClick
         */

        /*
//	@mfunc	Handle right mouse click.
//	@parm	The position where the right-click occurred, in
//			logical units.
//	@rdesc	TRUE if right click was handled.
LOGICAL CuiUpdate::RButtonDown ( const CPoint& lpt ) {

	if ( m_pCmd )
		return m_pCmd->RButtonDown(lpt);
	else
		return FALSE;

} // end of RButtonDown
         */

        /*
//	@mfunc	React to selection of the Cancel button in the dialog.
//	@parm	The dialog window.
void CuiUpdate::DialAbort ( CWnd* pWnd ) {

	if ( m_pCmd ) m_pCmd->DialAbort(pWnd);

} // end of DialAbort
         */

        /*
//	@mfunc	React to selection of the OK button in the dialog.
//	@parm	The dialog window.
LOGICAL CuiUpdate::DialFinish ( CWnd* pWnd ) {

	if ( m_pCmd )
		return m_pCmd->DialFinish(pWnd);
	else
		return FALSE;

} // end of DialFinish
         */

        /*
//	@mfunc	React to the selection of a point feature.
//	@parm	The point (if any) that has been selected.
void CuiUpdate::OnSelectPoint ( const CePoint* const pPoint ) {

	if ( m_pCmd ) {

		// Can't pick something created by a dependent op.
		if ( IsDependent(pPoint) )
			DependencyMessage();
		else
			m_pCmd->OnSelectPoint(pPoint);
	}
	else if ( pPoint )
		Run(*pPoint);

} // end of OnSelectPoint
         */

        /*
//	@mfunc	React to the selection of a line feature.
//	@parm	The line (if any) that has been selected.
void CuiUpdate::OnSelectArc ( const CeArc* const pArc ) {

	if ( m_pCmd )
		m_pCmd->OnUpdateSelect(pArc);
	else if ( pArc )
		Run(*pArc);

} // end of OnSelectArc
         */

        /// <summary>
        /// Checks if a feature that the user is trying to select is dependent on the
        /// edit that is currently being updated.
        /// </summary>
        /// <param name="feat">The feature the user is trying to use.</param>
        /// <returns>True if it's a dependent feature.</returns>
        internal bool IsDependent(Feature feat)
        {
	        // No problem if nothing specified!
	        if (feat==null)
                return false;

	        // The dependencies have to be defined!
	        if (m_DepOps==null)
                return false;

	        // No dependency if the feature has a primitive that's a
	        // multi-segment (since the position of multi-segments is
	        // expected to be immutable). This is perhaps a bit of a
	        // hack that is meant to get around problems that may
	        // arise when dealing with a complex series of intersections
	        // with river lines. If this solution proves to be unacceptable,
	        // the place to introduce an alternative is in the
	        // CuiIntersect::OnUpdateSelect method (which is currently
	        // only a placeholder).
            /*
	            const CePrimitive* const pPrim = pFeat->GetPrimitive();
	            if ( pPrim->GetType() == PTY_MULTI ) return FALSE;
            */

	        // Get the operation that created the feature.
	        Operation op = feat.Creator;
	        return m_DepOps.Contains(op);
        }

        /// <summary>
        /// Displays message that indicates an attempt to make use of a dependent feature.
        /// </summary>
        internal void DependencyMessage()
        {
            string msg = String.Format("{0}{1}{2}", 
                            "You are trying to use a feature with a position", System.Environment.NewLine,
		                    "that is dependent on the edit you are changing.");
            MessageBox.Show(msg);
        }

        /*
//	@mfunc	Receive a sub-command.
//	@parm	The ID of the sub-command.
//	@rdesc	TRUE if sub-command was dispatched.
LOGICAL CuiUpdate::Dispatch ( const INT4 id ) {

	if ( m_pCmd )
		return m_pCmd->Dispatch(id);
	else
		return FALSE;

} // end of Dispatch
         */

        /*
//	@mfunc	Return the ID of the cursor for this command.
INT4 CuiUpdate::GetCursorId ( void ) const {

	if ( m_pCmd )
		return m_pCmd->GetCursorId();
	else
		return 0;

} // end of GetCursorId
         */

        /*
//	@mfunc	Return a pointer to the object selected for update.
CeObject* CuiUpdate::GetpUpdate ( void ) const { return m_pUpdate; }
         */

        /// <summary>
        /// Accepts a new offset
        /// </summary>
        /// <param name="offset"></param>
        internal override void SetOffset(Offset offset)
        {
            if (m_Cmd != null)
                m_Cmd.SetOffset(offset);
        }

        /// <summary>
        /// Deletes any command dialog previously created to make an update.
        /// </summary>
        /// <param name="cmd">The command to get rid of</param>
        /// <returns>True if the supplied command matches the one previously created
        /// by this update object (in that case, m_Cmd gets disposed and nulled out).
        /// False if the supplied command is anything else.</returns>
        bool DeleteCommand(CommandUI cmd)
        {
	        // Return if the command wasn't started by this update.
	        if (!Object.ReferenceEquals(cmd, m_Cmd))
                return false;

	        // Get rid of the command.
            if (m_Cmd!=null)
            {
                m_Cmd.Dispose();
                m_Cmd = null;
            }

	        return true;
        }

        /// <summary>
        /// Rolls back to the last undo point, and updates dialog to reflect this.
        /// </summary>
        void Undo()
        {
            throw new NotImplementedException("UpdateUI.Undo");

            /*
	        CadastralMapModel.Current.Undo();
	        if (m_NumUndo>0)
                m_NumUndo--;

	        if (m_Info!=null)
                m_Info.SetUpdateCount(m_NumUndo);
             */
        }

        /*
//	@mfunc	Rolls back all undo points created by this command,
//			and updates dialog to reflect this.
void CuiUpdate::UndoAll ( void )
{
	CeMap* pMap = CeMap::GetpMap();
	while ( m_NumUndo )
	{
		pMap->Undo();
		m_NumUndo--;
	}

	if ( m_pInfo ) m_pInfo->SetUpdateCount(0);
}
         */

        /*
//	@mfunc	Sets an undo marker and and updates dialog to
//			reflect this.
void CuiUpdate::SetUndoMarker ( void )
{
	CeMap::GetpMap()->SetUndoMarker();
	m_NumUndo++;
	if ( m_pInfo ) m_pInfo->SetUpdateCount(m_NumUndo);
}
         */

        internal CommandUI ActiveCommand
        {
            get { return m_Cmd; }
        }
    }
}
