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
using System.Collections.Generic;
using System.Windows.Forms;

using Backsight.Editor.Forms;
using Backsight.Forms;
using Backsight.Editor.Operations;
using Backsight.Editor.Observations;

namespace Backsight.Editor.UI
{
    /// <written by="Steve Stanton" on="11-JUN-1999" was="CuiPointOnLine" />
    /// <summary>
    /// User interface for using one observed distance to subdivide a line.
    /// </summary>
    class SimpleLineSubdivisionUI : SimpleCommandUI, IDisposable
    {
        #region Class data

        /// <summary>
        /// The line being subdivided.
        /// </summary>
        LineFeature m_Line;

        /// <summary>
        /// The dialog for the command.
        /// </summary>
        SimpleLineSubdivisionControl m_Dialog;

        /// <summary>
        /// The distance to the split point.
        /// </summary>
        Distance m_Length;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor to subdivide the currently selected line.
        /// </summary>
        /// <param name="cc">The container for any dialogs</param>
        /// <param name="action">The action that initiated this command</param>
        internal SimpleLineSubdivisionUI(IControlContainer cc, IUserAction action)
            : base(cc, action)
        {
            LineFeature line = EditingController.SelectedLine;
            if (line == null)
                throw new InvalidOperationException("You must initially select the line you want to subdivide.");

            SetInitialValues();
            m_Line = line;
        }
    
        /// <summary>
        /// Constructor for doing an update.
        /// </summary>
        /// <param name="cc">The container for any dialogs</param>
        /// <param name="editId">The ID of the edit this command deals with.</param>
        /// <param name="updcmd">The update command.</param>
        internal SimpleLineSubdivisionUI(IControlContainer cc, EditingActionId editId, UpdateUI updcmd)
            : base(cc, editId, updcmd)
        {
            SetInitialValues();
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
        /// Initialize everything with null values. Used by constructors.
        /// </summary>
        void SetInitialValues()
        {
	        m_Dialog = null;
	        m_Line = null;
	        m_Length = new Distance();
        }

        /// <summary>
        /// Starts the user interface (if any) for this command.
        /// </summary>
        /// <returns>True if command started ok.</returns>
        internal override bool Run()
        {
            // Don't run more than once.
            if (m_Dialog!=null)
                throw new InvalidOperationException("SimpleLineSubdivisionUI.Run - Command is already running.");

            UpdateUI pup = this.Update;

            if (pup!=null)
                m_Dialog = new SimpleLineSubdivisionControl(pup);
            else
                m_Dialog = new SimpleLineSubdivisionControl(this, m_Line, this.Recall);

            this.Container.Display(m_Dialog);
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
                m_Dialog.Draw();
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
            this.Container.Clear();

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
                MessageBox.Show("SimpleLineSubdivisionUI.DialFinish - No dialog!");
                return false;
            }

            // De-select the line that's being split
            Controller.ClearSelection();

            // Get info from the dialog.
            m_Length = m_Dialog.Length;

            Distance d = new Distance(m_Length);
            if (m_Dialog.IsFromEnd)
                d.SetNegative();

            // If we are doing an update, alter the original operation.
            UpdateUI up = this.Update;

            if (up!=null)
            {
                // Get the original operation.
                SimpleLineSubdivisionOperation pop = (up.GetOp() as SimpleLineSubdivisionOperation);
                if (pop==null)
                {
                    MessageBox.Show("SimpleLineSubdivisionUI.DialFinish - Unexpected edit type.");
                    return false;
                }

                // Make the update.
                pop.Correct(m_Dialog.Length, m_Dialog.IsFromEnd);

                SimpleLineSubdivisionOperation rev = new SimpleLineSubdivisionOperation(pop, d);
                up.AddRevision(rev);
            }
            else
            {

                // Execute the edit
                SimpleLineSubdivisionOperation op = null;

                try
                {
                    op = new SimpleLineSubdivisionOperation(Session.WorkingSession, 0, m_Line, d);
                    op.Execute();
                }

                catch (Exception ex)
                {
                    //Session.WorkingSession.Remove(op);
                    MessageBox.Show(ex.StackTrace, ex.Message);
                    return false;
                }
            }

            // Destroy the dialog(s).
            KillDialogs();

            // Get the base class to finish up.
            return FinishCommand();
        }
    }
}
