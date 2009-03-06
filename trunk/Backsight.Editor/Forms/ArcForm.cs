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

using Backsight.Editor.Observations;


namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" was="CdAngle"/>
    /// <summary>
    /// Dialog for new users who aren't familiar with how to specify circular arcs as
    /// part of the dialog for creating a new connection path.
    /// </summary>
    /// <seealso cref="PathForm"/>
    partial class ArcForm : Form
    {
        #region Class data

        /// <summary>
        /// The angle at the BC (in radians).
        /// </summary>
        double m_Angle1;

        /// <summary>
        /// The angle at the EC (in radians).
        /// </summary>
        double m_Angle2;

        /// <summary>
        /// The radius of the arc
        /// </summary>
        Distance m_Radius;

        /// <summary>
        /// True if arc is clockwise.
        /// </summary>
        bool m_IsClockwise;

        /// <summary>
        /// True if arc defined
        /// </summary>
        bool m_IsDefined;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor defines a blank dialog
        /// </summary>
        internal ArcForm()
        {
            InitializeComponent();

            m_Angle1 = MathConstants.PIDIV2;
            m_Angle2 = MathConstants.PIDIV2;
            m_Radius = null;
            m_IsClockwise = true;
            m_IsDefined = false;
        }

        /// <summary>
        /// Constructor for use when editing a connection path
        /// </summary>
        /// <param name="leg">The leg that's being edited</param>
        internal ArcForm(CircularLeg leg)
        {
            InitializeComponent();

            m_Angle1 = leg.EntryAngle;
            m_Angle2 = leg.ExitAngle;
            m_Radius = new Distance(leg.ObservedRadius);
            m_IsClockwise = leg.IsClockwise;
            m_IsDefined = true;
        }

        #endregion

        /// <summary>
        /// The angle at the BC (in radians).
        /// </summary>
        internal double EntryAngle
        {
            get { return m_Angle1; }
        }

        /// <summary>
        /// The angle at the EC (in radians).
        /// </summary>
        internal double ExitAngle
        {
            get { return m_Angle2; }
        }

        /// <summary>
        /// The radius of the arc
        /// </summary>
        internal Distance Radius
        {
            get { return m_Radius; }
        }

        /// <summary>
        /// True if arc is clockwise.
        /// </summary>
        internal bool IsClockwise
        {
            get { return m_IsClockwise; }
        }

        private void ecAngleTextBox_Leave(object sender, EventArgs e)
        {
            // If the field is empty, fill it with whatever was
            // in the BC angle field.
            if (ecAngleTextBox.Text.Trim().Length==0)
                ecAngleTextBox.Text = bcAngleTextBox.Text;
        }

        private void clockwiseRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (clockwiseRadioButton.Checked)
                OnClockwise();
        }

        void OnClockwise()
        {
            m_IsClockwise = true;
            clockwiseRadioButton.Checked = true;
            counterClockwiseRadioButton.Checked = false;
        }

        private void counterClockwiseRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (counterClockwiseRadioButton.Checked)
                OnCounterClockwise();
        }

        void OnCounterClockwise()
        {
            m_IsClockwise = false;
            clockwiseRadioButton.Checked = false;
            counterClockwiseRadioButton.Checked = true;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            m_IsDefined = false;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // Validate the two angles.
            if (!ParseAngle(bcAngleTextBox))
            {
                m_IsDefined = false;
                bcAngleTextBox.Focus();
                return;
            }

            if (!ParseAngle(ecAngleTextBox))
            {
                m_IsDefined = false;
                ecAngleTextBox.Focus();
                return;
            }

            // Validate the radius.
            if (!ParseRadius())
            {
                m_IsDefined = false;
                radiusTextBox.Focus();
                return;
            }

            // Remember the dialog has been validated.
            m_IsDefined = true;

            DialogResult = DialogResult.OK;
            Close();
        }

        internal string Format()
        {
            // Return empty string if angle is undefined. Otherwise return
            // 1 or 2 angles in DMS, followed by radius and any
            // counter-clockwise curve indicator.

            if (!m_IsDefined)
                return String.Empty;

            string result;

            if (Math.Abs(m_Angle1-m_Angle2) < MathConstants.TINY)
                result = String.Format("({0} {1}",
                            RadianValue.AsShortString(m_Angle1),
                            m_Radius.Format());
            else
                result = String.Format("({0} {1} {2}",
                            RadianValue.AsShortString(m_Angle1),
                            RadianValue.AsShortString(m_Angle2),
                            m_Radius.Format());

            if (m_IsClockwise)
                result += "/ ";
            else
                result += " CC/ ";

            return result;
        }

        /// <summary>
        /// Parses an explicitly entered angle.
        /// </summary>
        /// <param name="tb">The control containing the entered angle</param>
        /// <returns>True if angle parses ok.</returns>
        bool ParseAngle(TextBox tb)
        {
            // Get the entered string.
            string str = tb.Text.Trim();

            // A leading "-" is not expected.
            if (str.Length>0 && str[0]=='-')
            {
                MessageBox.Show("Angles should not be signed.");
                return false;
            }

            // Validate entered angle. When dealing with the exit angle, only treat
            // it as an error if the field contains something. This assumes that
            // the entry angle is parsed FIRST.

            if (str.Length==0)
            {
                if (!Object.ReferenceEquals(tb, ecAngleTextBox))
                {
                    MessageBox.Show("The angle at the BC must be specified.");
                    return false;
                }

                m_Angle2 = m_Angle1;
                return true;
            }
 
            double srad;
            if (!RadianValue.TryParse(str, out srad))
            {
                MessageBox.Show("Invalid angle.");
                return false;
            }

            if (Object.ReferenceEquals(tb, bcAngleTextBox))
                m_Angle1 = Math.Abs(srad);
            else
                m_Angle2 = Math.Abs(srad);

            return true;
        }

        /// <summary>
        /// Parses an explicitly entered radius.
        /// </summary>
        /// <returns>True if radius parses ok.</returns>
        /// <remarks>This is identical to the version in CulDeSacForm.</remarks>
        bool ParseRadius()
        {
            // Get the entered string.
            string str = radiusTextBox.Text.Trim();

            // No distance if empty string.
            if (str.Length==0)
            {
                MessageBox.Show("The radius has not been specified.");
                return false;
            }

            // Parse the distance
            Distance radius;
            if (!Distance.TryParse(str, out radius))
            {
                MessageBox.Show("Invalid radius.");
                return false;
            }

            if (radius.Meters < MathConstants.TINY)
            {
                MessageBox.Show("Radius must be greater than zero.");
                return false;
            }

            // Remember the specified radius
            m_Radius = radius;
            return true;
        }

        private void ArcForm_Shown(object sender, EventArgs e)
        {
            // Entry and exit angles
            bcAngleTextBox.Text = RadianValue.AsShortString(m_Angle1);
            ecAngleTextBox.Text = RadianValue.AsShortString(m_Angle2);

            // If the curve is already defined (e.g. we are doing an update),
            // display the prviously defined stuff. Otherwise just initialize
            // for a clockwise curve (see default constructor).

            if (m_IsDefined)
            {
                // Observed radius
                radiusTextBox.Text = m_Radius.Format();

                // Direction of curve
                if (m_IsClockwise)
                    OnClockwise();
                else
                    OnCounterClockwise();
            }
            else
                OnClockwise();

            // Always start in the radius field.
            radiusTextBox.Focus();
        }
    }
}