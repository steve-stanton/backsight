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
using System.Windows.Forms;

using Backsight.Editor.Operations;


namespace Backsight.Editor.Forms
{
    /// <summary>
    /// Dialog for the <see cref="NewCircleUI"/>
    /// </summary>
    partial class NewCircleForm : Form
    {
        #region Class data

        /// <summary>
        /// The command that is running this dialog.
        /// </summary>
        readonly CommandUI m_Cmd;

        /// <summary>
        /// The circle (if any) that is being updated.
        /// </summary>
        LineFeature m_Update;

        /// <summary>
        /// The control that has the focus.
        /// </summary>
        Control m_Focus;

        /// <summary>
        /// A previous operation that was recalled (always null if doing
        /// an update).
        /// </summary>
        NewCircleOperation m_Recall;

        // For the operation ...

        /// <summary>
        /// Point at the center of the circle.
        /// </summary>
        PointFeature m_Center;

        /// <summary>
        /// Observed distance (either a <see cref="Distance"/> object,
        /// or an <see cref="OffsetPoint"/>).
        /// </summary>
        Observation m_Radius; // was m_pRadius

        // Preview related ...

        /// <summary>
        /// Currently displayed circle.
        /// </summary>
        Circle m_Circle;

        /// <summary>
        /// Point used to specify distance.
        /// </summary>
        PointFeature m_RadiusPoint;

        /// <summary>
        /// Entered distance value (if specified that way).
        /// </summary>
        Distance m_RadiusDistance; // was m_Radius

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a <c>NewCircleForm</c> for a brand new circle.
        /// </summary>
        /// <param name="cmd">The command creating this dialog</param>
        internal NewCircleForm(NewCircleUI cmd)
            : this(cmd, null)
        {
        }

        /// <summary>
        /// Creates a <c>NewCircleForm</c> for a circle, based on a previously
        /// defined circle.
        /// </summary>
        /// <param name="cmd">The command creating this dialog</param>
        /// <param name="recall">The editing operation that's being recalled (null
        /// if not doing a recall)</param>
        internal NewCircleForm(NewCircleUI cmd, Operation recall)
        {
            InitializeComponent();

            m_Cmd = cmd;
            m_Recall = (NewCircleOperation)recall;

            m_Center = null;
            m_Radius = null;
            m_Circle = null;
            m_RadiusPoint = null;
            m_RadiusDistance = null;
            m_Focus = null;

        }

        #endregion

        private void NewCircleForm_Shown(object sender, EventArgs e)
        {
            // If we are updating a circle that was previously created,
            // load the original info.

            if (m_Recall == null)
                ShowUpdate();
            else
                InitOp(m_Recall, false); // not an update
        }

        /// <summary>
        /// Point at the center of the circle.
        /// </summary>
        internal PointFeature Center
        {
            get { return m_Center; }
        }

        /// <summary>
        /// Observed distance (either a <see cref="Distance"/> object,
        /// or an <see cref="OffsetPoint"/>).
        /// </summary>
        internal Observation Radius
        {
            get { return m_Radius; }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            m_Cmd.DialAbort(this);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (m_Center != null && m_Radius != null)
                m_Cmd.DialFinish(this);
            else
            {
                if (m_Center==null)
                    MessageBox.Show("Center point has not been specified.");
                else
                    MessageBox.Show("Radius has not been specified.");
            }
        }

        /*
public:
	virtual	void			OnSelectPoint		( const CePoint* const pPoint );
	virtual	void			Paint				( const CePoint* const pPoint );

protected:
	virtual BOOL OnInitDialog();
	afx_msg void OnChangeRadius();
	afx_msg void OnSetfocusCentre();
	afx_msg void OnSetfocusRadius();
	afx_msg void OnChangeCentre();
	afx_msg void OnDestroy();
	afx_msg void OnKillfocusRadius();
	afx_msg void OnKillfocusCentre();

private:
	virtual	void			OnChange		( void );
	virtual	LOGICAL			ParseRadius		( void );
	virtual	void			PaintAll		( const LOGICAL draw=TRUE ) const;
	virtual	void			SetNormalColour	( const CePoint* const pPoint ) const;
	virtual	CePoint*		GetPoint		( const INT4 id );
	virtual	void			ShowUpdate		( void );
	virtual	CeNewCircle*	GetUpdateOp		( void ) const;
	virtual void			InitOp			( const CeNewCircle& op
											, const LOGICAL isUpdate );
         */

