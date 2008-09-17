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
using System.Diagnostics;
using System.ComponentModel;

using Backsight.Editor.Operations;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" was="CdDialog" />
    /// <summary>
    /// Base class for dialogs dealing with Intersect operations.
    /// </summary>
    partial class IntersectForm : Form
    {
        #region Class data

        /// <summary>
        /// The command displaying this dialog (either an instance of <see cref="IntersectUI"/>
        /// or <see cref="UpdateUI"/>). Nulled when the command is saved.
        /// </summary>
        CommandUI m_Cmd;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor, for the sake of VisualStudio designer.
        /// </summary>
        internal IntersectForm()
        {
            InitializeComponent();
            m_Cmd = null;
        }

        /// <summary>
        /// Creates a new <c>IntersectForm</c>
        /// </summary>
        /// <param name="cmd">The command displaying this dialog (not null)</param>
        /// <param name="title">The string to display in the form's title bar</param>
        protected IntersectForm(CommandUI cmd, string title)
        {
            InitializeComponent();
            this.Text = title;
            m_Cmd = cmd;
        }

        #endregion

        /// <summary>
        /// The command displaying this dialog (either an instance of <see cref="IntersectUI"/>
        /// or <see cref="UpdateUI"/>). Nulled when the command is saved.
        /// </summary>
        protected CommandUI GetCommand()
        {
            return m_Cmd;
        }

        internal virtual void OnDraw(PointFeature point)
        {
            throw new NotImplementedException("IntersectForm.OnDraw - not implemented by derived class");
        }

        /// <summary>
        /// Performs any processing on selection of a point feature.
        /// An implementation must be provided by derived classes, since this implementation
        /// just throws an exception, indicating that it needs to be implemented. While it
        /// would be preferable to declare this as an abstract method, an attempt to do
        /// so causes VisualStudio to complain (the designer needs to create an instance
        /// of the base class).
        /// </summary>
        /// <param name="point">The point that has just been selected.</param>
        internal virtual void OnSelectPoint(PointFeature point)
        {
            throw new NotImplementedException("IntersectForm.OnSelectPoint - not implemented by derived class");
        }

        /// <summary>
        /// Performs any processing on selection of a line feature.
        /// An implementation must be provided by derived classes, since this implementation
        /// just throws an exception, indicating that it needs to be implemented. While it
        /// would be preferable to declare this as an abstract method, an attempt to do
        /// so causes VisualStudio to complain (the designer needs to create an instance
        /// of the base class).
        /// </summary>
        /// <param name="point">The line that has just been selected.</param>
        internal virtual void OnSelectLine(LineFeature line)
        {
            throw new NotImplementedException("IntersectForm.OnSelectLine - not implemented by derived class");
        }

        /// <summary>
        /// Moves to the next wizard page (so long as the current page isn't the finish page).
        /// </summary>
        internal void AdvanceToNextPage()
        {
            if (!wizard.Page.IsFinishPage)
                wizard.Next();
        }

        internal IntersectOperation GetUpdateOp()
        {
            UpdateUI up = (m_Cmd as UpdateUI);
            if (up==null)
                return null;
            else
                return (up.GetOp() as IntersectOperation);
        }

        /// <summary>
        /// Handles click on the wizard Finish button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wizard_Finish(object sender, EventArgs e)
        {
            // The call to IntersectUI.DialFinish will usually lead to a call
            // to the Finish method below. It won't if we're doing an update.
            if (m_Cmd!=null && m_Cmd.DialFinish(null))
                m_Cmd = null;
        }

        /// <summary>
        /// Handles the Finish button. This is called by <see cref="IntersectUI.DialFinish"/>.
        /// An implementation must be provided by derived classes, since this implementation
        /// just throws an exception, indicating that it needs to be implemented. While it
        /// would be preferable to declare this as an abstract method, an attempt to do
        /// so causes VisualStudio to complain (the designer needs to create an instance
        /// of the base class).
        /// </summary>
        /// <returns>The point created at the intersection (null if an error was reported).
        /// The caller is responsible for disposing of the dialog and telling the controller
        /// the command is done)</returns>
        internal virtual PointFeature Finish()
        {
            throw new NotImplementedException("IntersectForm.DialFinish");
        }

        private void IntersectForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // If the command hasn't been finished, cancel it now.
            if (m_Cmd != null)
            {
                m_Cmd.DialAbort(null);
                m_Cmd = null;
            }
        }

        /// <summary>
        /// Attempts to calculate the position of the intersect, using the currently
        /// entered information.
        /// An implementation must be provided by derived classes, since this implementation
        /// just throws an exception, indicating that it needs to be implemented. While it
        /// would be preferable to declare this as an abstract method, an attempt to do
        /// so causes VisualStudio to complain (the designer needs to create an instance
        /// of the base class).
        /// </summary>
        /// <returns>The position of the intersect (null if there isn't one)</returns>
        internal virtual IPosition CalculateIntersect()
        {
            if (DesignMode)
                return null;

            throw new NotImplementedException("IntersectForm.CalculateIntersect");
        }

        /// <summary>
        /// Returns the point feature closest to an intersection involving one or more line features.
        /// </summary>
        /// <returns>Null (always). Dialogs that involve instances of <see cref="GetLineControl"/>
        /// are expected to override.</returns>
        internal virtual PointFeature GetDefaultClosestPoint()
        {
            return null;
        }

        /// <summary>
        /// Restores the last draw in the command's active display.
        /// </summary>
        internal void ErasePainting()
        {
            m_Cmd.ActiveDisplay.RestoreLastDraw();
        }
   }
}