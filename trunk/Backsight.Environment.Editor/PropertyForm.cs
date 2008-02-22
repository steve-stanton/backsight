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
    /// <summary>
    /// Dialog for displaying and editing an individual Backsight property.
    /// </summary>
    public partial class PropertyForm : Form
    {
        #region Class data

        /// <summary>
        /// The name of the property
        /// </summary>
        string m_Name;

        /// <summary>
        /// The value for the property
        /// </summary>
        string m_Value;

        #endregion

        #region Constructors

        public PropertyForm(string propertyName, string propertyValue)
        {
            InitializeComponent();

            m_Name = propertyName;
            m_Value = propertyValue;
        }

        #endregion

        private void PropertyForm_Shown(object sender, EventArgs e)
        {
            nameTextBox.Text = m_Name;
            valueTextBox.Text = m_Value;

            nameTextBox.Enabled = (m_Name.Length==0);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            string name = nameTextBox.Text.Trim();
            if (name.Length==0)
            {
                MessageBox.Show("Property name must be defined");
                return;
            }

            m_Name = name;
            m_Value = valueTextBox.Text.Trim();

            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dial = new OpenFileDialog();
            if (dial.ShowDialog() == DialogResult.OK)
                valueTextBox.Text = dial.FileName;
            dial.Dispose();
        }

        internal string PropertyName
        {
            get { return m_Name; }
        }

        internal string PropertyValue
        {
            get { return m_Value; }
        }
    }
}