        /*
void CdCircle::OnSetfocusCentre ( void ) { m_Focus = IDC_CENTRE; }

void CdCircle::OnChangeCentre ( void ) {

	// If the field is now empty, ensure that the centre point
	// is undefined.

	CEdit* pEdit = (CEdit*)GetDlgItem(IDC_CENTRE);

	if ( IsFieldEmpty(pEdit) ) {
		SetNormalColour(m_pCentre);
		m_pCentre = 0;
	}

	OnChange();

} // end of OnChangeCentre

void CdCircle::OnKillfocusCentre ( void ) {

	// Just return if the user specified the field by pointing.
	if ( m_pCentre ) return;

	// Convert into the address of a point.
	m_pCentre = GetPoint(IDC_CENTRE);

	// Display the point in the correct colour.
	PaintAll();

	// See if a new circle should be drawn.
	OnChange();	
}

////////////////////// Radius /////////////////////////////////////

void CdCircle::OnSetfocusRadius ( void ) { m_Focus = IDC_RADIUS; }

void CdCircle::OnChangeRadius ( void ) {

	if ( IsFieldEmpty(GetDlgItem(IDC_RADIUS)) ) {

		// If we already had radius info, reset it.
		SetNormalColour(m_pRadiusPoint);
		m_pRadiusPoint = 0;
		m_Radius = CeDistance();
		OnChange();
	}
	else {

		// If the first character is a "+" character, it
		// means the text was set via a pointing operation.
		// So if the first char is NOT a "+", treat it as
		// an entered radius.

		CEdit* pEdit = (CEdit*)GetDlgItem(IDC_RADIUS);
		CString str;
		pEdit->GetWindowText(str);
		if ( str[0]!='+' ) {
			SetNormalColour(m_pRadiusPoint);
			m_pRadiusPoint = 0;
			ParseRadius();
		}
	}

} // end of OnChangeRadius

void CdCircle::OnKillfocusRadius ( void ) {

	// No validation if the radius is being specified by pointing (in
	// that case, losing the focus is ok.
	if ( m_pRadiusPoint ) {
		OnChange();
		return;
	}

	// Return if the field is empty.
	CEdit* pEdit = (CEdit*)GetDlgItem(IDC_RADIUS);
	if ( IsFieldEmpty(pEdit) ) {
		OnChange();
		return;
	}

	// If the field starts with a "+" character, it must be
	// an explicitly entered point ID.
	CString str;
	pEdit->GetWindowText(str);
	if ( str[0]=='+' ) {
		m_pRadiusPoint = GetPoint(IDC_RADIUS);
		if ( !m_pRadiusPoint ) {
			pEdit->SetFocus();
			return;
		}
	}

	// Parse the radius.
	ParseRadius();

} // end of OnKillfocusRadius

/////////////////////////////////////////////////////////////////////////////
//
//	@mfunc Check whether the current data is enough to
//	construct a circle. If so, draw it. Remember
//	to erase any previously drawn circle.
//
//	@devnote Scabbed from CdGetDist
//
/////////////////////////////////////////////////////////////////////////////

void CdCircle::OnChange ( void ) {

	CeCircle* pCircle=0;	// Constructed circle.

	// If we had a circle before, erase it.
	if ( m_pCircle ) {
		m_pCircle->Erase();
		delete m_pCircle;
		m_pCircle = 0;
	}

	// And repaint any selected points, to ensure the erase
	// didn't leave a shadow line.
	PaintAll();

	// Get rid of any previous radius observation.
	delete m_pRadius;
	m_pRadius = 0;

	if ( m_pCentre && (m_pRadiusPoint || m_Radius.GetMetric()>TINY) ) {

		FLOAT8 radius;

		// If we have an offset point, get the radius.
		if ( m_pRadiusPoint ) {
			CeVertex centre(*m_pCentre);
			CeVertex edge(*m_pRadiusPoint);
			radius = sqrt(centre.DistanceSquared(edge));
		}
		else
			radius = m_Radius.GetMetric();

		// Construct circle & draw it.
		m_pCircle = new CeCircle(*m_pCentre,radius);
		m_pCircle->Draw();

		// Create the appropriate distance observation.

		if ( m_pRadiusPoint )
			m_pRadius = new CeOffsetPoint(*m_pRadiusPoint);
		else
			m_pRadius = new CeDistance(m_Radius);
	}

} // end of OnChange

/////////////////////////////////////////////////////////////////////////////
//
//	@mfunc Parse an explicitly entered radius.
//
//	@rdesc TRUE if distance parses ok.
//
/////////////////////////////////////////////////////////////////////////////

LOGICAL CdCircle::ParseRadius ( void ) {
			
	// Get the entered string.
	CHARS str[32];
	CEdit* pEdit = (CEdit*)GetDlgItem(IDC_RADIUS);
	pEdit->GetWindowText(str,sizeof(str)-1);

	// No radius if empty string (ignore any trailing white space).
	UINT4 slen = StrLength(str);
	str[slen] = '\0';
	if ( slen==0 ) {
		m_Radius = CeDistance();
		return TRUE;
	}

	// Parse the radius
	m_Radius = CeDistance(str);

	// Parsing was successful if the radius comes back
	// as a defined value.

	if ( !m_Radius.IsDefined() ) {
		AfxMessageBox ( "Invalid radius." );
		pEdit->SetFocus();
		return FALSE;
	}

	OnChange();
	return TRUE;

} // end of ParseRadius

// React to selection of a point on the map.

#include "CeColour.h"

void CdCircle::OnSelectPoint ( const CePoint* const pPoint ) {

	// Return if point is not defined.
	if ( !pPoint ) return;

	// Handle the pointing, depending on what field we were
	// last in.

	switch ( m_Focus ) {

	case IDC_CENTRE: {

		// Draw any previously selected centre point normally.
		SetNormalColour(m_pCentre);

		// Grab the new centre point.
		m_pCentre = (CePoint*)pPoint;

		// Draw the point in appropriate colour.
		GetpView()->Draw(*pPoint,COL_LIGHTBLUE);

		// Display its key (causes a call to OnChangeCentre).
		CString str("+");
		str += m_pCentre->FormatKey();
		GetDlgItem(IDC_CENTRE)->SetWindowText(str);

		// Move focus to the radius field.
		GetDlgItem(IDC_RADIUS)->SetFocus();

		return;
	}

	case IDC_RADIUS: {

		// The radius must be getting specified by pointing
		// at an offset point.

		// Ensure that any previously selected offset point is
		// drawn in its normal colout.
		SetNormalColour(m_pRadiusPoint);

		// Grab the new offset point.
		m_pRadiusPoint = (CePoint*)pPoint;

		// Draw the point in appropriate colour.
		GetpView()->Draw(*pPoint,COL_YELLOW);

		// Display the point number.
		CString str("+");
		str += m_pRadiusPoint->FormatKey();
		GetDlgItem(IDC_RADIUS)->SetWindowText(str);

		// Ensure any radius circle has been refreshed.
		OnChange();

		// Move focus to the OK button.
		GetDlgItem(IDOK)->SetFocus();

		return;
	}
	
	default:

		return;

	} // end switch

} // end of OnSelectPoint

void CdCircle::Paint ( const CePoint* const pPoint ) {

	if ( !pPoint )
		PaintAll();
	else {
		CeView* pView = GetpView();
		if ( pPoint==m_pCentre )
			pView->Draw(m_pCentre,COL_LIGHTBLUE);
		if ( pPoint==m_pRadiusPoint )
			pView->Draw(m_pRadiusPoint,COL_YELLOW);
	}

} // end of Paint

/////////////////////////////////////////////////////////////////////////////
//
//	@mfunc Handle any redrawing. This just ensures that points
//	are drawn in the right colour, and that any distance circle
//	shown is still there.
//
//	@parm TRUE to draw. FALSE to erase.
//
/////////////////////////////////////////////////////////////////////////////

void CdCircle::PaintAll ( const LOGICAL draw ) const {

	//	Draw any currently selected points & any distance circle.

	if ( m_pCircle ) {
		if ( draw )
			m_pCircle->Draw();
		else
			m_pCircle->Erase();
	}

	CeView* pView = GetpView();

	if ( draw ) {
		if ( m_pCentre ) pView->Draw(*m_pCentre,COL_LIGHTBLUE);
		if ( m_pRadiusPoint ) pView->Draw(*m_pRadiusPoint,COL_YELLOW);
	}
	else {
		if ( m_pCentre ) pView->Draw(*m_pCentre,COL_BLACK);
		if ( m_pRadiusPoint ) pView->Draw(*m_pRadiusPoint,COL_BLACK);
	}

} // end of PaintAll

/////////////////////////////////////////////////////////////////////////////
//
//	@mfunc	Set normal colour for a point.
//
//	@parm	The point to set the colour for.
//
/////////////////////////////////////////////////////////////////////////////

void CdCircle::SetNormalColour ( const CePoint* const pPoint ) const {

	// Return if point not specified.
	if ( !pPoint ) return;

	// Get the view to draw point in black.
	GetpView()->Draw(*pPoint,COL_BLACK);

} // end of SetNormalColour

void CdCircle::OnDestroy ( void ) {

	CDialog::OnDestroy();
	
	// Clean up the view.
	PaintAll(FALSE);
}

CePoint* CdCircle::GetPoint ( const INT4 id ) {

	CePoint* pPoint=0;

	// Return if the field is empty.
	CEdit* pEdit = (CEdit*)GetDlgItem(id);
	if ( IsFieldEmpty(pEdit) ) return 0;

	// Parse the ID value.
	UINT4 idnum;
	if ( !ReadUINT4(pEdit,idnum) ) {
		AfxMessageBox("Invalid point ID");
		pEdit->SetFocus();
		return 0;
	}

	// Ask the map to locate the address of the specified point.
	const CeKey key(idnum);
	pPoint = CeMap::GetpMap()->GetpPoint(key);
	if ( !pPoint ) {
		AfxMessageBox("No point with specified key.");
		pEdit->SetFocus();
		return 0;
	}

	// Ensure the text is preceded with a "+" character.
	CString str("+");
	str += pPoint->FormatKey();
	pEdit->SetWindowText(str);	// causes call to OnChange handler

	return pPoint;

} // end of GetPoint
        */

