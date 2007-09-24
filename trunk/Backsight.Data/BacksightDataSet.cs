/// <remarks>
/// Copyright 2007 - Steve Stanton. This file is part of Backsight
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
                AdapterFactory.ConnectionString = connectionString;
                Transaction.Execute(delegate
                {
                    AdapterFactory.Create<FontTableAdapter>().Update(this.Font);
                    AdapterFactory.Create<IdGroupTableAdapter>().Update(this.IdGroup);
                    AdapterFactory.Create<EntityTableAdapter>().Update(this.Entity);
                    AdapterFactory.Create<ThemeTableAdapter>().Update(this.Theme);
                    AdapterFactory.Create<LayerTableAdapter>().Update(this.Layer);
                });
            }

            finally
            {
                AcceptChanges();
            }
        }

        public void Load()
        {
            using (IConnection ic = AdapterFactory.GetConnection())
            {
                FontTableAdapter font = new FontTableAdapter();
                IdGroupTableAdapter idGroup = new IdGroupTableAdapter();
                EntityTableAdapter entity = new EntityTableAdapter();
                ThemeTableAdapter theme = new ThemeTableAdapter();
                LayerTableAdapter layer = new LayerTableAdapter();

                font.Connection =
                idGroup.Connection =
                entity.Connection =
                theme.Connection =
                layer.Connection = ic.Value;

                font.Fill(this.Font);
                idGroup.Fill(this.IdGroup);
                entity.Fill(this.Entity);
                theme.Fill(this.Theme);
                layer.Fill(this.Layer);
            }
        }

        /*
        void SetAllRowsAdded()
        {
            foreach (DataTable dt in this.Tables)
            {
                foreach (DataRow row in dt.Rows)
                {
                    row.SetAdded();
                }
            }
        }
        */

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
        public void AddInitialRows()
        {
            // Insert initial values for accommodating foreign key constraints
            Font.AddEmptyRow();
            IdGroup.AddEmptyRow();
            Entity.AddEmptyRow();
            Theme.AddEmptyRow();
            Layer.AddEmptyRow();

            // And for generating environment-related IDs
            SysId.AddEmptyRow();
        }

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
