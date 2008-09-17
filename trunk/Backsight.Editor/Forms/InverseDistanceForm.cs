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

namespace Backsight.Editor.Forms
{
    partial class InverseDistanceForm : InverseForm
    {
        #region Class data

        /// <summary>
        /// The first point for the distance calculation.
        /// </summary>
        PointFeature m_Point1;

        /// <summary>
        /// The second point for the distance calculation.
        /// </summary>
        PointFeature m_Point2;

        #endregion

        internal InverseDistanceForm()
        {
            InitializeComponent();
            color1Button.BackColor = InverseColors[0];
            color2Button.BackColor = InverseColors[1];

            m_Point1 = m_Point2 = null;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        internal override void InitializeUnits(DistanceUnit units)
        {
            if (units.UnitType == DistanceUnitType.Feet)
                fRadioButton.Checked = true;
            else if (units.UnitType == DistanceUnitType.Chains)
                cRadioButton.Checked = true;
            else
                mRadioButton.Checked = true;
        }

        private void mRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (mRadioButton.Checked)
            {
                base.SetMeters();
                ShowResult();
            }
        }

        private void fRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (fRadioButton.Checked)
            {
                base.SetFeet();
                ShowResult();
            }
        }

        private void cRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (cRadioButton.Checked)
            {
                base.SetChains();
                ShowResult();
            }
        }

        internal virtual void ShowResult()
        {
            // If we have two points, get the distance between them,
            // format the result, and display it.
            if (m_Point1!=null && m_Point2!=null)
            {
                // Get the distance on the mapping plane.
                double metric = Geom.Distance(m_Point1, m_Point2);
                distanceTextBox.Text = Format(metric, m_Point1, m_Point2);
            }
        }

        protected PointFeature Point1
        {
            get { return m_Point1; }
        }

        protected PointFeature Point2
        {
            get { return m_Point2; }
        }

        internal override void Draw()
        {
            ISpatialDisplay display = EditingController.Current.ActiveDisplay;

            if (m_Point1!=null)
                m_Point1.Draw(display, InverseColors[0]);

            if (m_Point2!=null)
                m_Point2.Draw(display, InverseColors[1]);
        }

        internal override void OnSelectPoint(PointFeature point)
        {
            // Ignore undefined points.
            if (point==null)
                return;

            // If both points are already defined, shift back the 2nd point
            if (m_Point2!=null)
            {
                m_Point1 = m_Point2;
                m_Point2 = point;
            }

            // If we already know the first point, the new point
            // always goes into the 2nd slot.
            if (m_Point1!=null)
                m_Point2 = point;
            else
                m_Point1 = point;

            // Make sure the edit boxes reflect what we now have
            point1TextBox.Text = GetPointText(m_Point1);
            point2TextBox.Text = GetPointText(m_Point2);

            // Display the distance if we've got 2 points.
            ShowResult();
        }
    }
}

