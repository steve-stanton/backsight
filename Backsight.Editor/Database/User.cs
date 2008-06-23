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
using System.Collections.Generic;

using Backsight.Data;

namespace Backsight.Editor.Database
{
    /// <summary>
    /// Database access to the <c>Users</c> table.
    /// </summary>
    class User
    {
        #region Static

        /// <summary>
        /// Selects all defined users (ordered by name)
        /// </summary>
        /// <returns>All defined users (may be an empty array, but never null)</returns>
        internal static User[] FindAll()
        {
            using (IConnection ic = AdapterFactory.GetConnection())
            {
                string sql = "SELECT [UserId], [Name] FROM [ced].[Users] ORDER BY [Name]";
                SqlCommand cmd = new SqlCommand(sql, ic.Value);
                List<User> result = new List<User>(1000);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        uint userId = (uint)rdr.GetInt32(0);
                        string name = rdr.GetString(1);
                        User u = new User(userId, name);
                        result.Add(u);
                    }
                }

                return result.ToArray();
            }
        }

        /// <summary>
        /// Attempts to select user information by primary key
        /// </summary>
        /// <param name="userId">The ID of the user to select</param>
        /// <returns>The corresponding user (null if not found)</returns>
        internal static User FindByPrimaryKey(uint userId)
        {
            using (IConnection ic = AdapterFactory.GetConnection())
            {
                string sql = String.Format("SELECT [Name] FROM [ced].[Users] WHERE [UserId]={0}", userId);
                SqlCommand cmd = new SqlCommand(sql, ic.Value);
                object o = cmd.ExecuteScalar();
                if (o==null)
                    return null;

                return new User(userId, o.ToString());
            }
        }

        /// <summary>
        /// Obtains the ID of the user who is currently logged in. If the user is not
        /// registered in the database, they will be added.
        /// </summary>
        /// <returns>The ID of the current user</returns>
        internal static User GetCurrentUser()
        {
            string userName = System.Environment.UserName;
            string sql = String.Format("SELECT [UserId] FROM [ced].[Users] WHERE [Name]='{0}'", userName);

            using (IConnection ic = AdapterFactory.GetConnection())
            {
                SqlCommand cmd = new SqlCommand(sql, ic.Value);
                object result = cmd.ExecuteScalar();
                if (result==null)
                {
                    sql = String.Format("INSERT INTO [ced].[Users] ([Name]) VALUES ('{0}')", userName);
                    cmd = new SqlCommand(sql, ic.Value);
                    cmd.ExecuteNonQuery();

                    cmd = new SqlCommand("SELECT SCOPE_IDENTITY()", ic.Value);
                    result = cmd.ExecuteScalar();
                    if (result == null)
                        throw new Exception("Failed to assign user ID");
                }

                uint userId = Convert.ToUInt32(result);
                return new User(userId, userName);
            }
        }

        #endregion

        #region Class data

        /// <summary>
        /// The internal ID for the user
        /// </summary>
        uint m_UserId;

        /// <summary>
        /// The user-perceived name for the user
        /// </summary>
        string m_Name;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>User</c> using information selected from the database.
        /// </summary>
        /// <param name="userId">The internal ID for the user</param>
        /// <param name="name">The user-perceived name for the user</param>
        User(uint userId, string name)
        {
            m_UserId = userId;
            m_Name = name;
        }

        #endregion

        /// <summary>
        /// The internal ID for the user
        /// </summary>
        internal uint UserId
        {
            get { return m_UserId; }
        }

        /// <summary>
        /// The user-perceived name for the user
        /// </summary>
        internal string Name
        {
            get { return m_Name; }
        }

        /// <summary>
        /// Provides a string for representing this object
        /// </summary>
        /// <returns>The <see cref="Name"/> property</returns>
        public override string ToString()
        {
            return m_Name;
        }
    }
}
