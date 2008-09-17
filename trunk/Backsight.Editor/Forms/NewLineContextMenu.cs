// <remarks>
// Copyright 2008 - Steve Stanton. This file is part of Backsight
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
using System.Windows.Forms;

using Backsight.Forms;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" on="07-JAN-2008" />
    /// <summary>
    /// Context menu for the <see cref="NewCircularArcUI"/>
    /// </summary>
    partial class NewLineContextMenu : ContextMenuStrip
    {
        #region Constructors

        /// <summary>
        /// Creates a new <c>NewLineContextMenu</c>, wiring each menuitem to the specified UI.
        /// </summary>
        /// <param name="ui">The user interface displaying this context menu</param>
        internal NewLineContextMenu(NewLineUI ui)
        {
            InitializeComponent();

            new UserAction(ctxSpecifyId, ui.SpecifyId);
            new UserAction(ctxCancel, ui.Cancel);
        }

        #endregion
    }
}