/// <remarks>
/// Copyright 2008 - Steve Stanton. This file is part of Backsight
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
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

using Backsight.Editor.Forms;
using Backsight.Forms;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="04-JAN-2008" />
    /// <summary>
    /// User interface for defining a new circular arc.
    /// </summary>
    class NewCircularArcUI : NewLineUI
    {
        #region Class data

        /// <summary>
        /// The circles incident on the start point.
        /// </summary>
        List<Circle> m_Circles;

        /// <summary>
        /// The circle the new arc should coincide with
        /// </summary>
        Circle m_NewArcCircle;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cc">Object for holding any displayed dialogs</param>
        /// <param name="action">The action that initiated this command</param>
        /// <param name="start">Point initially selected at start of command</param>
        internal NewCircularArcUI(IControlContainer cc, IUserAction action, PointFeature start)
            : base(cc, action, start)
        {
            m_Circles = null;
            m_NewArcCircle = null;
        }

        #endregion

        internal override bool Run()
        {
            // If a start point has been specified, confirm that it falls on at least
            // one circle. If NOT on a circle, tell the user and quit the command.
            if (StartPoint != null)
            {
                m_Circles = CadastralMapModel.Current.FindCircles(StartPoint, CircleTolerance);
                if (m_Circles.Count == 0)
                {
                    MessageBox.Show("The currently selected point does not coincide with any circles.");
                    AbortCommand();
                    return false;
                }
            }

            return base.Run();
        }

        /// <summary>
        /// The tolerance for finding circles
        /// </summary>
        ILength CircleTolerance
        {
            get { return new Length(0.001); }
        }

        internal override void Paint(PointFeature point)
        {
            // If applicable circles are defined, ensure they show
            if (m_Circles != null)
            {
                ISpatialDisplay display = ActiveDisplay;
                IDrawStyle style = new DottedStyle(Color.Magenta);
                foreach (Circle c in m_Circles)
                    c.Render(display, style);
            }

            base.Paint(point);
        }

        /// <summary>
        /// Appends a point to the new line. If it's the second point, the new line
        /// will be added to the map.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        internal override bool AppendToLine(PointFeature p)
        {
            // If the start point is not defined, just remember it (ensuring
            // that the point coincides with at least one circle).
            if (StartPoint == null)
            {
                m_Circles = CadastralMapModel.Current.FindCircles(p, CircleTolerance);
                if (m_Circles.Count == 0)
                {
                    MessageBox.Show("Selected point does not coincide with any circles.");
                    return false;
                }

                return true;
            }

            // Get the circles that pass through the selected point.
            List<Circle> circles = CadastralMapModel.Current.FindCircles(p, CircleTolerance);
            if (circles.Count == 0)
            {
                MessageBox.Show("Selected point does not coincide with any circles.");
                //SetLineCursor();
                return false;
            }

            // The point MUST coincide with one of the circles that
            // were found at the start point.
            List<Circle> comlist = Circle.GetCommonCircles(m_Circles, circles);
            if (comlist.Count == 0)
            {
                string msg = String.Empty;
                msg += ("Selected end point does not coincide with any of" + System.Environment.NewLine);
                msg += ("the circles that pass through the start point.");
                MessageBox.Show(msg);
                //SetLineCursor();
                return false;
            }

            // Could we have more than 1 to choose from?
            if (comlist.Count > 1)
            {
                string msg = String.Empty;
                msg += ("More than one circle is common to the start" + System.Environment.NewLine);
                msg += ("and the end point. Don't know what to do.");
                MessageBox.Show(msg);
                //SetLineCursor();
                return false;
            }

            // Remember the common circle (for the AddNewLine override)
            m_NewArcCircle = comlist[0];

            return base.AppendToLine(p);
        }

        /// <summary>
        /// Adds a new circular arc feature.
        /// </summary>
        /// <param name="end"></param>
        internal override void AddNewLine(PointFeature end)
        {
            MessageBox.Show("add circular arc");
            /*
            // Create the persistent edit (adds to current session)
            NewLineOperation op = new NewLineOperation();
            bool ok = op.Execute(m_Start, end);

            if (!ok)
                Session.CurrentSession.Remove(op);
             */
        }
    }
}
