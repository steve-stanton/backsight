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

using Backsight.Editor.Observations;
using Backsight.Editor.Operations;

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
        /// The initial definition for this leg (if <see cref="m_OtherSide"/> is undefined, the
        /// spans relate to both sides of the leg).
        /// </summary>
        readonly LegFace m_FirstSide;

        /// <summary>
        /// An alternate set of spans for this leg, used to represent staggered property lots.
        /// </summary>
        LegFace m_OtherSide;

        #endregion

        #region Constructors

        protected Leg(int nspan)
        {
            m_FirstSide = new LegFace(nspan);
            m_OtherSide = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Leg"/> class that corresponds to
        /// the end of another leg face (for use when breaking a straight leg).
        /// </summary>
        /// <param name="face">The face on the other leg</param>
        /// <param name="startIndex">The array index of the first span that should be copied.</param>
        protected Leg(LegFace face, int startIndex)
        {
            m_FirstSide = new LegFace(this, face, startIndex);
            m_OtherSide = null;
        }

        #endregion

        abstract internal Circle Circle { get; }
        abstract internal ILength Length { get; }
        abstract internal IPosition Center { get; }
        abstract internal void Project(ref IPosition pos, ref double bearing, double sfac);

        /// <summary>
        /// Obtains the geometry for spans along this leg (to be called only via implementations of <see cref="GetSections"/>).
        /// </summary>
        /// <param name="start">The position for the start of the leg.
        /// <param name="bearing">The bearing of the leg.</param>
        /// <param name="sfac">Scale factor to apply to distances.</param>
        /// <param name="spans">Information for the spans coinciding with this leg.</param>
        /// <returns>The sections along this leg</returns>
        abstract internal ILineGeometry[] GetSpanSections(IPosition start, double bearing, double sfac, SpanInfo[] spans);

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
            // If we're dealing with a circular arc, create the underlying circle (and a
            // center point). The radius of the circle is undefined at this stage, but the
            // circle must be present so that created arcs can be cross-referenced to it.

            // Note that it is conceivable that the center point will end up coinciding with
            // another point in the map (it could even coincide with another circular leg in
            // the same connection path).

            maxSequence++;
            CircularLeg cLeg = (this as CircularLeg);
            if (cLeg != null)
                cLeg.CreateCircle(ff, maxSequence.ToString());

            maxSequence = m_FirstSide.CreateFeatures(ff, maxSequence, startPoint, lastPoint);

            // Should the end of an alternate face share the end point of the primary face??
            if (m_OtherSide != null)
                maxSequence = m_OtherSide.CreateFeatures(ff, maxSequence, startPoint, lastPoint);

            return maxSequence;
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
        //abstract internal void CreateGeometry(EditingContext ctx, ref IPosition terminal, ref double bearing, double sfac);

        abstract internal bool Rollforward (ref PointFeature insert, PathOperation op,
                                                ref IPosition terminal, ref double bearing, double sfac);

        /// <summary>
        /// Does this leg contain a staggered face (with distances on both sides of the
        /// underlying line)?
        /// </summary>
        internal bool IsStaggered
        {
            get { return (m_OtherSide != null); }
        }

        /// <summary>
        /// The initial definition for this leg (if <see cref="AlternateFace"/> is undefined, the
        /// spans relate to both sides of the leg).
        /// </summary>
        internal LegFace PrimaryFace
        {
            get { return m_FirstSide; }
        }

        /// <summary>
        /// An alternate set of spans for this leg, used to represent staggered property lots.
        /// </summary>
        internal LegFace AlternateFace
        {
            get { return m_OtherSide; }
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
            m_FirstSide.GetFeatures(op, flist);

            if (m_OtherSide != null)
                m_OtherSide.GetFeatures(op, flist);
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

            if (m_FirstSide.IsCreatorOf(feature))
                return true;

            if (m_OtherSide != null && m_OtherSide.IsCreatorOf(feature))
                return true;

            return false;
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
    }
}
