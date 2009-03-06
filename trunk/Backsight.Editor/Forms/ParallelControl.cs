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
using System.Diagnostics;
using System.Drawing;

using Backsight.Editor.Operations;
using Backsight.Environment;
using Backsight.Geometry;
using Backsight.Forms;
using Backsight.Editor.Observations;

namespace Backsight.Editor.Forms
{
    public partial class ParallelControl : UserControl
    {
        #region Class data

		/// <summary>
        /// The command running this dialog.
        /// </summary>
        CommandUI m_Cmd;

        /// <summary>
        /// The reference line for the parallel.
        /// </summary>
        LineFeature m_Line;

        /// <summary>
        /// Default side for the parallel.
        /// </summary>
        bool m_IsLeft;

        // The offset can be ONE of the following ...

        /// <summary>
        /// Offset distance (signed). Distances to the left are less than zero.
        /// </summary>
        Distance m_Offset;

        /// <summary>
        /// Offset point
        /// </summary>
        PointFeature m_Point;

        // Used for drawing ...

        /// <summary>
        /// Southern end of parallel
        /// </summary>
        IPosition m_South;

        /// <summary>
        /// Northern end of parallel
        /// </summary>
        IPosition m_North;

        #endregion

        #region Constructors
		        
        internal ParallelControl(ParallelUI ui)
        {
            InitializeComponent();

            // Initialize everything.
            SetZeroValues();

            // Remember the command that's running the show (and the
            // line that's the reference line for the parallel).
            m_Cmd = ui;
            m_Line = ui.ReferenceLine;
        }

        internal ParallelControl(UpdateUI updcmd)
        {
            InitializeComponent();

	        // Initialize everything.
	        SetZeroValues();

	        // Remember the command that's running the show.
	        m_Cmd = updcmd;
        }
 
    	#endregion

        internal PointFeature OffsetPoint
        {
            get { return m_Point; }
        }

        internal Distance OffsetDistance
        {
            get { return m_Offset; }
        }

        internal string MyCaption
        {
            get { return "Specify Offset for Parallel Line"; }
        }

        private void offsetTextBox_TextChanged(object sender, EventArgs e)
        {
            // Ensure there is no parallel displayed.
            KillParallel();

            if (offsetTextBox.Text.Length==0)
            {
                // If we already had offset info, reset it.
                m_Point = null;
                m_Offset = null;

        		// Disable the "other way" button.
                otherWayButton.Enabled = false;
            }
            else
            {
                // The offset could have been specified by the user, or it could have
                // been set as the result of a pointing operation. In the latter case,
                // m_Point will be defined.

                if (m_Point==null)
                {
                    ParseOffset();

                    // Enable the "other way" button if we've got an offset.
                    if (m_Offset!=null)
                        otherWayButton.Enabled = true;
                }
                else
                {
                    // Confirm that the displayed text is the text that was
                    // added by SelectPoint. If not, the user may be trying
                    // to type in a offset. Permit fewer chars, in case the
                    // user is erasing.

                    string keystr = String.Format("+{0}", m_Point.FormattedKey);
                    string curstr = offsetTextBox.Text;
                    if (curstr.Length > keystr.Length)
                    {
                        string msg = String.Empty;
                        msg += ("What are you trying to do? If you want to type in a" + System.Environment.NewLine);
                        msg += ("new offset, you must initially delete the point ID" + System.Environment.NewLine);
                        msg += ("that is currently displayed.");
                        MessageBox.Show(msg);
                        return;
                    }

                    // Disable the "other way" button.
                    otherWayButton.Enabled = false;
                }
            }

	        // Try calculating the parallel. On success, draw it.
	        if (Calculate())
                Draw();
        }

