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

using Backsight.Environment;
using Backsight.Editor.Observations;
using Backsight.Editor.Xml;

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="27-FEB-1998" was="CeAreaSubdivision" />
    /// <summary>
    /// Subdivision of a polygon.
    /// </summary>
    class PolygonSubdivisionOperation : Operation
    {
        #region Class data

        /// <summary>
        /// Any polygon label that was de-activated as a result of the subdivision.
        /// </summary>
        TextFeature m_Label;

        /// <summary>
        /// The lines that were created.
        /// </summary>
        LineFeature[] m_Lines;
 
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for use during deserialization
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        /// <param name="t">The serialized version of this instance</param>
        internal PolygonSubdivisionOperation(Session s, PolygonSubdivisionType t)
            : base(s, t)
        {
            // Pick up any label to deactivate (this won't actually happen until
            // CalculateGeometry is called)

            if (t.DeactivatedLabel == null)
                m_Label = null;
            else
                m_Label = s.MapModel.Find<TextFeature>(t.DeactivatedLabel);

            // Pick up the line segments that were created

            SegmentType[] lines = t.Line;
            m_Lines = new LineFeature[lines.Length];

            for (int i=0; i<lines.Length; i++)
                m_Lines[i] = new LineFeature(this, lines[i]);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PolygonSubdivisionOperation"/> class
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        internal PolygonSubdivisionOperation(Session s)
            : base(s)
        {
        }

        #endregion

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get { return "Polygon subdivision"; }
        }

        /// <summary>
        /// Finds the observed length of a line that was created by this operation.
        /// </summary>
        /// <param name="line">The line to find</param>
        /// <returns>The corresponding distance (null if not found)</returns>
        internal override Distance GetDistance(LineFeature line)
        {
            return null;
        }

        /// <summary>
        /// The features that were created by this operation.
        /// </summary>
        internal override Feature[] Features
        {
            get { return m_Lines; }
        }

        internal override EditingActionId EditId
        {
            get { return EditingActionId.PolygonSubdivision; }
        }

        public override void AddReferences()
        {
            // Nothing to do
        }

        internal override bool Undo()
        {
            base.OnRollback();

            // Mark each created line for undo
            foreach (LineFeature line in m_Lines)
                Rollback(line);

            // If the polygon originally had a label, restore it.
            if (m_Label!=null)
                m_Label.Restore();

            return true;
        }

        /// <summary>
        /// Rollforward this operation.
        /// </summary>
        /// <returns>True on success</returns>
        internal override bool Rollforward()
        {
            // Return if this operation has not been marked as changed.
            if (!IsChanged)
                return base.OnRollforward();

            // Nothing to do

            // Rollforward the base class.
            return base.OnRollforward();
        }

        /// <summary>
        /// Executes this operation.
        /// </summary>
        /// <param name="sub">The polygon subdivision information.</param>
        internal void Execute(PolygonSub sub)
        {
            int numLine = sub.NumLink;
            if (numLine==0)
                throw new Exception("PolygonSubdivisionOperation.Execute - Nothing to add");

            // De-activate any label associated with the polygon we're subdividing.
            Polygon pol = sub.Polygon;
            m_Label = pol.Label;
            if (m_Label!=null)
                m_Label.IsInactive = true;

            // Mark the polygon for deletion
            pol.IsDeleted = true;

            // Get the default entity type for lines.
            CadastralMapModel map = MapModel;
            IEntity ent = map.DefaultLineType;

            // Allocate array to point to the lines we will be creating.
            m_Lines = new LineFeature[numLine];

            // Add lines for each link
            PointFeature start, end;
            for (int i=0; sub.GetLink(i, out start, out end); i++)
            {
                m_Lines[i] = map.AddLine(start, end, ent, this);
            }

            // Peform standard completion steps
            Complete();
        }

        /// <summary>
        /// Returns an object that represents this edit, and that can be serialized using
        /// the <c>XmlSerializer</c> class.
        /// <returns>The serializable version of this edit</returns>
        internal override OperationType GetSerializableEdit()
        {
            PolygonSubdivisionType t = new PolygonSubdivisionType();
            t.Id = this.DataId;

            if (m_Label != null)
                t.DeactivatedLabel = m_Label.DataId;

            SegmentType[] lines = new SegmentType[m_Lines.Length];
            for (int i=0; i<lines.Length; i++)
            {
                lines[i] = (SegmentType)m_Lines[i].GetSerializableLine();
            }

            t.Line = lines;
            return t;
        }

        /// <summary>
        /// Calculates the geometry for any features created by this edit.
        /// </summary>
        public override void CalculateGeometry()
        {
            if (m_Label != null)
                m_Label.Deactivate();
        }

        /// <summary>
        /// Attempts to locate a superseded (inactive) line that was the parent of
        /// a specific line.
        /// </summary>
        /// <param name="line">The line of interest</param>
        /// <returns>Null (always), since this edit doesn't supersede any lines.</returns>
        internal override LineFeature GetPredecessor(LineFeature line)
        {
            return null;
        }
    }
}
