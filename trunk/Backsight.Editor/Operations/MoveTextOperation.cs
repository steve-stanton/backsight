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
        TextFeature m_Text;

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
        /// Constructor for use during deserialization.
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        /// <param name="sequence">The sequence number of the edit within the session (specify 0 if
        /// a new sequence number should be reserved). A non-zero value is specified during
        /// deserialization from the database.</param>
        internal MoveTextOperation(Session s, uint sequence)
            : base(s, sequence)
        {
            m_Text = null;
            m_OldPosition = null;
            m_OldPolPosition = null;
            m_NewPosition = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveTextOperation"/> class
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        internal MoveTextOperation(Session s)
            : this(s, 0)
        {
        }

        #endregion

        /// <summary>
        /// The feature that was moved.
        /// </summary>
        internal TextFeature MovedText // was GetpLabel
        {
            get { return m_Text; }
            set { m_Text = value; }
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
        /// <param name="uc">The context in which editing revisions are being made (not null).
        /// Used to hold a record of any positional changes.</param>
        /// <returns>True if operation has been re-executed successfully</returns>
        internal override bool Rollforward(UpdateContext uc)
        {
            // Return if this operation has not been marked as changed.
            if (!IsChanged)
                return base.OnRollforward();

            // Nothing to do?

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
        internal override void RunEdit()
        {
            // Ensure the text has been moved to the revised position.
            // Should be no need to re-calculate enclosing polygon while deserializing
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
        /// <param name="text">The text to be moved</param>
        /// <param name="to">The position to move to</param>
        internal void Execute(TextFeature text, PointGeometry to)
        {
	        // Remember the text being moved.
	        m_Text = text;

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
    }
}
