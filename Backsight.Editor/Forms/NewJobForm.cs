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
using Backsight.Editor.FileStore;
using Backsight.Environment;

namespace Backsight.Editor.Forms
{
    /// <summary>
    /// Dialog for defining a new editing job.
    /// </summary>
    public partial class NewJobForm : Form
    {
        #region Class data

        /// <summary>
        /// The container for jobs (not null).
        /// </summary>
        readonly IJobContainer m_Container;

        /// <summary>
        /// Information for the newly created job (defined when user clicks the OK button)
        /// </summary>
        JobFile m_NewJob;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NewJobForm"/> class that
        /// uses a job container implemented by <see cref="JobCollectionFolder"/>.
        /// </summary>
        internal NewJobForm()
            : this(new JobCollectionFolder())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NewJobForm"/> class.
        /// </summary>
        /// <param name="jobContainer">The container for jobs (not null).</param>
        /// <exception cref="ArgumentNullException">If the specified container is null.</exception>
        internal NewJobForm(IJobContainer jobContainer)
        {
            InitializeComponent();

            if (jobContainer == null)
                throw new ArgumentNullException();

            m_Container = jobContainer;
        }

        #endregion

        private void okButton_Click(object sender, EventArgs e)
        {
            string jobName = jobNameTextBox.Text.Trim();
            if (jobName.Length == 0)
            {
                MessageBox.Show("You must specify a name for the job file");
                return;
            }

            // Confirm that the name is unique
            string[] names = m_Container.FindAllJobNames();
            if (Array.Exists<string>(names, delegate(string s) { return (String.Compare(s, jobName, true) == 0); }))
            {
                MessageBox.Show("Job name has already been used.");
                return;
            }

            // Grab the layer the job is for
            ILayer layer = (layerComboBox.SelectedItem as ILayer);
            if (layer == null || layer.Id == 0)
            {
                MessageBox.Show("You must specify the editing layer for the job");
                return;
            }

            // Create the job
            m_NewJob = m_Container.CreateJob(jobName, layer);

            // Test creation of new-style project
            Project.CreateNewProject(jobName, layer, String.Empty);
            MessageBox.Show("ok");

            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// The new job (created when user clicks on OK)
        /// </summary>
        internal JobFile NewJob
        {
            get { return m_NewJob; }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void NewJobForm_Load(object sender, EventArgs e)
        {
            // Load the layers combo
            IEnvironmentContainer ec = EnvironmentContainer.Current;
            ILayer[] layers = EnvironmentContainer.Current.Layers;
            layerComboBox.Items.AddRange(layers);

            // Remove first item if it's blank
            if (layerComboBox.Items.Count > 0 && layerComboBox.Items[0].ToString().Length == 0)
                layerComboBox.Items.RemoveAt(0);

            layerComboBox.SelectedIndex = 0;
        }
    }
}