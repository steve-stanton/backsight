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
using System.IO;
using System.Windows.Forms;
//using System.Data.SqlClient;

using Microsoft.Win32;

using Backsight.Data;
using Backsight.SqlServer;
using Backsight.Editor.Database;
using Backsight.Environment;
using Backsight.Editor.Forms;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="08-MAY-2008"/>
    /// <summary>
    /// Logic relating to startup of the Cadastral Editor
    /// </summary>
    class Starter
    {
        #region Static

        /// <summary>
        /// Attempts to open a job file
        /// </summary>
        /// <param name="fileSpec">The file specification of the job file</param>
        /// <returns>The job file</returns>
        /// <exception cref="Exception">If the specified file has an unexpected file type,
        /// the file does not exist, or <see cref="JobFile.CreateInstance"/> failed to parse
        /// the file.
        /// </exception>
        internal static JobFile GetJobFile(string fileSpec)
        {
            // Confirm it has the expected file extension (this is mainly to remind
            // people not to click on old-style files)
            string fileType = Path.GetExtension(fileSpec);
            if (String.Compare(fileType, JobFile.TYPE, true) != 0)
            {
                string msg = String.Format("Unexpected file extension (should be {0})", JobFile.TYPE);
                throw new Exception(msg);
            }

            // Confirm the file exists (it presumably exists if you double-clicked on it, but
            // perhaps the user launched from command line)
            if (!File.Exists(fileSpec))
            {
                string msg = String.Format("No such file: {0}", fileSpec);
                throw new Exception(msg);
            }

            // Parse the file
            return JobFile.CreateInstance(fileSpec);
        }

        #endregion

        #region Class data

        /// <summary>
        /// The job file that the user double-clicked on (null if the application
        /// was launched some other way)
        /// </summary>
        JobFile m_JobFile;

        /// <summary>
        /// The database connection string (blank if unknown)
        /// </summary>
        string m_ConnectionString;

        /// <summary>
        /// The ID of the user involved (0 for no user)
        /// </summary>
        int m_UserId;

        /// <summary>
        /// The ID of the job that's being edited
        /// </summary>
        int m_JobId;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>Starter</c> object in a situation where the user
        /// hasn't double-clicked on any job file.
        /// </summary>
        internal Starter()
            : this(null)
        {
        }

        /// <summary>
        /// Creates a new <c>Starter</c> in a situation where the user may have
        /// double-clicked on a job file.
        /// </summary>
        /// <param name="jobFile">The job file (could be null if user needs to be asked
        /// for info)</param>
        internal Starter(JobFile jobFile)
        {
            m_JobFile = jobFile;
            m_UserId = 0;

            if (m_JobFile == null)
            {
                m_ConnectionString = String.Empty;
                m_JobId = 0;
            }
            else
            {
                m_ConnectionString = m_JobFile.ConnectionString;
                m_JobId = m_JobFile.JobId;
            }
        }

        #endregion

        /// <summary>
        /// Attempt to open the application.
        /// </summary>
        /// <returns>True if application has opened ok. False if the application
        /// has failed to start.</returns>
        internal bool Open()
        {
            // If a job file was specified, attempt to connect to the database.

            AdapterFactory.ConnectionString = String.Empty;

            while (String.IsNullOrEmpty(AdapterFactory.ConnectionString))
            {
                // If the job file isn't defined, get the database connection string
                string cs = null;
                if (m_JobFile == null)
                {
                    cs = GetConnectionString();
                    if (String.IsNullOrEmpty(cs))
                        return false;
                }
                else
                    cs = m_JobFile.ConnectionString;

                // Attempt to open the database, to get the user ID for the person
                // who's currently logged in.

                try
                {
                    IEnvironmentContainer ec = new EnvironmentDatabase(cs);
                    EnvironmentContainer.Current = ec;
                    //using (SqlConnection c = new SqlConnection(cs))
                    //{
                    //    c.Open();
                    //}

                    // Remember the successful connection
                    SetConnectionString(cs);
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    m_JobFile = null;
                }
            }

            // If we get here, it means we have a valid database connection...

            // Get the ID of the current user
            m_UserId = User.GetUserId();

            // Get the job info from the database

            Job j = null;
            if (m_JobFile != null)
            {
                try
                {
                    j = Job.FindByJobId(m_JobFile.JobId);
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            // If we don't have a job, ask the user whether they want to open an existing
            // job, or create a new one.
            if (j==null)
            {
                GetJobForm dial = new GetJobForm();
                if (dial.ShowDialog() == DialogResult.OK)
                    j = dial.Job;

                dial.Dispose();
            }

            if (j==null)
                return false;

            MessageBox.Show(j.Name);
            return true;
        }

        string GetConnectionString()
        {
            string hklm = Registry.LocalMachine + @"\Software\Backsight";
            object o = Registry.GetValue(hklm, "ConnectionString", String.Empty);
            string cs = (o == null ? String.Empty : o.ToString());

            if (!String.IsNullOrEmpty(cs))
                GlobalUserSetting.LastConnection = cs;

            ConnectionForm dial = new ConnectionForm();
            if (dial.ShowDialog() == DialogResult.OK)
            {
                cs = dial.ConnectionString;
                SetConnectionString(cs);
            }
            else
                cs = String.Empty;

            dial.Dispose();
            return cs;
        }

        void SetConnectionString(string cs)
        {
            AdapterFactory.ConnectionString = cs;
            string hklm = Registry.LocalMachine + @"\Software\Backsight";
            Registry.SetValue(hklm, "ConnectionString", cs);
        }

        /// <summary>
        /// The ID of the current user (0 if not known)
        /// </summary>
        internal int UserId
        {
            get { return m_UserId; }
        }

        /// <summary>
        /// The ID of the current editing job (0 if not known)
        /// </summary>
        internal int JobId
        {
            get { return m_JobId; }
        }
    }
}