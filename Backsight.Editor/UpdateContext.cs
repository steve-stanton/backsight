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
using System.Text;
using System.Diagnostics;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="11-MAY-2009" />
    /// <summary>
    /// Information about changes made while the user makes a series of editing revisions.
    /// </summary>
    class UpdateContext
    {
        #region Class data

        /// <summary>
        /// Items that have been moved as a result of editing revision. Each list contains the
        /// moves relating to a single revision.
        /// </summary>
        readonly Stack<UpdateUndoMarker> m_Moves;

        /// <summary>
        /// The last editing operation that was completed prior to the creation of this context
        /// instance (null if the context was created at the very beginning of the session).
        /// </summary>
        readonly Operation m_LastEdit;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateContext"/> class.
        /// </summary>
        internal UpdateContext()
        {
            m_Moves = new Stack<UpdateUndoMarker>();
            m_LastEdit = Session.WorkingSession.LastOperation;
        }

        #endregion

        /// <summary>
        /// The number of undo markers that have been created via calls to <see cref="SetUndoMarker"/>.
        /// </summary>
        internal uint NumUndoMarkers
        {
            get { return (uint)m_Moves.Count; }
        }

        /// <summary>
        /// Sets an undo marker to indicate a savepoint that the user may go back to
        /// while making a series of editing revisions.
        /// </summary>
        internal void SetUndoMarker()
        {
            UpdateUndoMarker uum = new UpdateUndoMarker();

            UpdateUndoMarker lastMarker = m_Moves.Peek();
            if (lastMarker!=null && lastMarker.EditSequence==uum.EditSequence)
                throw new InvalidOperationException("Attempt to create another undo marker for the same edit");

            m_Moves.Push(uum);
        }

        /// <summary>
        /// Remembers a point that is about to be moved
        /// </summary>
        /// <param name="p">The point that is about to move</param>
        internal void AddMove(PointFeature p)
        {
            UpdateUndoMarker um = m_Moves.Peek();
            Debug.Assert(um != null);
            um.AddMove(p);
        }

        /// <summary>
        /// Rolls back changes (moves) that have occurred since the last undo marker (as defined
        /// via a prior call to <see cref="SetUndoMarker"/>).
        /// </summary>
        /// <returns>True if an undo marker was rolled back. False if everything has already
        /// been undone.</returns>
        internal bool Undo()
        {
            if (m_Moves.Count == 0)
                return false;

            UpdateUndoMarker um = m_Moves.Pop();
            Debug.Assert(um != null);
            um.Undo();
            return true;
        }

        /// <summary>
        /// Rolls back all changes known to this context instance. This just makes repetitive
        /// calls to <see cref="Undo"/>, exiting only when it returns <c>false</c>.
        /// </summary>
        internal void UndoAll()
        {
            for (;;)
                Undo();
        }
    }
}
