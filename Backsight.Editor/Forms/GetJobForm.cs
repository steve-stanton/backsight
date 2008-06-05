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

        private void GetJobForm_Shown(object sender, EventArgs e)
        {
            m_AllJobs = Job.FindAll();
            listBox.DataSource = m_AllJobs;
        }

        private void listBox_DoubleClick(object sender, EventArgs e)
        {
            Job j = (Job)listBox.SelectedItem;
            if (j!=null)
                MessageBox.Show("Open "+j);
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            NewJobForm dial = new NewJobForm(this);
            DialogResult = dial.ShowDialog();
            dial.Dispose();

            if (DialogResult == DialogResult.OK)
                Close();
        }

        /// <summary>
        /// Callback from <see cref="NewJobForm"/> that first validates the details for a new
        /// job, then records it in the database.
        /// </summary>
        /// <param name="jobName">The user-perceived name for the job (not null or blank)</param>
        /// <param name="zone">The spatial zone the job covers (not null)</param>
        /// <param name="layer">The (base) map layer for the job (not null)</param>
        /// <returns>The created job file (null on any validation or database insert error)</returns>
        internal JobFile CreateJob(string jobName, IZone zone, ILayer layer)
        {
            if (String.IsNullOrEmpty(jobName) || zone==null || layer==null)
                throw new ArgumentNullException();

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

            // Save a job file as well
            return SaveJobFile(job);
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            Job j = (listBox.SelectedItem as Job);
            if (j==null)
            {
                MessageBox.Show("You must first select the job to open");
                return;
            }

            if (SaveJobFile(j) != null)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        JobFile SaveJobFile(Job job)
        {
            m_JobFile = null;

            SaveFileDialog dial = new SaveFileDialog();
            dial.Title = "Save As";
            dial.DefaultExt = JobFileInfo.TYPE;
            dial.FileName = job.Name + JobFileInfo.TYPE;
            dial.Filter = "Cadastral Editor files (*.cedx)|*.cedx|All files (*)|*";

            string lastMap = Settings.Default.LastMap;
            if (!String.IsNullOrEmpty(lastMap))
                dial.InitialDirectory = Path.GetDirectoryName(lastMap);

            if (dial.ShowDialog() == DialogResult.OK)
            {
                JobFileInfo jfi = new JobFileInfo();
                jfi.ConnectionString = AdapterFactory.ConnectionString;
                jfi.JobId = job.JobId;
                m_JobFile = JobFile.SaveJobFile(dial.FileName, jfi);

                Settings.Default.LastMap = dial.FileName;
                Settings.Default.Save();
            }

            return m_JobFile;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// The selected job file (null if nothing was selected)
        /// </summary>
        internal JobFile JobFile
        {
            get { return m_JobFile; }
        }
    }
}