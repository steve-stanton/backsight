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

        internal static Job FindByJobId(int jobId)
        {
            using (IConnection ic = AdapterFactory.GetConnection())
            {
                string sql = "SELECT [Name], [ZoneId], [LayerId] FROM [dbo].[Jobs] WHERE [JobId]="+jobId;
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

        #endregion

        #region Class data

        /// <summary>
        /// The internal ID for the job
        /// </summary>
        int m_JobId;

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
        Job(int jobId, string name, int zoneId, int layerId)
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
        internal int JobId
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
    }
}
