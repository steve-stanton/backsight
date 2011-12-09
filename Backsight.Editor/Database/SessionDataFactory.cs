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
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;

using Backsight.Data;
using Backsight.Environment;

namespace Backsight.Editor.Database
{
    class SessionDataFactory
    {
        /// <summary>
        /// Loads session data for a job (for the specified user).
        /// </summary>
        /// <param name="model">The model to load</param>
        /// <param name="job">The job to load</param>
        internal static void Load(CadastralMapModel model, IJobInfo job)
        {
            throw new NotImplementedException();

            /*
            List<SessionData> sessions = new List<SessionData>(1000);

            // Grab information about all defined users
            IUser[] allUsers = User.FindAll();

            // Grab information about all defined jobs
            // TODO: This isn't very smart!
            Job[] allJobs = Job.FindAll();

            using (IConnection ic = ConnectionFactory.Create())
            {
                // Load information about the sessions involved
                SqlConnection con = ic.Value;
                LoadSessions(con, sessions, job);

                // Stuff the session IDs into a temp table and use it to load the edits
                string sessionTable = String.Format("#sessions_{0}", job.JobId);
                CopySessionIdsToTable(con, sessions, sessionTable);

                // Initialize session capacity in the model
                model.SetSessionCapacity(sessions.Count + 1);

                // Load information about the edits involved
                StringBuilder sb = new StringBuilder(200);
                sb.Append("SELECT [SessionId], [EditSequence], [Data]");
                sb.Append(" FROM [ced].[Edits] WHERE [SessionId] IN");
                sb.AppendFormat(" (SELECT [SessionId] FROM {0})", sessionTable);
                sb.Append(" ORDER BY [SessionId], [EditSequence]");
                SqlCommand cmd = new SqlCommand(sb.ToString(), con);

                SessionData curSessionData = null;
                Session curSession = null;
                IUser curUser = null;
                IJobInfo curJob = null;
                Trace.Write("Reading data...");

                EditDeserializer editDeserializer = new EditDeserializer(model);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        uint sessionId = (uint)reader.GetInt32(0);
                        uint editSequence = (uint)reader.GetInt32(1);

                        // Ensure we have the correct session object
                        if (curSessionData == null || curSessionData.Id != sessionId)
                        {
                            curSessionData = sessions.Find(delegate(SessionData s)
                            { return (s.Id == sessionId); });
                            Debug.Assert(curSessionData != null);

                            curUser = Array.Find<IUser>(allUsers, delegate(IUser u)
                            { return (u.UserId == curSessionData.UserId); });
                            Debug.Assert(curUser != null);

                            curJob = Array.Find<Job>(allJobs, delegate(Job j)
                            { return (j.JobId == curSessionData.JobId); });
                            Debug.Assert(curJob != null);

                            // Create the session (and append to the model)
                            curSession = Session.CreateCurrentSession(model, curSessionData, curUser, curJob);
                        }

                        string data = reader.GetString(2);
                        using (TextReader tr = new StringReader(data))
                        {
                            editDeserializer.SetReader(new TextEditReader(tr));
                            Operation edit = Operation.Deserialize(editDeserializer);

                            // The edit sequence should be repeated in the data string
                            Debug.Assert(edit.EditSequence == editSequence);
                        }
                    }
                }

                Trace.Write("Attaching attributes...");
                AttributeData.Load(model.GetFeatureIds());

                Trace.Write("Calculating geometry...");
                Operation[] edits = model.GetCalculationSequence();
                foreach (Operation op in edits)
                    op.CalculateGeometry(null);

                // Create spatial index
                Trace.Write("Indexing...");
                model.CreateIndex(edits);
            }
             */
        }

        /// <summary>
        /// Loads information relating to editing sessions.
        /// </summary>
        /// <param name="con">The database connection to use</param>
        /// <param name="sessions">The sessions to append to</param>
        /// <param name="job">The job that's being loaded</param>
        static void LoadSessions(SqlConnection con, List<SessionData> sessions, IJobInfo job)
        {
            throw new NotImplementedException();

            /*
            // Get the layer associated with the job
            ILayer layer = EnvironmentContainer.FindLayerById(job.LayerId);
            Debug.Assert(layer != null);

            // Determine whether we're dealing with a simple layer, or something that
            // is part of a theme.
            string layerClause;
            ITheme t = layer.Theme;
            if (t == null)
                layerClause = String.Format("[LayerId]={0}", layer.Id);
            else
                layerClause = String.Format("[LayerId] IN (SELECT [LayerId] FROM [ced].[Layers] WHERE [ThemeId]={0} AND [ThemeSequence]<={1})",
                                        t.Id, layer.ThemeSequence);

            // Define where clause for picking up the relevant published sessions
            StringBuilder sb = new StringBuilder(200);
            sb.Append(GetSelectFromSessions());
            sb.Append(" WHERE [JobId] IN");
            sb.Append(" (SELECT [JobId] FROM [ced].[Jobs]");
            sb.AppendFormat(" WHERE [ZoneId]={0} AND {1})", job.ZoneId, layerClause);

            SqlCommand cmd = new SqlCommand(sb.ToString(), con);
            LoadSessions(cmd, sessions);
             */
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
            DateTime startTime = reader.GetDateTime(3);
            DateTime endTime = reader.GetDateTime(4);
            uint numItem = (uint)reader.GetInt32(5);

            return new SessionData(sessionId, jobId, userId, startTime, endTime, numItem);
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

            for (int i = 0; i < rows.Length; i++)
            {
                rows[i] = dt.NewRow();
                rows[i][0] = (int)sessions[i].Id;
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
                    " ([JobId], [UserId], [StartTime], [EndTime], [NumItem])" +
                    " VALUES ({0}, {1}, {2}, {3}, 0)", jobId, userId, nowString, nowString);

                SqlCommand cmd = new SqlCommand(sql, ic.Value);
                cmd.ExecuteNonQuery();
                uint sessionId = DbUtil.GetLastId(ic.Value);
                return new SessionData(sessionId, jobId, userId, now, now, 0);
            }
        }

    }
}
