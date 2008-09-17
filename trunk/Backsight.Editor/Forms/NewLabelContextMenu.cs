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
using System.Diagnostics;

using Backsight.Forms;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" on="21-APR-2008" />
    /// <summary>
    /// Context menu for the <see cref="NewLabelUI"/>
    /// </summary>
    partial class NewLabelContextMenu : ContextMenuStrip
    {
        #region Constructors

        /// <summary>
        /// Creates a new <c>NewLabelContextMenu</c>, wiring each menuitem to the specified UI.
        /// </summary>
        /// <param name="ui">The user interface displaying this context menu</param>
        internal NewLabelContextMenu(NewLabelUI ui)
        {
            InitializeComponent();

            new UserAction(ctxAutoPosition, ui.ToggleAutoPosition);
            new UserAction(ctxAutoAngle, ui.ToggleAutoAngle);

            ctxAutoPosition.Checked = ui.IsAutoPosition;
            ctxAutoAngle.Checked = ui.IsAutoAngle;

            // If we're auto-positioning, the auto-angle option should be disabled (it may
            // still be checked to signify that it'll be re-enabled on turning off the
            // auto-angle option).
            if (ui.IsAutoPosition)
                ctxAutoAngle.Enabled = false;

            uint sizeFactor = ui.SizeFactor;
            new TextSizeAction(ctx500, ui.SetSizeFactor, 500, sizeFactor);
            new TextSizeAction(ctx200, ui.SetSizeFactor, 200, sizeFactor);
            new TextSizeAction(ctx150, ui.SetSizeFactor, 150, sizeFactor);
            new TextSizeAction(ctx100, ui.SetSizeFactor, 100, sizeFactor);
            new TextSizeAction(ctx75, ui.SetSizeFactor, 75, sizeFactor);
            new TextSizeAction(ctx50, ui.SetSizeFactor, 50, sizeFactor);
            new TextSizeAction(ctx25, ui.SetSizeFactor, 25, sizeFactor);

            new UserAction(ctxFinish, ui.Finish);
        }

        #endregion
    }
}
