// <remarks>
// Copyright 2008 - Steve Stanton. This file is part of Backsight
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

namespace Backsight.Data
{
    public partial class BacksightDataSet
    {
        partial class SchemaRow : IEditTable
        {
            public override string ToString()
            {
                return TableName;
            }

            public int Id
            {
                get { return SchemaId; }
            }

            public void FinishEdit()
            {
                if (IsAdded(this))
                    this.EndEdit();
                else
                    this.tableSchema.AddSchemaRow(this);
            }

            public static SchemaRow CreateSchemaRow(BacksightDataSet ds)
            {
                SchemaRow result = ds.Schema.NewSchemaRow();
                result.SetDefaultValues();
                return result;
            }

            internal void SetDefaultValues()
            {
                SchemaId = 0;
                TableName = String.Empty;
                IdColumnName = String.Empty;
            }

            /// <summary>
            /// Any text formatting templates associated with this table (may be an empty array)
            /// </summary>
            public ITemplate[] Templates
            {
                get
                {
                    SchemaTemplateRow[] temps = GetSchemaTemplateRows();
                    List<ITemplate> result = new List<ITemplate>(temps.Length);

                    foreach (SchemaTemplateRow t in temps)
                        result.Add(t.TemplateRow);

                    return result.ToArray();
                }
            }

            /// <summary>
            /// Any domains associated with columns in the table
            /// </summary>
            public IColumnDomain[] ColumnDomains
            {
                get { return GetColumnDomainRows(); }
                set
                {
                    if (value==null)
                        throw new ArgumentNullException();

                    // Grab the current table -> domain associations
                    BacksightDataSet ds = GetDataSet(this);
                    ColumnDomainDataTable tab = ds.ColumnDomain;
                    ColumnDomainRow[] cds = tab.FindByTableId(this.Id);

                    // Insert new associations
                    foreach (IColumnDomain cd in value)
                    {
                        if (!ColumnDomainRow.HasMatchInArray(cd, cds))
                            tab.AddColumnDomainRow(cd);
                    }

                    // Remove any associations that no longer apply
                    foreach (ColumnDomainRow row in cds)
                    {
                        int tableId = row.TableId;
                        int domainTableId = row.DomainId;
                        string columnName = row.ColumnName;

                        if (!Array.Exists<IColumnDomain>(value, delegate(IColumnDomain t)
                        {
                            int tt = (t.ParentTable == null ? 0 : t.ParentTable.Id);
                            if (tt != tableId)
                                return false;

                            int td = (t.Domain == null ? 0 : t.Domain.Id);
                            if (td != domainTableId)
                                return false;

                            return (columnName == t.ColumnName);
                        }))
                            row.Delete();
                    }
                }
            }
        }
    }
}
