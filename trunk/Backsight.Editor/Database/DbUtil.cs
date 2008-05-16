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
        internal static int GetLastId(SqlConnection c)
        {
            SqlCommand cmd = new SqlCommand("SELECT SCOPE_IDENTITY()", c);
            object o = cmd.ExecuteScalar();
            return Convert.ToInt32(o);
        }
    }
}
