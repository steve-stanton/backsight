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
    /// <summary>
    /// Wrapper on a database connection.
    /// </summary>
    /// <remarks>This class is utilized by <see cref="ConnectionFactory.GetConnection"/> to help avoid
    /// premature disposal of the connection. If you call <c>GetConnection</c> while a transaction
    /// is running, you get back a wrapper on a non-disposable connection. If there's no transaction,
    /// you get back a disposable wrapper. Consider the following:
    /// <code>
    /// 
    ///   using (IConnection conn = ConnectionFactory.GetConnection())
    ///   {
    ///     SqlCommand cmd = new SqlCommand("UPDATE STUFF SET THING=123", conn.Value);
    ///     cmd.ExecuteNonQuery();
    ///   }
    /// 
    /// </code>
    /// If we worked with a plain <c>SqlConnection</c>, it's <c>Dispose</c> method would get
    /// called when the application hits the end of the <c>using</c> block. By using the wrapper,
    /// we can control whether the connection will be disposed of or not.
    /// </remarks>
    public class ConnectionWrapper : IConnection
    {
        #region Class data

        /// <summary>
        /// The database connection
        /// </summary>
        readonly SqlConnection m_Connection;

        /// <summary>
        /// Should the connection be disposed of by the <see cref="Dispose"/> method?
        /// </summary>
        readonly bool m_IsDisposable;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>ConnectionWrapper</c> that wraps the supplied connection.
        /// </summary>
        /// <param name="c">The connection to wrap (not null).</param>
        /// <param name="isDisposable">Should the connection be disposed of by the
        /// <see cref="Dispose"/> method? Specify <c>false</c> if the connection is being
        /// used by an enclosing transaction.</param>
        /// <exception cref="ArgumentNullException">If a null connection was supplied</exception>
        internal ConnectionWrapper(SqlConnection c, bool isDisposable)
        {
            if (c==null)
                throw new ArgumentNullException();

            m_Connection = c;
            m_IsDisposable = isDisposable;
        }

        #endregion

        /// <summary>
        /// The wrapped connection.
        /// </summary>
        public SqlConnection Value
        {
            get { return m_Connection; }
        }

        /// <summary>
        /// Disposes of the connection, so long as it was tagged as disposable when this
        /// wrapper was instantiated.
        /// </summary>
        public void Dispose()
        {
            if (m_IsDisposable)
                m_Connection.Dispose();
        }
    }
}
