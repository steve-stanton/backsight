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

using Backsight.Forms;
using Backsight.Editor.Operations;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="10-SEP-2007" />
    /// <summary>
    /// User interface for deleting one or more features. This doesn't do much, since a user
    /// performs a deletion by selecting stuff, then picking an appropriate menuitem or
    /// button to delete the selection. At the present time, there is no user confirmation,
    /// although that's a possible enhancement.
    /// </summary>
    class DeletionUI : SimpleCommandUI
    {
        #region Class data

        // No data

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>DeletionUI</c>
        /// </summary>
        /// <param name="action">The action that initiated this command</param>
        internal DeletionUI(IUserAction action)
            : base(action)
        {
        }

        #endregion

        /// <summary>
        /// Runs a deletion command by creating the editing operation (using the current feature
        /// selection) & executing it. The controller is then told to clear the selection before
        /// completing the command.
        /// </summary>
        /// <returns>True if the command ran to completion. False if any exception arose (in that
        /// case, the controller would be told to abort the command).</returns>
        internal override bool Run()
        {
            EditingController c = Controller;
            DeletionOperation dop = null;

            try
            {
                SetCommandCursor();
                dop = new DeletionOperation();
                foreach (ISpatialObject so in c.Selection.Items)
                {
                    if (so is Feature)
                        dop.AddDeletion(so as Feature);
                }
                dop.Execute();
                c.ClearSelection();
                c.FinishCommand(this);
                return true;
            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Session.CurrentSession.Remove(dop);
                c.AbortCommand(this);
                return false;
            }
        }

        internal override void SetCommandCursor()
        {
            ActiveDisplay.MapPanel.Cursor = Cursors.WaitCursor;
        }
    }
}
