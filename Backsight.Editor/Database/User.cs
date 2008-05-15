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
    /// Database access to the <c>Users</c> table.
    /// </summary>
    static class User
    {
        /// <summary>
        /// Obtains the ID of the user who is currently logged in. If the user is not
        /// registered in the database, they will be added.
        /// </summary>
        /// <returns>The ID of the current user</returns>
        internal static int GetUserId()
        {
            string userName = System.Environment.UserName;
            string sql = String.Format("SELECT [UserId] FROM [dbo].[Users] WHERE [Name]='{0}'", userName);

            using (IConnection ic = AdapterFactory.GetConnection())
            {
                SqlCommand cmd = new SqlCommand(sql, ic.Value);
                object result = cmd.ExecuteScalar();
                if (result==null)
                {
                    sql = String.Format("INSERT INTO [dbo].[Users] ([Name]) VALUES ('{0}')", userName);
                    cmd = new SqlCommand(sql, ic.Value);
                    cmd.ExecuteNonQuery();

                    cmd = new SqlCommand("SELECT SCOPE_IDENTITY()", ic.Value);
                    result = cmd.ExecuteScalar();
                    if (result == null)
                        throw new Exception("Failed to assign user ID");
                }

                return Convert.ToInt32(result);
            }
        }
    }
}
