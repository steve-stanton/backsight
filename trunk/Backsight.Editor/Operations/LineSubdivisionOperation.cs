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
using System.Diagnostics;

using Backsight.Geometry;

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="23-OCT-1997" />
    /// <summary>
    /// Operation to subdivide a line.
    /// </summary>
    class LineSubdivisionOperation : Operation
    {
        #region Class data

        /// <summary>
        /// The line that was subdivided.
        /// </summary>
        LineFeature m_Line; // readonly

        /// <summary>
        /// The sections of the subdivided line.
        /// </summary>
        List<MeasuredLineFeature> m_Sections;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor sets everything to null, for use in deserialization
        /// </summary>
        public LineSubdivisionOperation()
        {
            m_Line = null;
            m_Sections = null;
        }

        /// <summary>
        /// Creates a new <c>LineSubdivision</c> for the supplied line.
        /// </summary>
        /// <param name="line">The line that is being subdivided.</param>
        internal LineSubdivisionOperation(LineFeature line)
            : base()
        {
            m_Line = line;
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
        internal void Execute(List<Distance> distances)
        {
            // Must have at least two distances
            if (distances == null)
                throw new ArgumentNullException();

            if (distances.Count < 2)
                throw new ArgumentException();

            m_Sections = new List<MeasuredLineFeature>(distances.Count);
            foreach (Distance d in distances)
                m_Sections.Add(new MeasuredLineFeature(null, d));

            // Adjust the observed distances
            double[] adjray = Adjust();

            // If the active editing layer is derived from the subdivided line's base layer,...
            //CeSubTheme* pSubTheme = GetSubTheme(*m_pArc);

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
        }

        /// <summary>
        /// Adjusts the observed distances for a line subdivision
        /// </summary>
        /// <returns>The adjusted distances (in meters) for each observed distance</returns>
        double[] Adjust()
        {
            ILength length = m_Line.Length;

            // Stash the observed distances into the results array. Denote
            // any fixed distances by negating the observed distance.
            double[] result = new double[m_Sections.Count];
            int nFix = 0;

            for (int i = 0; i < result.Length; i++)
            {
                Distance d = m_Sections[i].ObservedLength;
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
            if (nFix == m_Sections.Count)
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
        bool Adjust(double[] da, double length)
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
                    result.Add(mf.Line);

                    // If the point feature at the end of the section was created
                    // by this op, append that too.
                    PointFeature pf = mf.Line.EndPoint;
                    if (pf.Creator == this)
                        result.Add(pf);
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
        /// Rollforward this edit in response to some sort of update.
        /// </summary>
        /// <returns>True if operation has been re-executed successfully</returns>
        internal override bool Rollforward()
        {
            // Return if this operation has not been marked as changed.
            if (!IsChanged)
                return base.OnRollforward();

            // How many distances (it's at least 2).
            int ndist = m_Sections.Count;

            // Adjust the observed distances
            double[] adjray = Adjust();
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
                section.Line.EndPoint.Move(to);
            }

            // Rollforward the base class.
            return base.OnRollforward();
        }

        internal bool CanCorrect
        {
            get { return true; }
        }

        /// <summary>
        /// Creates a section for this arc subdivision op.
        /// </summary>
        /// <param name="start">The point at the start of the section</param>
        /// <param name="edist">The distance to the end of the section.</param>
        /// <returns>The created section</returns>
        LineFeature MakeSection(PointFeature start, double edist)
        {
            SectionGeometry section = AddSection(start, edist);
            LineFeature newLine = m_Line.MakeSubSection(section, this);
            //MapModel.EditingIndex.Add(newLine);
            return newLine;
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
            CadastralMapModel map = CadastralMapModel.Current;

            // Get the position for the end point.
            LineGeometry parent = m_Line.LineGeometry;
            IPosition end;
            parent.GetPosition(new Length(edist), out end);

            // Add points at these positions (with no ID & default entity). If they
            // did not previously exist, reference them to THIS operation.

            PointFeature ept = (end as PointFeature);
            if (ept==null)
                ept = (map.Index.QueryClosest(end, Length.Zero, SpatialType.Point) as PointFeature);

            if (ept==null)
            {
                ept = map.AddPoint(end, map.DefaultPointType, this);
                ept.SetNextId();
            }

            SectionGeometry section = new SectionGeometry(m_Line, start, ept);
            return section;
        }

        /// <summary>
        /// The line sections associated with this subdivision
        /// </summary>
        internal MeasuredLineFeature[] Sections
        {
            get { return m_Sections.ToArray(); }
        }

        /// <summary>
        /// Writes the content of this class. This is called by
        /// <see cref="XmlContentWriter.WriteElement"/>
        /// after the element name and class type (xsi:type) have been written.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteContent(XmlContentWriter writer)
        {
            writer.WriteFeatureReference("Line", m_Line);
            writer.WriteArray("SectionArray", "Section", m_Sections.ToArray());
        }

        /// <summary>
        /// Loads the content of this class. This is called by
        /// <see cref="XmlContentReader"/> during deserialization from XML (just
        /// after the default constructor has been invoked).
        /// </summary>
        /// <param name="reader">The reading tool</param>
        public override void ReadContent(XmlContentReader reader)
        {
            base.ReadContent(reader);
            m_Line = reader.ReadFeatureByReference<LineFeature>("Line");
            MeasuredLineFeature[] sections = reader.ReadArray<MeasuredLineFeature>("SectionArray", "Section");
            m_Sections = new List<MeasuredLineFeature>(sections);
        }
    }
}
