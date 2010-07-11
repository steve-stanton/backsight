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

using Backsight.Editor.Observations;

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="23-OCT-1997" />
    /// <summary>
    /// Operation to subdivide a line.
    /// </summary>
    class LineSubdivisionOperation : Operation, IRecallable, IRevisable
    {
        #region Class data

        /// <summary>
        /// The line that was subdivided.
        /// </summary>
        readonly LineFeature m_Line;

        /// <summary>
        /// The data entry string that defines the subdivision sections.
        /// </summary>
        readonly string m_EntryString;

        /// <summary>
        /// The default distance units to use when decoding the data entry string.
        /// </summary>
        readonly DistanceUnit m_DefaultEntryUnit;

        /// <summary>
        /// Are the distances observed from the end of the line?
        /// </summary>
        readonly bool m_IsEntryFromEnd;

        /// <summary>
        /// The sections of the subdivided line.
        /// </summary>
        List<MeasuredLineFeature> m_Sections;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LineSubdivisionOperation"/> class.
        /// </summary>
        /// <param name="session">The session the new instance should be added to</param>
        /// <param name="sequence">The sequence number of the edit within the session (specify 0 if
        /// a new sequence number should be reserved). A non-zero value is specified during
        /// deserialization from the database.</param>
        /// <param name="line">The line that is being subdivided.</param>
        /// <param name="entryString">The data entry string that defines the subdivision sections.</param>
        /// <param name="defaultEntryUnit">The default distance units to use when decoding
        /// the data entry string.</param>
        /// <param name="isEntryFromEnd">Are the distances observed from the end of the line?</param>
        internal LineSubdivisionOperation(Session session, uint sequence, LineFeature line,
                                            string entryString, DistanceUnit defaultEntryUnit, bool isEntryFromEnd)
            : base(session, sequence)
        {
            m_Line = line;
            m_EntryString = entryString;
            m_DefaultEntryUnit = defaultEntryUnit;
            m_IsEntryFromEnd = isEntryFromEnd;
            m_Sections = null;
        }

        #endregion

        /// <summary>
        /// The line that was subdivided.
        /// </summary>
        internal LineFeature Parent
        {
            get { return m_Line; }
        }

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get { return "Line subdivision"; }
        }

        /// <summary>
        /// Execute line subdivision.
        /// </summary>
        internal void Execute()
        {
            //Distance[] distances = GetDistances(m_EntryString, m_DefaultEntryUnit, m_IsEntryFromEnd);

            //// Must have at least two distances
            //if (distances == null)
            //    throw new ArgumentNullException();

            //if (distances.Length < 2)
            //    throw new ArgumentException();

            FeatureFactory ff = new FeatureFactory(this);

            // There's no need to actually define anything in the factory as far as points are
            // concerned - the ProcessFeatures method will end up using the default entity type
            // for points, and assign new feature IDs if necessary.

            // Same deal for lines. In that caee, ProcessFeatures creates sections that are
            // like the subdivided line.

            base.Execute(ff);

            ///////////////
            /*
            m_Sections = new List<MeasuredLineFeature>(distances.Length);
            foreach (Distance d in distances)
                m_Sections.Add(new MeasuredLineFeature(null, d));

            // Adjust the observed distances
            double[] adjray = GetAdjustedLengths(m_Line, distances);

            // Create line sections
            double edist = 0.0;		// Distance to end of section.
            PointFeature start = m_Line.StartPoint;

            for (int i = 0; i < adjray.Length; i++)
            {
                edist += adjray[i];
                m_Sections[i].Line = MakeSection(start, edist);
                start = m_Sections[i].Line.EndPoint;
            }

            // De-activate the parent line
            m_Line.IsInactive = true;

            // Peform standard completion steps
            Complete();
             */
        }

        /// <summary>
        /// Performs data processing that involves creating or retiring spatial features.
        /// Newly created features will not have any definition for their geometry - a
        /// subsequent call to <see cref="CreateGeometry"/> is needed to to that.
        /// </summary>
        /// <param name="ff">The factory class for generating any spatial features</param>
        internal override void ProcessFeatures(FeatureFactory ff)
        {
            Distance[] distances = GetDistances(m_EntryString, m_DefaultEntryUnit, m_IsEntryFromEnd);

            // Must have at least two distances
            if (distances == null)
                throw new ArgumentNullException();

            if (distances.Length < 2)
                throw new ArgumentException();

            m_Sections = new List<MeasuredLineFeature>(distances.Length);
            PointFeature start = m_Line.StartPoint;
            InternalIdValue item = new InternalIdValue(this.DataId);

            for (int i=0; i<distances.Length; i++)
            {
                PointFeature end;
                if (i == distances.Length-1)
                    end = m_Line.EndPoint;
                else
                {
                    item.ItemSequence++;
                    end = ff.CreatePointFeature(item.ToString());
                }

                item.ItemSequence++;
                SectionLineFeature line = ff.CreateSection(item.ToString(), m_Line, start, end);
                m_Sections.Add(new MeasuredLineFeature(line, distances[i]));
                start = end;
            }

            // Retire the original line
            ff.DeactivateLine(m_Line);
        }

        /// <summary>
        /// Performs the data processing associated with this editing operation.
        /// </summary>
        internal override void CalculateGeometry()
        {
            // Get adjusted lengths for each section
            Distance[] distances = new Distance[m_Sections.Count];
            for (int i = 0; i < distances.Length; i++)
                distances[i] = m_Sections[i].ObservedLength;
            double[] adjray = GetAdjustedLengths(m_Line, distances);

            double edist = 0.0;		// Distance to end of section.
            PointFeature start = m_Line.StartPoint;
            LineGeometry lineGeom = m_Line.LineGeometry;

            for (int i=0; i<adjray.Length; i++)
            {
                // Calculate the position at the end of the span
                edist += adjray[i];
                IPosition to;
                if (!lineGeom.GetPosition(new Length(edist), out to))
                    throw new Exception("Cannot adjust line section");

                // Get the point feature at the end of the span
                MeasuredLineFeature mf = m_Sections[i];
                PointFeature end = mf.Line.EndPoint;

                // Assign the calculated position so long as we're not at
                // the end of the line
                if (end != m_Line.EndPoint)
                    end.PointGeometry = PointGeometry.Create(to);

                // The end of the current span is the start of the next one
                start = end;
            }
        }

        /// <summary>
        /// Adjusts a set of distances so that they fit the length of a line.
        /// </summary>
        /// <param name="line">The line defining the required total distance</param>
        /// <param name="distances">The observed distances</param>
        /// <returns>The adjusted lengths (in meters).</returns>
        /// <exception cref="Exception">If any observed distance is less than or equal to zero</exception>
        /// <exception cref="InvalidOperationException">If all distances are tagged as fixed</exception>
        internal static double[] GetAdjustedLengths(LineFeature line, Distance[] distances)
        {
            ILength length = line.Length;

            // Stash the observed distances into the results array. Denote
            // any fixed distances by negating the observed distance.
            double[] result = new double[distances.Length];
            int nFix = 0;

            for (int i = 0; i < result.Length; i++)
            {
                Distance d = distances[i];
                double m = d.Meters;

                // Confirm that the distance is valid.
                if (m <= 0.0)
                    throw new Exception("Observed distances must be greater than 0.");

                // If distance is fixed, hold it as a negated value.
                if (d.IsFixed)
                {
                    nFix++;
                    m = -m;
                }

                result[i] = m;
            }

            // Confirm we have at least one un-fixed distance.
            if (nFix == distances.Length)
                throw new InvalidOperationException("All distances have been fixed. Cannot adjust.");

            // Do the adjustment.
            if (!Adjust(result, length.Meters))
                throw new Exception("Unable to adjust observed lengths");

            return result;
        }

        /// <summary>
        /// Adjusts a set of distances so that they fit a specific length.
        /// </summary>
        /// <param name="da">Array of input distances (in the same units as <paramref name="length"/>).
        /// Any fixed distances in the list must be denoted using a negated value.</param>
        /// <param name="length">The distance to fit to</param>
        /// <returns>True if distances adjusted ok. False if all distances were fixed.</returns>
        static bool Adjust(double[] da, double length)
        {
            // Accumulate the total observed distance, and the total
            // fixed distance.

            double totobs = 0.0;
            double totfix = 0.0;

            foreach (double d in da)
            {
                if (d < 0.0)
                {
                    totfix -= d; // i.e. add
                    totobs -= d;
                }
                else
                    totobs += d;
            }

            double diff = length - totobs;	// How much are we out?
            double play = totobs - totfix;	// How much do we have to play with?

            // Confirm we have something to play with.
            if (play < Double.Epsilon)
                return false;

            // Calculate the adjustment factor
            double factor = (play + diff) / play;

            // Generate adjusted distances.
            for (int i = 0; i < da.Length; i++)
            {
                if (da[i] < 0.0)
                    da[i] = -da[i];
                else
                    da[i] *= factor;
            }

            return true;
        }

        /// <summary>
        /// Finds the observed length of a line that was created by this operation.
        /// </summary>
        /// <param name="line">The line to find</param>
        /// <returns>The corresponding distance (null if not found)</returns>
        internal override Distance GetDistance(LineFeature line)
        {
            if (line==null || m_Sections==null)
                return null;

            foreach (MeasuredLineFeature mf in m_Sections)
            {
                if (Object.ReferenceEquals(mf.Line, line))
                    return mf.ObservedLength;
            }

            return null;
        }

        /// <summary>
        /// The features that were created by this operation.
        /// </summary>
        internal override Feature[] Features
        {
            get
            {
                if (m_Sections==null)
                    return new Feature[0];

                List<Feature> result = new List<Feature>(m_Sections.Count);

                foreach (MeasuredLineFeature mf in m_Sections)
                {
                    // Append point feature at the end of the section (so long as it's not
                    // the end of the subdivided line).
                    PointFeature pf = mf.Line.EndPoint;
                    if (pf.Creator == this)
                        result.Add(pf);

                    result.Add(mf.Line);
                }

                return result.ToArray();
            }
        }

        internal override EditingActionId EditId
        {
            get { return EditingActionId.LineSubdivision; }
        }

        public override void AddReferences()
        {
            m_Line.AddOp(this);

            foreach (MeasuredLineFeature m in m_Sections)
                m.ObservedLength.AddReferences(this);
        }

        /// <summary>
        /// Rollback this operation (occurs when a user undoes the last edit).
        /// </summary>
        /// <returns>True if operation was rolled back ok</returns>
        internal override bool Undo()
        {
            base.OnRollback();

            m_Line.CutOp(this);

            // Go through each section we created, marking each one as
            // deleted. Also mark the point features at the start of each
            // section, so long as it was created by this operation (should
            // do nothing for the 1st section).

            foreach (MeasuredLineFeature m in m_Sections)
            {
                LineFeature line = m.Line;
                line.Undo();

                PointFeature point = line.StartPoint;
                if (Object.ReferenceEquals(point.Creator, this))
                    point.Undo();
            }

            // Restore the original line
            m_Line.Restore();
            return true;
        }

        /// <summary>
        /// Obtains the distance observations associated with this operation.
        /// </summary>
        /// <returns>The distances attached to each line section</returns>
        List<Distance> GetDistances()
        {
            if (m_Sections==null)
                return new List<Distance>();

            List<Distance> result = new List<Distance>(m_Sections.Count);

            foreach (MeasuredLineFeature mf in m_Sections)
                result.Add(mf.ObservedLength);

            return result;
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

            // How many distances (it's at least 2).
            int ndist = m_Sections.Count;

            // Adjust the observed distances
            List<Distance> distances = GetDistances();
            double[] adjray = GetAdjustedLengths(m_Line, distances.ToArray());
            Debug.Assert(adjray.Length == m_Sections.Count);

            double edist = 0.0; // Distance to end of section.

            // Adjust the position of the end of each section (except the last one).
            for (int i = 0; i < (adjray.Length - 1); i++)
            {
                // Get the distance from the start of the parent line.
                edist += adjray[i];

                // Get a position on the parent that is that distance along the line.
                // It shouldn't fail.
                IPosition to;
                if (!m_Line.LineGeometry.GetPosition(new Length(edist), out to))
                    throw new RollforwardException(this, "Cannot adjust line section");

                // Move the point at the end of the section
                MeasuredLineFeature section = m_Sections[i];
                section.Line.EndPoint.MovePoint(uc, to);
            }

            // Rollforward the base class.
            return base.OnRollforward();
        }

        /// <summary>
        /// Creates a section for this subdivision op.
        /// </summary>
        /// <param name="start">The point at the start of the section</param>
        /// <param name="edist">The distance to the end of the section.</param>
        /// <returns>The created section</returns>
        SectionLineFeature MakeSection(PointFeature start, double edist)
        {
            SectionGeometry section = AddSection(start, edist);
            uint ss = Session.ReserveNextItem();
            return m_Line.MakeSubSection(this, ss, section);
        }

        /// <summary>
        /// Adds a line section to the map. This adds the geometry for the section,
        /// together with terminal points, but NOT the line feature.
        /// 
        /// The caller is responsible for associating the operation with the section,
        /// and the parent line with the operation.
        /// </summary>
        /// <param name="start">The point at the start of the section</param>
        /// <param name="edist">Distance from the start of the parent line to the end
        /// of the section.</param>
        /// <returns>The new section.</returns>
        SectionGeometry AddSection(PointFeature start, double edist)
        {
            PointFeature ept = AddSectionEndPoint(start, edist);
            SectionGeometry section = new SectionGeometry(m_Line, start, ept);
            return section;
        }

        /// <summary>
        /// Creates the point feature at the end of a subdivision section.
        /// </summary>
        /// <param name="start">The point at the start of the section</param>
        /// <param name="edist">Distance from the start of the parent line to the end
        /// of the section.</param>
        /// <returns>The point at the end of the section (may be a previously
        /// existing point)</returns>
        PointFeature AddSectionEndPoint(PointFeature start, double edist)
        {
            CadastralMapModel map = CadastralMapModel.Current;

            // Get the position for the end point.
            LineGeometry parent = m_Line.LineGeometry;
            IPosition end;
            parent.GetPosition(new Length(edist), out end);

            // Add points at these positions (with no ID & default entity). If they
            // did not previously exist, reference them to THIS operation.

            PointFeature ept = (end as PointFeature);
            if (ept == null)
                ept = (map.Index.QueryClosest(end, Length.Zero, SpatialType.Point) as PointFeature);

            if (ept == null)
            {
                ept = map.AddPoint(end, map.DefaultPointType, this);
                ept.SetNextId();
            }

            return ept;
        }

        /// <summary>
        /// The line sections associated with this subdivision
        /// </summary>
        internal MeasuredLineFeature[] Sections
        {
            get { return m_Sections.ToArray(); }
            //set { m_Sections = new List<MeasuredLineFeature>(value); }
        }

        /// <summary>
        /// Attempts to locate a superseded (inactive) line that was the parent of
        /// a specific line.
        /// </summary>
        /// <param name="line">The line of interest</param>
        /// <returns>Null (always), since this edit doesn't supersede any lines.</returns>
        internal override LineFeature GetPredecessor(LineFeature line)
        {
            foreach (MeasuredLineFeature mlf in m_Sections)
            {
                if (Object.ReferenceEquals(mlf.Line, line))
                    return m_Line;
            }

            return null;
            /*
	const INT4 nFace = GetNumFace();

	for ( INT4 i=0; i<nFace; i++ )
	{
		const CeObjectList* const pS = GetSectionList(i);
		if ( pS->IsReferredTo(&arc) ) return m_pArc;
	}

	return 0;
             */
        }

        /// <summary>
        /// The data entry string that defines the connection path.
        /// </summary>
        internal string EntryString
        {
            get { return m_EntryString; }
        }

        /// <summary>
        /// Are the distances observed from the end of the line?
        /// </summary>
        internal bool EntryFromEnd
        {
            get { return m_IsEntryFromEnd; }
        }

        /// <summary>
        /// The default distance units to use when decoding the data entry string.
        /// </summary>
        internal DistanceUnit EntryUnit
        {
            get { return m_DefaultEntryUnit; }
        }

        /// <summary>
        /// Converts a data entry string into the corresponding observations.
        /// </summary>
        /// <param name="entryString">The data entry string</param>
        ///	<param name="defaultEntryUnit">The default units</param>
        /// <param name="isEntryFromEnd">Are the distances observed from the end of the line?</param>
        /// <returns>The distances that correspond to the entry string, starting at the
        /// beginning of the line</returns>
        internal static Distance[] GetDistances(string entryString, DistanceUnit defaultEntryUnit,
                                                    bool isEntryFromEnd)
        {
            string[] items = entryString.Split(' ');
            List<Distance> result = new List<Distance>(items.Length);

            foreach (string t in items)
            {
                // Hold seperate reference, since may attempt to change foreach iterator variable below
                string s = t;

                // Strip out any repeat count
                int nRepeat = 1;
                int repeat = s.IndexOf('*');
                if (repeat>=0)
                {
                    string rest = s.Substring(repeat+1);

                    if (rest.Length > 0)
                    {
                        nRepeat = int.Parse(rest);
                        if (nRepeat<=0)
                            throw new Exception("Repeat count cannot be less than or equal to zero");
                    }

                    s = s.Substring(0, repeat);
                }

                // Parse the distance
                Distance d = new Distance(s, defaultEntryUnit);
                if (!d.IsDefined)
                    throw new Exception("Cannot parse distance: "+s);

                // Append distances to results list
                for (int i=0; i<nRepeat; i++)
                    result.Add(d);
            }

            // Reverse the distances if necessary
            if (isEntryFromEnd)
            {
                for (int i = 0, j = result.Count - 1; i < j; i++, j--)
                {
                    Distance tmp = result[i];
                    result[i] = result[j];
                    result[j] = tmp;
                }
            }

            return result.ToArray();
        }
    }
}
