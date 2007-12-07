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

        internal GetDistanceControl()
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
        /// Observed distance (either a <see cref="Distance"/> or an <see cref="OffsetPoint"/>).
        /// </summary>
        internal Observation ObservedDistance
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
            // Initialize combo box with a list of all line entity types
            // for the currently active editing layer.
            ILayer layer = EditingController.Current.ActiveLayer;
            CommandUI.LoadEntityCombo(lineTypeComboBox, SpatialType.Line, layer);

/*	
//	If we are updating a feature that was previously created,
//	load the original info. For distance-distance intersections,
//	we need to know which page this is, to determine whether we
//	should display info for the 1st or 2nd distance.

	ShowUpdate(this->m_PageNum);
*/
        }

        /// <summary>
        /// Initialize for an update (or recall)
        /// </summary>
        /// <param name="op">The edit that is being updated or recalled</param>
        /// <param name="pageNum">The number of the point involved (relevant only for
        /// a <see cref="IntersectTwoDistancesOperation"/>)</param>
        internal void ShowUpdate(IntersectOperation op, int pageNum)
        {
            if (op==null)
                return;

            if (op.EditId == EditingActionId.DistIntersect)
            {
            }
            else if (op.EditId == EditingActionId.DirDistIntersect)
            {
            }
            else
            {
                MessageBox.Show("GetDistanceControl.ShowUpdate - Unexpected editing operation");
            }
        }
        /*
//	@mfunc	Initialize dialog for an update.
//
//	@parm	The distance number (1 or 2). A value of 2 is only
//			valid for CdIntersectDist parents.
void CdGetDist::ShowUpdate ( const UINT1 distnum ) {

//	Return if no update object (and no recall op).
	const CeIntersect* pop = GetUpdateOp();
	if ( pop==0 ) pop = GetRecall();
	if ( !pop ) return;

//	Populate the dialog, depending on what sort of operation
//	we have.

	switch ( pop->GetType() ) {

	case CEOP_DIST_INTERSECT: {

		const CeIntersectDist* pOper = dynamic_cast<const CeIntersectDist*>(pop);

		if ( distnum==1 )
			this->Show( pOper->GetpFrom1()
					  , pOper->GetpDist1()
					  , pOper->GetpArc1() );
		else
			this->Show( pOper->GetpFrom2()
					  , pOper->GetpDist2()
					  , pOper->GetpArc2() );

		break;
	}

	case CEOP_DIRDIST_INTERSECT: {

		const CeIntersectDirDist* pOper = dynamic_cast<const CeIntersectDirDist*>(pop);
		this->Show ( pOper->GetpDistFrom()
				   , pOper->GetpDist()
				   , pOper->GetpDistArc() );

		break;
	}
         */

        /*
public:
	virtual void OnSelectPoint ( CePoint* pPoint );
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
