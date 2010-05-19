// <remarks>
// Copyright 2007 - Steve Stanton. This file is part of Backsight
//
// Backsight is free software; you can redistribute it and/or modify it under the terms
// of the GNU Lesser General Public License as published by the Free Software Foundation;
// either version 3 of the License, or (at your option) any later version.
//
// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
// </remarks>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Text;

using Backsight.Geometry;
using Backsight.Environment;
using Backsight.Editor.Observations;
using Backsight.Editor.Xml;

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="24-JAN-1998" was="CePath" />
    /// <summary>
    /// A connection path between two points. Like a traverse.
    /// </summary>
    class PathOperation : Operation, IRecallable, IRevisable
    {
        #region Class data

        /// <summary>
        /// The point where the path starts.
        /// </summary>
        readonly PointFeature m_From;

        /// <summary>
        /// The point where the path ends.
        /// </summary>
        readonly PointFeature m_To;

        /// <summary>
        /// The data entry string that defines the connection path.
        /// </summary>
        readonly string m_EntryString;

        /// <summary>
        /// The legs that make up the path
        /// </summary>
        List<Leg> m_Legs; // readonly

        /// <summary>
        /// The default distance units when this edit was originally executed.
        /// While each <c>Distance</c> observation holds the units, that is
        /// not sufficient to re-create the original data entry string (starting
        /// with something like "ft...").  It's useful to include this in the
        /// data entry string that gets persisted to the database, since the default
        /// units on deserialization may not be the same as the default units that
        /// prevailed when the edit was originally performed.
        /// </summary>
        DistanceUnit m_DefaultEntryUnit;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for use during deserialization
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        /// <param name="t">The serialized version of this instance</param>
        /*
        internal PathOperation(Session s, PathData t)
            : base(s, t)
        {
            CadastralMapModel mapModel = s.MapModel;

            m_From = mapModel.Find<PointFeature>(t.From);
            m_To = mapModel.Find<PointFeature>(t.To);
            m_EntryString = t.EntryString;

            // Create the legs
            LegData[] legs = t.Leg;
            m_Legs = new List<Leg>(legs.Length);
            PointFeature startPoint = m_From;
            IEntity lineType = EnvironmentContainer.FindEntityById(t.LineType);

            for (int i = 0; i < legs.Length; i++)
            {
                Leg leg = t.Leg[i].LoadLeg(this);
                m_Legs.Add(leg);

                // Create features for each span (without any geometry)
                startPoint = leg.CreateSpans(this, t.Leg[i].Span, startPoint, lineType);
            }
        }
        */

        /// <summary>
        /// Initializes a new instance of the <see cref="PathOperation"/> class
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="entryString"></param>
        internal PathOperation(Session s, PointFeature from, PointFeature to, string entryString)
            : base(s)
        {
            m_From = from;
            m_To = to;
            m_EntryString = entryString;
            m_DefaultEntryUnit = EditingController.Current.EntryUnit;
        }

        #endregion

        /// <summary>
        /// Parses the data entry string for this this connection path (with no attached features)
        /// </summary>
        /// <returns>The parsed path (includes a collection of legs that have no attached
        /// features)</returns>
        PathInfo ParsePath()
        {
            PathItem[] items = PathParser.GetPathItems(m_EntryString);
            PathInfo pd = new PathInfo(m_From, m_To);
            pd.Create(items);
            return pd;
        }

        /// <summary>
        /// Creates the features that represent this connection path
        /// </summary>
        /// <returns>The points that were created</returns>
        List<PointFeature> CreateFeatures()
        {
            PathInfo pd = ParsePath();

            // Get the legs (without any attached features)
            m_Legs = new List<Leg>(pd.GetLegs());

            // Get the rotation & scale factor to apply.
            double rotation = pd.RotationInRadians;
            double sfac = pd.ScaleFactor;

            // Go through each leg, adding features as required. This logic
            // is basically the same as the Draw() logic ...

            // Initialize position to the start of the path.
            IPosition gotend = m_From;

            // Initial bearing is whatever the desired rotation is.
            double bearing = rotation;

            // Create a list for holding newly created points
            List<PointFeature> createdPoints = new List<PointFeature>(100);

            // Go through each leg, asking them to make features.
            foreach (Leg leg in m_Legs)
                leg.Save(this, createdPoints, ref gotend, ref bearing, sfac);

            return createdPoints;
        }

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

            // Mark all created features
            Feature[] features = this.Features;
            foreach (Feature f in features)
                f.Undo();

            return true;
        }

        /// <summary>
        /// Rollforward this edit in response to some sort of update.
        /// </summary>
        /// <param name="uc">The context in which editing revisions are being made (not null).
        /// Used to hold a record of any positional changes.</param>
        /// <returns>True if operation has been re-executed successfully</returns>
        internal override bool Rollforward(UpdateContext uc)
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

            if (IsInsertAtEnd())
            {
                throw new NotImplementedException("PathOperation.Rollforward - insert at end");

                //LineFeature last = GetLastLine();
                //if (object.ReferenceEquals(last.EndPoint, m_To))
                //{
                    /*
			        CeMap* pMap = CeMap::GetpMap();
			        CeLocation* pS = pLast->GetpStart();
			        CeLocation* pE = pMap->AddDuplicate(*pEnd);
			        CePoint* pPoint = pMap->AddPoint(pE,0);
			        pPoint->SetpCreator(*this);
			        pPoint->SetNextId();
			        pLast->GetpLine()->ChangeEnds(*pS,*pE);
                    */
                //}
            }

            // Get the rotation & scale factor to apply.
            double rotation, scale;
            GetAdjustment(out rotation, out scale);

            // Notify each leg of the change ...

            // Initialize position to the start of the path.
            IPosition start = m_From;
            IPosition gotend = m_From;
            PointFeature insert = null; // No insert prior to start

            // Initial bearing is whatever the desired rotation is.
            double bearing = rotation;

            // Go through each leg, telling them to "save" (the leg actually
            // checks whether it already refers to features).
            foreach (Leg leg in m_Legs)
            {
                // For the second face of a staggered leg, we need to supply
                // the newly adjusted ends of the leg that has just been
                // processed (the regular version of it's rollforward function
                // does nothing).

                if (leg.FaceNumber == 2)
                {
                    throw new NotImplementedException("PathOperation.Rollforward - extra leg");
                    /*
			        CeExtraLeg* pLeg = dynamic_cast<CeExtraLeg*>(m_pLegs[i]);
			        if ( !pLeg ) {
				        AfxMessageBox("Second face has unexpected data type");
				        return FALSE;
			        }
			        if ( !pLeg->Rollforward(pInsert,*this,start,gotend) ) return FALSE;
                     */
                }
                else
                {
                    start = gotend;
                    if (!leg.Rollforward(uc, ref insert, this, ref gotend, ref bearing, scale))
                        return false;
                }
            }

            // Rollforward the base class.
            return base.OnRollforward();
        }

        /// <summary>
        /// Attempts to locate a superseded (inactive) line that was the parent of
        /// a specific line.
        /// </summary>
        /// <param name="line">The line of interest</param>
        /// <returns>The superseded line that the line of interest was derived from. Null if
        /// this edit did not create the line of interest.</returns>
        internal override LineFeature GetPredecessor(LineFeature line)
        {
            // This edit doesn't supersede anything
            return null;
        }

        /// <summary>
        /// Checks whether the specified leg corresponds to the last leg in this path.
        /// </summary>
        /// <param name="leg">The leg to check</param>
        /// <returns>True if the leg is the last leg of this path</returns>
        internal bool IsLastLeg(Leg leg)
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
        /// Draws a previously saved path.
        /// </summary>
        /// <param name="preview">True if the path should be drawn in preview 
        /// mode (i.e. in the normal construction colour, with miss-connects
        /// shown as dotted lines).</param>
        /*
        void Draw(bool preview)
        {
            foreach (Leg leg in m_Legs)
                leg.Draw(preview);
        }
        */

        /// <summary>
        /// Executes this operation. This attaches persistent features to the path.
        /// </summary>
        internal void Execute()
        {
            // To from and to points MUST be defined.
            if (m_From==null || m_To==null)
                throw new Exception("PathOperation.Execute -- terminal(s) not defined.");

            CreateFeatures();

            // Peform standard completion steps
            Complete();
        }

        /// <summary>
        /// Ensures a point feature exists at a specific location in the map model.
        /// </summary>
        /// <param name="p">The position where a point feature is required</param>
        /// <param name="extraPoints">Any extra points that should be considered. This should be
        /// loaded with any points that have been freshly created by this editing operation (since
        /// these new points will not exist in the map model until the edit commits)</param>
        /// <returns>The point feature at the specified position (may be a new point)</returns>
        internal PointFeature EnsurePointExists(IPosition p, List<PointFeature> extraPoints)
        {
            // First check for the obvious
            if (p is PointFeature)
                return (p as PointFeature);

            // Ensure the supplied position has been rounded to internal resolution
            IPointGeometry pg = PointGeometry.Create(p);

            // First check the list of extra points
            foreach (PointFeature x in extraPoints)
            {
                if (x.IsCoincident(pg))
                    return x;
            }

            // Get the map model to create an extra point if necessary
            PointFeature result = MapModel.EnsurePointExists(pg, this);

            // If the points was freshly created, assign an ID and add to the list of extra points
            if (Object.ReferenceEquals(result.Creator, this))
            {
                if (result.Id==null)
                    result.SetNextId();

                extraPoints.Add(result);
            }

            return result;
        }

        /// <summary>
        /// Returns adjustment parameters (the bare bones).
        /// </summary>
        /// <param name="rotation">The rotation to apply.</param>
        /// <param name="sfac">The scale factor to apply.</param>
        /// <returns></returns>
        bool GetAdjustment(out double rotation, out double sfac)
        {
            PathInfo pd = new PathInfo(this);

            double dN;			// Misclosure in northings
            double dE;			// Misclosure in eastings
            double precision;	// Precision denominator
            double length;		// Total observed length

            return pd.Adjust(out dN, out dE, out precision, out length, out rotation, out sfac);
        }

        /// <summary>
        /// Returns the total number of observed spans for this connection path.
        /// </summary>
        /// <returns>The total number of observed spans for this connection path.</returns>
        int GetCount()
        {
            // Accumulate the number of spans in each leg. Treat cul-de-sacs
            // with no observed spans as a count of 1 (although there is no
            // observation, a feature is still created for it).

            int tot = 0;

            foreach (Leg leg in m_Legs)
            {
                int nspan = Math.Max(1, leg.Count);
                tot += nspan;
            }

            return tot;
        }

        /// <summary>
        /// Gets the observed lengths of each span on this path. In the case of
        /// cul-de-sacs that have no observed span, the distance will be derived
        /// from the central angle and radius.
        /// </summary>
        /// <returns>The distances for this path (with an array length that equals
        /// the value of the <see cref="Count"/> property).</returns>
        Distance[] GetSpans()
        {
            List<Distance> result = new List<Distance>(GetCount());

            foreach (Leg leg in m_Legs)
                leg.GetSpans(result);

            return result.ToArray();
        }
        /*
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
         */

        /// <summary>
        /// Returns the definition of the leg which created a specific feature.
        /// </summary>
        /// <param name="feature">The feature to find.</param>
        /// <returns>The leg that created the feature (or null if the leg
        /// could not be found).</returns>
        Leg GetLeg(Feature feature)
        {
            foreach (Leg leg in m_Legs)
            {
                if (leg.IsCreatorOf(feature))
                    return leg;
            }

            return null;
        }

        /// <summary>
        /// Returns the definition of the straight leg which created a specific feature.
        /// </summary>
        /// <param name="prevStraightToo">Does the leg preceding the one found need to be
        /// straight as well? If you say <c>true</c>, a previous leg <b>must</b> exist.</param>
        /// <param name="feature">The feature to find.</param>
        /// <returns>The leg that created the feature (or null if the leg could
        /// not be found).</returns>
        StraightLeg GetStraightLeg(bool prevStraightToo, Feature feature)
        {
            // Find the leg that created the specified feature.
            int index = -1;
            for (int i = 0; i < m_Legs.Count && index < 0; i++)
            {
                if (m_Legs[i].IsCreatorOf(feature))
                    index = i;
            }

            // Return if it wasn't found.
            if (index < 0)
                return null;

            // If the preceding leg has to be straight, getting the
            // very first leg is no good.
            if (prevStraightToo && index==0)
                return null;

            // The leg HAS to be straight.
            StraightLeg straight = (m_Legs[index] as StraightLeg);
            if (straight == null)
                return null;

            // Check if the preceding leg has to be straight as well.
            if (prevStraightToo)
            {
                StraightLeg prev = (m_Legs[index - 1] as StraightLeg);
                if (prev == null)
                    return null;
            }

            return straight;
        }
        /*
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
        internal uint GetPrecision()
        {
            double de;				// Misclosure in eastings
            double dn;				// Misclosure in northings
            double prec;			// Precision
            double length;			// Total observed length
            double rotation;		// Rotation for adjustment
            double sfac;			// Adjustment scaling factor

            PathInfo pd = new PathInfo(this);
            pd.Adjust(out dn, out de, out prec, out length, out rotation, out sfac);

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
        internal int GetLegIndex(Leg leg)
        {
            return m_Legs.IndexOf(leg);
        }

        /// <summary>
        /// The legs that make up the path
        /// </summary>
        /// <returns></returns>
        internal Leg[] GetLegs()
        {
            return m_Legs.ToArray();
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

        /// <summary>
        /// Returns the very first line that was created along this
        /// connection path (if any).
        /// </summary>
        /// <returns>The first line (null if no lines were created).</returns>
        LineFeature GetFirstLine()
        {
            LineFeature first = null;

            for (int i = 0; i < m_Legs.Count && first == null; i++)
                first = m_Legs[i].GetFirstLine();

            return first;
        }

        /// <summary>
        /// Returns the very last line that was created along this
        /// connection path (if any).
        /// </summary>
        /// <returns>The last line (null if no lines were created).</returns>
        LineFeature GetLastLine()
        {
            LineFeature last = null;

            for (int i = (m_Legs.Count - 1); i >= 0 && last == null; i--)
                last = m_Legs[i].GetLastLine();

            return last;
        }

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

        /// <summary>
        /// Returns a string that represents the definition of
        /// this connection path.
        /// </summary>
        /// <returns>A string defining the complete path</returns>
        /*
        internal string GetString()
        {
            StringBuilder sb = new StringBuilder(m_Legs.Count * 20);
            sb.Append(m_DefaultEntryUnit.Abbreviation);
            sb.Append("...");

            for (int i = 0; i < m_Legs.Count; i++)
            {
                sb.Append(" ");
                string legstr = m_Legs[i].GetDataString(m_DefaultEntryUnit);
                sb.Append(legstr);
            }

            return sb.ToString();
        }
        */

        /// <summary>
        /// The data entry string that defines the connection path.
        /// </summary>
        internal string EntryString
        {
            get { return m_EntryString; }
        }

        /// <summary>
        /// Inserts a second face for a leg in this path.
        /// </summary>
        /// <param name="after">The leg that represents the existing face.</param>
        /// <param name="dists">Distance observations for the 2nd face.</param>
        /// <returns>The new leg.</returns>
        Leg InsertFace(Leg after, Distance[] dists)
        {
            // Get the index of the existing face.
            if (after==null)
                return null;

            int index = GetLegIndex(after);
            if (index < 0)
                return null;

            // Make an extra leg with the specified distances.
            ExtraLeg newLeg = new ExtraLeg(this, after, dists);

            // Create features for the extra leg.
            newLeg.MakeFeatures(this);

            // Insert the new leg into our array of legs.
            InsertLeg(after, newLeg);

            after.FaceNumber = 1;
            newLeg.FaceNumber = 2;

            return newLeg;
        }

        /// <summary>
        /// Gets the positions that define the end points of each leg in
        /// this path. The first position corresponds to the from-point,
        /// while the last position corresponds to the to-point. There
        /// will be one more position than the number of legs in the path
        /// (any instances of <see cref="ExtraLeg"/>  produce 2 coincident
        /// positions in succession).
        /// </summary>
        /// <returns>The positions for each leg in this path</returns>
        IPosition[] GetLegEnds() // was LoadVertexList
        {
            IPosition[] result = new IPosition[m_Legs.Count + 1];

            // Get the rotation & scale factor to apply.
            double rotation;
            double sfac;
            GetAdjustment(out rotation, out sfac);

            // Initialize position to the start of the path.
	        IPosition start = m_From;
            IPosition gotend = start; // Un-adjusted end point

            // Remember the initial position.
            result[0] = start;

            // Initial bearing is whatever the desired rotation is.
            double bearing = rotation;

            // Project each leg, recording the position at each end.
            for (int i=0; i<m_Legs.Count; i++)
            {
                Leg leg = m_Legs[i];
                leg.Project(ref gotend, ref bearing, sfac);
                result[i + 1] = gotend;
            }

            return result;
        }

        /// <summary>
        /// Returns the two end positions for a specific leg in this connection path.
        /// </summary>
        /// <param name="leg">The leg of interest.</param>
        /// <param name="start">The start of the leg.</param>
        /// <param name="end">The end of the leg.</param>
        /// <returns>True if positions defined ok.</returns>
        internal bool GetLegEnds(Leg leg, out IPosition start, out IPosition end)
        {
            start = end = null;

            // Get the array index of the specified leg.
            int index = GetLegIndex(leg);
            if (index < 0)
                return false;

            // Get positions for all legs.
            IPosition[] vlist = GetLegEnds();
            Debug.Assert(vlist.Length == m_Legs.Count + 1);

            // Grab the positions. If the specified leg is actually
            // an extra leg, we actually want the positions for the
            // leg that precedes it.
            if (leg is ExtraLeg)
            {
                index--;
                Debug.Assert(index >= 0);
            }

            start = vlist[index];
            end = vlist[index + 1];

            return true;
        }

        /// <summary>
        /// Performs the data processing associated with this editing operation.
        /// </summary>
        internal override void RunEdit()
        {
            // Get the rotation & scale factor to apply.
            PathInfo pd = ParsePath();
            double rotation = pd.RotationInRadians;
            double sfac = pd.ScaleFactor;

            // Go through each leg, creating the geometry for each span...

            // Initialize position to the start of the path.
            IPosition gotend = m_From;

            // Initial bearing is whatever the desired rotation is.
            double bearing = rotation;

            // Go through each leg, asking them to make features.
            foreach (Leg leg in m_Legs)
                leg.CreateGeometry(ref gotend, ref bearing, sfac);
        }
    }
}
