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
    /// Database access to the <c>LastRevision</c> table.
    /// </summary>
    static class LastRevision
    {
        /// <summary>
        /// Reserves the next available revision number. To minimize access contention on the
        /// database table that holds the last revision number, this should be done <b>outwith</b>
        /// the transaction that actually commits the revision.
        /// </summary>
        /// <returns>The reserved revision number</returns>
        internal static uint ReserveValue()
        {
            // I originally had the following as a stored procedure (arguably a better place
            // for it), but I'd rather keep things in code...

            uint result = 0;

            Transaction.Execute(delegate
            {
                SqlConnection con = Transaction.Connection.Value;
                SqlCommand cmd = new SqlCommand("DELETE FROM [ced].[LastRevision]", con);
                cmd.ExecuteNonQuery();

                cmd = new SqlCommand("INSERT INTO [ced].[LastRevision] (RevisionTime) VALUES (GETDATE())", con);
                cmd.ExecuteNonQuery();

                result = DbUtil.GetLastId(con);
            });

            return result;
        }
    }
}
