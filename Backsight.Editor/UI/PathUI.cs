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
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

using Backsight.Forms;
using Backsight.Editor.Forms;

namespace Backsight.Editor.UI
{
    /// <written by="Steve Stanton" on="28-OCT-1999" was="CuiPath"/>
    /// <summary>
    /// User interface for doing a connection path.
    /// </summary>
    class PathUI : SimpleCommandUI, IDisposable
    {
        #region Class data

        /// <summary>
        /// Dialog for getting the from point.
        /// </summary>
        GetPointForm m_DialFrom;

        /// <summary>
        /// Dialog for getting the to point.
        /// </summary>
        GetPointForm m_DialTo;

        /// <summary>
        /// The actual connection path.
        /// </summary>
        PathForm m_DialPath;

        /// <summary>
        /// The update dialog.
        /// </summary>
        UpdatePathForm m_DialUp;

        /// <summary>
        /// The from point.
        /// </summary>
        PointFeature m_From;

        /// <summary>
        /// The to point.
        /// </summary>
        PointFeature m_To;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for a new connection path.
        /// </summary>
        /// <param name="cc">The container for any dialog controls</param>
        /// <param name="action">The action that initiated this command</param>
        internal PathUI(IControlContainer cc, IUserAction action)
            : base(cc, action)
        {
            Zero();
        }

        /// <summary>
        /// Constructor for command recall.
        /// </summary>
        /// <param name="cc">The container for any dialog controls</param>
        /// <param name="action">The action that initiated this command</param>
        /// <param name="op">The operation that's being recalled.</param>
        internal PathUI(IControlContainer cc, IUserAction action, Operation op)
            : base(cc, action, null, op)
        {
            Zero();
        }

        /// <summary>
        /// Constructor for doing an update.
        /// </summary>
        /// <param name="cc">The container for any dialog controls</param>
        /// <param name="editId">The ID of the edit this command deals with.</param>
        /// <param name="updcmd">The update command.</param>
        internal PathUI(IControlContainer cc, EditingActionId editId, UpdateUI updcmd)
            : base(cc, editId, updcmd)
        {
            Zero();
        }

        #endregion

        /// <summary>
        /// Ensures all class data has initial values (for use by constructors)
        /// </summary>
        void Zero()
        {
            m_DialFrom = null;
            m_DialTo = null;
            m_DialPath = null;
            m_DialUp = null;
            m_From = null;
            m_To = null;
        }

        public override void Dispose()
        {
            base.Dispose(); // removes any controls from container

            // Ensure any sub-dialogs have been destroyed.
            KillDialogs();
        }

        /// <summary>
        /// Starts the user interface (if any) for this command.
        /// </summary>
        /// <returns>True if command started ok.</returns>
        internal override bool Run()
        {
            // Ensure nothing is currently selected (highlighted)
            Controller.ClearSelection();

            if (!StartUpdate())
                StartFrom();

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
            ISpatialDisplay display = ActiveDisplay;

            if (m_DialFrom!=null)
                m_DialFrom.Render(display);

            if (m_DialTo!=null)
                m_DialTo.Render(display);

            if (m_DialPath!=null)
                m_DialPath.Render(display); // was OnDraw(point)

            if (m_DialUp!=null)
                m_DialUp.Render(display);

            if (m_From!=null)
                m_From.Draw(display, Color.DarkBlue);

            if (m_To!=null)
                m_To.Draw(display, Color.Cyan);
        }

        /// <summary>
        /// Reacts to the selection of a point feature.
        /// </summary>
        /// <param name="point">The point (if any) that has been selected.</param>
        internal override void OnSelectPoint(PointFeature point)
        {
            if (m_DialFrom!=null)
                m_DialFrom.OnSelectPoint(point, true);
            else if (m_DialTo!=null)
                m_DialTo.OnSelectPoint(point, true);
            else if (m_DialPath!=null)
                m_DialPath.OnSelectPoint(point);
        }

        /// <summary>
        /// Reacts to selection of the Cancel button in the dialog.
        /// </summary>
        /// <param name="wnd">The currently active control (not used)</param>
        internal override void DialAbort(Control wnd)
        {
            // Destroy any sub-dialogs we have going.
            KillDialogs();

            // Get the base class to finish off.
            base.DialAbort(wnd);
        }

