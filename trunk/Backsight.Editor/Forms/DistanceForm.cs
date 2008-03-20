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
        }

        #endregion

        private void DistanceForm_Shown(object sender, EventArgs e)
        {

        }

        private void distanceTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void x1Button_Click(object sender, EventArgs e)
        {

        }

        private void x2Button_Click(object sender, EventArgs e)
        {

        }

        private void x3Button_Click(object sender, EventArgs e)
        {

        }

        private void x4Button_Click(object sender, EventArgs e)
        {

        }

        private void x5Button_Click(object sender, EventArgs e)
        {

        }

        private void x6Button_Click(object sender, EventArgs e)
        {

        }

        private void x7Button_Click(object sender, EventArgs e)
        {

        }

        private void x8Button_Click(object sender, EventArgs e)
        {

        }

        private void x9Button_Click(object sender, EventArgs e)
        {

        }

        private void mRadioButton_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void fRadioButton_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cRadioButton_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void newDefaultCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void wantLineCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void wantPointCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cancelButton_Click(object sender, EventArgs e)
        {

        }

        private void okButton_Click(object sender, EventArgs e)
        {

        }
    }
}