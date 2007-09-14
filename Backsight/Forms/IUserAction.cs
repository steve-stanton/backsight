/// <remarks>
/// Copyright 2007 - Steve Stanton. This file is part of Backsight
///
/// Backsight is free software; you can redistribute it and/or modify it under the terms
/// of the GNU Lesser General Public License as published by the Free Software Foundation;
/// either version 3 of the License, or (at your option) any later version.
///
/// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
/// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU Lesser General Public License for more details.
///
/// You should have received a copy of the GNU Lesser General Public License
/// along with this program. If not, see <http://www.gnu.org/licenses/>.
/// </remarks>

using System;
using System.Windows.Forms;

namespace Backsight.Forms
{
	/// <written by="Steve Stanton" on="14-SEP-2006" />
    /// <see>UserAction</see>
    public interface IUserAction
    {
        /// <summary>
        /// Ensures that UI elements reflect the current state of the system (enables
        /// or disables, sets or clears check marks, etc.).
        /// </summary>
        void Update();

        /// <summary>
        /// A name for the action (typically the text that appears on a menu item)
        /// </summary>
        string Title { get; }
    }
}
