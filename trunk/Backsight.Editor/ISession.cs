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
    }
}
