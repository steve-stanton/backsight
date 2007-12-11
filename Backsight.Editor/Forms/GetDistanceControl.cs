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
using System.Diagnostics;

using Backsight.Environment;
using Backsight.Editor.Operations;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" was="CdGetDist" />
    /// <summary>
    /// Dialog for getting the user to specify a distance (for use with
    /// Intersection edits).
    /// </summary>
    partial class GetDistanceControl : UserControl
    {
        #region Class data

        // Data for operation ...

        /// <summary>
        /// The point the distance was observed from.
        /// </summary>
        PointFeature m_From;

        /// <summary>
        /// Observed distance (either m_Distance or m_OffsetPoint).
        /// </summary>
        Observation m_ObservedDistance; // was m_pDistance

        /// <summary>
        /// Type of line (if any).
        /// </summary>
        IEntity m_LineType;

        // View-related ...

        /// <summary>
        /// Currently displayed circle.
        /// </summary>
        Circle m_Circle;

        /// <summary>
        /// Point used to specify distance.
        /// </summary>
        PointFeature m_DistancePoint;

        /// <summary>
        /// Entered distance value (if specified that way).
        /// </summary>
        Distance m_Distance;

        /// <summary>
        /// The offset point (if specified that way).
        /// </summary>
        OffsetPoint m_OffsetPoint;

        #endregion

        #region Constructors

        public GetDistanceControl()
        {
            InitializeComponent();

            m_From = null;
            m_ObservedDistance = null;
            m_LineType = null;
            m_Circle = null;
            m_DistancePoint = null;
            m_Distance = null;
            m_OffsetPoint = null;
        }

        #endregion

        /// <summary>
        /// The title that appears around the UI elements dealing with the entered distance
        /// </summary>
        public string DistanceTitle
        {
            get { return distanceGroupBox.Text; }
            set { distanceGroupBox.Text = value; }
        }

        /// <summary>
        /// Observed distance (either a <see cref="Distance"/> or an <see cref="OffsetPoint"/>).
        /// </summary>
        internal Observation ObservedDistance // was GetpDist
        {
            get { return m_ObservedDistance; }
        }

        /// <summary>
        /// The point the distance was observed from.
        /// </summary>
        internal PointFeature From
        {
            get { return m_From; }
        }

        /// <summary>
        /// Type of line (if any).
        /// </summary>
        internal IEntity LineType
        {
            get { return m_LineType; }
        }

        private void GetDistanceControl_Load(object sender, EventArgs e)
        {
            if (DesignMode)
                return;

            // Initialize combo box with a list of all line entity types
            // for the currently active editing layer.
            lineTypeComboBox.Load(SpatialType.Line);

/*	
//	If we are updating a feature that was previously created,
//	load the original info. For distance-distance intersections,
//	we need to know which page this is, to determine whether we
//	should display info for the 1st or 2nd distance.

	ShowUpdate(this->m_PageNum);
*/
        }

        private void distanceTextBox_TextChanged(object sender, EventArgs e)
        {
            /*
	if ( IsFieldEmpty(IDC_DISTANCE) ) {

//		If we already had direction info, reset it.
		SetNormalColour(m_pDistancePoint);
		m_pDistancePoint = 0;
		m_Distance = CeDistance();
		OnNewDistance();

	}
	else {

//		The distance could have been specified by the
//		user, or it could have been set as the result of
//		a pointing operation. In the latter case,
//		m_pDistancePoint will be defined.

		if ( !m_pDistancePoint ) {

//			Explicitly entered by the user.

//			Parse the distance.
			ParseDistance();
		}
	}
             */
        }

        private void distanceTextBox_Leave(object sender, EventArgs e)
        {
            /*

//	No validation if the distance is being specified by pointing (in
//	that case, losing the focus is ok.
	if ( m_pDistancePoint ) {
		OnNewDistance();
		return;
	}

//	Return if the field is empty.
	if ( IsFieldEmpty(IDC_DISTANCE) ) {
		OnNewDistance();
		return;
	}

//	Parse the distance.
	ParseDistance();
             */
        }

        private void fromPointTextBox_TextChanged(object sender, EventArgs e)
        {
            /*
//	If the field is now empty, ensure that the from point
//	is undefined.

	if ( IsFieldEmpty(IDC_FROMPOINT) ) {
		SetNormalColour(m_pFrom);
		m_pFrom = 0;
		OnNewDistance();
	}
	else if ( !m_pFrom )
		AfxMessageBox
		( "You can only specify the from-point by pointing at the map." );
             */
        }

        private void fromPointTextBox_Leave(object sender, EventArgs e)
        {
            /*
//	See if a new circle should be drawn.
	OnNewDistance();	
             */
        }

        private void lineTypeComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            /*
//	Get the new selection (if any)
	m_pLineType = ReadEntityCombo(IDC_LINE_TYPE);

//	If we have everything we need, move directly to the
//	next page. Otherwise move to the first field we need.
	if ( m_pFrom && m_pDistance ) {
		CdDialog* pDial = (CdDialog*)GetParent();
		pDial->PressButton(PSBTN_NEXT);
	}
	else if ( !m_pFrom )
		GetDlgItem(IDC_FROMPOINT)->SetFocus();
	else
		GetDlgItem(IDC_DISTANCE)->SetFocus();
             */
        }

        /// <summary>
        /// Reacts to selection of a point on the map.
        /// </summary>
        /// <param name="point"></param>
        internal void OnSelectPoint(PointFeature point)
        {
        }
        /*
void CdGetDist::OnSelectPoint ( CePoint* pPoint ) {

//	Return if point is not defined.
	if ( !pPoint ) return;

//	Return if point is not valid.
	if ( !IsPointValid() ) return;

//	Set colour
	SetColour(pPoint);

//	Handle the pointing, depending on what field we were
//	last in.

	switch ( m_Focus ) {

	case IDC_FROMPOINT: {

//		Save the from point.
		SetNormalColour(m_pFrom);
		m_pFrom = pPoint;

//		Display it (causes a call to OnChangeFromPoint).
		GetDlgItem(IDC_FROMPOINT)->SetWindowText(m_pFrom->FormatKey());

//		Move focus to the distance field.
		GetDlgItem(IDC_DISTANCE)->SetFocus();

		return;
	}

	case IDC_DISTANCE: {

//		The distance must be getting specified by pointing
//		at an offset point.

//		Define either the first or the second parallel point.
		SetNormalColour(m_pDistancePoint);
		m_pDistancePoint = pPoint;

//		Display the point number.
		GetDlgItem(IDC_DISTANCE)->SetWindowText
			(m_pDistancePoint->FormatKey());

//		Move focus to the line type.
		GetDlgItem(IDC_LINE_TYPE)->SetFocus();

		return;
	}
	
	default:

		return;

	} // end switch

} // end of OnSelectPoint
         */

        /*
//	@mfunc Check if it is OK to accept a selected point in the field
//	that last had the input focus.
//
/////////////////////////////////////////////////////////////////////////////

LOGICAL CdGetDist::IsPointValid ( void ) const {

	CHARS str[132];

	switch ( m_Focus ) {

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

	case IDC_DISTANCE: {

//		The distance must be getting specified by pointing
//		at an offset point.

//		Disallow if offset point has already been specified.
		if ( m_pDistancePoint ) {
			AfxMessageBox (
				"You have already specified a distance offset point." );
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

        /*
//	@mfunc Check whether the current data is enough to
//	construct a distance circle. If so, draw it. Remember
//	to erase any previously drawn circle.
//
/////////////////////////////////////////////////////////////////////////////

void CdGetDist::OnNewDistance ( void ) {

	CeCircle* pCircle=0;	// Constructed circle.

//	If we had a circle before, erase it.
	if ( m_pCircle ) {
		m_pCircle->Erase();
		delete m_pCircle;
		m_pCircle = 0;
	}

//	Undefine the address of the relevant distance observation.
	m_pDistance = 0;

	if ( m_pFrom && (m_pDistancePoint || m_Distance.GetMetric()>TINY) ) {

		FLOAT8 radius;

//		If we have an offset point, get the radius.
		if ( m_pDistancePoint ) {
			CeVertex centre(*m_pFrom);
			CeVertex edge(*m_pDistancePoint);
			radius = sqrt(centre.DistanceSquared(edge));
		}
		else
			radius = m_Distance.GetMetric();

//		Construct circle & draw it.
		m_pCircle = new CeCircle(*m_pFrom,radius);
		m_pCircle->Draw();

//		Create the appropriate distance observation (this is what
//		gets picked up when we actually go to work out the
//		intersection on the last page of the property sheet --
//		see CdIntersectTwo).

		if ( m_pDistancePoint ) {
			m_OffsetPoint.SetPoint(*m_pDistancePoint);
			m_pDistance = &m_OffsetPoint;
		}
		else
			m_pDistance = &m_Distance;
	}

} // end of OnNewDistance
         */

        /*
//	@mfunc Parse an explicitly entered distance.
//
//	@rdesc TRUE if distance parses ok.
//
/////////////////////////////////////////////////////////////////////////////

LOGICAL CdGetDist::ParseDistance ( void ) {
			
//	Get the entered string.
	CHARS str[32];
	CEdit* pEdit = (CEdit*)GetDlgItem(IDC_DISTANCE);
	pEdit->GetWindowText(str,sizeof(str)-1);

//	No distance if empty string (ignore any trailing
//	white space).
	UINT4 slen = StrLength(str);
	str[slen] = '\0';
	if ( slen==0 ) {
		m_Distance = CeDistance();
		return TRUE;
	}

//	Parse the distance.
	m_Distance = CeDistance(str);

//	Parsing was successful if the distance comes back
//	as a defined value.

	if ( !m_Distance.IsDefined() ) {
		AfxMessageBox ( "Invalid distance." );
		pEdit->SetFocus();
		return FALSE;
	}

	OnNewDistance();
	return TRUE;

} // end of ParseDistance
         */

        /*
//	@mfunc Set colour for a point.
//
//	@parm The point to draw.
//	@parm The ID of the field that the point relates to. The
//	default is the field that currently has the focus.
//
/////////////////////////////////////////////////////////////////////////////

void CdGetDist::SetColour ( const CePoint* const pPoint, const UINT id ) const {

//	Return if point not specified.
	if ( !pPoint ) return;

//	Determine the colour.

	COL col;
	UINT field = id;
	if ( !field ) field = m_Focus;

	switch ( field ) {
	case IDC_FROMPOINT: col = COL_LIGHTBLUE;	break;
	case IDC_DISTANCE:	col = COL_YELLOW;		break;
	default:			return;
	}

//	Ask the enclosing dialog to set the colour.

	CdDialog* pDial = (CdDialog*)GetParent();
	if ( pDial ) pDial->SetColour(*pPoint,col);

	return;

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

void CdGetDist::SetNormalColour ( const CePoint* const pPoint ) const {

//	Return if point not specified.
	if ( !pPoint ) return;

//	Ask the enclosing dialog to set the colour to black.

	CdDialog* pDial = (CdDialog*)GetParent();
	if ( pDial ) pDial->SetColour(*pPoint,COL_BLACK);

	return;

} // end of SetNormalColour
         */

        /*
void CdGetDist::OnDraw ( const CePoint* const pPoint ) const {

	if ( !pPoint )
		OnDrawAll();
	else {
		if ( pPoint==m_pFrom )
			SetColour(m_pFrom,IDC_FROMPOINT);
		if ( pPoint==m_pDistancePoint )
			SetColour(m_pDistancePoint,IDC_DISTANCE);
	}
}
         */

        /*
//	@mfunc Handle any redrawing. This just ensures that points
//	are drawn in the right colour, and that any distance circle
//	shown is still there.
//
//	@parm TRUE to draw. FALSE to erase.
//
//	This function is called at the end of the view's OnDraw
//	function. I though about reacting to a WM_PAINT message,
//	but wasn't sure what's going to get drawn first.
//
/////////////////////////////////////////////////////////////////////////////

void CdGetDist::OnDrawAll ( const LOGICAL draw ) const {

//	There's not much to draw, so just do everything.

//	Draw any currently selected points & any distance circle.

	if ( draw ) {
		SetColour(m_pFrom,IDC_FROMPOINT);
		SetColour(m_pDistancePoint,IDC_DISTANCE);
	}
	else {
		SetNormalColour(m_pFrom);
		SetNormalColour(m_pDistancePoint);
	}

	if ( m_pCircle ) {
		if ( draw )
			m_pCircle->Draw();
		else
			m_pCircle->Erase();
	}

	return;

} // end of OnDrawAll
         */

        /// <summary>
        /// Initialize for an update (or recall)
        /// </summary>
        /// <param name="op">The edit that is being updated or recalled</param>
        /// <param name="distNum">The sequence number of the distance involved (relevant only for
        /// a <see cref="IntersectTwoDistancesOperation"/>)</param>
        internal void ShowUpdate(IntersectOperation op, int distNum)
        {
            /*
        // Return if no update object (and no recall op).
        const CeIntersect* pop = GetUpdateOp();
        if ( pop==0 ) pop = GetRecall();
        if ( !pop ) return;
             */

            if (op==null)
                return;

            // Populate the dialog, depending on what sort of operation we have.
            if (op.EditId == EditingActionId.DistIntersect)
            {
                Debug.Assert(distNum==1 || distNum==2);
                IntersectTwoDistancesOperation oper = (IntersectTwoDistancesOperation)op;

                if (distNum==1)
                    ShowDistance(oper.Distance1FromPoint, oper.Distance1, oper.CreatedLine1);
                else
                    ShowDistance(oper.Distance2FromPoint, oper.Distance2, oper.CreatedLine2);
            }
            else if (op.EditId == EditingActionId.DirDistIntersect)
            {
                IntersectDirectionAndDistanceOperation oper = (IntersectDirectionAndDistanceOperation)op;
                ShowDistance(oper.DistanceFromPoint, oper.Distance, oper.CreatedDistanceLine);
            }
            else
            {
                MessageBox.Show("GetDistanceControl.ShowUpdate - Unexpected editing operation");
            }
        }

        /// <summary>
        /// Displays info for a specific distance observation.
        /// </summary>
        /// <param name="from">The point the distance was observed from.</param>
        /// <param name="obsv">The distance observation (either a <see cref="Distance"/>,
        /// or an <see cref="OffsetPoint"/>)</param>
        /// <param name="line">The line to show (null if no line).</param>
        void ShowDistance(PointFeature from, Observation obsv, LineFeature line)
        {
            // If we have an arc, define its entity type and scroll the
            // entity combo box to that type. Note that when the string
            // is selected, it is important that m_From and m_Distance
            // are null; otherwise <mf CdGetDist::OnSelChangeLineType>
            // may automatically move on to the next page of the wizard
            // dialog.

            m_From = null;
            m_Distance = null;
            m_LineType = null;

            if (line!=null)
            {
                m_LineType = line.EntityType;
                lineTypeComboBox.SelectEntity(m_LineType);
            }

            // Define the from-point of the distance
            m_From = from;
            fromPointTextBox.Text = m_From.FormattedKey;

            // What sort of distance observation do we have?
            m_ObservedDistance = null; // m_pDistance
            m_DistancePoint = null;
            m_Distance = null;

            if (obsv is Distance)
            {
                // Display the distance in the original data entry units.
                Distance dist = (obsv as Distance);
                m_Distance = new Distance(dist);
                distanceTextBox.Text = m_Distance.Format();

                // Create the appropriate distance observation (this is what
                // gets picked up when we actually go to work out the intersection.
                // (was on the last page of the property sheet -- see CdIntersectTwo)
                m_ObservedDistance = m_Distance;
            }
            else
            {
                // It SHOULD be an offset point.
                OffsetPoint off = (obsv as OffsetPoint);
                if (off==null)
                    throw new Exception("GetDistanceControl.ShowDistance - Unexpected type of distance.");

                // Get the point involved & display it's ID.
                m_DistancePoint = off.Point;
                distanceTextBox.Text = m_DistancePoint.FormattedKey;

                // Create the appropriate distance observation
                m_OffsetPoint.Point = m_DistancePoint;
                m_ObservedDistance = m_OffsetPoint;
            }
        }

        /*
public:
	virtual void OnDraw ( const CePoint* const pPoint=0 ) const;
         */

        /*
private:

	virtual LOGICAL		IsPointValid	( void ) const;
	virtual void		OnNewDistance	( void );
	virtual void		SetNormalColour	( const CePoint* const pPoint ) const;
	virtual void		SetColour		( const CePoint* const pPoint
										, const UINT id=0 ) const;
	virtual void		OnDrawAll		( const LOGICAL draw=TRUE ) const;
	virtual LOGICAL		ParseDistance	( void );
	virtual void		ShowUpdate		( const UINT1 distnum );
	virtual void		Show			( const CePoint* const pFrom
										, const CeObservation* const pDist
										, const CeArc* const pArc );
         */
    }
}
