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
    /// <written by="Steve Stanton" was="CdDistance"/>
    /// <summary>
    /// Dialog for new users who aren't familiar with how to specify distances as
    /// part of the dialog for creating a new connection path.
    /// </summary>
    /// <seealso cref="PathForm"/>
    partial class DistanceForm : Form
    {
        #region Class data

        /// <summary>
        /// The entered distance
        /// </summary>
        double m_Distance;

        /// <summary>
        /// The repeat count
        /// </summary>
        uint m_Repeat;

        /// <summary>
        /// The selected distance unit.
        /// </summary>
        DistanceUnit m_Unit;

        /// <summary>
        /// The units on entry to dialog.
        /// </summary>
        DistanceUnit m_CurUnit;

        /// <summary>
        /// True if default units should be changed.
        /// </summary>
        bool m_NewUnit;

        /// <summary>
        /// True if a line should be created.
        /// </summary>
        bool m_WantLine;

        /// <summary>
        /// True if a point should be created (can only be false if m_WantLine
        /// is also false).
        /// </summary>
        bool m_WantPoint;

        #endregion

        #region Constructors

        internal DistanceForm()
        {
            InitializeComponent();

            m_Distance = 0.0;
            m_Repeat = 1;
            m_Unit = EditingController.Current.EntryUnit;
            m_CurUnit = m_Unit;
            m_NewUnit = false;
            m_WantLine = true;
            m_WantPoint = true;
        }

        #endregion

        private void x1Button_Click(object sender, EventArgs e)
        {
            m_Repeat = 1;
        }

        private void x2Button_Click(object sender, EventArgs e)
        {
            m_Repeat = 2;
        }

        private void x3Button_Click(object sender, EventArgs e)
        {
            m_Repeat = 3;
        }

        private void x4Button_Click(object sender, EventArgs e)
        {
            m_Repeat = 4;
        }

        private void x5Button_Click(object sender, EventArgs e)
        {
            m_Repeat = 5;
        }

        private void x6Button_Click(object sender, EventArgs e)
        {
            m_Repeat = 6;
        }

        private void x7Button_Click(object sender, EventArgs e)
        {
            m_Repeat = 7;
        }

        private void x8Button_Click(object sender, EventArgs e)
        {
            m_Repeat = 8;
        }

        private void x9Button_Click(object sender, EventArgs e)
        {
            m_Repeat = 9;
        }

        private void distanceTextBox_TextChanged(object sender, EventArgs e)
        {
            // Get the entered string.
            string str = distanceTextBox.Text.Trim();

            // No distance if empty string.
            if (str.Length == 0)
            {
                m_Distance = 0.0;
                return;
            }

            double dist;
            if (!Double.TryParse(str, out dist))
            {
                MessageBox.Show("Invalid distance");
                distanceTextBox.Focus();
                return;
            }

            m_Distance = dist;
        }

        private void fRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            m_Unit = EditingController.Current.GetUnits(DistanceUnitType.Feet);
            SetNewDefault();
        }

        private void mRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            m_Unit = EditingController.Current.GetUnits(DistanceUnitType.Meters);
            SetNewDefault();
        }

        private void cRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            m_Unit = EditingController.Current.GetUnits(DistanceUnitType.Chains);
            SetNewDefault();
        }

        void SetNewDefault()
        {
            // We don't automatically make a selected unit the new default.

            newDefaultCheckBox.Checked = false;
            m_NewUnit = false;

            // Enable or disable the check box that lets you make the
            // specified unit the new default.

            if (Object.ReferenceEquals(m_Unit, m_CurUnit))
                newDefaultCheckBox.Enabled = false;
            else
                newDefaultCheckBox.Enabled = true;
        }

        private void newDefaultCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            m_NewUnit = newDefaultCheckBox.Checked;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            // Assign invalid values to everything.
            m_Repeat = 0;
            m_Distance = 0.0;
            m_Unit = null;
            m_NewUnit = false;
            m_WantLine = false;
            m_WantPoint = false;

            DialogResult = DialogResult.Cancel;
            Close();
        }

        internal string Format()
        {
            // If the user elected to change the default data entry units,
            // define the characters for doing this.

            string uchars; // Chars for new default units

            if (m_NewUnit)
                uchars = String.Format("{0}... ", m_Unit.Abbreviation);
            else
                uchars = String.Empty;

            // See if we should append an abbreviation. We do this so long
            // as a non-default unit has been picked, but the user does NOT
            // want to make it the new default.

            string abbrev; // Any abbreviation to append to the entered distance

            if (m_NewUnit || Object.ReferenceEquals(m_Unit, m_CurUnit))
                abbrev = String.Empty;
            else
                abbrev = m_Unit.Abbreviation;

            // Format the distance value.

            string result;

            if (m_Repeat == 0)
                result = uchars;
            else
                result = String.Format("{0}{1}{2}", uchars, m_Distance, abbrev);

            if (m_Repeat == 1)
                result += " ";
            else
                result += String.Format("*{0} ", m_Repeat);

            // Append flag if a line should not be created. If no line, we
            // may also want no point either.

            if (!m_WantLine)
            {
                if (m_WantPoint)
                    result += "/- "; // = /MC
                else
                    result += "/* "; // = /OP
            }

            return result;
        }

        private void DistanceForm_Shown(object sender, EventArgs e)
        {
            // Display current data entry units.
            if (m_CurUnit.UnitType == DistanceUnitType.Meters)
                mRadioButton.Checked = true;
            else if (m_CurUnit.UnitType == DistanceUnitType.Feet)
                fRadioButton.Checked = true;
            else if (m_CurUnit.UnitType == DistanceUnitType.Chains)
                cRadioButton.Checked = true;

            // Disable checkbox to allow new default units.
            newDefaultCheckBox.Checked = false;
            newDefaultCheckBox.Enabled = false;

            // Set "Create line" check mark.
            m_WantLine = true;
            wantLineCheckBox.Checked = true;

            // Set "Create point" check mark, but disable the window.
            m_WantPoint = true;
            wantPointCheckBox.Checked = true;
            wantPointCheckBox.Enabled = false;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // Confirm a valid distance has been specified. If not, set the
            // repeat count to zero so that a subsequent attempt to format
            // the distance will lead to a blank string (this makes it
            // possible to change data entry units without actually entering
            // a distance).

            if (m_Distance <= 0.0)
                m_Repeat = 0;

            // Change the default units if required
            if (m_NewUnit)
                EditingController.Current.JobFile.Data.EntryUnitType = m_Unit.UnitType;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void wantLineCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            m_WantLine = wantLineCheckBox.Checked;

            // If a line is desired, enable or disable the ability to say
            // you want a point too.
            wantPointCheckBox.Checked = true;

            if (m_WantLine)
                wantPointCheckBox.Enabled = false;
            else
                wantPointCheckBox.Enabled = true;
        }

        private void wantPointCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            m_WantPoint = wantPointCheckBox.Checked;
        }
    }
}