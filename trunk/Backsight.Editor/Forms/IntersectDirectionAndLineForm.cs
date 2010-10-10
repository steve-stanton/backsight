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
using System.Drawing;
using System.Diagnostics;

using Backsight.Editor.Operations;
using Backsight.Environment;
using Backsight.Editor.Observations;
using Backsight.Editor.UI;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" was="CdIntersectDirLine" />
    /// <summary>
    /// Dialog for the Intersect - Direction and Line command.
    /// </summary>
    /// <remarks>This was formerly the CdIntersectDirLine dialog, which was a CPropertySheet
    /// containing a CdGetDir, CdGetLine, and a CdIntersect object.</remarks>
    internal partial class IntersectDirectionAndLineForm : IntersectForm
    {
        #region Constructors

        /// <summary>
        /// Creates a new <c>IntersectDirectionAndLineForm</c>
        /// </summary>
        /// <param name="cmd">The command displaying this dialog (not null)</param>
        /// <param name="title">The string to display in the form's title bar</param>
        public IntersectDirectionAndLineForm(CommandUI cmd, string title)
            : base(cmd, title)
        {
            InitializeComponent();
        }

        #endregion

        private void IntersectDirectionAndLineForm_Shown(object sender, EventArgs e)
        {
            // Initialize the first page last, to ensure focus is on the initial text box
            // of the first page.
            intersectInfo.InitializeControl(this);
            getLine.InitializeControl(this, 1);
            getDirection.InitializeControl(this, 1);
        }

        internal override void OnDraw(PointFeature point)
        {
            getDirection.OnDrawAll();
            getLine.OnDraw();

            if (intersectInfo.Visible)
            {
                intersectInfo.OnDraw();

                IPosition x = intersectInfo.Intersection;
                if (x!=null && getDirection.LineType!=null)
                {
                    ISpatialDisplay display = GetCommand().ActiveDisplay;
                    IDrawStyle style = EditingController.Current.Style(Color.Magenta);
                    Direction d = getDirection.Direction;
                    style.Render(display, new IPosition[] { d.StartPosition, x });
                }
            }
        }

        /// <summary>
        /// Reacts to the selection of a point feature.
        /// </summary>
        /// <param name="point">The point (if any) that has been selected.</param>
        internal override void OnSelectPoint(PointFeature point)
        {
            if (getDirection.Visible)
                getDirection.OnSelectPoint(point);
            else if (getLine.Visible)
                getLine.OnSelectPoint(point);
            else if (intersectInfo.Visible)
                intersectInfo.OnSelectPoint(point);
        }

        /// <summary>
        /// Reacts to the selection of a line feature.
        /// </summary>
        /// <param name="line">The line (if any) that has been selected.</param>
        internal override void OnSelectLine(LineFeature line)
        {
            if (getDirection.Visible)
                getDirection.OnSelectLine(line);
            else if (getLine.Visible)
                getLine.OnSelectLine(line);
            else if (intersectInfo.Visible)
                intersectInfo.OnSelectLine(line);
        }

        /// <summary>
        /// Handles the Finish button.
        /// </summary>
        /// <returns>The point created at the intersection (null if an error was reported).
        /// The caller is responsible for disposing of the dialog and telling the controller
        /// the command is done)</returns>
        internal override PointFeature Finish()
        {
            // The intersection SHOULD be defined (the Finish button should have
            // been disabled if it wasn't)
            IPosition x = intersectInfo.Intersection;
            if (x==null)
            {
                MessageBox.Show("No intersection. Nothing to save");
                return null;
            }

            // If we're not doing an update, just save the edit
            UpdateUI up = (GetCommand() as UpdateUI);
            if (up == null)
                return SaveDirLine();

            // Remember the changes as part of the UI object (the original edit remains
            // unchanged for now)
            IntersectDirectionAndLineOperation op = (IntersectDirectionAndLineOperation)up.GetOp();
            UpdateItemCollection changes = op.GetUpdateItems(getDirection.Direction,
                                                             getLine.Line,
                                                             intersectInfo.ClosestPoint);
            if (!up.AddUpdate(op, changes))
                return null;

            // Return the point previously created at the intersect
            return op.IntersectionPoint;
        }

        /// <summary>
        /// Saves a direction-line intersection. 
        /// </summary>
        /// <returns>The point feature at the intersection (null if something went wrong).</returns>
        PointFeature SaveDirLine()
        {
            IntersectDirectionAndLineOperation op = null;

            try
            {
                Direction dir = getDirection.Direction;
                IEntity dirEnt = getDirection.LineType;

                LineFeature line = getLine.Line;
                bool wantSplit = getLine.WantSplit;

                IdHandle pointId = intersectInfo.PointId;
                PointFeature closeTo = intersectInfo.ClosestPoint;

                if (closeTo == null)
                {
                    IPosition xsect;
                    if (!dir.Intersect(line, closeTo, out xsect, out closeTo))
                        throw new Exception("Cannot calculate intersection point");

                    Debug.Assert(closeTo != null);
                }

                op = new IntersectDirectionAndLineOperation(Session.WorkingSession, 0, dir, line,
                                                                wantSplit, closeTo);
                op.Execute(pointId, dirEnt);
                return op.IntersectionPoint;
            }

            catch (Exception ex)
            {
                //Session.WorkingSession.Remove(op);
                MessageBox.Show(ex.StackTrace, ex.Message);
            }

            return null;
        }

        private void finishPage_ShowFromNext(object sender, EventArgs e)
        {
            // Enable finish button only if we have an intersection
            IPosition x = CalculateIntersect();
            wizard.NextEnabled = (x!=null);
        }

        /// <summary>
        /// Attempts to calculate the position of the intersect, using the currently
        /// entered information.
        /// </summary>
        /// <returns>The position of the intersect (null if there isn't one)</returns>
        internal override IPosition CalculateIntersect()
        {
            Direction dir = getDirection.Direction;
            if (dir==null)
                return null;

            LineFeature line = getLine.Line;
            if (line==null)
                return null;

            // The closest point may be null if the finish page has never been shown
            PointFeature closeTo = intersectInfo.ClosestPoint;
            IPosition xsect;
            PointFeature closest;
            dir.Intersect(line, closeTo, out xsect, out closest);
            return xsect;
            /*

            // If we got an intersection, and we did not previously have
            // a close-to point, define that now.
            if (ok && !m_pCloseTo)
            {
                m_pCloseTo = pClosest;
                ShowCloseTo();
            }
            */
        }

        /// <summary>
        /// Returns the point feature closest to an intersection involving one or more line features.
        /// </summary>
        /// <returns>Null (always). Dialogs that involve instances of <see cref="GetLineControl"/>
        /// are expected to override.</returns>
        internal override PointFeature GetDefaultClosestPoint()
        {
            Direction dir = getDirection.Direction;
            if (dir==null)
                return null;

            LineFeature line = getLine.Line;
            if (line==null)
                return null;

            // The closest point may be null if the finish page has never been shown
            PointFeature closeTo = intersectInfo.ClosestPoint;
            IPosition xsect;
            PointFeature closest;
            dir.Intersect(line, closeTo, out xsect, out closest);
            return closest;
        }

        private void finishPage_CloseFromBack(object sender, Gui.Wizard.PageEventArgs e)
        {
            // The intersection and any connecting lines should only be visible when
            // the finish page is on screen.
            ErasePainting();
        }
    }
}

