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

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" was="CdCuldesac"/>
    /// <summary>
    /// Dialog for new users who aren't familiar with how to specify cul-de-sacs as
    /// part of the dialog for creating a new connection path.
    /// </summary>
    /// <seealso cref="PathForm"/>
    partial class CulDeSacForm : Form
    {
        #region Class data

        /// <summary>
        /// The central angle in radians (always >0).
        /// </summary>
        double m_Radians;

        /// <summary>
        /// The radius of the arc that forms the cul-de-sac
        /// </summary>
        Distance m_Radius;

        /// <summary>
        /// True if angle is clockwise.
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
        internal CulDeSacForm()
        {
            InitializeComponent();

            m_Radians = 0.0;
            m_Radius = null;
            m_IsClockwise = true;
            m_IsDefined = false;
        }

        /// <summary>
        /// Constructor for use when editing a connection path
        /// </summary>
        /// <param name="leg">The leg that's being edited</param>
        internal CulDeSacForm(CircularLeg leg)
        {
            InitializeComponent();

            m_Radians = leg.CentralAngle;
            m_Radius = new Distance(leg.ObservedRadius);
            m_IsClockwise = leg.IsClockwise;
            m_IsDefined = true;
        }

        #endregion

        /// <summary>
        /// The central angle in radians (always >0).
        /// </summary>
        internal double CentralAngle
        {
            get { return m_Radians; }
        }

        /// <summary>
        /// The radius of the arc that forms the cul-de-sac
        /// </summary>
        internal Distance Radius
        {
            get { return m_Radius; }
        }

        /// <summary>
        /// True if angle is clockwise.
        /// </summary>
        internal bool IsClockwise
        {
            get { return m_IsClockwise; }
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
            // Validate the angle.
            if (!ParseAngle())
            {
                m_IsDefined = false;
                angleTextBox.Focus();
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

        /// <summary>
        /// Parses an explicitly entered angle.
        /// </summary>
        /// <returns>True if angle parses ok.</returns>
        bool ParseAngle()
        {
            // Get the entered string.
            string str = angleTextBox.Text.Trim();

            // A leading "-" is not expected.
            if (str.Length>0 && str[0]=='-')
            {
                MessageBox.Show("Central angles are not signed.");
                return false;
            }

            // Validate entered angle.
            double srad;
            if (!RadianValue.TryParse(str, out srad))
            {
                MessageBox.Show("Invalid central angle.");
                return false;
            }

            m_Radians = Math.Abs(srad);
            return true;
        }

        /// <summary>
        /// Parses an explicitly entered radius.
        /// </summary>
        /// <returns>True if radius parses ok.</returns>
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

        internal string Format()
        {
            // Return empty string if angle is undefined. Otherwise return
            // angle in DMS, followed by radius.

            if (!m_IsDefined)
                return String.Empty;

            string result = String.Format("{0}ca {1}",
                RadianValue.AsShortString(m_Radians), m_Radius.Format());

            if (m_IsClockwise)
                result += "/ ";
            else
                result += " cc/ ";

            return result;

        }

        private void CulDeSacForm_Shown(object sender, EventArgs e)
        {
            // If the curve is already defined (e.g. we are doing an update),
            // display the previously defined stuff. Otherwise just initialize
            // for a clockwise curve (see default constructor).

            if (m_IsDefined)
            {
                // Central angle
                angleTextBox.Text = RadianValue.AsShortString(m_Radians);

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
        }

        private void radiusTextBox_TextChanged(object sender, EventArgs e)
        {
            m_IsDefined = (radiusTextBox.Text.Trim().Length>0);
        }
    }
}