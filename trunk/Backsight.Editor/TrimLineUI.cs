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
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Backsight.Forms;
using Backsight.Editor.Operations;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="03-JAN-2008" />
    /// <summary>
    /// User interface for trimming dangling line features.
    /// </summary>
    class TrimLineUI : SimpleCommandUI
    {
        #region Class data

        // none

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>TrimLineUI</c>
        /// </summary>
        /// <param name="action">The action that initiated this command</param>
        internal TrimLineUI(IUserAction action)
            : base(action)
        {
        }

        #endregion

        /// <summary>
        /// Runs a trim dangles command by creating the editing operation (using the current feature
        /// selection) & executing it. The controller is then told to clear the selection before
        /// completing the command.
        /// </summary>
        /// <returns>True if the command ran to completion. False if any exception arose (in that
        /// case, the controller would be told to abort the command).</returns>
        internal override bool Run()
        {
            // Grab the current selection & filter out stuff that can't be trimmed
            EditingController c = Controller;
            ISpatialSelection ss = c.Selection;
            LineFeature[] lines = TrimLineOperation.PreCheck(ss.Items);
            if (lines.Length==0)
            {
                StringBuilder sb = new StringBuilder(200);
                sb.Append("Nothing can be trimmed. Possible reasons:");
                sb.Append(System.Environment.NewLine);
                sb.Append(System.Environment.NewLine);
                sb.Append("1. Lines have to be dangling.");
                sb.Append(System.Environment.NewLine);
                sb.Append("2. Only lines that are polygon boundaries can be trimmed.");
                sb.Append(System.Environment.NewLine);
                sb.Append("3. There has to be something left after trimming a line.");

                MessageBox.Show(sb.ToString());
                c.AbortCommand(this);
                return false;
            }

            TrimLineOperation op = null;

            try
            {
                op = new TrimLineOperation();
                op.Execute(lines);

                c.ClearSelection();
                c.FinishCommand(this);
                return true;
            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Session.CurrentSession.Remove(op);
                c.AbortCommand(this);
                return false;
            }
        }
    }
}
