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
                Name = String.Empty;
                TableName = String.Empty;
            }

            /// <summary>
            /// The name of the column that corresponds to the user-perceived ID
            /// associated with spatial features.
            /// </summary>
            public string FeatureIdColumnName
            {
                get { return "PIN"; }
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
            /// Any domain tables associated with columns in the table.
            /// </summary>
            public IDomainTable[] DomainTables
            {
                get
                {
                    TableDomainRow[] tds = GetTableDomainRows();
                    List<IDomainTable> result = new List<IDomainTable>(tds.Length);

                    foreach (TableDomainRow t in tds)
                        result.Add(t.DomainTablesRow);

                    return result.ToArray();
                }
            }

            /// <summary>
            /// Any domains associated with columns in the table
            /// </summary>
            public IColumnDomain[] Domains
            {
                get { return GetTableDomainRows(); }
            }
        }
    }
}
