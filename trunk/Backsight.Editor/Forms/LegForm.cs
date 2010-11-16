// <remarks>
// Copyright 2010 - Steve Stanton. This file is part of Backsight
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
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;

using Backsight.Editor.Observations;
using Backsight.Editor.Operations;


namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" was="CdLeg" />
    /// <summary>
    /// Dialog for specifying observed lengths for a second face of a
    /// subdivided line (or a second face of a leg on a connection path).
    /// </summary>
    public partial class LegForm : Form
    {
        #region Class data

        /// <summary>
        /// Length of leg (in meters on the ground)
        /// </summary>
        readonly double m_Length;

        /// <summary>
        /// Length entered so far (in meters on the ground)
        /// </summary>
        double m_Entered;

        /// <summary>
        /// The distances involved.
        /// </summary>
        Distance[] m_Distances;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LegForm"/> class.
        /// </summary>
        /// <param name="len">Length of leg (in meters on the ground)</param>
        public LegForm(double len)
        {
            InitializeComponent();

            m_Length = len;
            m_Entered = 0.0;
            m_Distances = null;
        }

        #endregion

        private void LegForm_Shown(object sender, EventArgs e)
        {
            // Display the length of the leg (in the current data
            // entry units).

            DistanceUnit dunit = EditingController.Current.EntryUnit;
            ShowResults(dunit.UnitType);
        }

        private void distancesTextBox_TextChanged(object sender, EventArgs e)
        {
            // Get an array of the distances.
            m_Entered = GetDistances();

            // Display the total entered length & what's left.
            ShowResults(GetUnits());
        }

        DistanceUnitType GetUnits()
        {
            if (metersRadioButton.Checked)
                return DistanceUnitType.Meters;

            if (feetRadioButton.Checked)
                return DistanceUnitType.Feet;

            if (chainsRadioButton.Checked)
                return DistanceUnitType.Chains;

            return DistanceUnitType.Meters; // just in case
        }

        private void chainsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ShowResults(DistanceUnitType.Chains);
        }

        private void feetRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ShowResults(DistanceUnitType.Feet);
        }

        private void metersRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ShowResults(DistanceUnitType.Meters);
        }

        void ShowResults (DistanceUnitType dunit)
        {
            DistanceUnit unit = EditingController.GetUnits(dunit);

            lengthTextBox.Text = unit.Format(m_Length);
            totalEnteredTextBox.Text = unit.Format(m_Entered);
            lengthLeftTextBox.Text = unit.Format(m_Length - m_Entered);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // Ensure the distances are all worked out (they should be).
            distancesTextBox_TextChanged(null, null);

            // Confirm that the entered length is exact. If not,
            // ask the user to confirm.
            double lengthLeft;
            if (!Double.TryParse(lengthLeftTextBox.Text, out lengthLeft))
            {
                MessageBox.Show("Cannot decode length left text box");
                return;
            }

            if (Math.Abs(lengthLeft) > 0.00001)
            {
                string msg = "WARNING: The entered length does not add up" + System.Environment.NewLine +
                             "to the length of the leg. The distances you" + System.Environment.NewLine +
                             "have entered will be adjusted to fit." + System.Environment.NewLine +
                             System.Environment.NewLine +
                             "Is this really what you want to do?";

                if (MessageBox.Show(msg, "Lengths don't add up", MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Hand) != DialogResult.Yes)
                    return;
            }

            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Parses the distances list box.
        /// </summary>
        /// <returns>The total distance entered, in meters.</returns>
        double GetDistances()
        {
            m_Distances = null;

            try
            {
                string entryString = GetEntryString();
                DistanceUnit defaultEntryUnit = EditingController.Current.EntryUnit;
                m_Distances = LineSubdivisionFace.GetDistances(entryString, defaultEntryUnit,
                                                                            false);

                // Return the total length (in meters)
                double result = 0.0;
                foreach (Distance d in m_Distances)
                    result += d.Meters;

                return result;
            }

            catch { }
            return 0.0;
        }

        /// <summary>
        /// Obtains a string holding the currently entered distances.
        /// </summary>
        /// <returns>The entered distances (each seperated by a space character).</returns>
        string GetEntryString()
        {
            StringBuilder result = new StringBuilder(1000);

            foreach (string t in distancesTextBox.Lines)
            {
                string s = t.Trim();

                // Skip empty lines
                if (s.Length == 0)
                    continue;

                // The user may have entered the *, about to append a repeat count. In that
                // case, ignore the trailing *.
                if (s.EndsWith("*"))
                    s = s.Substring(0, t.Length - 1);

                if (result.Length > 0)
                    result.Append(" ");

                result.Append(s);
            }

            return result.ToString();
        }

        /// <summary>
        /// The distances involved (could be null).
        /// </summary>
        internal Distance[] Distances
        {
            get { return m_Distances; }
        }
    }
}