        /// <summary>
        /// Gets rid of any active sub-dialog(s).
        /// </summary>
        void KillDialogs()
        {
            if (m_DialFrom!=null)
            {
                m_DialFrom.Dispose();
                m_DialFrom = null;
            }

            if (m_DialTo!=null)
            {
                m_DialTo.Dispose();
                m_DialTo = null;
            }

            if (m_DialPath!=null)
            {
                m_DialPath.Dispose();
                m_DialPath = null;
            }

            if (m_DialUp!=null)
            {
                m_DialUp.Dispose();
                m_DialUp = null;
            }
        }

        /// <summary>
        /// Reacts to action that concludes the command dialog.
        /// </summary>
        /// <param name="wnd">The dialog window. If this matches the dialog that
        /// this command knows about, the command will be executed (and, on success,
        /// the dialog will be destroyed). If it's some other window, it must
        /// be a sub-dialog created by our guy, so let it handle the request.</param>
        /// <returns>True if command finished ok. This implementation always
        /// returns <c>false</c>.</returns>
        internal override bool DialFinish(Control wnd)
        {
            // Get rid of sub-dialog(s).
            KillDialogs();

            // Get the base class to finish off.
            return FinishCommand();
        }

        /// <summary>
        /// Handles stuff when user clicks on the "Back" button on one of the point sub-dialogs.
        /// </summary>
        internal void OnPointBack()
        {
            if (m_DialTo!=null)
            {
                StopTo();
                StartFrom();
            }
        }

        /// <summary>
        /// Handles stuff when user clicks on the "Next" button on one of the point sub-dialogs.
        /// </summary>
        internal void OnPointNext()
        {
            if (m_DialFrom!=null)
            {
                StopFrom();
                StartTo();
            }
            else if (m_DialTo!=null)
            {
                StopTo();
                StartPath();
            }
        }

        /// <summary>
        /// Handles stuff when user clicks on the "Cancel" button on one of the point sub-dialogs.
        /// </summary>
        internal void OnPointCancel()
        {
            // Destroy the whole shooting match.
            DialAbort(null);
        }

        /// <summary>
        /// Start update dialog (if applicable).
        /// </summary>
        /// <returns>True if update dialog started.</returns>
        bool StartUpdate()
        {
            // Return if we're not doing an update.
            UpdateUI up = this.Update;
            if (up==null)
                return false;

            m_DialUp = new UpdatePathForm(up);
            m_DialUp.Show();
            return true;
        }

        /// <summary>
        /// Starts the from-point sub-dialog.
        /// </summary>
        void StartFrom()
        {
            m_DialFrom = new GetPointForm(this, "Starting Point", Color.DarkBlue, false);
            m_DialFrom.Show();
            m_DialFrom.OnSelectPoint(m_From, false);
        }

        /// <summary>
        /// Stops the from-point sub-dialog.
        /// </summary>
        void StopFrom()
        {
            if (m_DialFrom != null)
            {
                m_From = m_DialFrom.Point;
                m_DialFrom.Dispose();
                m_DialFrom = null;
            }

            if (m_From!=null)
                m_From.Draw(ActiveDisplay, Color.DarkBlue);
        }

        /// <summary>
        /// Starts the to-point sub-dialog.
        /// </summary>
        void StartTo()
        {
            m_DialTo = new GetPointForm(this, "Finishing Point", Color.Cyan, true);
            m_DialTo.Show();
            m_DialTo.OnSelectPoint(m_To, false);
        }

        /// <summary>
        /// Stops the to-point sub-dialog.
        /// </summary>
        void StopTo()
        {
            if (m_DialTo != null)
            {
                m_To = m_DialTo.Point;
                m_DialTo.Dispose();
                m_DialTo = null;
            }

            if (m_To!=null)
                m_To.Draw(ActiveDisplay, Color.Cyan);
        }

        /// <summary>
        /// Starts the main connection path sub-dialog.
        /// </summary>
        void StartPath()
        {
            Debug.Assert(m_DialFrom==null);
            Debug.Assert(m_DialTo==null);

            if (m_From==null || m_To==null)
                throw new Exception("Terminal points are unavailable.");

            m_DialPath = new PathForm(this, m_From, m_To);
            m_DialPath.Show();
        }
    }
}
