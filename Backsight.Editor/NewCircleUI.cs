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

using Backsight.Forms;
using Backsight.Editor.Forms;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="28-NOV-1998" was="CuiNewCircle" />
    /// <summary>
    /// User interface for creating a new circle.
    /// </summary>
    class NewCircleUI : SimpleCommandUI, IDisposable
    {
        #region Class data

        /// <summary>
        /// The dialog for the command
        /// </summary>
        NewCircleForm m_Dialog;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>NewCircleUI</c>
        /// </summary>
        /// <param name="cc">Object for holding any displayed dialogs</param>
        /// <param name="action">The action that initiated this command</param>
        internal NewCircleUI(IControlContainer cc, IUserAction action)
            : base(cc, action)
        {
            m_Dialog = null;
        }

        #endregion

        public override void Dispose()
        {
            base.Dispose(); // removes any controls from container

            if (m_Dialog!=null)
            {
                m_Dialog.Dispose();
                m_Dialog = null;
            }
        }

        /// <summary>
        /// Starts the user interface (if any) for this command.
        /// </summary>
        /// <returns>True if command started ok.</returns>
        internal override bool Run()
        {
            // Don't run more than once.
            if (m_Dialog!=null)
                throw new InvalidOperationException("NewCircleUI.Run - Command is already running.");

            // Are we doing an update?
            UpdateUI pup = this.Update;

            if (pup!=null)
                m_Dialog = new NewCircleForm(this);
            else
                m_Dialog = new NewCircleForm(this);

            m_Dialog.Show();
            return true;
        }
    }
}
