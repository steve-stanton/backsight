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
using Backsight.Editor.Xml;


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
        List<LineFeature> m_Lines;

        /// <summary>
        /// Any dangling end points.
        /// </summary>
        /// <remarks>In the original implementation, these were de-activated. The current
        /// implementation just marks these points as trimmed.</remarks>
        List<PointFeature> m_Points;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for use during deserialization
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        /// <param name="t">The serialized version of this instance</param>
        internal TrimLineOperation(Session s, TrimLineData t)
            : base(s, t)
        {
            CadastralMapModel mapModel = s.MapModel;

            string[] lineIds = t.Line;
            m_Lines = new List<LineFeature>(lineIds.Length);
            foreach (string lineId in lineIds)
            {
                LineFeature line = mapModel.Find<LineFeature>(lineId);
                Debug.Assert(line!=null);
                m_Lines.Add(line);
            }

            string[] pointIds = t.Point;
            m_Points = new List<PointFeature>(pointIds.Length);
            foreach (string pointId in pointIds)
            {
                PointFeature p = mapModel.Find<PointFeature>(pointId);
                Debug.Assert(p!=null);
                m_Points.Add(p);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrimLineOperation"/> class
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        internal TrimLineOperation(Session s)
            : base(s)
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

            // Restore point features that were de-activated
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
        /// <param name="uc">The context in which editing revisions are being made (not null).
        /// Used to hold a record of any positional changes.</param>
        /// <returns>True if operation has been re-executed successfully</returns>
        internal override bool Rollforward(UpdateContext uc)
        {
            // Nothing to do

            // Rollforward the base class.
            return base.OnRollforward();
        }

        /// <summary>
        /// Adds references to existing features referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        /// <remarks>
        /// This does not do anything. While the op does refer to the lines that it trimmed, this
        /// ends up only setting the trim status in each line. We do NOT cross-reference the feature
        /// to this op (for the same sort of reason as a <see cref="DeletionOperation"/>)
        /// </remarks>
        public override void AddReferences()
        {
            // Do nothing
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
            m_Lines = new List<LineFeature>(lines);

            foreach (LineFeature line in m_Lines)
            {
                // Mark it as trimmed.
                line.IsTrimmed = true;

                if (line.IsStartDangle())
                    AddTrimPoint(line.StartPoint);

                if (line.IsEndDangle())
                    AddTrimPoint(line.EndPoint);
            }

            // Peform standard completion steps
            Complete();
        }

        /// <summary>
        /// Remembers a point at the end of a trimmed section
        /// </summary>
        /// <param name="point"></param>
        void AddTrimPoint(PointFeature point)
        {
            if (m_Points==null)
                m_Points = new List<PointFeature>();

            if (!m_Points.Contains(point))
            {
                m_Points.Add(point);
                point.IsTrimmed = true;
            }
        }

        /// <summary>
        /// Performs the data processing associated with this editing operation.
        /// </summary>
        internal override void RunEdit()
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
            get { return m_Lines.ToArray(); }
        }

        /// <summary>
        /// Any dangling end points.
        /// </summary>
        internal PointFeature[] TrimPoints
        {
            get { return m_Points.ToArray(); }
        }
    }
}
