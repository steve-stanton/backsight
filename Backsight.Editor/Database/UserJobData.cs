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
using System.Diagnostics;

using Backsight.Data;

namespace Backsight.Editor.Database
{
    /// <summary>
    /// Database access to the <c>ced.UserJobs</c> table. This deals with revision numbers
    /// used to mark publication of edits.
    /// </summary>
    class UserJobData
    {
        #region Static

        /// <summary>
        /// Obtains the last revision number associated with the supplied job and user
        /// </summary>
        /// <param name="job">The job of interest</param>
        /// <param name="user">The user involved</param>
        /// <returns>The sequence number of the last job publication made by the specified user (0 if no
        /// publications have been made)</returns>
        internal static uint GetLastRevision(Job job, User user)
        {
            using (IConnection ic = AdapterFactory.GetConnection())
            {
                string sql = String.Format("SELECT [LastRevision] FROM [ced].[UserJobs] WHERE [UserId]={0} AND [JobId]={1}",
                                user.UserId, job.JobId);
                SqlCommand cmd = new SqlCommand(sql, ic.Value);
                object o = cmd.ExecuteScalar();
                if (o==null)
                    return 0;
                else
                    return Convert.ToUInt32(o);
            }
        }

        /// <summary>
        /// Associates the specified job and user with a revision number. This will normally be
        /// called as part of some bigger database transaction.
        /// </summary>
        /// <param name="job">The job of interest</param>
        /// <param name="user">The user involved</param>
        /// <param name="revision">The sequence number of the publication</param>
        internal static void SetLastRevision(Job job, User user, uint revision)
        {
            // Try an update first. If that fails, do an insert.

            using (IConnection ic = AdapterFactory.GetConnection())
            {
                string sql = String.Format("UPDATE [ced].[UserJobs] SET [LastRevision]={0} WHERE [UserId]={1} AND [JobId]={2}",
                                revision, user.UserId, job.JobId);
                SqlCommand cmd = new SqlCommand(sql, ic.Value);
                int nRows = cmd.ExecuteNonQuery();

                if (nRows==0)
                {
                    sql = String.Format("INSERT INTO [ced].[UserJobs] ([UserId], [JobId], [LastRevision]) VALUES ({0}, {1}, {2})",
                            user.UserId, job.JobId, revision);
                    cmd = new SqlCommand(sql, ic.Value);
                    nRows = cmd.ExecuteNonQuery();
                    Debug.Assert(nRows==1);
                }
            }
        }

        #endregion
    }
}
