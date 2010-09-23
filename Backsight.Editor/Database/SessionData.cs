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
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Data;
using System.Data.SqlTypes;
using System.Xml;

using Backsight.Data;
using Backsight.Environment;
using Backsight.Editor.Xml;

namespace Backsight.Editor.Database
{
    /// <summary>
    /// Information about an editing session
    /// </summary>
    class SessionData
    {
        #region Static

        /// <summary>
        /// Loads session data for a job (for the specified user).
        /// </summary>
        /// <param name="model">The model to load</param>
        /// <param name="job">The job to load</param>
        /// <param name="user">The user who is doing the load</param>
        internal static void Load(CadastralMapModel model, Job job, User user)
        {
            List<SessionData> sessions = new List<SessionData>(1000);

            // Grab information about all defined users
            User[] allUsers = User.FindAll();

            // Grab information about all defined jobs
            // TODO: This isn't very smart!
            Job[] allJobs = Job.FindAll();

            using (IConnection ic = ConnectionFactory.Create())
            {
                // Load information about the sessions involved
                SqlConnection con = ic.Value;
                LoadPublishedSessions(con, sessions, job);
                LoadUnpublishedSessions(con, sessions, job, user.UserId);

                // Stuff the session IDs into a temp table and use it to load the edits
                string sessionTable = String.Format("#sessions_{0}_{1}", job.JobId, user.UserId);
                CopySessionIdsToTable(con, sessions, sessionTable);

                // Initialize session capacity in the model
                model.SetSessionCapacity(sessions.Count+1);

                // Load information about the edits involved
                StringBuilder sb = new StringBuilder(200);
                sb.Append("SELECT [SessionId], [EditSequence], [Data]");
                sb.Append(" FROM [ced].[Edits] WHERE [SessionId] IN");
                sb.AppendFormat(" (SELECT [SessionId] FROM {0})", sessionTable);
                sb.Append(" ORDER BY [SessionId], [EditSequence]");
                SqlCommand cmd = new SqlCommand(sb.ToString(), con);

                SessionData curSessionData = null;
                Session curSession = null;
                User curUser = null;
                Job curJob = null;
                Trace.Write("Reading data...");

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        uint sessionId = (uint)reader.GetInt32(0);
                        uint editSequence = (uint)reader.GetInt32(1);

                        // Ensure we have the correct session object
                        if (curSessionData==null || curSessionData.m_SessionId!=sessionId)
                        {
                            curSessionData = sessions.Find(delegate(SessionData s)
                                            { return (s.m_SessionId==sessionId); });
                            Debug.Assert(curSessionData != null);

                            curUser = Array.Find<User>(allUsers, delegate(User u)
                                            { return (u.UserId==curSessionData.UserId); });
                            Debug.Assert(curUser != null);

                            curJob = Array.Find<Job>(allJobs, delegate(Job j)
                                            { return (j.JobId == curSessionData.JobId); });
                            Debug.Assert(curJob != null);

                            // Create the session (and append to the model)
                            curSession = Session.CreateCurrentSession(model, curSessionData, curUser, curJob);
                        }

                        SqlXml data = reader.GetSqlXml(2);
                        using (XmlReader xr = data.CreateReader())
                        {
                            Operation edit = DataFactory.Instance.ToOperation(curSession, xr);

                            // The edit sequence is repeated in the XML data
                            Debug.Assert(edit.EditSequence == editSequence);
                        }
                    }
                }

                Trace.Write("Attaching attributes...");
                AttributeData.Load(model.GetFeatureIds());

                Trace.Write("Calculating geometry...");
                Operation[] edits = model.GetCalculationSequence();
                foreach (Operation op in edits)
                    op.CalculateGeometry();

