// <remarks>
// Copyright 2009 - Steve Stanton. This file is part of Backsight
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

using Backsight.Editor.Observations;


namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="21-JAN-2009"/>
    /// <summary>
    /// Edit to move the reference point for a polygon label
    /// </summary>
    class MovePolygonPositionOperation : Operation
    {
        #region Class data

        /// <summary>
        /// The polygon label that was modified
        /// </summary>
        TextFeature m_Label;

        /// <summary>
        /// The original position of the reference point (null if the old position coincided
        /// with the top-left corner of the text)
        /// </summary>
        PointGeometry m_OldPosition;

        /// <summary>
        /// The revised position of the reference point.
        /// </summary>
        PointGeometry m_NewPosition;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>MovePolygonPositionOperation</c>
        /// </summary>
        public MovePolygonPositionOperation()
        {
            m_Label = null;
            m_OldPosition = null;
            m_NewPosition = null;
        }

        #endregion

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get { return "Move polygon reference position"; }
        }

        /// <summary>
        /// Finds the observed length of a line that was created by this operation.
        /// </summary>
        /// <param name="line">The line to find</param>
        /// <returns>
        /// The observed length of the line (null if this operation doesn't
        /// reference the specified line)
        /// </returns>
        internal override Distance GetDistance(LineFeature line)
        {
            return null;
        }

        /// <summary>
        /// The features created by this editing operation - an empty array
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
            get { return EditingActionId.MovePolygonPosition; }
        }

        /// <summary>
        /// Rollback this operation (occurs when a user undoes the last edit).
        /// </summary>
        /// <returns>True if operation was rolled back ok</returns>
        internal override bool Undo()
        {
            base.OnRollback();

            // Move the label's reference position back to where it was originally.
            m_Label.SetPolPosition(m_OldPosition);

            return true;
        }

        /// <summary>
        /// Rollforward this edit in response to some sort of update.
        /// </summary>
        /// <returns>
        /// True if operation has been re-executed successfully
        /// </returns>
        internal override bool Rollforward()
        {
            // Return if this operation has not been marked as changed.
            if (!IsChanged)
                return base.OnRollforward();

            // Nothing to do... I wonder if the enclosing polygon
            // needs to be recalculated (what if the reference position
            // is real close to a line that has moved)?

            // Rollforward the base class.
            return base.OnRollforward();
        }

        /// <summary>
        /// Adds references to existing features referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// <para/>
        /// This is called by the <see cref="Complete"/> method, to ensure
        /// that the referenced features are cross-referenced to the editing operations
        /// that depend on them.
        /// </summary>
        public override void AddReferences()
        {
            // Do nothing -- although the edit refers to the text feature that's
            // being moved, that reference doesn't have a bearing on any new feature
        }

        /// <summary>
        /// Writes the content of this class. This is called by
        /// <see cref="XmlContentWriter.WriteElement"/>
        /// after the element name and class type (xsi:type) have been written.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteContent(XmlContentWriter writer)
        {
            writer.WriteFeatureReference("Label", m_Label);

            if (m_OldPosition!=null)
            {
                writer.WriteLong("OldX", m_OldPosition.Easting.Microns);
                writer.WriteLong("OldY", m_OldPosition.Northing.Microns);
            }

            writer.WriteLong("NewX", m_NewPosition.Easting.Microns);
            writer.WriteLong("NewY", m_NewPosition.Northing.Microns);
        }

        /// <summary>
        /// Writes the attributes of this class.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteAttributes(XmlContentWriter writer)
        {
            base.WriteAttributes(writer);

            writer.WriteFeatureReference("Label", m_Label);

            if (m_OldPosition!=null)
            {
                writer.WriteLong("OldX", m_OldPosition.Easting.Microns);
                writer.WriteLong("OldY", m_OldPosition.Northing.Microns);
            }

            writer.WriteLong("NewX", m_NewPosition.Easting.Microns);
            writer.WriteLong("NewY", m_NewPosition.Northing.Microns);
        }

        /// <summary>
        /// Writes any child elements of this class. This will be called after
        /// all attributes have been written via <see cref="WriteAttributes"/>.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteChildElements(XmlContentWriter writer)
        {
            base.WriteChildElements(writer);
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
            m_Label = reader.ReadFeatureByReference<TextFeature>("Label");

            long x = reader.ReadLong("OldX");
            long y = reader.ReadLong("OldY");
            if (x==0 && y==0)
                m_OldPosition = null;
            else
                m_OldPosition = new PointGeometry(x, y);

            x = reader.ReadLong("NewX");
            y = reader.ReadLong("NewY");
            m_NewPosition = new PointGeometry(x, y);
        }

        /// <summary>
        /// Calculates the geometry for any features created by this edit.
        /// </summary>
        public override void CalculateGeometry()
        {
            // Nothing to do
        }

        /// <summary>
        /// Executes the move operation.
        /// </summary>
        /// <param name="label">The polygon label to modify</param>
        /// <param name="to">The revised position of the reference point.</param>
        internal void Execute(TextFeature label, PointGeometry to)
        {
            // Remember the label being moved.
            m_Label = label;

            // Remember the old and new positions.
            IPointGeometry txtPos = m_Label.Position;
            IPointGeometry polPos = m_Label.GetPolPosition();
            if (txtPos.IsCoincident(polPos))
                m_OldPosition = null;
            else
                m_OldPosition = PointGeometry.Create(polPos);

            m_NewPosition = to;

            // Move the reference position (and rework any enclosing polygon)
            m_Label.SetPolPosition(m_NewPosition);

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
    }
}
