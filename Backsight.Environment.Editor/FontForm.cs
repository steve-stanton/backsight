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

namespace Backsight.Environment.Editor
{
    public partial class FontForm : Form
    {
        private readonly IEditFont m_Edit;

        internal FontForm() : this(null)
        {
        }

        internal FontForm(IEditFont edit)
        {
            InitializeComponent();

            m_Edit = edit;
            if (m_Edit == null)
            {
                IEnvironmentFactory f = EnvironmentContainer.Factory;
                m_Edit = f.CreateFont();
            }

            m_Edit.BeginEdit();
        }

        private void FontForm_Shown(object sender, EventArgs e)
        {
            FontFamily[] fams = FontFamily.Families;
            fontFamilyComboBox.DataSource = fams;
            fontFamilyComboBox.DisplayMember = "Name";

            fontFamilyComboBox.SelectedItem = null;
            fontStyleComboBox.SelectedItem = null;
            sizeComboBox.SelectedItem = null;

            if (!String.IsNullOrEmpty(m_Edit.TypeFace))
            {
                fontFamilyComboBox.SelectedItem = Array.Find<FontFamily>(fams,
                    delegate(FontFamily ff) { return ff.Name==m_Edit.TypeFace; });

                bool isBold = (m_Edit.Modifiers & FontStyle.Bold)!=0;
                bool isItalic = (m_Edit.Modifiers & FontStyle.Italic)!=0;

                if (isBold && isItalic)
                    fontStyleComboBox.SelectedItem = "Bold Italic";
                else if (isBold)
                    fontStyleComboBox.SelectedItem = "Bold";
                else if (isItalic)
                    fontStyleComboBox.SelectedItem = "Italic";
                else
                    fontStyleComboBox.SelectedItem = "Regular";

                string s = m_Edit.PointSize.ToString();
                if (sizeComboBox.Items.Contains(s))
                    sizeComboBox.SelectedItem = s;
                else
                {
                    sizeComboBox.SelectedItem = null;
                    sizeComboBox.Text = s;
                }
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            m_Edit.CancelEdit();
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // Confirm font is defined
            string familyName = fontFamilyComboBox.Text;
            if (String.IsNullOrEmpty(familyName))
            {
                MessageBox.Show("You must specify a font");
                fontFamilyComboBox.Focus();
                return;
            }

            // If the style or size is unspecified, use the first item
            string fontStyle = fontStyleComboBox.Text;
            if (String.IsNullOrEmpty(fontStyle))
                fontStyle = fontStyleComboBox.Items[0].ToString();

            string sizeString = sizeComboBox.Text;
            if (String.IsNullOrEmpty(sizeString))
                sizeString = sizeComboBox.Items[0].ToString();

            // Confirm the text for the font family agrees with one of the
            // installed fonts
            FontFamily[] fams = FontFamily.Families;
            FontFamily fam = Array.Find<FontFamily>(fams, delegate(FontFamily ff)
                                { return String.Compare(familyName, ff.Name, true)==0; });
            if (fam == null)
            {
                MessageBox.Show("Cannot locate entered font name");
                fontFamilyComboBox.Focus();
                return;
            }

            float size;
            if (!Single.TryParse(sizeString, out size))
            {
                MessageBox.Show("Cannot parse font size");
                sizeComboBox.Focus();
                return;
            }

            m_Edit.TypeFace = fam.Name;

            string fs = fontStyleComboBox.Text;
            if (fs == "Regular")
                m_Edit.Modifiers = FontStyle.Regular;
            else if (fs == "Italic")
                m_Edit.Modifiers = FontStyle.Italic;
            else if (fs == "Bold")
                m_Edit.Modifiers = FontStyle.Bold;
            else if (fs == "Bold Italic")
                m_Edit.Modifiers = (FontStyle.Bold | FontStyle.Italic);

            m_Edit.PointSize = size;

            m_Edit.FinishEdit();
            this.DialogResult = DialogResult.OK;
            Close();
        }
    }
}