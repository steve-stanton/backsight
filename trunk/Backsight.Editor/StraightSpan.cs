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

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="08-FEB-1998" was="CeStraightSpan" />
    /// <summary>
    /// A single span in a straight leg.
    /// </summary>
    class StraightSpan
    {
        #region Class data

        /// <summary>
        /// The leg this span relates to.
        /// </summary>
        StraightLeg m_Leg;

        /// <summary>
        /// Position of start of leg.
        /// </summary>
        double m_LegStartN;

        /// <summary>
        /// Position of start of leg.
        /// </summary>
        double m_LegStartE;

        /// <summary>
        /// The sin(bearing) of the leg.
        /// </summary>
        double m_SinBearing;

        /// <summary>
        /// The cos(bearing) of the leg.
        /// </summary>
        double m_CosBearing;

        /// <summary>
        /// The scale factor to apply to distances on the leg.
        /// </summary>
        double m_ScaleFactor;

        /// <summary>
        /// Index of currently defined span (-1 if span is not defined).
        /// </summary>
        int m_Index;

        /// <summary>
        /// Position of start of span.
        /// </summary>
        IPosition m_Start;

        /// <summary>
        /// Position of end of span.
        /// </summary>
        IPosition m_End;

        /// <summary>
        /// True if the span has a line.
        /// </summary>
        bool m_IsLine;

        /// <summary>
        /// True if there is a point at the end.
        /// </summary>
        bool m_IsEndPoint;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>StraightSpan</c>
        /// </summary>
        /// <param name="leg">The leg that this span falls on.</param>
        /// <param name="start">The position of the start of the leg.</param>
        /// <param name="bearing">The bearing of the leg.</param>
        /// <param name="sfac">The scale factor to apply to distances on the leg.</param>
        StraightSpan(StraightLeg leg, IPosition start, double bearing, double sfac)
        {
            // Hold on to the supplied stuff.
            m_Leg = leg;
            m_LegStartN = start.Y;
            m_LegStartE = start.X;
            m_SinBearing = Math.Sin(bearing);
            m_CosBearing = Math.Cos(bearing);
            m_ScaleFactor = sfac;

            // Initialize values that will be defined via calls to Get().
            m_Index = -1;
            m_IsLine = false;
            m_IsEndPoint = false;
        }

        #endregion

        internal IPosition End
        {
            get { return m_End; }
        }

        /// <summary>
        /// Gets info for a specific span on a leg.
        /// </summary>
        /// <param name="index">Index of the span to get.</param>
        void Get(int index)
        {
            // Ask the leg to return the distance to the start and the
            // end of the requested span.
            double sdist, edist;
            m_Leg.GetDistances(index, out sdist, out edist);

        }
        /*
	FLOAT8 sdist;
	FLOAT8 edist;
	if ( !m_pLeg->GetDistances(index,sdist,edist) ) return FALSE;

//	See if the span has a line and a terminal point.
	m_IsLine = m_pLeg->HasLine(index);
	m_IsEndPoint = m_pLeg->HasEndPoint(index);

//	Define the start position.

	if ( index==0 )
		m_Start = CeVertex(m_LegStartE,m_LegStartN);
	else {
		sdist *= m_ScaleFactor;
		m_Start = CeVertex( m_LegStartE + (sdist*m_SinBearing)
						  , m_LegStartN + (sdist*m_CosBearing) );
	}

//	Define the end position.

	edist *= m_ScaleFactor;
	m_End = CeVertex( m_LegStartE + (edist*m_SinBearing)
		  		    , m_LegStartN + (edist*m_CosBearing) );

//	Remember the requested index
	m_Index = INT2(index);

	return TRUE;

} // end of Get

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Draw this span (if visible).
//
//////////////////////////////////////////////////////////////////////

void CeStraightSpan::Draw ( void ) const {

//	Get the view to do the draw.
	CeDraw* pDraw = GetpDraw();
	if ( !pDraw ) return;

//	Draw the line if it is visible.
	if ( m_IsLine ) 
		pDraw->Draw(m_Start,m_End);
	else
		pDraw->DrawDotted(m_Start,m_End);

//	Draw terminal point if it exists.
	if ( m_IsEndPoint ) pDraw->Draw(m_End);

} // end of Draw

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Erase this span.
//
//////////////////////////////////////////////////////////////////////

void CeStraightSpan::Erase ( void ) const {

//	Get the view to do the draw.
	CeDraw* pDraw = GetpDraw();
	if ( !pDraw ) return;

//	Erase the line (even if no line, it has been drawn dotted)
	pDraw->Erase(m_Start,m_End);

//	Erase terminal point if it exists
	if ( m_IsEndPoint ) pDraw->Erase(m_End);

} // end of Erase

#ifdef _CEDIT
//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Save this span in the map.
//
//	@parm	Pointer a new location that was inserted just before
//			this span. Defined only during rollforward.
//	@parm	Pointer to the feature that was previously associated with
//			this span. This will be non-zero when the span is being
//			saved as part of rollforward processing.
//	@parm	The location at the very end of the connection path
//			that this span is part of.
//
//	@rdesc	Pointer to the feature (if any) that represents the span.
//			If the span has a line, this will be a CeArc pointer. If
//			the span has no line, it may be a CePoint feature at the
//			END of the span. A null pointer is also valid, meaning
//			that there is no line & no terminal point.
//
//////////////////////////////////////////////////////////////////////

#include "CeLocation.h"

CeFeature* CeStraightSpan::Save	( CeLocation* pInsert
								, CeFeature* pOld
								, const CeLocation* const pVeryEnd ) const {

//	Get map info.
	CeMap* pMap = CeMap::GetpMap();

//	Pointer to the created feature (if any).
	CeFeature* pFeat = 0;

//	Make sure the start and end points have been rounded to
//	the internal resolution.
	CeLocation sloc(m_Start);
	CeLocation eloc(m_End);
	CeVertex svtx(sloc);
	CeVertex evtx(eloc);

	// If the span was previously associated with a feature, just
	// move it. If the feature is a line, we want to move the
	// location at the end (except in a case where a new line
	// has just been inserted prior to it, in which case we
	// need to change the start location so that it matches
	// the end of the new guy).

	if ( pOld ) {

		if ( m_IsLine ) {	// Feature should therefore be an arc.
			CeArc* pArc = dynamic_cast<CeArc*>(pOld);
			if ( !pArc )
				ShowMessage("CeStraightSpan::Save\nMismatched line");
			else {
				if ( pInsert ) {
					CeLocation* pE = pArc->GetpEnd();
					pArc->GetpLine()->ChangeEnds(*pInsert,*pE);
					if ( pE!=pVeryEnd ) pE->Move(evtx);
				}
				else {
					if ( pArc->GetpEnd()==pVeryEnd )
						pArc->GetpStart()->Move(svtx);
					else
						pArc->Move(sloc,eloc);
				}
			}
		}
		else if ( m_IsEndPoint ) { // Feature should be a point

			CePoint* pPoint = dynamic_cast<CePoint*>(pOld);
			if ( !pPoint )
				ShowMessage("CeStraightSpan::Save\nMismatched point");
			else {
				if ( pPoint->GetpVertex()!=pVeryEnd )
					pPoint->Move(eloc);
			}
		}
	}
	else {

//		If we have an end point, add it. If it creates something
//		new, assign an ID to it.

		if ( m_IsEndPoint ) {
			LOGICAL isold;
			pFeat = (CeFeature*)pMap->AddPoint(evtx,0,isold);
			if ( !isold ) pFeat->SetNextId();
		}

//		Add a line if we have one.
		if ( m_IsLine ) pFeat = pMap->AddArc(svtx,evtx,0);
	}

	return pFeat;

} // end of Save
#endif

#ifdef _CEDIT
//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Save a newly insert span.
//
//	@parm	The index of the new span.
//	@parm	The operation that the new span should be referred to.
//	@parm	Is the new span going to be the very last span in
//			the last leg of a connection path?
//
//	@rdesc	The line that was created.
//
//////////////////////////////////////////////////////////////////////

#include "CePath.h"

CeArc* CeStraightSpan::SaveInsert ( const UINT2 index
								  , const CePath& creator
								  , const LOGICAL isLast ) {

	// Get the end positions for the new span.
	Get(index);

	// Make sure the start and end points have been rounded to
	// the internal resolution.
	CeLocation sloc(m_Start);
	CeLocation eloc(m_End);
	CeVertex svtx(sloc);
	CeVertex evtx(eloc);

	// Get the location at the start of the span (in most cases,
	// it should be there already -- the only exception is a
	// case where the point was omitted).
	CeMap* pMap = CeMap::GetpMap();
	const CeLocation* pS = pMap->AddLocation(svtx);

	// If the insert is going to be the very last span in the
	// enclosing connection path, just pick up the terminal
	// location of the path.

	const CeLocation* pE=0;

	if ( isLast ) {

		// Pick up the end of the path.
		pE = creator.GetpEnd();

		// And ensure there has been no roundoff in the end position.
		evtx = CeVertex(pE->GetEasting(),pE->GetNorthing());
	}
	else {

		// Does the end coincide with something that is already
		// in the map? If so, create a duplicate of it (otherwise
		// add the end location as normal).

		// If we don't add a duplicate, we could be re-using a
		// location that comes later in the connection path (i.e.
		// it may later be moved again!).

		pE = pMap->FindLocation(&evtx,FALSE);

		if ( pE )
			pE = pMap->AddDuplicate(*pE);
		else
			pE = pMap->AddLocation(evtx);

		// Create a point at the end of the span (if there's a
		// point there already, create a duplicate).
		m_IsEndPoint = TRUE;
		CePoint* pPoint = pMap->AddPoint((CeLocation* const)pE,0);

		// Assign the next available ID to the point, and record
		// the specified op as the creator.
		pPoint->SetNextId();
		pPoint->SetpCreator(creator);

	}

	// Add a line.
	m_IsLine = TRUE;
	CeArc* pArc = pMap->AddArc(svtx,evtx,0);
	pArc->SetpCreator(creator);

	return pArc;

} // end of SaveInsert
         */
    }
}
