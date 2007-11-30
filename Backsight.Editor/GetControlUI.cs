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

using Backsight.Forms;
using Backsight.Editor.Forms;
using System.Windows.Forms;
using System.Drawing;
using Backsight.Environment;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="" />
    /// <summary>
    /// 
    /// </summary>
    class GetControlUI : SimpleCommandUI, IDisposable
    {
        #region Class data

        /// <summary>
        /// The dialog for getting control points
        /// </summary>
        GetControlForm m_Dialog;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>GetControlUI</c>
        /// </summary>
        /// <param name="action">The action that initiated this command</param>
        internal GetControlUI(IUserAction action)
            : base(action)
        {
            m_Dialog = null;
        }

        #endregion

        public override void Dispose()
        {
            KillDialogs();
        }

        /// <summary>
        /// Starts the user interface (if any) for this command.
        /// </summary>
        /// <returns>True if command started ok.</returns>
        internal override bool Run()
        {
            if (m_Dialog!=null)
                throw new InvalidOperationException("GetControlUI.Run - Dialog is already on screen");

            // Display the dialog for getting control points
            m_Dialog = new GetControlForm(this);
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
            get { return (m_Dialog!=null); }
        }

        /// <summary>
        /// Does any command-specific drawing.
        /// </summary>
        /// <param name="point">The specific point (if any) that the parent window has drawn. Not used.</param>
        internal override void Paint(PointFeature point)
        {
            if (m_Dialog!=null)
            {
                ISpatialDisplay display = ActiveDisplay;
                IDrawStyle style = Controller.Style(Color.Magenta);
                m_Dialog.Render(display, style);
            }
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
                MessageBox.Show("GetControlUI.DialFinish - No dialog!");
                return false;
            }

            // Destroy the dialog(s).
            KillDialogs();

            // Get the base class to finish up.
            return FinishCommand();
        }
    }
}
