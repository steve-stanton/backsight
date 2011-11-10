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


namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="10-NOV-1999" was="CeArcTrim" />
    /// <summary>
    /// Operation to assign "trim" status to dangling lines.
    /// </summary>
    class TrimLineOperation : Operation
    {
        #region Static

        /// <summary>
        /// Filters a list of features, returning any lines that are suitable
        /// for trimming.
        /// </summary>
        /// <param name="things">The list to validate</param>
        /// <returns>The elements in <paramref name="things"/> that can be trimmed (may
        /// be an empty array)</returns>
        internal static LineFeature[] PreCheck(IEnumerable<ISpatialObject> things)
        {
            List<LineFeature> result = new List<LineFeature>();

            foreach (ISpatialObject so in things)
            {
                LineFeature line = PreCheck(so);
                if (line!=null)
                    result.Add(line);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Checks if an object is acceptable for trimming.
        /// </summary>
        /// <param name="thing">The candidate object.</param>
        /// <returns>The supplied candidate, cast to a line (given that the candidate is
        /// suitable for trimming). Null if the candidate is not a line, or not suitable
        /// for trimming.</returns>
        static LineFeature PreCheck(ISpatialObject thing)
        {
            // It has no be a line
            LineFeature line = (thing as LineFeature);
            if (line==null)
                return null;

            // It has to be topological
            Topology t = line.Topology;
            if (t==null)
                return null;

            // The line should have system-defined topological line
            // sections (if it doesn't that would mean that the complete
            // line dangles).
            SectionTopologyList sectionList = (t as SectionTopologyList);
            if (sectionList==null)
                return null;

            if (sectionList.CanTrim())
                return line;

            return null;
        }

        #endregion

        #region Class data

        /// <summary>
        /// The lines that were selected for trimming.
        /// </summary>
        LineFeature[] m_Lines;

        /// <summary>
        /// Any dangling end points.
        /// </summary>
        /// <remarks>In the original implementation, these were de-activated. The current
        /// implementation just marks these points as trimmed.
        /// <para/>
        /// It may well be "incorrect" to record the points that were dangling, as
        /// this actually signifies the lines that were dangling at the time when the
        /// edit was originally performed. A more correct implementation would derive
        /// the trim points each time the edit is reloaded from the database. The only
        /// reason for not doing so is because special processing would be needed upon
        /// deserialization (topology is not available initially, so the processing would
        /// need to be deferred).
        /// </remarks>
        PointFeature[] m_Points;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TrimLineOperation"/> class
        /// </summary>
        /// <param name="s">The session the operation should be referred to (the session itself
        /// is not modified until the editing operation is saved to the database).</param>
        /// <param name="sequence">The sequence number of the edit within the session (specify 0 if
        /// a new sequence number should be reserved). A non-zero value is specified during
        /// deserialization from the database.</param>
        internal TrimLineOperation(Session s, uint sequence)
            : base(s, sequence)
        {
            m_Lines = null;
            m_Points = null;
        }

        #endregion

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get { return "Trim line"; }
        }

        /// <summary>
        /// Finds the observed length of a line that was created by this operation.
        /// </summary>
        /// <param name="line">The line to find</param>
        /// <returns>Null (always)</returns>
        internal override Distance GetDistance(LineFeature line)
        {
            return null;
        }

        /// <summary>
        /// The features created by this editing operation (an empty array)
        /// </summary>
        internal override Feature[] Features
        {
            get { return new Feature[0]; }
        }

        /// <summary>
        /// The unique identifier for this edit.
        /// </summary>
        internal override EditingActionId EditId
        {
            get { return EditingActionId.Trim; }
        }

        /// <summary>
        /// Rollback this operation (occurs when a user undoes the last edit).
        /// </summary>
        /// <returns>True if operation was rolled back ok</returns>
        internal override bool Undo()
        {
            base.OnRollback();

            // Revert the trim status of the line(s) involved.
            if (m_Lines!=null)
            {
                foreach (LineFeature line in m_Lines)
                    line.IsTrimmed = false;
            }

            // Revert the trim status of the point(s) involved.
            if (m_Points!=null)
            {
                foreach (PointFeature p in m_Points)
                    p.IsTrimmed = false;
            }

            return true;
        }

        /// <summary>
        /// Rollforward this edit in response to some sort of update.
        /// </summary>
        /// <returns>True if operation has been re-executed successfully</returns>
        internal override bool Rollforward()
        {
            throw new NotImplementedException();
            // Nothing to do

            // Rollforward the base class.
            //return base.OnRollforward();
        }

        /// <summary>
        /// Checks whether this operation makes reference to a specific feature.
        /// </summary>
        /// <param name="feat">The feature to check for.</param>
        /// <returns>False (always), since this edit does not add any references
        /// (see <see cref="AddReferences"/>)
        /// </returns>
        /// 
        bool HasReference(Feature feat)
        {
            return false;
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
            // No predecessors (for same reason that DeletionOperation does
            // not return predecessors)? Not sure about this.

            return null;
        }

        /// <summary>
        /// Executes this operation.
        /// </summary>
        /// <param name="lines">The lines to trim (previously validated via a call
        /// to the static <see cref="PreCheck"/> method).</param>
        internal void Execute(LineFeature[] lines)
        {
            m_Lines = lines;

            // Record the dangling end points.
            List<PointFeature> points = new List<PointFeature>(lines.Length);

            foreach (LineFeature line in m_Lines)
            {
                if (line.IsStartDangle())
                {
                    PointFeature start = line.StartPoint;
                    if (!points.Contains(start))
                        points.Add(start);
                }

                if (line.IsEndDangle())
                {
                    PointFeature end = line.EndPoint;
                    if (!points.Contains(end))
                        points.Add(end);
                }
            }

            m_Points = points.ToArray();

            base.Execute(new FeatureFactory(this));
        }

        /// <summary>
        /// Performs data processing that involves creating or retiring spatial features.
        /// Newly created features will not have any definition for their geometry - a
        /// subsequent call to <see cref="CalculateGeometry"/> is needed to to that.
        /// </summary>
        /// <param name="ff">The factory class for generating any spatial features</param>
        /// <remarks>This implementation does nothing. Derived classes that need to are
        /// expected to provide a suitable override.</remarks>
        internal override void ProcessFeatures(FeatureFactory ff)
        {
            // Modify the referenced features...

            foreach (LineFeature line in m_Lines)
                line.IsTrimmed = true;

            foreach (PointFeature point in m_Points)
                point.IsTrimmed = true;
        }

        /// <summary>
        /// The lines that were selected for trimming.
        /// </summary>
        internal LineFeature[] TrimmedLines
        {
            get { return m_Lines; }
            set { m_Lines = value; }
        }

        /// <summary>
        /// Any dangling end points.
        /// </summary>
        internal PointFeature[] TrimPoints
        {
            get { return m_Points; }
            set { m_Points = value; }
        }

        /// <summary>
        /// Obtains the features that are referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        /// <returns>
        /// The referenced features (never null, but may be an empty array).
        /// </returns>
        public override Feature[] GetRequiredFeatures()
        {
            List<Feature> result = new List<Feature>(m_Lines.Length + m_Points.Length);
            result.AddRange(m_Lines);
            result.AddRange(m_Points);
            return result.ToArray();
        }

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public override void WriteData(EditSerializer editSerializer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrimLineOperation"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal TrimLineOperation(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
            throw new NotImplementedException();
        }
    }
}
