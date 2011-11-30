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

namespace Backsight.Editor
{
    /// <summary>
    /// A user of Backsight.
    /// </summary>
    interface IUser
    {
        /// <summary>
        /// The internal ID for the user
        /// </summary>
        uint UserId { get; }

        /// <summary>
        /// The user-perceived name for the user
        /// </summary>
        string Name { get; }
    }
}
