// <remarks>
// Copyright 2011 - Steve Stanton. This file is part of Backsight
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
    /// <summary>
    /// An editing session
    /// </summary>
    interface ISession
    {
        /// <summary>
        /// The edits performed in this session.
        /// </summary>
        Operation[] Edits { get; }

        /// <summary>
        /// Unique ID for the session.
        /// </summary>
        uint Id { get; }

        /// <summary>
        /// The number of edits performed in this session
        /// </summary>
        int OperationCount { get; }

        /// <summary>
        /// When was session started? 
        /// </summary>
        DateTime StartTime { get; }


        /// <summary>
        /// When was the last edit performed?
        /// </summary>
        DateTime EndTime { get; }

        /// <summary>
        /// The user logged on for the session. 
        /// </summary>
        IUser User { get; }

        /// <summary>
        /// The model that contains this session
        /// </summary>
        CadastralMapModel MapModel { get; }

        /// <summary>
        /// Deletes information about this session from the database.
        /// </summary>
        void Delete();

        /// <summary>
        /// Records the fact that this session has been "saved". This doesn't actually
        /// save anything, since that happens each time you perform an edit.
        /// </summary>
        void SaveChanges();

        /// <summary>
        /// Reserves an item number for use with the current session. It is a lightweight
        /// request, because it just increments a counter. The database gets updated
        /// when an edit completes.
        /// </summary>
        /// <returns>The reserved item number</returns>
        uint AllocateNextItem();

        /// <summary>
        /// The last editing operation in this session (null if no edits have been performed)
        /// </summary>
        Operation LastOperation { get; }

        /// <summary>
        /// Updates the end-time (and item count) associated with this session
        /// </summary>
        void UpdateEndTime();

        /// <summary>
        /// Saves an editing operation as part of this session.
        /// </summary>
        /// <param name="edit">The edit to save</param>
        void SaveOperation(Operation edit);

        /// <summary>
        /// Have edits performed as part of this session been "saved" (as far as
        /// the user is concerned).
        /// </summary>
        bool IsSaved { get; }

        /// <summary>
        /// Rolls back the last operation in this session. The operation will be removed from
        /// the session's operation list.
        /// </summary>
        /// <returns>-1 if last operation failed to roll back. 0 if no operation to rollback.
        /// Otherwise the sequence number of the edit that was rolled back.</returns>
        int Rollback();

        /// <summary>
        /// Gets rid of edits that the user has not explicitly saved.
        /// </summary>
        void DiscardChanges();

        /// <summary>
        /// Attempts to locate an edit within this session
        /// </summary>
        /// <param name="editSequence">The sequence number of the edit to look for</param>
        /// <returns>The corresponding editing operation (null if not found)</returns>
        Operation FindOperation(uint editSequence);

        /// <summary>
        /// Obtains dependent edits within this session.
        /// </summary>
        /// <param name="deps">The dependent edits.</param>
        /// <param name="startOp">The first operation that should be touched (specify null
        /// for all edits in this session).</param>
        void Touch(List<Operation> deps, Operation startOp);
    }
}
