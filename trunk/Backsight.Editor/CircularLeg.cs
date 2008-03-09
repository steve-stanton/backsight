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
    /// <written by="Steve Stanton" on="21-JAN-1998" was="CeCircularLeg" />
    /// <summary>
    /// A circular leg in a connection path.
    /// </summary>
    [Serializable]
    class CircularLeg : Leg
    {
        #region Class data

        /// <summary>
        /// First angle. Either at the BC, or a central angle. In radians. It's
        /// a central angle if the FLG_CULDESAC is set.
        /// </summary>
        double m_Angle1;

        /// <summary>
        /// The angle at the EC (in radians). This will only be defined if the FLG_TWOANGLES
        /// flag bit is set (if not set, this value will be 0.0).
        /// </summary>
        double m_Angle2;

        /// <summary>
        /// Observed radius.
        /// </summary>
        Distance m_Radius;

        /// <summary>
        /// The circle that this leg sits on.
        /// </summary>
        Circle m_Circle;

        /// <summary>
        /// Flag bits
        /// </summary>
        CircularLegFlag m_Flag;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        CircularLeg()
            : base(0)
        {
            m_Angle1 = 0.0;
            m_Angle2 = 0.0;
            m_Radius = null;
            m_Circle = null;
            m_Flag = 0;
        }

        /// <summary>
        /// Creates a new <c>CircularLeg</c> with no spans.
        /// </summary>
        /// <param name="radius">The radius of the circular leg.</param>
        /// <param name="clockwise">True if the curve is clockwise.</param>
        /// <param name="span">The number of spans on the curve.</param>
        CircularLeg(Distance radius, bool clockwise, int nspan)
            : base(nspan)
        {
            // Angles were not specified.
            m_Angle1 = 0.0;
            m_Angle2 = 0.0;

            // Start out with undefined flag.
            m_Flag = 0;

            // Remember the radius.
            m_Radius = radius;

            // The circle for this leg won't be known till we create a span.
            m_Circle = null;

            // Remember if its NOT a clockwise curve.
            if (!clockwise)
                m_Flag |= CircularLegFlag.CounterClockwise;
        }

        #endregion

        /*
        private:
            virtual void		SaveSpan			( CeLocation*& pInsert
                                                    , const CePath& path
                                                    , CeCircularSpan& span
                                                    , const UINT2 index );
            virtual void		SetCuldesac			( const LOGICAL isculdesac );
         */

        /// <summary>
        /// The circle that this leg sits on.
        /// </summary>
        internal override Circle Circle
        {
            get { return m_Circle; }
        }

        /// <summary>
        /// Observed radius.
        /// </summary>
        internal Distance ObservedRadius
        {
            get { return m_Radius; }
        }

        /// <summary>
        /// The total length of this leg, in meters on the ground.
        /// </summary>
        internal override ILength Length
        {
            get
            {
                // If we have a cul-de-sac, we can determine the length using
                // just the central angle & the radius. Otherwise ask the base
                // class to return the total observed length.
                if (IsCulDeSac)
                {
                    double radius = m_Radius.Meters;
                    return new Length((MathConstants.PIMUL2 - m_Angle1) * radius);
                }
                else
                    return new Length(base.GetTotal());
            }
        }

        //	@mfunc	Given the position of the start of this leg, along with
        //			an initial bearing, project the end of the leg, along with
        //			an exit bearing.
        //
        //	@parm	The position for the start of the leg. Updated to be
        //			the position for the end of the leg.
        //	@parm	The bearing at the end of the previous leg. Updated for
        //			this leg.
        //	@parm	Scale factor to apply to distances (default=1.0).

        internal override void Project(ref IPosition pos, ref double bearing, double sfac)
        {
            throw new Exception("The method or operation is not implemented.");
            /*
//	Get circle info
	CeVertex centre;
	CeVertex ec;
	FLOAT8 ebearing;
	FLOAT8 bear2bc;
	GetPositions ( pos, bearing, sfac,
				   centre, bear2bc, ec, ebearing );

//	Stick results into return variables
	pos = ec;
	bearing = ebearing;
             */
        }

        /*
//	@mfunc	Given the position of the start of this leg, along with
//			an initial bearing, get other positions (and bearings)
//			relating to the circle.
//
//	@parm	The position for the BC.
//	@parm	The bearing at the BC.
//	@parm	Scale factor to apply to distances.
//	@parm	Position of the circle centre.
//	@parm	Bearing from the centre to the BC.
//	@parm	Position of the EC.
//	@parm	Exit bearing.
//
//////////////////////////////////////////////////////////////////////

void CeCircularLeg::GetPositions ( const CeVertex& bc
								 , const FLOAT8& sbearing
								 , const FLOAT8 sfac
								 , CeVertex& centre
								 , FLOAT8& bear2bc
								 , CeVertex& ec
								 , FLOAT8& ebearing ) const {

//	Have we got a cul-de-sac?
	LOGICAL cul = (m_Flag & FLG_CULDESAC);

//	Remember reverse bearing if we have a cul-de-sac.
	FLOAT8 revbearing = sbearing + PI;

//	Initialize current bearing.
	FLOAT8 bearing = sbearing;

//	Counter-clockwise?
	LOGICAL ccw;
	if ( m_Flag & FLG_CC )
		ccw = TRUE;
	else
		ccw = FALSE;

//	Get radius in metres on the ground (and scale it).
	FLOAT8 radius = m_Radius.GetMetric() * sfac;

//	Get to the centre (should really get to the P.I., but not sure
//	where that is when the curve is > half a circle -- need to
//	check some book).

	if ( cul ) {
		if ( ccw ) 
			bearing -= (m_Angle1*0.5);
		else
			bearing += (m_Angle1*0.5);
	}
	else {
		if ( ccw )
			bearing -= (PI-m_Angle1);
		else
			bearing += (PI-m_Angle1);
	}

	FLOAT8 dE = radius * sin(bearing);
	FLOAT8 dN = radius * cos(bearing);

	FLOAT8 x = bc.GetEasting() + dE;
	FLOAT8 y = bc.GetNorthing() + dN;

	centre = CeVertex(x,y);

//	Return the bearing from the centre to the BC (the reverse
//	of the bearing we just used).
	bear2bc = bearing + PI;

//	Now go out to the EC. For regular curves, figure out the
//	central angle by comparing the observed length of the curve
//	to the total circumference.

	if ( cul ) {

		if ( ccw )
			bearing -= (PI-m_Angle1);
		else
			bearing += (PI-m_Angle1);
	}
	else {

		FLOAT8 length = GetTotal() * sfac;
		FLOAT8 circumf = radius * PIMUL2;
		FLOAT8 ca = PIMUL2 * (length/circumf);

		if ( ccw )
			bearing += (PI-ca);
		else
			bearing -= (PI-ca);
	}

//	Define the position of the EC.
	x += (radius * sin(bearing));
	y += (radius * cos(bearing));
	ec = CeVertex(x,y);

//	Define the exit bearing. For cul-de-sacs, the exit bearing
//	is the reverse of the original bearing (lines are parallel).

	if ( cul )
		ebearing = revbearing;
	else {

//		If we have a second angle, use that
		FLOAT8 angle;
		if ( m_Flag & FLG_TWOANGLES )
			angle = m_Angle2;
		else
			angle = m_Angle1;

		if ( ccw )
			ebearing = bearing - (PI-angle);
		else
			ebearing = bearing + (PI-angle);
	}

} // end of GetPositions

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Draw this leg.
//
//	@parm	The position for the start of the leg. Updated to be
//			the position for the end of the leg.
//	@parm	The bearing at the end of the previous leg. Updated for
//			this leg.
//	@parm	Scale factor to apply to distances.
//	@parm	TRUE if the draw should actually erase.
//
//////////////////////////////////////////////////////////////////////

void CeCircularLeg::Draw ( CeVertex& pos
						 , FLOAT8& bearing
						 , const FLOAT8 sfac
						 , const LOGICAL erase ) const {

//	Create an undefined circular span
	CeCircularSpan span(this,pos,bearing,sfac);

//	Draw (or erase) each visible span in turn. Note that for
//	cul-de-sacs, there may be no observed spans.

	UINT2 nspan = this->GetCount();

	if ( nspan==0 ) {
		span.Get(0);
		if ( erase )
			span.Erase();
		else
			span.Draw();
	}
	else {
		for ( UINT2 i=0; i<nspan; i++ ) {
			span.Get(i);
			if ( erase )
				span.Erase();
			else
				span.Draw();
		}
	}

//	Update BC info to refer to the EC.
	pos = span.GetEC();
	bearing = span.GetExitBearing();

} // end of Draw

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Draw a previously saved leg.
//
//	@parm	TRUE if the path should be drawn in preview mode (i.e.
//			in the normal construction colour, with miss-connects
//			shown as dotted lines).
//
//////////////////////////////////////////////////////////////////////

void CeCircularLeg::Draw ( const LOGICAL preview ) const {

//	If NOT drawing in preview mode, just draw each feature
//	known to the leg. If we hit any gaps, draw a dotted line
//	in between.

	UINT2 nfeat = this->GetCount();

	if ( preview ) {

//		CeVertex startdot;		// Position of start of dotted section.
//		CeVertex enddot;		// Position of end of dotted section.

		for ( UINT2 i=0; i<nfeat; i++ ) {
			const CeFeature* const pFeat = this->GetpFeature(i);
			if ( pFeat ) pFeat->DrawThis(COL_MAGENTA);
		}
	}
	else {

//		Draw each feature the normal way.
		for ( UINT2 i=0; i<nfeat; i++ ) {
			const CeFeature* const pFeat = this->GetpFeature(i);
			if ( pFeat ) pFeat->DrawThis();
		}
	}

} // end of Draw

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Save features for this leg.
//
//	@parm	The connection path that this leg belongs to.
//	@parm	The position for the start of the leg. Updated to be
//			the position for the end of the leg.
//	@parm	The bearing at the end of the previous leg. Updated for
//			this leg.
//	@parm	Scale factor to apply to distances.
//
//////////////////////////////////////////////////////////////////////

LOGICAL CeCircularLeg::Save	( const CePath& op
							, CeVertex& terminal
							, FLOAT8& bearing
							, const FLOAT8 sfac ) {
	
//	Create an undefined circular span
	CeCircularSpan span(this,terminal,bearing,sfac);

//	If this leg already has an associated circle, move it. Otherwise
//	add a circle to the map that corresponds to this leg.
	if ( m_pCircle ) {

		// Get the centre point associated with the current op. If there
		// is one (i.e. it's not a point that existed before the op), just
		// move it. Otherwise add a new circle (along with a new centre
		// point).

		// Inactive centre points are ok (if you don't search for
		// them, a new circle will be added).

		CePoint* pCentre = m_pCircle->GetpCentre(&op,FALSE);

		if ( pCentre ) {

			// Get the span to modify the radius of the circle.
			span.SetCircle(m_pCircle);

			// Move the centre point.
			pCentre->Move(span.GetCentre());
		}
		else {

			// The existing centre location makes reference to the
			// circle, so clean that up.
			CeLocation* pOldLoc = m_pCircle->GetpCentre();
			pOldLoc->CutObject(*m_pCircle);

			// 19-OCT-99: The span.AddCircle call just returns
			// the circle that this leg already knows about! (the
			// span picked it up via it's constructor). So add the
			// new circle explicitly here & let the span know about
			// it.
			span.SetCircle(0);

			// Add a new circle.
			m_pCircle = span.AddCircle();
		}
	}
	else
		m_pCircle = span.AddCircle();

//	Create (or update) features for each span. Note that for
//	cul-de-sacs, there may be no observed spans.

	UINT2 nspan = max(1,this->GetCount());
	CeLocation* pNoInsert=0;

	for ( UINT2 i=0; i<nspan; i++ )
		this->SaveSpan(pNoInsert,op,span,i);

//	Update BC info to refer to the EC.
	terminal = span.GetEC();
	bearing = span.GetExitBearing();
	return TRUE;

} // end of Save
#endif

#ifdef _CEDIT
//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Rollforward this leg.
//
//	@parm	The location of the end of any new insert that
//			immediately precedes this leg. This will be
//			updated if this leg also ends with a new insert
//			(if not, it will be returned as a null value).
//	@parm	The connection path that this leg belongs to.
//	@parm	The position for the start of the leg. Updated to be
//			the position for the end of the leg.
//	@parm	The bearing at the end of the previous leg. Updated for
//			this leg.
//	@parm	Scale factor to apply to distances.
//
//////////////////////////////////////////////////////////////////////

LOGICAL CeCircularLeg::Rollforward ( CeLocation*& pInsert
								   , const CePath& op
								   , CeVertex& terminal
								   , FLOAT8& bearing
								   , const FLOAT8 sfac ) {
	
//	Create an undefined circular span
	CeCircularSpan span(this,terminal,bearing,sfac);

//	If this leg already has an associated circle, move it. Otherwise
//	add a circle to the map that corresponds to this leg.
	if ( m_pCircle ) {

		// Get the centre point associated with the current op. If there
		// is one (i.e. it's not a point that existed before the op), just
		// move it. Otherwise add a new circle (along with a new centre
		// point).

		// Inactive centre points are ok (if you don't search for
		// them, a new circle will be added).

		// 19-OCT-99: During rollforward, the op returned by SaveOp is
		// the op where rollforward started (not necessarily the op
		// that this leg belongs to). This probably needs to be changed
		// for other reasons, but for now, use the op that was supplied
		// (it was not previously supplied). If you don't do this, the
		// GetpCentre call will not find the centre point, even if it
		// was created by this leg, so it would always go to add a new
		// circle.

		//const CeOperation* const pop = CeMap::GetpMap()->SaveOp();
		//CePoint* pCentre = m_pCircle->GetpCentre(pop,FALSE);
		CePoint* pCentre = m_pCircle->GetpCentre(&op,FALSE);

		if ( pCentre ) {

			// Get the span to modify the radius of the circle.
			span.SetCircle(m_pCircle);

			// Move the centre point.
			pCentre->Move(span.GetCentre());
		}
		else {

			// The existing centre location makes reference to the
			// circle, so clean that up.
			CeLocation* pOldLoc = m_pCircle->GetpCentre();
			pOldLoc->CutObject(*m_pCircle);

			// 19-OCT-99: The span.AddCircle call just returns
			// the circle that this leg already knows about! (the
			// span picked it up via it's constructor). So add the
			// new circle explicitly here & let the span know about
			// it.
			span.SetCircle(0);

			// Add a new circle.
			m_pCircle = span.AddCircle();
		}
	}
	else
		m_pCircle = span.AddCircle();

//	Create (or update) features for each span. Note that for
//	cul-de-sacs, there may be no observed spans.

	UINT2 nspan = max(1,this->GetCount());

	for ( UINT2 i=0; i<nspan; i++ )
		this->SaveSpan(pInsert,op,span,i);

//	Update BC info to refer to the EC.
	terminal = span.GetEC();
	bearing = span.GetExitBearing();
	return TRUE;

} // end of Rollfoward
#endif

#ifdef _CEDIT
//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Save a specific span of this leg.
//
//	@parm	The location of the end of any new insert that
//			immediately precedes this span. This will be
//			updated if this span is also a new insert
//			(if not, it will be returned as a null value).
//	@parm	The connection path that this span is part of.
//	@parm	The span for the leg.
//	@parm	The index of the span to save.
//
//////////////////////////////////////////////////////////////////////

void CeCircularLeg::SaveSpan ( CeLocation*& pInsert
							 , const CePath& op
							 , CeCircularSpan& span
							 , const UINT2 index ) {

	// The very end of a connection path should never be moved.
	const CeLocation* const pVeryEnd = op.GetpEnd();

	if ( IsNewSpan(index) ) {

		// Is this the very last span in the connection path?
		const UINT2 nspan = max(1,GetCount());
		const LOGICAL isLast = (index==(nspan-1) && op.IsLastLeg(this));

		// Save the insert.
		CeArc* pNewArc = span.SaveInsert(index,op,isLast);

		// Record the new arc as part of this leg
		AddNewSpan(index,*pNewArc);

		// Remember the last insert position.
		pInsert = (CeLocation*)pNewArc->GetpEnd();
	}
	else  {

		// Get the span to save
		span.Get(index);

		// See if the span previously had a saved feature.
		CeFeature* pOld = (CeFeature*)this->GetpFeature(index);

		// Save the span.
		CeFeature* pFeat = span.Save(pInsert,pOld,pVeryEnd);

		// If the saved span is different from what we had before,
		// tell the base class about it.
		if ( pFeat!=pOld ) SetFeature(index,pFeat);

		// That wasn't an insert.
		pInsert = 0;
	}

} // end of SaveSpan
#endif

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Draw any angles for this leg.
//
//	@parm	The point the observation must be made from.
//			Specify 0 for all points.
//	@parm	The operation this leg belongs to.
//	@parm	Scale factor to apply to the leg.
//	@parm	The position of a backsight prior to the BC (may
//			be undefined).
//	@parm	The position of the start of the leg.
//	@parm	The bearing to the start of the leg. Updated to
//			refer to the bearing to the end of the leg.
//	@parm	The position of the end of the leg.
//	@parm	The view to draw in.
//	@parm	Display context for the draw.
//	@parm	The ground window for the draw (0 for no check).
//	@parm	The point the angle is directed to (0 if unknown).
//
//////////////////////////////////////////////////////////////////////

void CeCircularLeg::DrawAngles ( const CePoint* const pFrom
							   , const CeOperation& op
							   , const FLOAT8 sfac
							   , const CeVertex& bs
							   , const CeVertex& from
							   , FLOAT8& bearing
							   , CeVertex& to
							   , CeView* view
							   , CDC* pDC
							   , const CeWindow* const pWin ) const {

//	Get circle info
	CeVertex centre;
	CeVertex ec;
	FLOAT8 ebearing;
	FLOAT8 bear2bc;
	GetPositions ( from, bearing, sfac,
				   centre, bear2bc, ec, ebearing );

//	Stick results into return variables
	to = ec;
	bearing = ebearing;

//	Return if the window of the circle does not overlap
//	the display window.

	if ( pWin ) {
		const FLOAT8 xc = centre.GetEasting();
		const FLOAT8 yc = centre.GetNorthing();
		const FLOAT8 radius = this->GetRadius() * sfac;
		const CeVertex sw(xc-radius,yc-radius);
		const CeVertex ne(xc+radius,yc+radius);
		const CeWindow win(sw,ne);
		if ( !pWin->IsOverlap(win) ) return;
	}

//	Draw dotted lines to the centre of the circle.
	view->DrawDotted(from,centre,COL_BLACK);
	view->DrawDotted(centre,ec,COL_BLACK);

//	If we have a cul-de-sac, stick the central angle along
//	the bisector.

	if ( this->IsCuldesac() ) {

		// Return if a from point has been specified, but
		// it's not the centre point.
		if ( pFrom && GetpCentrePoint(op)!=pFrom ) return;

		view->DrawAngle(pDC,from,centre,ec,m_Angle1,FALSE);
	}
	else {

		// Return if a from point has been specified, but
		// it doesn't match either the BC or EC.
		LOGICAL drawBC = TRUE;
		LOGICAL drawEC = TRUE;
		FLOAT8 angle = 0.0;

		if ( pFrom ) {
			drawBC = (GetpStartPoint(op)==pFrom);
			drawEC = (GetpEndPoint(op)==pFrom);
		}

		// Draw the entry angle if it's not a right angle.
		if ( drawBC ) {
			angle = fabs(m_Angle1);
			if ( angle>0.000001 &&
				 fabs(angle-PIDIV2  )>0.000001 &&
				 fabs(angle-PIMUL1P5)>0.000001 )
				view->DrawAngle(pDC,bs,from,centre,angle,FALSE);
		}

		// Draw the exit angle too. We need to project to some
		// point that's an arbitrary distance along the exit
		// bearing.

		if ( drawEC ) {
			if ( m_Flag & FLG_TWOANGLES ) angle = fabs(m_Angle2);

			if ( angle>0.000001 &&
				 fabs(angle-PIDIV2  )>0.000001 &&
				 fabs(angle-PIMUL1P5)>0.000001 ) {
				CeVertex next(ec,ebearing,100.0);
				view->DrawAngle(pDC,centre,ec,next,angle,FALSE);
			}
		}
	}

} // end of DrawAngles

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Draw any angles for this leg.
//
//	@parm	The point the observation must be made from.
//			Specify 0 for all points.
//	@parm	The operation this leg belongs to.
//	@parm	Scale factor to apply to the leg.
//	@parm	The position of a backsight prior to the BC (may
//			be undefined).
//	@parm	The position of the start of the leg.
//	@parm	The bearing to the start of the leg. Updated to
//			refer to the bearing to the end of the leg.
//	@parm	The position of the end of the leg.
//	@parm	The thing we're drawing to.
//
//////////////////////////////////////////////////////////////////////

#include "CeDC.h"

void CeCircularLeg::DrawAngles ( const CePoint* const pFrom
							   , const CeOperation& op
							   , const FLOAT8 sfac
							   , const CeVertex& bs
							   , const CeVertex& from
							   , FLOAT8& bearing
							   , CeVertex& to
							   , CeDC& gdc ) const {

//	Get circle info
	CeVertex centre;
	CeVertex ec;
	FLOAT8 ebearing;
	FLOAT8 bear2bc;
	GetPositions ( from, bearing, sfac,
				   centre, bear2bc, ec, ebearing );

//	Stick results into return variables
	to = ec;
	bearing = ebearing;

//	Return if the window of the circle does not overlap
//	the display window.

	const CeWindow& drawin = gdc.GetWindow();

	if ( drawin.IsDefined() ) {
		const FLOAT8 xc = centre.GetEasting();
		const FLOAT8 yc = centre.GetNorthing();
		const FLOAT8 radius = this->GetRadius() * sfac;
		const CeVertex sw(xc-radius,yc-radius);
		const CeVertex ne(xc+radius,yc+radius);
		const CeWindow win(sw,ne);
		if ( !drawin.IsOverlap(win) ) return;
	}

//	Draw dotted lines to the centre of the circle.
	const CeStyle* const pStyle =
		GetpDraw()->GetBlackDottedLineStyle();
	gdc.SetLineStyle(*pStyle);
	gdc.Draw(from,centre);
	gdc.Draw(centre,ec);

//	If we have a cul-de-sac, stick the central angle along
//	the bisector.

	if ( this->IsCuldesac() ) {

		// Return if a from point has been specified, but
		// it's not the centre point.
		if ( pFrom && GetpCentrePoint(op)!=pFrom ) return;

		gdc.DrawAngle(from,centre,ec,m_Angle1,FALSE);
	}
	else {

		// Return if a from point has been specified, but
		// it doesn't match either the BC or EC.
		LOGICAL drawBC = TRUE;
		LOGICAL drawEC = TRUE;
		FLOAT8 angle = 0.0;

		if ( pFrom ) {
			drawBC = (GetpStartPoint(op)==pFrom);
			drawEC = (GetpEndPoint(op)==pFrom);
		}

		// Draw the entry angle if it's not a right angle.
		if ( drawBC ) {
			angle = fabs(m_Angle1);
			if ( angle>0.000001 &&
				 fabs(angle-PIDIV2  )>0.000001 &&
				 fabs(angle-PIMUL1P5)>0.000001 )
				gdc.DrawAngle(bs,from,centre,angle,FALSE);
		}

		// Draw the exit angle too. We need to project to some
		// point that's an arbitrary distance along the exit
		// bearing.

		if ( drawEC ) {
			if ( m_Flag & FLG_TWOANGLES ) angle = fabs(m_Angle2);

			if ( angle>0.000001 &&
				 fabs(angle-PIDIV2  )>0.000001 &&
				 fabs(angle-PIMUL1P5)>0.000001 ) {
				CeVertex next(ec,ebearing,100.0);
				gdc.DrawAngle(centre,ec,next,angle,FALSE);
			}
		}
	}

} // end of DrawAngles
        */

        /// <summary>
        /// The central angle for this leg (assuming the
        /// <see cref="IsCulDeSac"/> property is true)
        /// </summary>
        double CentralAngle
        {
            get { return m_Angle1; }
        }

        /// <summary>
        /// The entry angle for this leg (assuming it isn't a cul-de-sac -
        /// see the <see cref="IsCulDeSac"/> property)
        /// </summary>
        double EntryAngle
        {
            get { return m_Angle1; }
        }

        /// <summary>
        /// The exit angle for this leg (assuming it isn't a cul-de-sac -
        /// see the <see cref="IsCulDeSac"/> property)
        /// </summary>
        double ExitAngle
        {
            get
            {
                if ((m_Flag & CircularLegFlag.TwoAngles)!=0)
                    return m_Angle2;
                else
                    return m_Angle1;
            }
        }

        /// <summary>
        /// Defines this leg using the info in another leg. This does NOT
        /// touch the base class in any way.
        /// </summary>
        /// <param name="master"></param>
        void Define(CircularLeg master)
        {
            m_Angle1 = master.m_Angle1;
            m_Angle2 = master.m_Angle2;
            m_Radius = master.m_Radius;
            m_Flag = master.m_Flag;
            m_Circle = master.m_Circle;
        }

        /// <summary>
        /// Is this leg has been defined?. This just confirms that the
        /// radius is defined.
        /// </summary>
        bool IsDefined
        {
            get { return (m_Radius!=null && m_Radius.IsDefined); }
        }

        /// <summary>
        /// Is the leg directed clockwise?
        /// </summary>
        internal bool IsClockwise
        {
            get { return (m_Flag & CircularLegFlag.CounterClockwise) == 0; }
            set
            {
                // SS:20080309 - Don't know why the following was done...

                // Assume clockwise by clearing the flag bit.
                SetFlag(CircularLegFlag.CounterClockwise, false);

                // Set bit if NOT clockwise.
                if (value == false)
                    SetFlag(CircularLegFlag.CounterClockwise, true);
            }
        }

        /// <summary>
        /// Records the radius of this leg.
        /// </summary>
        /// <param name="radius">The radius to assign.</param>
        void SetRadius(Distance radius)
        {
            m_Radius = radius;
        }

        /// <summary>
        /// The observed radius, in meters
        /// </summary>
        internal double Radius
        {
            get { return (m_Radius == null ? 0.0 : m_Radius.Meters); }
        }

        /// <summary>
        /// Sets the entry (BC) angle. Note that when setting both the entry
        /// and exit angles, this function should be called BEFORE a call to
        /// <see cref="SetExitAngle"/>. This function should NOT be called
        /// for cul-de-sacs (use <see cref="SetCentralAngle"/> instead).
        /// </summary>
        /// <param name="bangle">The angle to assign, in radians.</param>
        void SetEntryAngle(double bangle)
        {
            // Store the specified angle.
            m_Angle1 = bangle;

            // This leg is not a cul-de-sac.
            IsCulDeSac = false;
        }

        /// <summary>
        /// Sets the exit (BC) angle. Note that when setting both the entry and
        /// exit angles, this function should be called AFTER a call to
        /// <see cref="SetEntryAngle"/>. This function should NOT be called
        /// for cul-de-sacs (use <see cref="SetCentralAngle"/> instead).
        /// </summary>
        /// <param name="eangle">The angle to assign, in radians.</param>
        void SetExitAngle(double eangle)
        {
            // If the angle is the same as the entry angle, store an
            // undefined exit angle, and set the flag bit to indicate
            // that only the entry angle is valid.

            if (Math.Abs(m_Angle1 - eangle) < MathConstants.TINY)
            {
                m_Angle2 = 0.0;
                SetFlag(CircularLegFlag.TwoAngles, false);
            }
            else
            {
                m_Angle2 = eangle;
                SetFlag(CircularLegFlag.TwoAngles, true);
            }

            // This leg is not a cul-de-sac.
            IsCulDeSac = false;
        }

        /// <summary>
        /// Sets the central angle for this leg. The leg will be flagged
        /// as being a cul-de-sac. 
        /// </summary>
        /// <param name="cangle">The central angle, in radians.</param>
        void SetCentralAngle(double cangle)
        {
            // Store the central angle.
            m_Angle1 = cangle;

            // The other angle is unused.
            m_Angle2 = 0.0;
            SetFlag(CircularLegFlag.TwoAngles, false);

            // This leg is a cul-de-sac
            IsCulDeSac = true;
        }

        /// <summary>
        /// Is the leg flagged as a cul-de-sac?
        /// </summary>
        internal bool IsCulDeSac
        {
            get { return (m_Flag & CircularLegFlag.CulDeSac) != 0; }
            set { SetFlag(CircularLegFlag.CulDeSac, value); }
        }

        /// <summary>
        /// Sets flag bit(s)
        /// </summary>
        /// <param name="flag">The flag bit(s) to set</param>
        /// <param name="setting">True to set, false to clear</param>
        void SetFlag(CircularLegFlag flag, bool setting)
        {
            if (setting)
                m_Flag |= flag;
            else
                m_Flag &= (~flag);
        }

        /// <summary>
        /// Returns the position at the center of the circle that this leg lies on.
        /// </summary>
        internal override IPosition Center
        {
            get
            {
                if (m_Circle != null)
                    return m_Circle.Center;
                else
                    return null;
            }
        }

        /*
//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Create transient CeMiscText objects for any observed
//			angles that are part of this leg.
//
//			The caller is responsible for deleting the text
//			objects once done with them.
//
//	@parm	The point the observation must be made from.
//			Specify 0 for all points.
//	@parm	The operation this leg belongs to.
//	@parm	Scale factor to apply to the leg.
//	@parm	The position of a backsight prior to the BC (may
//			be undefined).
//	@parm	The position of the start of the leg.
//	@parm	The bearing to the start of the leg. Updated to
//			refer to the bearing to the end of the leg.
//	@parm	The position of the end of the leg.
//	@parm	List of pointers to created text objects (appended to).
//	@parm	Should lines be produced too?
//
//	@rdesc	TRUE if the specified from point was encountered (this
//			does not necessarily mean that any text needed to be
//			generated for it).
//
//////////////////////////////////////////////////////////////////////

LOGICAL CeCircularLeg::CreateAngleText ( const CePoint* const pFrom
									   , const CeOperation& op
									   , const FLOAT8 sfac
									   , const CeVertex& bs
									   , const CeVertex& from
									   , FLOAT8& bearing
									   , CeVertex& to
									   , CPtrList& text
									   , const LOGICAL wantLinesToo ) const {

	// Get circle info
	CeVertex centre;
	CeVertex ec;
	FLOAT8 ebearing;
	FLOAT8 bear2bc;
	GetPositions ( from, bearing, sfac,
				   centre, bear2bc, ec, ebearing );

	// Stick results into return variables
	to = ec;
	bearing = ebearing;

	// If we have a cul-de-sac, stick the central angle along
	// the bisector.

	if ( this->IsCuldesac() ) {

		// Return if a from point has been specified, but
		// it's not the centre point.
		if ( pFrom && GetpCentrePoint(op)!=pFrom ) return FALSE;

		// Generate the text.
		MakeText(from,centre,ec,m_Angle1,text);

		// If a from-point was specified, and we found it,
		// indicate that it has now been handled.
		return (pFrom!=0);
	}
	else {

		// Return if a from point has been specified, but
		// it doesn't match either the BC or EC.
		LOGICAL drawBC = TRUE;
		LOGICAL drawEC = TRUE;
		FLOAT8 angle = 0.0;

		if ( pFrom ) {
			drawBC = (GetpStartPoint(op)==pFrom);
			drawEC = (GetpEndPoint(op)==pFrom);
		}

		// Draw the entry angle if it's not a right angle.
		if ( drawBC ) {
			angle = fabs(m_Angle1);
			if ( angle>0.000001 &&
				 fabs(angle-PIDIV2  )>0.000001 &&
				 fabs(angle-PIMUL1P5)>0.000001 )
					MakeText(bs,from,centre,m_Angle1,text);
		}

		// Draw the exit angle too. We need to project to some
		// point that's an arbitrary distance along the exit
		// bearing.

		if ( drawEC ) {
			if ( m_Flag & FLG_TWOANGLES ) angle = fabs(m_Angle2);

			if ( angle>0.000001 &&
				 fabs(angle-PIDIV2  )>0.000001 &&
				 fabs(angle-PIMUL1P5)>0.000001 ) {
				CeVertex next(ec,ebearing,100.0);
				MakeText(centre,ec,next,m_Angle2,text);
			}
		}

		// If a from-point was specified, and we found it,
		// indicate that it has now been handled.
		return (pFrom && (drawBC || drawEC));
	}

} // end of CreateAngleText

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Define a string with the observations that
//			make up this leg.
//
//	@parm	The string to define
//
//////////////////////////////////////////////////////////////////////

void CeCircularLeg::GetDataString ( CString& str ) const {

	// Initial angle
	str.Format("(%s",RadStr(m_Angle1));

	// If it's a cul-de-sac, just append the "CA" characters.
	// Otherwise we could have an exit angle as well.

	if ( IsCuldesac() )
		str += "ca ";
	else {

		if ( m_Flag & FLG_TWOANGLES ) {
			str += ' ';
			str += RadStr(m_Angle2);
		}

		str += ' ';
	}

	// The observed radius.
	str += m_Radius.Format();

	// Is it counter-clockwise?
	if ( !IsClockwise() ) str += " cc";

	// Append any observed distances if there are any.
	if ( GetCount() ) {
		str += '/';
		AddToString(str);
	}

	str += ')';

} // end of GetDataString

#ifdef _CEDIT
//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Save features for a second face that is based
//			on this leg.
//
//	@parm	The connection path that this leg belongs to.
//	@parm	The extra face to create features for.
//
//	@rdesc	TRUE if created ok.
//
//////////////////////////////////////////////////////////////////////

#include "CeExtraLeg.h"

LOGICAL CeCircularLeg::SaveFace ( const CePath& op
							    , CeExtraLeg& face ) const {

	// Get the terminal positions for this leg.
	CeVertex spos;
	CeVertex epos;
	if ( !op.GetLegEnds(*this,spos,epos) ) return FALSE;

	return face.MakeCurves(op,spos,epos,*m_pCircle,IsClockwise());

} // end of SaveFace
#endif

#ifdef _CEDIT
//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Rollforward the second face of this leg.
//
//	@parm	The location of the end of any new insert that
//			immediately precedes this leg. This will be
//			updated if this leg also ends with a new insert
//			(if not, it will be returned as a null value).
//	@parm	The connection path that this leg belongs to.
//	@parm	The second face.
//	@parm	The new position for the start of this leg.
//	@parm	The new position for the end of this leg.
//
//	@devnote The start and end positions passed in should
//			 correspond to where THIS leg currently ends.
//			 They are passed in because this leg may contain
//			 miss-connects (and maybe even missing end points).
//		     So it would be tricky trying trying to work it
//			 out now.
//
//	@rdesc	TRUE if rolled forward ok.
//
//////////////////////////////////////////////////////////////////////

LOGICAL CeCircularLeg::RollforwardFace ( CeLocation*& pInsert
									   , const CePath& op
									   , CeExtraLeg& face
									   , const CeVertex& spos
									   , const CeVertex& epos ) const {

	// Get the extra face to do it.
	return face.UpdateCurves(pInsert,op,spos,epos,*m_pCircle,IsClockwise());

} // end of RollforwardFace
#endif
         */

        internal override string DataString
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        internal override void Draw(ref IPosition terminal, ref double bearing, double sfac)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal override void Draw(bool preview)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal override void Save(Backsight.Editor.Operations.PathOperation op, ref IPosition terminal, ref double bearing, double sfac)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal override bool Rollforward(ref IPointGeometry insert, Backsight.Editor.Operations.PathOperation op, ref IPosition terminal, ref double bearing, double sfac)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal override bool SaveFace(Backsight.Editor.Operations.PathOperation op, ExtraLeg face)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal override bool RollforwardFace(ref IPointGeometry insert, Backsight.Editor.Operations.PathOperation op, ExtraLeg face, IPosition spos, IPosition epos)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
