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

using Backsight.Environment;
using Backsight.Editor.Observations;

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
        SegmentLineFeature[] m_Lines;
 
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for use during deserialization.
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        /// <param name="sequence">The sequence number of the edit within the session (specify 0 if
        /// a new sequence number should be reserved). A non-zero value is specified during
        /// deserialization from the database.</param>
        internal PolygonSubdivisionOperation(Session s, uint sequence)
            : base(s, sequence)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PolygonSubdivisionOperation"/> class
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        internal PolygonSubdivisionOperation(Session s)
            : this(s, 0)
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

        /// <summary>
        /// The lines that were created.
        /// </summary>
        internal SegmentLineFeature[] NewLines
        {
            get { return m_Lines; }
            set { m_Lines = value; }
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
            foreach (SegmentLineFeature line in m_Lines)
                Rollback(line);

            // If the polygon originally had a label, restore it.
            if (m_Label!=null)
                m_Label.Restore();

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
            m_Lines = new SegmentLineFeature[numLine];

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
        /// Performs the data processing associated with this editing operation.
        /// </summary>
        internal override void RunEdit()
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

        /// <summary>
        /// Any polygon label that was de-activated as a result of the subdivision.
        /// </summary>
        internal TextFeature DeactivatedLabel
        {
            get { return m_Label; }
            set { m_Label = value; }
        }
    }
}
