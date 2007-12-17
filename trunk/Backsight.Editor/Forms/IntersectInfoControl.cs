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

using Backsight.Environment;
using Backsight.Editor.Operations;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" was="CdIntersectOne" />
    /// <summary>
    /// Information about an intersection point.
    /// </summary>
    partial class IntersectInfoControl : UserControl
    {
        #region Class data

        /// <summary>
        /// ID and entity type for the intersection point.
        /// </summary>
        IdHandle m_PointId;

        /// <summary>
        /// First direction (if applicable)
        /// </summary>
        Direction m_Dir1;

        /// <summary>
        /// Second direction (if applicable)
        /// </summary>
        Direction m_Dir2;

        /// <summary>
        /// Entity type for line 1 (if any)
        /// </summary>
        IEntity m_Ent1;

        /// <summary>
        /// Entity type for line 2 (if any)
        /// </summary>
        IEntity m_Ent2;

        /// <summary>
        /// The intersection (if any)
        /// </summary>
        IPosition m_Intersect; // was also transient m_pPoint object

        /// <summary>
        /// Type of intersection.
        /// </summary>
        EditingActionId m_Type; // was local XTYPE enum

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor sets everything to null.
        /// </summary>
        public IntersectInfoControl()
        {
            InitializeComponent();

            // No point feature at the intersection.
            m_Intersect = null;
            m_PointId = null;

            // The type of intersection we are dealing with.
            m_Type = EditingActionId.Null;

            // No direction info so far.
            ResetOtherInfo(false);
        }

        #endregion

        /// <summary>
        /// Is the "Other Intersection" button visible? Should be set true when dealing
        /// with edits where there is a possibility of two intersections (e.g. as in
        /// a distance-distance intersection).
        /// </summary>
        public bool CanHaveTwoIntersections
        {
            get { return otherButton.Visible; }
            set { otherButton.Visible = value; }
        }

        internal void InitializeControl(IntersectForm parent)
        {
            if (parent is IntersectTwoDirectionsForm)
                m_Type = EditingActionId.DirIntersect;
            else
                throw new Exception("IntersectInfoControl.InitializeControl - Unexpected parent window.");

            // Ask the enclosing property sheet whether we are updating
            // an existing feature or not.

            // If we are updating a previously existing point, select
            // the previously defined entity type.
            IntersectOperation op = parent.GetUpdateOp();
            if (op==null)
            {
                // Load the entity combo box with a list for point features.
                IEntity ent = pointTypeComboBox.Load(SpatialType.Point);

                // Load the ID combo (reserving the first available ID).
                IdHelper.LoadIdCombo(pointIdComboBox, ent, m_PointId, true);


                // If we are auto-numbering, disable the ID combo.
                EditingController controller = EditingController.Current;
                if (controller.IsAutoNumber)
                    pointIdComboBox.Enabled = false;
            }
            else
            {
                // Select the entity type previously defined for the 
                // intersection point.
                PointFeature feat = op.IntersectionPoint;
                m_PointId = new IdHandle(feat);

                // Load the entity combo box with a list for point features
                // and disable it.
                pointTypeComboBox.Load(SpatialType.Point, feat.BaseLayer);
                pointTypeComboBox.Enabled = false;

                // Scroll the entity combo to the previously defined
                // entity type for the intersection point.
                IEntity curEnt = feat.EntityType;
                if (curEnt!=null)
                    pointTypeComboBox.SelectEntity(curEnt);

                // Display the point key (if any) and disable it.
                pointIdComboBox.Text = m_PointId.FormattedKey;
                pointIdComboBox.Enabled = false;
            }
        }

        private void pointTypeComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            /*
	// Get the new point type.
	CeEntity* pEnt = ReadEntityCombo(IDC_POINT_TYPE);

	// If the current ID does not apply to the new point type,
	// reload the ID combo (reserving a different ID).
	if ( !m_PointId.IsValidFor(pEnt) ) {
		CComboBox* pIdBox = (CComboBox*)GetDlgItem(IDC_POINT_ID);
		LoadIdCombo(pIdBox,pEnt,&m_PointId);
	}
	else
		m_PointId.SetEntity(pEnt);
             */
        }

        private void pointIdComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            /*
	CComboBox* pBox = (CComboBox*)GetDlgItem(IDC_POINT_ID);
	UINT4 id = ReadIdCombo(pBox);
	m_PointId.ReserveId(m_PointId.GetpEntity(),id);
             */
        }

        internal bool OnSetActive()
        {
            return false;
        /*
//	Enable standard buttons
	CdPage::OnSetActive();

//	Make sure any previous intersection info is initialized.
	this->ResetOtherInfo();

//	Display initial intersection positions (undefined).
	ShowIntersection();

//	Ask the enclosing property sheet to supply pointers to
//	the directions.

	CdDialog* pParent = (CdDialog*)GetParent();

	if ( m_Type==XTY_DIRDIR ) {

		CdIntersectDir* pDial =
			dynamic_cast<CdIntersectDir*>(pParent);

//		Try to get two directions & two line entity types.

		m_pDir1 = pDial->m_Course1.GetpDir();
		m_pDir2 = pDial->m_Course2.GetpDir();

		m_pEnt1 = pDial->m_Course1.GetpLineType();
		m_pEnt2 = pDial->m_Course2.GetpLineType();

//		If there is actually an intersection, show it.
		if ( m_pDir1 && m_pDir2 ) {
			if ( m_pDir1->Intersect(*m_pDir2,m_Intersect) )
				this->ShowIntersection();
		}

//		If there is no intersection, disable the finish button.
		if ( !m_Intersect.IsDefined() ) TurnFinishOff();
	}

	return TRUE;
         */
        }

        internal bool OnWizardFinish()
        {
            return false;
            /*
//	The intersection SHOULD be defined.
	if ( !m_Intersect.IsDefined() ) {
		AfxMessageBox ( "No intersection. Nothing to save" );
		return FALSE;
	}

	if ( GetUpdateOp() ) {

		// Correct the operation.
		Correct();
	}
	else {

		// Save the intersection.
		CePoint* ptSave = this->Save();
		if ( !ptSave ) {
			AfxMessageBox ( "Failed to save intersection." );
			return FALSE;
		}

		// Get the view to select the intersection point.
		CdDialog* pParent = (CdDialog*)GetParent();
		if ( pParent ) pParent->Select(ptSave);
	}

//	Close up the dialog
	return CdPage::OnWizardFinish();
             */
        }

        /*
//	@mfunc	Save the dialog (it should already by validated).
//
//	@rdesc	Pointer to the point feature at the intersection (null
//			if something went wrong).
//
/////////////////////////////////////////////////////////////////////////////

CePoint* CdIntersectOne::Save ( void ) {

	if ( m_Type == XTY_DIRDIR )
		return SaveDirDir();
	else {
		AfxMessageBox("CdIntersectOne::Save\nUnexpected sort of intersection.");
		return 0;
	}

} // end of Save
         */

        /*
//	@mfunc Save a direction-direction intersection.
//
//	@rdesc Pointer to the point feature at the intersection (null
//	if something went wrong).
//
/////////////////////////////////////////////////////////////////////////////

CePoint* CdIntersectOne::SaveDirDir ( void ) {

	// Create empty persistent object.
	CeMap* pMap = CeMap::GetpMap();
	CeIntersectDir* pSave = new ( os_database::of(pMap),
								  os_ts<CeIntersectDir>::get() )
								  CeIntersectDir();

	// Tell map a save is starting.
	pMap->SaveOp(pSave);

	// Execute the operation.
	LOGICAL ok = pSave->Execute(	*m_pDir1
								,	*m_pDir2
								,	m_PointId
								,	m_pEnt1
								,	m_pEnt2 );

	// Tell map save has finished.
	pMap->SaveOp(pSave,ok);

	// If everything ok, return pointer to intersection point.
	if ( ok ) return pSave->GetpIntersect();

	// Otherwise get rid of the persistently allocated memory
	// and return nothing.
	delete pSave;
	return 0;

} // end of SaveDirDir
         */

        void ResetOtherInfo(bool erase)
        {
        }
        /*
//	@mfunc	Reset data members that refer to things coming from
//			other property pages.
//
//	@parm	TRUE (default) if you want to erase previously
//			displayed info. The only time this should be passed
//			in as FALSE is during the constructor for this
//			dialog, because at that time, there cannot be any
//			previously displayed info (and it may in fact be
//			that we have junk floating around).
//
/////////////////////////////////////////////////////////////////////////////

void CdIntersectOne::ResetOtherInfo ( const LOGICAL erase ) {

//	If we were displaying an intersection point, erase it.
	if ( erase ) this->Erase();

//	First direction
	m_pDir1 = 0;
	m_pEnt1 = 0;

//	Second direction
	m_pDir2 = 0;
	m_pEnt2 = 0;

//	The position of the intersection
	m_Intersect.Reset();

} // end of ResetOtherInfo
         */

        /*
void CdIntersectOne::ShowIntersection ( void ) {

//	Get pointers to northing & easting edit boxes.
	CEdit* pNorthing = (CEdit*)GetDlgItem(IDC_NORTHING);
	CEdit* pEasting  = (CEdit*)GetDlgItem(IDC_EASTING);

//	If we were already showing an intersection point, erase it.
	this->Erase();

	if ( m_Intersect.IsDefined() ) {

//		Display the northing and easting.
		CHARS str[32];
		sprintf ( str, "%.4lf", m_Intersect.GetNorthing() );
		pNorthing->SetWindowText(str);
		sprintf ( str, "%.4lf", m_Intersect.GetEasting() );
		pEasting->SetWindowText(str);

//		Create point symbol at the intersection & ask the
//		view to draw it in magenta.
		m_pPoint = new CePoint(m_Intersect);
		this->OnDraw(0);
	}
	else {
		pNorthing->SetWindowText("<undefined>");
		pEasting->SetWindowText("<undefined>");
	}

} // end of ShowIntersection
         */

        /*
//	Erase the intersection point. This will also ask the enclosing dialog to
//	refresh any other transient things that may be drawn.

void CdIntersectOne::Erase ( void ) {

//	Erase any segments (do BEFORE we erase the point).
	this->EraseSegments();

//	Erase the point.
	if ( m_pPoint ) {
		m_pPoint->Erase();
		delete m_pPoint;
		m_pPoint = 0;
	}

//	Ask the enclosing dialog to refresh things.
	CdDialog* pDial = (CdDialog*)GetParent();
	if ( pDial ) pDial->OnDraw();

} // end of Erase
         */

        internal void OnDraw(PointFeature point)
        {
        }

        /*
//	Redraw the intersection point (if any). The supplied point
//	is only there because the pure virtual defined elsewhere
//	requires it. This function is needed so that the transient
//	intersection will still appear when the user zooms or pans
//	the map while this dialog is up.

void CdIntersectOne::OnDraw ( const CePoint* const pPoint ) const {

	CeView* pView = GetpView();
	if ( !pView ) return;

//	Draw the point in magenta.
	if ( m_pPoint ) {
		pView->Draw(*m_pPoint,COL_MAGENTA);
		this->ShowSegments();
	}

} // end of OnDraw
         */

        /*
//	Show lines if arcs will be added.

void CdIntersectOne::ShowSegments ( void ) const {

	if ( !m_pPoint ) return;

	CeVertex start;
	CeVertex end;

	CeView* pView = GetpView();
	if ( !pView ) return;

	if ( GetLine1(start,end) ) pView->Draw(start,end);
	if ( GetLine2(start,end) ) pView->Draw(start,end);

} // end of ShowSegments
         */

        /*
//	Erase lines if arcs will be added.

void CdIntersectOne::EraseSegments ( void ) const {

	if ( !m_pPoint ) return;

	CeVertex start;
	CeVertex end;

	CeView* pView = GetpView();
	if ( !pView ) return;

	if ( GetLine1(start,end) ) pView->Erase(start,end);
	if ( GetLine2(start,end) ) pView->Erase(start,end);

} // end of EraseSegments
         */

        /*
LOGICAL CdIntersectOne::GetLine1 ( CeVertex& start, CeVertex& end ) const {

//	No line if no intersection.
	if ( !m_pPoint ) return FALSE;

//	No line if no entity type for a line.
	if ( !m_pEnt1 ) return FALSE;

//	Get position of the start of line.
	start = CeVertex(*m_pDir1);

//	The end position is the position of the intersection.
	end = CeVertex(*m_pPoint);
	return TRUE;

} // end of GetLine1
         */

        /*
LOGICAL CdIntersectOne::GetLine2 ( CeVertex& start, CeVertex& end ) const {

//	No line if no intersection.
	if ( !m_pPoint ) return FALSE;

//	No line if no entity type for a line.
	if ( !m_pEnt2 ) return FALSE;

//	No line if no direction for it.
	if ( !m_pDir2 ) return FALSE;

//	Get position of the start of line.
	start = CeVertex(*m_pDir2);

//	The end position is the position of the intersection.
	end = CeVertex(*m_pPoint);
	return TRUE;

} // end of GetLine2
         */

        /*
//	@mfunc	Correct the creating op using the info from the dialog.
//
/////////////////////////////////////////////////////////////////////////////

void CdIntersectOne::Correct ( void ) const {

	CeIntersect* pop = GetUpdateOp();
	assert(pop);

	if ( m_Type == XTY_DIRDIR ) {
		
		CeIntersectDir* pOper =
			dynamic_cast<CeIntersectDir*>(pop);

		pOper->Correct ( *m_pDir1
					   , *m_pDir2
					   , m_pEnt1
					   , m_pEnt2 );
	}
	else {

		AfxMessageBox("Unexpected type of intersection");

	}

} // end of Correct
         */
    }
}
