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

using Backsight.SqlServer;
using System.Reflection;
using System.IO;

namespace Backsight.Environment.Editor
{
    /// <written by="Steve Stanton" on="20-SEP-2007" ticket="1"/>
    /// <summary>
    /// Dialog that gets displayed when the Environment Editor determines that Backsight
    /// system tables do not exist in a database.
    /// </summary>
    public partial class CreateTablesForm : Form, ILog
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

                // Create batch file for initializing the database
                string createFile = GetTableCreationScipt();

                m_Factory.CreateTables(this);
                m_IsCreated = true;
                LogMessage("Done");
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            finally
            {
                if (m_IsCreated)
                {
                    createButton.Text = "OK";
                    createButton.Enabled = true;
                }
                else
                    cancelButton.Enabled = true;
            }
        }

        string GetTableCreationScipt()
        {
            string tmpDir = Path.GetTempPath();
            string result = Path.Combine(tmpDir, "CreateBacksightTables.sql");

            // Copy the relevant resources to the temp file.
            Assembly a = Assembly.GetExecutingAssembly();
            Stream fs = a.GetManifestResourceStream("Backsight.Environment.Editor.Resources.CreateTables.sql");
            byte[] data = new byte[fs.Length];
            fs.Read(data, 0, data.Length);
            File.WriteAllBytes(result, data);

            //string args = String.Format("-S {0} -v DBNAME=\"{1}\" -i \"{2}\"", 
            string args = null;
            batchRunnerControl.Completed += new EventHandler(batchRunnerControl_Completed);
            batchRunnerControl.RunCommand("sqlcmd", args);

            return result;
        }

        void batchRunnerControl_Completed(object sender, EventArgs e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        private void CreateTablesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult = (m_IsCreated ? DialogResult.OK : DialogResult.Cancel);
        }

        public void LogMessage(string message) // ILog
        {
            int index = listBox.Items.Add(message);
            listBox.SelectedIndex = index;
        }
    }
}