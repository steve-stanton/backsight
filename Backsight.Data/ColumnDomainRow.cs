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
using System.Data;

using Backsight.Environment;

namespace Backsight.Data
{
    public partial class BacksightDataSet
    {
        public partial class ColumnDomainRow : IEditColumnDomain
        {
            /// <summary>
            /// Checks an array of <c>ColumnDomainRow</c> to see if any element refers
            /// to a specific database column and domain
            /// </summary>
            /// <param name="cd">The item to look for</param>
            /// <param name="rows">The rows to check</param>
            /// <returns>True if any row matches the search item</returns>
            /// <remarks>This helps to avoid the clumsy syntax of Array.Find, making
            /// calling code a bit more readable.</remarks>
            internal static bool HasMatchInArray(IColumnDomain cd, ColumnDomainRow[] rows)
            {
                int tableId = (cd.ParentTable == null ? 0 : cd.ParentTable.Id);
                int domainTableId = (cd.Domain == null ? 0 : cd.Domain.Id);
                string columnName = cd.ColumnName;

                foreach (ColumnDomainRow row in rows)
                {
                    if (row.TableId == tableId &&
                        row.DomainId == domainTableId &&
                        row.ColumnName == columnName)
                        return true;
                }

                return false;
            }

            #region Class data

            #endregion

            public override string ToString()
            {
                return String.Format("{0} -> {??}", ColumnName);
            }

            public void FinishEdit()
            {
                if (IsAdded(this))
                    this.EndEdit();
                else
                    this.tableColumnDomain.AddColumnDomainRow(this);
            }

            public static ColumnDomainRow CreateColumnDomainRow(BacksightDataSet ds)
            {
                ColumnDomainRow result = ds.ColumnDomain.NewColumnDomainRow();
                result.SetDefaultValues();
                return result;
            }

            internal void SetDefaultValues()
            {
                TableId = 0;
                ColumnName = String.Empty;
                DomainId = 0;
            }

            #region IEditColumnDomain Members

            public ITable ParentTable
            {
                get
                {
                    if (TableId == 0)
                        return null;
                    else
                        return this.SchemaRow;
                }

                set
                {
                    if (value == null)
                        TableId = 0;
                    else
                        TableId = value.Id;
                }
            }

            public IDomainTable Domain
            {
                get
                {
                    if (DomainId == 0)
                        return null;
                    else
                        return this.DomainTableRow;
                }

                set
                {
                    if (value == null)
                        DomainId = 0;
                    else
                        DomainId = value.Id;
                }
            }

            #endregion
        }
    }
}
