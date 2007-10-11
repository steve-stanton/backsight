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

using Backsight.Forms;
using Backsight.Editor.Operations;
using System.Windows.Forms;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="10-OCT-2007" />
    /// <summary>
    /// User interface for changing the topological status of a line.
    /// </summary>
    class SetTopologyUI : SimpleCommandUI
    {
        #region Class data

        /// <summary>
        /// The line that will have it's topological status changed.
        /// </summary>
        readonly LineFeature m_Line;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>SetTopologyUI</c>
        /// </summary>
        /// <param name="action">The action that initiated this command</param>
        /// <param name="line">The line to change topological status (not null)</param>
        internal SetTopologyUI(IUserAction action, LineFeature line)
            : base(action)
        {
            if (line==null)
                throw new ArgumentNullException();

            m_Line = line;
        }

        #endregion

        /// <summary>
        /// Starts the user interface (if any) for this command. This will change the
        /// topological status of the line that was passed to the constructor, then
        /// complete the command.
        /// </summary>
        /// <returns>True if command started (and completed) ok.</returns>
        internal override bool Run()
        {
            CadastralEditController c = Controller;
            SetTopologyOperation op = null;

            try
            {
                op = new SetTopologyOperation(m_Line);
                op.Execute();
                c.ClearSelection();
                c.FinishCommand(this);
                return true;
            }

            catch (Exception e)
            {
                Session.CurrentSession.Remove(op);
                c.AbortCommand(this);
                MessageBox.Show(e.Message);
            }

            return false;
        }
    }
}
