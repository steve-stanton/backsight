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

using Backsight.Environment;

namespace Backsight.Editor.Forms
{
    /// <summary>
    /// Dialog for defining a new editing project. If the user exits with OK, the project
    /// will get created, but it is not open (use <see cref="ProjectDatabase.OpenProject"/>
    /// to open the new project after closing this dialog).
    /// </summary>
    public partial class NewProjectForm : Form
    {
        #region Class data

        /// <summary>
        /// The container for projects (not null).
        /// </summary>
        readonly ProjectDatabase m_Container;

        /// <summary>
        /// The user-perceived name of the newly created project (defined when user clicks the OK button)
        /// </summary>
        string m_NewProjectName;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NewProjectForm"/> class that
        /// uses a project container implemented by <see cref="ProjectDatabase"/>.
        /// </summary>
        internal NewProjectForm()
            : this(new ProjectDatabase())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NewProjectForm"/> class.
        /// </summary>
        /// <param name="pd">The container for the project (not null).</param>
        /// <exception cref="ArgumentNullException">If the specified container is null.</exception>
        internal NewProjectForm(ProjectDatabase pd)
        {
            InitializeComponent();

            if (pd == null)
                throw new ArgumentNullException();

            m_Container = pd;
        }

        #endregion

        private void NewProjectForm_Load(object sender, EventArgs e)
        {
            // Load all defined editing layers
            IEnvironmentContainer ec = EnvironmentContainer.Current;
            layerComboBox.DataSource = ec.Layers;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            string projectName = projectNameTextBox.Text.Trim();
            if (projectName.Length == 0)
            {
                MessageBox.Show("You must specify a name for the project");
                return;
            }

            // Confirm that the name is unique
            string[] names = m_Container.FindAllProjectNames();
            if (Array.Exists<string>(names, delegate(string s) { return (String.Compare(s, projectName, true) == 0); }))
            {
                MessageBox.Show("Project name has already been used.");
                return;
            }

            // Grab the layer the project is for
            ILayer layer = (layerComboBox.SelectedItem as ILayer);
            if (layer == null || layer.Id == 0)
            {
                MessageBox.Show("You must specify the editing layer for the project");
                return;
            }

            // Create the project

            try
            {
                m_Container.CreateProject(projectName, layer);
                //EditingController.Current.CreateProject(projectName, layer);
                m_NewProjectName = projectName;
                DialogResult = DialogResult.OK;
                Close();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// The user-perceived name of the newly created project (defined when user clicks the OK button).
        /// Null if the user cancelled from this dialog.
        /// </summary>
        internal string NewProjectName
        {
            get { return m_NewProjectName; }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}