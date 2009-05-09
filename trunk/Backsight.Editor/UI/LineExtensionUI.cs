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

using Backsight.Environment;
using Backsight.Editor.Forms;
using Backsight.Forms;
using Backsight.Editor.Operations;
using Backsight.Editor.Observations;

namespace Backsight.Editor.UI
{
    /// <written by="Steve Stanton" on="06-DEC-1998" was="CuiArcExtend" />
    /// <summary>
    /// User interface for extending a line.
    /// </summary>
    class LineExtensionUI : SimpleCommandUI, IDisposable
    {
        #region Class data

        /// <summary>
        /// The line being extended.
        /// </summary>
        LineFeature m_ExtendLine;

        /// <summary>
        /// The dialog for the command.
        /// </summary>
        LineExtensionControl m_Dialog;

        /// <summary>
        /// The length of the extension.
        /// </summary>
        Distance m_Length;

        /// <summary>
        /// True if extending from the end of m_ExtendLine. False if from the start.
        /// </summary>
        bool m_IsExtendFromEnd;	// 

        /// <summary>
        /// The entity type for the extension line (if any).
        /// </summary>
        IEntity m_LineType;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor to extend the currently selected line.
        /// </summary>
        /// <param name="cc">The container for any dialogs</param>
        /// <param name="action">The action that initiated this command</param>
        /// <exception cref="InvalidOperationException">If a line is not currently selected</exception>
        internal LineExtensionUI(IControlContainer cc, IUserAction action)
            : base(cc, action)
        {
            LineFeature line = EditingController.SelectedLine;
            if (line == null)
                throw new InvalidOperationException("You must initially select the line you want to extend.");

            // The dialog will be created by Run().
            m_Dialog = null;

            // Remember the line that is being extended.
            m_ExtendLine = line;

            // And initialize the parameters for the operation's Execute() call.
            m_Length = null;
            m_IsExtendFromEnd = true;
            m_LineType = null;
        }

