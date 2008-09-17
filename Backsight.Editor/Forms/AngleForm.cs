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

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" was="CdAngle"/>
    /// <summary>
    /// Dialog for new users who aren't familiar with how to specify angles as
    /// part of the dialog for creating a new connection path.
    /// </summary>
    /// <seealso cref="PathForm"/>
    partial class AngleForm : Form
    {
        #region Class data

        /// <summary>
        /// The angle in radians (always >0).
        /// </summary>
        double m_Radians;

        /// <summary>
        /// True if angle is clockwise.
        /// </summary>
        bool m_IsClockwise;

        /// <summary>
        /// True if a value was entered.
        /// </summary>
        bool m_IsDefined;

        /// <summary>
        /// True for deflection.
        /// </summary>
        bool m_IsDeflection;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor defines a blank dialog
        /// </summary>
        internal AngleForm()
        {
            InitializeComponent();

            m_Radians = 0.0;
            m_IsClockwise = true;
            m_IsDefined = false;
            m_IsDeflection = false;
        }

        /// <summary>
        /// Constructor for the angle at the start of a straight leg 
        /// (used when editing a connection path).
        /// </summary>
        /// <param name="leg">The leg that's being edited</param>
        internal AngleForm(StraightLeg leg)
        {
            InitializeComponent();

            m_Radians = leg.StartAngle;
            m_IsClockwise = (m_Radians>=0.0);
            m_Radians = Math.Abs(m_Radians);
            m_IsDefined = true;
            m_IsDeflection = leg.IsDeflection;
        }

        #endregion

        /// <summary>
        /// Is the angle a deflection?
        /// </summary>
        internal bool IsDeflection
        {
            get { return m_IsDeflection; }
        }

        private void angleTextBox_TextChanged(object sender, EventArgs e)
        {
            if (angleTextBox.Text.Trim().Length==0)
            {
                // If we already had angle info, reset it.
                m_Radians = 0.0;
                m_IsClockwise = true;

                // Field is empty, so revert to defaults.
                clockwiseRadioButton.Checked = false;
                clockwiseRadioButton.Enabled = false;

                counterClockwiseRadioButton.Checked = false;
                counterClockwiseRadioButton.Enabled = false;
            }
            else
            {
                // Enable ability to specify clockwise/counter-clockwise.
                clockwiseRadioButton.Enabled = true;
                counterClockwiseRadioButton.Enabled = true;

                ParseAngle();
            }
        }

        private void angleTextBox_Leave(object sender, EventArgs e)
        {
            // Parse the angle if the angle field contains something
            if (angleTextBox.Text.Trim().Length>0)
                ParseAngle();
        }

        private void clockwiseRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (clockwiseRadioButton.Checked)
                OnClockwise();
        }

        void OnClockwise()
        {
            m_IsClockwise = true;
            m_Radians = Math.Abs(m_Radians);
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
            m_Radians = Math.Abs(m_Radians);
            clockwiseRadioButton.Checked = false;
            counterClockwiseRadioButton.Checked = true;
        }

        /// <summary>
        /// Parses an explicitly entered angle. 
        /// </summary>
        /// <returns>True if angle parses ok.</returns>
        bool ParseAngle()
        {
            // Get the entered string.
            string dirstr = angleTextBox.Text.Trim();

            // If all we have is a "-", disable the ability to specify
            // clockwise angle & return.
            if (dirstr.Length>0 && dirstr[0]=='-')
            {
                clockwiseRadioButton.Enabled = false;
                clockwiseRadioButton.Checked = false;
                counterClockwiseRadioButton.Checked = true;
                if (dirstr.Length==1)
                    return false;
            }

            // If the entered angle contains a "d" (anywhere), treat it
            // as a deflection (and strip it out).
            dirstr = dirstr.ToUpper();
            if (dirstr.Contains("D"))
            {
                dirstr = dirstr.Replace("D", String.Empty);
                m_IsDeflection = true;
                deflectionCheckBox.Checked = true;
            }

            // Validate entered angle.
            double srad = 0.0;
            if (dirstr.Length>0)
            {
                double trad;
                if (!RadianValue.TryParse(dirstr, out trad))
                {
                    MessageBox.Show("Invalid angle.");
                    angleTextBox.Focus();
                    return false;
                }

                srad = trad;
            }

            // If we have signed radians, it HAS to be a counter-clockwise
            // angle. Otherwise make sure we preserve the directional sense.
            // which may have been previously defined.
            if (srad<0.0)
                m_IsClockwise = false;

            m_Radians = Math.Abs(srad);

            // Ensure radio buttons are in sync with what we have.
            if (m_IsClockwise)
                OnClockwise();
            else
                OnCounterClockwise();

            return true;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            // Anything previously entered is invalid.
            m_IsDefined = false;

            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // The field is defined if the angle has been validated.
            m_IsDefined = ParseAngle();

            DialogResult = DialogResult.OK;
            Close();
        }

        internal string Format()
        {
            // Return empty string if angle is undefined. Otherwise return
            // angle in DMS, with leading "-" if it's counter-clockwise.

            if (!m_IsDefined)
                return String.Empty;

            string result;

            if (m_IsClockwise)
                result = RadianValue.AsShortString(m_Radians);
            else
                result = RadianValue.AsShortString(-m_Radians);

            if (m_IsDeflection)
                result += "d";

            // Append a space
            result += " ";

            return result;
        }

        private void deflectionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            m_IsDeflection = deflectionCheckBox.Checked;
        }

        private void AngleForm_Shown(object sender, EventArgs e)
        {
            // Return if the angle isn't defined.
            if (!m_IsDefined)
                return;

            if (m_IsClockwise)
            {
                clockwiseRadioButton.Checked = true;
                counterClockwiseRadioButton.Checked = false;
            }
            else
            {
                clockwiseRadioButton.Checked = false;
                counterClockwiseRadioButton.Checked = true;
            }

            deflectionCheckBox.Checked = m_IsDeflection;
            angleTextBox.Text = RadianValue.AsShortString(m_Radians);
        }

        double SignedAngle
        {
            get
            {
                if (m_IsClockwise)
                    return m_Radians;
                else
                    return -m_Radians;
            }
        }
    }
}