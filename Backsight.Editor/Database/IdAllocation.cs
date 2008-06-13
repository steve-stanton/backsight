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

using Backsight.Data;
using System.Text;

namespace Backsight.Editor.Database
{
    /// <summary>
    /// Database access to the <c>IdAllocation</c> table.
    /// </summary>
    class IdAllocation
    {
        #region Static

        /// <summary>
        /// Locates rows that refer to a specific job and user.
        /// </summary>
        /// <param name="job">The job</param>
        /// <param name="user">The user</param>
        /// <returns>The rows identifying ID ranges that have been allocated</returns>
        internal static IdAllocation[] FindByJobUser(Job job, User user)
        {
            using (IConnection ic = AdapterFactory.GetConnection())
            {
                List<IdAllocation> result = new List<IdAllocation>(1000);
                string sql = String.Format("{0} WHERE [JobId]={1} AND [UserId]={2}",
                                GetSelectString(), job.JobId, user.UserId);
                SqlCommand cmd = new SqlCommand(sql, ic.Value);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    IdAllocation item = ParseSelect(reader);
                    result.Add(item);
                }
                return result.ToArray();
            }
        }

        /// <summary>
        /// Inserts an ID allocation into the database
        /// </summary>
        /// <param name="idGroup">The ID group the allocation relates to</param>
        /// <param name="lowestId">The lowest value in the allocation (this is the primary key)</param>
        /// <param name="highestId">The highest value in the allocation</param>
        /// <param name="job">The job that the allocation is for</param>
        /// <param name="user">The user who made the allocation</param>
        /// <param name="insertTime"></param>
        /// <param name="numUsed">The number of IDs already used</param>
        internal static IdAllocation Insert(IdGroup idGroup, int lowestId, int highestId,
                                                Job job, User user, DateTime insertTime, int numUsed)
        {
            using (IConnection ic = AdapterFactory.GetConnection())
            {
                StringBuilder sb = new StringBuilder(200);
                sb.AppendFormat("INSERT INTO [dbo].[IdAllocation] ({0}) VALUES ", GetColumns());
                sb.AppendFormat("({0}, {1}, {2}, {3}, {4}, {5}, {6})",
                    idGroup.Id, lowestId, highestId, job.JobId, user.UserId,
                    DbUtil.GetDateTimeString(insertTime), numUsed);
                SqlCommand cmd = new SqlCommand(sb.ToString(), ic.Value);
                cmd.ExecuteNonQuery();
                return new IdAllocation(idGroup.Id, lowestId, highestId, (int)job.JobId, (int)user.UserId,
                                            insertTime, numUsed);
            }
        }

