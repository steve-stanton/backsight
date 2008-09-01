// <remarks>
// Copyright 2007 - Steve Stanton. This file is part of Backsight
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

namespace Backsight.Environment
{
	/// <written by="Steve Stanton" on="13-MAR-2007" />
    /// <summary>
    /// Methods for controlling the editing process.
    /// </summary>
    public interface IEditControl
    {
        /// <summary>
        /// Begins a series of edits to an item.
        /// </summary>
        void BeginEdit();

        /// <summary>
        /// Undoes changes since edits were last committed.
        /// </summary>
        void CancelEdit();

        /// <summary>
        /// Commits an edit. If the item is brand new, this will add the item into an
        /// instance of <c>IEnvironmentContainer</c>. If the item was previously part
        /// of a container, constraint checking will be enabled.
        /// </summary>
        void FinishEdit();

        /// <summary>
        /// Marks something for deletion
        /// </summary>
        void Delete();
    }
}
