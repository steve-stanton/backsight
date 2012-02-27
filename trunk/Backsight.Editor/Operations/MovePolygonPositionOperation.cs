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
        readonly TextFeature m_Label;

        /// <summary>
        /// The original position of the reference point (null if the old position coincided
        /// with the top-left corner of the text).
        /// </summary>
        PointGeometry m_OldPosition;

        /// <summary>
        /// The revised position of the reference point.
        /// </summary>
        PointGeometry m_NewPosition;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MovePolygonPositionOperation"/> class
        /// </summary>
        /// <param name="label">The polygon label to modify</param>
        internal MovePolygonPositionOperation(TextFeature label)
            : base()
        {
            m_Label = label;
            m_OldPosition = null;
            m_NewPosition = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MovePolygonPositionOperation"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal MovePolygonPositionOperation(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
            ReadData(editDeserializer, out m_Label, out m_NewPosition, out m_OldPosition);
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
        internal override void Undo()
        {
            base.OnRollback();

            // Move the label's reference position back to where it was originally.
            m_Label.SetPolPosition(m_OldPosition);
        }

        /// <summary>
        /// Obtains the features that are referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        /// <returns>The referenced features (never null, but may be an empty array).</returns>
        public override Feature[] GetRequiredFeatures()
        {
            return new Feature[] { m_Label };
        }

        /// <summary>
        /// Performs the data processing associated with this editing operation.
        /// </summary>
        /// <param name="ctx">The context in which the geometry is being calculated.</param>
        internal override void CalculateGeometry(EditingContext ctx)
        {
            // Move the label's reference position
            m_Label.SetPolPosition(m_NewPosition);
        }

        /// <summary>
        /// Executes the move operation.
        /// </summary>
        /// <param name="to">The revised position of the reference point.</param>
        internal void Execute(PointGeometry to)
        {
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

        /// <summary>
        /// The polygon label that was modified
        /// </summary>
        internal TextFeature Label
        {
            get { return m_Label; }
        }

        /// <summary>
        /// The original position of the reference point (null if the old position coincided
        /// with the top-left corner of the text).
        /// </summary>
        internal PointGeometry OldPosition
        {
            get { return m_OldPosition; }
            set { m_OldPosition = value; }
        }

        /// <summary>
        /// The revised position of the reference point.
        /// </summary>
        internal PointGeometry NewPosition
        {
            get { return m_NewPosition; }
            set { m_NewPosition = value; }
        }

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public override void WriteData(EditSerializer editSerializer)
        {
            base.WriteData(editSerializer);

            editSerializer.WriteFeatureRef<TextFeature>(DataField.Label, m_Label);
            editSerializer.WritePointGeometry(DataField.NewX, DataField.NewY, m_NewPosition);

            if (m_OldPosition != null)
                editSerializer.WritePointGeometry(DataField.OldX, DataField.OldY, m_OldPosition);
        }

        /// <summary>
        /// Reads data that was previously written using <see cref="WriteData"/>
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        /// <param name="label">The polygon label that was modified</param>
        /// <param name="newPosition">The revised position of the reference point.</param>
        /// <param name="oldPosition">The original position of the reference point.</param>
        static void ReadData(EditDeserializer editDeserializer, out TextFeature label, out PointGeometry newPosition,
                                out PointGeometry oldPosition)
        {
            label = editDeserializer.ReadFeatureRef<TextFeature>(DataField.Label);
            newPosition = editDeserializer.ReadPointGeometry(DataField.NewX, DataField.NewY);

            if (editDeserializer.IsNextField(DataField.OldX))
                oldPosition = editDeserializer.ReadPointGeometry(DataField.OldX, DataField.OldY);
            else
                oldPosition = null;
        }
    }
}
