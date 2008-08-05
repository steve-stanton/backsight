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
using System.Collections.Generic;

using Backsight.Editor.Database;
using Backsight.Environment;
using Backsight.Editor.Properties;

namespace Backsight.Editor.Forms
{
    /// <summary>
    /// Dialog for defining a new editing job (the entry in the database, plus
    /// the job file). The work of actually storing the new job in the database is done
    /// as part of the <see cref="GetJobForm"/> class.
    /// </summary>
    public partial class NewJobForm : Form
    {
        #region Class data

        /// <summary>
        /// The dialog displaying this one (if any)
        /// </summary>
        readonly GetJobForm m_Parent;

        /// <summary>
        /// The new job file (created when user clicks on OK)
        /// </summary>
        JobFile m_NewJob;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>NewJobForm</c>
        /// </summary>
        /// <param name="parent">The dialog displaying this one (not null)</param>
        /// <exception cref="ArgumentNullException">If the specified parent is null</exception>
        public NewJobForm(GetJobForm parent)
        {
            InitializeComponent();
            m_Parent = parent;
            m_NewJob = null;

            if (m_Parent==null)
                throw new ArgumentNullException();
        }

        #endregion

        private void NewJobForm_Shown(object sender, EventArgs e)
        {
            IEnvironmentContainer ec = EnvironmentContainer.Current;

            // Load all defined zones
            List<IZone> zones = new List<IZone>(ec.Zones);
            zones.Sort(delegate(IZone a, IZone b) { return a.Name.CompareTo(b.Name); });
            zoneComboBox.DataSource = zones;

            // Load all defined editing layers
            layerComboBox.DataSource = ec.Layers;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            // Grab the defined info. The zone and editing layer must
            // both be defined. And the job name of course.

            IZone zone = (zoneComboBox.SelectedItem as IZone);
            if (zone == null || zone.Id==0)
            {
                MessageBox.Show("You must specify a spatial zone for the job");
                return;
            }

            ILayer layer = (layerComboBox.SelectedItem as ILayer);
            if (layer == null || layer.Id==0)
            {
                MessageBox.Show("You must specify the editing layer for the job");
                return;
            }

            string jobName = jobNameTextBox.Text.Trim();
            if (jobName.Length == 0)
            {
                MessageBox.Show("You must specify a name for the job file");
                return;
            }

            // Get the parent dialog to do the rest
            m_NewJob = m_Parent.CreateJob(jobName, zone, layer);
            if (m_NewJob!=null)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }


        /// <summary>
        /// The new job (created when user clicks on Save)
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

        /// <summary>
        /// Creates a new job after validating the details
        /// </summary>
        /// <param name="jobName">The user-perceived name for the job (not null or blank)</param>
        /// <param name="zone">The spatial zone the job covers (not null)</param>
        /// <param name="layer">The (base) map layer for the job (not null)</param>
        /// <returns>The created job file (null on any validation or database insert error)</returns>
        /*
        JobFile CreateJob(string jobName, IZone zone, ILayer layer)
        {
            if (String.IsNullOrEmpty(jobName) || zone==null || layer==null)
                throw new ArgumentNullException();

            // Confirm the job name is unique
            Job job = Array.Find<Job>(m_AllJobs, delegate(Job j)
                { return String.Compare(j.Name, jobName, true)==0; });
            if (job != null)
            {
                if (m_Parent!=null)
                    m_Parent.SelectJob(job);
                MessageBox.Show("A job with that name already exists");
                return null;
            }

            // Confirm that there isn't another job that refers to the
            // same zone and editing layer
            job = Array.Find<Job>(m_AllJobs, delegate(Job j)
                { return (j.ZoneId==zone.Id && j.LayerId==layer.Id); });
            if (job != null)
            {
                if (m_Parent!=null)
                    m_Parent.SelectJob(job);
                string msg = String.Format("{0} already refers to the same zone and editing layer", job.Name);
                MessageBox.Show(msg);
                return null;
            }

            // Insert the new job into the database.
            // Don't bother including in m_AllJobs, since returning a valid job should cause
            // this dialog to close momentarily.
            job = Job.Insert(jobName, zone.Id, layer.Id);

            // Save a job file as well
            return EditingController.Current.SaveJobFile(job);
        }
         */

        private void zoneComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            layerComboBox.Focus();
        }

        private void layerComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            jobNameTextBox.Focus();
        }

        private void jobNameTextBox_Enter(object sender, EventArgs e)
        {
            if (jobNameTextBox.Text.Length == 0)
            {
                IZone zone = (zoneComboBox.SelectedItem as IZone);
                if (zone == null || zone.Id==0)
                    return;

                ILayer layer = (layerComboBox.SelectedItem as ILayer);
                if (layer == null || layer.Id==0)
                    return;

                string fileName = String.Format("{0}-{1}{2}", zone.Name, layer.Name, JobFileInfo.TYPE);
                string dirName;
                string lastMap = Settings.Default.LastMap;

                if (String.IsNullOrEmpty(lastMap))
                    dirName = Directory.GetCurrentDirectory();
                else
                    dirName = Path.GetDirectoryName(lastMap);

                jobNameTextBox.Text = Path.Combine(dirName, fileName);
            }
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog dial = new SaveFileDialog();
            dial.Title = "Save As";
            dial.DefaultExt = JobFileInfo.TYPE;
            dial.FileName = jobNameTextBox.Text;
            dial.Filter = "Cadastral Editor files (*.cedx)|*.cedx|All files (*)|*";

            if (!String.IsNullOrEmpty(dial.FileName))
                dial.InitialDirectory = Path.GetDirectoryName(dial.FileName);

            if (dial.ShowDialog() == DialogResult.OK)
                jobNameTextBox.Text = dial.FileName;

            dial.Dispose();
        }
    }
}