        private void otherWayButton_Click(object sender, EventArgs e)
        {
            m_IsLeft = !m_IsLeft;

            // Shouldn't be here if an offset point has been specified.
            if (m_Point!=null)
            {
                MessageBox.Show("Can't go the other way because the parallel passes through a point.");
                otherWayButton.Enabled = false;
                return;
            }

            // The offset distance SHOULD be available, but just check.
            if (m_Offset==null)
            {
                MessageBox.Show("Need an offset to do that.");
                otherWayButton.Enabled = false;
                return;
            }

            // Erase what we have.
            m_Cmd.ErasePainting();

            // Reverse the sign of the offset distance.
            if (m_Offset.Meters < 0.0)
                m_Offset.SetPositive();
            else
                m_Offset.SetNegative();

            // Re-calculate the parallel and draw it.
            if (Calculate())
                Draw();

            // Resume in the "Next" field.
            nextButton.Focus();
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            // An offset distance or offset point must be defined.

            if (m_Offset!=null || m_Point!=null)
            {
	            // Finish the command.
	            m_Cmd.DialFinish(this);
            }
            else
            {
	            MessageBox.Show("The parallel offset has not been specified.");
                offsetTextBox.Focus();
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
	        // Abort the command.
	        m_Cmd.DialAbort(this);
        }

        private void ParallelControl_Load(object sender, EventArgs e)
        {
            // If we're doing an update, display the previously specified
            // info, and do any painting. Otherwise it's a new op ...

            if (!InitUpdate() && !InitRecall())
            {
	        	// Disable the "other way" button (an offset has to be entered).
                otherWayButton.Enabled = false;

		        // For reference lines that are non-topological, we always start on the left.
		        m_IsLeft = true;

		        // Get the default side for the parallel. In case where the
		        // reference line is on the exterior edge of an island, we
		        // always go INTO the island. If neither side is an island,
		        // we go for the polygon that has the bigger area.
                /*
                 * SS: 21-AUG-2007 -- Leave this for now. The problem is that a LineFeature
                 * may be the boundary for many polygons. Nevertheless, it should be possible
                 * to do something if the line hasn't been intersected... do it later.
                 * 
        		// Get the currently active editing layer.
                ILayer curLayer = m_Cmd.ActiveLayer;

		        // Get the polygon rings on the left and right (if any).
		        Ring pL = m_Line.FindLeft(curLayer);
		        Ring pR = m_Line.FindRight(curLayer);

		        // If BOTH are defined
		        if (pL!=null && pR!=null)
                {
			        // If the left polygon is the exterior edge of an island,
			        // go for the right instead.

        			if (pL is Island)
				        m_IsLeft = false;
			        else
                    {
				        // If the right polygon isn't an island either, and
				        // it has an area bigger than the left, switch to the right.
				        //if (!(pR is Island) && pR.GetArea(curLayer) > pL.GetArea(curLayer) )
                        if (!(pR is Island) && pR.Area > pL.Area)
                            m_IsLeft = false;
			        }
		        }
		        else
                {
			        // If the right polygon is defined, go for that (otherwise
			        // stick with default parallel on the left).
			        if (pR!=null)
                        m_IsLeft = false;
		        }
                 */
        	}
        }

        internal void SelectPoint(PointFeature point)
        {
            // Get rid of any parallel already drawn.
            KillParallel();

            // Get rid of any previously entered distance.
            m_Offset = null;

            // Remember the selected point.
            m_Point = point;

            // If it's actually defined, re-draw the parallel and display
            // the key of the point.
            if (m_Point!=null)
            {
                // Calculate the parallel and draw it.
                Calculate();
                Draw();

                // Display the key of the offset point. This will call
                // the OnChange handler for the field.
                string keystr = String.Format("+{0}", m_Point.FormattedKey);
                offsetTextBox.Text = keystr;
            }
            else
            {
                // Disable the "other way" button.
                otherWayButton.Enabled = false;

                // Ensure the offset field is blank.
                offsetTextBox.Text = String.Empty;
            }
        }

        internal void Draw() // was Paint
        {
            // Nothing to do if parallel points undefined.
            if (m_South==null || m_North==null)
                return;

            Debug.Assert(m_Line!=null);
            ISpatialDisplay draw = m_Cmd.ActiveDisplay;
            IDrawStyle solidStyle = EditingController.Current.Style(Color.Magenta);
            IDrawStyle dottedStyle = new DottedStyle();

            if (m_Line is ArcFeature)
            {
                // The parallel portion is solid, while the remaining portion of the circle is dotted.
                ArcFeature arc = (m_Line as ArcFeature);
                CircularArcGeometry cg = new CircularArcGeometry(arc.Circle.Center, m_South, m_North, arc.IsClockwise);
                solidStyle.Render(draw, cg);
                cg.IsClockwise = !cg.IsClockwise;
                dottedStyle.Render(draw, cg);
            }
            else
            {	
		        // What's the bearing from the start to the end of the parallel?
                double bearing = Geom.BearingInRadians(m_South, m_North);

		        // What's the max length of a diagonal crossing the entire screen?
		        double maxdiag = m_Cmd.MaxDiagonal;

		        // Project to a point below the southern end of the parallel, as
		        // well as a point above the northern end.
                IPosition below = Geom.Polar(m_South, bearing+Constants.PI, maxdiag);
                IPosition above = Geom.Polar(m_North, bearing, maxdiag);

                LineSegmentGeometry.Render(below, m_South, draw, dottedStyle);
                LineSegmentGeometry.Render(m_South, m_North, draw, solidStyle);
                LineSegmentGeometry.Render(m_North, above, draw, dottedStyle);

		        // If we have an offset point, draw it in green.
		        if (m_Point!=null)
                    m_Point.Draw(draw, Color.Green);
            }
        }

        void KillParallel()
        {
            // Ensure parallel has been erased.
            m_Cmd.ErasePainting();

            // Get rid if the end positions.
            m_South = null;
            m_North = null;
        }

        bool Calculate()
        {
            // Ensure any previously defined end positions have been turfed.
            m_South = null;
            m_North = null;

            // Can't do nothing if the reference line is undefined.
            if (m_Line==null)
                return false;

            // Calculate the parallel points, depending on what sort of
            // observation we've got.

            IPosition north = null;
            IPosition south = null;
            bool ok = false;

            // Ensure the correct sign is defined in case of an offset distance.

            if (m_Offset!=null)
            {
                if (m_IsLeft)
                    m_Offset.SetNegative();
                else
                    m_Offset.SetPositive();

                ok = ParallelUI.Calculate(m_Line, m_Offset, out south, out north);
            }
            else if (m_Point!=null)
                ok = ParallelUI.Calculate(m_Line, m_Point, out south, out north);

            // If the calculation succeeded, allocate vertices to
            // hold the results we got.

            if (ok)
            {
                m_South = south;
                m_North = north;
            }

            return ok;
        }

        /// <summary>
        /// Parses an explicitly entered offset distance.
        /// </summary>
        /// <returns>True if offset parses ok.</returns>
        bool ParseOffset()
        {
            // Get the entered string.
            string str = offsetTextBox.Text.Trim();

            // Try to parse it.
            Distance offdist = new Distance(str);
            if (!offdist.IsDefined)
            {
                MessageBox.Show("Invalid offset distance");
                return false;
            }

            // Hold on to the parsed offset
            m_Offset = offdist;

            // Any previously specified offset point is irrelevant.
            if (m_Point!=null)
            {
                m_Cmd.ErasePainting();
                m_Point = null;
            }

            return true;
        }

        /// <summary>
        /// Initialize everything to zero (for use by constructors).
        /// </summary>
        void SetZeroValues()
        {
            m_Cmd = null;
            m_Line = null;
            m_IsLeft = false;
            m_Offset = null;
            m_Point = null;
            m_South = null;
            m_North = null;
        }

        bool InitUpdate()
        {
            // Get the creating op.
            ParallelOperation op = UpdateOp;
            if (op==null)
                return false;

            // Pick up the reference arc.
            m_Line = op.ReferenceLine;

            // Initialize the observed stuff.
            InitOp(op);
            return true;
        }

        bool InitRecall()
        {
            ParallelOperation op = (m_Cmd.Recall as ParallelOperation);
            if (op==null)
                return false;

            // Unlike updates, the reference line can be different
            // from what it was originally.

            // Initialize the observed stuff.
            InitOp(op);
            return true;
        }

        void InitOp(ParallelOperation op)
        {
            // The direction is significant only if an offset distance
            // has been specified.
            m_IsLeft = false;

            // Pick up the offset.
            Observation offset = op.Offset;
    	
            // If it's an observed distance, get the side.
            Distance dist = (offset as Distance);

            if (dist!=null)
            {
                m_Offset = new Distance(dist);
                m_IsLeft = m_Offset.SetPositive();
            }
            else
            {
                // The only other thing it could be is an offset point.
                OffsetPoint offPoint = (offset as OffsetPoint);
                if (offPoint!=null)
                    m_Point = offPoint.Point;
            }

            if (m_Point!=null)
                SelectPoint(m_Point);
            else if (m_Offset!=null)
                offsetTextBox.Text = m_Offset.Format();
        }

        bool IsUpdate
        {
            get { return (m_Cmd is UpdateUI); }
        }

        ParallelOperation UpdateOp
        {
            get
            {
                UpdateUI up = (m_Cmd as UpdateUI);
                return (up==null ? null : (ParallelOperation)up.GetOp());
            }
        }
    }
}