        void ShowUpdate()
        {
            // Get the operation that created the update object (if
            // we're doing an update).
            NewCircleOperation creator = GetUpdateOp();
            if (creator != null)
                InitOp(creator, true);
        }

        void InitOp(NewCircleOperation op, bool isUpdate)
        {
        }
        /*
void CdCircle::InitOp ( const CeNewCircle& op
					  , const LOGICAL isUpdate ) {

	// Get the centre point and display its ID, preceded by a "+" char.
	m_pCentre = op.GetCentre();
	CString keystr("+");
	keystr += m_pCentre->FormatKey();
	GetDlgItem(IDC_CENTRE)->SetWindowText(keystr);

	// Get the observation that was used to specify the radius.
	CeObservation* pRadius = op.GetRadius();

	// Make a copy of the relevant info, depending on whether the
	// radius was entered, or specified as an offset point.
	const CeOffsetPoint* pOffset = dynamic_cast<const CeOffsetPoint*>(pRadius);
	if ( pOffset ) {

		// Radius was specified as an offset point.
		m_pRadiusPoint = pOffset->GetpPoint();
		m_Radius = CeDistance();

		// Display the ID of the offset point, preceded with a "+" char.
		keystr = "+";
		keystr += m_pRadiusPoint->FormatKey();
		GetDlgItem(IDC_RADIUS)->SetWindowText(keystr);

		if ( isUpdate ) {

			// If there are any incident curves that were added using
			// a CeNewArc operation (on the same circle), disallow the
			// ability to change the offset point.

			// @devnote Long story. In short, if the offset point
			// gets changed, the user could move it anywhere, so quite
			// a sophisticated UI could be needed to re-define where
			// the curves should go (failing that, if you let the user
			// change things, one end of the curve moves, but not the 
			// end that met the offset point => looks bent). This is a
			// problem even if the curves have subsequently been
			// de-activated.

			CeArc* pArc = op.GetpArc();
			CeCircle* pCircle = pArc->GetpCircle();
			assert(pCircle);
			if ( pCircle->HasCurvesAt(m_pRadiusPoint->GetpVertex()) )
				GetDlgItem(IDC_RADIUS)->EnableWindow(FALSE);
		}
	}
	else {

		// Radius is (or should be) an entered distance.

		m_pRadiusPoint = 0;
		const CeDistance* pDist = dynamic_cast<const CeDistance*>(pRadius);
		if ( !pDist ) {
			ShowMessage("CdCircle::ShowUpdate\nUnexpected radius observation.");
			return;
		}
		m_Radius = CeDistance(*pDist);

		// Display the radius (will call OnChangeRadius).
		CEdit* pEdit = (CEdit*)GetDlgItem(IDC_RADIUS);
		pEdit->SetWindowText(m_Radius.Format());
	}

	// Ensure points are drawn ok.
	PaintAll();

} // end of InitOp
*/
        NewCircleOperation GetUpdateOp()
        {
            UpdateUI up = (m_Cmd as UpdateUI);
            return (up==null ? null : (NewCircleOperation)up.GetOp());
        }
    }
}