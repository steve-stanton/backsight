// <remarks>
// Copyright 2009 - Steve Stanton. This file is part of Backsight
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
using System.Collections.Generic;

using Backsight.Environment;
using System.Data.SqlClient;
using System.Data;

namespace Backsight.Data
{
    public partial class BacksightDataSet
    {
        public partial class DomainTableRow : IEditDomainTable
        {
            #region Class data

            /// <summary>
            /// The data for the domain table. The key is the lookup value, the
            /// value is the expanded value. Lazy loaded on the first call to the
            /// <see cref="Lookup"/> method.
            /// </summary>
            Dictionary<string, string> m_Data;

            #endregion

            public override string ToString()
            {
                return TableName;
            }

            public void FinishEdit()
            {
                if (IsAdded(this))
                    this.EndEdit();
                else
                    this.tableDomainTable.AddDomainTableRow(this);
            }

            public static DomainTableRow CreateDomainTableRow(BacksightDataSet ds)
            {
                DomainTableRow result = ds.DomainTable.NewDomainTableRow();
                result.SetDefaultValues();
                return result;
            }

            internal void SetDefaultValues()
            {
                DomainId = 0;
                TableName = String.Empty;
            }

            public int Id
            {
                get { return DomainId; }
            }

            /// <summary>
            /// Performs a lookup on the domain table
            /// </summary>
            /// <param name="shortValue">The abbreviated code to lookup</param>
            /// <returns>The expanded value for the lookup</returns>
            public string Lookup(string shortValue)
            {
                if (m_Data == null)
                    m_Data = LoadDomainTable();

                string result;

                if (m_Data.TryGetValue(shortValue, out result))
                    return result;
                else
                    return String.Empty;
            }

            /// <summary>
            /// Loads the domain table from the database
            /// </summary>
            /// <returns>A index of the domain table, keyed by the short value</returns>
            Dictionary<string, string> LoadDomainTable()
            {
                Dictionary<string, string> result = new Dictionary<string, string>();

                using (IConnection ic = ConnectionFactory.Create())
                {
                    string sql = "SELECT [ShortValue], [LongValue] FROM " + TableName;
                    SqlCommand cmd = new SqlCommand(sql, ic.Value);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string key = reader.GetString(0);
                            string val = reader.GetString(1);
                            result.Add(key, val);
                        }
                    }
                }

                return result;
            }
        }
    }
}

