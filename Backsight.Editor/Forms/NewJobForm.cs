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
using Backsight.Environment;
using System.IO;
using System.Collections.Generic;

namespace Backsight.Editor.Forms
{
    /// <summary>
    /// Dialog for defining a new editing job. The work of actually storing the new
    /// job in the database is done elsewhere.
    /// </summary>
    public partial class NewJobForm : Form
    {
        #region Class data

        /// <summary>
        /// The dialog displaying this one (if any)
        /// </summary>
        readonly GetJobForm m_Parent;

        /// <summary>
        /// All defined jobs
        /// </summary>
        readonly Job[] m_AllJobs;

        /// <summary>
        /// The new job file (created when user clicks on OK)
        /// </summary>
        JobFile m_NewJobFile;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>NewJobForm</c>
        /// </summary>
        /// <param name="parent">The dialog displaying this one (null if created some other way)</param>
        /// <exception cref="ArgumentNullException">If the specified parent is null</exception>
        public NewJobForm(GetJobForm parent)
        {
            InitializeComponent();
            m_Parent = parent;

            if (m_Parent==null)
                m_AllJobs = Job.FindAll();
            else
                m_AllJobs = m_Parent.Jobs;
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

        private void okButton_Click(object sender, EventArgs e)
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
                MessageBox.Show("You must specify a name for the job");
                return;
            }

            // Get the parent dialog to do the rest
            m_NewJobFile = CreateJob(jobName, zone, layer);
            if (m_NewJobFile!=null)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }


        /// <summary>
        /// The new job file (created when user clicks on OK)
        /// </summary>
        internal JobFile NewJobFile
        {
            get { return m_NewJobFile; }
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
    }
}