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
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Data;

using Backsight.Data;
using Backsight.Environment;
using System.Data.SqlTypes;
using System.Xml;

namespace Backsight.Editor.Database
{
    /// <summary>
    /// Information about an editing session
    /// </summary>
    class SessionData
    {
        #region Static

        /// <summary>
        /// The current editing session.
        /// </summary>
        static SessionData s_CurrentSession = null;

        internal static SessionData CurrentSession
        {
            get { return s_CurrentSession; }
            set { s_CurrentSession = value; }
        }

        /// <summary>
        /// Loads session data for a job (for the user who is currently running the application).
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        internal static List<SessionData> Load(Job job)
        {
            List<SessionData> sessions = new List<SessionData>(1000);

            // Get the ID of the current user
            int userId = User.GetUserId();

            using (IConnection ic = AdapterFactory.GetConnection())
            {
                // Load information about the sessions involved
                SqlConnection con = ic.Value;
                LoadPublishedSessions(con, sessions, job);
                LoadUnpublishedSessions(con, sessions, job, userId);

                // Stuff the session IDs into a temp table and use it to load the edits
                string sessionTable = String.Format("#sessions_{0}_{1}", job.JobId, userId);
                CopySessionIdsToTable(con, sessions, sessionTable);

                // Create the loader
                uint numItem = SumItems(sessions);
                XmlContentReader xcr = new XmlContentReader(numItem);

                // Load information about the edits involved
                StringBuilder sb = new StringBuilder(200);
                sb.Append("SELECT [SessionId], [EditSequence], [Data]");
                sb.Append(" FROM [dbo].[Edits] WHERE [SessionId] IN");
                sb.AppendFormat(" (SELECT [SessionId] FROM {0})", sessionTable);
                sb.Append(" ORDER BY [SessionId], [EditSequence]");
                SqlCommand cmd = new SqlCommand(sb.ToString(), con);

                SessionData curSession = null;
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int sessionId = reader.GetInt32(0);
                        int editSequence = reader.GetInt32(1);

                        // Ensure we have the correct session object
                        if (curSession==null || curSession.m_SessionId!=sessionId)
                        {
                            curSession = sessions.Find(delegate(SessionData s)
                                            { return (s.m_SessionId==sessionId); });
                            Debug.Assert(curSession!=null);
                        }

                        SqlXml data = reader.GetSqlXml(2);
                        using (XmlReader xr = data.CreateReader())
                        {
                            xcr.LoadOperation(curSession, editSequence, xr);
                        }
                    }
                }
            }

            sessions.TrimExcess();
            return sessions;
        }

        /// <summary>
        /// Returns the summation of the item counts associated with the supplied sessions
        /// </summary>
        /// <param name="sessions">The sessions of interest</param>
        /// <returns>The sum of the <see cref="ItemCount"/> property</returns>
        static uint SumItems(List<SessionData> sessions)
        {
            uint result = 0;

            foreach (SessionData s in sessions)
                result += s.m_NumItem;

            return result;
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
                layerClause = String.Format("[LayerId] IN (SELECT [LayerId] FROM [dbo].[Layer] WHERE [ThemeId]={0} AND [ThemeSequence]<={1})",
                                        t.Id, layer.ThemeSequence);

            // Define where clause for picking up the relevant published sessions
            StringBuilder sb = new StringBuilder(200);
            sb.Append(GetSelectFromSessions());
            sb.Append(" WHERE [Revision]>0 AND [JobId] IN");
            sb.Append(" (SELECT [JobId] FROM [dbo].[Jobs]");
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
        static void LoadUnpublishedSessions(SqlConnection con, List<SessionData> sessions, Job job, int userId)
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
            return "SELECT " + GetColumnNames() + "  FROM [dbo].[Sessions]";
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
            int sessionId = reader.GetInt32(0);
            int jobId = reader.GetInt32(1);
            int userId = reader.GetInt32(2);
            int revision = reader.GetInt32(3);
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
        internal static SessionData Insert(int jobId, int userId)
        {
            using (IConnection ic = AdapterFactory.GetConnection())
            {
                DateTime now = DateTime.Now;
                string nowString = DbUtil.GetDateTimeString(now);

                string sql = String.Format("INSERT INTO [dbo].[Sessions]" +
                    " ([JobId], [UserId], [Revision], [StartTime], [EndTime], [NumEdit])" +
                    " VALUES ({0}, {1}, 0, {2}, {3}, 0)", jobId, userId, nowString, nowString);

                SqlCommand cmd = new SqlCommand(sql, ic.Value);
                cmd.ExecuteNonQuery();
                int sessionId = DbUtil.GetLastId(ic.Value);
                return new SessionData(sessionId, jobId, userId, 0, now, now, 0);
            }
        }

        #endregion

        #region Class data

        /// <summary>
        /// Unique ID for the session.
        /// </summary>
        readonly int m_SessionId;

        /// <summary>
        /// The ID of the job the session is part of
        /// </summary>
        readonly int m_JobId;

        /// <summary>
        /// The ID of the user running the session
        /// </summary>
        readonly int m_UserId;

        /// <summary>
        /// The revision number for the session (0 if the session has not been published)
        /// </summary>
        int m_Revision;

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
        SessionData(int sessionId, int jobId, int userId, int revision, DateTime start, DateTime end, uint numItem)
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
    }
}
