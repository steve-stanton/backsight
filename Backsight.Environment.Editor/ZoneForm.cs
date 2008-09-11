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

namespace Backsight.Environment.Editor
{
    /// <summary>
    /// Dialog for working with Backsight "zone" (named areas of space)
    /// </summary>
    public partial class ZoneForm : Form
    {
        private readonly IEditZone m_Edit;

        #region Constructors

        internal ZoneForm()
            : this(null)
        {
        }

        internal ZoneForm(IEditZone edit)
        {
            InitializeComponent();

            m_Edit = edit;
            if (m_Edit == null)
            {
                IEnvironmentFactory f = EnvironmentContainer.Factory;
                m_Edit = f.CreateZone();
            }

            m_Edit.BeginEdit();
        }

        #endregion

        private void ZoneForm_Shown(object sender, EventArgs e)
        {
            nameTextBox.Text = m_Edit.Name;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            m_Edit.CancelEdit();
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (ValidateEdit())
            {
                m_Edit.FinishEdit();
                this.DialogResult = DialogResult.OK;
                Close();
            }
        }

        bool ValidateEdit()
        {
            string name = nameTextBox.Text.Trim();
            if (name.Length==0)
            {
                MessageBox.Show("A name must be supplied for the zone");
                nameTextBox.Focus();
                return false;
            }

            m_Edit.Name = name;

            return true;
        }
    }
}