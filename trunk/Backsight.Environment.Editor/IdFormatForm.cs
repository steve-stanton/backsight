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

namespace Backsight.Environment.Editor
{
    public partial class IdFormatForm : Form
    {
        private readonly IIdGroup m_Group;

        // The number of characters in the IDs for the smallest and highest
        // IDs in the group
        private readonly int m_MinWidth;
        private readonly int m_MaxWidth;

        private bool m_HasCheckDigit;
        private int m_NumWidth;
        private string m_Lead;
        private string m_Trail;
        private bool m_IsKeepZeroes;


        internal IdFormatForm(IIdGroup group)
        {
            InitializeComponent();
            m_Group = group;
            m_HasCheckDigit = group.HasCheckDigit;

            // Get the number of characters in the numeric portion
            // of keys formed for the specified group
            m_MinWidth = group.LowestId.ToString().Length;
            m_MaxWidth = group.HighestId.ToString().Length;

            /*
            if (group.Id==0)
            {
                m_Lead = String.Empty;
                m_Trail = String.Empty;
                m_IsKeepZeroes = false;
                m_NumWidth = m_MaxWidth;
            }
            else
             */
                ParseFormat(group.KeyFormat);
        }

        internal bool HasCheckDigit { get { return m_HasCheckDigit; } }
        internal string KeyFormat
        {
            get
            {
                string res = m_Lead;
                if ( m_NumWidth > 0 )
                {
                    if ( m_IsKeepZeroes )
                        res += ("{0:D" + m_NumWidth+ "}");
                    else
                        res += "{0}";
                }
                res += m_Trail;
                return res;
            }
        }

        private void IdFormatForm_Load(object sender, EventArgs e)
        {
            // Display info about the group
            this.Text = String.Format("{0} (range {1}-{2})", m_Group.Name, m_Group.LowestId, m_Group.HighestId);

            // Display any leader characters
            leadingCharsTextBox.Text = m_Lead;

            // Display any trailing characters
            trailingCharsTextBox.Text = m_Trail;

            // Display the width of the numeric portion
            numDigitTextBox.Text = m_NumWidth.ToString();

            // Disable the "keep zeroes" button if the width of the numeric
            // portion is less than or the same as the width of the smallest
            // ID in the range.
            if (m_NumWidth <= m_MinWidth)
            {
                keepLeadingZeroesCheckBox.Enabled = false;
                m_IsKeepZeroes = false; // just to be sure
            }

            // Should leading zeroes be preserved.
            keepLeadingZeroesCheckBox.Checked = m_IsKeepZeroes;

            // Does the group have a check digit?
            checkDigitCheckBox.Checked = m_HasCheckDigit;
        }

        private void checkDigitCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            m_HasCheckDigit = checkDigitCheckBox.Checked;

        }

        private void keepLeadingZeroesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            m_IsKeepZeroes = keepLeadingZeroesCheckBox.Checked;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // Confirm that the user has not typed in a numeric width that is too small
            try { m_NumWidth = Int32.Parse(numDigitTextBox.Text); }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            if (m_NumWidth < m_MaxWidth)
            {
                MessageBox.Show("The width of the numeric portion is too small.");
                numDigitTextBox.Focus();
                return;
            }

            // Disallow excessive length
            if (m_NumWidth > 20)
            {
                MessageBox.Show("The width of the numeric portion is excessive.");
                numDigitTextBox.Focus();
                return;

            }

            // Pick up any leading and trailing characters
            m_Lead = leadingCharsTextBox.Text.Trim();
            m_Trail = trailingCharsTextBox.Text.Trim();

            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        void ParseFormat(string format)
        {
            m_IsKeepZeroes = false;
            m_Lead = String.Empty;
            m_Trail = String.Empty;
            m_NumWidth = 0;

            if (String.IsNullOrEmpty(format)) // Shouldn't really be the case
                format = "{0}";

	        // Use the key format string to generate a key for the
	        // min-max values (this excludes any check digit).
            string minstr = String.Format(format, m_Group.LowestId);
            string maxstr = String.Format(format, m_Group.HighestId);
            int nc = maxstr.Length;

	        // Locate the first numeric digit (should be the same position
	        // in both the min and max strings).
            int iDigit = -1;
            int i;
            for ( i=0; i<nc && iDigit<0; i++ )
            {
                if ( Char.IsDigit(maxstr,i) ) iDigit = i;
            }

            // If the digit in the min-string is a zero, leading zeroes
	        // are preserved.
            if ( iDigit>=0 && minstr[iDigit] == '0' )
                m_IsKeepZeroes = true;

            // Strip the lead characters off the max string
            if ( iDigit > 0 )
            {
                m_Lead = maxstr.Substring(0,iDigit);
                maxstr = maxstr.Substring(iDigit);
                nc = maxstr.Length;
            }

            // Locate the last numeric digit
            for ( i=0, iDigit=-1; i<nc && iDigit<0; i++)
            {
                if ( !Char.IsDigit(maxstr,i) ) iDigit = i;
            }

            // Strip the trailing characters off
            if ( iDigit > 0 )
            {
                string prev = maxstr.Substring(0,iDigit);
                m_Trail = maxstr.Substring(iDigit);
                maxstr = prev;
                nc = maxstr.Length;
            }

        	// Remember the number of digits we have left (it MIGHT be 0).
	        m_NumWidth = nc;
        }

        private void numDigitTextBox_TextChanged(object sender, EventArgs e)
        {
            int n = 0;
            try { n = Int32.Parse(numDigitTextBox.Text); }
            catch { }
            keepLeadingZeroesCheckBox.Enabled = (n > m_MinWidth);
            if (!keepLeadingZeroesCheckBox.Enabled)
                keepLeadingZeroesCheckBox.Checked = false;
        }
    }
}
