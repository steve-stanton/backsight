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

using Backsight.Data;
using Backsight.Editor.Database;
using Backsight.Editor.Forms;
using Backsight.Environment;
using Backsight.SqlServer;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="08-MAY-2008"/>
    /// <summary>
    /// Logic relating to startup of the Cadastral Editor
    /// </summary>
    class Starter
    {
        #region Class data

        /// <summary>
        /// The job file that the user double-clicked on (null if the application
        /// was launched some other way)
        /// </summary>
        JobFile m_JobFile;

        /// <summary>
        /// The ID of the user involved (null for no user)
        /// </summary>
        User m_User;

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
            m_User = null;

            /*
            if (m_JobFile == null)
            {
                m_ConnectionString = String.Empty;
                m_JobId = 0;
            }
            else
            {
                m_ConnectionString = m_JobFile.Data.ConnectionString;
                m_JobId = m_JobFile.JobId;
            }
             */
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

            ConnectionFactory.ConnectionString = String.Empty;

            while (String.IsNullOrEmpty(ConnectionFactory.ConnectionString))
            {
                // If the job file isn't defined, get the database connection string
                string cs = null;
                if (m_JobFile == null)
                {
                    cs = LastDatabase.ConnectionString;

                    if (String.IsNullOrEmpty(cs))
                        cs = GetConnectionString();

                    if (String.IsNullOrEmpty(cs))
                        return false;
                }
                else
                    cs = m_JobFile.Data.ConnectionString;

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
            m_User = User.GetCurrentUser();

            // Confirm that we can get the job info from the database

            if (m_JobFile != null)
            {
                try
                {
                    uint jobId = m_JobFile.Data.JobId;
                    Job j = Job.FindByJobId(jobId);
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    m_JobFile = null;
                }
            }

            // If we don't have a job, ask the user whether they want to open an existing
            // job, or create a new one.
            if (m_JobFile==null)
            {
                GetJobForm dial = new GetJobForm();
                if (dial.ShowDialog() == DialogResult.OK)
                    m_JobFile = dial.NewJobFile;

                dial.Dispose();
            }

            return (m_JobFile != null);
        }

        string GetConnectionString()
        {
            string cs = String.Empty;

            ConnectionForm dial = new ConnectionForm();
            if (dial.ShowDialog() == DialogResult.OK)
            {
                cs = dial.ConnectionString;
                SetConnectionString(cs);
            }

            dial.Dispose();
            return cs;
        }

        void SetConnectionString(string cs)
        {
            ConnectionFactory.ConnectionString = cs;
            LastDatabase.ConnectionString = cs;
        }

        /// <summary>
        /// The current user (null if not known)
        /// </summary>
        internal User User
        {
            get { return m_User; }
        }

        /// <summary>
        /// The job file used to launch the application
        /// </summary>
        internal JobFile JobFile
        {
            get { return m_JobFile; }
        }
    }
}