        /// <summary>
        /// Constructor for doing an update.
        /// </summary>
        /// <param name="editId">The ID of the edit this command deals with.</param>
        /// <param name="updcmd">The update command.</param>
        internal LineExtensionUI(IControlContainer cc, EditingActionId editId, UpdateUI updcmd)
            : base(cc, editId, updcmd)
        {
            // The dialog will be created by Run().
            m_Dialog = null;

            // The line we extended is known via the update.
            m_ExtendLine = null;

            // And initialize the parameters for the operation's Execute() call.
            m_Length = null;
            m_IsExtendFromEnd = true;
            m_LineType = null;
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
                throw new InvalidOperationException("LineExtensionUI.Run - Command is already running.");

            // Are we doing an update?
            UpdateUI pup = this.Update;

            if (pup!=null)
                m_Dialog = new LineExtensionControl(pup);
            else
                m_Dialog = new LineExtensionControl(this, m_ExtendLine, this.Recall);

            this.Container.Display(m_Dialog);
            return true;
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
        /// Override indicates that this command performs painting. This means that the
        /// controller will periodically call the <see cref="Paint"/> method (probably during
        /// idle time).
        /// </summary>
        internal override bool PerformsPainting
        {
            get { return (m_Dialog!=null); }
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
                MessageBox.Show("LineExtensionUI.DialFinish - No dialog!");
                return false;
            }

            // If we are doing an update, alter the original operation.
            UpdateUI up = this.Update;

            if (up!=null)
            {
                // Get the original operation.
                LineExtensionOperation pop = (up.GetOp() as LineExtensionOperation);
                if (pop==null)
                {
                    MessageBox.Show("LineExtensionUI.DialFinish - Unexpected edit type.");
                    return false;
                }

                // Make the update.
                pop.Correct(m_Dialog.IsExtendFromEnd, m_Dialog.Length);
            }
            else
            {
        		// Get info from the dialog
                m_IsExtendFromEnd = m_Dialog.IsExtendFromEnd;
                m_Length = m_Dialog.Length;
                IdHandle idh = m_Dialog.PointId;
                CadastralMapModel map = CadastralMapModel.Current;
                m_LineType = (m_Dialog.WantLine ? map.DefaultLineType : null);

                // Execute the edit
                LineExtensionOperation op = null;

                try
                {
                    op = new LineExtensionOperation(Session.WorkingSession);
                    op.Execute(m_ExtendLine, m_IsExtendFromEnd, m_Length, idh, m_LineType);
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
        /// Calculates the start and end positions of a straight line extension.
        /// </summary>
        /// <param name="extendLine">The line being extended.</param>
        /// <param name="isFromEnd">True if extending from the end of the line.</param>
        /// <param name="dist">The length of the extension.</param>
        /// <param name="start">The position of the start of the extension.</param>
        /// <param name="end">The position of the end of the extension.</param>
        /// <returns>True if position have been worked out. False if there is insufficient data,
        /// or the extension is not straight (in that case, the start and end positions come
        /// back as nulls)</returns>
        internal static bool Calculate(LineFeature extendLine, bool isFromEnd, Distance dist,
                                        out IPosition start, out IPosition end)
        {
            start = end = null;

            // Can't calculate if there is insufficient data.
            if (extendLine==null || dist==null)
                return false;

            // The length must be defined.
            if (!dist.IsDefined)
                return false;

            // The line that's being extended can't be a circular arc.
            if (extendLine is ArcFeature)
                return false;

            // Get the point we're extending from.
            start = (isFromEnd ? extendLine.EndPoint : extendLine.StartPoint);

            // Get the point we're extending to ...

            // Get the orientation point. For multi-segments, we use the
            // point prior to the appropriate end point.
            IPosition orient = extendLine.LineGeometry.GetOrient(!isFromEnd, 0.0);

            // Get the bearing from the orientation point to the start of the extension.
            Turn turn = new Turn(orient, start);
            double bearing = turn.BearingInRadians;

            // Get the distance on the mapping plane.
            double plandist = dist.GetPlanarMetric(start, bearing, extendLine.CoordinateSystem);

            // Figure out the end of the extension.
            end = Geom.Polar(start, bearing, plandist);
            return true;
        }

        /// <summary>
        /// Calculates the start and end positions of an extension to a circular arc.
        /// </summary>
        /// <param name="extendLine">The line being extended.</param>
        /// <param name="isFromEnd">True if extending from the end of the line.</param>
        /// <param name="dist">The length of the extension.</param>
        /// <param name="start">The position of the start of the extension.</param>
        /// <param name="end">The position of the end of the extension.</param>
        /// <param name="center">The center of the circle on which the arc lies.</param>
        /// <param name="iscw">Is the circular arc directed clockwise?</param>
        /// <returns>True if position have been worked out. False if there is insufficient data,
        /// or the extension is not on a circular arc, or the length is more than the circumference
        /// of the circle (in those cases, the start and end positions come back as nulls)</returns>
        internal static bool Calculate(LineFeature extendLine, bool isFromEnd, Distance dist,
                                        out IPosition start, out IPosition end, out IPosition center, out bool iscw)
        {
            start = end = null;
            center = null;
            iscw = true;

            // Can't calculate if there is insufficient data.
            if (extendLine==null || dist==null)
                return false;

            // The length must be defined.
            if (!dist.IsDefined)
                return false;

            // The line that's being extended must be a circular arc.
            ArcFeature arc = (extendLine as ArcFeature);
            if (arc==null)
                return false;

            center = arc.Circle.Center;
            double radius = arc.Circle.Radius;
            iscw = arc.IsClockwise;

            // Get the length of the arc extension, in meters on the ground.
            double arclen = dist.Meters;

            // If the arc length exceeds the length of the circumference,
            // the end point can't be calculated.
            double circumf = Constants.PIMUL2 * radius;
            if (arclen > circumf)
                return false;

            // If we're extending from the start of the arc, the curve direction has
            // to be reversed too.
            if (!isFromEnd)
                iscw = !iscw;

            // Get the point we're extending from.
            start = (isFromEnd ? extendLine.EndPoint : extendLine.StartPoint);

            // Get the point we're extending to ...

            // Get the bearing from the center of the circle to the start of the arc.
            Turn turn = new Turn(center, start);
            double sbearing = turn.BearingInRadians;

            // Get the sector angle (in radians).
            double sector = arclen / radius;

            double ebearing = sbearing;
            if (iscw)
                ebearing += sector;
            else
                ebearing -= sector;

            end = Geom.Polar(center, ebearing, radius);

            // Re-calculate the arc length on the mapping plane,
            arclen = dist.GetPlanarMetric(start, end, extendLine.CoordinateSystem);

            // And adjust the end position accordingly.
            sector = arclen / radius;

            if (iscw)
                ebearing = sbearing + sector;
            else
                ebearing = sbearing - sector;

            end = Geom.Polar(center, ebearing, radius);
            return true;
        }
    }
}
