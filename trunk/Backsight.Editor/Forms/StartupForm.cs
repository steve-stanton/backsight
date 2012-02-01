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
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using Backsight.Data;
using Backsight.Editor.Properties;
using Backsight.Editor.FileStore;
//using Backsight.SqlServer;

namespace Backsight.Editor.Forms
{
    /// <summary>
    /// Dialog that will be shown to the user when the Cadastral Editor is launched directly
    /// (not by double-clicking on a cedx file).
    /// </summary>
    partial class StartupForm : Form
    {
        /// <summary>
        /// The dialog displaying this startup form.
        /// </summary>
        readonly MainForm m_Parent;

        internal StartupForm(MainForm parent)
        {
            InitializeComponent();
            m_Parent = parent;
        }

        private void StartupForm_Load(object sender, EventArgs e)
        {
            ShowDatabaseName();
            IJobContainer jc = new JobCollectionFolder();
            ProjectFile job = null;

            string lastJobName = Settings.Default.LastJobName;
            if (!String.IsNullOrEmpty(lastJobName))
                job = jc.OpenJob(lastJobName);

            if (job == null)
            {
                newProjectButton.BackColor = openLastButton.BackColor;
                this.AcceptButton = newProjectButton;
                openLastButton.BackColor = SystemColors.Control;
                openLastButton.Enabled = false;
            }
            else
            {
                openLastButton.Text = "&Open " + lastJobName;
                openLastButton.BackColor = SystemColors.Control;
                this.AcceptButton = openLastButton;
            }
        }

        void ShowDatabaseName()
        {
            string cs = LastDatabase.ConnectionString;
            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder(cs);
            lastDatabaseLabel.Text = String.Format(@"{0}\{1}", sb.DataSource, sb.InitialCatalog);
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void newJobButton_Click(object sender, EventArgs e)
        {
            try
            {
                Project p = EditingController.Current.OpenProject(null);
                if (p != null)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void openFileButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_Parent.OpenFile())
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Trace.Write(ex.StackTrace);
            }
        }

        private void databaseButton_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
            /*
            ConnectionForm dial = new ConnectionForm();
            if (dial.ShowDialog() == DialogResult.OK)
            {
                LastDatabase.ConnectionString = dial.ConnectionString;
                ShowDatabaseName();
            }

            dial.Dispose();
             */
        }

        private void openLastButton_Click(object sender, EventArgs e)
        {
            string lastProjectName = Settings.Default.LastProjectName;

            try
            {

                // Splash screen isn't shown here, so provide user with a visual cue
                openLastButton.Text = "Opening "+lastProjectName+" ...";
                openLastButton.BackColor = Color.Yellow;
                openLastButton.Refresh();

                m_Parent.OpenJob(lastProjectName);
                Close();
            }

            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Error reading {0} ({1})", lastProjectName, ex.Message));
                Trace.Write(ex.StackTrace);
                openLastButton.Enabled = false;
            }
        }
    }
}