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
using System.Data.SqlClient;
using System.IO;
using System.Drawing;

using Backsight.SqlServer;
using Backsight.Data;
using Backsight.Editor.Properties;
using System.Diagnostics;

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

            string lastFile = Settings.Default.LastMap;
            openLastButton.Enabled = File.Exists(lastFile);
            if (openLastButton.Enabled)
                openLastButton.Text = "&Open " + Path.GetFileName(lastFile);
            else
                openLastButton.BackColor = SystemColors.Control;
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
                EditingController.Current.OpenJob(null);
                if (CadastralMapModel.Current != null)
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
            }
        }

        private void databaseButton_Click(object sender, EventArgs e)
        {
            ConnectionForm dial = new ConnectionForm();
            if (dial.ShowDialog() == DialogResult.OK)
            {
                LastDatabase.ConnectionString = dial.ConnectionString;
                ShowDatabaseName();
            }

            dial.Dispose();
        }

        private void openLastButton_Click(object sender, EventArgs e)
        {
            string lastFile = Settings.Default.LastMap;

            try
            {
                // Splash screen isn't shown here, so provide user with a visual cue
                openLastButton.Text = "Opening "+Path.GetFileName(lastFile)+" ...";
                openLastButton.BackColor = Color.Yellow;
                openLastButton.Refresh();

                JobFile jf = new JobFile(lastFile);
                EditingController.Current.OpenJob(jf);
                Close();
            }

            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Error reading {0} ({1})", lastFile, ex.Message));
                Trace.Write(ex.StackTrace);
                openLastButton.Enabled = false;
            }
        }
    }
}