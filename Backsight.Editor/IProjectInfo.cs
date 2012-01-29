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
using Backsight.Forms;

namespace Backsight.Editor
{
    /// <summary>
    /// Information about current options for an editing project.
    /// </summary>
    /// <remarks>Implemented by <see cref="ProjectInfoFile"/></remarks>
    interface IProjectInfo
    {
        /// <summary>
        /// The container for the project data.
        /// </summary>
        ProjectSilo Container { get; }

        /// <summary>
        /// The user-perceived name of the project.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// A unique ID for the project
        /// </summary>
        Guid ProjectId { get; }

        /// <summary>
        /// Identifies a map layer associated with the project.
        /// </summary>
        int LayerId { get; }

        /// <summary>
        /// Current settings for the project (things like current defaults specified by the user).
        /// </summary>
        ProjectSettings Settings { get; }

        /// <summary>
        /// Has modified project information been saved?
        /// </summary>
        bool IsSaved { get; }

        /// <summary>
        /// Saves the project info as part of a persistent storage area.
        /// </summary>
        void Save();

        /// <summary>
        /// Loads a map model with the content of this project.
        /// </summary>
        /// <param name="mapModel">The model to load</param>
        void LoadModel(CadastralMapModel mapModel);

        /// <summary>
        /// Loads ID allocations that have been made for this project.
        /// </summary>
        /// <returns>The ID allocations (never null, but may be an empty array).</returns>
        IdAllocationInfo[] GetIdAllocations();

        /// <summary>
        /// Creates a brand new session for this project.
        /// </summary>
        /// <param name="sessionId">The ID to assign to the new session</param>
        /// <returns>The newly created session</returns>
        //ISession AppendWorkingSession(uint sessionId);
    }
}
