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
                SetAllRowsAdded();

                AdapterFactory.ConnectionString = connectionString;
                Transaction.Execute(delegate
                {
                    FontTableAdapter font = AdapterFactory.Create<FontTableAdapter>();
                    IdGroupTableAdapter idGroup = AdapterFactory.Create<IdGroupTableAdapter>();
                    EntityTableAdapter entity = AdapterFactory.Create<EntityTableAdapter>();
                    ThemeTableAdapter theme = AdapterFactory.Create<ThemeTableAdapter>();
                    LayerTableAdapter layer = AdapterFactory.Create<LayerTableAdapter>();

                    // Ensure we've added blank rows first (needed to maintain foreign
                    // key constraints)

                    //SaveEmptyRows();
                    /*

                    INSERT INTO [dbo].[Archive] ([ArchiveId], [ArchiveTime], [MapId], [Version], [VersionTime], [JobId], [UserId], [PrevArchiveId])
                    VALUES (0, GETDATE(), 0, 0, GETDATE(), 0, 0, 0)
                    GO

                    INSERT INTO [dbo].[Edition] ([EditionId], [Name], [Status])
                    VALUES (0, ' ', 'empty')
                    GO

                    INSERT INTO [dbo].[Entity] (EntityId, Name, IsPoint, IsLine, IsLineTopological, IsPolygon, IsText, FontId, LayerId, GroupId)
                    VALUES (0, ' ', 'n', 'n', 'n', 'n', 'n', 0, 0, 0 )
                    GO

                    INSERT INTO [dbo].[Font] ([FontId], [TypeFace], [PointSize], [IsBold], [IsItalic], [IsUnderline], [FontFile])
                    VALUES (0, ' ', 0.0, 'n', 'n', 'n', ' ')
                    GO

                    INSERT INTO [dbo].[IdGroup] (GroupId, Name, LowestId, HighestId, PacketSize, CheckDigit, KeyFormat)
                    VALUES (0, ' ', 1, 1, 1, 'n', '')
                    GO

                    INSERT INTO [dbo].[Job] ([JobId], [Name], [IsActive])
                    VALUES (0, ' ', 'n')
                    GO

                    INSERT INTO [dbo].[Layer] ([LayerId], [Name], [ThemeId], [ThemeSequence], [DefaultPointId], [DefaultLineId], [DefaultPolygonId], [DefaultTextId])
                    VALUES (0, ' ', 0, 0, 0, 0, 0, 0)
                    GO

                    INSERT INTO [dbo].[Map] ([MapId], [Name], [Version], [VersionTime], [JobId], [UserId], [ArchiveId], [Status], [IsActive])
                    VALUES (0, ' ', 0, GETDATE(), 0, 0, 0, ' ', 'n')
                    GO

                    INSERT INTO [dbo].[MapUser] ([UserId], [Name], [IsActive], [CanCheckout])
                    VALUES (0, ' ', 'n', 'n')
                    GO

                    INSERT INTO [dbo].[Theme] (ThemeId, Name)
                    VALUES (0, ' ')
                    GO

                    INSERT INTO [dbo].[SysId] (LastId) VALUES (0)
                    GO

                     */
                    font.Update(this.Font);
                    idGroup.Update(this.IdGroup);
                    entity.Update(this.Entity);
                    theme.Update(this.Theme);
                    layer.Update(this.Layer);
                });
            }

            finally
            {
                //this.EnforceConstraints = true;
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

        public string ConnectionString
        {
            get { return Backsight.Data.Properties.Settings.Default.BacksightConnectionString; }
            set
            {
                Backsight.Data.Properties.Settings.Default.BacksightConnectionString = value;
                Backsight.Data.Properties.Settings.Default.Save();
            }
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
        public void AddInitialRows()
        {
            // Insert initial values for accommodating foreign key constraints (the order
            // of calls is significant in terms of these constraints)
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
