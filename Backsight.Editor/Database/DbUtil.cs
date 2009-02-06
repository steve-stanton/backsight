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
using System.Data.SqlClient;
using System.Diagnostics;
using System.Data;
using Backsight.Data;

namespace Backsight.Editor.Database
{
    /// <summary>
    /// Miscellaneous helper methods that relate to the database.
    /// </summary>
    static class DbUtil
    {
        /// <summary>
        /// Obtains the last identity value assigned using the supplied connection.
        /// </summary>
        /// <param name="c">The connection involved</param>
        /// <returns>The last ID that was assigned</returns>
        public static uint GetLastId(SqlConnection c)
        {
            SqlCommand cmd = new SqlCommand("SELECT SCOPE_IDENTITY()", c);
            object o = cmd.ExecuteScalar();
            return Convert.ToUInt32(o);
        }

        /// <summary>
        /// Expresses a DateTime value according to ISO8601,
        /// (matching CONVERT style 126)
        /// </summary>
        /// <param name="dt">The date and time to respresent</param>
        /// <returns>The standardized string used to refer to the date</returns>
        public static string GetDateTimeString(DateTime dt)
        {
            return String.Format("CONVERT(DATETIME, '{0}', 126)", dt.ToString("s"));
        }

        /// <summary>
        /// Executes a SELECT statement using the specified connection 
        /// </summary>
        /// <param name="c">The database connection to use</param>
        /// <param name="sql">The select statement</param>
        /// <returns>The retrieved data</returns>
        public static DataTable ExecuteSelect(SqlConnection c, string sql)
        {
            //Trace.WriteLine(sql);
            SqlCommand cmd = new SqlCommand(sql, c);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        /// <summary>
        /// Executes an INSERT, UPDATE, or DELETE statement using the current database connection.
        /// </summary>
        /// <param name="sql">The SQL to execute</param>
        /// <returns>The number of rows affected by the statement</returns>
        public static int ExecuteNonQuery(string sql)
        {
            using (IConnection ic = ConnectionFactory.Create())
            {
                Trace.WriteLine(sql);
                SqlCommand cmd = new SqlCommand(sql, ic.Value);
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Executes a simple INSERT statement (one that inserts just one row) into a table
        /// where the primary key is an identity value generated by the database. If you
        /// want to execute an insert involving many rows, you must use <see cref="ExecuteNonQuery"/>
        /// </summary>
        /// <param name="sql">The insert statement</param>
        /// <returns>The assigned row ID</returns>
        public static uint ExecuteIdentityInsert(string sql)
        {
            using (IConnection ic = ConnectionFactory.Create())
            {
                Trace.WriteLine(sql);
                SqlConnection c = ic.Value;
                SqlCommand cmd = new SqlCommand(sql, c);
                if (cmd.ExecuteNonQuery()==1)
                    return DbUtil.GetLastId(c);

                throw new Exception("Insert failed to add a single row: "+sql);
            }
        }
    }
}
