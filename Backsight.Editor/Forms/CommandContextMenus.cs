/// <remarks>
/// Copyright 2008 - Steve Stanton. This file is part of Backsight
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

using Backsight.Forms;

namespace Backsight.Editor.Forms
{
    /// <summary>
    /// Placeholder class for holding context menus used by editor commands.
    /// This class is never actually instantiated.
    /// </summary>
    partial class CommandContextMenus : UserControl
    {
        internal CommandContextMenus()
        {
            InitializeComponent();
        }


        /// <summary>
        /// The context menu for the <see cref="NewLineUI"/> class
        /// </summary>
        internal ContextMenuStrip NewLineContextMenu(NewLineUI ui)
        {
            //ctxNewLineCancel.Click += new EventHandler(ui.Escape);
            //ctxNewLineSpecifyId.Click += new EventHandler(ui.SpecifyId);

            return newLineContextMenu;
        }

        //IUserAction action)

        /// <summary>
        /// The context menu for the <see cref="NewCircularArcUI"/> class
        /// </summary>
        internal ContextMenuStrip NewCircularArcContextMenu
        {
            get { return newArcContextMenu; }
        }
    }
}
