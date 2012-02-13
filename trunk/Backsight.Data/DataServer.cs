// <remarks>
// Copyright 2012 - Steve Stanton. This file is part of Backsight
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
using System.Reflection;
using System.Transactions;

namespace Backsight.Data
{
    /// <summary>
    /// Database access methods.
    /// </summary>
    public class DataServer : IDataServer
    {
        #region Class data

        /// <summary>
        /// The database connection string.
        /// </summary>
        readonly string m_ConnectionString;

        /// <summary>
        /// The connection that is being used to execute a multi-statement transaction (null
        /// if a transaction is not currently running).
        /// </summary>
        IConnection m_Transaction;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataServer"/> class.
        /// </summary>
        /// <param name="connectionString">The database connection string.</param>
        /// <exception cref="ArgumentNullException">If the supplied connection string is null or just whitespace.</exception>
        public DataServer(string connectionString)
        {
            if (String.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException();

            m_ConnectionString = connectionString;
            m_Transaction = null;
        }

        #endregion

        /// <summary>
        /// Creates a new adapter and associates it with a database connection.
        /// </summary>
        /// <typeparam name="T">The type of adapter to create</typeparam>
        /// <returns>The newly created adapter</returns>
        /// <exception cref="ArgumentException">If the supplied adapter type doesn't have a "Connection" property</exception>
        public T CreateAdapter<T>() where T : new()
        {
            SqlConnection c = (m_Transaction == null ? GetSqlConnection() : m_Transaction.Value);

            // By default, the connection property in generated code is internal, not public, so you
            // need to search using BindingFlags.NonPublic. The mention of BindingFlags.Public is there
            // just in case the property has been tweaked to be public (since searching for non-public
            // properties doesn't locate the public ones!).

            T a = new T();
            PropertyInfo pi = a.GetType().GetProperty("Connection", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (pi == null)
                throw new ArgumentException("Adapter does not have a 'Connection' property");

            pi.SetValue(a, c, null);
            return a;
        }

        /// <summary>
        /// Creates a new database connection
        /// </summary>
        /// <returns>The connection to the database (not opened)</returns>
        SqlConnection GetSqlConnection()
        {
            return new SqlConnection(m_ConnectionString);
        }

        /// <summary>
        /// Executes a SELECT statement.
        /// </summary>
        /// <param name="sql">The select statement</param>
        /// <returns>The retrieved data</returns>
        public DataTable ExecuteSelect(string sql)
        {
            try
            {
                // Try the select with the option that has been used in the past. This ensures that things like the
                // length of varchar columns is reflected in the returned DataTable. While this is most likely
                // irrelevant in most cases, it's difficult to assess every call, so err on the side of caution.

                // The problem is that certain queries fail when you ask for this extra info (a problem
                // when the DataReader is closed by the adapter). At that time, you get an exception with
                // the message "Internal connection fatal error".

                // From what I can tell, this error occurs when you have a query that involves a WITH clause to
                // select a subset of rows from one table, then refer to the results as the LEFT OUTER join table
                // when selecting from another table. The error arises if both tables have a primary key, but
                // does not arise if the PKs are dropped (suggesting a SQLServer bug).

                // I have been unable to find any forum that describes this issue, and it's probably not worth
                // the effort of posting a public query. As a workaround just try the select and, if it fails,
                // repeat the select without requesting the extra info.

                return ExecuteSelect(sql, MissingSchemaAction.AddWithKey);
            }

            catch (InvalidOperationException)
            {
                // Dump the exception to trace file so that problem queries can be more easily identified (though
                // not likely, it's possible that the caller really does require the extra info, so the trace
                // message will help in that case).

                return ExecuteSelect(sql, MissingSchemaAction.Add);
            }
        }

        /// <summary>
        /// Executes a SELECT statement on the default database with the specified value for the
        /// <see cref="System.Data.SqlClient.SqlDataAdapter.MissingSchemaAction"/> property.
        /// </summary>
        /// <param name="sql">The select statement</param>
        /// <param name="msa">Handling for dealing with table info</c></param>
        /// <returns>The retrieved data</returns>
        DataTable ExecuteSelect(string sql, MissingSchemaAction msa)
        {
            using (IConnection ic = GetConnection())
            {
                DataTable dt = new DataTable();

                using (SqlDataAdapter adapter = new SqlDataAdapter(sql, ic.Value))
                {
                    adapter.MissingSchemaAction = msa;
                    adapter.Fill(dt);
                }

                return dt;
            }
        }

        /// <summary>
        /// Executes a SELECT statement that returns a single int value (suitable for statements
        /// like <c>SELECT COUNT(*) FROM SomeTable</c>).
        /// </summary>
        /// <param name="sql">The select statement</param>
        /// <returns>
        /// The selected value (0 if the result was a database null).
        /// </returns>
        public int ExecuteSelectInt(string sql)
        {
            using (IConnection ic = GetConnection())
            {
                return ExecuteSelectInt(ic, sql, 0);
            }
        }

        /// <summary>
        /// Executes a SELECT statement that returns a single int value (suitable for statements
        /// like <c>SELECT COUNT(*) FROM SomeTable</c>).
        /// </summary>
        /// <param name="sql">The select statement</param>
        /// <param name="defaultResult">The value to return if the select results in a database null</param>
        /// <returns>The result of the selection (or the supplied default value if the result was a database null)</returns>
        int ExecuteSelectInt(IConnection ic, string sql, int defaultResult)
        {
            SqlCommand cmd = new SqlCommand(sql, ic.Value);
            object o = cmd.ExecuteScalar();
            return (o == null || o == DBNull.Value ? defaultResult : Convert.ToInt32(o));
        }

        /// <summary>
        /// Executes an INSERT, UPDATE, or DELETE statement.
        /// </summary>
        /// <param name="sql">The SQL to execute</param>
        /// <returns>
        /// The number of rows affected by the statement
        /// </returns>
        public int ExecuteNonQuery(string sql)
        {
            using (IConnection ic = GetConnection())
            {
                SqlCommand cmd = new SqlCommand(sql, ic.Value);
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Obtains a database connection.
        /// </summary>
        /// <returns>A wrapper on the connection to use. If a transaction is currently active (as defined via
        /// a prior call to <see cref="RunTransaction"/>), you get back a wrapper on the connection associated with
        /// the transaction. Otherwise you get a wrapper on a new connection object based on the settings
        /// defined through the <c>FFConnection</c> class. In either case, the connection is open at return.
        /// </returns>
        public IConnection GetConnection()
        {
            // If a transaction is not running, return a disposable wrapper on a new connection (see
            // remarks for RunTransaction method).

            if (m_Transaction == null)
            {
                SqlConnection conn = GetSqlConnection();
                conn.Open();
                return new ConnectionWrapper(conn, true);
            }
            else
            {
                return m_Transaction;
            }
        }

        /// <summary>
        /// Tests whether it is currently possible to connect to the default database.
        /// </summary>
        /// <returns>True if a call to <see cref="GetConnection"/> succeeds. False if any
        /// exception is raised.</returns>
        public bool CanGetConnection()
        {
            try
            {
                using (IConnection ic = GetConnection()) { }
                return true;
            }

            catch { }
            return false;
        }

        /// <summary>
        /// Runs a section of code as a multi-statement transaction.
        /// </summary>
        /// <param name="body">The code to execute within the transaction (may contain
        /// further calls to <c>RunTransaction</c>)</param>
        /// <remarks>
        /// The code within the body must obtain database connections via calls to
        /// <see cref="GetConnection"/>. If the body attempts to obtain connections using
        /// <see cref="GetAlternateConnection"/>, the transaction may either fail or be
        /// ineffective (I haven't tried it out to see what happens).
        /// </remarks>
        public void RunTransaction(TransactionBody body)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                Exec(body);
                ts.Complete();
            }
        }

        /// <summary>
        /// Executes the body of a multi-statement transaction.
        /// </summary>
        /// <param name="body">The code to execute within the transaction</param>
        void Exec(TransactionBody body)
        {
            bool connectionCreated = false;

            try
            {
                if (m_Transaction == null)
                {
                    lock (this)
                    {
                        if (m_Transaction == null)
                        {
                            SqlConnection conn = GetSqlConnection();
                            m_Transaction = new ConnectionWrapper(conn, false);
                            connectionCreated = true;
                        }
                    }

                    m_Transaction.Value.Open();
                }

                body();
            }

            finally
            {
                if (connectionCreated)
                {
                    m_Transaction.Value.Dispose();
                    m_Transaction = null;
                }
            }
        }
    }
}
