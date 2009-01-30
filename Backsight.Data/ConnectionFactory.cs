// <remarks>
// Copyright 2007 - Steve Stanton. This file is part of Backsight
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

namespace Backsight.Data
{
    /// <written by="Steve Stanton" on="10-NOV-2006" />
    /// <summary>
    /// Creates database connections
    /// </summary>
    public static class ConnectionFactory
    {
        static string s_ConnectionString;

        /// <summary>
        /// The database connection string that should be utilized by subsequent
        /// calls to the <see cref="GetConnection"/> method.
        /// </summary>
        public static string ConnectionString
        {
            get { return s_ConnectionString; }
            set { s_ConnectionString = value; }
        }

        /// <summary>
        /// Returns a database connection based on the connection string defined
        /// via the <see cref="ConnectionString"/> property.
        /// </summary>
        /// <returns>A wrapper on the connection to use. If a transaction is currently active (as defined via
        /// use of the <c>Transaction</c> class), you get back a wrapper on the connection associated with
        /// the transaction. Otherwise you get a wrapper on a new connection object based on the
        /// <c>ConnectionString</c> property setting. In either case, the connection is open at return.
        /// </returns>
        public static IConnection Create()
        {
            IConnection c = Transaction.Connection;
            if (c != null)
                return c;

            if (String.IsNullOrEmpty(s_ConnectionString))
                throw new InvalidOperationException("Connection string hasn't been defined");

            // Return a disposable connection wrapper
            SqlConnection conn = new SqlConnection(s_ConnectionString);
            conn.Open();
            return new ConnectionWrapper(conn, true);
        }
    }
}
