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
using System.Drawing;

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
            NewTextForm dial = new NewTextForm(null);
            if (dial.ShowDialog() != DialogResult.OK)
            {
                dial.Dispose();
                DialFinish(null);
                return false;
            }

            // Pick up the entered text and its entity type
            m_NewText = dial.EnteredText;
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

        /// <summary>
        /// Handles mouse-move.
        /// </summary>
        /// <param name="p">The new position of the mouse</param>
        internal override void MouseMove(IPosition p)
        {
            // If we previously drew a text outline, erase it now.
            EraseRect();

            // Draw a rectangle representing the outline of the text.
            DrawRect(p);
        }

        /// <summary>
        /// Handles a mouse down event
        /// </summary>
        /// <param name="p">The position where the click occurred</param>
        /// <returns>True (always), indicating that something was done.</returns>
        internal override bool LButtonDown(IPosition p)
        {
            // Erase any text outline.
            EraseRect();

            // Add a new label.
            TextFeature label = AddNewLabel(p, null);

            // Draw it.
            if (label!=null)
                label.Draw(ActiveDisplay, Color.Black);

            // Tell the base class.
            OnLabelAdd();

            // Get the info for the next piece of text.
            GetLabelInfo();

            return true;
        }

        /// <summary>
        /// Creates any applicable context menu
        /// </summary>
        /// <returns>The context menu for this command.</returns>
        internal override ContextMenuStrip CreateContextMenu()
        {
            return new NewTextContextMenu(this);
        }

        /// <summary>
        /// Handles the context menu "Horizontal" menuitem.
        /// </summary>
        /// <param name="action">The action that initiated this method call</param>
        internal void ToggleHorizontal(IUserAction action)
        {
            // Toggle the horizontal text option.
            m_IsHorizontal = !m_IsHorizontal;

            // If text is currently horizontal, revert to any rotation
            // angle that applies when the command was constructed.

            double rot = m_Rotation;
            if (m_IsHorizontal)
                rot = 0.0;

            SetRotation(rot);
        }

        /// <summary>
        /// Is the "Horizontal" menuitem checked in this UI's context menu?
        /// </summary>
        internal bool IsHorizontalChecked
        {
            get { return m_IsHorizontal; }
        }

        /// <summary>
        /// Is the "Horizontal" menuitem enabled in this UI's context menu?
        /// </summary>
        internal bool IsHorizontalEnabled
        {
            get { return (Math.Abs(m_Rotation) > MathConstants.TINY); }
        }

        internal void SetSizeFactor(IUserAction action)
        {
            TextSizeAction tsa = (TextSizeAction)action;
        }

        /// <summary>
        /// Handles the context menu "Cancel" menuitem.
        /// </summary>
        /// <param name="action">The action that initiated this method call</param>
        internal void Cancel(IUserAction action)
        {
            DialFinish(null);
        }

        /*
LOGICAL CuiNewText::RButtonDown ( const CPoint& lpt ) {

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
*/

        /// <summary>
        /// Creates a new item of text in the map.
        /// </summary>
        /// <param name="posn">Reference position for the text.</param>
        /// <param name="oldLabel">An old label that's being replaced. Should always
        /// be NULL. Default=NULL. This appears only because of the abstract definition
        /// in AddLabelUI.</param>
        /// <returns>The feature that was added.</returns>
        internal override TextFeature AddNewLabel(IPosition posn, TextFeature oldLabel)
        {
            throw new Exception("NewTextUI.AddNewLabel");
        }
        /*
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
        */

        /// <summary>
        /// Update a previously added item of text.
        /// </summary>
        /// <param name="label">The previously added text.</param>
        /// <returns>TRUE if an update was made.</returns>
        internal override bool Update(TextFeature label)
        {
            // The label MUST be miscellaneous text.
            MiscText text = (label.TextGeometry as MiscText);
            if (text==null)
                throw new Exception("Can only update miscellaneous text.");

            // Display dialog to get info.
            NewTextForm dial = new NewTextForm(label);
            if (dial.ShowDialog() != DialogResult.OK)
            {
                dial.Dispose();
                return false;
            }

            // Confirm that the entity type has been specified.
            IEntity ent = dial.EntityType;
            if (ent==null)
            {
                MessageBox.Show("Text type must be specified.");
                return false;
            }

            // Confirm that the new text is defined.
            m_NewText = dial.EnteredText.Trim();
            if (m_NewText.Length==0)
            {
                MessageBox.Show("You cannot delete text this way.");
                return false;
            }

            // Erase the current text.
            //pText->Erase();

            // Set the new text (this removes the old text from the
            // spatial index, changes the text, and re-indexes).
            text.SetText(label, m_NewText);

            // Change the text's entity type.
            if (ent.Id != label.EntityType.Id)
                label.EntityType = ent;

            // Redraw the text.
            label.Draw(ActiveDisplay, Color.Black);
            return true;
        }
    }
}
