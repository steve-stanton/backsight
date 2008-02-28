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
using System.Text;

using Backsight.Editor.Operations;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="21-JAN-1998" was="CeStraightLeg" />
    /// <summary>
    /// A straight leg in a connection path.
    /// </summary>
    [Serializable]
    class StraightLeg : Leg
    {
        #region Class data

        /// <summary>
        /// Angle at the start of the leg (signed). 
        /// </summary>
        double m_StartAngle;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>StraightLeg</c>
        /// </summary>
        /// <param name="nspan">The number of spans for the leg.</param>
        StraightLeg(int nspan)
            : base(nspan)
        {
            m_StartAngle = 0.0;
        }

        #endregion

        internal double StartAngle
        {
            get { return m_StartAngle; }
            set { m_StartAngle = value; }
        }

        internal override Circle Circle
        {
            get { return null; }
        }

        internal override IPosition Center
        {
            get { return null; }
        }

        /// <summary>
        /// The total observed length of this leg
        /// </summary>
        internal override ILength Length
        {
            get { return new Length(GetTotal()); }
        }

        internal override bool Save(PathOperation op, ref IPosition terminal, ref double bearing, double sfac)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal override bool Rollforward(ref IPointGeometry insert, PathOperation op, ref IPosition terminal, ref double bearing, double sfac)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal override bool SaveFace(PathOperation op, ExtraLeg face)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal override bool RollforwardFace(ref IPointGeometry insert, PathOperation op, ExtraLeg face, IPosition spos, IPosition epos)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Given the position of the start of this leg, along with an initial bearing,
        /// project the end of the leg, along with an exit bearing.
        /// </summary>
        /// <param name="pos">The position at the start of the leg.</param>
        /// <param name="bearing">The initial bearing (e.g. if the previous leg was also
        /// a straight leg from A to B, the bearing is from A through B).</param>
        /// <param name="sfac">Scaling factor to apply. Default=1.0</param>
        internal override void Project(ref IPosition pos, ref double bearing, double sfac)
        {
            // Add on any initial angle (it may be a deflection).
            if (Math.Abs(m_StartAngle) > Double.Epsilon)
            {
                if (IsDeflection)
                    bearing += m_StartAngle;
                else
                    bearing += (m_StartAngle - Math.PI);
            }

            // Get the total length of the leg.
            double length = GetTotal() * sfac;

            // Figure out shifts.
            double dE = length * Math.Sin(bearing);
            double dN = length * Math.Cos(bearing);

            // Define the end position.
            pos = new Position(pos.X + dE, pos.Y + dN);
        }

        /// <summary>
        /// Draws this leg
        /// </summary>
        /// <param name="pos">The position for the start of the leg. Updated to be
        /// the position for the end of the leg.</param>
        /// <param name="bearing">The bearing at the end of the previous leg. Updated
        /// for this leg.</param>
        /// <param name="sfac">Scale factor to apply to distances.</param>
        void Draw(ref IPosition pos, ref double bearing, double sfac)
        {
            // Add on any initial angle (it may be a deflection).
            if (Math.Abs(m_StartAngle) > Double.Epsilon)
            {
                if (IsDeflection)
                    bearing += m_StartAngle;
                else
                    bearing += (m_StartAngle - Math.PI);
            }

            // Create a straight span
        }
        /*
	CeStraightSpan span(this,pos,bearing,sfac);

//	Draw (or erase) each visible span in turn.

	UINT2 nspan = this->GetCount();

	for ( UINT2 i=0; i<nspan; i++ ) {
		span.Get(i);
		if ( erase )
			span.Erase();
		else
			span.Draw();
	}

//	Return the end position of the last span.
	pos = span.GetEnd();

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

void CeStraightLeg::Draw ( const LOGICAL preview ) const {

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

//	@mfunc	Save features for this leg.
//
//	@parm	The connection path that this leg belongs to (not used).
//	@parm	The position for the start of the leg. Updated to be
//			the position for the end of the leg.
//	@parm	The bearing at the end of the previous leg. Updated for
//			this leg.
//	@parm	Scale factor to apply to distances.

LOGICAL CeStraightLeg::Save ( const CePath& op
							, CeVertex& terminal
							, FLOAT8& bearing
							, const FLOAT8 sfac ) {

	// Add on any initial angle (it may be a deflection).
	if ( fabs(m_StartAngle)>TINY ) {
		if ( IsDeflection() )
			bearing += m_StartAngle;
		else
			bearing += (m_StartAngle-PI);
	}

	// Create a straight span
	CeStraightSpan span(this,terminal,bearing,sfac);
	UINT2 nspan = this->GetCount();

	for ( UINT2 i=0; i<nspan; i++ ) {

		// Get info for the current span (this defines the
		// adjusted start and end positions, among other
		// things).
		span.Get(i);

		// Get the span to save itself.
		CeFeature* pFeat = span.Save(0,0,0);
		SetFeature(i,pFeat);

	} // next span

	// Return the end position of the last span.
	terminal = span.GetEnd();
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

LOGICAL CeStraightLeg::Rollforward ( CeLocation*& pInsert
								   , const CePath& op
								   , CeVertex& terminal
								   , FLOAT8& bearing
								   , const FLOAT8 sfac ) {

	// Add on any initial angle (it may be a deflection).
	if ( fabs(m_StartAngle)>TINY ) {
		if ( IsDeflection() )
			bearing += m_StartAngle;
		else
			bearing += (m_StartAngle-PI);
	}

	// Create a straight span
	CeStraightSpan span(this,terminal,bearing,sfac);
	UINT2 nspan = this->GetCount();

	// The very end of a connection path should never be moved.
	const CeLocation* const pVeryEnd = op.GetpEnd();

	for ( UINT2 i=0; i<nspan; i++ ) {

		// Get info for the current span (this defines the
		// adjusted start and end positions, among other
		// things).
		span.Get(i);

		// If we've got a newly inserted span
		if ( IsNewSpan(i) ) {
			const LOGICAL isLast = (i==(nspan-1) && op.IsLastLeg(this));
			CeArc* pNewArc = span.SaveInsert(i,op,isLast);
			AddNewSpan(i,*pNewArc);
			pInsert = (CeLocation*)pNewArc->GetpEnd();
		}
		else {
			CeFeature* pOld = (CeFeature*)this->GetpFeature(i);
			if ( pOld )
				span.Save(pInsert,pOld,pVeryEnd);
			else {
				CeFeature* pFeat = span.Save(pInsert,0,pVeryEnd);
				SetFeature(i,pFeat);
			}
			pInsert = 0;
		}

	} // next span

//	Return the end position of the last span.
	terminal = span.GetEnd();
	return TRUE;

} // end of Rollforward
#endif

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Draw any angles for this leg.
//
//	@parm	The point the observation must be made from.
//			Specify 0 for all points.
//	@parm	The operation this leg belongs to.
//	@parm	Scale factor to apply to the leg.
//	@parm	The position of a backsight prior to the start of
//			the leg (may be undefined).
//	@parm	The position of the start of this leg.
//	@parm	The bearing to the start of the leg. Updated to
//			refer to the bearing to the end of the leg.
//	@parm	The position of the end of this leg.
//	@parm	The view to draw in.
//	@parm	Display context for the draw.
//	@parm	The ground window for the draw (0 for no check).
//
//////////////////////////////////////////////////////////////////////

void CeStraightLeg::DrawAngles ( const CePoint* const pFrom
							   , const CeOperation& op
							   , const FLOAT8 sfac
							   , const CeVertex& bs
							   , const CeVertex& from
							   , FLOAT8& bearing
							   , CeVertex& to
							   , CeView* view
							   , CDC* pDC
							   , const CeWindow* const pWin ) const {

//	Project to the end of the leg.
	to = from;
	this->Project(to,bearing,sfac);

	// If a from point has been specified, and this leg does
	// not start at it, just return.
	if ( pFrom && GetpStartPoint(op)!=pFrom ) return;

//	Return if this leg does not have an initial angle.
	const FLOAT8 angle = fabs(m_StartAngle);
	if ( angle<TINY ) return;

//	Return if the angle is real-close to a right angle
	if ( fabs(angle-PIDIV2  )<0.000001 ||
		 fabs(angle-PIMUL1P5)<0.000001 ) return;

//	Return if the from-point is not visible.
	if ( pWin && !pWin->IsOverlap(from) ) return;

	// Turn it over to the view, remembering that the angle may
	// in fact be a deflection.
	view->DrawAngle(pDC,bs,from,to,angle,IsDeflection());

} // end of DrawAngles

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Draw any angles for this leg.
//
//	@parm	The point the observation must be made from.
//			Specify 0 for all points.
//	@parm	The operation this leg belongs to.
//	@parm	Scale factor to apply to the leg.
//	@parm	The position of a backsight prior to the start of
//			the leg (may be undefined).
//	@parm	The position of the start of this leg.
//	@parm	The bearing to the start of the leg. Updated to
//			refer to the bearing to the end of the leg.
//	@parm	The position of the end of this leg.
//	@parm	The thing we're drawing to.
//
//////////////////////////////////////////////////////////////////////

#include "CeDC.h"

void CeStraightLeg::DrawAngles ( const CePoint* const pFrom
							   , const CeOperation& op
							   , const FLOAT8 sfac
							   , const CeVertex& bs
							   , const CeVertex& from
							   , FLOAT8& bearing
							   , CeVertex& to
							   , CeDC& gdc ) const {

//	Project to the end of the leg.
	to = from;
	this->Project(to,bearing,sfac);

	// If a from point has been specified, and this leg does
	// not start at it, just return.
	if ( pFrom && GetpStartPoint(op)!=pFrom ) return;

//	Return if this leg does not have an initial angle.
	const FLOAT8 angle = fabs(m_StartAngle);
	if ( angle<TINY ) return;

//	Return if the angle is real-close to a right angle
	if ( fabs(angle-PIDIV2  )<0.000001 ||
		 fabs(angle-PIMUL1P5)<0.000001 ) return;

	// Turn it over to the view, remembering that the angle may
	// in fact be a deflection.
	gdc.DrawAngle(bs,from,to,angle,IsDeflection());

} // end of DrawAngles
*/

        /// <summary>
        /// Records a deflection angle at the start of this leg.
        /// </summary>
        /// <param name="value">The deflection, in radians. Negated values go
        /// counter-clockwise.</param>
        void SetDeflection(double value)
        {
            // Record the deflection angle at the start of this leg.
            m_StartAngle = value;

            // Remember that it's a deflection (as opposed to a regular angle).
	        base.SetDeflection(true);
        }
        /*
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

LOGICAL CeStraightLeg::CreateAngleText ( const CePoint* const pFrom
									   , const CeOperation& op
									   , const FLOAT8 sfac
									   , const CeVertex& bs
									   , const CeVertex& from
									   , FLOAT8& bearing
									   , CeVertex& to
									   , CPtrList& text
									   , const LOGICAL wantLinesToo ) const {

	// Project to the end of the leg.
	to = from;
	this->Project(to,bearing,sfac);

	// If a from point has been specified, and this leg does
	// not start at it, just return.
	if ( pFrom && GetpStartPoint(op)!=pFrom ) return FALSE;

	// Return if this leg does not have an initial angle.
	const FLOAT8 angle = fabs(m_StartAngle);
	if ( angle<TINY ) return TRUE;

	// Return if the angle is real-close to a right angle
	if ( fabs(angle-PIDIV2  )<0.000001 ||
		 fabs(angle-PIMUL1P5)<0.000001 ) return TRUE;

	// Create the text.
	MakeText(bs,from,to,m_StartAngle,text);

	return TRUE;

} // end of CreateAngleText

#ifdef _CEDIT
//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Break this leg into two legs. The break must leave
//			at least one distance in each of the resultant legs.
//
//	@parm	The connection path that contains this leg.
//	@parm	The index of the span that should be at the start
//			of the extra leg.
//
//	@rdesc	The address of the extra leg (at the end of the
//			original leg).
//
//////////////////////////////////////////////////////////////////////

CeLeg* CeStraightLeg::Break ( CePath& op
							, const INT4 index ) {

	// Can't break right at the start or end.
	const UINT2 nTotal = GetCount();
	if ( index<=0 || index>=nTotal ) return 0;

	// Create a new straight leg with the right number of spans.
	const UINT2 nSpan = nTotal-index;
	CeStraightLeg* pNewLeg =
		new ( os_database::of(this)
		    , os_ts<CeStraightLeg>::get() ) CeStraightLeg(nSpan);

	// Tell the operation to insert the new leg.
	if ( !op.InsertLeg(this,pNewLeg) ) {
		delete pNewLeg;
		return 0;
	}

	// Stick in a (clockwise) angle of 180 degrees.
	pNewLeg->m_StartAngle = PI;

	// Move observations etc from the end of the original leg.
	MoveEndLeg(index,*pNewLeg);

	return pNewLeg;

} // end of Break
#endif
        */

        /// <summary>
        /// A string representing the observations for this leg. 
        /// </summary>
        internal override string DataString
        {
            get
            {
                StringBuilder sb = new StringBuilder(100);

                // The initial angle.
                if (Math.Abs(m_StartAngle) > Double.Epsilon)
                {
                    sb.Append(RadianValue.AsShortString(m_StartAngle));
                    sb.Append(" ");
                }

                // The observed lengths.
                AddToString(sb);

                return sb.ToString();
            }
        }

        /*
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

LOGICAL CeStraightLeg::SaveFace ( const CePath& op
							    , CeExtraLeg& face ) const {

	// Get the terminal positions for this leg.
	CeVertex spos;
	CeVertex epos;
	if ( !op.GetLegEnds(*this,spos,epos) ) return FALSE;

	// Get the extra leg to do the rest.
	return face.MakeSegments(op,spos,epos);

} // end of SaveFace

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

LOGICAL CeStraightLeg::RollforwardFace ( CeLocation*& pInsert
									   , const CePath& op
									   , CeExtraLeg& face
									   , const CeVertex& spos
									   , const CeVertex& epos ) const {

	// Get the extra face to do it.
	return face.UpdateSegments(pInsert,op,spos,epos);

} // end of RollforwardFace
#endif
         */
    }
}
