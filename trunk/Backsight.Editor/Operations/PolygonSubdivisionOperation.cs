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
using Backsight.Environment;

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
        /// The lines that were created (all simple line segments).
        /// </summary>
        LineFeature[] m_Lines;
 
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PolygonSubdivisionOperation"/> class.
        /// </summary>
        internal PolygonSubdivisionOperation()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PolygonSubdivisionOperation"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal PolygonSubdivisionOperation(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
            if (editDeserializer.IsNextField(DataField.DeactivatedLabel))
            {
                m_Label = editDeserializer.ReadFeatureRef<TextFeature>(DataField.DeactivatedLabel);
                m_Label.IsInactive = true; // later ?
            }

            m_Lines = editDeserializer.ReadPersistentArray<LineFeature>(DataField.Lines);
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
        /// The features that were created by this operation.
        /// </summary>
        internal override Feature[] Features
        {
            get { return m_Lines; }
        }

        /// <summary>
        /// The lines that were created.
        /// </summary>
        internal LineFeature[] NewLines
        {
            get { return m_Lines; }
        }

        internal override EditingActionId EditId
        {
            get { return EditingActionId.PolygonSubdivision; }
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
        /// Rollforward this edit in response to some sort of update.
        /// </summary>
        /// <returns>True if operation has been re-executed successfully</returns>
        internal override bool Rollforward()
        {
            throw new NotImplementedException();
            /*
            // Return if this operation has not been marked as changed.
            if (!IsChanged)
                return base.OnRollforward();

            // Nothing to do

            // Rollforward the base class.
            return base.OnRollforward();
             */
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

            // If the polygon contains just one label, de-activate it. This covers a "well-behaved" situation,
            // where the label inside the polygon is likely to be redundant after the subdivision (it also
            // conforms to logic used in the past). In a situation where the polygon contains multiple labels,
            // it's less clear whether the labels become redundant or not, so we keep them all.
            Polygon pol = sub.Polygon;
            if (pol.LabelCount == 1)
            {
                m_Label = pol.Label;
                if (m_Label!=null)
                    m_Label.IsInactive = true;
            }

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

        /// <summary>
        /// Obtains the features that are referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        /// <returns>
        /// The referenced features (never null, but may be an empty array).
        /// </returns>
        public override Feature[] GetRequiredFeatures()
        {
            List<Feature> result = new List<Feature>(m_Lines.Length * 2);

            foreach (LineFeature line in m_Lines)
            {
                result.Add(line.StartPoint);
                result.Add(line.EndPoint);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public override void WriteData(EditSerializer editSerializer)
        {
            base.WriteData(editSerializer);

            if (m_Label != null)
                editSerializer.WriteFeatureRef<TextFeature>(DataField.DeactivatedLabel, m_Label);

            editSerializer.WritePersistentArray<LineFeature>(DataField.Lines, m_Lines);
        }
    }
}
