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
using Backsight.Editor.Observations;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" was="CdIntersectDir" />
    /// <summary>
    /// Dialog for the Intersect - Two Directions command.
    /// </summary>
    partial class IntersectTwoDirectionsForm : IntersectForm
    {
        /// <summary>
        /// Creates a new <c>IntersectTwoDirectionsForm</c>
        /// </summary>
        /// <param name="cmd">The command displaying this dialog (not null)</param>
        /// <param name="title">The string to display in the form's title bar</param>
        internal IntersectTwoDirectionsForm(CommandUI cmd, string title)
            : base(cmd, title)
        {
            InitializeComponent();
        }

        private void IntersectTwoDirectionsForm_Shown(object sender, EventArgs e)
        {
            // Initialize the first page last, to ensure focus is on the initial text box
            // of the first page.
            intersectInfo.InitializeControl(this);
            getDirection1.InitializeControl(this, 1);

            // getDirection2 gets initialized by directionTwoPage_ShowFromNext
        }

        internal override void OnDraw(PointFeature point)
        {
            getDirection1.OnDrawAll();
            getDirection2.OnDrawAll();

            if (intersectInfo.Visible)
            {
                intersectInfo.OnDraw();

                IPosition x = intersectInfo.Intersection;
                if (x!=null)
                {
                    ISpatialDisplay display = GetCommand().ActiveDisplay;
                    IDrawStyle style = EditingController.Current.Style(Color.Magenta);

                    if (getDirection1.LineType!=null)
                    {
                        Direction d = getDirection1.Direction;
                        style.Render(display, new IPosition[] { d.StartPosition, x });
                    }

                    if (getDirection2.LineType!=null)
                    {
                        Direction d = getDirection2.Direction;
                        style.Render(display, new IPosition[] { d.StartPosition, x });
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
            GetDirectionControl gdc = GetVisibleDirectionControl();
            if (gdc!=null)
                gdc.OnSelectPoint(point);
        }

        /// <summary>
        /// Reacts to the selection of a line feature.
        /// </summary>
        /// <param name="line">The line (if any) that has been selected.</param>
        internal override void OnSelectLine(LineFeature line)
        {
            GetDirectionControl gdc = GetVisibleDirectionControl();
            if (gdc!=null)
                gdc.OnSelectLine(line);
        }

        GetDirectionControl GetVisibleDirectionControl()
        {
            if (getDirection1.Visible)
                return getDirection1;

            if (getDirection2.Visible)
                return getDirection2;

            return null;
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
                return SaveDirDir();

            // Apply corrections and return the point previously created at the intersect
            Correct(upd);
            return upd.IntersectionPoint;
        }

        /// <summary>
        /// Saves a direction-direction intersection. 
        /// </summary>
        /// <returns>The point feature at the intersection (null if something went wrong).</returns>
        PointFeature SaveDirDir()
        {
            IntersectTwoDirectionsOperation op = null;

            try
            {
                Direction d1 = getDirection1.Direction;
                IEntity e1 = getDirection1.LineType;
                Direction d2 = getDirection2.Direction;
                IEntity e2 = getDirection2.LineType;
                IdHandle pointId = intersectInfo.PointId;

                op = new IntersectTwoDirectionsOperation(Session.WorkingSession, 0, d1, d2);
                op.Execute(pointId, e1, e2);
                return op.IntersectionPoint;
            }

            catch (Exception ex)
            {
                //Session.WorkingSession.Remove(op);
                MessageBox.Show(ex.StackTrace, ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Correct an edit using the info from this dialog.
        /// </summary>
        void Correct(IntersectOperation io)
        {
            IntersectTwoDirectionsOperation op = (IntersectTwoDirectionsOperation)io;
            op.Correct(getDirection1.Direction, getDirection2.Direction,
                       getDirection1.LineType, getDirection2.LineType);
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
            Direction d1 = getDirection1.Direction;
            if (d1==null)
                return null;

            Direction d2 = getDirection2.Direction;
            if (d2==null)
                return null;

            return d1.Intersect(d2);
        }

        private void directionTwoPage_ShowFromNext(object sender, EventArgs e)
        {
            // Initialize the direction now (rather than when the form is shown). In
            // a situation where the user has just changed the default offset (on page
            // 1 of the wizard), it makes little sense to show the old offset when
            // page 2 is displayed.

            getDirection2.InitializeControl(this, 2);
        }

        private void finishPage_CloseFromBack(object sender, Gui.Wizard.PageEventArgs e)
        {
            // The intersection and any connecting lines should only be visible when
            // the finish page is on screen.
            ErasePainting();            
        }
    }
}

