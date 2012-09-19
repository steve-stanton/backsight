// <remarks>
// Copyright 2012 - Steve Stanton. This file is part of Backsight
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
using System.Text;

using Backsight.Editor.Observations;
using Backsight.Editor.Operations;
using Backsight.Environment;


namespace Backsight.Editor
{
    /// <summary>
    /// One face of a <see cref="Leg"/> in a connection path.
    /// </summary>
    /// <seealso cref="PathOperation"/>
    class LegFace : IPersistent
    {
        #region Class data

        /// <summary>
        /// The leg that this face relates to.
        /// </summary>
        internal Leg Leg { get; set; }

        /// <summary>
        /// A sequence number for this face (denoting the order in which the leg was created during
        /// the lifetime of a project).
        /// <para/>
        /// When dealing with transient legs (such as those created for the sake of preview), the sequence
        /// number may be undefined (with a value of <see cref="InternalIdValue.Empty"/>). You must
        /// explicitly assign the sequence value in situations where it is significant.
        /// </summary>
        /// <remarks>In the context of something like a connection path, each successive leg
        /// will have a sequence number that is one less than the sequence assigned to features
        /// on that leg. For example, successive legs might have sequence values of 10, 25, 30.
        /// <para/>
        /// Now suppose you later update the path by inserting an extra leg into the sequence.
        /// In that case, the sequence may end up as 10, 400, 25, 30.
        /// </remarks>
        internal InternalIdValue Sequence { get; set; }

