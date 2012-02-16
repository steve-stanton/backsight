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
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

using Backsight.Data;
using Backsight.Environment;

namespace Backsight.Editor.Database
{
    /// <summary>
    /// Miscellaneous helper methods that relate to the database.
    /// </summary>
    static class DbUtil
    {
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
            adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            adapter.Fill(dt);
            return dt;
        }

        /// <summary>
        /// Creates a new row for a database table
        /// </summary>
        /// <param name="t">The table of interest</param>
        /// <returns>A new row in the table (not yet inserted, with default data for
        /// all columns)</returns>
        /// <exception cref="InvalidOperationException">If there is no database connection</exception>
        internal static DataRow CreateNewRow(ITable t)
        {
            IDataServer ds = EditingController.Current.DataServer;
            if (ds == null)
                throw new InvalidOperationException("No database available");

            using (IConnection ic = ds.GetConnection())
            {
                string sql = String.Format("SELECT * FROM {0} WHERE 1=0", t.TableName);
                SqlDataAdapter adapter = new SqlDataAdapter(sql, ic.Value);
                DataTable dt = new DataTable(t.TableName);

                // Ensure the length is defined for char columns
                adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

                adapter.Fill(dt);
                return dt.NewRow();
            }
        }

        /// <summary>
        /// Saves a row in the database (adds if the <see cref="DataRow.RowState"/> has
        /// a value of <c>DataRowState.Detached</c>, otherwise attempts to update). This
        /// logic may well be defective in a situation where the row has already been added
        /// to a <c>DataTable</c>.
        /// </summary>
        /// <param name="row">The row to add. Usually a row that was created via a
        /// prior call to <see cref="CreateNewRow"/> (the important thing is that
        /// the table name must be defined as part of the rows associated <c>DataTable</c>)</param>
        /// <remarks>This makes use of the <see cref="SqlCommandBuilder"/> class to
        /// generate the relevant insert statement, which requires that the table involved
        /// must have a primary key (if not, you will get an exception)
        /// </remarks>
        /// <exception cref="ArgumentException">If the <see cref="DataTable"/> associated
        /// with the supplied row does not contain a defined table name.</exception>
        /// <exception cref="InvalidOperationException">If there is no database connection</exception>
        public static void SaveRow(DataRow row)
        {
            IDataServer ds = EditingController.Current.DataServer;
            if (ds == null)
                throw new InvalidOperationException("No database available");

            DataTable dt = row.Table;
            string tableName = dt.TableName;
            if (String.IsNullOrEmpty(tableName))
                throw new ArgumentException("Table name for row is not defined");

            using (IConnection ic = ds.GetConnection())
            {
                SqlDataAdapter a = new SqlDataAdapter("SELECT * FROM " + tableName, ic.Value);
                SqlCommandBuilder cb = new SqlCommandBuilder(a);
                if (row.RowState == DataRowState.Detached)
                    dt.Rows.Add(row);

                int nRows = a.Update(dt);
                Debug.Assert(nRows == 1);
            }
        }
    }
}
