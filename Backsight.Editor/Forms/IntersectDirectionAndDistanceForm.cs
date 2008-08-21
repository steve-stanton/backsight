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
using System.Drawing;

using Backsight.Editor.Operations;
using Backsight.Environment;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" was="CdIntersectDirDist" />
    /// <summary>
    /// Dialog for the Intersect - Direction and Distance command.
    /// </summary>
    partial class IntersectDirectionAndDistanceForm : IntersectForm
    {
        /// <summary>
        /// Creates a new <c>IntersectTwoDirectionsForm</c>
        /// </summary>
        /// <param name="cmd">The command displaying this dialog (not null)</param>
        /// <param name="title">The string to display in the form's title bar</param>
        internal IntersectDirectionAndDistanceForm(CommandUI cmd, string title)
            : base(cmd, title)
        {
            InitializeComponent();
        }

        private void IntersectDirectionAndDistanceForm_Shown(object sender, EventArgs e)
        {
            // Initialize the first page last, to ensure focus is on the initial text box
            // of the first page.
            intersectInfo.InitializeControl(this);
            getDistance.InitializeControl(this, 1);
            getDirection.InitializeControl(this, 1);
        }

        internal override void OnDraw(PointFeature point)
        {
            getDirection.OnDrawAll();
            getDistance.OnDrawAll();

            if (intersectInfo.Visible)
            {
                intersectInfo.OnDraw();

                IPosition x = intersectInfo.Intersection;
                if (x!=null)
                {
                    ISpatialDisplay display = GetCommand().ActiveDisplay;
                    IDrawStyle style = EditingController.Current.Style(Color.Magenta);

                    if (getDirection.LineType!=null)
                    {
                        Direction d = getDirection.Direction;
                        style.Render(display, new IPosition[] { d.StartPosition, x });
                    }

                    if (getDistance.LineType!=null)
                    {
                        PointFeature p = getDistance.From;
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
            if (getDirection.Visible)
                getDirection.OnSelectPoint(point);
            else if (getDistance.Visible)
                getDistance.OnSelectPoint(point);
        }

        /// <summary>
        /// Reacts to the selection of a line feature.
        /// </summary>
        /// <param name="line">The line (if any) that has been selected.</param>
        internal override void OnSelectLine(LineFeature line)
        {
            if (getDirection.Visible)
                getDirection.OnSelectLine(line);
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

            // Save the intersect point if we're not updating
            IntersectOperation upd = GetUpdateOp();
            if (upd==null)
                return SaveDirDist();

            // Apply corrections and return the point previously created at the intersect
            Correct(upd);
            return upd.IntersectionPoint;
        }

        /// <summary>
        /// Saves a direction-distance intersection. 
        /// </summary>
        /// <returns>The point feature at the intersection (null if something went wrong).</returns>
        PointFeature SaveDirDist()
        {
            IntersectDirectionAndDistanceOperation op = null;

            try
            {
                Direction dir = getDirection.Direction;
                IEntity e1 = getDirection.LineType;
                Observation dist = getDistance.ObservedDistance;
                PointFeature from = getDistance.From;
                IEntity e2 = getDistance.LineType;
                IdHandle pointId = intersectInfo.PointId;
                bool isdefault = intersectInfo.IsDefault;

                op = new IntersectDirectionAndDistanceOperation();
                op.Execute(dir, dist, from, isdefault, pointId, e1, e2);
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
            IntersectDirectionAndDistanceOperation op = (IntersectDirectionAndDistanceOperation)io;
            op.Correct(getDirection.Direction,
                       getDistance.ObservedDistance, getDistance.From,
                       intersectInfo.IsDefault, getDirection.LineType, getDistance.LineType);
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

            Observation dist = getDistance.ObservedDistance;
            PointFeature from = getDistance.From;
            if (dist==null || from==null)
                return null;

            bool isdefault = intersectInfo.IsDefault;

            IPosition xsect, xsect1, xsect2;
            if (IntersectDirectionAndDistanceOperation.Calculate(dir, dist, from, isdefault,
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