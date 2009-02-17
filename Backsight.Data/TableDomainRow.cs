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
        public partial class TableDomainRow : IEditColumnDomain
        {
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
                    this.tableTableDomain.AddTableDomainRow(this);
            }

            public static TableDomainRow CreateTableDomainRow(BacksightDataSet ds)
            {
                TableDomainRow result = ds.TableDomain.NewTableDomainRow();
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


            public IDomainTable Domain
            {
                get
                {
                    if (DomainId == 0)
                        return null;
                    else
                        return this.DomainTablesRow;
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
