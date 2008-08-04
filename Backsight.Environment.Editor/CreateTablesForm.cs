/// <remarks>
/// Copyright 2007 - Steve Stanton. This file is part of Backsight
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
using System.Reflection;
using System.IO;
using System.Data.SqlClient;

using Backsight.SqlServer;
using Backsight.Data;
using Backsight.Forms;

namespace Backsight.Environment.Editor
{
    /// <written by="Steve Stanton" on="20-SEP-2007" ticket="1"/>
    /// <summary>
    /// Dialog that gets displayed when the Environment Editor determines that Backsight
    /// system tables do not exist in a database.
    /// </summary>
    public partial class CreateTablesForm : Form, ILog, IBatchRunner
    {
        #region Class data

        /// <summary>
        /// The factory for creating database tables.
        /// </summary>
        readonly TableFactory m_Factory;

        /// <summary>
        /// Have tables been successfully created?
        /// </summary>
        bool m_IsCreated;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>CreateTablesForm</c> using the the supplied factory.
        /// </summary>
        /// <param name="tf">The factory that can be used to create database tables.
        /// This should be an instance where its <see cref="DoTablesExist"/> method
        /// returns <c>false</c>.
        /// </param>
        public CreateTablesForm(TableFactory tf)
        {
            InitializeComponent();
            m_Factory = tf;
            m_IsCreated = false;
        }

        #endregion

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            // The create button becomes the OK button if tables have been created successfully
            // (see finally block).
            if (m_IsCreated)
            {
                Close();
                return;
            }

            try
            {
                createButton.Enabled = false;
                cancelButton.Enabled = false;

                // Ensure relevant files are in the working directory
                string tabFile = GetResourceFile("CreateTables.sql"); 
                //string fkFile = GetResourceFile("AddForeignKeys.sql"); 

                // Form command line arguments
                string cs = m_Factory.ConnectionString;
                SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder(cs);
                string serverName = csb.DataSource;
                string dbName = csb.InitialCatalog;

                // Create command script
                //string cmdFile = Path.GetTempFileName();
                //cmdFile = Path.ChangeExtension(cmdFile, ".bat");

                //using (StreamWriter sw = File.CreateText(cmdFile))
                //{
                //    sw.WriteLine(String.Format("sqlcmd -S {0} -d {1} -i {2}", serverName, dbName, tabFile));
                //    sw.WriteLine(String.Format("sqlcmd -S {0} -d {1} -i {2}", serverName, dbName, fkFile));
                //}

                // Run the command script
                //batchRunnerControl.RunCommand(this, cmdFile);
                string args = String.Format("-S {0} -d {1} -i {2}", serverName, dbName, tabFile);
                batchRunnerControl.RunCommand(this, "sqlcmd", args);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        string GetResourceFile(string resourceName)
        {
            Assembly a = Assembly.GetExecutingAssembly();
            Stream fs = a.GetManifestResourceStream("Backsight.Environment.Editor.Resources." + resourceName);
            byte[] data = new byte[fs.Length];
            fs.Read(data, 0, data.Length);
            string tmpDir = Path.GetTempPath();
            string fileName = Path.Combine(tmpDir, resourceName);
            File.WriteAllBytes(fileName, data);
            return fileName;
        }

        private void CreateTablesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult = (m_IsCreated ? DialogResult.OK : DialogResult.Cancel);
        }

        public void LogMessage(string message) // ILog
        {
            batchRunnerControl.ReportMessage(message);
            //int index = listBox.Items.Add(message);
            //listBox.SelectedIndex = index;
        }

        #region IBatchRunner Members

        public void RunCompleted(Exception e)
        {
            if (e == null)
            {
                m_IsCreated = true;

                // For some reason, the "Done" doesn't getting scrolled into view, so try logging
                // a further empty line.
                LogMessage("Done");
                LogMessage(String.Empty); 

                createButton.Text = "OK";
                createButton.Enabled = true;

                // Get rid of the command script (for now, we just leave the sql scripts
                // lying around, since they could be useful if something went wrong)
                //File.Delete(batchRunnerControl.LastCommand);
            }
            else
            {
                cancelButton.Enabled = true;
            }
        }

        #endregion
    }
}