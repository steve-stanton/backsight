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

namespace Backsight.Editor.Forms
{
    /// <summary>
    /// Dialog for selecting the things that should be listed during a
    /// file check.
    /// </summary>
    public partial class FileCheckForm : Form
    {
        /// <summary>
        /// The selected check options
        /// </summary>
        CheckType m_Options;

        public FileCheckForm()
        {
            InitializeComponent();
            m_Options = CheckType.Null;
        }

        /// <summary>
        /// The selected check options
        /// </summary>
        internal CheckType Options
        {
            get { return m_Options; }
        }

        private void FileCheckForm_Shown(object sender, EventArgs e)
        {
            // Get default options from the registry.
            string regstr = GlobalUserSetting.Read("FileCheck");
            if (String.IsNullOrEmpty(regstr))
                regstr = CheckItem.GetAllCheckLetters();

            // Convert to option flags
            m_Options = CheckItem.GetOptions(regstr);

            // Set check marks beside all the selected options.
            foreach (char c in regstr)
            {
                CheckType check = CheckItem.GetOption(c);
                CheckBox cb = null;

                if (check == CheckType.SmallLine)
                    cb = smallLineCheckBox;
                else if (check == CheckType.Dangle)
                    cb = danglingCheckBox;
                else if (check == CheckType.Overlap)
                    cb = overlapCheckBox;
                else if (check == CheckType.Floating)
                    cb = floatingCheckBox;
                else if (check == CheckType.Bridge)
                    cb = bridgeCheckBox;
                else if (check == CheckType.SmallPolygon)
                    cb = smallPolygonCheckBox;
                else if (check == CheckType.NotEnclosed)
                    cb = notEnclosedCheckBox;
                else if (check == CheckType.NoLabel)
                    cb = noLabelCheckBox;
                else if (check == CheckType.NoPolygonForLabel)
                    cb = noPolygonForLabelCheckBox;
                else if (check == CheckType.NoAttributes)
                    cb = noAttributesCheckBox;
                else if (check == CheckType.MultiLabel)
                    cb = multiLabelCheckBox;

                if (cb != null)
                    cb.Checked = true;
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // Pick up the checked items.

            CheckType checks = CheckType.Null;

            if (smallLineCheckBox.Checked)
                checks |= CheckType.SmallLine;

            if (danglingCheckBox.Checked)
                checks |= CheckType.Dangle;

            if (overlapCheckBox.Checked)
                checks |= CheckType.Overlap;

            if (floatingCheckBox.Checked)
                checks |= CheckType.Floating;

            if (bridgeCheckBox.Checked)
                checks |= CheckType.Bridge;

            if (smallPolygonCheckBox.Checked)
                checks |= CheckType.SmallPolygon;

            if (notEnclosedCheckBox.Checked)
                checks |= CheckType.NotEnclosed;

            if (noLabelCheckBox.Checked)
                checks |= CheckType.NoLabel;

            if (noPolygonForLabelCheckBox.Checked)
                checks |= CheckType.NoPolygonForLabel;

            if (noAttributesCheckBox.Checked)
                checks |= CheckType.NoAttributes;

            if (multiLabelCheckBox.Checked)
                checks |= CheckType.MultiLabel;

            // Hold options in the registry.
            string regstr = CheckItem.GetCheckLetters(checks);
            GlobalUserSetting.Write("FileCheck", regstr);

            // Remember the selected options
            m_Options = checks;

            this.DialogResult = DialogResult.OK;
            Close();
        }
    }
}