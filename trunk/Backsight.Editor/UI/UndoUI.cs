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
using System.Windows.Forms;

using Backsight.Forms;

namespace Backsight.Editor.UI
{
    /// <written by="Steve Stanton" on="17-OCT-2007" />
    /// <summary>
    /// User interface for undoing an edit. This doesn't do much, since there is currently
    /// no user interaction.
    /// </summary>
    class UndoUI : SimpleCommandUI
    {
        #region Class data

        // no data

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>UndoUI</c>
        /// </summary>
        /// <param name="action">The action that initiated this command</param>
        internal UndoUI(IUserAction action)
            : base(action)
        {
        }

        #endregion

        /// <summary>
        /// Undoes the last edit.
        /// </summary>
        /// <returns>True if the command ran to completion. False if there was nothing to
        /// undo in the current editing session, or an exception arose.</returns>
        internal override bool Run()
        {
            EditingController c = Controller;

            try
            {
                // Turn off any highlighted features (confusing if the feature is undone by the rollback).
                c.ClearSelection();

                // Ask the map to rollback the last operation (restricting 
                // to the current editing session).
                ISession s = CadastralMapModel.Current.WorkingSession;
                if (CadastralMapModel.Current.Rollback(s))
                {
                    c.FinishCommand(this);
                    return true;
                }

                MessageBox.Show("Nothing to undo from current session.");
            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            c.AbortCommand(this);
            return false;
        }
    }
}
