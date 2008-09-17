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
using Backsight.Forms;
using Backsight.Geometry;

namespace Backsight.Editor.Forms
{
    /// <summary>
    /// Dialog for obtaining terminal point (for use when creating parallel lines).
    /// </summary>
    public partial class TerminalControl : UserControl
    {
        #region Class data

        /// <summary>
        /// The command running this dialog.
        /// </summary>
        CommandUI m_Cmd;

        /// <summary>
        /// Is this the dialog for the last (i.e. second) terminal?
        /// </summary>
        bool m_IsLast;

        /// <summary>
        /// The position of the terminal.
        /// </summary>
        IPosition m_Terminal;

        /// <summary>
        /// The terminal line (if any).
        /// </summary>
        LineFeature m_Line;

        #endregion

        #region Constructors

        internal TerminalControl(ParallelUI cmd, bool isLast)
        {
            InitializeComponent();

            m_Cmd = cmd;
            m_IsLast = isLast;
            m_Line = null;
            m_Terminal = null;
        }

        internal TerminalControl(UpdateUI updcmd, bool isLast)
        {
            InitializeComponent();

        	m_Cmd = updcmd;
	        m_IsLast = isLast;
            m_Line = null;
            m_Terminal = null;
        }

        #endregion

        internal LineFeature TerminalLine
        {
            get { return m_Line; }
        }

        internal IPosition TerminalPosition
        {
            get { return m_Terminal; }
        }

        private void noTerminalCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // If we previously had a terminal line, erase the terminal
            // position and unhighlight the arc.

            ISpatialDisplay draw = m_Cmd.ActiveDisplay;

            if (m_Line!=null)
                m_Cmd.ErasePainting();

            m_Line = null;
            ParallelUI cmd = Command;

            // Draw the parallel point instead.
            if (m_IsLast)
                m_Terminal = cmd.ParallelTwo;
            else
                m_Terminal = cmd.ParallelOne;

            EditingController.Current.Style(Color.Yellow).Render(draw, m_Terminal);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            m_Cmd.DialAbort(this);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            m_Cmd.DialFinish(this);
        }

        private void TerminalControl_Load(object sender, EventArgs e)
        {
            ISpatialDisplay view = m_Cmd.ActiveDisplay;

            // Get the "real" command that's running this dialog (not any update).
            ParallelUI cmd = Command;
            Debug.Assert(cmd!=null);

            if (m_IsLast)
            {
                MyCaption = "Terminal 2";
                okButton.Text = "&Finish";

                // If the reference line for the parallel isn't a circular
                // arc, disable and hide the "Other Way" button.
                if (!(cmd.ReferenceLine is ArcFeature))
                {
                    otherWayButton.Enabled = false;
                    otherWayButton.Visible = false;
                }
            }
            else
            {
                MyCaption = "Terminal 1";
                okButton.Text = "&Next...";

        		// Disable (and hide) the "Other Way" button.
                otherWayButton.Enabled = false;
                otherWayButton.Visible = false;
            }

            // If we are not updating a previously created parallel,
            // get a terminal position.
            int state = InitUpdate();
            if (state==0 && !FindTerminal())
                state = -1;

            // Get out if that somehow failed.
            if (state < 0)
            {
                m_Cmd.DialAbort(this);
                return;
            }

            // Draw the terminal point in yellow.
            EditingController.Current.Style(Color.Yellow).Render(view, m_Terminal);

            // If we don't have a terminal line, alter the text that says
            // that it's highlighted! And make the "don't terminate" checkbox
            // invisible.
            if (m_Line==null)
            {
                messageLabel1.Visible = false;
                messageLabel2.Text = "If you want to terminate on a line, select it.";
                messageLabel3.Visible = false;
                noTerminalCheckBox.Visible = false;
            }
        }

        /// <summary>
        /// Tries to find a terminal line to end the parallel on.
        /// </summary>
        /// <returns>False if parallel positions are not available. True
        /// otherwise (that does not mean that a terminal was actually found).
        /// </returns>
        bool FindTerminal()
        {
            // Get the position of the parallel point.
            ParallelUI cmd = this.Command;
            IPosition parpos;

            if (m_IsLast)
                parpos = cmd.ParallelTwo;
            else
                parpos = cmd.ParallelOne;

            // The parallel point HAS to be known.
            if (parpos==null)
            {
                MessageBox.Show("Parallel point has not been calculated");
                return false;
            }

            // Treat the parallel point as the initial terminal.
            m_Terminal = parpos;

            // Get the offset to the parallel.
            double offset = cmd.GetPlanarOffset();

            // Figure out a search radius (the smaller of half the offset,
            // or half a centimetre at the current draw scale).
            double scale = cmd.ActiveDisplay.MapScale;
            ILength tol = new Length(Math.Min(offset*0.5, scale*0.005));

            // Search for the line closest to the parallel point, and
            // within the search radius. The line has to be visible
            // and selectable.
            CadastralMapModel map = CadastralMapModel.Current;
            //m_Line = map.FindClosestLine(parpos, tol, true);
            m_Line = (map.Index.QueryClosest(parpos, tol, SpatialType.Line) as LineFeature);

            // If we found something, highlight it (after confirming that
            // it really does intersect the parallel).
            if (m_Line!=null)
            {
                IPosition xsect = cmd.GetIntersect(m_Line, m_IsLast);
                if (xsect==null)
                    m_Line = null;
                else
                {
                    //pView->UnHighlight(m_pArc);
                    //pView->Highlight(*m_pArc);
                    m_Terminal = xsect;
                }
            }

            return true;
        }

