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
    }
}
