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
using System.Drawing;

namespace Backsight.Editor.Forms
{
    /// <summary>
    /// Dialog for showing the results of the adjustment performed for a connection path.
    /// </summary>
    partial class AdjustmentForm : Form
    {
        #region Class data

        /// <summary>
        /// Total observed length (in meters)
        /// </summary>
        readonly double m_Length;

        /// <summary>
        /// Misclosure in northing (in meters)
        /// </summary>
        readonly double m_DeltaN;

        /// <summary>
        /// Misclosure in easthing (in meters)
        /// </summary>
        readonly double m_DeltaE;

        /// <summary>
        /// Precision denominator
        /// </summary>
        readonly double m_Precision;

        /// <summary>
        /// The dialog that invoked this dialog.
        /// </summary>
        readonly PathForm m_Parent;

        #endregion

        #region Constructors

        internal AdjustmentForm(double dN, double dE, double precision, double length, PathForm parent)
        {
            InitializeComponent();

            m_DeltaN = dN;
            m_DeltaE = dE;
            m_Precision = precision;
            m_Length = length;
            m_Parent = parent;
        }

        #endregion

        private void AdjustmentForm_Shown(object sender, EventArgs e)
        {
            // Display at top left corner of the screen.
            this.Location = new Point(0, 0);

            // Display the adjustment results in the current data entry units.
            CadastralMapModel map = CadastralMapModel.Current;
            DistanceUnit entry = map.EntryUnit;

            DistanceUnitType unitType = entry.UnitType;
            if (unitType==DistanceUnitType.Feet)
                OnFeet();
            else if (unitType==DistanceUnitType.Chains)
                OnChains();
            else
                OnMeters();

            if (Math.Abs(m_Precision) < MathConstants.TINY)
                precisionLabel.Text = "exact";
            else
                precisionLabel.Text = String.Format("1:{0:0.0}", m_Precision);
        }

        private void mRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (mRadioButton.Checked)
                OnMeters();
        }

        void OnMeters()
        {
            mRadioButton.Checked = true;
            ShowResults(DistanceUnitType.Meters);
        }

        private void fRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (fRadioButton.Checked)
                OnFeet();
        }

        void OnFeet()
        {
            fRadioButton.Checked = true;
            ShowResults(DistanceUnitType.Feet);
        }

        private void cRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (cRadioButton.Checked)
                OnChains();
        }

        void OnChains()
        {
            cRadioButton.Checked = true;
            ShowResults(DistanceUnitType.Chains);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();

            m_Parent.OnDestroyAdj();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();

            // Tell the parent to save the path.
            m_Parent.Save();
        }

        void ShowResults(DistanceUnitType type)
        {
            CadastralMapModel map = CadastralMapModel.Current;
            DistanceUnit unit = map.GetUnits(type);
            if (unit==null)
                return;

            lengthLabel.Text = unit.Format(m_Length, false, -1);
            deltaNorthingLabel.Text = unit.Format(m_DeltaN, false, -1);
            deltaEastingLabel.Text = unit.Format(m_DeltaE, false, -1);
        }
    }
}