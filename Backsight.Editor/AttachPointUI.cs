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

using Backsight.Environment;
using Backsight.Forms;
using Backsight.Editor.Forms;
using Backsight.Geometry;
using Backsight.Editor.Operations;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="04-FEB-2003" was="CuiAttachPoint" />
    /// <summary>
    /// User interface for adding a point on a line.
    /// </summary>
    class AttachPointUI : SimpleCommandUI
    {
        #region Class data

        /// <summary>
        /// The entity type to assign to new points 
        /// </summary>
        IEntity m_PointType;

        /// <summary>
        /// Should the command be automatically repeated? 
        /// </summary>
        bool m_Repeat;

        /// <summary>
        /// The currently selected line 
        /// </summary>
        LineFeature m_Line;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>AttachPointUI</c>
        /// </summary>
        /// <param name="action">The action that initiated the command</param>
        internal AttachPointUI(IUserAction action)
            : base(action)
        {
            m_PointType = null;
            m_Repeat = false;
            m_Line = null;
        }

        #endregion

        /// <summary>
        /// Starts the user interface (if any) for this command.
        /// </summary>
        /// <returns>True if command started ok.</returns>
        internal override bool Run()
        {
            // Disallow if points are not currently drawn.
            if (!ArePointsDrawn())
            {
                MessageBox.Show("To add points, you must initially make them visible (see Edit-Preferences).",
                                "Points are currently invisible");
                DialFinish(null);
                return false;
            }

            // Ensure nothing is currently selected (highlighted), since we'll be doing our own.
            Controller.ClearSelection();

            // Display dialog to get the point type
            AttachPointForm dial = new AttachPointForm(this);
            if (dial.ShowDialog() != DialogResult.OK)
            {
                dial.Dispose();
                DialFinish(null);
                return false;
            }

            // Confirm that the entity type has been specified.
            m_PointType = dial.PointType;
            if (m_PointType==null)
            {
                MessageBox.Show("Point type must be specified.");
                DialFinish(null);
                return false;
            }

            // See if the command should auto-repeat or not
            m_Repeat = dial.ShouldRepeat;

            // Switch on the command cursor
            SetCommandCursor();

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

        internal override void Paint(PointFeature point)
        {
            // Highlight selected line if we've got one
            Highlight(m_Line);
        }

        internal override void MouseMove(IPosition p)
        {
            // The following is pretty much the same as what the controller would do to
            // cover auto-select...

            // The ground tolerance is 1mm at the draw scale.
            ISpatialDisplay draw = ActiveDisplay;
            ILength tol = new Length(0.001 * draw.MapScale);

            // Just return if we previously selected something, and the
            // search point lies within tolerance.
            if (m_Line!=null)
            {
                ILength dist = m_Line.Distance(p);
                if (dist.Meters < tol.Meters)
                    return;

                // Ensure the previously select line gets un-highlighted
                m_Line = null;
                ErasePainting();
            }

            // Ask the map to find the closest line (if any)
            ISpatialIndex index = CadastralMapModel.Current.Index;
            m_Line = (index.QueryClosest(p, tol, SpatialType.Line) as LineFeature);
            Highlight(m_Line);
        }

        internal override bool LButtonDown(IPosition p)
        {
            // Quit if we don't have a selected line
            if (m_Line==null)
            {
                AbortCommand();
                return false;
            }

            // Get the position on the selected line that is closest
            // to the supplied position.
            ISpatialDisplay draw = ActiveDisplay;
            ILength tol = new Length(0.001 * draw.MapScale);
            IPointGeometry pg = PointGeometry.Create(p);
            IPosition pos = m_Line.LineGeometry.GetClosest(pg, tol);
            if (pos==null)
            {
                MessageBox.Show("You appear to be moving the mouse too fast");
                return false;
            }

            // Attach a new point to the line
            AttachPointOperation op = null;

            try
            {
                op = new AttachPointOperation(Session.CurrentSession);
                op.Execute(m_Line, pos, m_PointType);

                // Ensure the draw includes the extra point (perhaps a bit of overkill to
                // draw just one point).
                Controller.RefreshAllDisplays();
            }

            catch (Exception ex)
            {
                Session.CurrentSession.Remove(op);
                MessageBox.Show(ex.Message);
                return true;
            }

            // Exit the command if we're not supposed to repeat
            if (!m_Repeat)
                AbortCommand();

            return true;
        }

        internal override bool DialFinish(Control wnd)
        {
            // Get the base class to finish up.
            return FinishCommand();
        }

        internal override void SetCommandCursor()
        {
            ActiveDisplay.MapPanel.Cursor = EditorResources.AttachPointCursor;
        }

        /// <summary>
        /// Reacts to a situation where the user presses the ESC key, by aborting this command.
        /// </summary>
        internal override void Escape()
        {
            AbortCommand();
        }
    }
}
