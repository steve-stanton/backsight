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
using System.Diagnostics;

using Backsight.Editor.Operations;
using Backsight.Environment;
using Backsight.Geometry;
using Backsight.Editor.Observations;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="21-JAN-1998" was="CeLeg" />
    /// <summary>
    /// A leg in a connection path. This is the base class for <see cref="StraightLeg"/>
    /// and <see cref="CircularLeg"/>.
    /// </summary>
    abstract class Leg
    {
        #region Class data

        /// <summary>
        /// The data that defines each span on this leg (should always contain at least
        /// one element).
        /// </summary>
        SpanInfo[] m_Spans;

        /// <summary>
        /// The face number of this leg (if this leg is staggered). In the range [0,2]. A value
        /// of zero means the leg is not staggered.
        /// </summary>
        byte m_FaceNumber;

        #endregion

        #region Constructors

        protected Leg(int nspan)
        {
            // Allocate an array of spans (always at least ONE).
            m_Spans = new SpanInfo[Math.Max(1, nspan)];
            for (int i = 0; i < m_Spans.Length; i++)
                m_Spans[i] = new SpanInfo();

            m_FaceNumber = 0;
        }

        #endregion

        abstract public Circle Circle { get; }
        abstract public ILength Length { get; }
        abstract internal IPosition Center { get; }
        abstract public void Project (ref IPosition pos, ref double bearing, double sfac);

        /// <summary>
        /// Draws this leg
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="pos">The position for the start of the leg. Updated to be
        /// the position for the end of the leg.</param>
        /// <param name="bearing">The bearing at the end of the previous leg. Updated
        /// for this leg.</param>
        /// <param name="sfac">Scale factor to apply to distances.</param>
        abstract public void Render(ISpatialDisplay display, ref IPosition pos, ref double bearing, double sfac);

        abstract internal void Draw (bool preview);

        //[Obsolete]
        //abstract internal void Save(FeatureFactory ff, List<PointFeature> createdPoints, ref IPosition terminal,
        //                                ref double bearing, double sfac);

        /// <summary>
        /// Creates spatial features (points and lines) for this leg. The created
        /// features don't have any geometry.
        /// </summary>
        /// <param name="ff">The factory for creating new spatial features</param>
        /// <param name="maxSequence">The highest sequence number assigned to features
        /// preceding this leg</param>
        /// <param name="startPoint">The point (if any) at the start of this leg. May be
        /// null in a situation where the preceding leg ended with an "omit point" directive.</param>
        /// <param name="lastPoint">The point that should be used for the very end
        /// of the leg (specify null if a point should be created at the end of the leg).</param>
        /// <returns>The sequence number assigned to the last feature that was created</returns>
        internal uint CreateFeatures(FeatureFactory ff, uint maxSequence, PointFeature startPoint, PointFeature lastPoint)
        {
            PointFeature from = startPoint;
            PointFeature to = null;
            InternalIdValue itemId = new InternalIdValue(maxSequence);

            // If we're dealing with a circular arc, create the underlying circle (and a
            // center point). The radius of the circle is undefined at this stage, but the
            // circle must be present so that created arcs can be cross-referenced to it.

            // Note that it is conceivable that the center point will end up coinciding with
            // another point in the map (it could even coincide with another circular leg in
            // the same connection path).

            CircularLeg cLeg = (this as CircularLeg);
            if (cLeg != null)
            {
                itemId.ItemSequence++;
                cLeg.CreateCircle(ff, itemId.ToString());
            }

            int nSpan = m_Spans.Length;
            for (int i = 0; i < nSpan; i++, from = to)
            {
                SpanInfo span = GetSpanData(i);

                // If we have an end point, add it (so long as this span is not
                // at the very end of the connection path).

                to = null;
                if (span.HasEndPoint)
                {
                    if (i == (nSpan - 1))
                        to = lastPoint;

                    if (to == null)
                    {
                        itemId.ItemSequence++;
                        to = ff.CreatePointFeature(itemId.ToString());
                    }

                    Debug.Assert(to != null);
                }

                // A line can only exist if both end points are defined (the "omit point"
                // directive may well be used to finish a leg without a point, so the first
                // span in the next leg can't have a line).

                if (span.HasLine && from != null)
                {
                    itemId.ItemSequence++;
                    LineFeature line = CreateLine(ff, itemId.ToString(), from, to);
                    line.ObservedLength = span.ObservedDistance;
                    span.CreatedFeature = line;
                }
                else
                {
                    span.CreatedFeature = to;
                }
            }

            return itemId.ItemSequence;
        }

        /// <summary>
        /// Creates a line feature that corresponds to one of the spans on this leg.
        /// </summary>
        /// <param name="ff">The factory for creating new spatial features</param>
        /// <param name="itemName">The name for the item involved</param>
        /// <param name="from">The point at the start of the line (not null).</param>
        /// <param name="to">The point at the end of the line (not null).</param>
        /// <returns>The created line (never null)</returns>
        abstract internal LineFeature CreateLine(FeatureFactory ff, string itemName, PointFeature from, PointFeature to);

        /// <summary>
        /// Defines the geometry for this leg.
        /// </summary>
        /// <param name="ctx">The context in which the geometry is being calculated</param>
        /// <param name="terminal">The position for the start of the leg. Updated to be
        /// the position for the end of the leg.</param>
        /// <param name="bearing">The bearing at the end of the previous leg. Updated for this leg.</param>
        /// <param name="sfac">Scale factor to apply to distances.</param>
        abstract internal void CreateGeometry(EditingContext ctx, ref IPosition terminal, ref double bearing, double sfac);

        abstract internal bool Rollforward (ref PointFeature insert, PathOperation op,
                                                ref IPosition terminal, ref double bearing, double sfac);

        //abstract internal bool SaveFace (PathOperation op, ExtraLeg face);
        abstract internal bool RollforwardFace(ref IPointGeometry insert, PathOperation op, ExtraLeg face,
                                                    IPosition spos, IPosition epos);

        /// <summary>
        /// The number of observed distances (may be 0 when dealing with cul-de-sacs
        /// that are defined in terms on a center point and central angle).
        /// </summary>
        public int Count
        {
            get
            {
                if (NumSpan==1)
                    return (m_Spans[0].ObservedDistance==null ? 0 : 1);
                else
                    return NumSpan;
            }
        }

        public bool HasEndPoint(int index)
        {
            SpanInfo sd = GetSpanData(index);

            if (sd==null)
                return false;
            else
                return sd.HasEndPoint;
        }

        ///// <summary>
        ///// The point (if any) at the end of this leg (may be null if the end
        ///// of the leg was defined with the "omit point" option)
        ///// </summary>
        //internal PointFeature EndPoint
        //{
        //    get
        //    {
        //        SpanData lastSpan = m_Spans[m_Spans.Length - 1];
        //        Feature f = lastSpan.CreatedFeature;
        //        if (f is LineFeature)
        //            return (f as LineFeature).EndPoint;
        //        else
        //            return (f as PointFeature);
        //    }
        //}

        /*
        internal bool IsDeflection
        {
            get { return m_Spans[0].IsDeflection; }
        }
        */

        bool IsCurve
        {
            get { return (this.Circle!=null); }
        }

        /// <summary>
        /// Does this leg contain a staggered face (with distances on both sides of the
        /// underlying line)?
        /// </summary>
        internal bool IsStaggered
        {
            get { return (m_FaceNumber != 0); }
        }

        /// <summary>
        /// The face number of this leg (if this leg is staggered). In the range [0,2]. A value
        /// of zero means the leg is not staggered.
        /// </summary>
        internal uint FaceNumber
        {
            get { return (uint)m_FaceNumber; }

            set
            {
                if (value>2)
                    throw new ArgumentOutOfRangeException();

                m_FaceNumber = (byte)value;
            }
        }

        /// <summary>
        /// Sets the distance of a specific span in this leg.
        /// </summary>
        /// <param name="distance">The distance to assign.</param>
        /// <param name="index">The index of the distance [0,NumSpan-1]</param>
        /// <param name="qualifier"></param>
        /// <returns>True if index was valid.</returns>
        internal bool SetDistance(Distance distance, int index, LegItemFlag qualifier)
        {
            // Return if index is out of range.
            if (index<0 || index>=NumSpan)
                return false;

            // Remember any qualifier.
            if (qualifier != 0)
                m_Spans[index].Flags |= qualifier;

            // Assign the distance
            m_Spans[index].ObservedDistance = distance;
            return true;
        }

        /// <summary>
        /// Gets the total observed length of this leg
        /// </summary>
        /// <returns>The sum of the observed lengths for this leg, in meters on the ground</returns>
        public double GetTotal()
        {
            double total = 0.0;

            foreach (SpanInfo sd in m_Spans)
            {
                Distance d = sd.ObservedDistance;
                if (d!=null)
                    total += d.Meters;
            }

            return total;
        }

        /// <summary>
        /// Gets the observed distance to the start and end of a specific
        /// span, in meters on the ground.
        /// </summary>
        /// <param name="index">Index of the required span.</param>
        /// <param name="sdist">Distance to the start of the span.</param>
        /// <param name="edist">Distance to the end of the span.</param>
        public void GetDistances(int index, out double sdist, out double edist)
        {
            // Confirm required index is in range.
            if (index >= NumSpan)
                throw new IndexOutOfRangeException("Leg.GetDistances -- bad index");

            sdist = edist = 0.0;

            // Initialize distance so far.
            double total = 0.0;

            // Accumulate the distance to the required span.
            for (int i = 0; i <= index; i++)
            {
                if (i == index)
                    sdist = total;

                total += m_Spans[i].ObservedDistance.Meters;

                if (i == index)
                    edist = total;
            }
        }

        /// <summary>
        /// Returns the feature that corresponds to a specific span on this leg.
        /// </summary>
        /// <param name="index">Index of the span.</param>
        /// <returns>The associated feature (if any).</returns>
        protected Feature GetFeature(int index)
        {
            // Confirm the index is valid.
            if (index<0 || index>=NumSpan)
                throw new IndexOutOfRangeException("Leg.GetFeature - Bad index");

            return m_Spans[index].CreatedFeature;
        }

        /// <summary>
        /// Handles rollback processing for this leg. This de-activates all
        /// features that were created.
        /// </summary>
        /// <param name="op">The operation that this leg relates to.</param>
        /// <returns></returns>
        /*
        internal bool OnRollback(Operation op)
        {
            // Go through each span, de-activating every feature that
            // was created by the specified operation.
            for (int i = 0; i < NumSpan; i++)
            {
                // What sort of feature (if any) represents the span. If
                // it's null, just skip the span. If it's a point created
                // by the specified op, de-activate the point. If it's a
                // line created by the specified op, de-activate the line,
                // as well as the two end points (so long as they were
                // created by the specified op).

                Feature feat = m_Spans[i].CreatedFeature;
                if (feat != null)
                {
                    feat.OnRollback(op);
                }
            }

            return true;
        }
        */

        /// <summary>
        /// Handles any intersections created along this leg.
        /// </summary>
        /// <param name="op">The operation that created the leg.</param>
        void Intersect(Operation op)
        {
            // Go through each span, intersecting every feature that was created.
            foreach (SpanInfo sd in m_Spans)
            {
                Feature feat = sd.CreatedFeature;
                if (feat != null)
                    feat.IsMoved = true;
            }
        }

        /// <summary>
        /// Loads a list of the features that were created by this leg.
        /// </summary>
        /// <param name="op">The operation that this leg relates to.</param>
        /// <param name="flist">The list to store the results. This list will be
        /// appended to, so you may want to clear the list prior to call.</param>
        /// <remarks>The <see cref="CircularLeg"/> provides an override that
        /// is responsible for appending the center point.</remarks>
        internal virtual void GetFeatures(Operation op, List<Feature> flist)
        {
            foreach (SpanInfo sd in m_Spans)
            {
                Feature f = sd.CreatedFeature;

                if (f != null)
                {
                    // If the feature is a line, include the end point
                    LineFeature line = (f as LineFeature);
                    if (line!=null)
                    {
                        PointFeature point = line.EndPoint;
                        if (Object.ReferenceEquals(point.Creator, op))
                            flist.Add(point);
                    }

                    // Append the feature (point or line) associated with the span
                    flist.Add(f);
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
        internal void GetSpans(List<Distance> distances)
        {
            foreach (SpanInfo sd in m_Spans)
            {
                Distance d = sd.ObservedDistance;
                if (d == null)
                {
                    Debug.Assert(this is CircularLeg);
                    Debug.Assert(m_Spans.Length == 1);

                    // If this is a cul-de-sac that had no observed spans, get the
                    // circular leg to define the distance (in meters on the ground).
                    DistanceUnit mUnit = EditingController.GetUnits(DistanceUnitType.Meters);
                    distances.Add(new Distance(this.Length.Meters, mUnit));
                }
                else
                    distances.Add(d);
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
        internal bool IsCreatorOf(Feature feature)
        {
            // NOTE: the logic here should be similar to that in Leg.GetFeatures ...

            // What operation does this leg belong to?
            Operation op = feature.Creator;
            if (op == null)
                return false;

            // If the feature is inactive, we'll want CePrimitive::GetpPoint()
            // to also search for inactive points.
            //LOGICAL onlyActive = feature.IsActive();

            foreach (SpanInfo sd in m_Spans)
            {
                // Skip if this was a null span.
                Feature f = sd.CreatedFeature;
                if (f==null)
                    continue;

                // Return if we have a match.
                if (Object.ReferenceEquals(f, feature))
                    return true;

                // If the feature is a line, also check any point feature at the end.
                LineFeature line = (f as LineFeature);
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
        internal PointFeature GetEndPoint(Operation op)
        {
            // If the very last feature for this leg is a point, that's the thing we want.
            Feature feat = m_Spans[NumSpan - 1].CreatedFeature;
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
            // If the first feature for this leg is a point, that's the thing we want.
            Feature feat = m_Spans[0].CreatedFeature;
            PointFeature point = (feat as PointFeature);
            if (point!=null)
                return point;

            // Otherwise the first feature should be a line, so
            // we want IT'S start point (either active or inactive).
            LineFeature line = (feat as LineFeature);
            if (line!=null)
            {
                point = line.StartPoint;
                if (Object.ReferenceEquals(point.Creator, op))
                    return point;
            }

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

        /*
        /// <summary>
        /// Marks this leg as having a deflection angle at the start. This applies only to
        /// straight legs. There must be a preceding leg for this to make any sense.
        /// </summary>
        /// <param name="set">Mark as deflection? Default=true.</param>
        protected void SetDeflection(bool set)
        {
            m_Spans[0].IsDeflection = set;
        }
        */

        /// <summary>
        /// Checks if this leg will generate a line feature.
        /// </summary>
        /// <param name="index">The index of the span in question.</param>
        /// <returns>True if line feature will be produced.</returns>
        public bool HasLine(int index)
        {
            // No feature if the span index is out of range.
            SpanInfo sd = GetSpanData(index);
            if (sd==null)
                return false;
            else
                return sd.HasLine;
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
            // Expand the array of span data
            int numSpan = NumSpan;
            SpanInfo[] newSpans = new SpanInfo[numSpan + 1];

            // Copy over stuff prior to the new distance
            for (int i=0; i<index; i++)
                newSpans[i] = m_Spans[i];

            // Stick in the new guy with miss-connect flag
            SpanInfo extraSpan = new SpanInfo();
            extraSpan.ObservedDistance = newdist;
            extraSpan.Flags = LegItemFlag.MissConnect;
            newSpans[index] = extraSpan;

            // If a line is required, flag it for creation when rollforward runs (we can't do
            // it right now, since the end positions are currently coincident).
            if (wantLine)
                extraSpan.Flags |= LegItemFlag.NewLine;

            // Copy over the rest.
            for (int i=index; i<numSpan; i++)
                newSpans[i + 1] = m_Spans[i];

            // Replace original arrays with the new ones
            m_Spans = newSpans;

            /*
            // If we inserted at the very start, ensure that any
            // deflection angle switch is still with the very first span.
            if (index==0 && (m_Spans[1].Flags & LegItemFlag.Deflection)!=0)
            {
                m_Spans[0].Flags |= LegItemFlag.Deflection;
                m_Spans[1].Flags &= (~LegItemFlag.Deflection);
            }
             */
        }

        /// <summary>
        /// Returns the index of a specific distance along this leg.
        /// </summary>
        /// <param name="dist">The distance to look for.</param>
        /// <returns>The index of the distance (-1 if not found).</returns>
        int GetIndex(Distance dist)
        {
            for (int i=0; i<m_Spans.Length; i++)
            {
                Distance d = m_Spans[i].ObservedDistance;
                if (Object.ReferenceEquals(d, dist))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Returns the index of a feature along this leg.
        /// </summary>
        /// <param name="feat">The feature to look for.</param>
        /// <returns>The index of the feature (-1 if not found).</returns>
        internal int GetIndex(Feature feat)
        {
            for (int i=0; i<m_Spans.Length; i++)
            {
                Feature f = m_Spans[i].CreatedFeature;
                if (Object.ReferenceEquals(f, feat))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// The number of spans in this leg is the number of elements in the
        /// <see cref="m_Spans"/> array.
        /// </summary>
        internal int NumSpan
        {
            get { return m_Spans.Length; }
        }

        /// <summary>
        /// Breaks this leg into two legs. The break must leave at least one distance
        /// in each of the resultant legs.
        /// </summary>
        /// <param name="index">The index of the span that should be at the start of the extra leg.
        /// Should be greater than zero.</param>
        /// <param name="to">The leg to move stuff to. Must have enough space to hold the extra stuff.</param>
        /// <returns>True if moved ok.</returns>
        internal bool MoveEndLeg(int index, Leg to)
        {
            // Destination MUST have exactly the right size.
            if (to.NumSpan != (this.NumSpan-index))
                return false;

            // Transfer stuff to the new leg.
            for (int iSrc=index, iDst=0; iSrc<NumSpan; iSrc++, iDst++)
                to.m_Spans[iDst] = m_Spans[iSrc];

            // Check for special case where the entire leg has been
            // copied (this isn't really expected to happen).
            if (index==0)
            {
                m_Spans = null;
                return true;
            }

            int numSpan = index;
            SpanInfo[] newSpans = new SpanInfo[numSpan];

            // Copy over the initial stuff.
            for (int i=0; i<numSpan; i++)
                newSpans[i] = m_Spans[i];

            // Replace the original array with the new one.
            m_Spans = newSpans;

            return true;
        }

        /// <summary>
        /// Checks whether a specific span is new.
        /// </summary>
        /// <param name="index">The index of the span of interest.</param>
        /// <returns>True if new span.</returns>
        internal bool IsNewSpan(int index)
        {
            SpanInfo sd = GetSpanData(index);

            if (sd==null)
                return false;
            else
                return sd.IsNewSpan;            
        }

        /// <summary>
        /// Returns the very first line that was created along this leg (if any).
        /// </summary>
        /// <returns>The first line (null if no lines were created).</returns>
        internal LineFeature GetFirstLine()
        {
            foreach (SpanInfo sd in m_Spans)
            {
                LineFeature line = (sd.CreatedFeature as LineFeature);
                if (line!=null)
                    return line;
            }

            return null;
        }

        /// <summary>
        /// Returns the very last line that was created along this leg (if any).
        /// </summary>
        /// <returns>The last line (null if no lines were created).</returns>
        internal LineFeature GetLastLine()
        {
            for (int i=(m_Spans.Length-1); i>=0; i--)
            {
                LineFeature line = (m_Spans[i].CreatedFeature as LineFeature);
                if (line!=null)
                    return line;
            }

            return null;
        }

        /// <summary>
        /// Appends observations to a string that represents this leg.
        /// </summary>
        /// <param name="str">The string buffer to append to.</param>
        /// <param name="defaultEntryUnit">The distance units that should be treated as the default.
        /// Formatted distances that were specified using these units will not contain the units
        /// abbreviation</param>
        /*
        internal void AddToString(StringBuilder str, DistanceUnit defaultEntryUnit)
        {
            // Return if there are no observed spans.
            if (NumSpan==0)
                return;

            string[] dists = new string[m_Spans.Length];

            // Format each distance.
            for (int i=0; i<m_Spans.Length; i++)
            {
                SpanData sd = m_Spans[i];
                Distance d = sd.ObservedDistance;
                if (d == null) // is this possible?
                    dists[i] = String.Empty;
                else
                {
                    // Get the formatted distance
                    string distString = FormatDistance(d, defaultEntryUnit);

                    // Append any qualifiers
                    if (sd.IsMissConnect)
                        distString += "/-";

                    if (sd.IsOmitPoint)
                        distString += "/*";

                    dists[i] = distString;
                }
            }

            // Output the first distance
            str.Append(dists[0]);

            // Output subsequent distances (with possible repeat count for unqualified spans)
            int numDist = (dists[0].Contains("/") ? 0 : 1);

            for (int i=1; i<dists.Length; i++)
            {
                // If the current distance has a qualifier, flush out any repeat count and
                // the current distance (rather than something like 10*4/-, we output 10*3 10/-).
                // This is just to avoid potential confusion.

                if (dists[i].Contains("/"))
                {
                    if (numDist>1)
                        str.Append("*"+numDist);

                    str.Append(" ");
                    str.Append(dists[i]);
                    numDist = 0;
                }
                else
                {
                    if (dists[i] == dists[i-1])
                        numDist++;
                    else
                    {
                        if (numDist>1)
                            str.Append("*"+numDist);

                        str.Append(" ");
                        str.Append(dists[i]);
                        numDist = 1;
                    }
                }
            }

            if (numDist>1)
                str.Append("*"+numDist);
        }
        */

        /// <summary>
        /// Formats a distance so that it can be persisted as a string. Unlike the various
        /// <see cref="Distance.Format"/> methods, this method will not truncate observed
        /// distances to a specific number of significant digits.
        /// </summary>
        /// <param name="d">The distance to format</param>
        /// <param name="defaultEntryUnit">The distance units that should be treated as the default.
        /// Formatted distances that were specified using these units will not contain the units
        /// abbreviation</param>
        /// <returns>A string representing the supplied distance</returns>
        string FormatDistance(Distance d, DistanceUnit defaultEntryUnit)
        {
            string str = d.ObservedValue.ToString();

            if (d.EntryUnit.UnitType == defaultEntryUnit.UnitType)
                return str;
            else
                return str + d.EntryUnit.Abbreviation;
        }

        /*
        /// <summary>
        /// Records that this leg is one of two legs that make up a staggered leg.
        /// </summary>
        /// <param name="face">The face number (1 or 2).</param>
        internal void SetStaggered(int face)
        {
            if (face==1)
                m_Spans[0].IsFace1 = true;
            else if (face==2)
                m_Spans[0].IsFace2 = true;
        }
        */

        /// <summary>
        /// Creates a set of line segments (and points) for this leg. This function is called
        /// only when adding features to an <see cref="ExtraLeg"/>. THIS leg needs to be the
        /// second face of a pair of legs.
        /// </summary>
        /// <param name="op">The connection path that this leg belongs to.</param>
        /// <param name="spos">The position at the start of the leg.</param>
        /// <param name="epos">The position at the end of the leg.</param>
        /// <returns>True if created ok.</returns>
        internal bool MakeSegments(PathOperation op, IPosition spos, IPosition epos)
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
            double bearing = Geom.BearingInRadians(spos, epos);

            // Define start of first segment.
            IPosition start = spos;
            IPosition end;

            // Haven't got anywhere yet.
            double totobs = 0.0;
            CadastralMapModel map = CadastralMapModel.Current;

	        // Add non-topological arcs for each observed distance.
            //for (int i=0; i<NumSpan; i++, start=end)
            for (int i = 0; i < NumSpan; i++, start = end)
            {
                Distance d = m_Spans[i].ObservedDistance;
                Debug.Assert(d != null);

                // Update the observed length.
                totobs += d.Meters;

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
                if (Object.ReferenceEquals(endPoint.Creator, op) && endPoint.FeatureId==null)
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
                //Debug.Assert(m_Creations!=null);
                //Debug.Assert(m_Creations[i]==null);
                //m_Creations[i] = line;
                Debug.Assert(m_Spans[i].CreatedFeature == null);
                m_Spans[i].CreatedFeature = line;
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
        internal bool UpdateSegments(IPointGeometry insert, PathOperation op,
                                        IPosition spos, IPosition epos)
        {
            throw new NotImplementedException("Leg.UpdateSegments");
            /*
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
            double bearing = Geom.BearingInRadians(spos, epos);

            // Haven't got anywhere yet.
            double totobs = 0.0;

            // Go through each feature (which correspond to lines), shifting the location at
            // the end of each one. Except the last one, which is the start of the last leg.
            // The actual terminals should get moved when the adjacent legs are processed.

            for (int i=0; i<(NumSpan-1); i++)
            {
                // Update the observed length.
                Distance d = m_Spans[i].ObservedDistance;
                Debug.Assert(d != null);
                totobs += d.Meters;

                // Apply factor to get us to the end of the leg.
                double elen = totobs * factor;

                // Define the position of the point at the end of the span.
                IPosition pos = Geom.Polar(spos, bearing, elen);

                // Get the line feature that was added and move
                // the location at the end of it.
                LineFeature line = (m_Spans[i].CreatedFeature as LineFeature);
                Debug.Assert(line != null);
                line.EndPoint.MovePoint(uc, pos);
            }

            return true;
             */
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
        internal bool MakeCurves(PathOperation op, IPosition spos, IPosition epos, Circle circle, bool iscw)
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
                totobs += m_Spans[i].ObservedDistance.Meters;

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
                if (Object.ReferenceEquals(endPoint.Creator, op) && endPoint.FeatureId==null)
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
                Debug.Assert(m_Spans[i].CreatedFeature==null);
                m_Spans[i].CreatedFeature = line;
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
        internal bool UpdateCurves(IPointGeometry insert, PathOperation op, IPosition spos, IPosition epos,
                            Circle circle, bool iscw)
        {
            throw new NotImplementedException();
            /*
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
                totobs += m_Spans[i].ObservedDistance.Meters;

                // Apply factor to get us to the end of the leg.
                double elen = totobs * factor;

                // Define the end position.
                bool isEndDefined = curve.GetPosition(new Length(elen), out end);
                Debug.Assert(isEndDefined);

                // Get the line feature that was added.
                ArcFeature arc = (m_Spans[i].CreatedFeature as ArcFeature);
                Debug.Assert(arc!=null);

                // If the arc is now on a different circle, change it.
                arc.Move(circle, iscw);

                // Move the location at the end of the arc (so long
                // as it's not the very last location).
                if (i<(NumSpan-1))
                    arc.EndPoint.MovePoint(uc, end);
            }

            return true;
             */
        }

        /// <summary>
        /// Gets the data defining the span at the specified array index.
        /// </summary>
        /// <param name="index">The array index of the desired span</param>
        /// <returns>The corresponding span data (null if specified array index was
        /// out of bounds)</returns>
        internal SpanInfo GetSpanData(int index)
        {
            if (index<0 || index>=m_Spans.Length)
                return null;
            else
                return m_Spans[index];
        }
    }
}
