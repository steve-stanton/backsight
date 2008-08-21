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
using System.Diagnostics;

using Backsight.Editor.Forms;
using Backsight.Forms;
using Backsight.Geometry;
using Backsight.Editor.Operations;
using Backsight.Environment;

namespace Backsight.Editor
{
	/// <written by="Steve Stanton" on="03-JUL-2007" />
    /// <summary>
    /// User interface for defining a new line.
    /// </summary>
    class NewLineUI : SimpleCommandUI
    {
        #region Class data

        /// <summary>
        /// The point at the start of the new line.
        /// </summary>
        PointFeature m_Start;

        /// <summary>
        /// The point the mouse is currently close to (may end up being either the start or
        /// the end of the new line).
        /// </summary>
        PointFeature m_CurrentPoint;

        /// <summary>
        /// The last mouse position
        /// </summary>
        IPointGeometry m_End;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cc">Object for holding any displayed dialogs</param>
        /// <param name="action">The action that initiated this command</param>
        /// <param name="start">Point initially selected at start of command</param>
        internal NewLineUI(IControlContainer cc, IUserAction action, PointFeature start)
            : base(cc, action)
        {
            m_Start = start;
            m_End = null;
            m_CurrentPoint = start;
        }

        #endregion

        internal override bool Run()
        {
            // Ensure any initial selection has been cleared (if the user clicks in space
            // to cancel the line-add command, the selection needs to be clear).
            Controller.ClearSelection();

            SetCommandCursor();
            return true;
        }

        internal override void SetCommandCursor()
        {
            ActiveDisplay.MapPanel.Cursor = EditorResources.PenCursor;
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
            ISpatialDisplay display = ActiveDisplay;
            IDrawStyle style = Controller.DrawStyle;
            style.FillColor = style.LineColor = Color.Magenta;

            if (m_CurrentPoint!=null)
                style.Render(display, m_CurrentPoint);

            if (m_Start!=null && m_End!=null)
            {
                RenderGeometry(display, style);

                EditingController ec = EditingController.Current;
                if (ec.JobFile.Data.AreIntersectionsDrawn && ArePointsDrawn() && AddingTopology())
                {
                    // The intersect geometry could be null when starting to add a new
                    // circular arc
                    LineGeometry line = GetIntersectGeometry();
                    if (line!=null)
                    {
                        IntersectionFinder xf = new IntersectionFinder(line, false);
                        style.FillColor = Color.Transparent;
                        xf.Render(display, style);
                    }
                }
            }
        }

        /// <summary>
        /// Draws the new line based on currently entered data
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The drawing style</param>
        internal virtual void RenderGeometry(ISpatialDisplay display, IDrawStyle style)
        {
            if (m_Start!=null && m_End!=null)
                style.Render(display, new IPosition[] { m_Start, m_End });
        }

        /// <summary>
        /// Geometry that can be used to detect intersections with the map
        /// </summary>
        /// <returns>The geometry for the new line (null if insufficient information has been specified)</returns>
        internal virtual LineGeometry GetIntersectGeometry()
        {
            if (m_Start==null || m_End==null)
                return null;

            ITerminal endTerm = new FloatingTerminal(m_End);
            return new SegmentGeometry(m_Start, endTerm);
        }

        /// <summary>
        /// Is the line that's being added going to form a polygon boundary?
        /// </summary>
        /// <returns></returns>
        bool AddingTopology()
        {
            CadastralMapModel map = CadastralMapModel.Current;
            IEntity ent = map.DefaultLineType;
            return (ent==null ? false : ent.IsPolygonBoundaryValid);
        }

        internal override void MouseMove(IPosition p)
        {
            PointFeature oldCurrentPoint = m_CurrentPoint;
            CadastralMapModel map = CadastralMapModel.Current;
            EditingController ec = EditingController.Current;
            ILength size = new Length(ec.JobFile.Data.PointHeight * 0.5);
            m_CurrentPoint = (map.QueryClosest(p, size, SpatialType.Point) as PointFeature);

            if (m_Start==null)
            {
                // If the mouse is over a different point, ensure the old point is erased
                if (oldCurrentPoint!=null && !Object.ReferenceEquals(oldCurrentPoint, m_CurrentPoint))
                    ErasePainting();

                return;
            }

            if (m_CurrentPoint==null)
                m_End = PointGeometry.Create(p);
            else
                m_End = m_CurrentPoint;

            if (m_End.IsCoincident(m_Start))
            {
                m_End = null;
                return;
            }

            ErasePainting();
        }

        internal override bool LButtonDown(IPosition p)
        {
            // Cancel the new line if there is no point selected.
            if (m_CurrentPoint==null)
            {
                DialAbort(null);
                return true;
            }

            // If we don't have the first point yet, remember the start location.
            // Otherwise remember the end point & add the line.
            AppendToLine(m_CurrentPoint);
            return true;
        }

        /// <summary>
        /// Appends a point to the new line. If it's the second point, the new line
        /// will be added to the map.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        internal virtual bool AppendToLine(PointFeature p)
        {
            // If the start point is not defined, just remember it.
            if (m_Start==null)
            {
                m_Start = p;
                return true;
            }

		    // Confirm the point is  different from the start.
            if (p.IsCoincident(m_Start))
            {
			    MessageBox.Show("End point cannot match the start point.");
			    return false;
		    }

		    // Add the new line.
            AddNewLine(p);

            DialFinish(null);
	        return true;
        }

        /// <summary>
        /// Adds a new line segment feature.
        /// </summary>
        /// <param name="end"></param>
        internal virtual void AddNewLine(PointFeature end)
        {
            // Create the persistent edit (adds to current session)
            NewLineOperation op = new NewLineOperation();
            bool ok = op.Execute(m_Start, end);

            if (!ok)
                Session.WorkingSession.Remove(op);
        }

        internal override void DialAbort(Control wnd)
        {
            AbortCommand();
        }

        internal override bool DialFinish(Control wnd)
        {
            FinishCommand();
            return true;
        }

        /// <summary>
        /// The point at the start of the new line (null if not yet specified)
        /// </summary>
        protected PointFeature StartPoint
        {
            get { return m_Start; }
        }

        /// <summary>
        /// The last mouse position
        /// </summary>
        protected IPointGeometry LastMousePosition
        {
            get { return m_End; }
            set { m_End = value; }
        }

        /// <summary>
        /// Creates any applioable context menu
        /// </summary>
        /// <returns>The context menu for this command.</returns>
        internal override ContextMenuStrip CreateContextMenu()
        {
            return new NewLineContextMenu(this);
        }

        /// <summary>
        /// Handles the context menu "Specify ID" menuitem.
        /// </summary>
        /// <param name="action">The action that initiated this method call</param>
        internal void SpecifyId(IUserAction action)
        {
            // Ask the user for the point ID.
            GetKeyForm dial = new GetKeyForm("Specify Point ID");
            if (dial.ShowDialog() == DialogResult.OK)
            {
                // Locate the point with the specified key
                string keyval = dial.Key;
                CadastralMapModel map = CadastralMapModel.Current;
                PointFeature point = new FindPointByIdQuery(map.Index, keyval).Result;

                if (point==null)
                    MessageBox.Show("Cannot find point with specified key");
                else
                    AppendToLine(point);
            }
            dial.Dispose();
        }

        /// <summary>
        /// Handles the context menu "Cancel" menuitem.
        /// </summary>
        /// <param name="action">The action that initiated this method call</param>
        internal void Cancel(IUserAction action)
        {
            Escape();
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
