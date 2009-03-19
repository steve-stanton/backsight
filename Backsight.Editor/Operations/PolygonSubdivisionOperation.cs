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
        /// Default constructor, for use in deserialization
        /// </summary>
        public PolygonSubdivisionOperation()
        {
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
        /// Writes the attributes of this class.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteAttributes(XmlContentWriter writer)
        {
            base.WriteAttributes(writer);
            writer.WriteFeatureReference("DeactivatedLabel", m_Label);
        }

        /// <summary>
        /// Writes any child elements of this class. This will be called after
        /// all attributes have been written via <see cref="WriteAttributes"/>.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteChildElements(XmlContentWriter writer)
        {
            base.WriteChildElements(writer);

            foreach (LineFeature line in m_Lines)
                writer.WriteElement("Line", line);
        }

        /// <summary>
        /// Defines the attributes of this content
        /// </summary>
        /// <param name="reader">The reading tool</param>
        public override void ReadAttributes(XmlContentReader reader)
        {
            base.ReadAttributes(reader);
            m_Label = reader.ReadFeatureByReference<TextFeature>("DeactivatedLabel");
        }

        /// <summary>
        /// Defines any child content related to this instance. This will be called after
        /// all attributes have been defined via <see cref="ReadAttributes"/>.
        /// </summary>
        /// <param name="reader">The reading tool</param>
        public override void ReadChildElements(XmlContentReader reader)
        {
            base.ReadChildElements(reader);
            m_Lines = reader.ReadArray<LineFeature>("Line");
        }

        /// <summary>
        /// Calculates the geometry for any features created by this edit.
        /// </summary>
        public override void CalculateGeometry()
        {
            // Nothing to do
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
