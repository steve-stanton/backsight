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
    public partial class PreferencesForm : Form
    {
        /// <summary>
        /// The scale at which points will start to draw.
        /// </summary>
        /*
        uint m_ShowScale;

        FLOAT8 m_Height;		// The height of point symbols, in
        // meters on the ground.
        LOGICAL m_ShowX;		// TRUE if intersections should be
        // displayed.
        */

        /// <summary>
        /// True if changes to preferences will impact draws.
        /// </summary>
        //private bool m_IsRedrawRequired = false;

        /// <summary>
        /// True if the screen needs to be erased prior to redraw (e.g. if points
        /// need to be drawn smaller than they were).
        /// </summary>
        //private bool m_IsEraseRequired = false;

        public PreferencesForm()
        {
            InitializeComponent();

            CadastralMapModel cmm = (CadastralMapModel)SpatialController.Current.MapModel;

            // Points tab
            pointScaleTextBox.Text = String.Format("{0:F0}", cmm.ShowPointScale);
            pointSizeTextBox.Text = String.Format("{0:F2}", cmm.PointHeight.Meters);
            showIntersectionsCheckBox.Checked = cmm.AreIntersectionsDrawn;

            // Labels tab
            labelScaleTextBox.Text = String.Format("{0:F0}", cmm.ShowLabelScale);
            textRotationAngleLabel.Text = RadianValue.AsShortString(cmm.DefaultTextRotation);
            nominalScaleTextBox.Text = cmm.NominalMapScale.ToString();
            ShowFont();

            // Units tab
            SetEntryUnit(cmm.EntryUnit);
            SetDisplayUnit(cmm.DisplayUnit);

            // Line annotation tab
            LineAnnotationStyle anno = cmm.Annotation;
            annoScaleTextBox.Text = String.Format("{0:F0}", anno.ShowScale);
            annoHeightTextBox.Text = String.Format("{0:F2}", anno.Height);

            if (anno.ShowAdjustedLengths)
                annoAdjustedLengthsRadioButton.Checked = true;
            else if (anno.ShowObservedLengths)
                annoObservedLengthsRadioButton.Checked = true;
            else
                annoNothingRadioButton.Checked = true;

            observedAnglesCheckBox.Checked = anno.ShowObservedAngles;

            // Symbology tab

        }

        void ShowFont()
        {
            /*
        CHARS str[128];

    //	Get the default font.
        CeMap* pMap = CeMap::GetpMap();
        const CeFont* const pFont = pMap->GetpFont();

    //	Show the full font name (if any).

        CListBox* pField = (CListBox*)GetDlgItem(IDC_FACENAME);
        pField->ResetContent();

        if ( pFont ) {

            sprintf ( str, "%s - %g point",
                        pFont->GetpName(),
                        pFont->GetPointSize() );
            if ( pFont->IsBold() ) strcat ( str, " Bold" );
            if ( pFont->IsItalic() ) strcat ( str, " Italic" );
            pField->AddString(str);

        }
        else
            pField->AddString("<not defined>");
    }
             */
        }

        void SetEntryUnit(DistanceUnit du)
        {
            RadioButton rb = GetEntryRadioButton(du.UnitType);
            if (rb!=null)
                rb.Checked = true;
        }

        void SetDisplayUnit(DistanceUnit du)
        {
            RadioButton rb = GetDisplayRadioButton(du.UnitType);
            if (rb!=null)
                rb.Checked = true;
        }

        RadioButton GetEntryRadioButton(DistanceUnitType unitType)
        {
            switch (unitType)
            {
                case DistanceUnitType.Meters:
                    return enterMetersRadioButton;
                case DistanceUnitType.Feet:
                    return enterFeetRadioButton;
                case DistanceUnitType.Chains:
                    return enterChainsRadioButton;
            }

            throw new ArgumentException();
        }

        RadioButton GetDisplayRadioButton(DistanceUnitType unitType)
        {
            switch (unitType)
            {
                case DistanceUnitType.Meters:
                    return displayMetersRadioButton;
                case DistanceUnitType.Feet:
                    return displayFeetRadioButton;
                case DistanceUnitType.Chains:
                    return displayChainsRadioButton;
                case DistanceUnitType.AsEntered:
                    return displayAsEnteredRadioButton;
            }

            throw new ArgumentException();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            CadastralMapModel cmm = (CadastralMapModel)SpatialController.Current.MapModel;

            // Point threshold scale

            double pointScale;
            if (!Double.TryParse(pointScaleTextBox.Text, out pointScale))
            {
                tabControl.SelectedTab = PointsTabPage;
                pointScaleTextBox.Focus();
                MessageBox.Show("Cannot parse threshold scale for points");
                return;
            }

            if (Math.Abs(pointScale - cmm.ShowPointScale) > 0.001)
            {
                cmm.ShowPointScale = pointScale;
            }

            // Point size

            double pointSize;
            if (!Double.TryParse(pointSizeTextBox.Text, out pointSize))
            {
                tabControl.SelectedTab = PointsTabPage;
                pointSizeTextBox.Focus();
                MessageBox.Show("Cannot parse point size");
                return;
            }

            if (Math.Abs(pointSize - cmm.PointHeight.Meters) > 0.001)
                cmm.PointHeight = new Length(pointSize);

            // Should intersections be drawn?
            cmm.AreIntersectionsDrawn = showIntersectionsCheckBox.Checked;

            // Label threshold scale

            double labelScale;
            if (!Double.TryParse(labelScaleTextBox.Text, out labelScale))
            {
                tabControl.SelectedTab = LabelsTabPage;
                labelScaleTextBox.Focus();
                MessageBox.Show("Cannot parse threshold scale for text labels");
                return;
            }

            if (Math.Abs(labelScale - cmm.ShowLabelScale) > 0.001)
                cmm.ShowLabelScale = labelScale;

            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
