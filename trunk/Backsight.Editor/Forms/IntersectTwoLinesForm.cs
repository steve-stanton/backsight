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
using Backsight.Editor.Operations;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" was="CdIntersectLine" />
    /// <summary>
    /// Dialog for the Intersect - Two Lines command.
    /// </summary>
    /// <remarks>This was formerly the CdIntersectDirLine dialog, which was a CPropertySheet
    /// containing two CdGetLine dialogs, and a CdIntersect.</remarks>
    partial class IntersectTwoLinesForm : IntersectForm
    {
        #region Constructors

        /// <summary>
        /// Creates a new <c>IntersectTwoLinesForm</c>
        /// </summary>
        /// <param name="cmd">The command displaying this dialog (not null)</param>
        /// <param name="title">The string to display in the form's title bar</param>
        public IntersectTwoLinesForm(CommandUI cmd, string title)
            : base(cmd, title)
        {
            InitializeComponent();
        }

        #endregion

        private void IntersectTwoLinesForm_Shown(object sender, EventArgs e)
        {
            // Initialize the first page last, to ensure focus is on the initial text box
            // of the first page.
            intersectInfo.InitializeControl(this);
            getLine2.InitializeControl(this, 2);
            getLine1.InitializeControl(this, 1);

            getLine1.SetLineColor(Color.DarkBlue);
            getLine2.SetLineColor(Color.LightBlue);
        }

        internal override void OnDraw(PointFeature point)
        {
            getLine1.OnDraw();
            getLine2.OnDraw();

            if (intersectInfo.Visible)
                intersectInfo.OnDraw();
        }

        /// <summary>
        /// Reacts to the selection of a point feature.
        /// </summary>
        /// <param name="point">The point (if any) that has been selected.</param>
        internal override void OnSelectPoint(PointFeature point)
        {
            if (getLine1.Visible)
                getLine1.OnSelectPoint(point);
            else if (getLine2.Visible)
                getLine2.OnSelectPoint(point);
            else if (intersectInfo.Visible)
                intersectInfo.OnSelectPoint(point);
        }

        /// <summary>
        /// Reacts to the selection of a line feature.
        /// </summary>
        /// <param name="line">The line (if any) that has been selected.</param>
        internal override void OnSelectLine(LineFeature line)
        {
            if (getLine1.Visible)
                getLine1.OnSelectLine(line);
            else if (getLine2.Visible)
                getLine2.OnSelectLine(line);
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
            if (x == null)
            {
                MessageBox.Show("No intersection. Nothing to save");
                return null;
            }

            // Save the intersect point if we're not updating
            IntersectOperation upd = GetUpdateOp();
            if (upd==null)
                return SaveLineLine();

            // Apply corrections and return the point previously created at the intersect
            Correct(upd);
            return upd.IntersectionPoint;
        }

        /// <summary>
        /// Saves a line-line intersection. 
        /// </summary>
        /// <returns>The point feature at the intersection (null if something went wrong).</returns>
        PointFeature SaveLineLine()
        {
            IntersectTwoLinesOperation op = null;

            try
            {
                LineFeature line1 = getLine1.Line;
                bool wantSplit1 = getLine1.WantSplit;

                LineFeature line2 = getLine2.Line;
                bool wantSplit2 = getLine2.WantSplit;

                IdHandle pointId = intersectInfo.PointId;
                PointFeature closeTo = intersectInfo.ClosestPoint;

                op = new IntersectTwoLinesOperation();
                op.Execute(line1, line2, closeTo, wantSplit1, wantSplit2, pointId);
                return op.IntersectionPoint;
            }

            catch (Exception ex)
            {
                Session.WorkingSession.Remove(op);
                MessageBox.Show(ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Correct an edit using the info from this dialog.
        /// </summary>
        void Correct(IntersectOperation io)
        {
            IntersectTwoLinesOperation op = (IntersectTwoLinesOperation)io;
            op.Correct(getLine1.Line, getLine2.Line, intersectInfo.ClosestPoint,
                            getLine1.WantSplit, getLine2.WantSplit);
        }

        private void finishPage_ShowFromNext(object sender, EventArgs e)
        {
            // Enable finish button only if we have an intersection
            IPosition x = CalculateIntersect();
            wizard.NextEnabled = (x != null);
        }

        /// <summary>
        /// Attempts to calculate the position of the intersect, using the currently
        /// entered information.
        /// </summary>
        /// <returns>The position of the intersect (null if there isn't one)</returns>
        internal override IPosition CalculateIntersect()
        {
            LineFeature line1 = getLine1.Line;
            if (line1 == null)
                return null;

            LineFeature line2 = getLine2.Line;
            if (line2 == null)
                return null;

            // The closest point may be null if the finish page has never been shown
            PointFeature closeTo = intersectInfo.ClosestPoint;
            IPosition xsect;
            PointFeature closest;
            line1.Intersect(line2, closeTo, out xsect, out closest);
            return xsect;
        }

        /// <summary>
        /// Returns the point feature closest to an intersection involving one or more line features.
        /// </summary>
        /// <returns>Null (always). Dialogs that involve instances of <see cref="GetLineControl"/>
        /// are expected to override.</returns>
        internal override PointFeature GetDefaultClosestPoint()
        {
            LineFeature line1 = getLine1.Line;
            if (line1 == null)
                return null;

            LineFeature line2 = getLine2.Line;
            if (line2 == null)
                return null;

            // The closest point may be null if the finish page has never been shown
            PointFeature closeTo = intersectInfo.ClosestPoint;
            IPosition xsect;
            PointFeature closest;
            line1.Intersect(line2, closeTo, out xsect, out closest);
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