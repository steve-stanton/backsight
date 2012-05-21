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
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Backsight.Data;
using Backsight.Editor.Properties;
using Backsight.Environment;

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
            // Pick up a canned environment from embedded resource file
            IEnvironmentContainer ec = new EnvironmentResource();
            EnvironmentContainer.Current = ec;

            ShowDatabaseName();

            // Determine whether a previously open project is still available
            bool canOpen = false;
            string lastProjectName = Settings.Default.LastProjectName;
            if (!String.IsNullOrEmpty(lastProjectName))
            {
                ProjectDatabase pd = new ProjectDatabase();
                canOpen = pd.CanOpen(lastProjectName);
            }

            if (canOpen)
            {
                openLastButton.Text = "&Open " + lastProjectName;
                this.AcceptButton = openLastButton;
            }
            else
            {
                newProjectButton.BackColor = openLastButton.BackColor;
                this.AcceptButton = newProjectButton;
                openLastButton.BackColor = SystemColors.Control;
                openLastButton.Enabled = false;
            }
        }

        void ShowDatabaseName()
        {
            string cs = LastDatabase.ConnectionString;
            DbConnectionStringBuilder sb = new DbConnectionStringBuilder();
            sb.ConnectionString = cs;
            object dataSource = sb["Data Source"];
            object initialCatalog = sb["Initial Catalog"];
            lastDatabaseLabel.Text = String.Format(@"{0}\{1}", dataSource, initialCatalog);

            //SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder(cs);
            //lastDatabaseLabel.Text = String.Format(@"{0}\{1}", sb.DataSource, sb.InitialCatalog);
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void newProjectButton_Click(object sender, EventArgs e)
        {
            try
            {
                Project p = null;

                using (NewProjectForm dial = new NewProjectForm())
                {
                    if (dial.ShowDialog() == DialogResult.OK)
                    {
                        string projectName = dial.NewProjectName;
                        m_Parent.OnProjectOpening();
                        p = new ProjectDatabase().OpenProject(projectName);
                    }
                }

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
            // The connection dialog is currently disabled while I figure out how to
            // remove an explicit dependency on SQLServer
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
                openLastButton.Text = "Opening " + lastProjectName + " ...";
                openLastButton.BackColor = Color.Yellow;
                openLastButton.Refresh();

                m_Parent.OpenProject(lastProjectName);
                Close();
            }

            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Error reading {0} ({1})", lastProjectName, ex.Message));
                Trace.Write(ex.StackTrace);
                openLastButton.BackColor = openFileButton.BackColor;
                openLastButton.Enabled = false;
            }
        }
    }
}