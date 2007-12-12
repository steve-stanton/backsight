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
using System.Collections.Generic;

using Backsight.Environment;
using Backsight.Editor.Operations;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" was="CdGetDir" />
    /// <summary>
    /// Dialog for getting the user to specify a direction (for use with
    /// Intersection edits).
    /// </summary>
    partial class GetDirectionControl : UserControl
    {
        #region Class data

        /// <summary>
        /// The current default offset distance (may be less then zero, denoting an offset
        /// to the left)
        /// </summary>
        Distance m_DefaultOffset;

        // Direction-related ...

        /// <summary>
        /// The backsight point (if any).
        /// </summary>
        PointFeature m_Backsight;

        /// <summary>
        /// The from point.
        /// </summary>
        PointFeature m_From;

        /// <summary>
        /// First parallel point (if direction specified that way).
        /// </summary>
        PointFeature m_Par1;

        /// <summary>
        /// Second parallel point (if direction specified that way).
        /// </summary>
        PointFeature m_Par2;

        /// <summary>
        /// Direction angle. If <c>m_Par1</c> is defined, this should be 0.0 and should
        /// not be used. Always a positive value.
        /// </summary>
        double m_Radians;

        /// <summary>
        /// Is <c>m_Radians</c> a deflection?
        /// </summary>
        bool m_IsDeflection;

        /// <summary>
        /// True if direction is expected to be clockwise (the default). This can
        /// be set to false only if a backsight is defined.
        /// </summary>
        bool m_IsClockwise;

        // Offset-related ...

        /// <summary>
        /// Offset distance (if specified that way).
        /// </summary>
        Distance m_OffsetDistance; // was m_Offset

        /// <summary>
        /// True if offset distance specified and it's to the right of the direction.
        /// </summary>
        bool m_IsRight;

        /// <summary>
        /// Point that defines offset.
        /// </summary>
        PointFeature m_OffsetPoint; // was m_pOffset

        /// <summary>
        /// Line type (if defined, a line should be generated to any intersection point).
        /// </summary>
        IEntity m_LineType;

        // Internal housekeeping stuff ...

        /// <summary>
        /// Currently drawn direction.
        /// </summary>
        Direction m_Dir;

        /// <summary>
        /// Any circles incident on the from point.
        /// </summary>
        List<Circle> m_Circles;

        /// <summary>
        /// True if the backsight should be the center of a curve that the from-point
        /// coincides with.
        /// </summary>
        bool m_WantCentre;

    	//UINT m_Focus;		// The field that last had the focus.

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public GetDirectionControl()
        {
            InitializeComponent();

            m_DefaultOffset = null;
            m_Backsight = null;
            m_From = null;
            m_Par1 = null;
            m_Par2 = null;
            m_Radians = 0.0;
            m_IsDeflection = false;
            m_IsClockwise = true;
            m_OffsetDistance = null;
            m_IsRight = true;
            m_OffsetPoint = null;
            m_LineType = null;
            m_Dir = null;
            m_Circles = new List<Circle>();
            m_WantCentre = false;
            //m_Focus = 0;
        }

        #endregion

        /*
private:

	virtual LOGICAL		IsPointValid	( void ) const;
	virtual void		OnNewDirection	( void );
	virtual LOGICAL		OnOffsetDistance ( void );
	virtual void		SetNormalColour ( const CePoint* const pPoint ) const;
	virtual void		SetColour		( const CePoint* const pPoint, const UINT id=0 ) const;
	virtual void		OnDrawAll		( const LOGICAL draw=TRUE ) const;
	virtual LOGICAL		ParseDirection	( void );

	virtual LOGICAL		ShowUpdate		( const UINT1 dir );
	virtual void		Show			( const CeDirection* const pDir
										, const CeArc* const pArc );
	virtual void		ShowAngle		( const CeDirection* const pDir );
	virtual void		ShowBearing		( const CeDirection* const pDir );
	virtual void		ShowParallel	( const CeDirection* const pDir );
	virtual void		ShowOffset		( const CeDirection* const pDir );
	virtual	void		ShowKey			( const int id
										, const CePoint* const pPoint );
	virtual	void		GetCircles		( void );
	virtual	LOGICAL		CanSetDefaultOffset ( void ) const;
         */

        /// <summary>
        /// Performs any processing on selection of a point feature.
        /// </summary>
        /// <param name="point">The point that has just been selected.</param>
        internal void OnSelectPoint(PointFeature point)
        {
            /*

//	Return if point is not defined.
	if ( !pPoint ) return;

//	Return if point is not valid.
	if ( !IsPointValid() ) return;

//	Set colour
	SetColour(pPoint);

	CHARS	 str[132];		// Character string

//	Handle the pointing, depending on what field we were
//	last in.

	switch ( m_Focus ) {

	case IDC_FROMPOINT: {

//		Save the from point.
		SetNormalColour(m_pFrom);
		m_pFrom = pPoint;

		// Initialize list of any circles that are incident on
		// the new from point ...
		GetCircles();

		// Display it (causes a call to OnChangeFromPoint).
		ShowKey(IDC_FROMPOINT,m_pFrom);

//		Move focus to the backsight field.
		GetDlgItem(IDC_BACKSIGHT)->SetFocus();

		return;
	}

	case IDC_BACKSIGHT: {

//		Ensure that any previously selected backsight reverts
//		to its normal colour.
		SetNormalColour(m_pBacksight);

//		Save the specified backsight.
		m_pBacksight = pPoint;

		// Display it (causes a call to OnChangeBacksight).
		ShowKey(IDC_BACKSIGHT,m_pBacksight);

//		Move focus to the direction field.
		GetDlgItem(IDC_DIRECTION)->SetFocus();

		return;
	}

	case IDC_DIRECTION: {

//		The direction must be getting specified by pointing
//		to two parallel points.

//		Define either the first or the second parallel point.
		if ( m_pPar1 ) {
			SetNormalColour(m_pPar2);
			m_pPar2 = pPoint;
		}
		else {
			SetNormalColour(m_pPar1);
			m_pPar1 = pPoint;
		}

//		Figure out the window text.

		if ( m_pPar1 && m_pPar2 ) {
			sprintf ( str, "%s->", m_pPar1->FormatKey() );
			strcat ( str, m_pPar2->FormatKey() );
		}
		else
			sprintf ( str, "%s->", m_pPar1->FormatKey() );

//		Display it.
		GetDlgItem(IDC_DIRECTION)->SetWindowText(str);

//		Move focus to the offset field if we have both
//		parallel points.
		if ( m_pPar2 ) GetDlgItem(IDC_OFFSET)->SetFocus();

		return;
	}
	
	case IDC_OFFSET: {

//		Hold on to the offset point & ensure distance is null.
		SetNormalColour(m_pOffset);
		m_pOffset = pPoint;
		m_Offset = CeDistance();

//		Display the window text.
		ShowKey(IDC_OFFSET,m_pOffset);

		// Can't set default offset if it was specified via a point
		TurnOff(IDC_SET_DEFAULT_OFFSET);

//		Move focus to the line type.
		GetDlgItem(IDC_LINE_TYPE)->SetFocus();

		return;
	}

	default:

		return;

	} // end switch
             */
        }

        /// <summary>
        /// Performs any processing on selection of a line feature.
        /// </summary>
        /// <param name="point">The line that has just been selected.</param>
        internal void OnSelectLine(LineFeature line)
        {
            /*

	// Return if line is not defined.
	if ( !pArc ) return;

	// If the focus is in the backsight field, and the selected
	// line connects to the from-point, define the backsight to
	// be the point at the other end of the line.

	if ( !m_pFrom ) return;
	if ( m_Focus!=IDC_BACKSIGHT ) return;

	const CeLine* const pLine = pArc->GetpLine();
	const CeLocation* const psLoc = pLine->GetpStart();
	const CeLocation* const peLoc = pLine->GetpEnd();

	CeLayerList curlayer;	// only the currently active theme
	CePoint* pPoint = 0;

	if ( m_pFrom->GetpVertex() == psLoc )
		pPoint = peLoc->GetpPoint(pLine,&curlayer);
	else if ( m_pFrom->GetpVertex() == peLoc )
		pPoint = psLoc->GetpPoint(pLine,&curlayer);

	// Return if a connected point cannot be found.
	if ( !pPoint ) {
		ShowMessage("Cannot locate backsight point.");
		return;
	}

	OnSelectPoint(pPoint);
             */
        }

        internal void OnDraw(PointFeature point)
        {
            /*

	if ( !pPoint )
		OnDrawAll();
	else {
		if ( pPoint==m_pBacksight ) SetColour(m_pBacksight,IDC_BACKSIGHT);
		if ( pPoint==m_pFrom      ) SetColour(m_pFrom,     IDC_FROMPOINT);
		if ( pPoint==m_pPar1      ) SetColour(m_pPar1,     IDC_DIRECTION);
		if ( pPoint==m_pPar2      ) SetColour(m_pPar2,     IDC_DIRECTION);
		if ( pPoint==m_pOffset    ) SetColour(m_pOffset,   IDC_OFFSET);
	}
             */
        }

        internal Direction Direction
        {
            get { return m_Dir; }
        }

        internal IEntity LineType
        {
            get { return m_LineType; }
        }

        private void GetDirectionControl_Load(object sender, EventArgs e)
        {
            /*
	CdPage::OnInitDialog();
	//AfxMessageBox("OnInitDialog");

//	Default settings for direction radio buttons.
	TurnRadioOff(IDC_CLOCKWISE);
	TurnRadioOff(IDC_COUNTER_CLOCKWISE);

//	Default settings for offset radio buttons (not checked, not enabled)
	TurnRadioOff(IDC_LEFT);
	TurnRadioOff(IDC_RIGHT);

	// Clear the button that lets the displayed offset value
	// be set as default
	TurnOff(IDC_SET_DEFAULT_OFFSET);

	// If we've got a default offset, display it (and set the
	// appropriate radio button)
	CString value = AfxGetApp()->GetProfileString("Settings","DirectionOffset");
	if ( !value.IsEmpty() )
	{
		m_DefaultOffset = CeDistance((LPCTSTR)value);
		m_DefaultOffset.SetFixed();
	}

//	Initialize combo box with a list of all line entity types
//	for the currently active theme.
	CeMap* pMap = CeMap::GetpMap();
	const CeTheme* const pTheme = pMap->GetpTheme();
	m_pLineType = LoadEntityCombo(IDC_LINE_TYPE,pTheme,LINE);

	if ( m_DefaultOffset.IsDefined() )
	{
		TurnComboOff(IDC_LINE_TYPE);
		m_pLineType = 0;
	}

	CdDialog* pParent = (CdDialog*)GetParent();
	CdIntersectDirLine* pDirLine =
		dynamic_cast<CdIntersectDirLine*>(pParent);

	if ( pDirLine ) {

		// For direction-line intersections, the default is NOT
		// to add a line, so if we're not doing an update, make
		// sure the line type is undefined.
		if ( !GetUpdateOp() ) {
			CComboBox* pBox = (CComboBox*)GetDlgItem(IDC_LINE_TYPE);
			m_pLineType = 0;
			pBox->SelectString(-1,"<none>");
		}
	}

	// If we are updating a feature that was previously created (or recalling
	// a previous edit), load the original info. For direction-direction intersections,
	// we need to know which page this is, to determine whether we
	// should display info for the 1st or 2nd direction.

	if ( !ShowUpdate(this->m_PageNum) )
	{
		// Display default offset (if there is one). Setting the text will cause
		// a call to OnChangeOffset, which will define m_Offset & m_IsRight
		if ( m_DefaultOffset.IsDefined() )
			GetDlgItem(IDC_OFFSET)->SetWindowText(m_DefaultOffset.Format(TRUE));
	}
             */
        }

        /*
// We remember the ID of the last thing that was in focus so
// that we can respond to a possible OnSelectPoint.

void CdGetDir::OnSetfocusBacksight() { m_Focus = IDC_BACKSIGHT;	}
void CdGetDir::OnSetfocusFrompoint() { m_Focus = IDC_FROMPOINT; }
void CdGetDir::OnSetfocusOffset() { m_Focus = IDC_OFFSET; }
void CdGetDir::OnSetfocusDirection() { m_Focus = IDC_DIRECTION; }
         */

        /*
//	@mfunc Check if it is OK to accept a selected point in the field
//	that last had the input focus.
//
/////////////////////////////////////////////////////////////////////////////

LOGICAL CdGetDir::IsPointValid ( void ) const {

	CHARS str[132];

	switch ( m_Focus ) {

	case IDC_BACKSIGHT: {

//		Disallow a backsight if the direction has been specified
//		using two (or even just one) parallel points.

		if ( m_pPar1 ) {
			sprintf ( str, "%s\n%s",
				"You cannot specify a backsight if you intend",
				"to define direction using two points." );
			AfxMessageBox( str );
			return FALSE;
		}

		return TRUE;
	}

	case IDC_FROMPOINT: {

//		If a from point is already defined, allow the point only
//		if the user wants to change it. If the from point is not
//		already defined, the from point is always valid.

		if ( m_pFrom ) {
			sprintf ( str, "%s\n%s",
				"You have already specified the from-point.",
				"If you want to change it, erase it first." );
			AfxMessageBox ( str );
			return FALSE;
		}
		else
			return TRUE;
	}

	case IDC_DIRECTION: {

//		The direction must be getting specified by pointing
//		to two parallel points. This is not allowed if a
//		backsight has been specified.

		if ( m_pBacksight ) {
			sprintf ( str, "%s\n%s",
				"You cannot specify two points for direction",
				"when the direction has a backsight." );
			AfxMessageBox( str );
			return FALSE;
		}

//		Disallow if two parallel points have already been specified.
		if ( m_pPar1 && m_pPar2 ) {
			AfxMessageBox (
				"You have already specified two points for direction." );
			return FALSE;
		}

		return TRUE;
	}
	
	case IDC_OFFSET: {

//		Disallow if an offset has already been specified.
		if ( m_pOffset ) {
			AfxMessageBox ( "You have already specified the offset point." );
			return FALSE;
		}

//		Disallow if the distance is already defined.
		if ( m_Offset.IsDefined() ) {
			AfxMessageBox ( "You have already specified an offset distance." );
			return FALSE;
		}

		return TRUE;
	}

	default:

//		If it's none of the above fields, a point is not valid.
//		Just return quietly, in case the user is just mucking about
//		pointing at stuff in the map window.

		return FALSE;

	} // end switch

} // end of IsPointValid
         */

        /////////////////////////////////////////////////////////////////////////////
        //
        //	Radio buttons fall into 2 groups:
        //
        //	Group 1 -- Related to direction.
        //
        //	IDC_CLOCKWISE -- direction measured clockwise.
        //	IDC_COUNTER_CLOCKWISE -- direction measured counter-clockwise.
        //
        //	Group 2 -- Related to offset.
        //
        //	IDC_LEFT -- offset to left of direction.
        //	IDC_RIGHT -- offset to right of direction.
        //
        //	The aim of the functions that follow is to validate that the
        //	user's action is consistent with everything that has gone
        //	before. In practice, many of the radio buttons do not need
        //	to be visited by the user, because they are frequently set
        //	or activated in response to pointing operations, or on the
        //	basis of what has been entered into edit boxes.
        //
        /////////////////////////////////////////////////////////////////////////////

        /*

void CdGetDir::OnClockwise ( void ) {

	m_IsClockwise = TRUE;
	m_Radians = fabs(m_Radians);
	CheckDlgButton(IDC_CLOCKWISE,1);
	CheckDlgButton(IDC_COUNTER_CLOCKWISE,0);
	OnNewDirection();
}

void CdGetDir::OnCounterClockwise ( void ) {

	m_IsClockwise = FALSE;
	m_Radians = fabs(m_Radians);
	CheckDlgButton(IDC_CLOCKWISE,0);
	CheckDlgButton(IDC_COUNTER_CLOCKWISE,1);
	OnNewDirection();
}
         */

        /*
// Offset to the left

void CdGetDir::OnLeft ( void ) {

	m_IsRight = FALSE;
	CheckDlgButton(IDC_LEFT,1);
	CheckDlgButton(IDC_RIGHT,0);
	OnNewDirection();
}

// Offset to the right

void CdGetDir::OnRight ( void ) {

	m_IsRight = TRUE;
	CheckDlgButton(IDC_LEFT,0);
	CheckDlgButton(IDC_RIGHT,1);
	OnNewDirection();
}
         */
        /////////////////////////////////////////////////////////////////////////////
        //
        //	The OnChange functions that follow are executed whenever
        //	new text is entered into one of the edit boxes (either
        //	by the user, or by calls made to SetWindowText). The
        //	intent of these functions is to validate the action, and
        //	to enable or disable fields that are no longer applicable
        //	as a result.
        //
        //	Take care on calls to SetWindowText, because that may
        //	cause the same function to be called again.
        //
        /////////////////////////////////////////////////////////////////////////////

        /*
void CdGetDir::OnChangeBacksight ( void ) {

//	If field is now empty, ensure that backsight is undefined,
//	and see if this impacts any displayed direction.

	if ( IsFieldEmpty(IDC_BACKSIGHT) ) {
		SetNormalColour(m_pBacksight);
		m_pBacksight = 0;
		OnNewDirection();
	}
	else if ( !m_pBacksight )
		AfxMessageBox
		( "You can only specify the backsight by pointing at the map." );
	
} // end of OnChangeBacksight
         */

        /*
void CdGetDir::OnChangeFromPoint ( void ) {

//	If the field is now empty, ensure that the from point
//	is undefined.

	if ( IsFieldEmpty(IDC_FROMPOINT) ) {
		SetNormalColour(m_pFrom);
		m_pFrom = 0;
		GetCircles();
		GetDlgItem(IDC_BACKSIGHT)->EnableWindow(TRUE);
		OnNewDirection();
	}
	else if ( !m_pFrom )
		AfxMessageBox
		( "You can only specify the from-point by pointing at the map." );

} // end of OnChangeFromPoint
         */

        /*
void CdGetDir::OnChangeDirection ( void ) {

	if ( IsFieldEmpty(IDC_DIRECTION) ) {

//		If we already had direction info, reset it.
		SetNormalColour(m_pPar1);
		SetNormalColour(m_pPar2);
		m_pPar1 = 0;
		m_pPar2 = 0;
		m_Radians = 0.0;
		m_IsClockwise = TRUE;
		m_IsDeflection = FALSE;
		OnNewDirection();

//		Field is empty, so revert to defaults.
		TurnRadioOff(IDC_CLOCKWISE);
		TurnRadioOff(IDC_COUNTER_CLOCKWISE);

	}
	else {

//		The direction could have been specified by the
//		user, or it could have been set as the result of
//		a pointing operation. In the latter case, m_pPar1
//		will be defined.

		if ( m_pPar1 ) {
			TurnRadioOff(IDC_CLOCKWISE);
			TurnRadioOff(IDC_COUNTER_CLOCKWISE);
		}
		else {

//			Explicitly entered by the user.

//			Enable ability to specify clockwise/counter-clockwise.
			TurnOn(IDC_CLOCKWISE);
			TurnOn(IDC_COUNTER_CLOCKWISE);

//			Parse the direction.
			ParseDirection();
		}

	}

} // end of OnChangeDirection
         */

        /*
void CdGetDir::OnChangeOffset ( void ) {

	if ( IsFieldEmpty(IDC_OFFSET) ) {

//		Erase any previously defined offset info.
		SetNormalColour(m_pOffset);
		m_pOffset = 0;
		m_Offset = CeDistance();
		OnNewDirection();

//		Go back to default settings.
		TurnRadioOff(IDC_LEFT);
		TurnRadioOff(IDC_RIGHT);
		TurnOn(IDC_LINE_TYPE);
	}
	else {

//		If the method used to define the offset was by
//		pointing at the map, enable line type, and set
//		the other radio buttons accordingly.

		if ( m_pOffset ) {
			TurnRadioOff(IDC_LEFT);
			TurnRadioOff(IDC_RIGHT);
			TurnOn(IDC_LINE_TYPE);
			TurnOff(IDC_SET_DEFAULT_OFFSET);
		}
		else { // Offset specified by entered distance.

			TurnOn(IDC_LEFT);
			TurnOn(IDC_RIGHT);
			TurnComboOff(IDC_LINE_TYPE);
			m_pLineType = 0;

//			Parse the distance.
			OnOffsetDistance();
		}
	}

	if ( CanSetDefaultOffset() )
		TurnOn(IDC_SET_DEFAULT_OFFSET);
	else
		TurnOff(IDC_SET_DEFAULT_OFFSET);

} // end of OnChangeOffset
         */

        /*
LOGICAL CdGetDir::CanSetDefaultOffset ( void ) const
{
	if ( m_pOffset ) return FALSE;

	CeDistance soff(m_Offset);
	if ( !m_IsRight ) soff.SetNegative();
	return (soff != m_DefaultOffset);
}
         */

        /*
void CdGetDir::OnSelChangeLineType ( void ) {

//	Get the new selection (if any)
	m_pLineType = ReadEntityCombo(IDC_LINE_TYPE);

//	If we have a direction, move directly to the next page.
	if ( m_pDir ) {
		CdDialog* pDial = (CdDialog*)GetParent();
		pDial->PressButton(PSBTN_NEXT);
	}

} // end of OnSelChangeLineType
         */

        /*
void CdGetDir::OnKillfocusDirection ( void ) {

//	No validation if the direction is being specified by pointing (in
//	that case, losing the focus before the second point has been
//	specified is normal, because the user has to point at the 2nd
//	parallel point).
	if ( m_pPar1 ) {
		OnNewDirection();
		return;
	}

//	Return if the field is empty.
	if ( IsFieldEmpty(IDC_DIRECTION) ) {
		OnNewDirection();
		return;
	}

//	Parse the direction.
	ParseDirection();

} // end of OnKillfocusDirection
         */

        /*
void CdGetDir::OnKillfocusOffset ( void ) {

//	Return if the field is empty.
	if ( IsFieldEmpty(IDC_OFFSET) ) {
		OnNewDirection();
		return;
	}

//	Return if the text was obtained via a pointing operation.
	if ( m_pOffset ) {
		OnNewDirection();
		return;
	}

//	Get the entered string.
	if ( !OnOffsetDistance() ) {
		OnNewDirection();
		return;
	}

//	Try new direction.
	OnNewDirection();

} // end of OnKillfocusOffset
         */

        /*
void CdGetDir::OnKillfocusFromPoint ( void ) {

//	See if a new direction should be drawn.
	OnNewDirection();	
}

void CdGetDir::OnKillfocusBacksight ( void ) {

//	See if a new direction should be drawn.
	OnNewDirection();	
}
         */

        /*
//	@mfunc Check whether the current data is enough to
//	construct a direction. If so, draw it. Take care to
//	erase any previously drawn direction.
//
/////////////////////////////////////////////////////////////////////////////

void CdGetDir::OnNewDirection ( void ) {

	if ( CanSetDefaultOffset() )
		TurnOn(IDC_SET_DEFAULT_OFFSET);
	else
		TurnOff(IDC_SET_DEFAULT_OFFSET);

	CeDirection* pDir=0;	// Constructed direction.
	CeAngle angle;			// Angle from a backsight.
	CeDeflection deflect;	// Deflection
	CeBearing bearing;		// Bearing from north.
	CeParallel par;			// Parallel to 2 points.
	CeOffset* pOffset=0;	// Constructed offset.
	CeOffsetDistance odist;	// Offset distance.
	CeOffsetPoint opt;		// Offset point.
	FLOAT8 srad;			// Signed radian value.

//	Apply sign to any direction we have.
	if ( m_IsClockwise )
		srad = m_Radians;
	else
		srad = -m_Radians;


	if ( m_pBacksight ) {

		// If we have a backsight, we could either have a regular
		// angle or a deflection. To construct either, we need a
		// from-point as well.

		// Note that an angle of zero (passing through the backsight
		// or foresight) is fine.

		if ( m_pFrom ) {

			if ( m_IsDeflection ) {
				deflect = CeDeflection(*m_pBacksight,*m_pFrom,srad);
				pDir = &deflect;
			}
			else {
				angle = CeAngle(*m_pBacksight,*m_pFrom,srad);
				pDir = &angle;
			}
		}

	}
	else if ( m_pFrom ) {

//		No backsight, so we could have either a bearing,
//		or a direction defined using 2 parallel points.
//		Since a bearing of zero is quite valid, we check
//		the dialog field to see if this is an entered value,
//		or just the initial value.

		if ( m_pPar1 && m_pPar2 ) {
			par = CeParallel(*m_pFrom,*m_pPar1,*m_pPar2);
			pDir = &par;
		}
		else if ( m_Radians>TINY || !IsFieldEmpty(IDC_DIRECTION) ) {
			bearing = CeBearing(*m_pFrom,srad);
			pDir = &bearing;
		}

	}

//	If we have formed a direction, see if there's an offset.

	if ( pDir ) {

//		It could have been specified by a point, or by
//		entering a distance left or right of the direction.

		if ( m_pOffset ) {
			opt = CeOffsetPoint(*m_pOffset);
			pOffset = &opt;
		}
		else if ( m_Offset.IsDefined() ) {
			odist = CeOffsetDistance(m_Offset,!m_IsRight);
			pOffset = &odist;
		}

//		If we got an offset, include it in the direction.
		if ( pOffset ) pDir->SetOffset(pOffset);

//		Return if we previously had a direction, and what
//		we have now is just the same.
		if ( m_pDir && *m_pDir==*pDir ) return;

	}

//	Get pointer to the dialog that encloses this one.
	CdDialog* pDial = (CdDialog*)GetParent();
	if ( !pDial ) return;

//	If we previously drew a direction, erase it now.
	if ( m_pDir ) {
		pDial->Erase(*m_pDir);
		delete m_pDir;
		m_pDir = 0;
	}

//	If we have a new direction, draw it.
	if ( pDir ) {
		m_pDir = pDir->MakeNewCopy();
		pDial->Draw(*m_pDir);

//		Redraw the from-point. Otherwise any previous version of the
//		line leaves a white remnant on the point.
		OnDraw(m_pDir->GetpFrom());
	}


	return;

} // end of OnNewDirection
         */

        /*
//	@mfunc Set colour for a point.
//
//	@parm The point to draw.
//	@parm The ID of the field that the point relates to. The
//	default is the field that currently has the focus.
//
/////////////////////////////////////////////////////////////////////////////

void CdGetDir::SetColour ( const CePoint* const pPoint, const UINT id ) const {

//	Return if point not specified.
	if ( !pPoint ) return;

//	Determine the colour.

	COL col;
	UINT field = id;
	if ( !field ) field = m_Focus;

	switch ( field ) {
	case IDC_BACKSIGHT:	col = COL_DARKBLUE;		break;
	case IDC_FROMPOINT: col = COL_LIGHTBLUE;	break;
	case IDC_DIRECTION: col = COL_YELLOW;		break;
	case IDC_OFFSET:	col = COL_GREEN;		break;
	default:			return;
	}

	// Draw the point in the proper colour.
	pPoint->DrawThis(col);

} // end of SetColour
         */

        /*
//	@mfunc Set colour for a point. The colour depends on the
//	field that currently has the focus.
//
//	@parm The point to set the colour for.
//	@parm The ID of the field that the point relates to. The
//	default is the field that currently has the focus.
//
/////////////////////////////////////////////////////////////////////////////

void CdGetDir::SetNormalColour ( const CePoint* const pPoint ) const {

//	Return if point not specified.
	if ( !pPoint ) return;

//	Ask the enclosing dialog to set the colour to black.

	CdDialog* pDial = (CdDialog*)GetParent();
	if ( pDial ) pDial->SetColour(*pPoint,COL_BLACK);

	return;

} // end of SetNormalColour
         */

        /*
//	@mfunc Handle any redrawing. This just ensures that points
//	are drawn in the right colour, and that any direction line
//	shown is still there.
//
//	@parm TRUE to draw. FALSE to erase.
//
//	This function is called at the end of the view's OnDraw
//	function. I though about reacting to a WM_PAINT message,
//	but wasn't sure what's going to get drawn first.
//
/////////////////////////////////////////////////////////////////////////////

void CdGetDir::OnDrawAll ( const LOGICAL draw ) const {

//	There's not much to draw, so just do everything.

//	Draw any currently selected points & any direction.

	if ( draw ) {
		SetColour(m_pBacksight,IDC_BACKSIGHT);
		SetColour(m_pFrom,IDC_FROMPOINT);
		SetColour(m_pPar1,IDC_DIRECTION);
		SetColour(m_pPar2,IDC_DIRECTION);
		SetColour(m_pOffset,IDC_OFFSET);
	}
	else {
		SetNormalColour(m_pBacksight);
		SetNormalColour(m_pFrom);
		SetNormalColour(m_pPar1);
		SetNormalColour(m_pPar2);
		SetNormalColour(m_pOffset);
	}

	if ( m_pDir ) {
		
		CdDialog* pDial = (CdDialog*)GetParent();
		if ( pDial ) {
			if ( draw )
				pDial->Draw(*m_pDir);
			else
				pDial->Erase(*m_pDir);
		}
	}

	return;

} // end of OnDraw
         */

        /*
//	@mfunc Parse an explicitly entered offset distance.
//
/////////////////////////////////////////////////////////////////////////////

LOGICAL CdGetDir::OnOffsetDistance ( void ) {

//	Null out the current offset.
	m_Offset = CeDistance();

//	Get the entered string.
	CHARS str[32];
	CEdit* pEdit = (CEdit*)GetDlgItem(IDC_OFFSET);
	UINT4 nc = pEdit->GetWindowText(str,sizeof(str)-1);
	if ( nc==0 ) return FALSE;

//	Ignore any trailing white space.
	UINT4 slen = StrLength(str);
	str[slen] = '\0';

//	If all we have is a "-", disable the ability to specify
//	offset right & return.
	if ( str[0]=='-' ) {
		TurnRadioOff(IDC_RIGHT);
		CheckDlgButton(IDC_LEFT,1);
		if ( slen==1 ) return FALSE;
	}

//	Parse the distance.
	CeDistance dist(str);

//	Convert the entered offset to a float.
//	CHARS* end;
//	FLOAT8 dval = strtod(str,&end);

//	The character that stopped the scan should have been the trailing null.
//	if ( *end ) {
	if ( !dist.IsDefined() ) {
		AfxMessageBox ( "Offset distance contains extraneous characters." );
		return FALSE;
	}

//	Save the entered distance (in the current document units if
//	units were not specified). Make it a fixed distance.

	m_Offset = dist;
	m_Offset.SetFixed();

//	If we have signed offset, it HAS to be an offset to the
//	left. Otherwise make sure we preserve the directional sense.
//	which may have been previously defined.

	if ( m_Offset.SetPositive() )	// i.e. the offset had to be
		m_IsRight = FALSE;			// made positive => offset left
	else
		m_IsRight = TRUE;

//	If the offset is signed, make it an offset left and
//	disable the ability to make it an offset right.

	if ( m_IsRight )
		OnRight();
	else
		OnLeft();
	
	return TRUE;

} // end of OnOffsetDistance
         */

        /*
//	@mfunc Parse an explicitly entered direction.
//
//	@rdesc TRUE if direction parses ok.
//
/////////////////////////////////////////////////////////////////////////////

LOGICAL CdGetDir::ParseDirection ( void ) {
			
	// Get the entered string.
	CString dirstr;
	CEdit* pEdit = (CEdit*)GetDlgItem(IDC_DIRECTION);
	pEdit->GetWindowText(dirstr);

	// Ignore any trailing white space.
	dirstr.TrimLeft();
	dirstr.TrimRight();

//	If all we have is a "-", disable the ability to specify
//	clockwise direction & return.
	if ( dirstr[0]=='-' ) {
		TurnRadioOff(IDC_CLOCKWISE);
		CheckDlgButton(IDC_COUNTER_CLOCKWISE,1);
		if ( dirstr.GetLength()==1 ) return FALSE;
	}

	// If the entered angle contains a "d" (anywhere), treat it
	// as a deflection (and strip it out).
	dirstr.MakeUpper();
	INT4 dindex = dirstr.Find('D');
	if ( dindex>=0 ) {
		CString temp(dirstr);
		dirstr = temp.Left(dindex) + temp.Mid(dindex+1);
		m_IsDeflection = TRUE;
	}
	else
		m_IsDeflection = FALSE;

//	Validate entered angle.
	FLOAT8 srad=0.0;
	if ( !dirstr.IsEmpty() ) {
		if ( !StrRad(srad,(LPCTSTR)dirstr) ) {
			AfxMessageBox ( "Invalid angle." );
			pEdit->SetFocus();
			return FALSE;
		}
	}

//	If we have signed radians, it HAS to be a counter-clockwise
//	angle. Otherwise make sure we preserve the directional sense.
//	which may have been previously defined.
	if ( srad<0.0 ) m_IsClockwise = FALSE;
	m_Radians = fabs(srad);

//	The radian value should ALWAYS be > 0.0. These calls
//	will fix the sign & go on to call OnNewDirection.

	if ( m_IsClockwise )
		OnClockwise();
	else
		OnCounterClockwise();

	return TRUE;

} // end of ParseDirection
         */

        /*
//	@mfunc	Initialize dialog for an update.
//
//	@parm	The direction number (1 or 2). A value of 2 is only
//			valid for CdIntersectDir parents.
//
//	@rdesc	TRUE if update (or recalled command) was shown.
//
/////////////////////////////////////////////////////////////////////////////

#include "CeIntersectDirLine.h"

LOGICAL CdGetDir::ShowUpdate ( const UINT1 dir ) {

//	Return if no update feature (and no recall op).
	const CeIntersect* pop = GetUpdateOp();
	if ( pop==0 ) pop = GetRecall();
	if ( !pop ) return FALSE;

//	Ensure that there is no direction line. If you don't do
//	this, it is possible that when we select a string in the
//	entity combo, we'll move directly to the next page of the
//	dialog (see <mf CdGetDir::OnSelChangeLineType>).
	delete m_pDir;
	m_pDir = 0;

//	Populate the dialog, depending on what sort of operation
//	we have.

	switch ( pop->GetType() ) {

	case CEOP_DIR_INTERSECT: {

		const CeIntersectDir* pOper = dynamic_cast<const CeIntersectDir*>(pop);

		if ( dir==1 )
			this->Show( pOper->GetpDir1(), pOper->GetpArc1() );
		else
			this->Show( pOper->GetpDir2(), pOper->GetpArc2() );

		break;
	}

	case CEOP_DIRDIST_INTERSECT: {

		const CeIntersectDirDist* pOper = dynamic_cast<const CeIntersectDirDist*>(pop);
		this->Show( pOper->GetpDir(), pOper->GetpDirArc() );
		break;
	}

	case CEOP_DIRLINE_INTERSECT: {

		const CeIntersectDirLine* pOper = dynamic_cast<const CeIntersectDirLine*>(pop);
		this->Show( pOper->GetpDir(), pOper->GetpDirArc() );
		break;
	}

	default:

		AfxMessageBox("CdGetDir::ShowUpdate\nUnexpected editing operation");
		break;

	} // end switch

//	Ensure everything is drawn ok.
	this->OnDrawAll();
	return TRUE;

} // end of ShowUpdate
         */

        /*
//	@mfunc	Display info for a specific direction & arc.
//
//	@parm	The direction to show.
//	@parm	The arc to show (0 if no arc).
//
/////////////////////////////////////////////////////////////////////////////

#include "CeArc.h"

void CdGetDir::Show ( const CeDirection* const pDir
					, const CeArc* const pArc ) {

//	If we have an arc, define its entity type and scroll the
//	entity combo box to that type. Note that when the string
//	is selected, it is important that m_pDir is null; otherwise
//	<mf CdGetDir::OnSelChangeLineType> may automatically move
//	on to the next page of the wizard dialog.

	if ( pArc ) {
		m_pLineType = pArc->GetpEntity();
		if ( m_pLineType ) {
			CComboBox* pBox = (CComboBox*)GetDlgItem(IDC_LINE_TYPE);
			pBox->SelectString(-1,m_pLineType->GetpName());
		}
	}
	else {
		CComboBox* pBox = (CComboBox*)GetDlgItem(IDC_LINE_TYPE);
		pBox->SelectString(-1,"<none>");
		m_pLineType = 0;
	}

//	Define the from-point of the direction.
	m_pFrom = (CePoint*)pDir->GetpFrom();
	ShowKey(IDC_FROMPOINT,m_pFrom);

	// Ensure we've got any circle info.
	GetCircles();

//	Does the direction have an offset?
	const CeOffset* const pOffset = pDir->GetpOffset();
	if ( pOffset ) this->ShowOffset(pDir);

//	The rest depends on what sort of direction we have.
	switch ( pDir->GetType() ) {

	case DIR_BEARING: {
		this->ShowBearing(pDir);
		break;
	}

	case DIR_ANGLE: {
		this->ShowAngle(pDir);
		break;
	}

	case DIR_PARALLEL: {
		this->ShowParallel(pDir);
		break;
	}

	case DIR_DEFLECTION: {

		this->ShowAngle(pDir);	// same as angle
		break;
	}

	default:

		AfxMessageBox("CdGetDir::Show\nCannot display direction info.");
		break;

	} // end switch

} // end of Show
         */

        /*
//	@mfunc	Display info for a direction angle.
//
//	@parm	The direction to show.
//
/////////////////////////////////////////////////////////////////////////////

void CdGetDir::ShowAngle ( const CeDirection* const pDir ) {

	const CeAngle* const pAngle =
		dynamic_cast<const CeAngle* const>(pDir);
	if ( !pAngle ) return;

//	There should be no parallel points.
	m_pPar1 = 0;
	m_pPar2 = 0;

//	Get the backsight
	m_pBacksight = (CePoint*)pAngle->GetpBacksight();
	ShowKey(IDC_BACKSIGHT,m_pBacksight);

//	Define the observed angle.
	m_Radians = pAngle->GetObservation();
	m_IsClockwise = (m_Radians>=0.0);
	m_Radians = fabs(m_Radians);

	CString dirstr = RadStrDms(m_Radians);
	if ( pAngle->GetType() == DIR_DEFLECTION ) dirstr += "d";
	GetDlgItem(IDC_DIRECTION)->SetWindowText(dirstr);

//	Indicate whether it's clockwise or otherwise.

	TurnRadioOn(IDC_CLOCKWISE);
	TurnRadioOn(IDC_COUNTER_CLOCKWISE);

	if ( m_IsClockwise ) {
		CheckDlgButton(IDC_CLOCKWISE,1);
		CheckDlgButton(IDC_COUNTER_CLOCKWISE,0);
	}
	else {
		CheckDlgButton(IDC_CLOCKWISE,0);
		CheckDlgButton(IDC_COUNTER_CLOCKWISE,1);
	}

} // end of ShowAngle
         */

        /*
//	@mfunc	Display info for a direction bearing.
//
//	@parm	The direction to show.
//
/////////////////////////////////////////////////////////////////////////////

void CdGetDir::ShowBearing ( const CeDirection* const pDir ) {

	const CeBearing* const pBearing =
		dynamic_cast<const CeBearing* const>(pDir);
	if ( !pBearing ) return;

//	There should be nothing in the backsight field.
	m_pBacksight = 0;
	m_pPar1 = 0;
	m_pPar2 = 0;

//	Define the bearing.

	m_Radians = pBearing->GetBearing();
	GetDlgItem(IDC_DIRECTION)->SetWindowText(RadStrDms(m_Radians));

//	Always comes back clockwise.

	m_IsClockwise = TRUE;

	TurnRadioOn(IDC_CLOCKWISE);
	TurnRadioOn(IDC_COUNTER_CLOCKWISE);

	CheckDlgButton(IDC_CLOCKWISE,1);
	CheckDlgButton(IDC_COUNTER_CLOCKWISE,0);

} // end of ShowBearing
         */

        /*
//	@mfunc	Display info for a direction defined as parallel to
//			2 other points.
//
//	@parm	The direction to show.
//
/////////////////////////////////////////////////////////////////////////////

void CdGetDir::ShowParallel ( const CeDirection* const pDir ) {

	const CeParallel* const pPar =
		dynamic_cast<const CeParallel* const>(pDir);
	if ( !pPar ) return;

//	Pick up the points that define the parallel.
	m_pPar1 = pPar->GetpStart();
	m_pPar2 = pPar->GetpEnd();

//	Display the IDs of the 2 points.
	CHARS str[132];
	sprintf ( str, "%d->", m_pPar1->FormatKey() );
	strcat ( str, m_pPar2->FormatKey() );
	GetDlgItem(IDC_DIRECTION)->SetWindowText(str);

//	Disallow the ability to say whether the direction is clockwise
//	or otherwise.

	CheckDlgButton(IDC_CLOCKWISE,0);
	CheckDlgButton(IDC_COUNTER_CLOCKWISE,0);
	TurnRadioOff(IDC_CLOCKWISE);
	TurnRadioOff(IDC_COUNTER_CLOCKWISE);

//	The angle value should be undefined.
	m_Radians = 0.0;
	m_IsClockwise = TRUE;

} // end of ShowParallel
         */

        /*
//	@mfunc	Display any offset info for a direction.
//
//	@parm	The direction that may have an offset.
//
/////////////////////////////////////////////////////////////////////////////

void CdGetDir::ShowOffset ( const CeDirection* const pDir ) {

//	Initialize offset-related data members.
	m_IsRight = TRUE;
	m_Offset = CeDistance();
	m_pOffset = 0;

//	Return if there is no offset.
	const CeOffset* const pOffset = pDir->GetpOffset();
	if ( !pOffset ) return;


//	It could be one of 2 types of offset.

	const CeOffsetDistance* const pDist =
		dynamic_cast<const CeOffsetDistance* const>(pOffset);

	if ( pDist ) {

//		Display the offset distance, in the current data entry units.
//		Setting the window text will end up calling OnChangeOffset,
//		which calls OnOffsetDistance, where it will be deduced that
//		we have an offset to the right. This may or may not be correct.

		m_Offset = pDist->GetOffset();
		const CeDistanceUnit& entry = CeMap::GetpMap()->GetEntryUnit();
		GetDlgItem(IDC_OFFSET)->SetWindowText(m_Offset.Format(entry));

//		Override whatever we got above.

		m_IsRight = pDist->IsRight();

		if ( m_IsRight )
			OnRight();
		else
			OnLeft();
	}
	else {

//		It SHOULD be an offset point.

		const CeOffsetPoint* const pPoint =
			dynamic_cast<const CeOffsetPoint* const>(pOffset);

		if ( pPoint ) {

//			Display the ID of the offset point.

			m_pOffset = pPoint->GetpPoint();
			ShowKey(IDC_OFFSET,m_pOffset);

//			Disable the radio buttons to allow specification to
//			the right or the left.
			TurnRadioOff(IDC_LEFT);
			TurnRadioOff(IDC_RIGHT);

		}
		else {
			AfxMessageBox("CdGetDir::ShowOffset\nUnexpected type of offset.");
			return;
		}
	}

} // end of ShowOffset
         */

        /*
//	@mfunc	Handle activation of this dialog.
//
/////////////////////////////////////////////////////////////////////////////

BOOL CdGetDir::OnSetActive ( void ) {

//	Enable standard buttons
	CdPage::OnSetActive();

//	Make sure everything is drawn on top.
	this->OnDrawAll(TRUE);
	return TRUE;

} // end of OnSetActive
         */

        private void useCenterCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Draw any existing backsight point normally.
            //SetNormalColour(m_pBacksight);

            // Toggle the status.
            m_WantCentre = !m_WantCentre;

            // If the user now wants to use the centre point as backsight
            if (m_WantCentre)
            {
                // How many circles were incident on the from-point?
                int ncircle = m_Circles.Count;

                // There SHOULD be at least one (otherwise the checkbox that
                // leads to this function should have been disabled).
                if (ncircle==0)
                {
                    MessageBox.Show("The from-point does not coincide with any circular arcs.");
                    useCenterCheckBox.Checked = false;
                    return;
                }

                // Get the circle involved. If there's more than one, we need
                // to ask the user which one.
                Circle circle = m_Circles[0];
                if (ncircle > 1)
                {
                    // Ask the user to select a circle.
                    GetCircleForm dial = new GetCircleForm(m_Circles);
                    if (dial.ShowDialog() != DialogResult.OK)
                    {
                        useCenterCheckBox.Checked = false;
                        dial.Dispose();
                        return;
                    }

                    circle = dial.Circle;
                    dial.Dispose();
                }

                // Get the point at the center.
                m_Backsight = circle.CenterPoint;

                // Confirm that we got something.
                if (m_Backsight==null)
                {
                    MessageBox.Show("Cannot find the center point.");
                    useCenterCheckBox.Checked = false;
                    return;
                }

		        // Set colour
                //m_Focus = IDC_BACKSIGHT;
                //SetColour(m_pBacksight);

		        // Display the key of the backsight.
		        ShowKey(backsightTextBox, m_Backsight);

		        // Disable the backsight field.
                backsightTextBox.Enabled = false;

        		// Resume in the direction field.
                angleTextBox.Focus();
            }
            else
            {
                // Reset and enable backsight stuff.
                m_Backsight = null;
                backsightTextBox.Enabled = true;
                backsightTextBox.Text = String.Empty;

                // Resume in the backsight field.
                backsightTextBox.Focus();
            }

            //OnNewDirection();
            ErasePainting();
        }

        void ShowKey(TextBox tb, PointFeature point)
        {
            string keystr = String.Empty;
            if (point!=null)
            {
                keystr = point.FormattedKey;
                if (keystr.Length==0)
                    keystr = "+";
            }

            tb.Text = keystr;
        }

        void GetCircles()
        {
            useCenterCheckBox.Checked = false;
            useCenterCheckBox.Visible = false;
            m_WantCentre = false;

            if (m_Circles==null)
                m_Circles = new List<Circle>();

            if (m_From!=null)
            {
                // Sometimes the BC or EC is apparently not EXACT when the data
                // arrives from a foreign source, so allow 1mm on the ground.
                ILength tol = new Length(0.001);
                m_Circles = CadastralMapModel.Current.FindCircles(m_From, tol);

                // If the from point lies on a curve, make the
                // "use centre" checkbox visible.
                if (m_Circles.Count>0)
                    useCenterCheckBox.Visible = true;
            }
            else
                m_Circles.Clear();
        }

        private void setDefaultOffsetButton_Click(object sender, EventArgs e)
        {
            // The following message should have been trapped already,
            // displayed only to reveal potential bug
            if (m_OffsetPoint!=null)
            {
                MessageBox.Show("Can't set default if offset is defined via a point feature.");
                return;
            }

            // Handle case where no offset is defined (remove from registry)
            if (Math.Abs(m_OffsetDistance.Meters) < Constants.TINY)
            {
                GlobalUserSetting.Write("DirectionOffset", String.Empty);
                return;
            }

            // Handle a defined value, prepending with a "-" character
            // if the offset is to the left
            string value = m_OffsetDistance.Format();
            if (!m_IsRight)
                value = "-" + value;
            GlobalUserSetting.Write("DirectionOffset", value);

            // Remember the new default (it's different from m_OffsetDistance if
            // it's an offset to the left)
            m_DefaultOffset = new Distance(value);
            m_DefaultOffset.SetFixed();

            // Disable the button (the user has to change it in order to re-enable)
            setDefaultOffsetButton.Enabled = false;
        }

        /// <summary>
        /// Indicates that any painting previously done by a command should be erased. This
        /// tells the command's active display that it should revert the display buffer to
        /// the way it was at the end of the last draw from the map model. Given that a command
        /// supports painting, it's <c>Paint</c> method will be called during idle time.
        /// </summary>
        internal void ErasePainting()
        {
            EditingController.Current.ActiveDisplay.RestoreLastDraw();
        }
    }
}
