/// <remarks>
/// Copyright 2008 - Steve Stanton. This file is part of Backsight
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
    /// <written by="Steve Stanton" />
    /// <summary>
    /// Dialog for obtaining a feature ID from the user.
    /// </summary>
    partial class GetKeyForm : Form
    {
        #region Class data

        /// <summary>
        /// The key entered by the user (may be null)
        /// </summary>
        string m_Key;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>GetKeyForm</c> with the usual title bar.
        /// </summary>
        internal GetKeyForm()
        {
            InitializeComponent();

            m_Key = null;
        }

        /// <summary>
        /// Creates a new <c>GetKeyForm</c> with the specified title bar.
        /// </summary>
        /// <param name="title">The text for the title bar</param>
        internal GetKeyForm(string title)
            : this()
        {
            this.Text = title;
        }


        #endregion

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            string k = keyTextBox.Text.Trim();
            if (k.Length==0)
            {
                MessageBox.Show("You have not specified anything.");
                keyTextBox.Focus();
                return;
            }

            m_Key = k;
            this.DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// The key entered by the user (may be null)
        /// </summary>
        internal string Key
        {
            get { return m_Key; }
        }
    }
}