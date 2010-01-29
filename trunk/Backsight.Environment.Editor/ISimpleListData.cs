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

using System.Windows.Forms;

namespace Backsight.Environment.Editor
{
    /// <summary>
    /// Information needed for the <see cref="SimpleListControl"/>
    /// </summary>
    interface ISimpleListData
    {
        /// <summary>
        /// Obtains the environment items that should be displayed.
        /// </summary>
        /// <returns>The active set of environment items</returns>
        IEnvironmentItem[] GetEnvironmentItems();

        /// <summary>
        /// Creates a dialog that is suitable for entering a new environment item, or
        /// modifying an existing item.
        /// </summary>
        /// <param name="item">An existing item that needs to be updated (specify null
        /// if creating a brand new item)</param>
        /// <returns>The dialog to display</returns>
        Form GetEntryDialog(IEnvironmentItem item);
    }
}
