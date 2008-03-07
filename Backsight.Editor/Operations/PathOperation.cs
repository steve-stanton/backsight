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
using System.Drawing;

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
            // No point doing anything if the line is undefined.
            if (line==null)
                return null;

            // Ask each leg to try to locate the line.
            Distance dist = null;

            for (int i = 0; i < m_Legs.Count && dist == null; i++)
                dist = m_Legs[i].GetDistance(line);

            return dist;
        }

        /// <summary>
        /// The features created by this editing operation.
        /// </summary>
        internal override Feature[] Features
        {
            get
            {
                List<Feature> result = new List<Feature>(100);

                foreach (Leg leg in m_Legs)
                    leg.GetFeatures(this, result);

                return result.ToArray();
            }
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
            base.OnRollback();

            // Cut references that features make to this operation.
            if (m_From != null)
                m_From.CutOp(this);

            if (m_To != null)
                m_To.CutOp(this);

            // Ask each leg to rollback.
            foreach (Leg leg in m_Legs)
                leg.OnRollback(this);

            return true;
        }

        /// <summary>
        /// Rollforward this edit in response to some sort of update.
        /// </summary>
        /// <returns>True if operation has been re-executed successfully</returns>
        internal override bool Rollforward()
        {
        	// Return if this operation has not been marked as changed.
            if (!IsChanged)
                return base.OnRollforward();

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

            // Rollforward the base class.
            return base.OnRollforward();
        }

        /*
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
         */

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

        /// <summary>
        /// Checks whether the specified leg corresponds to the last leg in this path.
        /// </summary>
        /// <param name="leg">The leg to check</param>
        /// <returns>True if the leg is the last leg of this path</returns>
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

        /// <summary>
        /// Creates this path using an array of path items.
        /// </summary>
        /// <param name="items">Array of path items.</param>
        /// <remarks>
        /// Note that if the path object is TRANSIENT, no points or arcs will be created
        /// for the path, even if the path items indicate that they should be. -- TODO:
        /// check whether this is still relevant.
        /// </remarks>
        void Create(PathItem[] items)
        {
            // Count the number of legs.
            int numLeg = CountLegs(items);
            if (numLeg==0)
                throw new Exception("PathOperation.Create -- No connection legs");

            m_Legs.Capacity = numLeg;

            // Create each leg.

            int legnum=0;       // Current leg number
            int nexti=0;        // Index of the start of the next leg

            for (int si=0; si<items.Length; si=nexti)
            {
                // Skip if no leg number (could be new units spec).
                if (items[si].Leg==null)
                {
                    nexti = si+1;
                    continue;
                }

                // Confirm the leg count is valid.
                if (legnum+1>numLeg)
                    throw new Exception("PathOperation.Create -- Bad number of path legs.");

                // Create the leg.
                Leg newLeg;
                if (items[si].ItemType == PathItemType.BC)
                    newLeg = CreateCircularLeg(items, si, out nexti);
                else
                    newLeg = CreateStraightLeg(items, si, out nexti);

                // Exit if we failed to create the leg.
                if (newLeg==null)
                    throw new Exception("PathOperation.Create -- Unable to create leg");

                m_Legs.Add(newLeg);
            }

            // Confirm we created the number of legs we expected.
            if (numLeg!=m_Legs.Count)
                throw new Exception("PathOperation.Create -- Unexpected number of legs");
        }

        /// <summary>
        /// Counts the number of legs for this path.
        /// </summary>
        /// <param name="items">Array of path items.</param>
        /// <returns>The number of legs.</returns>
        int CountLegs(PathItem[] items)
        {
            // Each path item contains a leg number, arranged sequentially.
            uint nleg=0;

            foreach (PathItem item in items)
                nleg = Math.Max(nleg, item.Leg);

            return (int)nleg;
        }

        /// <summary>
        /// Creates a circular leg.
        /// </summary>
        /// <param name="items">Array of path items.</param>
        /// <param name="si">Index to the item where the leg data starts.</param>
        /// <param name="nexti">Index of the item where the next leg starts.</param>
        /// <returns>The new leg.</returns>
        Leg CreateCircularLeg(PathItem[] items, int si, out int nexti)
        {
            // Confirm that the first item refers to the BC.
            if (items[si].ItemType != PathItemType.BC)
                throw new Exception("PathOperation.CreateCircularLeg - Not starting at BC");

            // The BC has to be followed by at least 3 items: angle, radius
            // and EC (add an extra 1 to account for 0-based indexing).
        	if (items.Length < si+4)
                throw new Exception("PathOperation.CreateCircularLeg - Insufficient curve data");

            double bangle = 0.0;		// Angle at BC
            double cangle = 0.0;		// Central angle
            double eangle = 0.0;		// Angle at EC
            bool twoangles = false;	    // True if bangle & eangle are both defined.
            bool clockwise = true;		// True if curve is clockwise
            int irad = 0;				// Index of the radius item
            bool cul = false;			// True if cul-de-sac case

            // Point to item following the BC.
            nexti = si+1;
            PathItemType type = items[nexti].ItemType;

            // If the angle following the BC is a central angle
            if (type==PathItemType.CentralAngle)
            {
                // We have a cul-de-sac
                cul = true;

                // Get the central angle.
                cangle = items[nexti].Value;
                nexti++;
            }
            else if (type==PathItemType.BcAngle)
            {
                // Get the entry angle.
                bangle = items[nexti].Value;
                nexti++;

                // Does an exit angle follow?
                if (items[nexti].ItemType == PathItemType.EcAngle)
                {
                    eangle = items[nexti].Value;
                    twoangles = true;
                    nexti++;
                }
            }
            else
            {
                // The field after the BC HAS to be an angle.
                throw new Exception("Angle does not follow BC");
            }

            // Must be followed by radius.
            if (items[nexti].ItemType != PathItemType.Radius)
                throw new Exception("Radius does not follow angle");

            // Get the radius
            Distance radius = items[nexti].GetDistance();
            irad = nexti;
            nexti++;

            // The item after the radius indicates whether the curve is counterclockwise.
            if (items[nexti].ItemType == PathItemType.CounterClockwise)
            {
                nexti++;
                clockwise = false;
            }

            // Get the leg ID.
            uint legnum = items[si].Leg;

            // How many distances have we got?
	        int ndist = 0;
            for (; nexti<items.Length && items[nexti].Leg==legnum; nexti++)
            {
                if (items[nexti].IsDistance)
                    ndist++;
            }

            // Create the leg.
            //CircularLeg leg = new CircularLeg(radius, clockwise, ndist);

            return null;
        }
        /*

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
         */

        /// <summary>
        /// Creates a straight leg.
        /// </summary>
        /// <param name="items">Array of path items.</param>
        /// <param name="si">Index to the item where the leg data starts.</param>
        /// <param name="nexti">Index of the item where the next leg starts.</param>
        /// <returns>The new leg.</returns>
        Leg CreateStraightLeg(PathItem[] items, int si, out int nexti)
        {
            // Get the leg ID.
            uint legnum = items[si].Leg;

            // How many distances have we got?
            int ndist = 0;
            for (nexti=si; nexti<items.Length && items[nexti].Leg==legnum; nexti++)
            {
                if (items[nexti].IsDistance)
                    ndist++;
            }

            // Create the leg.
            StraightLeg leg = new StraightLeg(ndist);

            // Assign each distance.
            ndist = 0;
            for (int i=si; i<nexti; i++)
            {
                Distance d = items[i].GetDistance();
                if (d!=null)
                {
                    // See if there is a qualifier after the distance
                    LegItemFlag qual = LegItemFlag.Null;
                    if ((i+1) < nexti)
                    {
                        PathItemType nexttype = items[i+1].ItemType;

                        if (nexttype==PathItemType.MissConnect)
                            qual = LegItemFlag.MissConnect;

                        if (nexttype==PathItemType.OmitPoint)
                            qual = LegItemFlag.OmitPoint;
                    }

                    leg.SetDistance(d, ndist, qual);
                    ndist++;
                }
                
            }

            // If the first item is an angle, remember it as part of the leg.
            if (items[si].ItemType == PathItemType.Angle)
                leg.StartAngle = items[si].Value;
            else if (items[si].ItemType == PathItemType.Deflection)
                leg.SetDeflection(items[si].Value);

            // Return a reference to the new leg
            return leg;
        }

        /// <summary>
        /// Adjusts the path (Helmert adjustment).
        /// </summary>
        /// <param name="dN">Misclosure in northing.</param>
        /// <param name="dE">Misclosure in easting.</param>
        /// <param name="precision">Precision denominator (zero if no adjustment needed).</param>
        /// <param name="length">Total observed length.</param>
        /// <param name="rotation">The clockwise rotation to apply (in radians).</param>
        /// <param name="sfac">The scaling factor to apply.</param>
        /// <returns>True if adjusted ok.</returns>
        bool Adjust(out double dN, out double dE, out double precision, out double length,
                    out double rotation, out double sfac)
        {
            dN = dE = precision = length = rotation = sfac = 0.0;

            // Initialize position to the start of the path, corresponding to the initial
            // un-adjusted end point.
            IPosition start = m_From;
            IPosition gotend = new Position(m_From);

            // Initial bearing is due north.
            double bearing = 0.0;

            // Go through each leg, updating the end position, and getting
            // the total path length.
            foreach (Leg leg in m_Legs)
            {
                length += leg.Length.Meters;
                leg.Project(ref gotend, ref bearing, sfac);
            }

            // Get the bearing and distance of the end point we ended up with.
            double gotbear = Geom.Bearing(m_From, gotend).Radians;
            double gotdist = Geom.Distance(m_From, gotend);

            // Get the bearing and distance we want.
            double wantbear = Geom.Bearing(m_From, m_To).Radians;
            double wantdist = Geom.Distance(m_From, m_To);

            // Figure out the rotation.
            rotation = wantbear-gotbear;

            // Rotate the end point we got.
            gotend = Geom.Rotate(m_From, gotend, new RadianValue(rotation));

            // Calculate the line scale factor.
            double linefac = CadastralMapModel.Current.CoordinateSystem.GetLineScaleFactor(m_From, gotend);

            // Figure out where the rotated end point ends up when we apply the line scale factor.
            gotend = Geom.Polar(m_From, wantbear, gotdist*linefac);

            // What misclosure do we have?
            dN = gotend.Y - m_To.Y;
            dE = gotend.X - m_To.X;
            double delta = Math.Sqrt(dN*dN + dE*dE);

            // What's the precision denominator (use a value of 0 to denote an exact match).
            if (delta > MathConstants.TINY)
                precision = wantdist/delta;
            else
                precision = 0.0;

            // Figure out the scale factor for the adjustment (use a value of 0 if the start and end
            // points are coincident). The distances here have NOT been adjusted for the line scale factor.
            if (gotdist > MathConstants.TINY)
                sfac = wantdist/gotdist;
            else
                sfac = 0.0;

            return true;
        }

        /// <summary>
        /// Draws this path.
        /// </summary>
        /// <param name="rotation">The rotation to apply (in radians). Clockwise rotations are>0.</param>
        /// <param name="sfac">The scale factor to apply.</param>
        void Draw(double rotation, double sfac)
        {
            // Do nothing if the scale factor is undefined.
            if (Math.Abs(sfac) < MathConstants.TINY)
                return;

            // Initialize position to the start of the path.
            IPosition gotend = new Position(m_From);

            // Initial bearing is whatever the desired rotation is.
            double bearing = rotation;

            // Get each leg to draw itself
            foreach (Leg leg in m_Legs)
                leg.Draw(ref gotend, ref bearing, sfac);

            // Re-draw the terminal points to ensure that their color is on top.
            DrawEnds();
        }

        /// <summary>
        /// Draws a previously saved path.
        /// </summary>
        /// <param name="preview">True if the path should be drawn in preview 
        /// mode (i.e. in the normal construction colour, with miss-connects
        /// shown as dotted lines).</param>
        void Draw(bool preview)
        {
            foreach (Leg leg in m_Legs)
                leg.Draw(preview);
        }

        /// <summary>
        /// Draws the end points for this path.
        /// </summary>
        void DrawEnds()
        {
            ISpatialDisplay display = EditingController.Current.ActiveDisplay;

            if (m_From!=null)
                m_From.Draw(display, Color.DarkBlue);

            if (m_To!=null)
                m_To.Draw(display, Color.LightBlue);
        }
        /*
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
         */

        /// <summary>
        /// Returns adjustment parameters (the bare bones).
        /// </summary>
        /// <param name="rotation">The rotation to apply.</param>
        /// <param name="sfac">The scale factor to apply.</param>
        /// <returns></returns>
        bool GetAdjustment(out double rotation, out double sfac)
        {
            double dN;			// Misclosure in northings
            double dE;			// Misclosure in eastings
            double precision;	// Precision denominator
            double length;		// Total observed length

            return Adjust(out dN, out dE, out precision, out length, out rotation, out sfac);
        }

        /*
//	@mfunc	Handle any intersections created by this operation.
//
//////////////////////////////////////////////////////////////////////

void CePath::Intersect ( void ) {

	for ( UINT2 i=0; i<m_NumLeg; i++ )
		m_pLegs[i]->Intersect(*this);

} // end of Intersect
#endif


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
*/

        /// <summary>
        /// Calculates the precision of the connection path.
        /// </summary>
        /// <returns>The precision</returns>
        uint GetPrecision()
        {
            double de;				// Misclosure in eastings
            double dn;				// Misclosure in northings
            double prec;			// Precision
            double length;			// Total observed length
            double rotation;		// Rotation for adjustment
            double sfac;			// Adjustment scaling factor

            Adjust(out dn, out de, out prec, out length, out rotation, out sfac);

            // If it's REALLY good, return 1 billion.
            if (prec > (double)0xFFFFFFFF)
                return 1000000000;
            else
                return (uint)prec;
        }

        /// <summary>
        /// Returns the array index of a specific leg.
        /// </summary>
        /// <param name="leg">The leg of interest.</param>
        /// <returns>The array index of the leg (-1 if not found).</returns>
        int GetLegIndex(Leg leg)
        {
            return m_Legs.IndexOf(leg);
        }

        /// <summary>
        /// Returns the leg at a specific place along the connection path.
        /// </summary>
        /// <param name="index">The array index of the desired leg.</param>
        /// <returns>The requested leg (null if index out of range).</returns>
        Leg GetLeg(int index)
        {
            if (index<0 || index>=m_Legs.Count)
                return null;
            else
                return m_Legs[index];
        }

        /// <summary>
        /// The total number of legs in this path.
        /// </summary>
        int NumLeg
        {
            get { return m_Legs.Count; }
        }

        /// <summary>
        /// Inserts an extra leg into this connection path.
        /// </summary>
        /// <param name="curLeg">The leg that we already know about.</param>
        /// <param name="newLeg">The extra leg that should follow it.</param>
        /// <returns>True if inserted ok.</returns>
        internal bool InsertLeg(Leg curLeg, Leg newLeg)
        {
            // Find the leg.
            if (curLeg==null || newLeg==null)
                return false;

            int index = GetLegIndex(curLeg);
            if (index<0)
                return false;

            // Do the insert
            m_Legs.Insert(index+1, newLeg);
            return true;
        }

        /// <summary>
        /// The point where the path starts.
        /// </summary>
        internal PointFeature StartPoint
        {
            get { return m_From; }
        }

        /// <summary>
        /// The point where the path ends.
        /// </summary>
        internal PointFeature EndPoint
        {
            get { return m_To; }
        }

        /*
//////////////////////////////////////////////////////////////////////
//
//	@mfunc	Return the very first line that was created along
//			this connection path (if any).
//
//	@rdesc	The first line (null if no lines were created).
//
//////////////////////////////////////////////////////////////////////

private CeArc* CePath::GetFirstArc ( void ) const {

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

private CeArc* CePath::GetLastArc ( void ) const {

	CeArc* pLast = 0;

	for ( INT2 i=(m_NumLeg-1); i>=0 && pLast==0; i-- ) {
		pLast = m_pLegs[i]->GetLastArc();
	}

	return pLast;

} // end of GetLastArc
*/

        /// <summary>
        /// Have any new spans been inserted at the very start of this connection path?
        /// </summary>
        /// <returns>True if insert(s) at the start.</returns>
        bool IsInsertAtStart()
        {
            return m_Legs[0].IsNewSpan(0);
        }

        /// <summary>
        /// Have any new spans been inserted at the very end of this connection path?
        /// </summary>
        /// <returns>True if insert(s) at the end.</returns>
        bool IsInsertAtEnd()
        {
            int endIndex = m_Legs.Count - 1;
            Leg leg = m_Legs[endIndex];
            int nSpan = leg.Count;
            return leg.IsNewSpan(nSpan-1);
        }

        /*
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
