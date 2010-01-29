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

namespace Backsight.Environment.Editor
{
    /// <summary>
    /// Something that displays some type of environment item.
    /// </summary>
    public interface IDisplayControl
    {
        /// <summary>
        /// Prompts the user for information for a new environment item.
        /// </summary>
        void NewItem();

        /// <summary>
        /// Updates the currently selected environment item.
        /// </summary>
        void UpdateSelectedItem();

        /// <summary>
        /// Removes the currently selected environment item.
        /// </summary>
        void DeleteSelectedItem();

        /// <summary>
        /// Refresh the list
        /// </summary>
        void RefreshList();
    }
}
