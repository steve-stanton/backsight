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
using System.IO;

using Backsight.Editor.Database;
using Backsight.Environment;
using Backsight.Data;
using Backsight.Editor.Properties;
using System.Collections.Generic;

namespace Backsight.Editor.Forms
{
    /// <summary>
    /// Dialog that lets the user indicate whether they want to open an existing job, create
    /// a new one, or exit.
    /// </summary>
    public partial class GetJobForm : Form
    {
        #region Class data

        /// <summary>
        /// The job file (null if nothing was selected)
        /// </summary>
        JobFile m_JobFile;

        /// <summary>
        /// All defined jobs
        /// </summary>
        Job[] m_AllJobs;

        #endregion

        #region Constructors

        public GetJobForm()
        {
            InitializeComponent();
            m_JobFile = null;
            m_AllJobs = null;
        }

        #endregion

        /// <summary>
        /// All defined jobs
        /// </summary>
        internal Job[] Jobs
        {
            get { return m_AllJobs; }
        }

        /// <summary>
        /// Selects a specific item in the job list
        /// </summary>
        /// <param name="job">The instance to select</param>
        internal void SelectJob(Job job)
        {
            listBox.SelectedItem = job;
        }

        private void GetJobForm_Shown(object sender, EventArgs e)
        {
            m_AllJobs = Job.FindAll();
            listBox.DataSource = m_AllJobs;
        }

        private void listBox_DoubleClick(object sender, EventArgs e)
        {
            Job j = (Job)listBox.SelectedItem;
            if (j!=null)
                SaveFileAndClose(j);
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            NewJobForm dial = new NewJobForm(this);
            dial.ShowDialog();
            dial.Dispose();
        }

        /// <summary>
        /// Callback from <see cref="NewJobForm"/> that first validates the details for a new
        /// job, then records it in the database.
        /// </summary>
        /// <param name="jobFileName">The user-perceived name for the job (not null or blank)</param>
        /// <param name="zone">The spatial zone the job covers (not null)</param>
        /// <param name="layer">The (base) map layer for the job (not null)</param>
        /// <returns>The created job data (null on any validation or database insert error)</returns>
        internal JobFile CreateJob(string jobFileName, IZone zone, ILayer layer)
        {
            if (String.IsNullOrEmpty(jobFileName) || zone==null || layer==null)
                throw new ArgumentNullException();

            string jobName = Path.GetFileNameWithoutExtension(jobFileName);

            // Confirm the job name is unique
            Job job = Array.Find<Job>(m_AllJobs, delegate(Job j)
                { return String.Compare(j.Name, jobName, true)==0; });
            if (job != null)
            {
                listBox.SelectedItem = job;
                MessageBox.Show("A job with that name already exists");
                return null;
            }

            // Confirm that there isn't another job that refers to the
            // same zone and editing layer
            job = Array.Find<Job>(m_AllJobs, delegate(Job j)
                { return (j.ZoneId==zone.Id && j.LayerId==layer.Id); });
            if (job != null)
            {
                listBox.SelectedItem = job;
                string msg = String.Format("{0} already refers to the same zone and editing layer", job.Name);
                MessageBox.Show(msg);
                return null;
            }

            // Insert the new job into the database.
            // Don't bother including in m_AllJobs, since returning a valid job should cause
            // this dialog to close momentarily.
            job = Job.Insert(jobName, zone.Id, layer.Id);

            // Include the new job in the list of all known jobs and select it
            List<Job> tmp = new List<Job>(m_AllJobs.Length+1);
            tmp.AddRange(m_AllJobs);
            tmp.Add(job);
            m_AllJobs = tmp.ToArray();
            listBox.DataSource = m_AllJobs;
            SelectJob(job);

            // Save the information in a job file
            JobFileInfo jfi = new JobFileInfo();
            jfi.ConnectionString = AdapterFactory.ConnectionString;
            jfi.JobId = job.JobId;
            JobFile jobFile = JobFile.SaveJobFile(jobFileName, jfi);

            Settings.Default.LastMap = jobFileName;
            Settings.Default.Save();

            return jobFile;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            Job j = (listBox.SelectedItem as Job);
            if (j==null)
            {
                MessageBox.Show("You must first select an editing job from the list");
                return;
            }

            SaveFileAndClose(j);
        }

        void SaveFileAndClose(Job job)
        {
            m_JobFile = EditingController.Current.SaveJobFile(job);
            if (m_JobFile != null)
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
        /// The new job file (created when user clicks on Save button)
        /// </summary>
        internal JobFile NewJobFile
        {
            get { return m_JobFile; }
        }
    }
}