        /// <summary>
        /// Deletes an ID allocation
        /// </summary>
        /// <param name="lowestId">The lowest value in the allocation (this is the primary key)</param>
        /// <returns>The number of rows deleted (</returns>
        internal static int Delete(int lowestId)
        {
            using (IConnection ic = AdapterFactory.GetConnection())
            {
                string sql = String.Format("DELETE FROM [dbo].[IdAllocation] WHERE [LowestId]={0}", lowestId);
                SqlCommand cmd = new SqlCommand(sql, ic.Value);
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Trims or extends an ID allocation
        /// </summary>
        /// <param name="lowestId">The lowest value in the allocation (this is the primary key)</param>
        /// <param name="highestId">The trimmed upper end of the allocation</param>
        /// <returns>The number of rows updated (should be 1)</returns>
        internal static int UpdateHighestId(int lowestId, int highestId)
        {
            using (IConnection ic = AdapterFactory.GetConnection())
            {
                string sql = String.Format("UPDATE [dbo].[IdAllocation] SET [HighestId]={0}" +
                                            " WHERE [LowestId]={1}", highestId, lowestId);
                SqlCommand cmd = new SqlCommand(sql, ic.Value);
                return cmd.ExecuteNonQuery();
            }
        }

        #endregion

        #region Class data

        /// <summary>
        /// The unique ID of the ID group associated with this allocation
        /// </summary>
        int m_GroupId;

        /// <summary>
        /// The lowest value in the allocation (this is the primary key)
        /// </summary>
        int m_LowestId;

        /// <summary>
        /// The highest value in the allocation
        /// </summary>
        int m_HighestId;

        /// <summary>
        /// The ID of the job that the allocation is for
        /// </summary>
        int m_JobId;

        /// <summary>
        /// The ID of the user who made the allocation
        /// </summary>
        int m_UserId;

        /// <summary>
        /// When was the allocation inserted into the database?
        /// </summary>
        DateTime m_TimeAllocated;

        /// <summary>
        /// The number of IDs that have been used
        /// </summary>
        int m_NumUsed;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>IdAllocation</c> using information selected from the database
        /// </summary>
        /// <param name="groupId">The unique ID of the ID group</param>
        /// <param name="lowestId">The lowest value in the allocation (this is the primary key)</param>
        /// <param name="highestId">The highest value in the allocation</param>
        /// <param name="jobId">The ID of the job that the allocation is for</param>
        /// <param name="userId">The ID of the user who made the allocation</param>
        /// <param name="timeAllocated">When was the allocation inserted into the database?</param>
        /// <param name="numUsed">The number of IDs that have been used</param>
        IdAllocation(int groupId, int lowestId, int highestId, int jobId, int userId,
                            DateTime timeAllocated, int numUsed)
        {
            m_GroupId = groupId;
            m_LowestId = lowestId;
            m_HighestId = highestId;
            m_JobId = jobId;
            m_UserId = userId;
            m_TimeAllocated = timeAllocated;
            m_NumUsed = numUsed;
        }

        #endregion

        /// <summary>
        /// The names of the columns in the database table.
        /// </summary>
        /// <returns>A comma-seperated list of the column names</returns>
        static string GetColumns()
        {
            return "[GroupId], [LowestId], [HighestId], [JobId], [UserId], [TimeAllocated], [NumUsed]";
        }

        /// <summary>
        /// Obtains a select statement that can be used to select all columns from the
        /// database table. Rows returned using this select can be subsequently parsed
        /// with a call to <see cref="ParseSelect"/>.
        /// </summary>
        /// <returns>The SQL select statement (with no where clause)</returns>
        static string GetSelectString()
        {
            return String.Format("SELECT {0} FROM [dbo].[IdAllocation]", GetColumns());
        }

        /// <summary>
        /// Parses a selection that refers to the columns identified via a prior call to
        /// <see cref="GetSelectString"/>
        /// </summary>
        /// <param name="reader">The database reader, positioned at the row that needs to
        /// be parsed</param>
        /// <returns>Data corresponding to the content of the row</returns>
        static IdAllocation ParseSelect(SqlDataReader reader)
        {
            int groupId = reader.GetInt32(0);
            int lowestId = reader.GetInt32(1);
            int highestId = reader.GetInt32(2);
            int jobId = reader.GetInt32(3);
            int userId = reader.GetInt32(4);
            DateTime timeAllocated = reader.GetDateTime(5);
            int numUsed = reader.GetInt32(6);

            return new IdAllocation(groupId, lowestId, highestId, jobId, userId, timeAllocated, numUsed);
        }

        /// <summary>
        /// The unique ID of the ID group associated with this allocation
        /// </summary>
        internal int GroupId
        {
            get { return m_GroupId; }
        }

        /// <summary>
        /// The number of IDs in this allocation
        /// </summary>
        internal int Size
        {
            get { return (m_HighestId-m_LowestId+1); }
        }

        /// <summary>
        /// The lowest value in the allocation (this is the primary key)
        /// </summary>
        internal int LowestId
        {
            get { return m_LowestId; }
        }

        /// <summary>
        /// The highest value in the allocation
        /// </summary>
        internal int HighestId
        {
            get { return m_HighestId; }
        }

        /// <summary>
        /// The number of IDs that have been used
        /// </summary>
        internal int NumUsed
        {
            get { return m_NumUsed; }
        }

        /// <summary>
        /// Increments the number of used IDs.
        /// </summary>
        /// <remarks>TODO: This doesn't update the database. Either need to make sure it
        /// happens, or revisit whether it should actually be stored in the db</remarks>
        internal void IncrementNumUsed()
        {
            m_NumUsed++;
        }

        /// <summary>
        /// Trims or extends an ID allocation
        /// </summary>
        /// <param name="highestId">The new upper end of the allocation</param>
        /// <returns>The number of rows updated (should be 1)</returns>
        internal int UpdateHighestId(int highestId)
        {
            int nRows = UpdateHighestId(m_LowestId, highestId);
            if (nRows > 0)
                m_HighestId = highestId;
            return nRows;
        }
    }
}
