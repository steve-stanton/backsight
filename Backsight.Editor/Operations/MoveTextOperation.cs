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

using Backsight.Geometry;
using Backsight.Editor.Observations;

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="12-DEC-1997" was="CeMoveLabel" />
    /// <summary>
    /// Edit to move an item of text to a new position.
    /// </summary>
    class MoveTextOperation : Operation
    {
        #region Class data

        /// <summary>
        /// The feature that was moved.
        /// </summary>
        readonly TextFeature m_Text;

        /// <summary>
        /// Where the text used to be.
        /// </summary>
        PointGeometry m_OldPosition;

        /// <summary>
        /// The old reference position (null if its identical to m_OldPosition)
        /// </summary>
        PointGeometry m_OldPolPosition;

        /// <summary>
        /// Where the text was moved to. This doubles as the new polygon reference position.
        /// </summary>
        PointGeometry m_NewPosition;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveTextOperation"/> class
        /// </summary>
        /// <param name="text">The text to be moved</param>
        internal MoveTextOperation(TextFeature text)
            : base()
        {
            m_Text = text;
            m_OldPosition = null;
            m_OldPolPosition = null;
            m_NewPosition = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveTextOperation"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal MoveTextOperation(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
            ReadData(editDeserializer, out m_Text, out m_OldPosition, out m_NewPosition, out m_OldPolPosition);
        }

        #endregion

        /// <summary>
        /// The feature that was moved.
        /// </summary>
        internal TextFeature MovedText // was GetpLabel
        {
            get { return m_Text; }
        }

        /// <summary>
        /// Where the text used to be.
        /// </summary>
        internal PointGeometry OldPosition
        {
            get { return m_OldPosition; }
            set { m_OldPosition = value; }
        }

        /// <summary>
        /// The old reference position (null if its identical to m_OldPosition)
        /// </summary>
        internal PointGeometry OldPolPosition
        {
            get { return m_OldPolPosition; }
            set { m_OldPolPosition = value; }
        }

        /// <summary>
        /// Where the text was moved to. This doubles as the new polygon reference position.
        /// </summary>
        internal PointGeometry NewPosition
        {
            get { return m_NewPosition; }
            set { m_NewPosition = value; }
        }

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get { return "Move text"; }
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
            get { return EditingActionId.MoveLabel; }
        }

        /// <summary>
        /// Rollback this operation (occurs when a user undoes the last edit).
        /// </summary>
        /// <returns>True if operation was rolled back ok</returns>
        internal override bool Undo()
        {
            base.OnRollback();

            // Move the label's text back to where it was originally.
            m_Text.Move(m_OldPosition);

            // Move polygon reference position too if its defined
            if (m_OldPolPosition!=null)
                m_Text.SetPolPosition(m_OldPolPosition);
            else
                m_Text.RecalculateEnclosingPolygon();

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

            // Nothing to do?

            // Rollforward the base class.
            return base.OnRollforward();
             */
        }

        /// <summary>
        /// Checks whether this operation makes reference to a specific feature.
        /// </summary>
        /// <param name="feat">The feature to check for.</param>
        /// <returns>False (always), since this edit doesn't depend on anything</returns>
        bool HasReference(Feature feat)
        {
            return false;
        }

        /// <summary>
        /// Performs the data processing associated with this editing operation.
        /// </summary>
        /// <param name="ctx">The context in which the geometry is being calculated.</param>
        internal override void CalculateGeometry(EditingContext ctx)
        {
            // Ensure the text has been moved to the revised position. This method
            // is used only while deserializing from the database, so there should
            // be no need to re-calculate enclosing polygon 
            m_Text.Move(m_NewPosition);
        }

        /// <summary>
        /// Attempts to locate a superseded (inactive) line that was the parent of
        /// a specific line.
        /// </summary>
        /// <param name="line">The line of interest</param>
        /// <returns>Null (always), since this operation doesn't create any lines.</returns>
        internal override LineFeature GetPredecessor(LineFeature line)
        {
            return null;
        }

        /// <summary>
        /// Executes the move operation.
        /// </summary>
        /// <param name="to">The position to move to</param>
        internal void Execute(PointGeometry to)
        {
        	// Remember the old and new positions.
            m_OldPosition = PointGeometry.Create(m_Text.Position);

            // Remember old polygon reference position if it's different from the text position
            m_OldPolPosition = PointGeometry.Create(m_Text.GetPolPosition());
            if (m_OldPosition.IsCoincident(m_OldPolPosition))
                m_OldPolPosition = null;

            m_NewPosition = to;

            // Move the text and ensure any enclosing polygon has been reworked
            m_Text.Move(to);
            m_Text.RecalculateEnclosingPolygon();

            // Peform standard completion steps
            Complete();
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
            return new Feature[0];
        }

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public override void WriteData(EditSerializer editSerializer)
        {
            base.WriteData(editSerializer);

            editSerializer.WriteFeatureRef<TextFeature>(DataField.Text, m_Text);
            editSerializer.WritePointGeometry(DataField.OldX, DataField.OldY, m_OldPosition);
            editSerializer.WritePointGeometry(DataField.NewX, DataField.NewY, m_NewPosition);

            if (m_OldPolPosition != null)
                editSerializer.WritePointGeometry(DataField.OldPolygonX, DataField.OldPolygonY, m_OldPolPosition);
        }

        /// <summary>
        /// Reads data that was previously written using <see cref="WriteData"/>
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        /// <param name="text">The feature that was moved</param>
        /// <param name="oldPosition">The original position of the text.</param>
        /// <param name="newPosition">Where the text was moved to. This doubles as the new polygon reference position.</param>
        /// <param name="oldPolPosition">The old reference position (null if its identical to the old position)</param>
        static void ReadData(EditDeserializer editDeserializer, out TextFeature text, out PointGeometry oldPosition,
                                out PointGeometry newPosition, out PointGeometry oldPolPosition)
        {
            text = editDeserializer.ReadFeatureRef<TextFeature>(DataField.Text);
            oldPosition = editDeserializer.ReadPointGeometry(DataField.OldX, DataField.OldY);
            newPosition = editDeserializer.ReadPointGeometry(DataField.NewX, DataField.NewY);

            if (editDeserializer.IsNextField(DataField.OldPolygonX))
                oldPolPosition = editDeserializer.ReadPointGeometry(DataField.OldPolygonX, DataField.OldPolygonY);
            else
                oldPolPosition = null;
        }
    }
}