        /// <summary>
        /// The data that defines each span on this leg face (should always contain at least one element).
        /// </summary>
        /// <remarks>When dealing with cul-de-sacs that are defined only with a central angle (no distances),
        /// this will be an array containing one element, with a null observed distance.
        /// </remarks>
        SpanInfo[] m_Spans;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LegFace"/> class that has undefined spans, and
        /// no sequence number.
        /// </summary>
        internal LegFace(int nspan)
        {
            // Allocate an array of spans (always at least ONE).
            m_Spans = new SpanInfo[Math.Max(1, nspan)];
            for (int i = 0; i < m_Spans.Length; i++)
                m_Spans[i] = new SpanInfo();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LegFace"/> class.
        /// <param name="leg">The leg this face relates to.</param>
        /// <param name="dists">The observed distances for this face.</param>
        /// </summary>
        internal LegFace(Leg leg, Distance[] dists)
        {
            this.Leg = leg;

            m_Spans = new SpanInfo[dists.Length];
            for (int i=0; i<m_Spans.Length; i++)
            {
                m_Spans[i] = new SpanInfo();
                m_Spans[i].ObservedDistance = dists[i];
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LegFace"/> class.
        /// </summary>
        /// <param name="leg">The leg this face relates to.</param>
        /// <param name="copy">The face to copy from.</param>
        /// <param name="startIndex">The start index of the first span to copy</param>
        internal LegFace(Leg leg, LegFace copy, int startIndex)
        {
            this.Leg = leg;

            int nSpan = copy.m_Spans.Length - startIndex;
            if (nSpan <= 0)
                throw new IndexOutOfRangeException();

            // Perform shallow copy
            m_Spans = new SpanInfo[nSpan];
            Array.Copy(copy.m_Spans, startIndex, m_Spans, 0, nSpan); 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LegFace"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal LegFace(EditDeserializer editDeserializer)
        {
            // Only connection paths should generate LegFace instances
            PathOperation op = (editDeserializer.CurrentEdit as PathOperation);
            if (op == null)
                throw new ApplicationException("Unexpected creating edit for a leg face");

            this.Sequence = editDeserializer.ReadInternalId(DataField.Id);

            if (editDeserializer.IsNextField(DataField.PrimaryFaceId))
            {
                InternalIdValue parentLegId = editDeserializer.ReadInternalId(DataField.PrimaryFaceId);
                Leg = op.FindLeg(parentLegId);

                if (Leg == null)
                    throw new ApplicationException("Cannot locate leg " + parentLegId);

                Leg.AlternateFace = this;
            }
            else
            {
                // This should never happen. Primary faces are not serialized using the LegFace
                // class (we only use LegFace as part of a PathOperation to simplify import of
                // extra legs from old CEdit files).

                throw new ApplicationException();
            }

            // Convert the data entry string into observed spans
            string entryString = editDeserializer.ReadString(DataField.EntryString);
            DistanceUnit defaultEntryUnit = EditingController.Current.EntryUnit;
            Distance[] dists = LineSubdivisionFace.GetDistances(entryString, defaultEntryUnit, false);
            m_Spans = new SpanInfo[dists.Length];

            for (int i=0; i<m_Spans.Length; i++)
            {
                m_Spans[i] = new SpanInfo() { ObservedDistance = dists[i] };
            }
        }

        #endregion

        /// <summary>
        /// The face number of this leg (if this leg is staggered). In the range [0,2]. A value
        /// of zero means the leg is not staggered.
        /// </summary>
        internal uint FaceNumber
        {
            get
            {
                if (this.Leg == null)
                    return 0;
                else
                    return (uint)(this.Leg.AlternateFace == this ? 2 : 1);
            }
        }

        /// <summary>
        /// Inserts an extra distance into this leg.
        /// </summary>
        /// <param name="newdist">The new distance to insert.</param>
        /// <param name="curdist">A distance that this leg already knows about.</param>
        /// <param name="isBefore">Should the new distance go before the existing one?</param>
        /// <param name="wantLine">Should a new line be created (it won't happen until rollforward
        /// occurs, but it will get marked to happen).</param>
        /// <returns>The index where the extra distance was saved.</returns>
        internal int Insert(Distance newdist, Distance curdist, bool isBefore, bool wantLine)
        {
            // Get the index of the currently defined distance.
            int index = GetIndex(curdist);
            if (index < 0)
                return -1;

            if (isBefore)
            {
                InsertAt(index, newdist, wantLine);
                return index;
            }
            else
            {
                InsertAt(index + 1, newdist, wantLine);
                return index + 1;
            }
        }

        /// <summary>
        /// The number of spans in this face is the number of elements in the
        /// <see cref="m_Spans"/> array.
        /// </summary>
        internal int NumSpan
        {
            get { return m_Spans.Length; }
        }

        /// <summary>
        /// The number of internal IDs that should be reserved for this face.
        /// </summary>
        /// <value>1 for the face itself +  2 for every span (even though they may be unused).</value>
        internal uint NumIds
        {
            get { return (1 + 2 * (uint)NumSpan); }
        }

        /// <summary>
        /// The number of observed distances (may be 0 when dealing with cul-de-sacs
        /// that are defined in terms on a center point and central angle).
        /// </summary>
        internal int Count
        {
            get
            {
                if (NumSpan == 1)
                    return (m_Spans[0].ObservedDistance == null ? 0 : 1);
                else
                    return NumSpan;
            }
        }

        /// <summary>
        /// Obtains the observed lengths of each span on
        /// this leg. In the case of cul-de-sacs that have no observed span,
        /// the distance will be derived from the central angle and radius.
        /// </summary>
        internal Distance[] GetObservedDistances()
        {
            List<Distance> distances = new List<Distance>();

            foreach (SpanInfo sd in m_Spans)
            {
                Distance d = sd.ObservedDistance;
                if (d == null)
                {
                    Debug.Assert(this.Leg is CircularLeg);
                    Debug.Assert(m_Spans.Length == 1);

                    // If this is a cul-de-sac that had no observed spans, get the
                    // circular leg to define the distance (in meters on the ground).
                    DistanceUnit mUnit = EditingController.GetUnits(DistanceUnitType.Meters);
                    distances.Add(new Distance(this.Leg.Length.Meters, mUnit));
                }
                else
                    distances.Add(d);
            }

            return distances.ToArray();
        }

        /// <summary>
        /// Inserts a new distance into this face.
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
            for (int i = 0; i < index; i++)
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
            for (int i = index; i < numSpan; i++)
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
        internal int GetIndex(Distance dist)
        {
            for (int i = 0; i < m_Spans.Length; i++)
            {
                Distance d = m_Spans[i].ObservedDistance;
                if (Object.ReferenceEquals(d, dist))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Gets the data defining the span at the specified array index.
        /// </summary>
        /// <param name="index">The array index of the desired span</param>
        /// <returns>The corresponding span data (null if specified array index was
        /// out of bounds)</returns>
        internal SpanInfo GetSpanData(int index)
        {
            if (index < 0 || index >= m_Spans.Length)
                return null;
            else
                return m_Spans[index];
        }

        /// <summary>
        /// The data that defines each span on this face (should always contain at least one element).
        /// </summary>
        internal SpanInfo[] Spans
        {
            get { return m_Spans; }
        }

        /// <summary>
        /// Creates stubs for all items that might be created for this face.
        /// </summary>
        /// <param name="creator">The editing operation to mark as the creator of the features.</param>
        /// <param name="pointType">The entity type for created points</param>
        /// <param name="lineType">The entity type for created lines</param>
        /// <returns>Two stubs for each span along this face (one point, one line per span).</returns>
        internal FeatureStub[] CreateStubs(Operation creator, IEntity pointType, IEntity lineType)
        {
            List<FeatureStub> stubList = new List<FeatureStub>();
            uint sequence = this.Sequence.ItemSequence;

            // Reserve two items for each span along the leg (the very first stub will have an
            // ID one greater than the face itself).
            for (int i = 0; i < m_Spans.Length; i++)
            {
                stubList.Add(CreateStub(creator, ++sequence, pointType));
                stubList.Add(CreateStub(creator, ++sequence, lineType));
            }

            return stubList.ToArray();
        }

        /// <summary>
        /// Creates a stub for an item on this face.
        /// </summary>
        /// <param name="itemSequence">The sequence number to assign to the stub</param>
        /// <param name="ent">The entity type for the stub</param>
        /// <returns>The created stub</returns>
        FeatureStub CreateStub(Operation creator, uint itemSequence, IEntity ent)
        {
            InternalIdValue iid = new InternalIdValue(itemSequence);
            return new FeatureStub(creator, iid, ent, null);
        }

        /// <summary>
        /// Creates spatial features (points and lines) for this face. The created
        /// features don't have any geometry.
        /// </summary>
        /// <param name="ff">The factory for creating new spatial features</param>
        /// <param name="startPoint">The point (if any) at the start of this leg. May be
        /// null in a situation where the preceding leg ended with an "omit point" directive.</param>
        /// <param name="lastPoint">The point that should be used for the very end
        /// of the leg (specify null if a point should be created at the end of the leg).</param>
        internal void CreateFeatures(FeatureFactory ff, PointFeature startPoint, PointFeature lastPoint)
        {
            PointFeature from = startPoint;
            PointFeature to = null;

            // The initial item sequence relates to the face itself. The first feature along the face will
            // have a sequence number one higher.
            uint maxSequence = this.Sequence.ItemSequence;

            int nSpan = m_Spans.Length;
            for (int i = 0; i < nSpan; i++, from = to)
            {
                SpanInfo span = GetSpanData(i);

                // If we have an end point, add it (so long as this span is not
                // at the very end of the connection path).

                to = null;
                maxSequence++;

                if (span.HasEndPoint)
                {
                    if (i == (nSpan - 1))
                        to = lastPoint;

                    if (to == null)
                        to = ff.CreatePointFeature(maxSequence.ToString());

                    Debug.Assert(to != null);
                }

                // A line can only exist if both end points are defined (the "omit point"
                // directive may well be used to finish a leg without a point, so the first
                // span in the next leg can't have a line).

                maxSequence++;
                if (span.HasLine && from != null)
                {
                    LineFeature line = this.Leg.CreateLine(ff, maxSequence.ToString(), from, to);
                    line.ObservedLength = span.ObservedDistance;

                    // Alternate faces should by non-topological. And mark as "void" so that it can be
                    // skipped on export to AutoCad.
                    if (FaceNumber == 2)
                    {
                        line.SetTopology(false); // should probably be false already
                        line.IsVoid = true;
                    }

                    span.CreatedFeature = line;
                }
                else
                {
                    span.CreatedFeature = to;
                }
            }
        }

        /// <summary>
        /// Loads a list of the features that were created on this face.
        /// </summary>
        /// <param name="op">The operation that this face relates to.</param>
        /// <param name="flist">The list to store the results. This list will be
        /// appended to, so you may want to clear the list prior to call.</param>
        internal void GetFeatures(Operation op, List<Feature> flist)
        {
            foreach (SpanInfo sd in m_Spans)
            {
                Feature f = sd.CreatedFeature;

                if (f != null)
                {
                    // If the feature is a line, include the end point
                    LineFeature line = (f as LineFeature);
                    if (line != null)
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
        /// Returns the point feature that is at the end of this leg.
        /// </summary>
        /// <param name="op">The operation that is expected to have created the end point.</param>
        /// <returns>The point object at the end (could conceivably be null).</returns>
        internal PointFeature GetEndPoint(Operation op)
        {
            // If the very last feature for this leg is a point, that's the thing we want.
            Feature feat = m_Spans[NumSpan - 1].CreatedFeature;
            PointFeature point = (feat as PointFeature);
            if (point != null)
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
        /// Checks whether this face is the face that caused the creation of a specific feature.
        /// </summary>
        /// <param name="feature">The feature to search for. The creator of this feature
        /// is expected to match the operation that contains this face.
        /// </param>
        /// <returns>True if this face caused the creation of the feature.</returns>
        internal bool IsCreatorOf(Feature feature)
        {
            foreach (SpanInfo sd in m_Spans)
            {
                // Skip if this was a null span.
                Feature f = sd.CreatedFeature;
                if (f == null)
                    continue;

                // Return if we have a match.
                if (Object.ReferenceEquals(f, feature))
                    return true;

                // If the feature is a line, also check any point feature at the end.
                LineFeature line = (f as LineFeature);
                if (line == null)
                    continue;

                //const CePoint* pPoint = pArc->GetpEnd()->GetpPoint(*pop, onlyActive);
                PointFeature point = line.EndPoint;
                if (Object.ReferenceEquals(point, feature))
                    return true;

                // If it's a circular arc, check the circle center point too.
                ArcFeature arc = (line as ArcFeature);
                if (arc != null && Object.ReferenceEquals(arc.Circle.CenterPoint, feature))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the index of a feature along this face.
        /// </summary>
        /// <param name="feat">The feature to look for.</param>
        /// <returns>The index of the feature (-1 if not found).</returns>
        internal int GetIndex(Feature feat)
        {
            for (int i = 0; i < m_Spans.Length; i++)
            {
                Feature f = m_Spans[i].CreatedFeature;
                if (Object.ReferenceEquals(f, feat))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Gets the total observed length of this face
        /// </summary>
        /// <returns>The sum of the observed lengths for this face, in meters on the ground</returns>
        internal double GetTotal()
        {
            double total = 0.0;

            foreach (SpanInfo sd in m_Spans)
            {
                Distance d = sd.ObservedDistance;
                if (d != null)
                    total += d.Meters;
            }

            return total;
        }

        /// <summary>
        /// Sets the distance of a specific span in this face.
        /// </summary>
        /// <param name="distance">The distance to assign.</param>
        /// <param name="index">The index of the distance [0,NumSpan-1]</param>
        /// <param name="qualifier"></param>
        /// <returns>True if index was valid.</returns>
        internal bool SetDistance(Distance distance, int index, LegItemFlag qualifier)
        {
            // Return if index is out of range.
            if (index < 0 || index >= NumSpan)
                return false;

            // Remember any qualifier.
            if (qualifier != 0)
                m_Spans[index].Flags |= qualifier;

            // Assign the distance
            m_Spans[index].ObservedDistance = distance;
            return true;
        }

        /// <summary>
        /// Truncates this face by discarding one or more spans at the end (for use when breaking
        /// straight legs).
        /// </summary>
        /// <param name="truncatedLength">The number of spans that should be retained.</param>
        /// <exception cref="ArgumentException">If the truncated length would lead to an empty leg, or nothing
        /// would be truncated.</exception>
        internal void TruncateLeg(int truncatedLength)
        {
            if (truncatedLength <= 0 || truncatedLength >= m_Spans.Length)
                throw new ArgumentException();

            // Shrink the array (throwaway the spans at the end)
            Array.Resize<SpanInfo>(ref m_Spans, truncatedLength);
        }

        /// <summary>
        /// Obtains a string that corresponds to the observed distances for spans on this face.
        /// </summary>
        /// <param name="defaultEntryUnit">The distance units that should be treated as the default
        /// (null if there is no default).
        /// Formatted distances that were specified using these units will not contain the units
        /// abbreviation. Specify null if the units should always be appended.</param>
        /// <returns>A data entry string corresponding to the distances for this face</returns>
        string GetEntryString(DistanceUnit defaultEntryUnit)
        {
            // Return if there are no observed spans.
            if (NumSpan==0)
                return String.Empty;

            string[] dists = new string[m_Spans.Length];

            // Format each distance.
            for (int i=0; i<m_Spans.Length; i++)
            {
                SpanInfo sd = m_Spans[i];
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
            var str = new StringBuilder();
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

            return str.ToString();
        }

        /// <summary>
        /// Formats a distance so that it can be persisted as a string. Unlike the various
        /// <see cref="Distance.Format"/> methods, this method will not truncate observed
        /// distances to a specific number of significant digits.
        /// </summary>
        /// <param name="d">The distance to format</param>
        /// <param name="defaultEntryUnit">The distance units that should be treated as the default
        /// (null if there is no default).
        /// Formatted distances that were specified using these units will not contain the units
        /// abbreviation. Specify null if the units should always be appended.</param>
        /// abbreviation.</param>
        /// <returns>A string representing the supplied distance</returns>
        string FormatDistance(Distance d, DistanceUnit defaultEntryUnit)
        {
            string str = d.ObservedValue.ToString();

            if (defaultEntryUnit != null && d.EntryUnit.UnitType == defaultEntryUnit.UnitType)
                return str;
            else
                return str + d.EntryUnit.Abbreviation;
        }

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public void WriteData(EditSerializer editSerializer)
        {
            editSerializer.WriteInternalId(DataField.Id, this.Sequence);

            if (Leg.PrimaryFace != this)
                editSerializer.WriteInternalId(DataField.PrimaryFaceId, Leg.PrimaryFace.Sequence);

            editSerializer.WriteString(DataField.EntryString, GetEntryString(null));
        }
    }
}