                // Create spatial index
                Trace.Write("Indexing...");
                model.CreateIndex(edits);
            }
        }

        /// <summary>
        /// Loads information relating to published sessions, sorted by revision number.
        /// </summary>
        /// <param name="con">The database connection to use</param>
        /// <param name="sessions">The sessions to append to</param>
        /// <param name="job">The job that's being loaded</param>
        static void LoadPublishedSessions(SqlConnection con, List<SessionData> sessions, Job job)
        {
            // Get the layer associated with the job
            ILayer layer = EnvironmentContainer.FindLayerById(job.LayerId);
            Debug.Assert(layer!=null);

            // Determine whether we're dealing with a simple layer, or something that
            // is part of a theme.
            string layerClause;
            ITheme t = layer.Theme;
            if (t==null)
                layerClause = String.Format("[LayerId]={0}", layer.Id);
            else
                layerClause = String.Format("[LayerId] IN (SELECT [LayerId] FROM [ced].[Layers] WHERE [ThemeId]={0} AND [ThemeSequence]<={1})",
                                        t.Id, layer.ThemeSequence);

            // Define where clause for picking up the relevant published sessions
            StringBuilder sb = new StringBuilder(200);
            sb.Append(GetSelectFromSessions());
            sb.Append(" WHERE [Revision]>0 AND [JobId] IN");
            sb.Append(" (SELECT [JobId] FROM [ced].[Jobs]");
            sb.AppendFormat(" WHERE [ZoneId]={0} AND {1})", job.ZoneId, layerClause);
            sb.Append(" ORDER BY [Revision]");

            SqlCommand cmd = new SqlCommand(sb.ToString(), con);
            LoadSessions(cmd, sessions);
        }

        /// <summary>
        /// Loads information relating to unpublished sessions, sorted by session ID.
        /// </summary>
        /// <param name="con">The database connection to use</param>
        /// <param name="sessions">The sessions to append to</param>
        /// <param name="job">The job that's being loaded</param>
        /// <param name="userId">The ID of the user involved</param>
        static void LoadUnpublishedSessions(SqlConnection con, List<SessionData> sessions, Job job, uint userId)
        {
            // Include user sessions (relevant to the job in question) that have not been published
            StringBuilder sb = new StringBuilder(200);
            sb.Append(GetSelectFromSessions());
            sb.AppendFormat(" WHERE [Revision]=0 AND [UserId]={0} AND [JobId]={1}",
                                userId, job.JobId);
            sb.Append(" ORDER BY [SessionId]");

            SqlCommand cmd = new SqlCommand(sb.ToString(), con);
            LoadSessions(cmd, sessions);
        }

        /// <summary>
        /// Obtains a select statement for retrieving data from the <c>Sessions</c> table
        /// </summary>
        /// <returns>A select statement for selecting rows from the <c>Sessions</c> table,
        /// without any where clause. Rows returned by this select can be parsed using
        /// a call to <see cref="ParseSelectFromSessions"/>.</returns>
        static string GetSelectFromSessions()
        {
            return "SELECT " + GetColumnNames() + "  FROM [ced].[Sessions]";
        }

        /// <summary>
        /// Obtains the column names for the <c>Sessions</c> table
        /// </summary>
        /// <returns>A comma-seperated list of the columns in the database table.</returns>
        static string GetColumnNames()
        {
            StringBuilder sb = new StringBuilder(200);

            sb.Append("[SessionId], ");
            sb.Append("[JobId], ");
            sb.Append("[UserId], ");
            sb.Append("[Revision], ");
            sb.Append("[StartTime], ");
            sb.Append("[EndTime], ");
            sb.Append("[NumItem]");

            return sb.ToString();
        }

        /// <summary>
        /// Parses a row selected from the <c>Sessions</c> table
        /// </summary>
        /// <param name="reader">The data reader (positioned at the row to parse). The
        /// query columns are expected to match the columns obtained via a prior call
        /// to <see cref="GetSelectFromSessions"/></param>
        /// <returns>The object representing the session</returns>
        static SessionData ParseSelectFromSessions(SqlDataReader reader)
        {
            uint sessionId = (uint)reader.GetInt32(0);
            uint jobId = (uint)reader.GetInt32(1);
            uint userId = (uint)reader.GetInt32(2);
            uint revision = (uint)reader.GetInt32(3);
            DateTime startTime = reader.GetDateTime(4);
            DateTime endTime = reader.GetDateTime(5);
            uint numItem = (uint)reader.GetInt32(6);

            return new SessionData(sessionId, jobId, userId, revision, startTime, endTime, numItem);
        }

        /// <summary>
        /// Loads information about sessions
        /// </summary>
        /// <param name="cmd">The select statement to execute (referring to the columns
        /// defined via a prior call to <see cref="GetSelectFromSessions"/>)</param>
        /// <param name="sessions">The list to append to</param>
        static void LoadSessions(SqlCommand cmd, List<SessionData> sessions)
        {
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    SessionData s = ParseSelectFromSessions(reader);
                    sessions.Add(s);
                }
            }
        }

        /// <summary>
        /// Copies the IDs of all loaded sessions into a new table.
        /// </summary>
        /// <param name="con">The database connection to use</param>
        /// <param name="tableName">The name of the table to create and load. This will
        /// typically be a temporary table (a name starting with the "#" character
        /// in SqlServer systems).</param>
        /// <param name="sessions">The sessions of interest</param>
        static void CopySessionIdsToTable(SqlConnection con, List<SessionData> sessions, string tableName)
        {
            // Stick session IDs into an array of row objects
            DataTable dt = new DataTable(tableName);
            dt.Columns.Add(new DataColumn("SessionId", typeof(int)));
            DataRow[] rows = new DataRow[sessions.Count];

            for (int i=0; i<rows.Length; i++)
            {
                rows[i] = dt.NewRow();
                rows[i][0] = (int)sessions[i].m_SessionId;
            }

            // Create the temp table
            string sql = String.Format("CREATE TABLE {0} ([SessionId] INT NOT NULL)", tableName);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.ExecuteNonQuery();

            // Bulk copy the rows into the temp table
            SqlBulkCopy bcp = new SqlBulkCopy(con);
            bcp.BatchSize = rows.Length;
            bcp.DestinationTableName = tableName;
            bcp.WriteToServer(rows);
        }

        /// <summary>
        /// Inserts a new session into the database
        /// </summary>
        /// <returns></returns>
        internal static SessionData Insert(uint jobId, uint userId)
        {
            using (IConnection ic = ConnectionFactory.Create())
            {
                DateTime now = DateTime.Now;
                string nowString = DbUtil.GetDateTimeString(now);

                string sql = String.Format("INSERT INTO [ced].[Sessions]" +
                    " ([JobId], [UserId], [Revision], [StartTime], [EndTime], [NumItem])" +
                    " VALUES ({0}, {1}, 0, {2}, {3}, 0)", jobId, userId, nowString, nowString);

                SqlCommand cmd = new SqlCommand(sql, ic.Value);
                cmd.ExecuteNonQuery();
                uint sessionId = DbUtil.GetLastId(ic.Value);
                return new SessionData(sessionId, jobId, userId, 0, now, now, 0);
            }
        }

        /// <summary>
        /// Sets the revision number for unpublished sessions.
        /// </summary>
        /// <param name="job">The job that's being published</param>
        /// <param name="user">The user who made the edits</param>
        /// <param name="revision">The revision number to assign</param>
        /// <returns>The number of sessions that were updated</returns>
        internal static int SetLastRevision(Job job, User user, uint revision)
        {
            using (IConnection ic = ConnectionFactory.Create())
            {
                string sql = String.Format("UPDATE [ced].[Sessions] SET [Revision]={0} "+
                                           "WHERE [JobId]={1} AND [UserId]={2} AND [Revision]=0",
                                           revision, job.JobId, user.UserId);
                SqlCommand cmd = new SqlCommand(sql, ic.Value);
                return cmd.ExecuteNonQuery();
            }
        }

        #endregion

        #region Class data

        /// <summary>
        /// Unique ID for the session.
        /// </summary>
        readonly uint m_SessionId;

        /// <summary>
        /// The ID of the job the session is part of
        /// </summary>
        readonly uint m_JobId;

        /// <summary>
        /// The ID of the user running the session
        /// </summary>
        readonly uint m_UserId;

        /// <summary>
        /// The revision number for the session (0 if the session has not been published)
        /// </summary>
        uint m_Revision;

        /// <summary>
        /// When was session started? 
        /// </summary>
        readonly DateTime m_Start;

        /// <summary>
        /// When was the last edit performed?
        /// </summary>
        DateTime m_End;

        /// <summary>
        /// The number of items (objects) created by the session
        /// </summary>
        uint m_NumItem;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>SessionData</c> using information selected from the database
        /// </summary>
        /// <param name="sessionId">Unique ID for the session.</param>
        /// <param name="jobId">The ID of the job the session is part of</param>
        /// <param name="userId">The ID of the user running the session</param>
        /// <param name="revision">The revision number for the session (0 if the session has not been published)</param>
        /// <param name="start">When was session started? </param>
        /// <param name="end">When was the last edit performed?</param>
        /// <param name="numItem">TThe number of items (objects) created by the session</param>
        SessionData(uint sessionId, uint jobId, uint userId, uint revision, DateTime start, DateTime end, uint numItem)
        {
            m_SessionId = sessionId;
            m_JobId = jobId;
            m_UserId = userId;
            m_Revision = revision;
            m_Start = start;
            m_End = end;
            m_NumItem = numItem;
        }

        #endregion

        /// <summary>
        /// Unique ID for the session.
        /// </summary>
        internal uint Id
        {
            get { return m_SessionId; }
        }

        /// <summary>
        /// The ID of the user running the session
        /// </summary>
        internal uint UserId
        {
            get { return m_UserId; }
        }

        /// <summary>
        /// The ID of the job the session is part of
        /// </summary>
        internal uint JobId
        {
            get { return m_JobId; }
        }

        /// <summary>
        /// When was session started? 
        /// </summary>
        internal DateTime StartTime
        {
            get { return m_Start; }
        }

        /// <summary>
        /// When was the last edit performed?
        /// </summary>
        internal DateTime EndTime
        {
            get { return m_End; }
        }

        /// <summary>
        /// The number of items (objects) created by the session
        /// </summary>
        internal uint NumItem
        {
            get { return m_NumItem; }
            set { m_NumItem = value; }
        }

        /// <summary>
        /// Deletes this session from the database. This will succeed only
        /// if no edits have been performed as part of the session.
        /// </summary>
        internal void Delete()
        {
            using (IConnection ic = ConnectionFactory.Create())
            {
                string sql = String.Format("DELETE FROM [ced].[Sessions] WHERE [SessionId]={0}",
                                                m_SessionId);
                SqlCommand cmd = new SqlCommand(sql, ic.Value);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Updates the end-time associated with this session. This should be
        /// called at the end of each editing operation.
        /// </summary>
        internal void UpdateEndTime()
        {
            using (IConnection ic = ConnectionFactory.Create())
            {
                DateTime now = DateTime.Now;
                string nowString = DbUtil.GetDateTimeString(now);
                string sql = String.Format("UPDATE [ced].[Sessions] SET [EndTime]={0}, [NumItem]={1}" +
                                            " WHERE [SessionId]={2}", nowString, m_NumItem, m_SessionId);
                SqlCommand cmd = new SqlCommand(sql, ic.Value);
                cmd.ExecuteNonQuery();
                m_End = now;
            }
        }

        /// <summary>
        /// Reserves an item number for use with this session. This should only
        /// be called for the currently active session. It is a lightweight
        /// request, because it just increments a counter. The database will not
        /// get updated until <see cref="UpdateEndTime"/> is called.
        /// </summary>
        /// <returns>The reserved item number</returns>
        internal uint ReserveNextItem()
        {
            Debug.Assert(m_SessionId == Session.WorkingSession.Id);
            m_NumItem++;
            return m_NumItem;
        }

        /// <summary>
        /// Gets rid of edits that the user has not explicitly saved. This gets rid of the edits,
        /// but does not revert the item count of update time that is stored in the <c>Sessions</c>
        /// table (that isn't considered to be bad).
        /// </summary>
        /// <param name="lastItemToKeep">The item number of the last thing that needs
        /// to be kept</param>
        internal void DiscardEdits(uint lastItemToKeep)
        {
            Transaction.Execute(delegate
            {
                // Get rid of the edits
                string sql = String.Format("DELETE FROM [ced].[Edits] WHERE [SessionId]={0} AND [EditSequence]>{1}",
                                                m_SessionId, lastItemToKeep);
                SqlCommand cmd = new SqlCommand(sql, Transaction.Connection.Value);
                cmd.ExecuteNonQuery();

                // Go back to the old item count (and update session time)
                m_NumItem = lastItemToKeep;
                UpdateEndTime();
            });
        }

        /// <summary>
        /// The revision number for the session (0 if the session has not been published)
        /// </summary>
        internal uint Revision
        {
            get { return m_Revision; }
            set { m_Revision = value; }
        }
    }
}
