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
using System.Windows.Forms;

using Backsight.Editor.Forms;
using Backsight.Forms;
using Backsight.Environment;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="28-MAR-1999" was="CuiNewText"/>
    /// <summary>
    /// User interface for adding a new item of miscellaneous text.
    /// </summary>
    class NewTextUI : AddLabelUI, IDisposable
    {
        #region Class data

        /// <summary>
        /// The text that is being added.
        /// </summary>
        string m_NewText;

        /// <summary>
        /// Should the text be horizontal?
        /// </summary>
        bool m_IsHorizontal;

        /// <summary>
        /// The default rotation angle at the time when the command was created.
        /// </summary>
        double m_Rotation;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>NewTextUI</c>
        /// </summary>
        /// <param name="cc">The container for any dialogs</param>
        /// <param name="action">The action that initiated this command</param>
        internal NewTextUI(IControlContainer cc, IUserAction action)
            : base(cc, action)
        {
            m_NewText = String.Empty;

	        // Get the map's current rotation angle to see if we are
	        // adding horizontal text or not. Remember the value so that
	        // we can restore it when the command finishes.
        	m_Rotation = CadastralMapModel.Current.DefaultTextRotation;
	        m_IsHorizontal = (Math.Abs(m_Rotation) < MathConstants.TINY);
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
	        // If a rotation angle was defined when the command started,
	        // ensure it is still the map's default.
            // TODO: SS20080402 - I think this should be done elsewhere

	        if ( Math.Abs(m_Rotation) > MathConstants.TINY )
                CadastralMapModel.Current.DefaultTextRotation = m_Rotation;
        }

        #endregion

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
                msg += ("Labels are currently invisible. To add new text," + System.Environment.NewLine);
                msg += ("you must initially make it visible (see Edit-Preferences).");
                MessageBox.Show(msg);
                DialFinish(null);
                return false;
            }

            // Tell the view to un-highlight (we'll be doing our own).
            Controller.ClearSelection();

            // Get info for the first item of text.
            return GetLabelInfo();
        }

        internal override bool IsAutoPosition
        {
            get { return false; }
        }

        internal override TextFeature AddNewLabel(IPosition posn, TextFeature oldLabel)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal override bool Update(TextFeature label)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Gets the information relating to a single new item of text. It gets re-called
        /// after each piece of text has been positioned.
        /// </summary>
        /// <returns>True if info supplied. False if command is done.</returns>
        bool GetLabelInfo()
        {
            // Ensure the normal cursor is displayed
            SetNormalCursor();

            // Display dialog to get info.
            NewTextForm dial = new NewTextForm();
            if (dial.ShowDialog() != DialogResult.OK)
            {
                dial.Dispose();
                DialFinish(null);
                return false;
            }

            // Pick up the entered text and its entity type
            m_NewText = dial.GetText();
            IEntity ent = dial.EntityType;

            // All done with dialog
            dial.Dispose();

            // Confirm that the entity type has been specified.
            if (ent==null)
            {
                MessageBox.Show("Text type must be specified.");
                DialFinish(null);
                return false;
            }

            // Tell the base class about the entity type.
            base.Entity = ent;

            // Notify the base class of what's about to be added.
            if (!SetDimensions(m_NewText))
            {
                DialFinish(null);
                return false;
            }

            // Switch on the command cursor (we won't add anything until
            // the user left clicks).
            SetCommandCursor();
            return true;
        }
        /*		
//	@mfunc	Handle mouse-move.
//
//	@parm	The new position of the mouse, in logical units.
//
//////////////////////////////////////////////////////////////////////

void CuiNewText::MouseMove ( const CPoint& lpt ) {

	// If we previously drew a text outline, erase it now.
	EraseRect();

	// Get the position in ground units.
	CeVertex pos;
	GetpWnd()->LPToGround(lpt,&pos);

	// Draw a rectangle representing the outline of the text.
	DrawRect(pos);

} // end of MouseMove

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	React to selection of the OK button in the dialog.
//
//	@parm	The dialog window (needed because of definition of
//			pure virtual in CuiCommand). Not used.
//
//////////////////////////////////////////////////////////////////////

LOGICAL CuiNewText::DialFinish ( CWnd* pWnd ) {

	// Ensure any text outline has been erased.
	EraseRect();

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

void CuiNewText::LButtonDown ( const CPoint& lpt ) {

	// Erase any text outline.
	EraseRect();

	// Add a new label.
	CeVertex pos;
	GetpWnd()->LPToGround(lpt,&pos);
	CeLabel* pLabel = AddNewLabel(pos);

	// Draw it.
	if ( pLabel ) GetpWnd()->Draw(pLabel,COL_BLACK);

	// Tell the base class.
	OnLabelAdd();

	// Get the info for the next piece of text.
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

LOGICAL CuiNewText::RButtonDown ( const CPoint& lpt ) {

	// Display sub-menu shown while adding new labels, routing
	// any WM_COMMAND messages back to the window of the session.

	CMenu menu;
	menu.LoadMenu(IDR_CURSOR_MENU);

	// Get the sub-menu.
	CMenu* pMenu = menu.GetSubMenu(7);
	if ( !pMenu ) return FALSE;

	// Convert logical units into screen coordinates.
	CPoint point;
	LPToScreen(lpt,point);

	// Set or clear horizontal text option, depending on what
	// the current rotation is.

	if ( m_IsHorizontal )
		pMenu->CheckMenuItem(ID_HORIZONTAL,MF_CHECKED|MF_BYCOMMAND);
	else
		pMenu->CheckMenuItem(ID_HORIZONTAL,MF_UNCHECKED|MF_BYCOMMAND);

	// If there was no rotation to start with, disable the option.
	if ( fabs(m_Rotation)<TINY )
		pMenu->EnableMenuItem(ID_HORIZONTAL,MF_GRAYED|MF_BYCOMMAND);

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
//			of <mf CuiNewText::RButtonDown>.
//
//	@parm	The ID of the sub-command.
//
//	@rdesc	TRUE if sub-command was dispatched.
//
//////////////////////////////////////////////////////////////////////

LOGICAL CuiNewText::Dispatch ( const INT4 id ) {

	switch ( id ) {

	case ID_FINISH: {
		DialFinish(0);
		return TRUE;
	}
	
	case ID_HORIZONTAL: {

		// Toggle the horizontal text option.
		m_IsHorizontal = !m_IsHorizontal;

		// If text is currently horizontal, revert to any rotation
		// angle that applies when the command was constructed.

		FLOAT8 rot = m_Rotation;
		if ( m_IsHorizontal ) rot = 0.0;
		SetRotation(rot);

		return TRUE;
	}

	case ID_TEXT_500:
	case ID_TEXT_200:
	case ID_TEXT_150:
	case ID_TEXT_100:
	case ID_TEXT_75:
	case ID_TEXT_50:
	case ID_TEXT_25: { return SetSizeId(id); }

	} // end switch

	AfxMessageBox("CuiNewText::Dispatch\nUnexpected sub-command");
	return FALSE;

} // end of Dispatch

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Return the ID of the cursor for this command.
//
//////////////////////////////////////////////////////////////////////

INT4 CuiNewText::GetCursorId ( void ) const {

	return IDC_REVERSE_ARROW;
}

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Create a new item of text in the map.
//
//	@parm	Reference position for the text.
//	@parm	An old label that's being replaced. Should always
//			be NULL. Default=NULL. This appears only because
//			of the pure virtual definition in CuiAddLabel.
//
//	@rdesc	Pointer to the CeLabel feature that was added.
//
//////////////////////////////////////////////////////////////////////

CeLabel* CuiNewText::AddNewLabel ( const CeVertex& posn
								 , CeLabel* pOldLabel ) {

	CeMap* pMap = CeMap::GetpMap();

	// Create an undefined persistent operation.
	CeNewLabel* pSave = new ( os_database::of(pMap),
							  os_ts<CeNewLabel>::get() )
							  CeNewLabel();

	// Tell map a save is starting.
	pMap->SaveOp(pSave);

	// Execute the operation
	LOGICAL ok = pSave->Execute	((LPCTSTR)m_NewText
								,posn
								,(FLOAT4)GetHeight()
								,GetEntity());

	// Tell map the save has finished.
	pMap->SaveOp(pSave,ok);

	// If things failed, delete persistent memory for the op.
	if ( !ok ) {
		delete pSave;
		return 0;
	}

	// Remember that the map has been changed.
	SetChanged();

	return pSave->GetpLabel();

} // end of AddNewLabel

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Update a previously added item of text.
//
//	@parm	The previously added text.
//
//	@rdesc	TRUE if an update was made.
//
//////////////////////////////////////////////////////////////////////

#include "CeMiscText.h"

LOGICAL CuiNewText::Update ( CeLabel& label ) {

	// The label MUST be miscellaneous text.
	CeMiscText* pText =
		dynamic_cast<CeMiscText*>(label.GetpText());
	if ( !pText ) {
		ShowMessage("Can only update miscellaneous text.");
		return FALSE;
	}

	// Display dialog to get info.
	CdNewText dial(&label);
	if ( dial.DoModal()!=IDOK ) return FALSE;

	// Confirm that the entity type has been specified.
	const CeEntity* const pEnt = dial.GetpEntity();
	if ( !pEnt ) {
		ShowMessage("Text type must be specified.");
		return FALSE;
	}

	// Confirm that the new text is defined.
	m_NewText = dial.GetText();
	m_NewText.TrimLeft();
	m_NewText.TrimRight();

	if ( m_NewText.IsEmpty() ) {
		ShowMessage("You cannot delete text this way.");
		return FALSE;
	}

	// Erase the current text.
	pText->Erase();

	// And set the new text (this removes the old text from the
	// spatial index, changes the text, and re-indexes).
	pText->SetText(m_NewText);

	// Change the text's entity type.
	if ( label.GetpEntity()!=pEnt ) label.SetEntity(*pEnt);

	// Redraw the text.
	pText->Draw(COL_BLACK);

	return TRUE;

} // end of Update
         */
    }
}
