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
    /// Dialog for setting editing preferences
    /// </summary>
    /// <remarks>
    /// Each page was formerly implemented by a seperate class:
    /// CdPrefPoint, CdPrefLabel, CdUnits, CdLineAnno, and CdPrefSymbology
    /// </remarks>
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

            ShowPointsPage(cmm);
            ShowLabelsPage(cmm);
            ShowUnitsPage(cmm);
            ShowLineAnnotationPage(cmm);
            ShowSymbologyPage(cmm);
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

        /*
void CdPrefLabel::OnFont() 
{
	CFontDialog dial;
	dial.m_cf.Flags |= CF_TTONLY;			// Only true-type fonts
	dial.m_cf.Flags |= CF_FORCEFONTEXIST;	// Font must exist(!)
	dial.m_cf.Flags |= CF_SCALABLEONLY;		//
	dial.m_cf.Flags &= (~CF_EFFECTS);		// No effects or colour
//	dial.m_cf.Flags |= CF_WYSIWYG;			// Same on printer

//	Check if there is a default font already. If so, make
//	that display by default.

	LOGFONT LogFont;
	CeMap* pMap = CeMap::GetpMap();
	CeFont* pFont = pMap->GetpFont();
	if ( pFont ) {

//		The height that gets defined relates to the
//		nominal scale, which could be quite a bit different
//		from the current draw scale. I tried changing this
//		so that CFontDialog would initialize with the right
//		point size, but MFC appears to select a point size
//		that is double what I expect. To avoid the perception
//		of a bug, lets just avoid the default on size.

		pFont->SetLogFont(LogFont);
		LogFont.lfHeight = 0;

		dial.m_cf.lpLogFont = &LogFont;
		dial.m_cf.Flags |= CF_INITTOLOGFONTSTRUCT;
	}

	if ( dial.DoModal()==IDOK ) {

//		Define a font with the selected characteristics.
		UINT4 ptsize = UINT4(dial.GetSize());
		CeFont font(dial.GetFaceName(),ptsize);
		if ( dial.IsBold() ) font.SetBold();
		if ( dial.IsItalic() ) font.SetItalic();
		if ( dial.IsUnderline() ) font.SetUnderline();

//		CHARS str[132];
//		sprintf ( str, "Point size=%lf Height=%lf",
//			ptsize, ht );
//		AfxMessageBox(str);

//		Ensure the map has such a font and make it
//		the default.
		pMap->SetDefaultFont(font);
		this->ShowFont();

	}

} // end of OnFont
         */

        private void okButton_Click(object sender, EventArgs e)
        {
            CadastralMapModel cmm = (CadastralMapModel)SpatialController.Current.MapModel;

            if (SavePointsPage(cmm) &&
                SaveLabelsPage(cmm) &&
                SaveUnitsPage(cmm) &&
                SaveLineAnnotationPage(cmm) &&
                SaveSymbologyPage(cmm))
            {
                this.DialogResult = DialogResult.OK;
                Close();
            }
        }

        void ShowPointsPage(CadastralMapModel cmm)
        {
            pointScaleTextBox.Text = String.Format("{0:F0}", cmm.ShowPointScale);
            pointSizeTextBox.Text = String.Format("{0:F2}", cmm.PointHeight.Meters);
            showIntersectionsCheckBox.Checked = cmm.AreIntersectionsDrawn;
        }

        bool SavePointsPage(CadastralMapModel cmm)
        {
            // Point threshold scale

            double pointScale;
            if (!Double.TryParse(pointScaleTextBox.Text, out pointScale))
            {
                tabControl.SelectedTab = PointsTabPage;
                pointScaleTextBox.Focus();
                MessageBox.Show("Cannot parse threshold scale for points");
                return false;
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
                return false;
            }

            if (Math.Abs(pointSize - cmm.PointHeight.Meters) > 0.001)
                cmm.PointHeight = new Length(pointSize);

            // Should intersections be drawn?
            cmm.AreIntersectionsDrawn = showIntersectionsCheckBox.Checked;
            return true;
        }

        void ShowLabelsPage(CadastralMapModel cmm)
        {
            labelScaleTextBox.Text = String.Format("{0:F0}", cmm.ShowLabelScale);
            textRotationAngleLabel.Text = RadianValue.AsShortString(cmm.DefaultTextRotation);
            nominalScaleTextBox.Text = cmm.NominalMapScale.ToString();
            ShowFont();
        }

        bool SaveLabelsPage(CadastralMapModel cmm)
        {
            // Label threshold scale

            double labelScale;
            if (!Double.TryParse(labelScaleTextBox.Text, out labelScale))
            {
                tabControl.SelectedTab = LabelsTabPage;
                labelScaleTextBox.Focus();
                MessageBox.Show("Cannot parse threshold scale for text labels");
                return false;
            }

            if (Math.Abs(labelScale - cmm.ShowLabelScale) > 0.001)
                cmm.ShowLabelScale = labelScale;

            // Rotation angle (can't be changed via this dialog)

            // Nominal scale
            int nominalScale;
            if (!Int32.TryParse(nominalScaleTextBox.Text, out nominalScale) || nominalScale<0)
            {
                tabControl.SelectedTab = LabelsTabPage;
                nominalScaleTextBox.Focus();
                MessageBox.Show("Nominal scale for text labels is not valid");
                return false;
            }

            if (nominalScale != cmm.NominalMapScale)
                cmm.NominalMapScale = (uint)nominalScale;

            // TODO: Font

            return true;
        }

        void ShowUnitsPage(CadastralMapModel cmm)
        {
            SetEntryUnit(cmm.EntryUnit);
            SetDisplayUnit(cmm.DisplayUnit);
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

        bool SaveUnitsPage(CadastralMapModel cmm)
        {
            if (enterMetersRadioButton.Checked)
                cmm.EntryUnitType = DistanceUnitType.Meters;
            else if (enterFeetRadioButton.Checked)
                cmm.EntryUnitType = DistanceUnitType.Feet;
            else if (enterChainsRadioButton.Checked)
                cmm.EntryUnitType = DistanceUnitType.Chains;

            if (displayMetersRadioButton.Checked)
                cmm.DisplayUnitType = DistanceUnitType.Meters;
            else if (displayFeetRadioButton.Checked)
                cmm.DisplayUnitType = DistanceUnitType.Feet;
            else if (displayChainsRadioButton.Checked)
                cmm.DisplayUnitType = DistanceUnitType.Chains;
            else if (displayAsEnteredRadioButton.Checked)
                cmm.DisplayUnitType = DistanceUnitType.AsEntered;

            return true;
        }

        void ShowLineAnnotationPage(CadastralMapModel cmm)
        {
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
        }

        bool SaveLineAnnotationPage(CadastralMapModel cmm)
        {
            // Annotation scale threshold

            double showScale;
            if (!Double.TryParse(annoScaleTextBox.Text, out showScale))
            {
                tabControl.SelectedTab = LineAnnotationTabPage;
                annoScaleTextBox.Focus();
                MessageBox.Show("Cannot parse scale threshold for line annotations");
                return false;
            }

            if (Math.Abs(showScale - cmm.Annotation.ShowScale) > 0.001)
                cmm.Annotation.ShowScale = showScale;

            // Annotation height

            double height;
            if (!Double.TryParse(annoHeightTextBox.Text, out height))
            {
                tabControl.SelectedTab = LineAnnotationTabPage;
                annoHeightTextBox.Focus();
                MessageBox.Show("Cannot parse height for line annotations");
                return false;
            }

            if (Math.Abs(height - cmm.Annotation.Height) > 0.001)
                cmm.Annotation.Height = height;

            // What needs to be shown...

            LineAnnotationStyle anno = cmm.Annotation;
            anno.ShowAdjustedLengths = false;
            anno.ShowObservedLengths = false;

            if (annoAdjustedLengthsRadioButton.Checked)
                anno.ShowAdjustedLengths = true;
            else if (annoObservedLengthsRadioButton.Checked)
                anno.ShowObservedLengths = true;

            anno.ShowObservedAngles = observedAnglesCheckBox.Checked;

            return true;
        }

        void ShowSymbologyPage(CadastralMapModel cmm)
        {
            int symScale = GlobalUserSetting.ReadInt("SymbologyScale", 5000);
            symScaleTextBox.Text = String.Format("{0:F0}", symScale);
        }

        bool SaveSymbologyPage(CadastralMapModel cmm)
        {
            int symScale;
            if (!Int32.TryParse(symScaleTextBox.Text, out symScale) || symScale<0)
            {
                tabControl.SelectedTab = SymbologyTabPage;
                symScaleTextBox.Focus();
                MessageBox.Show("Threshold scale for symbology is not valid");
                return false;
            }

            GlobalUserSetting.WriteInt("SymbologyScale", symScale);
            return true;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}