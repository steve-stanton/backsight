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
    /// Inverse distance and angle calculator
    /// </summary>
    partial class InverseDistanceAngleForm : InverseForm
    {
        #region Class data

        /// <summary>
        /// The first point for the distance-angle calculation.
        /// </summary>
        PointFeature m_Point1;

        /// <summary>
        /// The second point for the distance-angle calculation.
        /// </summary>
        PointFeature m_Point2;

        /// <summary>
        /// The third point for the distance-angle calculation.
        /// </summary>
        PointFeature m_Point3;

        /// <summary>
        /// True for clockwise angle
        /// </summary>
        bool m_Clockwise;

        #endregion

        #region Constructors

        public InverseDistanceAngleForm()
        {
            InitializeComponent();
            color1Button.BackColor = InverseColors[0];
            color2Button.BackColor = InverseColors[1];
            color3Button.BackColor = InverseColors[2];

            m_Point1 = m_Point2 = m_Point3 = null;
            m_Clockwise = true;
        }

        #endregion

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

        private void cwRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (cwRadioButton.Checked)
            {
                m_Clockwise = true;
                ShowResult();
            }
        }

        private void ccwRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (ccwRadioButton.Checked)
            {
                m_Clockwise = false;
                ShowResult();
            }
        }

        internal override void Draw()
        {
            ISpatialDisplay display = CadastralEditController.Current.ActiveDisplay;

            if (m_Point1!=null)
                m_Point1.Draw(display, InverseColors[0]);

            if (m_Point2!=null)
                m_Point2.Draw(display, InverseColors[1]);

            if (m_Point3!=null)
                m_Point3.Draw(display, InverseColors[2]);
        }

        void ShowResult()
        {
            // If we have the first two points, get the distance between
            // them, format the result, and display it.
            if (m_Point1!=null && m_Point2!=null)
            {
                double metric = Geom.Distance(m_Point1, m_Point2);
                distance1TextBox.Text = Format(metric, m_Point1, m_Point2);
            }

            // Same for the second pair of points.
            if (m_Point2!=null && m_Point3!=null)
            {
                double metric = Geom.Distance(m_Point2, m_Point3);
                distance2TextBox.Text = Format(metric, m_Point2, m_Point3);
            }

            // If we have all 3 points, display the angle.
            if (m_Point1!=null && m_Point2!=null && m_Point3!=null)
            {
                // Get the clockwise angle.
                Turn reft = new Turn(m_Point2, m_Point1);
                double ang = reft.GetAngle(m_Point3).Radians;

                // Get the complement if we actually want it anti-clockwise.
                if (!m_Clockwise)
                    ang = Constants.PIMUL2 - ang;

                angleTextBox.Text = RadianValue.AsString(ang);
            }
        }

        internal override void OnSelectPoint(PointFeature point)
        {
            // Ignore undefined points.
            if (point==null)
                return;

            // If all 3 points are already defined, shift back the 2nd 
            // and 3rd points
            if (m_Point3!=null)
            {
                m_Point1 = m_Point2;
                m_Point2 = m_Point3;
            }

            // Stick the point into the appropriate slot (always the
            // last slot if the first 2 were previously defined).
            if (m_Point1==null)
                m_Point1 = point;
            else if (m_Point2==null)
                m_Point2 = point;
            else
                m_Point3 = point;

            // Make sure the edit boxes reflect what we now have
            point1TextBox.Text = GetPointText(m_Point1);
            point2TextBox.Text = GetPointText(m_Point2);
            point3TextBox.Text = GetPointText(m_Point3);

            // Display what results we can.
            ShowResult();
        }
    }
}