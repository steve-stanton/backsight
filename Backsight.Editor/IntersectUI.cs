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
using System.Windows.Forms;
using System.Diagnostics;

using Backsight.Forms;
using Backsight.Editor.Forms;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="01-DEC-1999" was="CuiIntersect" />
    /// <summary>
    /// User interface for an Intersect command.
    /// </summary>
    class IntersectUI : SimpleCommandUI, IDisposable
    {
        #region Class data

        /// <summary>
        /// The dialog for the command.
        /// </summary>
        IntersectForm m_Dialog;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for a brand new intersection.
        /// </summary>
        /// <param name="action">The action that initiated the command</param>
        internal IntersectUI(IUserAction action)
            : base(action)
        {
            Initialize();
        }

        /// <summary>
        /// Constructor for command recall.
        /// </summary>
        /// <param name="action">The action that initiated this command</param>
        /// <param name="op">The operation that's being recalled.</param>
        internal IntersectUI(IUserAction action, Operation op)
            : base(null, action, null, op)
        {
            Initialize();
        }

        /// <summary>
        /// Constructor for doing an update.
        /// </summary>
        /// <param name="action">The action that initiated this command</param>
        /// <param name="updcmd">The update command.</param>
        internal IntersectUI(IUserAction action, UpdateUI updcmd)
            : base(null, action, updcmd)
        {
            Initialize();
        }

        #endregion

        /// <summary>
        /// Initializes this UI (for use by constructors)
        /// </summary>
        void Initialize()
        {
            Debug.Assert(this.EditId != EditingActionId.Null);
            m_Dialog = null;
        }

        public override void Dispose()
        {
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
                throw new InvalidOperationException("IntersectUI.Run - Command is already running.");

            // Are we doing an update?
            UpdateUI pup = this.Update;

            // Create the appropriate sort of dialog.
            EditingActionId edid = this.EditId;

            if (edid == EditingActionId.DirIntersect)
            {
                if (pup==null)
                    m_Dialog = new IntersectTwoDirectionsForm(this, "Intersect two directions");
                else
                    m_Dialog = new IntersectTwoDirectionsForm(pup, "Update (intersect two directions)");
            }
            else if (edid == EditingActionId.DirDistIntersect)
            {
                if (pup==null)
                    m_Dialog = new IntersectDirectionAndDistanceForm(this, "Intersect direction and distance");
                else
                    m_Dialog = new IntersectDirectionAndDistanceForm(this, "Update (intersect direction and distance)");
            }
            else if (edid == EditingActionId.LineIntersect)
            {
                /*
                if (pup==null)
                    m_Dialog = new IntersectTwoLinesForm(this, "Intersect two lines");
                else
                    m_Dialog = new IntersectTwoLinesForm(this, "Update (intersect two lines)");
                 */
            }
            else if (edid == EditingActionId.DistIntersect)
            {
                if (pup==null)
                    m_Dialog = new IntersectTwoDistancesForm(this, "Intersect two distances");
                else
                    m_Dialog = new IntersectTwoDistancesForm(this, "Update (intersect two distances)");
            }
            else if (edid == EditingActionId.DirLineIntersect)
            {
                if (pup==null)
                    m_Dialog = new IntersectDirectionAndLineForm(this, "Intersect direction and line");
                else
                    m_Dialog = new IntersectDirectionAndLineForm(this, "Update (intersect direction and line)");
            }
            else
            {
                throw new Exception("IntersectUI.Run - Unexpected command id.");
            }

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
                m_Dialog.OnDraw(point);
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
        /// Reacts to the selection of a line feature.
        /// </summary>
        /// <param name="line">The line (if any) that has been selected.</param>
        internal override void OnSelectLine(LineFeature line)
        {
            if (m_Dialog!=null)
                m_Dialog.OnSelectLine(line);
        }

        /// <summary>
        /// Reacts to selection of the OK button in the dialog.
        /// </summary>
        /// <param name="wnd">The dialog window (ignored).</param>
        /// <returns></returns>
        internal override bool DialFinish(Control wnd)
        {
            if (m_Dialog==null)
            {
                MessageBox.Show("IntersectUI.DialFinish - No dialog!");
                return false;
            }

            PointFeature p = m_Dialog.Finish();
            if (p==null)
                return false;

            // Get the controller to select the intersection point
            ISpatialSelection ss = new SpatialSelection(p);
            Controller.SetSelection(ss);

            m_Dialog.Dispose();
            m_Dialog = null;
            return FinishCommand();
        }
    }
}
