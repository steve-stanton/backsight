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
    /// <summary>
    /// Dialog for letting the user select the type of inverse calculations
    /// they want to perform.
    /// </summary>
    public partial class InverseChoiceForm : Form
    {
        #region Class data

        /// <summary>
        /// The selected option
        /// </summary>
        InverseType m_Option;

        /// <summary>
        /// The dialog created to accommodate the selected option (null if option
        /// is currently <c>InverseType.None</c>)
        /// </summary>
        InverseForm m_Dialog;

        #endregion

        #region Constructors

        public InverseChoiceForm()
        {
            InitializeComponent();

            m_Option = InverseType.None;
            m_Dialog = null;
        }

        #endregion

        internal InverseForm SelectedForm
        {
            get { return m_Dialog; }
        }

        void SelectAndClose(InverseType it, InverseForm dial)
        {
            m_Option = it;
            m_Dialog = dial;
            DialogResult = (it==InverseType.None ? DialogResult.Cancel : DialogResult.OK);
            Close();
        }

        private void distanceButton_Click(object sender, EventArgs e)
        {
            SelectAndClose(InverseType.Distance, new InverseDistanceForm()); // CdInvDist
        }

        private void distanceBearingButton_Click(object sender, EventArgs e)
        {
            SelectAndClose(InverseType.DistanceBearing, null); // CdInvDistBear
        }

        private void distanceAngleButton_Click(object sender, EventArgs e)
        {
            SelectAndClose(InverseType.DistanceAngle, null); // CdInvDistAng
        }

        private void arcDistanceButton_Click(object sender, EventArgs e)
        {
            SelectAndClose(InverseType.ArcDistance, null); // CdInvArcDist
        }

        private void arcDistanceBearingButton_Click(object sender, EventArgs e)
        {
            SelectAndClose(InverseType.ArcDistanceBearing, null);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            SelectAndClose(InverseType.None, null); // CdInvArcDistBear
        }
    }
}