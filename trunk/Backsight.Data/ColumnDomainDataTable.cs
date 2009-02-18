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

using Backsight.Environment;

namespace Backsight.Data
{
    public partial class BacksightDataSet
    {
        partial class ColumnDomainDataTable
        {
            /// <summary>
            /// Locates rows that refer to a specific table
            /// </summary>
            /// <param name="tableId">The ID of the table to look for</param>
            /// <returns>The rows that match the specified table ID</returns>
            internal ColumnDomainRow[] FindByTableId(int tableId)
            {
                BacksightDataSet ds = (BacksightDataSet)this.DataSet;
                string query = String.Format("{0}={1}", columnTableId.ColumnName, tableId);
                return (ColumnDomainRow[])Select(query);
            }

            /// <summary>
            /// Creates a new row in the <c>ColumnDomains</c> table
            /// </summary>
            /// <param name="cd">Information for the row to add</param>
            /// <returns>The row inserted into this table</returns>
            internal ColumnDomainRow AddColumnDomainRow(IColumnDomain cd)
            {
                ColumnDomainRow result = NewColumnDomainRow();
                result.TableId = (cd.ParentTable == null ? 0 : cd.ParentTable.Id);
                result.ColumnName = cd.ColumnName;
                result.DomainId = (cd.Domain == null ? 0 : cd.Domain.Id);
                AddColumnDomainRow(result);
                return result;
            }
        }
    }
}
