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

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" was="CdGetPoint" />
    /// <summary>
    /// Dialog for getting the user to specify a point (either by entering the
    /// ID, or pointing at the map). Not as general-purpose as it sounds, since
    /// it is exclusively used by the <see cref="PathUI"/> class.
    /// </summary>
    partial class GetPointForm : Form
    {
        #region Class data

        /// <summary>
        /// The parent command.
        /// </summary>
        readonly PathUI m_Parent;

        /// <summary>
        /// The title for the window.
        /// </summary>
        readonly string m_Title;

        /// <summary>
        /// The color hint for the point.
        /// </summary>
        readonly Color m_Color;

        /// <summary>
        /// The selected point.
        /// </summary>
        PointFeature m_Point;

        /// <summary>
        /// Was the point selected by pointing at the map?
        /// </summary>
        bool m_IsPointed;

        /// <summary>
        /// Should the "back" button be enabled?
        /// </summary>
        bool m_IsBackEnabled;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>GetPointForm</c>
        /// </summary>
        /// <param name="cmd">The parent command.</param>
        /// <param name="title">The title for the window.</param>
        /// <param name="col">The color hint for the point.</param>
        /// <param name="enableBack">Should the "back" button be enabled?</param>
        internal GetPointForm(PathUI cmd, string title, Color col, bool enableBack)
        {
            InitializeComponent();

            m_Point = null;
            m_Color = col;
            m_Title = title;
            m_IsPointed = false;
            m_IsBackEnabled = enableBack;
        }

        #endregion

        /*
public:
	virtual	void		OnSelectPoint	( CePoint* pPoint
										, const LOGICAL movenext=TRUE );
         */

        /// <summary>
        /// The selected point.
        /// </summary>
        internal PointFeature Point
        {
            get { return m_Point; }
        }

        private void GetPointForm_Shown(object sender, EventArgs e)
        {
            // Display at top left corner of the screen.
            this.Location = new Point(0, 0);

            // Display the desired title.
            this.Text = m_Title;

            // Disable the "back" button if necessary.
            backButton.Enabled = m_IsBackEnabled;
        }
        /*
void CdGetPoint::OnBack() 
{
	m_Parent.OnPointBack();
}

void CdGetPoint::OnCancel() 
{
	m_Parent.OnPointCancel();
}

void CdGetPoint::OnOK() 
{
	// Confirm that we have a point!
	if ( !m_pPoint ) {
		AfxMessageBox("You must select a point.");
		GetDlgItem(IDC_POINT)->SetFocus();
		return;
	}
	
	m_Parent.OnPointNext();
}

void CdGetPoint::OnChangePoint() 
{
	// If the field is now empty, allow the user to type in an ID.
	if ( IsFieldEmpty(IDC_POINT) ) {
		SetNormalColour();
		m_IsPointed = FALSE;
		m_pPoint = 0;
	}
}

void CdGetPoint::OnDrawItem(int nIDCtl, LPDRAWITEMSTRUCT lpDrawItemStruct) 
{
	if ( nIDCtl==IDC_COL ) SetButtonColour();
}

private void CdGetPoint::SetButtonColour ( void ) const {

	CButton* pButton = (CButton*)GetDlgItem(IDC_COL);
	CClientDC dc(pButton);
	CRect crect;
	pButton->GetClientRect(&crect);
	CBrush brush(m_Colour);
	dc.SelectObject(&brush);
	dc.Rectangle(crect);

} // end of SetButtonColour

void CdGetPoint::OnKillfocusPoint() 
{
	// Just return if the user specified the point by pointing.
	if ( m_IsPointed ) return;

	// Return if the field is empty.
	if ( IsFieldEmpty(IDC_POINT) ) return;

	// Get address of the edit box.
	CEdit* pEdit = (CEdit*)GetDlgItem(IDC_POINT);

	// Parse the ID value.
	UINT4 idnum;
	if ( !ReadUINT4(pEdit,idnum) ) {
		AfxMessageBox("Invalid point ID");
		pEdit->SetFocus();
		return;
	}

	// Ask the map to locate the address of the specified point.
	const CeKey key(idnum);
	m_pPoint = CeMap::GetpMap()->GetpPoint(key);
	if ( !m_pPoint ) {
		AfxMessageBox("No point with specified ID.");
		pEdit->SetFocus();
		return;
	}

	// Tell the parent dialog we're done.
	m_Parent.OnPointNext();
	
} // end of OnKillfocusPoint

/////////////////////////////////////////////////////////////////////////////
//
//	@mfunc Set normal colour for currently select point (if any).
//
/////////////////////////////////////////////////////////////////////////////

private void CdGetPoint::SetNormalColour ( void ) const {

	// Ask the point to draw itself.
	if ( m_pPoint ) m_pPoint->DrawThis();

} // end of SetNormalColour

/////////////////////////////////////////////////////////////////////////////
//
//	@mfunc Set colour for the currently selected point.
//
//	@parm The point to draw.
//
/////////////////////////////////////////////////////////////////////////////

private void CdGetPoint::SetColour ( void ) const {

	// Draw the point in the correct colour.
	if ( m_pPoint ) m_pPoint->DrawThis(m_Colour);

} // end of SetColour

void CdGetPoint::OnSelectPoint ( CePoint* pPoint
							   , const LOGICAL movenext ) {

	// Return if point is not defined.
	if ( !pPoint ) return;

	// Ensure that any previously selected point reverts
	// to its normal colour.
	SetNormalColour();

	// Remember the new point.
	m_pPoint = pPoint;
	m_IsPointed = TRUE;

	// Set the colour of the point.
	SetColour();

	// Display the point's ID.
	GetDlgItem(IDC_POINT)->SetWindowText(m_pPoint->FormatKey());

	// Tell the command that's running this dialog to move on.
	if ( movenext ) m_Parent.OnPointNext();

} // end of OnSelectPoint
				   
/////////////////////////////////////////////////////////////////////////////
//
//	@mfunc Check if a field is empty. If it contains white space characters,
//	it is considered to be empty.
//
/////////////////////////////////////////////////////////////////////////////

private LOGICAL CdGetPoint::IsFieldEmpty ( const UINT idd ) const {

	// Say the field is empty if the field ID is unknown.
	CWnd* pWnd = GetDlgItem(idd);
	if ( !pWnd ) return TRUE;

	// See if field is empty.
	return ::IsFieldEmpty(pWnd);

} // end of IsFieldEmpty
				   
/////////////////////////////////////////////////////////////////////////////
//
//	@mfunc	Do any painting that this dialog does.
//
/////////////////////////////////////////////////////////////////////////////

void CdGetPoint::Paint ( void ) const {

	SetColour();

} // end of Paint
         */
    }
}