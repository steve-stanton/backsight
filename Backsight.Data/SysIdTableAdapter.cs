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

namespace Backsight.Data.BacksightDataSetTableAdapters
{
    partial class SysIdTableAdapter
    {
        SqlCommand m_Update;

        public virtual int UpdateQuery(int NewValue, int OldValue)
        {
            if (m_Update==null)
            {
                m_Update = new SqlCommand();
                m_Update.CommandText = "UPDATE SysId set LastId=@NewValue where LastId=@OldValue";
                m_Update.CommandType = System.Data.CommandType.Text;
                m_Update.Parameters.Add(new SqlParameter("@NewValue", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, 0, 0, "LastId", System.Data.DataRowVersion.Current, false, null, "", "", ""));
                m_Update.Parameters.Add(new SqlParameter("@OldValue", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, 0, 0, "LastId", System.Data.DataRowVersion.Original, false, null, "", "", ""));
            }

            SqlCommand command = m_Update;
            command.Parameters[0].Value = ((int)(NewValue));
            command.Parameters[1].Value = ((int)(OldValue));
            System.Data.ConnectionState previousConnectionState = command.Connection.State;
            if (((command.Connection.State & System.Data.ConnectionState.Open) 
                        != System.Data.ConnectionState.Open))
            {
                command.Connection.Open();
            }
            int returnValue;
            try
            {
                returnValue = command.ExecuteNonQuery();
            }
            finally
            {
                if ((previousConnectionState == System.Data.ConnectionState.Closed))
                {
                    command.Connection.Close();
                }
            }
            return returnValue;
        }
    }
}
