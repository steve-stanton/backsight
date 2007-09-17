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
using System.Diagnostics;

namespace Backsight.Editor.Forms
{
    public partial class GetOffsetForm : Form
    {
        #region Class data

        /// <summary>
        /// The command that is running this dialog.
        /// </summary>
        CommandUI m_Cmd;
        
        /// <summary>
        /// The offset defined via this dialog.
        /// </summary>
        Offset m_Offset;

        /// <summary>
        /// For inhibiting event handlers.
        /// </summary>
        bool m_IsStatic;

        #endregion

        #region Constructors

        internal GetOffsetForm(CommandUI cmd, Offset offset)
        {
            InitializeComponent();

            m_Cmd = cmd;
            m_Offset = null;

            if (offset!=null)
            {
                if (offset is OffsetDistance)
                    m_Offset = new OffsetDistance(offset as OffsetDistance);
                else if (offset is OffsetPoint)
                    m_Offset = new OffsetPoint(offset as OffsetPoint);
            }

            // Don't let OnChangeOffset delete any offset until OnInitDialog has finished.
	        m_IsStatic = true;
        }

        #endregion

        private void GetOffsetForm_Shown(object sender, EventArgs e)
        {
            // If we already have an offset, display it.
            if (m_Offset!=null)
            {
                // If it's an offset point, just display the point ID and disable the left/right
                // radio buttons. For an offset distance, display the distance and check
                // the appropriate radio button.

                if (m_Offset is OffsetPoint)
                {
                    OffsetPoint op = (m_Offset as OffsetPoint);
                    offsetTextBox.Text = String.Format("+{0}", op.Point.FormattedKey);
                    leftRadioButton.Enabled = rightRadioButton.Enabled = false;
                }
                else if (m_Offset is OffsetDistance)
                {
                    OffsetDistance od = (m_Offset as OffsetDistance);
                    Distance dist = od.Offset;
                    offsetTextBox.Text = dist.Format();

                    if (od.IsRight)
                    {
                        leftRadioButton.Checked = false;
                        rightRadioButton.Checked = true;
                    }
                    else
                    {
                        rightRadioButton.Checked = false;
                        leftRadioButton.Checked = true;
                    }
                }
                else
                    throw new ArgumentException("Unexpected type of offset");
            }
            else
            {
                // Disable the left/right radios. They'll get enabled if
                // an offset distance is entered.
                leftRadioButton.Enabled = rightRadioButton.Enabled = false;
            }

            // OnChangeOffset is now free to null m_Offset as required.
            m_IsStatic = false;
        }

        Offset Offset
        {
            get { return m_Offset; }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            m_Cmd.DialAbort(this);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // The offset may not be defined (user may be trying to get rid
            // of a previously defined offset).

            m_Cmd.DialFinish(this);
        }

        private void leftRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (leftRadioButton.Checked)
                SetOffsetRight(false);
        }

        private void rightRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (rightRadioButton.Checked)
                SetOffsetRight(true);
        }

        void SetOffsetRight(bool isRight)
        {
            // The offset HAS to be an offset distance.
            OffsetDistance dist = (m_Offset as OffsetDistance);
            if (dist==null)
                return;

            // Record the offset side.
            if (isRight)
                dist.SetRight();
            else
                dist.SetLeft();

            // Tell the command.
            m_Cmd.SetOffset(dist);
        }

        private void offsetTextBox_TextChanged(object sender, EventArgs e)
        {
            // Do NOTHING if the offset is being displayed during startup
            if (m_IsStatic)
                return;

            // If the offset field is empty, disable the left/right radio buttons.
            // Otherwise enable them so long as we have an offset distance.
            string str = offsetTextBox.Text.Trim();
            if (str.Length==0)
            {
                leftRadioButton.Enabled = rightRadioButton.Enabled = false;
                m_Offset = null;
                m_Cmd.SetOffset(null);
            }
            else
            {
                // If the offset is NOT an offset point, parse the offset,
                // notify the command, and enable the left/right radios.
                if (m_Offset is OffsetPoint)
                    return;

                // Get the entered text and parse it.
                Distance dist = new Distance(str);
                if (!dist.IsDefined)
                    return;

                // If we previously had an offset distance, see whether
                // it is to the left or right. Then throw it away.
                bool isLeft = true;
                if (m_Offset!=null)
                {
                    OffsetDistance offDist = (m_Offset as OffsetDistance);
                    Debug.Assert(offDist!=null);
                    isLeft = !offDist.IsRight;
                    m_Offset = null;
                }
                else
                {
                    rightRadioButton.Checked = false;
                    leftRadioButton.Checked = true;
                }

                // Enable radio buttons.
                leftRadioButton.Enabled = rightRadioButton.Enabled = true;

                // Create a new offset distance and tell the command.
                m_Offset = new OffsetDistance(dist, isLeft);
                m_Cmd.SetOffset(m_Offset);
            }
        }

        internal void OnSelectPoint(PointFeature point)
        {
            // Return if point is not defined.
            if (point==null)
                return;

            // Get rid of any existing offset.
            m_Offset = null;

            // Create an offset point object.
            m_Offset = new OffsetPoint(point);

            // Display the key of the offset point.
            offsetTextBox.Text = String.Format("+{0}", point.FormattedKey);

            // Disable the left-right radio buttons.
            leftRadioButton.Enabled = rightRadioButton.Enabled = false;

            // Tell the command.
            m_Cmd.SetOffset(m_Offset);
        }
    }
}
