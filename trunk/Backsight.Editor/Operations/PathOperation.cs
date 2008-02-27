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
using System.Collections.Generic;

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="24-JAN-1998" was="CePath" />
    /// <summary>
    /// A connection path between two points. Like a traverse.
    /// </summary>
    [Serializable]
    class PathOperation : Operation
    {
        #region Class data

        /// <summary>
        /// The point where the path starts.
        /// </summary>
        PointFeature m_From;

        /// <summary>
        /// The point where the path ends.
        /// </summary>
        PointFeature m_To;

        /// <summary>
        /// The legs that make up the path
        /// </summary>
        readonly List<Leg> m_Legs;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>PathOperation</c> to connect the specified points (with no defined
        /// spans).
        /// </summary>
        internal PathOperation(PointFeature from, PointFeature to)
        {
            m_From = from;
            m_To = to;
            m_Legs = new List<Leg>();
        }

        #endregion

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get { return "Connection path"; }
        }

        /// <summary>
        /// Finds the observed length of a line that was created by this operation.
        /// </summary>
        /// <param name="line">The line to find</param>
        /// <returns>The observed length of the line (null if this operation doesn't
        /// reference the specified line)</returns>
        internal override Distance GetDistance(LineFeature line)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// The features created by this editing operation.
        /// </summary>
        internal override Feature[] Features
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        /// <summary>
        /// The unique identifier for this edit.
        /// </summary>
        internal override EditingActionId EditId
        {
            get { return EditingActionId.Path; }
        }

        /// <summary>
        /// Adds references to existing features referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        public override void AddReferences()
        {
            m_From.AddOp(this);
            m_To.AddOp(this);
        }

        /// <summary>
        /// Rollback this operation (occurs when a user undoes the last edit).
        /// </summary>
        /// <returns>True if operation was rolled back ok</returns>
        internal override bool Undo()
        {
            throw new Exception("The method or operation is not implemented.");
            /*
//	Rollback any sub-operations.
	CeOperation::OnRollback();

//	Cut references that features make to this operation.
	if ( m_pFrom ) m_pFrom->CutOp(*this);
	if ( m_pTo ) m_pTo->CutOp(*this);

//	Ask each leg to rollback.
	for ( UINT2 i=0; i<m_NumLeg; i++ ) {
		m_pLegs[i]->OnRollback(*this);
		m_pLegs[i] = 0;
	}

	return TRUE;
             */
        }

        /// <summary>
        /// Rollforward this edit in response to some sort of update.
        /// </summary>
        /// <returns>True if operation has been re-executed successfully</returns>
        internal override bool Rollforward()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Attempts to locate a superseded (inactive) line that was the parent of
        /// a specific line.
        /// </summary>
        /// <param name="line">The line of interest</param>
        /// <returns>The superseded line that the line of interest was derived from. Null if
        /// this edit did not create the line of interest.</returns>
        internal LineFeature GetPredecessor(LineFeature line)
        {
            // This edit doesn't supersede anything
            return null;
        }

        /// <summary>
        /// Returns true, to indicate that this edit can be corrected.
        /// </summary>
        bool CanCorrect
        {
            get { return true; }
        }

        bool IsLastLeg(Leg leg)
        {
            return Object.ReferenceEquals(m_Legs[m_Legs.Count-1], leg);
        }

        /// <summary>
        /// Checks whether this operation makes reference to a specific feature.
        /// </summary>
        /// <param name="feat">The feature to check for.</param>
        /// <returns>True if this edit depends on (contains a reference to) the supplied feature</returns>
        bool HasReference(Feature feat)
        {
            return Object.ReferenceEquals(m_From, feat) || Object.ReferenceEquals(m_To, feat);
        }

        /*
public:
							CePath				( const CePoint& from
												, const CePoint& to );
							CePath				( const CePath& copy );
	virtual					~CePath				( void );
	virtual const CHARS*	GetpTitle			( void ) const;
	virtual void			Draw				( const FLOAT8 rotation
												, const FLOAT8 sfac
												, const LOGICAL erase=FALSE ) const;
	virtual void			Draw				( const LOGICAL preview=FALSE ) const;
	virtual void			Erase				( const FLOAT8 rotation
												, const FLOAT8 sfac ) const;
	virtual void			DrawEnds			( void ) const;
	virtual CeDistance*		GetpDistance		( const CeArc* const pArc ) const;
	virtual UINT4			GetCount			( void ) const;
	virtual void			GetpFeatures		( CeFeature** pFeatures ) const;
	virtual void			GetSpans			( CeDistance* distances ) const;
	virtual void			DrawAngles			( const CePoint* const pFrom
												, CeDC& gdc ) const;
	virtual void			DrawAngles			( const CePoint* const pFrom
												, CeView* view
												, CDC* pDC
												, const CeWindow* const pWin=0 ) const;
	virtual void			CreateAngleText		( CPtrList& text
												, const LOGICAL wantLinesToo
												, const CePoint* const pFrom ) const;
	virtual	UINT4			GetPrecision		( void ) const;
	virtual LOGICAL			Adjust				( FLOAT8& dNorthing
												, FLOAT8& dEasting
												, FLOAT8& precision
												, FLOAT8& length
												, FLOAT8& rotation
												, FLOAT8& sfac ) const;
	virtual UINT4			GetFeatures			( CeObjectList& flist ) const;
	virtual LOGICAL			GetCircles			( CeObjectList& clist
												, const CePoint& point ) const;
	virtual CeLeg*			GetpLeg				( const CeFeature& feature ) const;
	virtual CeLeg*			GetpLeg				( const INT4 index ) const;
	virtual CeStraightLeg*	GetpStraightLeg		( const LOGICAL prevStraightToo
												, const CeFeature& feature ) const;
	virtual LOGICAL			HasReference		( const CeFeature* const pFeat ) const;
	virtual INT4			GetLegIndex			( const CeLeg& leg ) const;
	virtual INT4			GetNumLeg			( void ) const;
	virtual LOGICAL			IsLastLeg			( const CeLeg* const pLeg ) const;
	virtual const CeLocation* GetpStart			( void ) const;
	virtual const CeLocation* GetpEnd			( void ) const;
	virtual void			GetString			( CString& str ) const;
	virtual	UINT4			LoadVertexList		( CeVertexList& vlist ) const;
	virtual	LOGICAL			GetLegEnds			( const CeLeg& leg
												, CeVertex& start
												, CeVertex& end ) const;
	virtual LOGICAL			Rollback			( void );
	virtual LOGICAL			Execute				( void );
	virtual void			AddReferences		( void ) const;
	virtual LOGICAL			Create				( const CePathItem* const items
												, const UINT2 nitem );
	virtual void			Intersect			( void );
	virtual LOGICAL			Rollforward			( void );
	virtual LOGICAL			InsertLeg			( const CeLeg* const pCurLeg
												, const CeLeg* const pNewLeg );
	virtual CeLeg*			InsertFace			( CeLeg* pAfter
												, const UINT4 nDist
												, const CeDistance* dists );

private:

	virtual UINT2			CountLegs			( const CePathItem* const items
												, const UINT2 nitem ) const;
	virtual	LOGICAL			GetAdjustment		( FLOAT8& rotation
												, FLOAT8& sfac ) const;
	virtual CeArc*			GetFirstArc			( void ) const;
	virtual CeArc*			GetLastArc			( void ) const;
	virtual void			KillLegs			( void );
	virtual	CeLeg*			CreateStraightLeg	( const CePathItem* const items
												, const UINT2 nitem
												, const UINT2 si
												, UINT2& ei ) const;
	virtual	CeLeg*			CreateCircularLeg	( const CePathItem* const items
												, const UINT2 nitem
												, const UINT2 si
												, UINT2& ei ) const;
	virtual	LOGICAL			IsInsertAtStart		( void ) const;
	virtual	LOGICAL			IsInsertAtEnd		( void ) const;
         */

        bool Create(PathItem[] items)
        {
            return false;
        }
        /*
//	@mfunc	Create this path using an array of path items. This
//			function should be called after allocating the memory
//			for a CePath object.
//
//			Note that if the path object is TRANSIENT, no points
//			or arcs will be created for the path, even if the
//			path items indicate that they should be.
//
//	@parm	Array of path items.
//	@parm	The number of items in the array.
//
//	@rdesc	TRUE if path successfully populated.
//
//////////////////////////////////////////////////////////////////////

LOGICAL CePath::Create ( const CePathItem* const items
					   , const UINT2 nitem ) {

//	Count the number of legs.
	m_NumLeg = CountLegs(items,nitem);
	if ( m_NumLeg==0 ) {
		ShowMessage("CePath::Create\nNo connection legs");
		return FALSE;
	}

//	Create array of pointers to each leg.
	m_pLegs = new ( os_database::of(this),
				    os_typespec::get_pointer(),
					m_NumLeg ) CeLeg*[m_NumLeg];

//	Create each leg.

	UINT2 legnum=0;			// Current leg number
	UINT2 nexti=0;			// Index of the start of the next leg

	for ( UINT2 si=0; si<nitem; si=nexti ) {

//		Skip if no leg number (could be new units spec).
		if ( items[si].GetLeg()==0 ) {
			nexti = si+1;
			continue;
		}

//		Confirm the leg count is valid.
		if ( legnum+1>m_NumLeg ) {
			ShowMessage("CePath::Create\nBad number of path legs.");
			return FALSE;
		}

//		Create the leg.
		if ( items[si].GetType()==PAT_BC )
			m_pLegs[legnum] = CreateCircularLeg(items,nitem,si,nexti);
		else
			m_pLegs[legnum] = CreateStraightLeg(items,nitem,si,nexti);

//		Exit if we failed to create the leg.
		if ( !m_pLegs[legnum] ) return FALSE;

//		Increment the number of legs we have created.
		legnum++;

	} // next leg

//	Confirm we created the number of legs we expected.
	if ( m_NumLeg!=legnum ) {
		ShowMessage("CePath::Create -- unexpected number of legs");
		return FALSE;
	}

	return TRUE;

} // end of Create
#endif

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Count the number of legs for this path.
//
//	@parm	Array of path items.
//	@parm	The number of items in the array.
//
//	@rdesc	The number of legs.
//
//////////////////////////////////////////////////////////////////////

UINT2 CePath::CountLegs ( const CePathItem* const items
					    , const UINT2 nitem ) const {

//	Each path item contains a leg number, arranged sequentially.

	UINT2 nleg=0;
	UINT2 curleg;

	for ( UINT2 i=0; i<nitem; i++ ) {
		curleg = items[i].GetLeg();
		if ( curleg>nleg ) nleg = curleg;
	}

	return nleg;

} // end of CountLegs

#ifdef _CEDIT
//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Create a circular leg.
//
//	@parm	Array of path items.
//	@parm	The number of items in the array.
//	@parm	Index to the item where the leg data starts.
//	@parm	Index of the item where the next leg starts.
//
//	@rdesc	Pointer to the new leg.
//
//////////////////////////////////////////////////////////////////////

CeLeg* CePath::CreateCircularLeg ( const CePathItem* const items
								 , const UINT2 nitem
							     , const UINT2 si
								 , UINT2& nexti ) const {

//	Confirm that the first item refers to the BC.
	if ( items[si].GetType() != PAT_BC ) {
		ShowMessage("CePath::CreateCircularLeg\nNot starting at BC");
		return 0;
	}

//	The BC has to be followed by at least 3 items: angle, radius
//	and EC (add an extra 1 to account for 0-based indexing).
	if ( nitem < si+4 ) {
		ShowMessage("CePath::CreateCircularLeg\nInsufficient curve data");
		return 0;
	}

	FLOAT8		bangle=0.0;			// Angle at BC
	FLOAT8		cangle=0.0;			// Central angle
	FLOAT8		eangle=0.0;			// Angle at EC
	CeDistance	radius;				// Radius
	LOGICAL		twoangles=FALSE;	// True if bangle & eangle are both defined.
	LOGICAL		clockwise=TRUE;		// True if curve is clockwise
	UINT2		irad=0;				// Index of the radius item
	LOGICAL		cul=FALSE;			// True if cul-de-sac case

//	Point to item following the BC.
	nexti = si+1;
	PAT type = items[nexti].GetType();

//	If the angle following the BC is a central angle
	if (  type==PAT_CANGLE ) {

//		We have a cul-de-sac
		cul = TRUE;

//		Get the central angle.
		cangle = items[nexti].GetValue();
		nexti++;
	}
	else if ( type==PAT_BANGLE ) {

//		Get the entry angle.
		bangle = items[nexti].GetValue();
		nexti++;

//		Does an exit angle follow?
		if ( items[nexti].GetType() == PAT_EANGLE ) {
			eangle = items[nexti].GetValue();
			twoangles = TRUE;
			nexti++;
		}
	}
	else {

//		The field after the BC HAS to be an angle.
		ShowMessage("Angle does not follow BC");
		return 0;
	}

//	Must be followed by radius.
	if ( items[nexti].GetType() != PAT_RADIUS ) {
		ShowMessage("Radius does not follow angle");
		return 0;
	}

//	Get the radius
	items[nexti].GetDistance(radius);
	irad = nexti;
	nexti++;

//	The item after the radius indicates whether the curve
//	is counterclockwise.
	if ( items[nexti].GetType() == PAT_CC ) {
		nexti++;
		clockwise = FALSE;
	}

//	Get the leg ID.
	UINT2 legnum = items[si].GetLeg();

//	How many distances have we got?
	UINT2 ndist=0;
	for ( ; nexti<nitem && items[nexti].GetLeg()==legnum; nexti++ ) {
		if ( items[nexti].IsDistance() ) ndist++;
	}

//	Create the leg.
	CeCircularLeg* pLeg =
		new ( os_database::of(this)
		    , os_ts<CeCircularLeg>::get() )
			  CeCircularLeg(radius,clockwise,ndist);
//	CHARS msg[80];
//	sprintf ( msg, "CePath::CreateCircularLeg = %x", pLeg );
//	ShowMessage(msg);

//	Set the entry angle or the central angle, depending on
//	what we have.
	if ( cul )
		pLeg->SetCentralAngle(cangle);
	else
		pLeg->SetEntryAngle(bangle);

//	Assign second angle if we have one.
	if ( twoangles ) pLeg->SetExitAngle(eangle);

//	Assign each distance, starting one after the radius.
	CeDistance dist;
	ndist=0;
	for ( UINT2 i=irad+1; i<nexti; i++ ) {
		if ( items[i].GetDistance(dist) ) {

//			See if there is a qualifier after the distance
			SWT qual = SWT_NULL;
			if ( i+1<nexti ) {
				PAT nexttype = items[i+1].GetType();
				if ( nexttype==PAT_MC ) qual = SWT_MC;
				if ( nexttype==PAT_OP ) qual = SWT_OP;
			}

			pLeg->SetDistance(dist,ndist,qual);
			ndist++;
		}
	}

//	Return pointer to the leg.
	return pLeg;

} // end of CreateCircularLeg
#endif

#ifdef _CEDIT
//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Create a straight leg.
//
//	@parm	Array of path items.
//	@parm	The number of items in the array.
//	@parm	Index to the item where the leg data starts.
//	@parm	Index of the last item used to define the leg.
//
//	@rdesc	Pointer to the new leg.
//
//////////////////////////////////////////////////////////////////////

CeLeg* CePath::CreateStraightLeg ( const CePathItem* const items
								 , const UINT2 nitem
							     , const UINT2 si
								 , UINT2& nexti ) const {

//	Get the leg ID.
	UINT2 legnum = items[si].GetLeg();

//	How many distances have we got?
	UINT2 ndist=0;
	for ( nexti=si; nexti<nitem && items[nexti].GetLeg()==legnum; nexti++ ) {
		if ( items[nexti].IsDistance() ) ndist++;
	}

//	Create the leg.
	CeStraightLeg* pLeg = new ( os_database::of(this)
							  , os_ts<CeStraightLeg>::get() )
							  CeStraightLeg(ndist);
//	CHARS msg[80];
//	sprintf ( msg, "CePath::CreateStraightLeg = %x", pLeg );
//	ShowMessage(msg);

//	Assign each distance.
	CeDistance dist;
	ndist=0;
	for ( UINT2 i=si; i<nexti; i++ ) {
		if ( items[i].GetDistance(dist) ) {

//			See if there is a qualifier after the distance
			SWT qual = SWT_NULL;
			if ( i+1<nexti ) {
				PAT nexttype = items[i+1].GetType();
				if ( nexttype==PAT_MC ) qual = SWT_MC;
				if ( nexttype==PAT_OP ) qual = SWT_OP;
			}
			
			pLeg->SetDistance(dist,ndist,qual);
			ndist++;
		}
	}

//	If the first item is an angle, remember it as part of the leg.

	if ( items[si].GetType() == PAT_ANGLE )
		pLeg->SetAngle(items[si].GetValue());
	else if ( items[si].GetType() == PAT_DEFLECTION )
		pLeg->SetDeflection(items[si].GetValue());

//	Return pointer to the leg.
	return pLeg;

} // end of CreateStraightLeg
#endif

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Adjust the path (Helmert adjustment).
//
//	@parm	Misclosure in northing.
//	@parm	Misclosure in easting.
//	@parm	Precision denominator (zero if no adjustment needed).
//	@parm	Total observed length.
//	@parm	The clockwise rotation to apply (in radians).
//	@parm	The scaling factor to apply.
//
//	@rdesc	TRUE if adjusted ok.
//
//////////////////////////////////////////////////////////////////////

LOGICAL CePath::Adjust ( FLOAT8& dN
					   , FLOAT8& dE
					   , FLOAT8& precision
					   , FLOAT8& length
					   , FLOAT8& rotation
					   , FLOAT8& sfac ) const {

//	Initialize position to the start of the path.
	CeVertex start(*m_pFrom);
	CeVertex gotend(start);		// Un-adjusted end point

//	Initial bearing is due north.
	FLOAT8 bearing = 0.0;
	length = 0.0;

//	Go through each leg, updating the end position, and getting
//	the total path length.
	for ( UINT2 i=0; i<m_NumLeg; i++ ) {
		length += m_pLegs[i]->GetLength();
		m_pLegs[i]->Project(gotend,bearing);
	}

//	Get the bearing and distance of the end point we ended up with.
	CeQuadVertex qgot(start,gotend);
	FLOAT8 gotbear = qgot.GetBearing();
	FLOAT8 gotdist = sqrt(start.DistanceSquared(gotend));

//	Get the bearing and distance we want.
	CeVertex wantend(*m_pTo);
	CeQuadVertex qwant(start,wantend);
	FLOAT8 wantbear = qwant.GetBearing();
	FLOAT8 wantdist = sqrt(start.DistanceSquared(wantend));

//	Figure out the rotation.
	rotation = wantbear-gotbear;

//	Rotate the end point we got.
	gotend.Rotate(start,rotation);

//	Calculate the line scale factor.
	FLOAT8 linefac =
		CeMap::GetpMap()->GetCoordSystem().GetLineScaleFactor(start,gotend);

//	Figure out where the rotated end point ends up when we
//	apply the line scale factor.
	gotend = CeVertex(start,wantbear,gotdist*linefac);

//	What misclosure do we have?
	dN = gotend.GetNorthing() - wantend.GetNorthing();
	dE = gotend.GetEasting() - wantend.GetEasting();
	if ( fabs(dN)<TINY ) dN = 0.0;
	if ( fabs(dE)<TINY ) dE = 0.0;
	FLOAT8 delta = sqrt(dN*dN + dE*dE);

//	What's the precision denominator (use a value of 0 to
//	denote an exact match).
	if ( delta>TINY )
		precision = wantdist/delta;
	else
		precision = 0.0;

//	Figure out the scale factor for the adjustment (use a
//	value of 0 if the start and end points are coincident).
//	The distances here have NOT been adjusted for the line
//	scale factor.
	if ( gotdist>TINY )
		sfac = wantdist/gotdist;
	else
		sfac = 0.0;

	return TRUE;

} // end of Adjust

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Draw this path.
//
//	@parm	The rotation to apply (in radians). Clockwise rotations
//			are>0.
//	@parm	The scale factor to apply.
//	@parm	TRUE if the draw should actually erase (default
//			is FALSE).
//
//////////////////////////////////////////////////////////////////////

void CePath::Draw ( const FLOAT8 rotation
				  , const FLOAT8 sfac
				  , const LOGICAL erase ) const {

	// Do nothing if the scale factor is undefined.
	if ( fabs(sfac)<TINY ) return;

//	Initialize position to the start of the path.
	CeVertex start(*m_pFrom);
	CeVertex gotend(start);		// Un-adjusted end point

//	Initial bearing is whatever the desired rotation is.
	FLOAT8 bearing = rotation;

//	Go through each leg, updating the end position, and getting
//	the total path length.
	for ( UINT2 i=0; i<m_NumLeg; i++ ) {
		if ( erase )
			m_pLegs[i]->Erase(gotend,bearing,sfac);
		else
			m_pLegs[i]->Draw(gotend,bearing,sfac);
	}

//	Re-draw the terminal points to ensure that their colour
//	is on top.
	DrawEnds();

} // end of Draw

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Draw a previously saved path.
//
//	@parm	TRUE if the path should be drawn in preview mode (i.e.
//			in the normal construction colour, with miss-connects
//			shown as dotted lines).
//
//////////////////////////////////////////////////////////////////////

void CePath::Draw ( const LOGICAL preview ) const {

	for ( UINT2 i=0; i<m_NumLeg; i++ )
		m_pLegs[i]->Draw(preview);

} // end of Draw

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Erase this path.
//
//	@parm	The rotation to apply (in radians). Clockwise rotations
//			are>0.
//	@parm	The scale factor to apply.
//
//////////////////////////////////////////////////////////////////////

void CePath::Erase ( const FLOAT8 rotation
				   , const FLOAT8 sfac ) const {

	this->Draw(rotation,sfac,TRUE);

} // end of Erase

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Draw the end points for this path.
//
//////////////////////////////////////////////////////////////////////

void CePath::DrawEnds ( void  ) const {

//	It is assumed that the colours specified here are consistent
//	with the dialog (see CdPath::SetColour).

	if ( m_pFrom ) m_pFrom->DrawThis(COL_DARKBLUE);
	if ( m_pTo   ) m_pTo->DrawThis(COL_LIGHTBLUE);

} // end of DrawEnds

#ifdef _CEDIT
//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Execute this operation. This attaches persistent features
//			to the path.
//
//////////////////////////////////////////////////////////////////////

LOGICAL CePath::Execute ( void ) {

//	To from and to points MUST be defined.
	if ( !(m_pFrom && m_pTo) ) {
		ShowMessage ( "CePath::Execute -- terminal(s) not defined." );
		return FALSE;
	}

//	Get the rotation & scale factor to apply.
	FLOAT8 rotation;
	FLOAT8 sfac;
	GetAdjustment( rotation, sfac );

//	Go through each leg, adding features as required. This logic
//	is basically the same as the Draw() logic ...

//	Initialize position to the start of the path.
	CeVertex start(*m_pFrom);
	CeVertex gotend(start);		// Un-adjusted end point

//	Initial bearing is whatever the desired rotation is.
	FLOAT8 bearing = rotation;

//	Go through each leg, asking them to make features.
	for ( UINT2 i=0; i<m_NumLeg; i++ ) {
		if ( !(m_pLegs[i]->Save(*this,gotend,bearing,sfac)) ) return FALSE;
	}

//	Add references to this operation.
	this->AddReferences();

//	Handle any intersection the path has with the map.
	this->Intersect();

//	Clean up the map.
	CeMap::GetpMap()->CleanEdit();

	return TRUE;

} // end of Execute
#endif

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Return adjustment parameters (the bare bones).
//
//	@parm	The rotation to apply.
//	@parm	The scale factor to apply.
//
//////////////////////////////////////////////////////////////////////

LOGICAL CePath::GetAdjustment ( FLOAT8& rotation
							  , FLOAT8& sfac ) const {

	FLOAT8 dN;			// Misclosure in northings
	FLOAT8 dE;			// Misclosure in eastings
	FLOAT8 precision;	// Precision denominator
	FLOAT8 length;		// Total observed length

	return this->Adjust(dN,dE,precision,length,rotation,sfac);

} // end of GetAdjustment

#ifdef _CEDIT
//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Handle any intersections created by this operation.
//
//////////////////////////////////////////////////////////////////////

void CePath::Intersect ( void ) {

	for ( UINT2 i=0; i<m_NumLeg; i++ )
		m_pLegs[i]->Intersect(*this);

} // end of Intersect
#endif

#ifdef _CEDIT
//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Rollforward this operation.
//
//	@rdesc	TRUE on success.
//
//////////////////////////////////////////////////////////////////////

#include "CeExtraLeg.h"

LOGICAL CePath::Rollforward ( void ) {

	// Return if this operation has not been marked as changed.
	if ( !this->IsChanged() ) return CeOperation::OnRollforward();

	// If a line was originally attached to the start point (but
	// is now preceded by one or more inserts), alter the start
	// location so that its at a duplicate location.

	// Don't do it at the start, since that's already covered
	// via the passing of the end of the last insert (see below).
	// If you do the following, the duplicate position will just
	// get changed again!

	//if ( IsInsertAtStart() ) {
	//	CeArc* pFirst = GetFirstArc();
	//	CeLocation* pStart = (CeLocation*)m_pFrom->GetpVertex();
	//	if ( pFirst->GetpStart() == pStart ) {
	//		CeMap* pMap = CeMap::GetpMap();
	//		CeLocation* pS = pMap->AddDuplicate(*pStart);
	//		CeLocation* pE = pFirst->GetpEnd();
	//		pFirst->GetpLine()->ChangeEnds(*pS,*pE);
	//	}
	//}

	// And similarly at the end. This is needed, because the very
	// end the path never gets shifted (we need to detach it now
	// to make sure the end of the existing line does move, since
	// an insert will be taking its place).

	if ( IsInsertAtEnd() ) {
		CeArc* pLast = GetLastArc();
		CeLocation* pEnd = (CeLocation*)m_pTo->GetpVertex();
		if ( pLast->GetpEnd() == pEnd ) {
			CeMap* pMap = CeMap::GetpMap();
			CeLocation* pS = pLast->GetpStart();
			CeLocation* pE = pMap->AddDuplicate(*pEnd);
			CePoint* pPoint = pMap->AddPoint(pE,0);
			pPoint->SetpCreator(*this);
			pPoint->SetNextId();
			pLast->GetpLine()->ChangeEnds(*pS,*pE);
		}
	}

	// Get the rotation & scale factor to apply.
	FLOAT8 rotation;
	FLOAT8 sfac;
	GetAdjustment( rotation, sfac );

//	Notify each leg of the change ...

//	Initialize position to the start of the path.
	CeVertex start(*m_pFrom);
	CeVertex gotend(start);		// Un-adjusted end point
	CeLocation* pInsert = 0;	// No insert prior to start

//	Initial bearing is whatever the desired rotation is.
	FLOAT8 bearing = rotation;

	// Go through each leg, telling them to "save" (the leg actually
	// checks whether it already refers to features).

	for ( UINT2 i=0; i<m_NumLeg; i++ ) {

		// For the second face of a staggered leg, we need to supply
		// the newly adjusted ends of the leg that has just been
		// processed (the regular version of it's rollforward function
		// does nothing).

		if ( m_pLegs[i]->GetFaceNumber()==2 ) {
			CeExtraLeg* pLeg = dynamic_cast<CeExtraLeg*>(m_pLegs[i]);
			if ( !pLeg ) {
				AfxMessageBox("Second face has unexpected data type");
				return FALSE;
			}
			if ( !pLeg->Rollforward(pInsert,*this,start,gotend) ) return FALSE;
		}
		else {
			start = gotend;
			if ( !(m_pLegs[i]->Rollforward(pInsert,*this,gotend,bearing,sfac)) ) return FALSE;
		}

	} // next leg

//	Rollforward the base class.
	return CeOperation::OnRollforward();

} // end of Rollforward
#endif

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Find the observed length of an arc that was created
//			by this operation.
//
//	@parm	The arc to find.
//
//	@rdesc	Pointer to the distance (0 if not found).
//
//////////////////////////////////////////////////////////////////////

CeDistance* CePath::GetpDistance ( const CeArc* const pArc ) const {

//	No point doing anything if the arc pointer is undefined.
	if ( !pArc ) return 0;

//	We need a feature pointer for the call.
//	const CeFeature* const pFeat =
//		dynamic_cast<const CeFeature* const>(pArc);

//	Ask each leg to try to locate the arc.

	CeDistance* pDist=0;

	for ( UINT2 i=0; i<m_NumLeg && !pDist; i++ )
//		pDist = m_pLegs[i]->GetpDistance(pFeat);
		pDist = m_pLegs[i]->GetpDistance(pArc);	//09dec99

	return pDist;

} // end of GetpDistance

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Return the total number of observed spans for this
//			connection path. The return value is suitable for
//			allocating memory for subsequent calls to
//			<mf CePath::GetpFeatures> and <mf CePath::GetDistances>.
//
//////////////////////////////////////////////////////////////////////

UINT4 CePath::GetCount ( void ) const {

//	Accumulate the number of spans in each leg. Treat cul-de-sacs
//	with no observed spans as a count of 1 (although there is no
//	observation, a feature is still created for it).

	UINT4 tot=0;

	for ( UINT2 i=0; i<m_NumLeg; i++ ) {
		UINT2 nspan = max(1,m_pLegs[i]->GetCount());
		tot += UINT4(nspan);
	}

	return tot;

} // end of GetCount

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Load an array with pointers to all the features that
//			exist on this path. Each span could be represented by
//			pointers to CeArc or CePoint features. A span that is
//			a miss-connect will have a NULL feature pointer.
//
//	@parm	Array of CeFeature pointers to load. The allocation
//			for the array must match the value returned from
//			<CePath::GetCount>.
//
//////////////////////////////////////////////////////////////////////

void CePath::GetpFeatures ( CeFeature** pFeatures ) const {

	UINT4 tot=0;

	for ( UINT2 i=0; i<m_NumLeg; i++ )
		m_pLegs[i]->GetpFeatures(pFeatures,tot);

} // end of GetpFeatures

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Load an array of CeDistance objects with the observed
//			lengths of each span on this path. In the case of
//			cul-de-sacs that have no observed span, the distance
//			will be derived from the central angle and radius.
//
//	@parm	Array of CeDistance objects to load. The allocation
//			for this array must match the value returned from
//			<CePath::GetCount>.
//
//////////////////////////////////////////////////////////////////////

void CePath::GetSpans ( CeDistance* distances ) const {

	UINT4 tot=0;

	for ( UINT2 i=0; i<m_NumLeg; i++ )
		m_pLegs[i]->GetSpans(distances,tot);

} // end of GetSpans

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Draw observed angles recorded as part of this op.
//
//	@parm	The point the observation must be made from.
//			Specify 0 for all points.
//	@parm	The view to draw in.
//	@parm	Display context for the draw.
//	@parm	The ground window for the draw (0 for no check).
//
//////////////////////////////////////////////////////////////////////

void CePath::DrawAngles	( const CePoint* const pFrom
						, CeView* view
						, CDC* pDC
						, const CeWindow* const pWin ) const {

	// If a from-point has been specified, it must be a point
	// that was created by this path (you can't start a path
	// off with an angle, so the angle in question would have
	// to be some interior angle).
	if ( pFrom && pFrom->GetpCreator()!=(CeOperation*)this ) return;

	// Skip this op if it has an update.
	if ( this->GetpLatest() ) return;

	// Get the rotation & scale factor that was applied to
	// this path.
	FLOAT8 rotation;
	FLOAT8 sfac;
	GetAdjustment( rotation, sfac );

	// Initial bearing is whatever the rotation was.
	FLOAT8 bearing = rotation;

	// The first leg does not have a backsight. We pass down
	// vertices to each leg because the leg may contain empty
	// spans (i.e. with neither an arc or a point feature).

	CeVertex bs;
	CeVertex from(*m_pFrom);
	CeVertex to;

	// Get each leg to draw its angles.
	for ( UINT2 i=0; i<m_NumLeg; i++ ) {
		m_pLegs[i]->DrawAngles(pFrom,*this,sfac,bs,from,bearing,to,view,pDC,pWin);
		bs = from;
		from = to;
	}

} // end of DrawAngles

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Draw observed angles recorded as part of this op.
//
//	@parm	The point the observation must be made from.
//			Specify 0 for all points.
//	@parm	The thing we're drawing to.
//
//////////////////////////////////////////////////////////////////////

void CePath::DrawAngles	( const CePoint* const pFrom
						, CeDC& gdc ) const {

	// If a from-point has been specified, it must be a point
	// that was created by this path (you can't start a path
	// off with an angle, so the angle in question would have
	// to be some interior angle).
	if ( pFrom && pFrom->GetpCreator()!=(CeOperation*)this ) return;

	// Skip this op if it has an update.
	if ( this->GetpLatest() ) return;

	// Get the rotation & scale factor that was applied to
	// this path.
	FLOAT8 rotation;
	FLOAT8 sfac;
	GetAdjustment( rotation, sfac );

	// Initial bearing is whatever the rotation was.
	FLOAT8 bearing = rotation;

	// The first leg does not have a backsight. We pass down
	// vertices to each leg because the leg may contain empty
	// spans (i.e. with neither an arc or a point feature).

	CeVertex bs;
	CeVertex from(*m_pFrom);
	CeVertex to;

	// Get each leg to draw its angles.
	for ( UINT2 i=0; i<m_NumLeg; i++ ) {
		m_pLegs[i]->DrawAngles(pFrom,*this,sfac,bs,from,bearing,to,gdc);
		bs = from;
		from = to;
	}

} // end of DrawAngles

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Return a list of the features that were created by
//			this operation.
//
//	@parm	The list to store the results. This list will be
//			appended to, so you may want to clear the list
//			prior to call.
//
//	@rdesc	The number of features in the list at return.
//
//////////////////////////////////////////////////////////////////////

#include "CeObjectList.h"

UINT4 CePath::GetFeatures ( CeObjectList& flist ) const {

	// Append the features that were created on each leg of
	// the path.
	for ( UINT2 i=0; i<m_NumLeg; i++ ) m_pLegs[i]->GetFeatures(*this,flist);

	return flist.GetCount();

} // end of GetFeatures

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Return the definition of the leg which created a
//			specific feature.
//
//	@parm	The feature to find.
//
//	@rdesc	Pointer to the leg that created the feature (or null
//			if the leg could not be found).
//
//////////////////////////////////////////////////////////////////////

CeLeg* CePath::GetpLeg ( const CeFeature& feature ) const {

	for ( UINT2 i=0; i<m_NumLeg; i++ ) {
		if ( m_pLegs[i]->IsCreatorOf(feature) ) return m_pLegs[i];
	}

	return 0;

} // end of GetpLeg

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Return the definition of the straight leg which
//			created a specific feature.
//
//	@parm	Does the leg preceding the one found need to be
//			straight as well? If you say TRUE, a previous
//			leg MUST exist.
//	@parm	The feature to find.
//
//	@rdesc	Pointer to the leg that created the feature (or null
//			if the leg could not be found).
//
//////////////////////////////////////////////////////////////////////

CeStraightLeg* CePath::GetpStraightLeg ( const LOGICAL prevStraightToo
									   , const CeFeature& feature ) const {

	// Find the leg that created the specified feature.
	INT2 index = -1;
	for ( UINT2 i=0; i<m_NumLeg && index<0; i++ ) {
		if ( m_pLegs[i]->IsCreatorOf(feature) ) index = i;
	}

	// Return if it wasn't found.
	if ( index<0 ) return 0;

	// If the preceding leg has to be straight, getting the
	// very first leg is no good.
	if ( prevStraightToo && index==0 ) return 0;

	// The leg HAS to be straight.
	CeStraightLeg* pStraight =
		dynamic_cast<CeStraightLeg*>(m_pLegs[index]);
	if ( !pStraight ) return 0;

	// Check if the preceding leg has to be straight as well.
	if ( prevStraightToo ) {
		CeStraightLeg* pPrev = 
			dynamic_cast<CeStraightLeg*>(m_pLegs[index-1]);
		if ( pPrev==0 ) return 0;
	}

	return pStraight;

} // end of GetpStraightLeg

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Get any circles that were used to establish the position
//			of a point that was created by this operation.
//
//	@parm	The list to append any circles to (duplicates will NOT
//			be inserted).
//	@parm	One of the point features created by this op (either
//			explicitly referred to, or added as a consequence of
//			creating a new line).
//
//	@rdesc	TRUE if request was handled (does not necessarily mean
//			that any circles were found). FALSE if this is a
//			do-nothing function.
//
//////////////////////////////////////////////////////////////////////

#include "CeCircle.h"

LOGICAL CePath::GetCircles ( CeObjectList& clist
						   , const CePoint& point ) const {

	// Process each leg in turn.
	for ( UINT2 i=0; i<m_NumLeg; i++ ) {

		// If the leg created the specified point
		if ( m_pLegs[i]->IsCreatorOf(point) ) {

			// Get the circle (if any) for the leg (straight legs
			// always return nothing).
			CeCircle* pCircle = m_pLegs[i]->GetpCircle();

			// If we got something, add it into the return list (no dups).
			if ( pCircle ) clist.AddReference(pCircle);

			// Even if we have just inserted a circle, it's possible
			// that the NEXT leg is also circular. We want to add it
			// as well, so long as the point is at the END of the
			// current leg.

			if ( (i+1) < m_NumLeg ) {
				pCircle = m_pLegs[i+1]->GetpCircle();
				if ( pCircle &&
					 m_pLegs[i]->GetpEndPoint(*this) == &point )
					 clist.AddReference(pCircle);
			}

			// We're all done. Since the point was created by the
			// current leg, there's no way any other leg could have
			// also created it.
			return TRUE;
		}

	} // next leg

	return TRUE;

} // end of GetCircles

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Create transient CeMiscText objects for any observed
//			angles that are part of this operation.
//
//			The caller is responsible for deleting the text
//			objects once done with them.
//
//	@parm	List of pointers to created text objects (appended to).
//	@parm	Should lines be produced too?
//	@parm	The associated point that this op must reference.
//
//////////////////////////////////////////////////////////////////////

void CePath::CreateAngleText ( CPtrList& text
							 , const LOGICAL wantLinesToo
							 , const CePoint* const pFrom ) const {

	// If a from-point has been specified, it must be a point
	// that was created by this path (you can't start a path
	// off with an angle, so the angle in question would have
	// to be some interior angle).
	if ( pFrom && pFrom->GetpCreator()!=(CeOperation*)this ) return;

	// Skip this op if it has an update.
	if ( this->GetpLatest() ) return;

	// Get the rotation & scale factor that was applied to
	// this path.
	FLOAT8 rotation;
	FLOAT8 sfac;
	GetAdjustment( rotation, sfac );

	// Initial bearing is whatever the rotation was.
	FLOAT8 bearing = rotation;

	// The first leg does not have a backsight. We pass down
	// vertices to each leg because the leg may contain empty
	// spans (i.e. with neither an arc or a point feature).

	CeVertex bs;
	CeVertex from(*m_pFrom);
	CeVertex to;

	// Get each leg to create the angle text (exit as soon
	// as a leg says it handled the specified from-point).
	for ( UINT2 i=0; i<m_NumLeg; i++ ) {
		if ( m_pLegs[i]->CreateAngleText(pFrom,*this,sfac,bs,from,bearing,to,text,wantLinesToo) ) return;
		bs = from;
		from = to;
	}

} // end of CreateAngleText

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Calculate the precision of the connection path.
//
//	@rdesc	The precision.
//
//////////////////////////////////////////////////////////////////////

UINT4 CePath::GetPrecision ( void ) const {

	FLOAT8 de;				// Misclosure in eastings
	FLOAT8 dn;				// Misclosure in northings
	FLOAT8 prec;			// Precision
	FLOAT8 length;			// Total observed length
	FLOAT8 rotation;		// Rotation for adjustment
	FLOAT8 sfac;			// Adjustment scaling factor

	Adjust(dn,de,prec,length,rotation,sfac);

	// If it's REALLY good, return 1 billion.

	if ( prec > FLOAT8(0xFFFFFFFF) )
		return 1000000000;
	else
		return UINT4(prec);

} // end of GetPrecision

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Return the array index of a specific leg.
//
//	@parm	The leg of interest.
//
//	@rdesc	The array index of the leg (-1 if not found).
//
//////////////////////////////////////////////////////////////////////

INT4 CePath::GetLegIndex ( const CeLeg& leg ) const {

	for ( INT4 i=0; i<m_NumLeg; i++ ) {
		if ( m_pLegs[i] == &leg ) return i;
	}

	return -1;

} // end of GetLegIndex

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Return the address of a leg.
//
//	@parm	The array index of the desired leg.
//
//	@rdesc	The address of the leg (null if index out of range).
//
//////////////////////////////////////////////////////////////////////

CeLeg* CePath::GetpLeg ( const INT4 index ) const {

	if ( index<0 || index>=m_NumLeg ) return 0;
	return m_pLegs[index];

} // end of GetpLeg

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Return the total number of legs in this path.
//
//	@rdesc	The leg count.
//
//////////////////////////////////////////////////////////////////////

INT4 CePath::GetNumLeg ( void ) const
	{ return (INT4)m_NumLeg; }

#ifdef _CEDIT
//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Insert an extra leg into this connection path.
//
//	@parm	The leg that we already know about.
//	@parm	The extra leg that should follow it.
//
//	@rdesc	TRUE if inserted ok.
//
//////////////////////////////////////////////////////////////////////

LOGICAL CePath::InsertLeg ( const CeLeg* const pCurLeg
						  , const CeLeg* const pNewLeg ) {

	// Find the leg.
	if ( pCurLeg==0 || pNewLeg==0 ) return FALSE;
	INT4 index = GetLegIndex(*pCurLeg);
	if ( index<0 ) return FALSE;

	// Define 1st array index for the new guy.
	index++;

	// Allocate a new array of leg pointers.
	CeLeg** newlegs =
		new ( os_database::of(this)
			, os_typespec::get_pointer()
			, m_NumLeg+1 ) CeLeg*[m_NumLeg+1];

	// Copy over the stuff prior to the new one.
	for ( UINT2 i=0; i<index; i++ ) { newlegs[i] = m_pLegs[i]; }

	// Append the new one.
	newlegs[index] = (CeLeg*)pNewLeg;

	// Copy over any stuff that comes after it.
	for ( i=index; i<m_NumLeg; i++ ) { newlegs[i+1] = m_pLegs[i]; }

	// Replace the original array with the new one.
	delete [] m_pLegs;
	m_pLegs = newlegs;
	m_NumLeg++;
	return TRUE;

} // end of InsertLeg
#endif

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Get the location where this path starts from.
//
//	@rdesc	The start location.
//
//////////////////////////////////////////////////////////////////////

const CeLocation* CePath::GetpStart ( void ) const {

	if ( m_pFrom )
		return m_pFrom->GetpVertex();
	else
		return 0;

} // end of GetpStart

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Get the location where this path ends.
//
//	@rdesc	The end location.
//
//////////////////////////////////////////////////////////////////////

const CeLocation* CePath::GetpEnd ( void ) const {

	if ( m_pTo )
		return m_pTo->GetpVertex();
	else
		return 0;

} // end of GetpEnd

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Return the very first line that was created along
//			this connection path (if any).
//
//	@rdesc	The first line (null if no lines were created).
//
//////////////////////////////////////////////////////////////////////

CeArc* CePath::GetFirstArc ( void ) const {

	CeArc* pFirst = 0;

	for ( UINT2 i=0; i<m_NumLeg && pFirst==0; i++ ) {
		pFirst = m_pLegs[i]->GetFirstArc();
	}

	return pFirst;

} // end of GetFirstArc

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Return the very last line that was created along
//			this connection path (if any).
//
//	@rdesc	The last line (null if no lines were created).
//
//////////////////////////////////////////////////////////////////////

CeArc* CePath::GetLastArc ( void ) const {

	CeArc* pLast = 0;

	for ( INT2 i=(m_NumLeg-1); i>=0 && pLast==0; i-- ) {
		pLast = m_pLegs[i]->GetLastArc();
	}

	return pLast;

} // end of GetLastArc

#ifdef _CEDIT
//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Have any new spans been inserted at the very start
//			of this connection path?
//
//	@rdesc	TRUE if insert(s) at the start.
//
//////////////////////////////////////////////////////////////////////

LOGICAL CePath::IsInsertAtStart ( void ) const {

	const CeLeg* const pLeg = m_pLegs[0];
	const UINT2 nSpan = pLeg->GetCount();
	if ( nSpan==0 ) return FALSE;
	return pLeg->IsNewSpan(0);

} // end of IsInsertAtStart
#endif

#ifdef _CEDIT
//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Have any new spans been inserted at the very end
//			of this connection path?
//
//	@rdesc	TRUE if insert(s) at the end.
//
//////////////////////////////////////////////////////////////////////

LOGICAL CePath::IsInsertAtEnd ( void ) const {

	const CeLeg* const pLeg = m_pLegs[m_NumLeg-1];
	const UINT2 nSpan = pLeg->GetCount();
	if ( nSpan==0 ) return FALSE;
	return pLeg->IsNewSpan(nSpan-1);

} // end of IsInsertAtEnd
#endif

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Return a string that represents the definition of
//			this connection path.
//
//	@parm	The string to hold the result.
//
//////////////////////////////////////////////////////////////////////

void CePath::GetString ( CString& str ) const {

	str.Empty();

	for ( UINT2 i=0; i<m_NumLeg; i++ ) {
		CString legstr;
		m_pLegs[i]->GetDataString(legstr);
		if ( i>0 ) str += ' ';
		str += legstr;
	}

} // end of GetString

#ifdef _CEDIT
//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Insert a second face for a leg in this path.
//
//	@parm	The leg that represents the existing face.
//	@parm	The number of distance objects for the 2nd face.
//	@parm	Distance observations for the 2nd face.
//
//	@rdesc	The address of the new leg.
//
//////////////////////////////////////////////////////////////////////

#include "CeExtraLeg.h"

CeLeg* CePath::InsertFace ( CeLeg* pAfter
						  , const UINT4 nDist
						  , const CeDistance* dists ) {

	// Get the index of the existing face.
	if ( !pAfter ) return 0;
	INT4 index = GetLegIndex(*pAfter);
	if ( index<0 ) return 0;

	// Make an extra leg with the specified distances.
	CeExtraLeg* pNewLeg = new ( os_database::of(this)
							  , os_ts<CeExtraLeg>::get() ) 
								CeExtraLeg(*pAfter,nDist,dists);

	// Create features for the extra leg.
	pNewLeg->MakeFeatures(*this);

	// Insert the new leg into our array of legs.
	InsertLeg(pAfter,pNewLeg);

	pAfter->SetStaggered(1);
	pNewLeg->SetStaggered(2);

	return pNewLeg;

} // end of InsertFace
#endif

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Load a vertex list with the positions that define
//			the end points of each leg in this path. The first
//			vertex corresponds to the from-point, while the
//			last vertex corresponds to the to-point. There
//			will be one more vertex than the number of legs
//			in the path (any CeExtraLeg's produce 2 coincident
//			positions in succession).
//
//	@parm	The vertex list to load.
//
//	@rdesc	The number of positions in the list (equal to the
//			number of legs + 1).
//
//////////////////////////////////////////////////////////////////////

#include "CeVertexList.h"

UINT4 CePath::LoadVertexList ( CeVertexList& vlist ) const {

	// Get the rotation & scale factor to apply.
	FLOAT8 rotation;
	FLOAT8 sfac;
	GetAdjustment( rotation, sfac );

	// Initialize position to the start of the path.
	CeVertex start(*m_pFrom);
	CeVertex gotend(start);		// Un-adjusted end point

	// Remember the initial position.
	vlist.Append(start);

	// Initial bearing is whatever the desired rotation is.
	FLOAT8 bearing = rotation;

	// Project each leg, recording the position at each end.
	for ( UINT2 i=0; i<m_NumLeg; i++ ) {
		m_pLegs[i]->Project(gotend,bearing,sfac);
		vlist.Append(gotend);
	}

	return vlist.GetCount();

} // end of LoadVertexList

//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Return the two end positions for a specific
//			leg in this connection path.
//
//	@parm	The leg of interest.
//	@parm	The start of the leg.
//	@parm	The end of the leg.
//
//	@rdesc	TRUE if positions defined ok.
//
//////////////////////////////////////////////////////////////////////

#include "CeExtraLeg.h"

LOGICAL CePath::GetLegEnds ( const CeLeg& leg
						   , CeVertex& start
						   , CeVertex& end ) const {

	// Get the array index of the specified leg.
	INT4 index = GetLegIndex(leg);
	if ( index<0 ) return FALSE;

	// Get positions for all legs.
	CeVertexList vlist;
	UINT4 nPos = LoadVertexList(vlist);
	assert(nPos==m_NumLeg+1);

	// Grab the positions. If the specified leg is actually
	// an extra leg, we actually want the positions for the
	// leg that precedes it.

	const CeExtraLeg* pExtra = dynamic_cast<const CeExtraLeg*>(&leg);
	if ( pExtra ) {
		index--;
		assert(index>=0);
	}

	start = vlist.GetPosition(index);
	end = vlist.GetPosition(index+1);

	return TRUE;

} // end of GetLegEnds
         */
    }
}
