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
using System.Collections.Generic;
using System.Windows.Forms;

using Backsight.Editor.Database;
using Backsight.Environment;
using Backsight.Editor.FileStore;

namespace Backsight.Editor.Forms
{
    /// <summary>
    /// Dialog that lets the user specify an an existing job.
    /// </summary>
    public partial class GetJobForm : Form
    {
        #region Class data

        /// <summary>
        /// The job info (null if nothing was selected)
        /// </summary>
        IJobInfo m_Job;

        /// <summary>
        /// The names of all defined jobs
        /// </summary>
        string[] m_AllJobs;

        #endregion

        #region Constructors

        public GetJobForm()
        {
            InitializeComponent();
            m_Job = null;
            m_AllJobs = null;
        }

        #endregion

        private void GetJobForm_Shown(object sender, EventArgs e)
        {
            m_AllJobs = new JobCollectionFolder().FindAllJobNames();
            listBox.DataSource = m_AllJobs;
        }

        private void listBox_DoubleClick(object sender, EventArgs e)
        {
            if (listBox.SelectedItem != null)
                OpenJobAndClose(listBox.SelectedItem.ToString());
        }

        void OpenJobAndClose(string jobName)
        {
            m_Job = new JobCollectionFolder().OpenJob(jobName);

            if (m_Job != null)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Info for the new job (created when user clicks on Save button)
        /// </summary>
        internal IJobInfo Job
        {
            get { return m_Job; }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (listBox.SelectedItem == null)
                MessageBox.Show("You must first select an existing job");
            else
                OpenJobAndClose(listBox.SelectedItem.ToString());
        }
    }
}