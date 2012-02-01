// <remarks>
// Copyright 2008 - Steve Stanton. This file is part of Backsight
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
    /// Dialog that lets the user specify an an existing project.
    /// </summary>
    public partial class OpenProjectForm : Form
    {
        #region Class data

        /// <summary>
        /// The name of the selected project (null if nothing was selected)
        /// </summary>
        string m_ProjectName;

        /// <summary>
        /// The names of all defined projects
        /// </summary>
        string[] m_AllNames;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenProjectForm"/> class.
        /// </summary>
        public OpenProjectForm()
        {
            InitializeComponent();
            m_ProjectName = null;
            m_AllNames = null;
        }

        #endregion

        private void OpenProjectForm_Shown(object sender, EventArgs e)
        {
            m_AllNames = new ProjectDatabase().FindAllProjectNames();
            listBox.DataSource = m_AllNames;
        }

        private void listBox_DoubleClick(object sender, EventArgs e)
        {
            if (listBox.SelectedItem != null)
            {
                m_ProjectName = listBox.SelectedItem.ToString();
                this.DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// The name of the selected project (null if nothing was selected)
        /// </summary>
        internal string ProjectName
        {
            get { return m_ProjectName; }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (listBox.SelectedItem == null)
            {
                MessageBox.Show("You must first select an existing project");
            }
            else
            {
                m_ProjectName = listBox.SelectedItem.ToString();
                this.DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}