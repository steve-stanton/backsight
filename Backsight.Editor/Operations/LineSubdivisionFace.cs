// <remarks>
// Copyright 2010 - Steve Stanton. This file is part of Backsight
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
using System.Linq;
using System.Collections.Generic;

using Backsight.Editor.Observations;
using System.Diagnostics;


namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="27-MAR-2002" />
    /// <summary>
    /// The sections that define one face of a subdivided line.
    /// </summary>
    class LineSubdivisionFace : IPersistent
    {
        #region Static

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
                if (repeat >= 0)
                {
                    string rest = s.Substring(repeat + 1);

                    if (rest.Length > 0)
                    {
                        nRepeat = int.Parse(rest);
                        if (nRepeat <= 0)
                            throw new Exception("Repeat count cannot be less than or equal to zero");
                    }

                    s = s.Substring(0, repeat);
                }

                // Parse the distance
                Distance d = new Distance(s, defaultEntryUnit);
                if (!d.IsDefined)
                    throw new Exception("Cannot parse distance: " + s);

                // Append distances to results list
                for (int i = 0; i < nRepeat; i++)
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

        #endregion

        #region Class data

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
        List<LineFeature> m_Sections;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LineSubdivisionFace"/> class.
        /// </summary>
        /// <param name="entryString">The data entry string that defines the subdivision sections.</param>
        /// <param name="defaultEntryUnit">The default distance units to use when decoding
        /// the data entry string.</param>
        /// <param name="isEntryFromEnd">Are the distances observed from the end of the line?</param>
        internal LineSubdivisionFace(string entryString, DistanceUnit defaultEntryUnit, bool isEntryFromEnd)
        {
            m_EntryString = entryString;
            m_DefaultEntryUnit = defaultEntryUnit;
            m_IsEntryFromEnd = isEntryFromEnd;
            m_Sections = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineSubdivisionFace"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal LineSubdivisionFace(EditDeserializer editDeserializer)
        {
            ReadData(editDeserializer, out m_EntryString, out m_IsEntryFromEnd, out m_DefaultEntryUnit);

            // The sections are calculated ??
            m_Sections = null;
        }

        #endregion

        /// <summary>
        /// Checks whether this face contains a specific line section.
        /// </summary>
        /// <param name="line">The section to look for</param>
        /// <returns>True if one of the section on this face matches the supplied line.</returns>
        internal bool HasSection(LineFeature line)
        {
            return (m_Sections == null ? false : m_Sections.Contains(line));
        }

        /// <summary>
        /// The data entry string that defines the sections on this face.
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
        /// Converts the data entry string into the corresponding observations.
        /// </summary>
        /// <returns>The distances that correspond to the entry string, starting at the
        /// beginning of the line</returns>
        internal Distance[] GetDistances()
        {
            return GetDistances(m_EntryString, m_DefaultEntryUnit, m_IsEntryFromEnd);
        }

        /// <summary>
        /// Creates line sections along this face.
        /// </summary>
        /// <param name="parentLine">The line that is being subdivided</param>
        /// <param name="ff">Factory for producing new features (the important thing
        /// is the editing operation that's involved).</param>
        internal void CreateSections(LineFeature parentLine, FeatureFactory ff)
        {
            Distance[] distances = GetDistances();

            // Must have at least two distances
            if (distances == null)
                throw new ArgumentNullException();

            if (distances.Length < 2)
                throw new ArgumentException();

            m_Sections = new List<LineFeature>(distances.Length);
            PointFeature start = parentLine.StartPoint;
            InternalIdValue item = new InternalIdValue(ff.Creator.DataId);

            for (int i = 0; i < distances.Length; i++)
            {
                PointFeature end;
                if (i == distances.Length - 1)
                    end = parentLine.EndPoint;
                else
                {
                    item.ItemSequence++;
                    end = ff.CreatePointFeature(item.ToString());
                }

                item.ItemSequence++;
                LineFeature line = ff.CreateSection(item.ToString(), parentLine, start, end);
                line.ObservedLength = distances[i];
                m_Sections.Add(line);
                start = end;
            }
        }

        /// <summary>
        /// Performs the data processing associated with this face.
        /// </summary>
        /// <param name="parentLine">The line that is being subdivided</param>
        /// <param name="ctx">The context in which the geometry is being calculated.</param>
        internal void CalculateGeometry(LineFeature parentLine, EditingContext ctx)
        {
            // Get adjusted lengths for each section
            Distance[] distances = new Distance[m_Sections.Count];
            for (int i = 0; i < distances.Length; i++)
                distances[i] = m_Sections[i].ObservedLength;
            double[] adjray = GetAdjustedLengths(parentLine, distances);

            double edist = 0.0;		// Distance to end of section.
            PointFeature start = parentLine.StartPoint;
            LineGeometry lineGeom = parentLine.LineGeometry;

            for (int i = 0; i < adjray.Length; i++)
            {
                // Calculate the position at the end of the span
                edist += adjray[i];
                IPosition to;
                if (!lineGeom.GetPosition(new Length(edist), out to))
                    throw new Exception("Cannot adjust line section");

                // Get the point feature at the end of the span
                LineFeature line = m_Sections[i];
                PointFeature end = line.EndPoint;

                // Assign the calculated position so long as we're not at
                // the end of the line
                if (end != parentLine.EndPoint)
                    end.ApplyPointGeometry(ctx, PointGeometry.Create(to));

                // The end of the current span is the start of the next one
                start = end;
            }
        }

        /// <summary>
        /// The new features created along this face.
        /// </summary>
        /// <param name="parentLine">The line that was subdivided</param>
        /// <returns>The new features created to represent the sections of this face.</returns>
        internal Feature[] GetNewFeatures(LineFeature parentLine)
        {
            if (m_Sections == null)
                return new Feature[0];

            Operation parentEdit = parentLine.Creator;
            List<Feature> result = new List<Feature>();

            foreach (LineFeature line in m_Sections)
            {
                // Append point feature at the end of the section (so long as it's not
                // the end of the subdivided line).
                PointFeature pf = line.EndPoint;
                if (pf.Creator != parentEdit)
                    result.Add(pf);

                result.Add(line);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Attempts to locate the line section with a specific ID.
        /// </summary>
        /// <param name="dataId">The ID to look for</param>
        /// <returns>The observation for the corresponding section (null if not found)</returns>
        internal LineFeature FindObservedLine(string dataId)
        {
            LineFeature result = null;

            if (m_Sections != null)
            {
                result = m_Sections.Find(delegate(LineFeature t)
                {
                    return (t.DataId == dataId);
                });
            }

            return result;
        }

        /// <summary>
        /// The line sections associated with this face
        /// </summary>
        internal LineFeature[] Sections
        {
            get { return m_Sections.ToArray(); }
        }

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public void WriteData(EditSerializer editSerializer)
        {
            editSerializer.WriteString(DataField.EntryString, m_EntryString);
            editSerializer.WriteBool(DataField.EntryFromEnd, m_IsEntryFromEnd);
            editSerializer.WriteDistanceUnit(DataField.DefaultEntryUnit, m_DefaultEntryUnit);
        }

        /// <summary>
        /// Reads data that was previously written using <see cref="WriteData"/>
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        /// <param name="entryString">The data entry string that defines the subdivision sections.</param>
        /// <param name="entryFromEnd">Are the distances observed from the end of the line?</param>
        /// <param name="defaultEntryUnit">The default distance units to use when decoding the data entry string.</param>
        static void ReadData(EditDeserializer editDeserializer, out string entryString, out bool entryFromEnd, out DistanceUnit defaultEntryUnit)
        {
            entryString = editDeserializer.ReadString(DataField.EntryString);
            entryFromEnd = editDeserializer.ReadBool(DataField.EntryFromEnd);
            defaultEntryUnit = editDeserializer.ReadDistanceUnit(DataField.DefaultEntryUnit);
        }

        internal void ExchangeData(UpdateItem item)
        {
            // Note the distances currently attached to the line sections
            Distance[] lineDistances = GetCurrentDistances();

            // Apply the new distances and/or flip the annotation side
            Distance[] newDistances = (Distance[])item.Value;
            Debug.Assert(newDistances.Length == m_Sections.Count);
            Distance[] oldDistances = new Distance[newDistances.Length];

            for (int i = 0; i < newDistances.Length; i++)
            {
                // Remember the current distance for the section, noting whether the annotation is being
                // flipped to the other side (it's possible it has previously been switched).
                LineFeature section = m_Sections[i];
                oldDistances[i] = new Distance(section.ObservedLength);
                oldDistances[i].IsAnnotationFlipped = newDistances[i].IsAnnotationFlipped;

                // Attach the updated length to the line and optionally flip the side the annotation will appear on.
                Distance d = new Distance(newDistances[i]);
                section.ObservedLength = d;

                if (newDistances[i].IsAnnotationFlipped)
                    d.IsAnnotationFlipped = true; // check this for re-flips
            }

            item.Value = oldDistances;
        }

        Distance[] GetCurrentDistances()
        {
            Distance[] result = new Distance[m_Sections.Count];

            for (int i = 0; i < result.Length; i++)
                result[i] = m_Sections[i].ObservedLength;

            return result;
        }
    }
}
