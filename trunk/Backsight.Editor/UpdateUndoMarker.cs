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
using System.Collections.Generic;


namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="12-MAY-2009" />
    /// <summary>
    /// Information about changes made while the user makes an editing revision.
    /// In situations where the update consists of several edits, you create an instance
    /// of <see cref="UpdateUndoMarker"/> prior to each edit. This acts as a savepoint
    /// that the user can rollback to.
    /// <para/>
    /// Undo markers are expected to span at least one editing operation (software elsewhere
    /// should disallow an attempt to create more than one undo marker for the same edit).
    /// </summary>
    class UpdateUndoMarker
    {
        #region Class data

        /// <summary>
        /// Items that have been moved as a result of an edit.
        /// </summary>
        readonly List<Move> m_Moves;

        /// <summary>
        /// The last editing operation that was completed prior to the creation of this undo
        /// marker (null if the marker was created at the very beginning of the session).
        /// </summary>
        readonly Operation m_LastEdit;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateUndoMarker"/> class.
        /// </summary>
        internal UpdateUndoMarker()
        {
            m_Moves = new List<Move>();
            m_LastEdit = Session.WorkingSession.LastOperation;
        }

        #endregion

        /// <summary>
        /// The sequence number of the last edit that was completed prior to the creation of
        /// this undo marker (0 if the marker was created at the very beginning of the session).
        /// </summary>
        internal uint EditSequence
        {
            get { return (m_LastEdit==null ? 0 : m_LastEdit.EditSequence); }
        }

        /// <summary>
        /// Remembers a point that is about to be moved
        /// </summary>
        /// <param name="p">The point that is about to move</param>
        internal void AddMove(PointFeature p)
        {
            Move m = new Move(p);
            m_Moves.Add(m);
        }

        /// <summary>
        /// Rolls back changes that occurred after creation of this undo marker.
        /// </summary>
        internal void Undo()
        {
            // Get rid of the edits saved to the database
            Session s = Session.WorkingSession;
            Operation op = s.LastOperation;

            while (op!=null && op.EditSequence > this.EditSequence)
            {
                s.Rollback();
                op = s.LastOperation;
            }

            // Ensure features revert to their original positions. It probably shouldn't matter,
            // but undo the moves in reverse order
            for (int i=m_Moves.Count-1; i>=0; i--)
                m_Moves[i].Undo();
        }

        /// <summary>
        /// Is this an empty undo marker (with no moves recorded)?
        /// </summary>
        internal bool IsEmpty
        {
            get { return (m_Moves.Count == 0); }
        }
    }
}
