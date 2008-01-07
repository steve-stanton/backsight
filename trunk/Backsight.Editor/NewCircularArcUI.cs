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

using Backsight.Forms;
using Backsight.Editor.Forms;
using Backsight.Editor.Operations;

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
        /// True if user wants the short arc.
        /// </summary>
        bool m_IsShortArc;

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
            m_IsShortArc = true;
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
        /// Geometry that can be used to detect intersections with the map. This will be called
        /// by <c>base.Paint</c> if intersections with the map need to be shown during mouse moves.
        /// </summary>
        /// <returns>The geometry for the new line (null if insufficient information has been specified)</returns>
        internal override LineGeometry GetIntersectGeometry()
        {
            if (m_Circles==null)
                return null;

            PointFeature start = StartPoint;
            if (start==null)
                return null;

            IPointGeometry end = LastMousePosition;
            if (end==null)
                return null;

            ITerminal endTerm = new FloatingTerminal(end);

            // Find the circle that is closest to the end (only looking at the valid circles
            // that are within a 1mm tolerance). Return null geometry if none of the circles
            // are within reach.

            Circle best = m_Circles[0];
            double bestdist = best.Distance(end).Meters;

            if (m_Circles.Count > 1)
            {
                // See if there is any better choice
                for (int i=1; i<m_Circles.Count; i++)
                {
                    Circle c = m_Circles[i];
                    double dist = c.Distance(end).Meters;
                    if (dist < bestdist)
                    {
                        best = c;
                        bestdist = dist;
                    }
                }
            }

            // Ignore if the best circle is too far away
            double tol = CircleTolerance.Meters;
            if (bestdist > tol)
                return null;

            // Project the end point ON to the circle.

            /*
			if ( pBest && m_pStart ) {

				// Project the end point ON to the circle.
				CeVertex centre(*pBest);
				CeTurn eturn(centre,end);
				FLOAT8 bearing = eturn.GetBearing();
				CeVertex cirend(centre,bearing,pBest->GetRadius());

				// Get the clockwise angle from the start to the
				// current end point.
				CeTurn sturn(centre,*m_pStart);
				FLOAT8 angle = sturn.GetAngle(cirend);

				// Figure out which direction the curve should go, depending
				// on whether the user wants the short arc or the long one.
				LOGICAL iscw;
				if ( angle < PI )
					iscw = m_IsShortArc;
				else
					iscw = !m_IsShortArc;

				// Remember the point we've got to.
				m_End = CeLocation(cirend);
				m_pEnd = &m_End;

				// Create new curve and draw it.
				CeCurve curve(*pBest,*m_pStart,*m_pEnd,iscw);

				//Draw(cirend);
				// Check for intersections.
				this->ShowX(&curve);
			}
             */
            //return new ArcGeometry(
            return new SegmentGeometry(start, endTerm);
        }

        internal override void MouseMove(IPosition p)
        {
            base.MouseMove(p);
            /*
            CadastralMapModel map = CadastralMapModel.Current;
            ILength size = new Length(map.PointHeight.Meters * 0.5);
            m_CurrentPoint = (map.QueryClosest(p, size, SpatialType.Point) as PointFeature);

            if (m_Start==null)
                return;

            m_End = PointGeometry.Create(p);
            if (m_End.IsCoincident(m_Start))
            {
                m_End = null;
                return;
            }

            ErasePainting();
             */
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
            // Create the persistent edit (adds to current session)
            NewLineOperation op = new NewLineOperation();
            bool ok = op.Execute(StartPoint, end, m_NewArcCircle, m_IsShortArc);

            if (!ok)
                Session.CurrentSession.Remove(op);
        }

        /// <summary>
        /// Creates any applioable context menu
        /// </summary>
        /// <returns>The context menu for this command.</returns>
        internal override ContextMenuStrip CreateContextMenu()
        {
            NewCircularArcContextMenu result = new NewCircularArcContextMenu(this);
            result.IsShortArc = m_IsShortArc;
            return result;
        }

        /// <summary>
        /// Handles the context menu "Short arc" menuitem
        /// </summary>
        /// <param name="action">The action that initiated this method call</param>
        internal void ShortArc(IUserAction action)
        {
            m_IsShortArc = true;
            ErasePainting();
        }

        /// <summary>
        /// Handles the context menu "Long arc" menuitem
        /// </summary>
        /// <param name="action">The action that initiated this method call</param>
        internal void LongArc(IUserAction action)
        {
            m_IsShortArc = false;
            ErasePainting();
        }
    }
}
