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

using Backsight.Data;

namespace Backsight.Editor.Database
{
    /// <summary>
    /// Database access to the <c>Jobs</c> table.
    /// </summary>
    class Job
    {
        #region Static

        /// <summary>
        /// Selects all defined jobs (ordered by name)
        /// </summary>
        /// <returns>All defined jobs (may be an empty array, but never null)</returns>
        internal static Job[] FindAll()
        {
            using (IConnection ic = ConnectionFactory.Create())
            {
                string sql = "SELECT [JobId], [Name], [ZoneId], [LayerId] FROM [ced].[Jobs] ORDER BY [Name]";
                SqlCommand cmd = new SqlCommand(sql, ic.Value);
                List<Job> result = new List<Job>(1000);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        uint jobId = (uint)rdr.GetInt32(0);
                        Job j = new Job(jobId, rdr.GetString(1), rdr.GetInt32(2), rdr.GetInt32(3));
                        result.Add(j);
                    }
                }

                return result.ToArray();
            }
        }

        /// <summary>
        /// Attempts to locate job information based on the internal job ID.
        /// </summary>
        /// <param name="jobId">The ID of the job to select</param>
        /// <returns>The corresponding job (null if not found)</returns>
        internal static Job FindByJobId(uint jobId)
        {
            using (IConnection ic = ConnectionFactory.Create())
            {
                string sql = "SELECT [Name], [ZoneId], [LayerId] FROM [ced].[Jobs] WHERE [JobId]="+jobId;
                SqlCommand cmd = new SqlCommand(sql, ic.Value);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.HasRows)
                    {
                        rdr.Read();
                        return new Job(jobId, rdr.GetString(0), rdr.GetInt32(1), rdr.GetInt32(2));
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Inserts a new job into the database
        /// </summary>
        /// <param name="jobName">The user-perceived name for the job</param>
        /// <param name="zoneId">The ID of the spatial zone the job covers</param>
        /// <param name="layerId">The ID of the (base) map layer for the job</param>
        /// <returns>The newly created job</returns>
        internal static Job Insert(string jobName, int zoneId, int layerId)
        {
            using (IConnection ic = ConnectionFactory.Create())
            {
                string sql = String.Format("INSERT INTO [ced].[Jobs] ([Name], [ZoneId], [LayerId]) VALUES ('{0}', {1}, {2})",
                                            jobName, zoneId, layerId);
                SqlCommand cmd = new SqlCommand(sql, ic.Value);
                cmd.ExecuteNonQuery();
                uint jobId = DbUtil.GetLastId(ic.Value);
                return new Job(jobId, jobName, zoneId, layerId);
            }
        }

        #endregion

        #region Class data

        /// <summary>
        /// The internal ID for the job
        /// </summary>
        uint m_JobId;

        /// <summary>
        /// The user-perceived name for the job
        /// </summary>
        string m_Name;

        /// <summary>
        /// The internal ID of the spatial zone the job covers
        /// </summary>
        int m_ZoneId;

        /// <summary>
        /// The internal ID of the map layer the job involves. If the layer is
        /// part of a theme, edits made as part of the job will affect this layer, as well as
        /// layers derived from it.
        /// </summary>
        int m_LayerId;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>Job</c> using information selected from the database.
        /// </summary>
        /// <param name="jobId">The internal ID for the job</param>
        /// <param name="name">The user-perceived name for the job</param>
        /// <param name="zoneId">The internal ID of the spatial zone the job covers</param>
        /// <param name="layerId">The internal ID of the map layer the job involves. If the layer is
        /// part of a theme, edits made as part of the job will affect this layer, as well as
        /// layers derived from it.</param>
        internal Job(uint jobId, string name, int zoneId, int layerId)
        {
            m_JobId = jobId;
            m_Name = name;
            m_ZoneId = zoneId;
            m_LayerId = layerId;
        }

        #endregion

        /// <summary>
        /// The internal ID for the job
        /// </summary>
        internal uint JobId
        {
            get { return m_JobId; }
        }

        /// <summary>
        /// The user-perceived name for the job
        /// </summary>
        internal string Name
        {
            get { return m_Name; }
        }

        /// <summary>
        /// The internal ID of the spatial zone the job covers
        /// </summary>
        internal int ZoneId
        {
            get { return m_ZoneId; }
        }

        /// <summary>
        /// The internal ID of the map layer the job involves. If the layer is
        /// part of a theme, edits made as part of the job will affect this layer, as well as
        /// layers derived from it.
        /// </summary>
        internal int LayerId
        {
            get { return m_LayerId; }
        }

        /// <summary>
        /// Override returns the user-perceived name for the job.
        /// </summary>
        /// <returns>The <see cref="Name"/> property</returns>
        public override string ToString()
        {
            return m_Name;
        }
    }
}
