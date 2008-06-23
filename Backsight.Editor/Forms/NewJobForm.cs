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
        /// The dialog displaying this one.
        /// </summary>
        readonly GetJobForm m_Parent;

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

            if (m_Parent==null)
                throw new ArgumentNullException();
        }

        #endregion

        private void NewJobForm_Shown(object sender, EventArgs e)
        {
            IEnvironmentContainer ec = EnvironmentContainer.Current;

            // Load all defined zones
            zoneComboBox.DataSource = ec.Zones;

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
            if (m_Parent.CreateJob(jobName, zone, layer)!=null)
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
    }
}