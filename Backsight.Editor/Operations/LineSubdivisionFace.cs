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
using System.ComponentModel;
using System.Diagnostics;

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="27-MAR-2002" was="CeArcSubdivisionFace" />
    /// <summary>
    /// The sections that define one face of a subdivided line.
    /// </summary>
    class LineSubdivisionFace : IPossibleList<LineSubdivisionFace>, IXmlContent
    {
        #region Class data

        /// <summary>
        /// The line sections created for this face.
        /// </summary>
        List<MeasuredLineFeature> m_Sections;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a <c>LineSubdivisionFace</c> that holds the definition of
        /// observed distances, but not the corresponding line sections.
        /// </summary>
        /// <param name="distances"></param>
        internal LineSubdivisionFace(List<Distance> distances)
        {
            m_Sections = new List<MeasuredLineFeature>(distances.Count);
            foreach (Distance d in distances)
                m_Sections.Add(new MeasuredLineFeature(null, d));
        }

        #endregion

        #region IPossibleList<LineSubdivisionFace> Members

        [Browsable(false)]
        public int Count
        {
            get { return 1; }
        }

        public LineSubdivisionFace this[int index]
        {
            get
            {
                if (index!=0)
                    throw new ArgumentOutOfRangeException();

                return this;
            }
        }

        public IPossibleList<LineSubdivisionFace> Add(LineSubdivisionFace thing)
        {
            return new BasicList<LineSubdivisionFace>(this, thing);
        }

        public IPossibleList<LineSubdivisionFace> Remove(LineSubdivisionFace thing)
        {
            if (!Object.ReferenceEquals(this, thing))
                throw new ArgumentException();

            return null;
        }

        #endregion

        #region IEnumerable<LineSubdivisionFace> Members

        public IEnumerator<LineSubdivisionFace> GetEnumerator()
        {
            yield return this;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator(); // not this one, the other one
        }

        #endregion

        /// <summary>
        /// The line sections created for this face.
        /// </summary>
        internal MeasuredLineFeature[] Sections
        {
            get { return m_Sections.ToArray(); }
        }

        internal double[] GetAdjustedLengths(ILength length)
        {
            // Stash the observed distances into the results array. Denote
            // any fixed distances by negating the observed distance.
            double[] result = new double[m_Sections.Count];
            int nFix = 0;

            for (int i=0; i<result.Length; i++)
            {
                Distance d = m_Sections[i].ObservedLength;
                double m = d.Meters;

                // Confirm that the distance is valid.
                if (m<=0.0)
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
	        if (nFix==m_Sections.Count)
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
                if (d<0.0)
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
            double factor = (play+diff)/play;

            // Generate adjusted distances.
            for (int i=0; i<da.Length; i++)
            {
                if (da[i]<0.0)
                    da[i] = -da[i];
                else
                    da[i] *= factor;
            }

            return true;
        }
        /// <summary>
        /// Adds references to existing features referenced by this face
        /// </summary>
        /// <param name="op">The operation that makes use of this face</param>
        internal void AddReferences(Operation op)
        {
            foreach (MeasuredLineFeature m in m_Sections)
                m.ObservedLength.AddReferences(op);
        }

        /// <summary>
        /// Rollback this face (occurs when a user undoes the edit containing this face).
        /// </summary>
        /// <param name="op">The editing operation that contains this face</param>
        internal void Undo(LineSubdivisionOperation op)
        {
            // Go through each section we created, marking each one as
            // deleted. Also mark the point features at the start of each
            // section, so long as it was created by this operation (should
            // do nothing for the 1st section).

            foreach (MeasuredLineFeature m in m_Sections)
            {
                LineFeature line = m.Line;
                line.Undo();

                PointFeature point = line.StartPoint;
                if (Object.ReferenceEquals(point.Creator, op))
                    point.Undo();
            }
        }

        /// <summary>
        /// Rollforwards this face in response to some sort of update.
        /// </summary>
        /// <param name="op">The editing operation that contains this face</param>
        internal void Rollforward(LineSubdivisionOperation op)
        {
            // How many distances (it's at least 2).
            int ndist = m_Sections.Count;

            // Adjust the observed distances
            LineFeature parent = op.Parent;
            double[] adjray = GetAdjustedLengths(parent.Length);
            Debug.Assert(adjray.Length==m_Sections.Count);

            double edist = 0.0; // Distance to end of section.

            // Adjust the position of the end of each section (except the last one).
            for (int i=0; i<(adjray.Length-1); i++)
            {
                // Get the distance from the start of the parent line.
                edist += adjray[i];

                // Get a position on the parent that is that distance along the line.
                // It shouldn't fail.
                IPosition to;
                if (!parent.LineGeometry.GetPosition(new Length(edist), out to))
                    throw new RollforwardException(op, "Cannot adjust line section");

                // Move the point at the end of the section
                MeasuredLineFeature section = m_Sections[i];
                section.Line.EndPoint.Move(to);
            }
        }

        /// <summary>
        /// Writes the content of this class. This is called by
        /// <see cref="XmlContentWriter.WriteElement"/>
        /// after the element name and class type (xsi:type) have been written.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public void WriteContent(XmlContentWriter writer)
        {
            writer.WriteArray("SectionArray", "Section", m_Sections.ToArray());
        }
    }
}