        /// <summary>
        /// Reacts to the selection of a line feature.
        /// </summary>
        /// <param name="line">The line (if any) that has been selected.</param>
        internal void SelectLine(LineFeature line)
        {
            ISpatialDisplay view = m_Cmd.ActiveDisplay;
            IPosition xsect = null;
            ParallelUI cmd = Command;

            // Confirm that the line actually intersects the parallel.
            if (line!=null)
            {
                xsect = cmd.GetIntersect(line, m_IsLast);
                if (xsect==null)
                {
                    MessageBox.Show("Selected line does not intersect the parallel");

                    // De-select the line the user picked
                    EditingController.Current.ClearSelection();

                    // Re-highlight the arc if had originally (if any).
                    if (m_Line!=null)
                        EditingController.Current.Select(m_Line);

                    return;
                }
            }

            // Ensure everything is erased.
            m_Cmd.ErasePainting();

            // Hold on to new terminal position.
            if (xsect!=null)
                m_Terminal = xsect;
            else if (m_IsLast)
                m_Terminal = cmd.ParallelTwo;
            else
                m_Terminal = cmd.ParallelOne;

            // If we previously had an arc selected (and it's not the
            // newly selected line), ensure that it's been unhighlighted.
            //if (m_pArc && m_pArc != pArc) m_pArc->UnHighlight();

            // Hold on to the new terminal arc (if any).
            m_Line = line;

            // If it's defined, ensure the "don't use terminal" check
            // box is clear. And change the static text that tells the
            // user what to do.
            if (m_Line!=null)
            {
                noTerminalCheckBox.Checked = false;
                messageLabel2.Text = "If you want to terminate on a different line, select it.";
            }

            // Ensure everything is drawn as expected.
            cmd.Draw();

            // Resume focus on the Next/Finish button.
            okButton.Focus();
        }

        internal void Draw() // was Paint
        {
            // Ensure nothing else is currently selected/highlighted.
            ISpatialDisplay view = m_Cmd.ActiveDisplay;
            //m_Cmd.ErasePainting();

            // If we've got a terminal line, highlight it.
            if (m_Line!=null)
                m_Line.Render(view, new HighlightStyle());

            // And draw the terminal position on top of it.
            if (m_Terminal!=null)
                EditingController.Current.Style(Color.Yellow).Render(view, m_Terminal);
        }

        private void otherWayButton_Click(object sender, EventArgs e)
        {
            // Tell the command that's running this dialog.
            ParallelUI cmd = this.Command;
            cmd.ReverseArc();
        }

        int InitUpdate()
        {
            // Get the creating op.
            ParallelOperation op = UpdateOp;
            if (op==null)
                return 0;

            ISpatialDisplay view = m_Cmd.ActiveDisplay;
            ParallelUI cmd = Command;

            // The originally produced parallel may have been
            // changed to have a different offset.

            // Get the line that the parallel originally terminated on (if any).
            if (m_IsLast)
                m_Line = op.Terminal2;
            else
                m_Line = op.Terminal1;

            // If we didn't terminate on any particular line, that's
            // the way it will remain, Otherwise confirm that the
            // parallel continues to intersect it. In the event that
            // the parallel no longer intersects, get another
            // terminal position (and maybe a different terminal line).
            if (m_Line!=null)
            {
                m_Terminal = cmd.GetIntersect(m_Line, m_IsLast);
                if (m_Terminal!=null)
                {
                    // The parallel still intersects the terminal line that was originally specified,
                    // so highlight the terminal line (having de-selected and unhighlighted
                    // anything that was previously highlighted).

                    m_Line.Render(view, new HighlightStyle());
                    //pView->UnHighlight(m_pArc);
                    //pView->Highlight(*m_pArc);
                }
                else
                {
                    // Get a new terminal position (preferably coincident with some other line).

                    // DON'T try to find a new terminal if the op is being corrected due to a
                    // problem in rollforward preview. In that case, we want the user to see
                    // where the old terminal was ... well, leave that nicety for now.

                    if (!FindTerminal())
                        return -1;
                }
            }

            return 1;
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

        ParallelUI Command
        {
            get
            {
                UpdateUI up = (m_Cmd as UpdateUI);
                CommandUI cmd = (up==null ? m_Cmd : up.ActiveCommand);
                return (cmd as ParallelUI);
            }
        }

        string MyCaption
        {
            set
            {
                // do nothing for now
            }
        }
    }
}
