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
using System.Data;
using System.Data.SqlClient;
using System.Transactions;

using Backsight.Data.BacksightDataSetTableAdapters;

namespace Backsight.Data
{
    public partial class BacksightDataSet
    {
        const string YES = "y";
        const string NO = "n";

        static string AsString(bool b)
        {
            return (b==true ? YES : NO);
        }

        public void Save(string connectionString)
        {
            try
            {
                IDataServer ds = new DataServer(connectionString);
                ds.RunTransaction(delegate
                {
                    ds.CreateAdapter<ColumnDomainTableAdapter>().Update(this.ColumnDomain);
                    ds.CreateAdapter<DomainTableTableAdapter>().Update(this.DomainTable);
                    ds.CreateAdapter<FontTableAdapter>().Update(this.Font);
                    ds.CreateAdapter<IdGroupTableAdapter>().Update(this.IdGroup);
                    ds.CreateAdapter<EntityTypeTableAdapter>().Update(this.EntityType);
                    ds.CreateAdapter<ThemeTableAdapter>().Update(this.Theme);
                    ds.CreateAdapter<LayerTableAdapter>().Update(this.Layer);
                    ds.CreateAdapter<SysIdTableAdapter>().Update(this.SysId);
                    ds.CreateAdapter<PropertyTableAdapter>().Update(this.Property);
                    ds.CreateAdapter<SchemaTableAdapter>().Update(this.Schema);
                    ds.CreateAdapter<TemplateTableAdapter>().Update(this.Template);
                    ds.CreateAdapter<SchemaTemplateTableAdapter>().Update(this.SchemaTemplate);
                    ds.CreateAdapter<EntityTypeSchemaTableAdapter>().Update(this.EntityTypeSchema);
                    ds.CreateAdapter<ZoneTableAdapter>().Update(this.Zone);
                });
            }

            finally
            {
                AcceptChanges();
            }
        }

        public void Load(string connectionString)
        {
            IDataServer ds = new DataServer(connectionString);

            ds.RunTransaction(delegate
            {
                ColumnDomainTableAdapter columnDomain = ds.CreateAdapter<ColumnDomainTableAdapter>();
                DomainTableTableAdapter domainTable = ds.CreateAdapter<DomainTableTableAdapter>();
                FontTableAdapter font = ds.CreateAdapter<FontTableAdapter>();
                IdGroupTableAdapter idGroup = ds.CreateAdapter<IdGroupTableAdapter>();
                EntityTypeTableAdapter entity = ds.CreateAdapter<EntityTypeTableAdapter>();
                ThemeTableAdapter theme = ds.CreateAdapter<ThemeTableAdapter>();
                LayerTableAdapter layer = ds.CreateAdapter<LayerTableAdapter>();
                SysIdTableAdapter sysid = ds.CreateAdapter<SysIdTableAdapter>();
                PropertyTableAdapter prop = ds.CreateAdapter<PropertyTableAdapter>();
                SchemaTableAdapter schema = ds.CreateAdapter<SchemaTableAdapter>();
                TemplateTableAdapter template = ds.CreateAdapter<TemplateTableAdapter>();
                SchemaTemplateTableAdapter schemaTemplate = ds.CreateAdapter<SchemaTemplateTableAdapter>();
                EntityTypeSchemaTableAdapter entityTypeSchema = ds.CreateAdapter<EntityTypeSchemaTableAdapter>();
                ZoneTableAdapter zone = ds.CreateAdapter<ZoneTableAdapter>();

                columnDomain.Fill(this.ColumnDomain);
                domainTable.Fill(this.DomainTable);
                font.Fill(this.Font);
                idGroup.Fill(this.IdGroup);
                entity.Fill(this.EntityType);
                theme.Fill(this.Theme);
                layer.Fill(this.Layer);
                sysid.Fill(this.SysId);
                prop.Fill(this.Property);
                schema.Fill(this.Schema);
                template.Fill(this.Template);
                schemaTemplate.Fill(this.SchemaTemplate);
                entityTypeSchema.Fill(this.EntityTypeSchema);
                zone.Fill(this.Zone);
            });
        }

        /// <summary>
        /// Has a row been added to a table?
        /// </summary>
        /// <param name="row">The row to check</param>
        /// <returns>True if the supplied row has any state other than <c>DataRowState.Detached</c></returns>
        static bool IsAdded(DataRow row)
        {
            return (row.RowState != DataRowState.Detached);
        }

        /// <summary>
        /// Adds rows that should exist in an empty Backsight environment.
        /// </summary>
        /*
        public void AddInitialRows()
        {
            // Insert initial values for accommodating foreign key constraints
            Font.AddEmptyRow();
            IdGroup.AddEmptyRow();
            EntityType.AddEmptyRow();
            Theme.AddEmptyRow();
            Layer.AddEmptyRow();

            // And for generating environment-related IDs
            SysId.AddEmptyRow();
        }
        */

        /// <summary>
        /// Returns the data set for the supplied row (expected to be an item in
        /// some instance of <c>BacksightDataSet</c>). This method exists only because
        /// I can't see any auto-generated method to return the dataset for a row, and
        /// I don't want to sprinkle casts around everywhere.
        /// </summary>
        /// <param name="row">The row of interest</param>
        /// <returns>The enclosing dataset</returns>
        internal static BacksightDataSet GetDataSet(DataRow row)
        {
            return (BacksightDataSet)row.Table.DataSet;
        }
    }
}
