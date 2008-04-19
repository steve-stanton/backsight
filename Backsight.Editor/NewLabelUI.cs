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
using System.Data;
using System.Windows.Forms;

using Backsight.Environment;
using Backsight.Editor.Forms;
using Backsight.Forms;
using Backsight.Editor.Properties;
using Backsight.Geometry;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="03-JAN-1999" was="CuiNewLabel"/>
    /// <summary>
    /// User interface for adding new polygon labels
    /// </summary>
    class NewLabelUI : AddLabelUI
    {
        #region Class data

        /// <summary>
        /// The attribute schema for the polygons.
        /// </summary>
        ITable m_Schema;

        /// <summary>
        /// Annotation template (null means use IDs)
        /// </summary>
        ITemplate m_Template;

        /// <summary>
        /// The last row that was added (if any).
        /// </summary>
        DataRow m_LastRow;

        /// <summary>
        /// The polygon enclosing the last mouse position.
        /// </summary>
        Polygon m_Polygon;

        /// <summary>
        /// The ID (and entity type) of the label that is currently being positioned.
        /// </summary>
        IdHandle m_PolygonId;

        /// <summary>
        /// True if labels should be positioned automatically.
        /// </summary>
        bool m_IsAutoPos;

        /// <summary>
        /// True if orientation should be automatically determined
        /// </summary>
        bool m_IsAutoAngle;

        /// <summary>
        /// The current orientation line
        /// </summary>
        LineFeature m_Orient;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>NewLabelUI</c>
        /// </summary>
        /// <param name="cc">The container for any dialogs</param>
        /// <param name="action">The action that initiated this command</param>
        internal NewLabelUI(IControlContainer cc, IUserAction action)
            : base(cc, action)
        {
            m_Schema = null;
            m_Template = null;
            m_LastRow = null;
            m_Polygon = null;
            m_PolygonId = new IdHandle();
            m_IsAutoPos = Settings.Default.AutoPosition;
            m_IsAutoAngle = Settings.Default.AutoAngle;
            m_Orient = null;
        }

        #endregion

        internal override bool IsAutoPosition
        {
            get { return m_IsAutoPos; }
        }

        /// <summary>
        /// Starts the user interface (if any) for this command.
        /// </summary>
        /// <returns>True if command started ok.</returns>
        internal override bool Run()
        {
            // Disallow if labels are not currently drawn.
            if (!IsTextDrawn)
            {
                string msg = String.Empty;
                msg += ("Labels are currently invisible. To add new labels," + System.Environment.NewLine);
                msg += ("you must initially make them visible (see Edit-Preferences).");
                MessageBox.Show(msg);
                DialFinish(null);
                return false;
            }

            // Tell the view to un-highlight (we'll be doing our own).
            Controller.ClearSelection();

            // Display dialog to get info that will be common to all
            // the labels we will be adding.
            NewLabelForm dial = new NewLabelForm();
            if (dial.ShowDialog() != DialogResult.OK)
            {
                DialFinish(null);
                return false;
            }

            // Pick up the entered info.
            m_Schema = dial.Schema;
            m_Template = dial.Template;

            // Confirm that the entity type has been specified.
            IEntity ent = dial.Entity;
            if (ent == null)
            {
                MessageBox.Show("Polygon type must be specified.");
                DialFinish(null);
                return false;
            }

            // Tell the base class about the entity type.
            base.Entity = ent;

            // Get info for the first label.
            return GetLabelInfo();
        }

        /// <summary>
        /// Get the information relating to a single new label. This will be
        /// called after the attribute schema, annotation template and polygon
        /// entity type has been defined. It gets re-called after each label has
        /// been positioned.
        /// </summary>
        /// <returns>True if info supplied. False if command is done.</returns>
        bool GetLabelInfo()
        {
            // Ensure the normal cursor is displayed
            SetNormalCursor();

            // Ask the base class to return the entity type for the labels.
	        IEntity ent = base.Entity;

            // Get the polygon ID ...

            // If we are auto-numbering, just get the next ID. Otherwise
            // ask for it, showing the default.

            if (CadastralMapModel.Current.IsAutoNumber)
                m_PolygonId.ReserveId(ent, 0);
            else
            {
                GetIdForm dial = new GetIdForm(ent, m_PolygonId);
                if (dial.ShowDialog() != DialogResult.OK)
                {
                    DialFinish(null);
                    return false;
                }
            }

            // Start by assuming the text is the key.
            string str = m_PolygonId.FormattedKey;

            // Get any attributes.
            if (m_Schema != null)
            {
                if (!GetAttributes() || m_LastRow == null)
                {
                    DialFinish(null);
                    return false;
                }

                // If an annotation template has been specified, use that
                // to get the text from the row (pass in the ID in case the
                // template includes a reference to the key).
                throw new NotImplementedException("NewLabelUI.GetLabelInfo");
                /*
		            if ( m_pTemplate ) {
			            CString text;
			            m_pTemplate->GetText(*m_pLastRow,(LPCTSTR)str,text);
			            str = text;
		            }
                 */
            }

            // Tell the base class.
            if (!SetDimensions(str))
            {
                DialFinish(null);
                return false;
            }

            // Switch on the command cursor (we won't add anything until
            // the user left clicks inside a polygon).
            SetCommandCursor();
            return true;
        }

        /// <summary>
        /// Handles mouse-move.
        /// </summary>
        /// <param name="pos">The new position of the mouse</param>
        internal override void MouseMove(IPosition pos)
        {
            // If we previously drew a text outline, erase it now.
            EraseRect();

            // Find the polygon (if any) that encloses the mouse position.
            CadastralMapModel map = CadastralMapModel.Current;
            ISpatialIndex index = map.Index;
            IPointGeometry pg = PointGeometry.Create(pos);
            Polygon enc = new FindPointContainerQuery(index, pg).Result;

            // If it's different from what we previously had, remember the
            // new enclosing polygon.
            if (!Object.ReferenceEquals(enc, m_Polygon))
            {
                // If we had something before, and we filled it, erase
                // the fill now.
                DrawPolygon(false);

                // Remember the polygon we're now enclosed by (if any).
                m_Polygon = enc;

                // Draw the new polygon.
                DrawPolygon(true);

                // See if a new orientation applies
                CheckOrientation(pg);

                // If the enclosing polygon does not have a label, use the
                // standard cursor and fill the polygon. Otherwise use the
                // gray cursor.
                SetCommandCursor();
            }

            // Draw a rectangle representing the outline of the text.
            DrawRect(pos);
        }

        /*		
//	@mfunc	React to selection of the OK button in the dialog.
//
//	@parm	The dialog window (needed because of definition of
//			pure virtual in CuiCommand). Not used.
//
//////////////////////////////////////////////////////////////////////

LOGICAL CuiNewLabel::DialFinish ( CWnd* pWnd ) {

	// Ensure any text outline has been erased.
	EraseRect();

	// Clear any polygon fill
	DrawPolygon(FALSE);

	// And un-highlight any orientation line
	if ( m_pOrient )
	{
		m_pOrient->UnHighlight();
		m_pOrient = 0;
	}

	// If the last row we created was not associated with a
	// label, get rid of it now.
	if ( m_pLastRow && !m_pLastRow->GetpId() ) {
		delete m_pLastRow;
		m_pLastRow = 0;
	}

	// Get the base class to finish up.
	return FinishCommand();

} // end of DialFinish

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Handle left mouse click.
//
//	@parm	The position where the left-click occurred, in logical
//			units.
//
//////////////////////////////////////////////////////////////////////

void CuiNewLabel::LButtonDown ( const CPoint& lpt ) {

	// Make sure the enclosing polygon refers to the point we
	// have been supplied.
	MouseMove(lpt);

	// Exit the command if the user left-clicked in an area that
	// does not have a polygon (perhaps the user doesn't know that
	// you exit via the right-click menu).
	if ( !m_pPolygon ) {
		DialFinish(0);
		return;
	}

	// Issue error if the enclosing polygon already has a label (this
	// was supposed to be pretty obvious, given that the cursor is
	// grey, and the polygon is not filled in that case).
	if ( !IsValidPolygon() ) {
		AfxMessageBox("Polygon already has a label");
		SetCommandCursor();
		return;
	}

	// Erase any text outline.
	EraseRect();

	// Erase any highlighting.
	DrawPolygon(FALSE);

	if ( m_pOrient )
	{
		m_pOrient->UnHighlight();
		m_pOrient = 0;
	}

	// Does the polygon actually contain a label that shared
	// with a base layer?
	const CeTheme& curtheme = GetActiveTheme();
	CeLabel* pOldLabel = m_pPolygon->FindLabel(curtheme);

	// Add a new label.

	CeLabel* pLabel = 0;

	if ( m_IsAutoPos )
		pLabel = AddNewLabel(pOldLabel);
	else {
		CeVertex pos;
		GetpWnd()->LPToGround(lpt,&pos);
		pLabel = AddNewLabel(pos,pOldLabel);
	}

	// Draw it (do it below)
	//if ( pLabel ) GetpWnd()->Draw(pLabel,COL_BLACK);

	// Tell the base class (resets the last known position, used
	// to erase labels during dragging). 
	OnLabelAdd();

	// Undefine the enclosing polygon pointer so that the cursor
	// will become grey as the user moves the mouse out of the
	// polygon.
	m_pPolygon = 0;

	// Draw it. Note that if the new label replaces an old label,
	// the AddNewLabel call explicitly erases it, but something
	// from within a subsequent CleanEdit call re-draws it in
	// red. To get around that, ensure it's erased again here.
	// ...changes made elsewhere may now make this redundant

	if ( pLabel )
	{
		if ( pOldLabel ) pOldLabel->Erase();
		GetpWnd()->Draw(pLabel,COL_BLACK);
	}

	// Get the info for the next label.
	GetLabelInfo();

} // end of LButtonDown					   } 

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Handle right mouse click.
//
//	@parm	The position where the right-click occurred, in
//			logical units.
//
//	@rdesc	TRUE (always)
//
//////////////////////////////////////////////////////////////////////

LOGICAL CuiNewLabel::RButtonDown ( const CPoint& lpt ) {

	// Display sub-menu shown while adding new labels, routing
	// any WM_COMMAND messages back to the window of the session.

	CMenu menu;
	menu.LoadMenu(IDR_CURSOR_MENU);

	// Get the sub-menu.
	CMenu* pMenu = menu.GetSubMenu(3);
	if ( !pMenu ) return FALSE;

	// Convert logical units into screen coordinates.
	CPoint point;
	LPToScreen(lpt,point);

	// If labels should be auto-positioned, set check mark.
	if ( m_IsAutoPos )
		pMenu->CheckMenuItem(ID_AUTO_POSITION,MF_CHECKED|MF_BYCOMMAND);

	// If the orientation should adjust automatically, set check mark
	if ( m_IsAutoAngle )
		pMenu->CheckMenuItem(ID_AUTO_ANGLE,MF_CHECKED|MF_BYCOMMAND);

	// And the current size factor.
	INT4 id = GetSizeId();
	if ( id ) pMenu->CheckMenuItem(id,MF_CHECKED|MF_BYCOMMAND);

	pMenu->TrackPopupMenu(TPM_LEFTALIGN|TPM_RIGHTBUTTON,
						  point.x,point.y,GetpWnd());

	return TRUE;

} // end of RButtonDown

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Receive a sub-command. These actually get sent down
//			via the view class, which is reacting to the handling
//			of <mf CuiNewLabel::RButtonDown>.
//
//	@parm	The ID of the sub-command.
//
//	@rdesc	TRUE if sub-command was dispatched.
//
//////////////////////////////////////////////////////////////////////

LOGICAL CuiNewLabel::Dispatch ( const INT4 id ) {

	switch ( id ) {

	case ID_FINISH: {
		DialFinish(0);
		return TRUE;
	}

	case ID_AUTO_POSITION: {

		// Ensure any text outline has been erased.
		EraseRect();

		//m_LastPos.Reset();
		OnLabelAdd();	// Bit of a hack

		// Toggle the auto-positioning option.
		m_IsAutoPos = !m_IsAutoPos;
		SetBooleanSetting("Auto-Position",m_IsAutoPos);

		// If we are now auto-positioning, erase any rectangle
		// that we had.

		// Set the appropriate command cursor.
		SetCommandCursor();

		return TRUE;
	}

	case ID_AUTO_ANGLE:
	{
		m_IsAutoAngle = !m_IsAutoAngle;
		SetBooleanSetting("Auto-Angle",m_IsAutoAngle);

		if ( !m_IsAutoAngle && m_pOrient )
		{
			m_pOrient->UnHighlight();
			m_pOrient = 0;
		}

		return TRUE;
	}

	case ID_TEXT_500:
	case ID_TEXT_200:
	case ID_TEXT_150:
	case ID_TEXT_100:
	case ID_TEXT_75:
	case ID_TEXT_50:
	case ID_TEXT_25: {

		SetSizeId(id);
		SetCommandCursor();
		return TRUE;
	}

	} // end switch

	AfxMessageBox("CuiNewLabel::Dispatch\nUnexpected sub-command");
	return FALSE;

} // end of Dispatch

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Return the ID of the cursor for this command.
//
//////////////////////////////////////////////////////////////////////

INT4 CuiNewLabel::GetCursorId ( void ) const {

	if ( IsValidPolygon() ) {

		// If the polygon actually contains a label that is
		// drawn on the current editing theme, take this
		// opportunity to draw it in grey.
		const CeTheme& curtheme = GetActiveTheme();
		const CeLabel* const pLabel = m_pPolygon->FindLabel(curtheme);
		if ( pLabel ) pLabel->DrawThis(COL_GREY);

		if ( m_IsAutoPos )
			return IDC_WAND;
		else
			return IDC_REVERSE_ARROW;
	}
	else {
		if ( m_IsAutoPos )
			return IDC_GREY_WAND;
		else
			return IDC_GREY_REV_ARROW;
	}

} // end of GetCursorId
        */

        /// <summary>
        /// Draws (or erases) the polygon that the mouse currently sits in.
        /// </summary>
        /// <param name="draw">Draw the polygon? [Default was true]</param>
        /// <returns>True if a polygon was drawn or erased.</returns>
        bool DrawPolygon(bool draw)
        {
            // Return if there is no valid polygon.
            if (!IsValidPolygon())
                return false;

            if (draw)
            {
                m_Polygon.Render(ActiveDisplay, new DrawStyle());
                if (m_Orient != null)
                    m_Orient.Render(ActiveDisplay, new HighlightStyle());
            }
            else
                ErasePainting();

            return true;
        }

        /// <summary>
        /// Do we currently have a valid polygon for adding a new label?
        /// </summary>
        /// <returns>True if <c>m_Polygon</c> is defined and valid.</returns>
        bool IsValidPolygon()
        {
            // Return if there is no polygon to draw.
            if (m_Polygon==null)
                return false;

            // Try to find an existing label inside the polygon.
            TextFeature label = m_Polygon.Label;

            // If we got something, the normal course of action is
            // to disallow addition of another label. However, if the
            // current editing layer is derived from the base layer
            // of the label, we'll allow it ... UNLESS the existing
            // label is key text and the replacement would be key
            // text as well.

            if (label == null)
                return true;

            //if (m_Template==null && label.TextGeometry is KeyText

            return false;
        }
        /*

	if ( pLabel ) {
		if ( m_pTemplate==0 &&
			 pLabel->GetpText()->GetType() == PTY_KTEXT ) return FALSE;
		const CeTheme* const pBase = pLabel->GetBaseTheme();
		return (pBase && curtheme.IsDerivedFrom(*pBase));
	}

} // end of IsValidPolygon
        */

        /*
//////////////////////////////////////////////////////////////////////
//
//	@private
//
//	@mfunc	Create a new polygon label.
//
//	@parm	An old shared label that is being replaced.
//
//	@rdesc	Pointer to the CeLabel feature that was added.
//
//////////////////////////////////////////////////////////////////////

CeLabel* CuiNewLabel::AddNewLabel ( CeLabel* pOldLabel ) {

	assert(m_pPolygon);

	// Get the dimensions of the text.
	FLOAT8 width = GetWidth();
	FLOAT8 height = GetHeight();

	// Get a position for the annotation.
	CeVertex pos;
	if ( !m_pPolygon->GetLabelPosition(width,height,pos) ) return 0;

	// Add the label (assumes key label for now).
	return AddNewLabel(pos,pOldLabel);

} // end of AddNewLabel
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="posn"></param>
        /// <param name="oldLabel"></param>
        /// <returns></returns>
        internal override TextFeature AddNewLabel(IPosition posn, TextFeature oldLabel)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /*
//	@private
//
//	@mfunc	Create a new polygon label in the map.
//
//	@parm	Reference position for the label.
//	@parm	An old shared label that is being replaced.
//
//	@rdesc	Pointer to the CeLabel feature that was added.

CeLabel* CuiNewLabel::AddNewLabel ( const CeVertex& posn
								  , CeLabel* pOldLabel ) {

	assert(m_pPolygon);
	CeMap* pMap = CeMap::GetpMap();
	CeLabel* pNewLabel=0;

	// If we are replacing another label, use that label's ID
	// and ensure that any ID we reserved has been released.
	if ( pOldLabel ) {

		// Get the entity type that the ID handle was assigned
		// before freeing the handle.
		const CeEntity* const pEnt = m_PolygonId.GetpEntity();
		m_PolygonId.FreeId();
		assert(pEnt);

		// Create an undefined persistent operation.
		CeNewLabelEx* pSave = new ( os_database::of(pMap),
								    os_ts<CeNewLabelEx>::get() )
								    CeNewLabelEx();

		// Tell map a save is starting.
		pMap->SaveOp(pSave);

		// Execute the operation
		LOGICAL ok;
		if ( m_pTemplate && m_pLastRow ) {

			ok = pSave->Execute( posn
							   , GetHeight()
							   , *pEnt
							   , *m_pLastRow
							   , *m_pTemplate
							   , *m_pPolygon
							   , *pOldLabel );

			// Confirm that the row got cross-referenced to an ID (not
			// sure what the above ends up doing).
			if ( ok && !m_pLastRow->GetpId() ) {
				AfxMessageBox("Attributes were not attached to an ID");
				ok = FALSE;
			}

			// Ensure the row is cross-referenced to the op.
			if ( ok ) m_pLastRow->AddOp(*pSave);
		}
		else 
			ok = pSave->Execute	( posn
								, GetHeight()
								, *pEnt
								, *m_pPolygon
								, *pOldLabel );

		// Tell map the save has finished.
		pMap->SaveOp(pSave,ok);

		// If things failed, delete persistent memory for the op.
		if ( !ok ) {
			delete pSave;
			return 0;
		}

		// Pick up the address of the new label.
		pNewLabel = pSave->GetpLabel();
	}
	else {

		// Not replacing an existing label, so confirm that
		// the polygon ID has been reserved.
		
		if ( !m_PolygonId.IsReserved() ) {
			AfxMessageBox("CuiNewLabel::AddNewLabel\nNo polygon ID");
			return 0;
		}

		// Check whether another label exists in the specified
		// polygon, but on a layer that is derived from the current
		// editing theme. If so, note the entity type and free the
		// supplied ID.

		CeLabel* pOldLabel = m_pPolygon->GetBaseLabel();
		const CeEntity* pEnt=0;

		if ( pOldLabel ) {
			pEnt = m_PolygonId.GetpEntity();
			m_PolygonId.FreeId();
		}

		// Create an undefined persistent operation.
		CeNewLabel* pSave = new ( os_database::of(pMap),
								  os_ts<CeNewLabel>::get() )
								  CeNewLabel();

		// Tell map a save is starting.
		pMap->SaveOp(pSave);

		// Execute the operation
		LOGICAL ok;
		if ( m_pTemplate && m_pLastRow ) {

			if ( pOldLabel )
				ok = pSave->Execute( posn
								   , GetHeight()
								   , *pEnt
								   , *m_pLastRow
								   , *m_pTemplate
								   , *m_pPolygon
								   , *pOldLabel );
			else
				ok = pSave->Execute( posn
								   , GetHeight()
								   , m_PolygonId
								   , *m_pLastRow
								   , *m_pTemplate
								   , m_pPolygon );

			// Confirm that the row got cross-referenced to an ID (not
			// sure what the above ends up doing).
			if ( ok && !m_pLastRow->GetpId() ) {
				AfxMessageBox("Attributes were not attached to an ID");
				ok = FALSE;
			}
		}
		else {

			if ( pOldLabel )
				ok = pSave->Execute(posn,GetHeight(),*pEnt,*m_pPolygon,*pOldLabel);
			else
				ok = pSave->Execute(posn,GetHeight(),m_PolygonId,m_pPolygon);
		}

		// Tell map the save has finished.
		pMap->SaveOp(pSave,ok);

		// If things failed, delete persistent memory for the op.
		if ( !ok ) {
			delete pSave;
			return 0;
		}

		// Pick up the address of the new label.
		pNewLabel = pSave->GetpLabel();
	}

	// If a new label has actually been added
	if ( pNewLabel ) {

		// Remember that the map has been changed.
		SetChanged();

		// If a row was created, but we only created key text,
		// make sure the label's ID has been cross-referenced
		// to the row (the call to CeRow::SetId makes pointers
		// both ways).

		// @devnote This was introduced 20-OCT-99. While this
		// was previously done for CeRowText labels, it was NOT
		// done for CeKeyText labels. Hence, if you added a new
		// label to the Assessment layer (which currently has
		// no row text templates), you'd get key text, but the
		// attributes you entered would have been stored, but
		// would not be referenced by the ID. So, if you went
		// to update the polygon's attributes, it said there
		// were no attributes.

		// Note that key text is NOT referenced to the creating
		// op. This is inconsistent with the way it has always
		// worked for row text ... row-text labels don't REALLY
		// need to be cross-referenced to the op, but it makes
		// sense if you think of the row as being "used" by the
		// row text.

		if ( m_pLastRow ) {
			CeFeatureId* pFid = pNewLabel->GetpId();
			assert(pFid);
			if ( pFid ) m_pLastRow->SetId(*pFid);
		}
	}

	return pNewLabel;

} // end of AddNewLabel
        */


        /// <summary>
        /// Gets the attributes for a new label.
        /// </summary>
        /// <returns>True if attributes entered OK.</returns>
        bool GetAttributes()
        {
            // There HAS to be a schema.
            if (m_Schema==null)
                return false;


            MessageBox.Show("Attribute data entry is not currently implemented");
            return false;
        }
        /*
	// Allocate a blob of memory to hold the attribute data.
	CHARS* pAttrData = new CHARS[m_pSchema->GetSchemaMemSize()];

	// If we previously created a row, make a copy of it's data.
	// Otherwise define default attributes.

	if ( m_pLastRow ) {

		m_pLastRow->SaveDataCopy(pAttrData);

		// If it didn't get associated with a label (somehow), get
		// rid of it now.
		if ( !m_pLastRow->GetpId() ) {
			delete m_pLastRow;
			m_pLastRow = 0;
		}
	}
	else
		m_pSchema->MakeDefaultBlob(pAttrData);

	// Create a new row (without an ID).
	m_pLastRow = new ( os_database::of(m_pSchema)
				     , os_ts<CeRow>::get() )
					   CeRow(*m_pSchema,pAttrData);

	// Don't need our copy of the attribute data any more.
	delete [] pAttrData;
	pAttrData = 0;

	// Construct 'container dialog' to hold controls
	CdRowDialog ParentDialog(TRUE,FALSE);

	// Next construct empty dynamic dialog to have controls added to
	CeDynamicDialog DynamicDialog(&ParentDialog,0,14,4000,10,5);
		
	// Set the title for the dialog window
	DynamicDialog.SetWindowTitle("Specify Attributes for Next Label");

	// Set the dynamic dialog in the container dialog so that
	// OnSize messages get handled.
	ParentDialog.SetDynamicDialog(&DynamicDialog);

	// Construct the object for displaying a row of attributes.
	CeRowDisplay DisplayObject(&DynamicDialog,TRUE,m_pLastRow);

	// Set the object used to construct the controls
	ParentDialog.SetDisplayObject(&DisplayObject);

	// Present modal dialog.
	if ( ParentDialog.DoModal()!=IDOK ) return FALSE;
	
	// if the user requested setting of attributes and some were changed, then they
	// are set in the CeRow object

	if ( DynamicDialog.IsChanged() ) {
		m_pLastRow->OnPreChange();
		LOGICAL retval = DisplayObject.UpdateRow(TRUE);
		m_pLastRow->OnPostChange();
		return retval;
	}

	return TRUE;

} // end of GetAttributes
        */

        internal override bool Update(TextFeature label)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /*
//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Update a previously added polygon label.
//
//	@parm	The previously added label.
//
//	@rdesc	TRUE if an update was made.
//
//////////////////////////////////////////////////////////////////////

LOGICAL CuiNewLabel::Update ( CeLabel& label ) {

	// For the time being, "update" means updating the attributes
	// associated with the label. There's no way to update the
	// entity type, schema, or annotation template (apart from
	// deleting an existing label, and re-adding a new one).

	// The update of attributes is currently handled via the
	// view class.

	return FALSE;

} // end of Update
        */

        /// <summary>
        /// Sees whether the orientation of the new text should be altered. This
        /// occurs if the auto-angle capability is enabled, and the specified
        /// position is close to any visible line.
        /// </summary>
        /// <param name="refpos">The current mouse position (reference position
        /// for the new label)</param>
        /// <returns>True if the orientation angle got changed.</returns>
        bool CheckOrientation(IPointGeometry refpos)
        {
            // Return if the auto-angle function is disabled.
            if (!m_IsAutoAngle)
                return false;

            // Try to get an orientation line
            LineFeature orient = GetOrientation(refpos);
            if (!Object.ReferenceEquals(orient, m_Orient))
                ErasePainting();

            m_Orient = null;
            if (orient == null)
                return false;

            // Locate the closest point on the line (we SHOULD find it,
            // but if we don't, just bail out)
            ISpatialDisplay display = ActiveDisplay;
            ILength tol = new Length(0.002 * display.MapScale);
            IPosition closest = orient.LineGeometry.GetClosest(refpos, tol);
            if (closest == null)
                return false;

            m_Orient = orient;
            if (m_Orient==null)
                return false;

            // Highlight the new orientation line
            m_Orient.Render(display, new HighlightStyle());

            // Get the rotation angle
            IPointGeometry cg = PointGeometry.Create(closest);
            double rot = m_Orient.LineGeometry.GetRotation(cg);
            SetRotation(rot);
            return true;
        }

        /// <summary>
        /// Sees whether the orientation of the new text should be altered. This
        /// occurs if the auto-orient capability is enabled, and the specified
        /// position is close to any visible lne.
        /// </summary>
        /// <param name="posn">The position to use for making the check.</param>
        /// <returns></returns>
        LineFeature GetOrientation(IPointGeometry posn)
        {
            // The ground tolerance is 2mm at the draw scale.
            ISpatialDisplay display = ActiveDisplay;
            double tol = 0.002 * display.MapScale;

            // If we previously selected something, see if the search point
            // lies within tolerance. If so, there's no change.
            if (m_Orient != null)
            {
                double dist = m_Orient.Distance(posn).Meters;
                if (dist < tol)
                    return m_Orient;
            }

            // Get the map to find the closest line
            CadastralMapModel map = CadastralMapModel.Current;
            ISpatialIndex index = map.Index;
            return (index.QueryClosest(posn, new Length(tol), SpatialType.Line) as LineFeature);
        }
    }
}
