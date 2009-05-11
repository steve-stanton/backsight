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
using Backsight.Editor.Xml;


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
        /// Constructor for use during deserialization.
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        /// <param name="t">The serialized version of this instance</param>
        internal MovePolygonPositionOperation(Session s, MovePolygonPositionType t)
            : base(s, t)
        {
            m_Label = s.MapModel.Find<TextFeature>(t.Label);
            m_NewPosition = new PointGeometry(t.NewX, t.NewY);

            if (t.OldXSpecified && t.OldYSpecified)
                m_OldPosition = new PointGeometry(t.OldX, t.OldY);
            else
                m_OldPosition = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MovePolygonPositionOperation"/> class
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        internal MovePolygonPositionOperation(Session s)
            : base(s)
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
        /// <param name="uc">The context in which editing revisions are being made (not null).
        /// Used to hold a record of any positional changes.</param>
        /// <returns>True if operation has been re-executed successfully</returns>
        internal override bool Rollforward(UpdateContext uc)
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
        /// Returns an object that represents this edit, and that can be serialized using
        /// the <c>XmlSerializer</c> class.
        /// <returns>The serializable version of this edit</returns>
        internal override OperationType GetSerializableEdit()
        {
            MovePolygonPositionType t = new MovePolygonPositionType();
            base.SetSerializableEdit(t);

            t.Label = m_Label.DataId;
            t.NewX = m_NewPosition.Easting.Microns;
            t.NewY = m_NewPosition.Northing.Microns;

            if (m_OldPosition != null)
            {
                t.OldX = m_OldPosition.Easting.Microns;
                t.OldY = m_OldPosition.Northing.Microns;

                t.OldXSpecified = t.OldYSpecified = true;
            }

            return t;
        }

        /// <summary>
        /// Performs the data processing associated with this editing operation.
        /// </summary>
        internal override void RunEdit()
        {
            // Move the label's reference position -- the name of this method
            // really needs to be changed.
            m_Label.SetPolPosition(m_NewPosition);
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
