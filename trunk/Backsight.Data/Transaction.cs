/// <remarks>
/// Copyright 2007 - Steve Stanton. This file is part of Backsight
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
using System.Transactions;

namespace Backsight.Data
{
    /// <written by="Steve Stanton" on="10-NOV-2006" />
    /// <summary>
    /// Supports database transactions that utilize a specific connection for the duration.
    /// This is meant to avoid dependence on MSDTC, which would be utilized if a transaction
    /// works with more than one connection.
    /// </summary>
    public static class Transaction
    {
        /// <summary>
        /// Stuff to do as part of a call to <c>Transaction.Execute</c>. This will be
        /// called after the database connection has been opened, so it shouldn't contain
        /// stuff that is likely to keep the connection open for an extended time (e.g. it
        /// shouldn't prompt the user for anything).
        /// <para/>
        /// If the body of a transaction encounters some problem, and returns without raising
        /// an exception, any updates already made <b>will</b> be committed. To abort the transaction,
        /// you must throw an exception (you will probably want to catch it soon after your
        /// call to <c>Transaction.Execute</c>).
        /// </summary>
        public delegate void TransactionBody();

        /// <summary>
        /// The connection to use for the duration of a transaction
        /// </summary>
        private static ConnectionWrapper s_Connection = null;

        /// <summary>
        /// Wrapper on the connection that's being used to execute the current transaction (null if
        /// a transaction isn't in progress)
        /// </summary>
        public static IConnection Connection
        {
            get { return s_Connection; }
        }

        /// <summary>
        /// Runs a section of code as a multi-statement transaction.
        /// </summary>
        /// <param name="body">The code to execute within the transaction (may contain
        /// further calls to <c>Transaction.Execute</c>)</param>
        public static void Execute(TransactionBody body)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                Exec(body);
                ts.Complete();
            }
        }

        private static void Exec(TransactionBody body)
        {
            bool connectionCreated = false;

            try
            {
                if (s_Connection==null)
                {
                    lock (typeof(Transaction))
                    {
                        if (s_Connection==null)
                        {
                            string cs = AdapterFactory.ConnectionString;
                            if (String.IsNullOrEmpty(cs))
                                throw new InvalidOperationException("Connection string hasn't been defined");

                            SqlConnection conn = new SqlConnection(cs);
                            s_Connection = new ConnectionWrapper(conn, false);
                            connectionCreated = true;
                        }
                    }
                    s_Connection.Value.Open();
                }

                body();
            }

            finally
            {
                if (connectionCreated)
                {
                    s_Connection.Value.Dispose();
                    s_Connection = null;
                }
            }
        }
    }
}
