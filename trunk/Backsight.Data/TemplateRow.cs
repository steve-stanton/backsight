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

using Backsight.Environment;

namespace Backsight.Data
{
    public partial class BacksightDataSet
    {
        partial class TemplateRow : IEditTemplate
        {
            public override string ToString()
            {
                return Name;
            }

            public string Format
            {
                get { return TemplateFormat; }
                set { TemplateFormat = value; }
            }

            public int Id
            {
                get { return TemplateId; }
            }

            public bool IsNew
            {
                get { return !IsAdded(this); }
            }

            /// <summary>
            /// The database table the template applies to (the table may be associated with
            /// several templates).
            /// </summary>
            public ITable Schema
            {
                get
                {
                    BacksightDataSet ds = GetDataSet(this);
                    ITable[] tables = (ITable[])ds.Schema.Select();

                    return Array.Find<ITable>(tables, delegate(ITable t)
                    {
                        ITemplate[] tableTemplates = t.Templates;
                        ITemplate foundTemplate = Array.Find<ITemplate>(tableTemplates, delegate(ITemplate temp)
                                            { return temp.Id == this.TemplateId; });
                        return (foundTemplate != null);
                    });
                }

                set
                {
                    if (value==null)
                        throw new ArgumentNullException("Attempt to assign null table to template");

                    // If this template was previously associated with a table, we'll just
                    // update the association. Otherwise 
                    ITable oldTable = this.Schema;
                    SchemaTemplateDataTable tab = GetDataSet(this).SchemaTemplate;
                    SchemaTemplateRow row = null;
                    if (oldTable != null)
                        row = tab.FindBySchemaIdTemplateId(oldTable.Id, this.TemplateId);

                    if (row == null)
                    {
                        row = tab.NewSchemaTemplateRow();
                        row.SchemaId = value.Id;
                        row.TemplateId = this.TemplateId;
                        tab.AddSchemaTemplateRow(row);
                    }
                    else
                    {
                        row.SchemaId = value.Id;
                    }
                }
            }

            public void FinishEdit()
            {
                if (IsAdded(this))
                    this.EndEdit();
                else
                    this.tableTemplate.AddTemplateRow(this);
            }

            internal static TemplateRow CreateTemplateRow(BacksightDataSet ds)
            {
                TemplateRow result = ds.Template.NewTemplateRow();
                result.SetDefaultValues();
                return result;
            }

            internal void SetDefaultValues()
            {
                TemplateId = 0;
                Name = String.Empty;
                TemplateFormat = String.Empty;
            }
        }
    }
}
