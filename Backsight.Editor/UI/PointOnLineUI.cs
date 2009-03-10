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
    class PointOnLineUI : SimpleCommandUI, IDisposable
    {
        #region Class data

        /// <summary>
        /// The line being subdivided.
        /// </summary>
        LineFeature m_Line;

        /// <summary>
        /// The dialog for the command.
        /// </summary>
        PointOnLineControl m_Dialog;

        /// <summary>
        /// The distance to the split point.
        /// </summary>
        Distance m_Length;

        /// <summary>
        /// True if distance is from the end of the line. False if from start.
        /// </summary>
        bool m_IsFromEnd;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for a fresh edit.
        /// </summary>
        /// <param name="cc">The container for any dialogs</param>
        /// <param name="action">The action that initiated this command</param>
        /// <param name="line">The line being subdivided.</param>
        internal PointOnLineUI(IControlContainer cc, IUserAction action, LineFeature line)
            : base(cc, action)
        {
            SetInitialValues();
            m_Line = line;
        }
    
        /// <summary>
        /// Constructor for command recall.
        /// </summary>
        /// <param name="cc">The container for any dialogs</param>
        /// <param name="action">The action that initiated this command</param>
        /// <param name="op">The operation that's being recalled.</param>
        /// <param name="line">The line to subdivide.</param>
        internal PointOnLineUI(IControlContainer cc, IUserAction action, Operation op, LineFeature line)
            : base(cc, action, null, op)
        {
            SetInitialValues();
            m_Line = line;
        }
    
        /// <summary>
        /// Constructor for doing an update.
        /// </summary>
        /// <param name="cc">The container for any dialogs</param>
        /// <param name="action">The action that initiated this command</param>
        /// <param name="updcmd">The update command.</param>
        internal PointOnLineUI(IControlContainer cc, IUserAction action, UpdateUI updcmd)
            : base(cc, action, updcmd)
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
	        m_IsFromEnd = false;
        }

        /// <summary>
        /// Starts the user interface (if any) for this command.
        /// </summary>
        /// <returns>True if command started ok.</returns>
        internal override bool Run()
        {
            // Don't run more than once.
            if (m_Dialog!=null)
                throw new InvalidOperationException("PointOnLineUI.Run - Command is already running.");

            UpdateUI pup = this.Update;

            if (pup!=null)
                m_Dialog = new PointOnLineControl(pup);
            else
                m_Dialog = new PointOnLineControl(this, m_Line, this.Recall);

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
                MessageBox.Show("PointOnLineUI.DialFinish - No dialog!");
                return false;
            }

            // If we are doing an update, alter the original operation.
            UpdateUI up = this.Update;

            if (up!=null)
            {
                // Get the original operation.
                PointOnLineOperation pop = (up.GetOp() as PointOnLineOperation);
                if (pop==null)
                {
                    MessageBox.Show("PointOnLineUI.DialFinish - Unexpected edit type.");
                    return false;
                }

                // Make the update.
                pop.Correct(m_Dialog.Length, m_Dialog.IsFromEnd);
            }
            else
            {
                // De-select the line that's being split
                Controller.ClearSelection();

                // Get info from the dialog.
                m_IsFromEnd = m_Dialog.IsFromEnd;
                m_Length = m_Dialog.Length;

                // Execute the edit
                PointOnLineOperation op = null;

                try
                {
                    op = new PointOnLineOperation(Session.WorkingSession);
                    op.Execute(m_Line, m_Length, m_IsFromEnd);
                }

                catch (Exception ex)
                {
                    Session.WorkingSession.Remove(op);
                    MessageBox.Show(ex.Message);
                    return false;
                }
            }

            // Destroy the dialog(s).
            KillDialogs();

            // Get the base class to finish up.
            return FinishCommand();
        }

        /// <summary>
        /// Calculates the positions of the split point.
        /// </summary>
        /// <param name="line">The line being subdivided.</param>
        /// <param name="dist">The distance to the split point.</param>
        /// <param name="isFromEnd">Is the distance from the end of the line?</param>
        /// <returns>The calculated position (null if the distance is longer than the line being subdivided,
        /// or supplied information is incomplete)</returns>
        internal static IPosition Calculate(LineFeature line, Distance dist, bool isFromEnd)
        {
        	// Can't calculate if there is insufficient data.
            if (line==null || dist==null)
                return null;

            // The length must be defined.
            if (!dist.IsDefined)
                return null;

            // Return if the observed distance is longer than the total
	        // length of the line.
            double maxlen = line.Length.Meters;
            double obsvlen = dist.Meters;
            if (obsvlen > maxlen)
                return null;

            // Get the approximate position of the split point.
            IPosition start, approx;
            LineGeometry g = line.LineGeometry;
            if (isFromEnd)
            {
                start = line.EndPoint;
                g.GetPosition(new Length(maxlen-obsvlen), out approx);
            }
            else
            {
                start = line.StartPoint;
                g.GetPosition(new Length(obsvlen), out approx);
            }

            // Get the distance to the approximate position on the mapping plane.
            ICoordinateSystem sys = CadastralMapModel.Current.CoordinateSystem;
            double planlen = dist.GetPlanarMetric(start, approx, sys);

            // Figure out the true position on the line.
            IPosition splitpos;
            if (isFromEnd)
                g.GetPosition(new Length(maxlen-planlen), out splitpos);
            else
                g.GetPosition(new Length(planlen), out splitpos);

            return splitpos;
        }
    }
}
