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
using Backsight.Editor.Observations;
using Backsight.Editor.UI;

namespace Backsight.Editor.Forms
{
    /// <summary>
    /// Dialog for the <see cref="PointOnLineUI"/>
    /// </summary>
    public partial class PointOnLineControl : UserControl
    {
        #region Class data

        /// <summary>
        /// The command that is running this dialog.
        /// </summary>
        CommandUI m_Cmd;

        /// <summary>
        /// The line that is being subdivided.
        /// </summary>
        LineFeature m_Line;

        /// <summary>
        /// The distance to the split point.
        /// </summary>
        Distance m_Length;

        /// <summary>
        /// True if distance is from the end of the line. False if from start.
        /// </summary>
        bool m_IsFromEnd;

        /// <summary>
        /// The maximum length, in metres on the ground (as opposed to the mapping plane).
        /// </summary>
        double m_MaxLength;

        #endregion

        #region Constructors

        internal PointOnLineControl(PointOnLineUI cmd, LineFeature line, Operation recall)
        {
            InitializeComponent();

            m_Cmd = cmd;
            m_Line = line;
            m_MaxLength = 0.0;

            PointOnLineOperation op = (recall as PointOnLineOperation);
            if (op!=null)
            {
                m_Length = new Distance(op.Distance);
                m_IsFromEnd = m_Length.SetPositive();
            }
            else
            {
                m_Length = new Distance();
                m_IsFromEnd = false;
            }
        }
        internal PointOnLineControl(UpdateUI updcmd)
        {
            InitializeComponent();

            m_Cmd = updcmd;
            m_Line = null;
            m_IsFromEnd = false;
            m_Length = new Distance();
            m_MaxLength = 0.0;
        }

        #endregion

        /// <summary>
        /// True if distance is from the end of the line. False if from start.
        /// </summary>
        internal bool IsFromEnd
        {
            get { return m_IsFromEnd; }
        }

        /// <summary>
        /// The distance to the split point.
        /// </summary>
        internal Distance Length
        {
            get { return m_Length; }
        }

        private void PointOnLineControl_Load(object sender, EventArgs e)
        {
            // Hide warning message.
            warningLabel.Visible = false;

            // If we are doing an update, pull in the previously defined stuff
            InitUpdate();

            // Get the max observed length.
            m_MaxLength = m_Line.GroundLength.Meters;

            // Display the original observed distance (if any). Do this
            // AFTER setting m_MaxLength (otherwise IDC_WARNING will appear
            // when doing an update).
            if (m_Length.IsDefined)
                distanceTextBox.Text = m_Length.Format();
        }

        private void distanceTextBox_TextChanged(object sender, EventArgs e)
        {
            // Parse the distance to the split point.
            string d = distanceTextBox.Text.Trim();
            if (d.Length==0)
                m_Length = new Distance();
            else
                m_Length = new Distance(d);

            // If the observed length is greater than the max, reveal warning.
            warningLabel.Visible = (m_Length.Meters > m_MaxLength);

            // Clear current highlightling (Draw will be called in idle time)
            m_Cmd.ErasePainting();
        }

        private void otherWayButton_Click(object sender, EventArgs e)
        {
            // Toggle the end the distance is observed from.
            m_IsFromEnd = !m_IsFromEnd;

            // Set the focus to something useful (keeping the focus on
            // the "other way" button is kind of useless).
            distanceTextBox.Focus();

            // Clear current highlightling (Draw will be called in idle time)
            m_Cmd.ErasePainting();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            // Abort the command.
            m_Cmd.DialAbort(this);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (m_Line!=null && m_Length.IsDefined)
            {
                // The observed length can't be too long.
                if (m_Length.Meters > m_MaxLength)
                {
                    MessageBox.Show("Observed distance is longer than the length of the line.");
                    return;
                }

                m_Cmd.DialFinish(this);
            }
            else
            {
                if (m_Line==null)
                    MessageBox.Show("The line to subdivide has not been specified.");
                else
                    MessageBox.Show("The distance to the split point has not been specified.");
            }
        }

        internal void Draw()
        {
            ISpatialDisplay display = m_Cmd.ActiveDisplay;

            // If we're doing an update, draw the original split point in grey.
            PointOnLineOperation pop = UpdateOp;
            if (pop!=null)
            {
                PointFeature point = pop.NewPoint;
                if (point!=null)
                    point.Draw(display, Color.Gray);
            }

            // Ensure the line that's being subdivided is still highlighted
            IDrawStyle style = EditingController.Current.HighlightStyle;
            m_Line.Render(display, style);

            // Calculate the position of the split point.
            IPosition splitpos = PointOnLineUI.Calculate(m_Line, m_Length, m_IsFromEnd);
            if (splitpos!=null)
            {
                style = EditingController.Current.Style(Color.Magenta);
                style.Render(display, splitpos);
            }
        }

        PointOnLineOperation UpdateOp
        {
            get
            {
                UpdateUI up = (m_Cmd as UpdateUI);
                return (up==null ? null : (PointOnLineOperation)up.GetOp());
            }
        }

        bool IsUpdate
        {
            get { return (m_Cmd is UpdateUI); }
        }

        bool InitUpdate()
        {
            // Get the creating op.
            PointOnLineOperation pop = this.UpdateOp;
            if (pop==null)
                return false;

            Form parent = ParentForm;
            parent.Text = "Update Line Subdivision";

            m_Line = pop.Line;
            m_Length = new Distance(pop.Distance);
            m_IsFromEnd = m_Length.SetPositive();

            return true;
        }
    }
}
