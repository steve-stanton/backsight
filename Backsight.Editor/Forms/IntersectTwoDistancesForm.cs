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
using Backsight.Environment;
using Backsight.Editor.UI;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" was="CdIntersectDist" />
    /// <summary>
    /// Dialog for the Intersect - Two Distances command.
    /// </summary>
    /// <remarks>This was formerly the CdIntersectDist dialog, which was a CPropertySheet
    /// containing two CdGetDist objects and a CdIntersectTwo object.</remarks>
    partial class IntersectTwoDistancesForm : IntersectForm
    {
        #region Constructors

        /// <summary>
        /// Creates a new <c>IntersectTwoDistancesForm</c>
        /// </summary>
        /// <param name="cmd">The command displaying this dialog (not null)</param>
        /// <param name="title">The string to display in the form's title bar</param>
        internal IntersectTwoDistancesForm(CommandUI cmd, string title)
            : base(cmd, title)
        {
            InitializeComponent();
        }

        #endregion

        private void IntersectTwoDistancesForm_Shown(object sender, EventArgs e)
        {
            // Initialize the first page last, to ensure focus is on the initial text box
            // of the first page.
            intersectInfo.InitializeControl(this);
            getDistance1.InitializeControl(this, 1);
            getDistance2.InitializeControl(this, 2);
        }

        internal override void OnDraw(PointFeature point)
        {
            getDistance1.OnDrawAll();
            getDistance2.OnDrawAll();

            if (intersectInfo.Visible)
            {
                intersectInfo.OnDraw();

                IPosition x = intersectInfo.Intersection;
                if (x!=null)
                {
                    ISpatialDisplay display = GetCommand().ActiveDisplay;
                    IDrawStyle style = EditingController.Current.Style(Color.Magenta);

                    if (getDistance1.LineType!=null)
                    {
                        PointFeature p = getDistance1.From;
                        style.Render(display, new IPosition[] { p, x });
                    }

                    if (getDistance2.LineType!=null)
                    {
                        PointFeature p = getDistance2.From;
                        style.Render(display, new IPosition[] { p, x });
                    }
                }
            }
        }

        /// <summary>
        /// Reacts to the selection of a point feature.
        /// </summary>
        /// <param name="point">The point (if any) that has been selected.</param>
        internal override void OnSelectPoint(PointFeature point)
        {
            if (getDistance1.Visible)
                getDistance1.OnSelectPoint(point);
            else if (getDistance2.Visible)
                getDistance2.OnSelectPoint(point);
        }

        /// <summary>
        /// Reacts to the selection of a line feature (by doing nothing)
        /// </summary>
        /// <param name="line">The line (if any) that has been selected.</param>
        internal override void OnSelectLine(LineFeature line)
        {
            // do nothing
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
                return SaveDistDist();

            // Remember the changes as part of the UI object (the original edit remains
            // unchanged for now)
            IntersectTwoDistancesOperation op = (IntersectTwoDistancesOperation)up.GetOp();
            UpdateItemCollection changes = op.GetUpdateItems(getDistance1.ObservedDistance,
                                                             getDistance1.From,
                                                             getDistance2.ObservedDistance,
                                                             getDistance2.From,
                                                             intersectInfo.IsDefault);
            if (!up.AddUpdate(op, changes))
                return null;

            // Return the point previously created at the intersect
            return op.IntersectionPoint;
        }

        /// <summary>
        /// Saves a distance-distance intersection. 
        /// </summary>
        /// <returns>The point feature at the intersection (null if something went wrong).</returns>
        PointFeature SaveDistDist()
        {
            IntersectTwoDistancesOperation op = null;

            try
            {
                Observation dist1 = getDistance1.ObservedDistance;
                PointFeature from1 = getDistance1.From;
                IEntity e1 = getDistance1.LineType;

                Observation dist2 = getDistance2.ObservedDistance;
                PointFeature from2 = getDistance2.From;
                IEntity e2 = getDistance2.LineType;

                IdHandle pointId = intersectInfo.PointId;
                bool isdefault = intersectInfo.IsDefault;

                op = new IntersectTwoDistancesOperation(dist1, from1, dist2, from2, isdefault);
                op.Execute(pointId, e1, e2);
                return op.IntersectionPoint;
            }

            catch (Exception ex)
            {
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
            Observation dist1 = getDistance1.ObservedDistance;
            PointFeature from1 = getDistance1.From;
            if (from1==null || dist1==null)
                return null;

            Observation dist2 = getDistance2.ObservedDistance;
            PointFeature from2 = getDistance2.From;
            if (from2==null || dist2==null)
                return null;

            bool isdefault = intersectInfo.IsDefault;

            IPosition xsect, xsect1, xsect2;
            if (IntersectTwoDistancesOperation.Calculate(dist1, from1, dist2, from2, isdefault,
                                                            out xsect, out xsect1, out xsect2))
                return xsect;

            return null;
        }

        private void finishPage_CloseFromBack(object sender, Gui.Wizard.PageEventArgs e)
        {
            // The intersection and any connecting lines should only be visible when
            // the finish page is on screen.
            ErasePainting();
        }
    }
}