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
using Backsight.Editor.Forms;
using Backsight.Editor.Operations;

namespace Backsight.Editor.UI
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

        /// <summary>
        /// Constructor for doing an update.
        /// </summary>
        /// <param name="cc">Object for holding any displayed dialogs</param>
        /// <param name="editId">The ID of the edit this command deals with.</param>
        /// <param name="updcmd">The update command.</param>
        internal NewCircleUI(IControlContainer cc, EditingActionId editId, UpdateUI updcmd)
            : base(cc, editId, updcmd)
        {
            m_Dialog = null;
        }

        #endregion

        public override void Dispose()
        {
            base.Dispose(); // removes any controls from container
            KillDialogs();
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
                m_Dialog = new NewCircleForm(pup);
            else
                m_Dialog = new NewCircleForm(this, this.Recall);

            m_Dialog.Show();
            return true;
        }

        /// <summary>
        /// Override indicates that this command performs painting. This means that the
        /// controller will periodically call the <see cref="Paint"/> method (probably during
        /// idle time).
        /// </summary>
        internal override bool PerformsPainting
        {
            get { return true; }
        }

        /// <summary>
        /// Does any command-specific drawing.
        /// </summary>
        /// <param name="point">The specific point (if any) that the parent window has drawn. Not used.</param>
        internal override void Paint(PointFeature point)
        {
            if (m_Dialog!=null)
                m_Dialog.Draw(point);
        }

        /// <summary>
        /// Reacts to the selection of a point feature.
        /// </summary>
        /// <param name="point">The point (if any) that has been selected.</param>
        internal override void OnSelectPoint(PointFeature point)
        {
            if (m_Dialog!=null)
                m_Dialog.OnSelectPoint(point);
        }

        /// <summary>
        /// Reacts to selection of the Cancel button in the dialog.
        /// </summary>
        /// <param name="wnd">The dialog window. If this matches the dialog that
        /// this command knows about, the dialog will be destroyed and the command
        /// terminates. If it's some other window, it must be a sub-dialog created
        /// by our guy, so let it handle the request.</param>
        internal override void DialAbort(Control wnd)
        {
            KillDialogs();
            AbortCommand();
        }

        /// <summary>
        /// Destroys any dialogs that are currently displayed.
        /// </summary>
        void KillDialogs()
        {
            if (m_Dialog!=null)
            {
                m_Dialog.Dispose();
                m_Dialog = null;
            }
        }

        /// <summary>
        /// Reacts to selection of the OK button in the dialog.
        /// </summary>
        /// <param name="wnd">The dialog window. If this matches the dialog that
        /// this command knows about, the command will be executed (and, on success,
        /// the dialog will be destroyed). If it's some other window, it must
        /// be a sub-dialog created by our guy, so let it handle the request.</param>
        /// <returns></returns>
        internal override bool DialFinish(Control wnd)
        {
            if (m_Dialog==null)
            {
                MessageBox.Show("NewCircleUI.DialFinish - No dialog!");
                return false;
            }

            // Get info from the dialog.
            PointFeature center = m_Dialog.Center;
            Observation radius = m_Dialog.Radius;

            // Both items must be defined (the dialog should have confirmed
            // this in its OnOK handler).
            if (center==null || radius==null)
            {
                MessageBox.Show("NewCircleUI.DialFinish - Insufficient data to add circle.");
                return false;
            }

            // If we are doing an update, alter the original operation.
            UpdateUI pup = this.Update;

            if (pup!=null)
            {
                // Get the original operation.
                NewCircleOperation pop = (pup.GetOp() as NewCircleOperation);
                if (pop==null)
                {
                    MessageBox.Show("NewCircleUI.DialFinish - Unexpected edit type.");
                    return false;
                }

                pop.Correct(center, radius);
            }
            else
            {
                // Create empty persistent object (adds to current session)
                NewCircleOperation op = null;

                try
                {
                    op = new NewCircleOperation(center, radius);
                    op.Execute();
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.StackTrace, ex.Message);
                    return false;
                }
            }

            // Get the base class to finish up.
            return FinishCommand();
        }
    }
}
