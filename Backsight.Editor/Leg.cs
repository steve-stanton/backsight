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
using System.Text;
using System.Diagnostics;

using Backsight.Editor.Operations;
using Backsight.Environment;
using Backsight.Geometry;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="21-JAN-1998" was="CeLeg" />
    /// <summary>
    /// A leg in a connection path. This is the base class for <see cref="StraightLeg"/>
    /// and <see cref="CircularLeg"/>.
    /// </summary>
    [Serializable]
    abstract class Leg
    {
        #region Class data

        /// <summary>
        /// The observed distances for this leg. May be empty (for cul-de-sacs
        /// that have no observed spans)
        /// </summary>
        Distance[] m_Distances;

        /// <summary>
        /// The features that correspond to each observed distance. This can
        /// be a <c>LineFeature</c>, <c>PointFeature</c>, or <c>null</c>.
        /// Contains Max(1, m_Distances.Length) elements.
        /// </summary>
        Feature[] m_Creations;

        /// <summary>
        /// Array of switches on each span (one for each observed distance).
        /// Null if there aren't any switches.
        /// </summary>
        LegItemFlag[] m_Switches;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>Leg</c> with everything set to null.
        /// </summary>
        protected Leg()
        {
        }

        protected Leg(int nspan)
        {
            // Allocate an array of feature pointers (always at least ONE).
            // And an array for switches. And distances.
            if (nspan == 0)
            {
                m_Distances = null;
                m_Creations = new Feature[1];
                m_Switches = null;
            }
            else
            {
                m_Distances = new Distance[nspan];
                m_Creations = new Feature[nspan];
                m_Switches = new LegItemFlag[nspan];
            }
        }

        #endregion

        abstract internal Circle Circle { get; }
        abstract internal ILength Length { get; }
        abstract internal IPosition Center { get; }
        abstract internal string DataString { get; }
        abstract internal void Project (ref IPosition pos, ref double bearing, double sfac);
        abstract internal bool Save (PathOperation op, ref IPosition terminal, ref double bearing, double sfac);
        abstract internal bool Rollforward (ref IPointGeometry insert, PathOperation op,
                                                ref IPosition terminal, ref double bearing, double sfac);
        abstract internal bool SaveFace (PathOperation op, ExtraLeg face);
        abstract internal bool RollforwardFace (ref IPointGeometry insert, PathOperation op, ExtraLeg face,
                                                    IPosition spos, IPosition epos);

        internal int Count
        {
            get { return m_Distances.Length; }
        }

        internal bool HasEndPoint(int index)
        {
            if (index >= Count || (m_Switches[index] & LegItemFlag.OmitPoint) != 0)
                return false;
            else
                return true;
        }

        protected bool IsDeflection
        {
            get { return (m_Switches != null && (m_Switches[0] & LegItemFlag.Deflection) != 0); }
        }

        bool IsCurve
        {
            get { return (this.Circle!=null); }
        }

        bool IsStaggered
        {
            get { return (FaceNumber != 0); }
        }

        uint FaceNumber
        {
            get
            {
                if (m_Switches == null)
                    return 0;

                if ((m_Switches[0] & LegItemFlag.Face1) != 0)
                    return 1;

                if ((m_Switches[0] & LegItemFlag.Face2) != 0)
                    return 2;

                return 0;
            }
        }

        /// <summary>
        /// Sets the distance of a specific span in this leg.
        /// </summary>
        /// <param name="distance">The distance to assign.</param>
        /// <param name="index">The index of the distance [0,m_NumSpan-1]</param>
        /// <param name="qualifier"></param>
        /// <returns>True if index was valid.</returns>
        protected bool SetDistance(Distance distance, int index, LegItemFlag qualifier)
        {
            // Return if index is out of range.
            if (index<0 || index>=m_Distances.Length)
                return false;

            // Remember any qualifier.
            if (qualifier != 0)
                m_Switches[index] |= qualifier;

            // Assign the distance
            m_Distances[index] = distance;
            return true;
        }

        /// <summary>
        /// Gets the total observed length of this leg
        /// </summary>
        /// <returns>The sum of the observed lengths for this leg, in meters on the ground</returns>
        protected double GetTotal()
        {
            double total = 0.0;

            foreach(Distance d in m_Distances)
                total += d.Meters;

            return total;
        }

        /// <summary>
        /// Gets the observed distance to the start and end of a specific
        /// span, in meters on the ground.
        /// </summary>
        /// <param name="index">Index of the required span.</param>
        /// <param name="sdist">Distance to the start of the span.</param>
        /// <param name="edist">Distance to the end of the span.</param>
        internal void GetDistances(int index, out double sdist, out double edist)
        {
            // Confirm required index is in range.
            if (index >= m_Distances.Length)
                throw new IndexOutOfRangeException("Leg.GetDistances -- bad index");

            sdist = edist = 0.0;

            // Initialize distance so far.
            double total = 0.0;

            // Accumulate the distance to the required span.
            for (int i = 0; i < index; i++)
            {
                if (i == index)
                    sdist = total;

                total += m_Distances[i].Meters;

                if (i == index)
                    edist = total;
            }
        }

        /// <summary>
        /// Holds on to a reference to a feature that corresponds to a
        /// specific span on this leg.
        /// </summary>
        /// <param name="index">Index of the span.</param>
        /// <param name="feat">The associated feature.</param>
        /// <returns></returns>
        void SetFeature(int index, Feature feat)
        {
            // Confirm the index is valid. For cul-de-sacs with no observed
            // spans, we only have one valid index.
            if (m_Distances.Length>0 && index>=m_Distances.Length)
                throw new IndexOutOfRangeException("Leg.SetFeature - Bad index");

            // The array of feature references should already be defined.
            if (m_Creations==null)
                throw new Exception("Leg.SetFeature - No feature list");

            if (m_Distances.Length == 0)
                m_Creations[0] = feat;
            else
                m_Creations[index] = feat;

        }

        /// <summary>
        /// Returns the feature that corresponds to a specific span on this leg.
        /// </summary>
        /// <param name="index">Index of the span.</param>
        /// <returns>The associated feature (if any).</returns>
        Feature GetFeature(int index)
        {
            // Confirm the index is valid. For cul-de-sacs with no observed
            // spans, we only have one valid index.
            if (m_Distances.Length > 0 && index >= m_Distances.Length)
                throw new IndexOutOfRangeException("Leg.GetFeature - Bad index");


            // The array of feature references should already be defined.
            if (m_Creations == null)
                throw new Exception("Leg.GetFeature - No feature list");

            if (m_Distances.Length == 0)
                return m_Creations[0];
            else
                return m_Creations[index];
        }

        /// <summary>
        /// Handles rollback processing for this leg. This de-activates all
        /// features that were created.
        /// </summary>
        /// <param name="op">The operation that this leg relates to.</param>
        /// <returns></returns>
        internal bool OnRollback(Operation op)
        {
            // Return if there are no creations.
            if (m_Creations==null)
                return true;

            // If there were no observed spans, we must be dealing with
            // a cul-de-sac, in which case we should know about ONE feature.
            int nspan = Math.Max(1, m_Distances.Length);

            // Go through each span, de-activating every feature that
            // was created by the specified operation.
            for (int i = 0; i < nspan; i++)
            {
                // What sort of feature (if any) represents the span. If
                // it's null, just skip the span. If it's a point created
                // by the specified op, de-activate the point. If it's a
                // line created by the specified op, de-activate the line,
                // as well as the two end points (so long as they were
                // created by the specified op).

                Feature feat = m_Creations[i];
                if (feat != null)
                    throw new NotImplementedException();
                    //feat.OnRollback(op);
            }

            return true;
        }

        /// <summary>
        /// Handles any intersections created along this leg.
        /// </summary>
        /// <param name="op">The operation that created the leg.</param>
        void Intersect(Operation op)
        {
            // Return if there are no creations.
            if (m_Creations==null)
                return;

            // If there were no observed spans, we must be dealing with
            // a cul-de-sac, in which case we should know about ONE feature
            // (unless it was a mis-connect).
            int nspan = Math.Max(1, m_Distances.Length);

            // Go through each span, intersecting every feature that
            // was created.
            for (int i = 0; i < nspan; i++)
            {
                Feature feat = m_Creations[i];
                if (feat != null)
                    feat.IsMoved = true;
            }
        }

        /// <summary>
        /// Tries to find the observed length for a feature that may have been
        /// created on this leg. Note that for lines representing cul-de-sacs
        /// with no observed spans, there is no observed length (the length
        /// in that case comes from the central angle & radius).
        /// </summary>
        /// <param name="feat">The feature to find.</param>
        /// <returns>Reference to the distance (null if not found).</returns>
        internal Distance GetDistance(Feature feat)
        {
            Distance dist = null;

            for (int i = 0; i < m_Distances.Length && dist == null; i++)
            {
                if (Object.ReferenceEquals(m_Creations[i], feat))
                    dist = m_Distances[i];
            }

            return dist;
        }

        /// <summary>
        /// Loads a list with references to all the features that exist on
        /// this leg. This function is called by <see cref="PathOperation.GetFeatures"/>
        /// </summary>
        /// <param name="features">The list to load.</param>
        /*
        void GetFeatures(List<Feature> features)
        {
            features.AddRange(m_Creations);
        }
        */

        /// <summary>
        /// Loads a list of the features that were created by this operation.
        /// </summary>
        /// <param name="op">The operation that this leg relates to.</param>
        /// <param name="flist">The list to store the results. This list will be
        /// appended to, so you may want to clear the list prior to call.</param>
        internal void GetFeatures(Operation op, List<Feature> flist)
        {
            // Append the features representing this leg.
            int nspan = Math.Max(1, m_Distances.Length);

            for (int i = 0; i < nspan; i++)
            {
                if (m_Creations[i] != null)
                {
                    // Append to the return list.
                    flist.Add(m_Creations[i]);

                    // If the feature is a line, also process any point
                    // feature at the end. If it's a circular arc, add
                    // the circle centre point if it isn't already in
                    // the list.
                    LineFeature line = (m_Creations[i] as LineFeature);
                    if (line!=null)
                    {
        				// Include inactive points.
                        PointFeature point = line.EndPoint;
                        if (Object.ReferenceEquals(point.Creator, op))
                            flist.Add(point);

                        if (line is ArcFeature)
                        {
                            ArcFeature arc = (line as ArcFeature);
                            point = arc.Circle.CenterPoint;
                            if (Object.ReferenceEquals(point.Creator, op) && !flist.Contains(point))
                                flist.Add(point);

                        }
                    }
                }
            }
        }

        /// <summary>
        /// Loads a list of distances with the observed lengths of each span on
        /// this leg. In the case of cul-de-sacs that have no observed span,
        /// the distance will be derived from the central angle and radius.
        /// This function is called by <see cref="PathOperation.GetSpans"/>.
        /// </summary>
        /// <param name="distances">The list of Distance objects to load.</param>
        void GetSpans(List<Distance> distances)
        {
            if (m_Distances.Length == 0)
            {
                // If this is a cul-de-sac that had no observed spans, get the
                // circular leg to define the distance (in meters on the ground).
                DistanceUnit mUnit = CadastralMapModel.Current.GetUnits(DistanceUnitType.Meters);
                distances.Add(new Distance(this.Length.Meters, mUnit));
            }
            else
            {
                // Otherwise copy over the distances as they were entered.
                distances.AddRange(m_Distances);
            }
        }

        /// <summary>
        /// Checks whether this leg is the leg that caused the creation of
        /// a specific feature.
        /// </summary>
        /// <param name="feature">The feature to search for. The creator of this feature
        /// is expected to match the operation that contains this leg. Note that if this
        /// feature does not have a creating op, a return value of <c>false</c> will
        /// always be returned (this assumes that a leg must be part of SOME operation).
        /// </param>
        /// <returns>True if this leg caused the creation of the feature.</returns>
        bool IsCreatorOf(Feature feature)
        {
            // NOTE: the logic here should be similar to that in Leg.GetFeatures ...

            // What operation does this leg belong to?
            Operation op = feature.Creator;
            if (op == null)
                return false;

            // If the feature is inactive, we'll want CePrimitive::GetpPoint()
            // to also search for inactive points.
            //LOGICAL onlyActive = feature.IsActive();

            int nspan = Math.Max(1, m_Distances.Length);

            for (int i=0; i<nspan; i++)
            {
                // Skip if this was a null span.
                if (m_Creations[i]==null)
                    continue;

                // Return if we have a match.
                if (Object.ReferenceEquals(m_Creations[i], feature))
                    return true;

                // If the feature is a line, also check any point feature at the end.
                LineFeature line = (m_Creations[i] as LineFeature);
                if (line==null)
                    continue;

                //const CePoint* pPoint = pArc->GetpEnd()->GetpPoint(*pop, onlyActive);
                PointFeature point = line.EndPoint;
                if (Object.ReferenceEquals(point, feature))
                    return true;

                // If it's a circular arc, check the circle center point too.
                ArcFeature arc = (line as ArcFeature);
                if (arc!=null && Object.ReferenceEquals(arc.Circle.CenterPoint, feature))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the point feature that is at the end of this leg.
        /// </summary>
        /// <param name="op">The operation that is expected to have created the end point.</param>
        /// <returns>The point object at the end (could conceivably be null).</returns>
        PointFeature GetEndPoint(Operation op)
        {
            if (m_Creations==null)
                return null;

            // If the very last feature for this leg is a point, that's the thing we want.
            int nspan = Math.Max(1, m_Distances.Length);
            Feature feat = m_Creations[nspan-1];
            PointFeature point = (feat as PointFeature);
            if (point!=null)
                return point;

            // Otherwise the last feature should be a line object, so
            // we want IT'S end point (either active or inactive).
            LineFeature line = (feat as LineFeature);
            point = line.EndPoint;
            if (Object.ReferenceEquals(point.Creator, op))
                return point;

            return null;
        }

        /// <summary>
        /// Returns the point feature (if any) that is at the start of this leg.
        /// </summary>
        /// <param name="op">The operation that is expected to have created the start point.</param>
        /// <returns>The point object at the start (may be null).</returns>
        PointFeature GetStartPoint(Operation op)
        {
            if (m_Creations==null)
                return null;

            // If the first feature for this leg is a point, that's the thing we want.
            Feature feat = m_Creations[0];
            PointFeature point = (feat as PointFeature);
            if (point!=null)
                return point;

            // Otherwise the first feature should be a CeArc object, so
            // we want IT'S start point (either active or inactive).
            LineFeature line = (feat as LineFeature);
            point = line.StartPoint;
            if (Object.ReferenceEquals(point.Creator, op))
                return point;

            return null;
        }

        /// <summary>
        /// Returns the point feature (if any) that is at the center of the circle that
        /// this leg lies on (does nothing for straight legs).
        /// </summary>
        /// <param name="op">The operation that is expected to have created the center point.</param>
        /// <returns>The point object at the centre (may be null).</returns>
        PointFeature GetCenterPoint(Operation op)
        {
            // Get the circle (if any) that this leg lies on.
            Circle circle = this.Circle;
            if (circle==null)
                return null;

            // Ask the circle to return the point (inactive center points are ok).
            PointFeature point = circle.CenterPoint;
            if (Object.ReferenceEquals(point.Creator, op))
                return point;

            return null;
        }

        /// <summary>
        /// Marks this leg as having a deflection angle at the start. This applies only to
        /// straight legs. There must be a preceding leg for this to make any sense.
        /// </summary>
        /// <param name="set">Mark as deflection? Default=true.</param>
        protected void SetDeflection(bool set)
        {
            // Return if there are no observed spans.
            if (m_Switches==null)
                return;

            if (set)
                m_Switches[0] |= LegItemFlag.Deflection;
            else
                m_Switches[0] &= (~LegItemFlag.Deflection);
        }

        /// <summary>
        /// Checks if this leg will generate a line feature.
        /// </summary>
        /// <param name="index">The index of the span in question.</param>
        /// <returns>True if line feature will be produced.</returns>
        internal bool HasLine(int index)
        {
            // No feature if the span index is out of range.
            if (index >= m_Distances.Length)
                return false;

            LegItemFlag swt = m_Switches[index];
            if ((swt & LegItemFlag.MissConnect)!=0 || (swt & LegItemFlag.OmitPoint)!=0)
                return false;
            else
                return true;
        }
        /*
//	@mfunc	Create a transient CeMiscText object that represents
//			an observed angle.
//
//			The caller is responsible for deleting the text
//			objects once done with them.
//
//	@parm	Backsight for the angle.
//	@parm	Position that the angle relates to.
//	@parm	Foresight for the angle.
//	@parm	The observed angle (negated if counter-clockwise)
//	@parm	List to hold a pointer to the created text.

void CeLeg::MakeText ( const CeVertex& bs
					 , const CeVertex& from
					 , const CeVertex& fs
					 , const FLOAT8 sangle
					 , CPtrList& text ) const {

	// Get the bearing on the bisector of the angle.
	CeTurn ref(from,bs);
	FLOAT8 cwang = ref.GetAngle(fs);
	FLOAT8 bearing = ref.GetBearing(cwang*0.5);

	// Make sure the bearing is in the right hemisphere!
	FLOAT8 angle = fabs(sangle);
	if ( (angle>PI && cwang<PI) || (angle<PI && cwang>PI) ) {
		bearing += PI;
		if ( bearing>PIMUL2 ) bearing -= PIMUL2;
	}

	// Get the height of the text, in metres on the ground.
	CeMap* pMap = CeMap::GetpMap();
	FLOAT8 grheight = pMap->GetLineAnnoHeight();

	// We will offset by the height, or half the point size
	// (whichever is bigger).
	FLOAT8 offset = grheight;
	FLOAT8 poff = pMap->GetPointHeight() * 0.6; // 10% extra
	offset = max(poff,grheight);

	// Project to the position that passes through the
	// middle of the text.
	CeVertex pos(from,bearing,offset);

	// Create text object.
	CeMiscText* pText = new CeMiscText(pos,RadStr(angle));

	// And define the height and clockwise rotation from horizontal.
	pText->SetHeight(FLOAT4(grheight));

	if ( bearing>0.0 && bearing<=PI ) {	// i.e. angle to the east
		// Assumes TA_LEFT justification.
		pText->SetRotation(FLOAT4(bearing+PIMUL1P5));
	}
	else {
		// Assumes TA_RIGHT justification.
		pText->SetRotation(FLOAT4(bearing-PIMUL1P5));
	}

	// Remember the text that has been created.
	text.AddTail(pText);

} // end of MakeText
         */

        /*
//	@mfunc	Load a multi-line edit box with the observed distances
//			(if any) for this leg. Each item in the list is
//			associated with the address of the corresponding
//			CeDistance object.
//
//	@rdesc	The number of distances that were listed.

         INT4 CeLeg::ListDistances ( CListBox* pList ) const {

	pList->ResetContent();

	if ( m_NumSpan==0 ) {
		pList->AddString("see central angle");
		return 0;
	}

	for ( UINT2 i=0; i<m_NumSpan; i++ ) {
		INT4 index = pList->AddString((LPCTSTR)m_Distances[i].Format());
		pList->SetItemDataPtr(index,&m_Distances[i]);

	}

	return (INT4)m_NumSpan;

} // end of ListDistances
        */

        /// <summary>
        /// Inserts an extra distance into this leg.
        /// </summary>
        /// <param name="newdist">The new (transient) distance to insert.</param>
        /// <param name="curdist">A distance that this leg already knows about.</param>
        /// <param name="isBefore">Should the new distance go before the existing one?</param>
        /// <param name="wantLine">Should a new line be created (it won't happen until rollforward
        /// occurs, but it will get marked to happen).</param>
        /// <returns>The index where the extra distance was saved.</returns>
        int Insert(Distance newdist, Distance curdist, bool isBefore, bool wantLine)
        {
            // Get the index of the currently defined distance.
            int index = GetIndex(curdist);
            if (index<0)
                return -1;

            if (isBefore)
            {
                InsertAt(index, newdist, wantLine);
                return index;
            }
            else
            {
                InsertAt(index+1, newdist, wantLine);
                return index+1;
            }
        }

        /// <summary>
        /// Inserts a new distance into this leg.
        /// </summary>
        /// <param name="index">The index where the new distance should go.</param>
        /// <param name="newdist">The distance to insert.</param>
        /// <param name="wantLine">Should a new line be created (it won't happen until rollforward
        /// occurs, but it will get marked to happen).</param>
        void InsertAt(int index, Distance newdist, bool wantLine)
        {
            // Expand the array of distances, as well as the arrays
            // for creations and switches.
            int numSpan = m_Distances.Length;
            Distance[] newd = new Distance[numSpan+1];
            Feature[] newf = new Feature[numSpan+1];
            LegItemFlag[] news = new LegItemFlag[numSpan+1];

            // Copy over stuff prior to the new distance
            for (int i=0; i<index; i++)
            {
                newd[i] = m_Distances[i];
                newf[i] = m_Creations[i];
                news[i] = m_Switches[i];
            }

            // Stick in the new guy with miss-connect flag
            newd[index] = newdist;
            newf[index] = null;
            news[index] = LegItemFlag.MissConnect;

            // If a line is required, flag it for creation when rollforward runs (we can't do
            // it right now, since the end positions are currently coincident).
            if (wantLine)
                news[index] |= LegItemFlag.NewLine;

            // Copy over the rest.
            for (int i=index; i<numSpan; i++)
            {
                newd[i+1] = m_Distances[i];
                newf[i+1] = m_Creations[i];
                news[i+1] = m_Switches[i];
            }

            // Replace original arrays with the new ones
            m_Distances = newd;
            m_Creations = newf;
            m_Switches = news;

            // If we inserted at the very start, ensure that any
            // deflection angle switch is still with the very first span.
            if (index==0 && (m_Switches[1] & LegItemFlag.Deflection)!=0)
            {
                m_Switches[0] |= LegItemFlag.Deflection;
                m_Switches[1] &= (~LegItemFlag.Deflection);
            }
        }

        /// <summary>
        /// Returns the index of a specific distance along this leg.
        /// </summary>
        /// <param name="dist">The distance to look for.</param>
        /// <returns>The index of the distance (-1 if not found).</returns>
        int GetIndex(Distance dist)
        {
            for (int i=0; i<m_Distances.Length; i++)
            {
                if (Object.ReferenceEquals(m_Distances[i], dist))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Returns the index of a feature along this leg.
        /// </summary>
        /// <param name="feat">The feature to look for.</param>
        /// <returns>The index of the feature (-1 if not found).</returns>
        int GetIndex(Feature feat)
        {
            for (int i=0; i<m_Distances.Length; i++)
            {
                if (Object.ReferenceEquals(m_Creations[i], feat))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// The number of spans in this leg is the number of elements in the
        /// <see cref="m_Distances"/> array.
        /// </summary>
        int NumSpan
        {
            get { return m_Distances.Length; }
        }

        /// <summary>
        /// Breaks this leg into two legs. The break must leave at least one distance
        /// in each of the resultant legs.
        /// </summary>
        /// <param name="index">The index of the span that should be at the start of the extra leg.
        /// Should be greater than zero.</param>
        /// <param name="to">The leg to move stuff to. Must have enough space to hold the extra stuff.</param>
        /// <returns>True if moved ok.</returns>
        bool MoveEndLeg(int index, Leg to)
        {
            // Destination MUST have exactly the right size.
            if (to.NumSpan != (this.NumSpan-index))
                return false;

            // Transfer stuff to the new leg.
            for (int iSrc=index, iDst=0; iSrc<NumSpan; iSrc++, iDst++)
            {
                to.m_Distances[iDst] = m_Distances[iSrc];
                to.m_Creations[iDst] = m_Creations[iSrc];
                to.m_Switches[iDst] = m_Switches[iSrc];
            }

            // Check for special case where the entire leg has been
            // copied (this isn't really expected to happen).
            if (index==0)
            {
                m_Distances = null;
                m_Creations = null;
                m_Switches = null;
                return true;
            }

            int numSpan = index;
            Distance[] newd = new Distance[numSpan];
            Feature[] newf = new Feature[numSpan];
            LegItemFlag[] news = new LegItemFlag[numSpan];

            // Copy over the initial stuff.
            for (int i=0; i<numSpan; i++)
            {
                newd[i] = m_Distances[i];
                newf[i] = m_Creations[i];
                news[i] = m_Switches[i];
            }

            // Replace the original arrays with the new ones.
            m_Distances = newd;
            m_Creations = newf;
            m_Switches = news;

            return true;
        }

        /// <summary>
        /// Checks whether a specific span is new.
        /// </summary>
        /// <param name="index">The index of the span of interest.</param>
        /// <returns>True if new span.</returns>
        internal bool IsNewSpan(int index)
        {
            if (NumSpan==0)
                return false;

            return ((m_Switches[index] & LegItemFlag.NewLine)!=0);
        }

        /// <summary>
        /// Remembers the line created for a new span.
        /// </summary>
        /// <param name="index">The index of the span of interest.</param>
        /// <param name="newspan">The line to refer to.</param>
        void AddNewSpan(int index, LineFeature newspan)
        {
            Debug.Assert(IsNewSpan(index));

            // Point to the new line, and clear the flags that denote a new span.
            SetFeature(index, newspan);
            m_Switches[index] &= (~LegItemFlag.NewLine);
            m_Switches[index] &= (~LegItemFlag.MissConnect);
        }

        /// <summary>
        /// Returns the very first line that was created along this leg (if any).
        /// </summary>
        /// <returns>The first line (null if no lines were created).</returns>
        LineFeature GetFirstLine()
        {
            int nSpan = Math.Max(1, NumSpan);

            for (int i=0; i<nSpan; i++)
            {
                LineFeature line = (m_Creations[i] as LineFeature);
                if (line!=null)
                    return line;
            }

            return null;
        }

        /// <summary>
        /// Returns the very last line that was created along this leg (if any).
        /// </summary>
        /// <returns>The last line (null if no lines were created).</returns>
        LineFeature GetLastLine()
        {
            int nSpan = Math.Max(1, NumSpan);

            for (int i=(nSpan-1); i>=0; i--)
            {
                LineFeature line = (m_Creations[i] as LineFeature);
                if (line!=null)
                    return line;
            }

            return null;
        }

        /// <summary>
        /// Appends observations to a string that represents this leg.
        /// </summary>
        /// <param name="str">The string buffer to append to.</param>
        protected void AddToString(StringBuilder str)
        {
            // Return if there are no observed spans.
            if (NumSpan==0)
                return;

            // Format each distance.
            for (int i=0; i<NumSpan; i++)
            {
                str.Append(m_Distances[i].Format());
                str.Append(" ");

                if ((m_Switches[i] & LegItemFlag.MissConnect)!=0)
                    str.Append("/- ");

                if ((m_Switches[i] & LegItemFlag.OmitPoint)!=0)
                    str.Append("/* ");
            }
        }

        /// <summary>
        /// Records that this leg is one of two legs that make up a staggered leg.
        /// </summary>
        /// <param name="face">The face number (1 or 2).</param>
        void SetStaggered(int face)
        {
            if (face==1)
                m_Switches[0] |= LegItemFlag.Face1;
            else if (face==2)
                m_Switches[0] |= LegItemFlag.Face2;
        }

        /// <summary>
        /// Creates a set of line segments (and points) for this leg. This function is called
        /// only when adding features to an <see cref="ExtraLeg"/>. THIS leg needs to be the
        /// second face of a pair of legs.
        /// </summary>
        /// <param name="op">The connection path that this leg belongs to.</param>
        /// <param name="spos">The position at the start of the leg.</param>
        /// <param name="epos">The position at the end of the leg.</param>
        /// <returns>True if created ok.</returns>
        bool MakeSegments(PathOperation op, IPosition spos, IPosition epos)
        {
            Debug.Assert(NumSpan>0);
            if (NumSpan==0)
                return false;

            // Get the desired length. Construct a transient geometry, to ensure the length
            // takes account of any coordinate roundoff.
            double len = new LineSegmentGeometry(spos, epos).Length.Meters;

            // Get the observed length.
            double obs = this.Length.Meters;

            // Get the adjustment factor for stretching-compressing the observed distances.
            double factor = len/obs;

            // Get the bearing of the line.
            double bearing = Geom.Bearing(spos, epos).Radians;

            // Define start of first segment.
            IPosition start = spos;
            IPosition end;

            // Haven't got anywhere yet.
            double totobs = 0.0;
            CadastralMapModel map = CadastralMapModel.Current;

	        // Add non-topological arcs for each observed distance.
            for (int i=0; i<NumSpan; i++, start=end)
            {
                // Update the observed length.
                totobs += m_Distances[i].Meters;

                // Apply factor to get us to the end of the leg.
                double elen = totobs * factor;

                // Define the end position.
                end = Geom.Polar(spos, bearing, elen);

                // The point at the start of the span should already be there
                PointFeature startPoint = map.EnsurePointExists(start, op);

                // Add a point at the end of the span (so long as we're not at the
                // end of the leg). Duplicates are made if there is already a point
                // there -- this simplifies the implementation of UpdateSegments.
                // If we're at the end of the leg, a point should probably be
                // there already.
                PointFeature endPoint;
                if (i<(NumSpan-1))
                    endPoint = map.AddPoint(end, map.DefaultPointType, op);
                else
                    endPoint = map.EnsurePointExists(end, op);

                // If the point at the end of the span was created by the operation, and
                // an ID has not already been assigned, assign the next available ID
                if (Object.ReferenceEquals(endPoint.Creator, op) && endPoint.Id==null)
                    endPoint.SetNextId();                

                // Add non-topological line to the map.
                IEntity blank = EnvironmentContainer.FindBlankEntity();
                LineFeature line = map.AddLine(startPoint, endPoint, blank, op);

                // The line should have been added as non-topological (since the blank entity type
                // is supposed to be non-topological), but make it explicit here. And mark the line
                // as "void" so that it can be skipped on export to AutoCad.
                line.SetTopology(false); // should be false already, 
                line.IsVoid = true;

                // And remember the line!
                Debug.Assert(m_Creations!=null);
                Debug.Assert(m_Creations[i]==null);
                m_Creations[i] = line;
            }

            return true;
        }

        /// <summary>
        /// Updates a set of line segments (and points) for this leg. This function is called only when
        /// rolling forward an <see cref="ExtraLeg"/>. THIS leg needs to be the second face of a pair
        /// of legs.
        /// </summary>
        /// <param name="insert"></param>
        /// <param name="op">The connection path that this leg belongs to.</param>
        /// <param name="spos">The position at the start of the leg.</param>
        /// <param name="epos">The position at the end of the leg.</param>
        /// <returns>True if updated ok.</returns>
        bool UpdateSegments(IPointGeometry insert, PathOperation op, IPosition spos, IPosition epos)
        {
            Debug.Assert(NumSpan>0);
            if (NumSpan==0)
                return false;

            // Get the desired length. Construct a transient geometry, to ensure the length
            // takes account of any coordinate roundoff.
            double len = new LineSegmentGeometry(spos, epos).Length.Meters;

            // Get the observed length.
            double obs = this.Length.Meters;

            // Get the adjustment factor for stretching-compressing the observed distances.
            double factor = len/obs;

            // Get the bearing of the line.
            double bearing = Geom.Bearing(spos, epos).Radians;

            // Haven't got anywhere yet.
            double totobs = 0.0;

            // Go through each feature (which correspond to lines), shifting the location at
            // the end of each one. Except the last one, which is the start of the last leg.
            // The actual terminals should get moved when the adjacent legs are processed.

            for (int i=0; i<(NumSpan-1); i++)
            {
                // Update the observed length.
                totobs += m_Distances[i].Meters;

                // Apply factor to get us to the end of the leg.
                double elen = totobs * factor;

                // Define the position of the point at the end of the span.
                IPosition pos = Geom.Polar(spos, bearing, elen);

                // Get the line feature that was added and move
                // the location at the end of it.
                LineFeature line = (m_Creations[i] as LineFeature);
                Debug.Assert(line!=null);
                line.EndPoint.Move(pos);
            }

            return true;
        }

        /// <summary>
        /// Creates a set of circular arcs (and points) for this leg. This function is called only when
        /// adding features to an <see cref="ExtraLeg"/>. THIS leg needs to be the second face of a
        /// pair of legs.
        /// </summary>
        /// <param name="op">The connection path that this leg belongs to.</param>
        /// <param name="spos">The position at the start of the leg.</param>
        /// <param name="epos">The position at the end of the leg.</param>
        /// <param name="circle">The circle the arcs should be related to (updated to refer to
        /// the new arcs).</param>
        /// <param name="iscw">Should the arcs be directed clockwise?</param>
        /// <returns>True if created ok.</returns>
        bool MakeCurves(PathOperation op, IPosition spos, IPosition epos, Circle circle, bool iscw)
        {
            Debug.Assert(NumSpan>0);
            if (NumSpan==0)
                return false;

            // Get the desired length. Construct a transient geometry, to ensure the length
            // takes account of any coordinate roundoff.
            CircularArcGeometry curve = new CircularArcGeometry(circle, spos, epos, iscw);
            double len = curve.Length.Meters;

            // Get the observed length.
            double obs = this.Length.Meters;

            // Get the adjustment factor for stretching-compressing the observed distances.
            double factor = len/obs;

            // Define start of first arc.
            IPosition start = curve.BC;
            IPosition end;

            // Haven't got anywhere yet.
            double totobs = 0.0;
            CadastralMapModel map = CadastralMapModel.Current;

	        // Add non-topological arcs for each observed distance.
            for (int i=0; i<NumSpan; i++, start=end)
            {
                // Update the observed length.
                totobs += m_Distances[i].Meters;

                // Apply factor to get us to the end of the leg.
                double elen = totobs * factor;

                // Define the end position.
                bool isEndDefined = curve.GetPosition(new Length(elen), out end);
                Debug.Assert(isEndDefined);

                // The point at the start of the span should already be there
                PointFeature startPoint = map.EnsurePointExists(start, op);

                // Add a point at the end of the span (so long as we're not at the
                // end of the leg). Duplicates are made if there is already a point
                // there -- this simplifies the implementation of UpdateCurves.
                // If we're at the end of the leg, a point should probably be
                // there already.
                PointFeature endPoint;
                if (i<(NumSpan-1))
                    endPoint = map.AddPoint(end, map.DefaultPointType, op);
                else
                    endPoint = map.EnsurePointExists(end, op);

                // If the point at the end of the span was created by the operation, and
                // an ID has not already been assigned, assign the next available ID
                if (Object.ReferenceEquals(endPoint.Creator, op) && endPoint.Id==null)
                    endPoint.SetNextId();

                // Add non-topological line to the map.
                IEntity blank = EnvironmentContainer.FindBlankEntity();
                LineFeature line = map.AddCircularArc(circle, startPoint, endPoint, iscw, blank, op);

                // The line should have been added as non-topological (since the blank entity type
                // is supposed to be non-topological), but make it explicit here. And mark the line
                // as "void" so that it can be skipped on export to AutoCad.
                line.SetTopology(false); // should be false already, 
                line.IsVoid = true;

                // And remember the line!
                Debug.Assert(m_Creations!=null);
                Debug.Assert(m_Creations[i]==null);
                m_Creations[i] = line;
            }

            return true;
        }

        /// <summary>
        /// Updates a set of arcs (and points) for this leg. This function is called only when
        /// rolling forward an <see cref="ExtraLeg"/>. THIS leg needs to be the second face of
        /// a pair of legs.
        /// </summary>
        /// <param name="insert"></param>
        /// <param name="op">The connection path that this leg belongs to.</param>
        /// <param name="spos">The position at the start of the leg.</param>
        /// <param name="epos">The position at the end of the leg.</param>
        /// <param name="circle">The circle the curves should be related to (not necessarily the
        /// same one that the curves were previously related to).</param>
        /// <param name="iscw">Should the curves be directed clockwise?</param>
        /// <returns>True if updated ok.</returns>
        bool UpdateCurves(IPointGeometry insert, PathOperation op, IPosition spos, IPosition epos,
                            Circle circle, bool iscw)
        {
            Debug.Assert(NumSpan>0);
            if (NumSpan==0)
                return false;

            // Get the desired length. Construct a transient geometry, to ensure the length
            // takes account of any coordinate roundoff.
            CircularArcGeometry curve = new CircularArcGeometry(circle, spos, epos, iscw);
            double len = curve.Length.Meters;

            // Get the observed length.
            double obs = this.Length.Meters;

            // Get the adjustment factor for stretching-compressing the observed distances.
            double factor = len/obs;

            // Define start of first arc.
            IPosition start = curve.BC;
            IPosition end;

            // Haven't got anywhere yet.
            double totobs = 0.0;
            CadastralMapModel map = CadastralMapModel.Current;

            for (int i=0; i<NumSpan; i++, start=end)
            {
                // Update the observed length.
                totobs += m_Distances[i].Meters;

                // Apply factor to get us to the end of the leg.
                double elen = totobs * factor;

                // Define the end position.
                bool isEndDefined = curve.GetPosition(new Length(elen), out end);
                Debug.Assert(isEndDefined);

                // Get the line feature that was added.
                ArcFeature arc = (m_Creations[i] as ArcFeature);
                Debug.Assert(arc!=null);

                // If the arc is now on a different circle, change it.
                arc.Move(circle, iscw);

                // Move the location at the end of the arc (so long
                // as it's not the very last location).
                if (i<(NumSpan-1))
                    arc.EndPoint.Move(end);
            }

            return true;
        }
    }
}
