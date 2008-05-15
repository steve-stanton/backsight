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

using Backsight.Editor.Database;

namespace Backsight.Editor.Forms
{
    /// <summary>
    /// Dialog that lets the user indicate whether they want to open an existing job, create
    /// a new one, or exit.
    /// </summary>
    public partial class GetJobForm : Form
    {
        Job m_Job;

        public GetJobForm()
        {
            InitializeComponent();
            m_Job = null;
        }

        private void GetJobForm_Shown(object sender, EventArgs e)
        {
            Job[] jobs = Job.FindAll();
            listBox.DataSource = jobs;
        }

        private void listBox_DoubleClick(object sender, EventArgs e)
        {
            Job j = (Job)listBox.SelectedItem;
            if (j!=null)
                MessageBox.Show("Open "+j);
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            NewJobForm dial = new NewJobForm();
            DialogResult = dial.ShowDialog();
            if (DialogResult == DialogResult.OK)
                m_Job = dial.Job;

            dial.Dispose();

            if (m_Job!=null)
                Close();
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            Job j = (listBox.SelectedItem as Job);
            if (j==null)
            {
                MessageBox.Show("You must first select the job to open");
                return;
            }

            m_Job = j;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// The selected job (null if nothing was selected)
        /// </summary>
        internal Job Job
        {
            get { return m_Job; }
        }
    }
}