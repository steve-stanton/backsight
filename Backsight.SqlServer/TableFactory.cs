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
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Data;
using System.Collections.Generic;

using Smo=Microsoft.SqlServer.Management.Smo;

using Backsight.Data;

namespace Backsight.SqlServer
{
    /// <written by="Steve Stanton" on="27-MAR-2007" />
    /// <summary>
    /// Creates Backsight system tables
    /// </summary>
    public class TableFactory
    {
        public delegate void OnTableCreate(string tableName);

        #region Class data

        /// <summary>
        /// The database the tables will be created in.
        /// </summary>
        private readonly Smo.Database m_Database;

        #endregion

        #region Constructors

        public TableFactory(Smo.Database db)
        {
            if (db==null)
                throw new ArgumentNullException();

            m_Database = db;
        }

        #endregion

        /// <summary>
        /// Creates a database table for each DataTable that's part of a BacksightDataSet.
        /// Does not define any foreign key constraints (after loading, you should call
        /// CreateForeignKeyConstraints)
        /// </summary>
        /// <param name="handler"></param>
        public void CreateTables(OnTableCreate handler)
        {
            DropForeignKeyConstraints();

            BacksightDataSet ds = new BacksightDataSet();
            foreach (DataTable dt in ds.Tables)
            {
                CreateTable(dt);
            }

            // Add simple checks (unfortunately, this info isn't held as part of the
            // generated DataSet, so need to explicitly identify each table involved)

            AddSimpleChecks(ds.Archive, ds.Archive.Checks);
            AddSimpleChecks(ds.Checkout, ds.Checkout.Checks);
            AddSimpleChecks(ds.DomainList, ds.DomainList.Checks);
            AddSimpleChecks(ds.Domain, ds.Domain.Checks);
            AddSimpleChecks(ds.Entity, ds.Entity.Checks);
            AddSimpleChecks(ds.EntitySchema, ds.EntitySchema.Checks);
            AddSimpleChecks(ds.Field, ds.Field.Checks);
            AddSimpleChecks(ds.Font, ds.Font.Checks);
            AddSimpleChecks(ds.IdAllocation, ds.IdAllocation.Checks);
            AddSimpleChecks(ds.IdFree, ds.IdFree.Checks);
            AddSimpleChecks(ds.IdGroup, ds.IdGroup.Checks);
            AddSimpleChecks(ds.Job, ds.Job.Checks);
            AddSimpleChecks(ds.Layer, ds.Layer.Checks);
            AddSimpleChecks(ds.Map, ds.Map.Checks);
            AddSimpleChecks(ds.MapUser, ds.MapUser.Checks);
            AddSimpleChecks(ds.Schema, ds.Schema.Checks);
            AddSimpleChecks(ds.SchemaField, ds.SchemaField.Checks);
            AddSimpleChecks(ds.SchemaTemplate, ds.SchemaTemplate.Checks);
            AddSimpleChecks(ds.Template, ds.Template.Checks);
            AddSimpleChecks(ds.Theme, ds.Theme.Checks);

            // Insert initial rows
            /*
INSERT INTO [dbo].[Archive] ([ArchiveId], [ArchiveTime], [MapId], [Version], [VersionTime], [JobId], [UserId], [PrevArchiveId])
VALUES (0, GETDATE(), 0, 0, GETDATE(), 0, 0, 0)
GO

INSERT INTO [dbo].[Entity] (EntityId, Name, IsPoint, IsLine, IsLineTopological, IsPolygon, IsText, FontId, GroupId)
VALUES (0, ' ', 'n', 'n', 'n', 'n', 'n', 0, 0 )
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

INSERT INTO [dbo].[Layer] (LayerId, Name, BaseLayerId, ThemeId, DefaultPointId, DefaultLineId, DefaultPolygonId, DefaultTextId )
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
            //CreateForeignKeyConstraints();
        }

        public void CreateForeignKeyConstraints()
        {
            BacksightDataSet ds = new BacksightDataSet();
            foreach (DataRelation dr in ds.Relations)
                AddForeignKeyConstraint(dr);
        }

        void AddSimpleChecks(DataTable dt, string[] checks)
        {
            Smo.Table t = m_Database.Tables[dt.TableName];
            if (t==null)
                throw new Exception("Cannot locate table "+dt.TableName);

            for (int i=0; i<checks.Length; i++)
            {
                string checkName = String.Format("{0}Check{1}", t.Name, i+1);
                AddCheck(t, checkName, checks[i]);
            }
        }

        void DropForeignKeyConstraints()
        {
            BacksightDataSet ds = new BacksightDataSet();
            List<Smo.ForeignKey> fks = new List<Smo.ForeignKey>();

            foreach (DataTable dt in ds.Tables)
            {
                Smo.Table t = m_Database.Tables[dt.TableName];
                if (t!=null)
                {
                    foreach (Smo.ForeignKey fk in t.ForeignKeys)
                        fks.Add(fk);
                }
            }

            foreach (Smo.ForeignKey fk in fks)
                fk.Drop();
        }

        void CreateTable(DataTable dt)
        {
            // Drop any previously created version of the table
            Smo.Table t = m_Database.Tables[dt.TableName];
            if (t!=null)
                t.Drop();

            // Create the table
            t = new Smo.Table(m_Database, dt.TableName);
            foreach (DataColumn c in dt.Columns)
                AddColumn(t, c);

            t.Create();

            // Define primary key
            CreatePrimaryKey(t, dt);

            // Define pk & any unique constraints
            CreateIndexes(t, dt);
        }

        Smo.Column AddColumn(Smo.Table t, string columnName, Smo.DataType dt, bool isNullable)
        {
            Smo.Column c = new Smo.Column(t, columnName, dt);
            t.Columns.Add(c);
            c.Nullable = isNullable;
            return c;
        }

        Smo.Index CreatePrimaryKey(Smo.Table t, DataTable dt)
        {
            DataColumn[] pk = dt.PrimaryKey;
            if (pk==null || pk.Length==0)
                return null;

            Smo.Index idx = new Smo.Index(t, t.Name+"Key");
            idx.IndexKeyType = Smo.IndexKeyType.DriPrimaryKey;
            SetUniqueIndexColumns(idx, pk);
            idx.Create();
            return idx;
        }

        void SetUniqueIndexColumns(Smo.Index idx, DataColumn[] dc)
        {
            idx.IsUnique = true;
            foreach (DataColumn c in dc)
            {
                Smo.IndexedColumn ic = new Smo.IndexedColumn(idx, c.ColumnName);
                idx.IndexedColumns.Add(ic);
            }
        }

        void CreateIndexes(Smo.Table t, DataTable dt)
        {
            int indexNum = 0;

            foreach (Constraint c in dt.Constraints)
            {
                if (c is UniqueConstraint)
                {
                    indexNum++;
                    CreateUniqueIndex(t, (UniqueConstraint)c, indexNum);
                }
            }
        }

        Smo.Index CreateUniqueIndex(Smo.Table t, UniqueConstraint uc, int indexNum)
        {
            if (uc.IsPrimaryKey)
                return null;

            Smo.Index idx = new Smo.Index(t, t.Name+"Index"+indexNum);
            idx.IndexKeyType = Smo.IndexKeyType.DriUniqueKey;
            SetUniqueIndexColumns(idx, uc.Columns);
            idx.Create();
            return idx;
        }

        void AddForeignKeyConstraint(DataRelation dr)
        {
            DataColumn[] parent = dr.ParentColumns;
            DataColumn[] child = dr.ChildColumns;

            if (parent.Length!=1 || child.Length!=1)
                throw new Exception("Unsupported data relation: "+dr.RelationName);

            Smo.Table pt = m_Database.Tables[dr.ParentTable.TableName];
            if (pt==null)
                throw new Exception("Cannot locate parent table for relation: "+dr.RelationName);

            Smo.Table ct = m_Database.Tables[dr.ChildTable.TableName];
            if (ct==null)
                throw new Exception("Cannot locate child table for relation: "+dr.RelationName);

            Smo.ForeignKey fk = new Smo.ForeignKey(ct, dr.RelationName);
            Smo.ForeignKeyColumn fkc = new Smo.ForeignKeyColumn(fk, child[0].ColumnName, parent[0].ColumnName);
            fk.Columns.Add(fkc);
            fk.ReferencedTable = pt.Name;
            fk.Create();
        }

        Smo.DataType GetDataType(DataColumn dc)
        {
            Type t = dc.DataType;

            if (t == typeof(System.Int32))
                return Smo.DataType.Int;

            if (t == typeof(System.Int16))
                return Smo.DataType.SmallInt;

            if (t == typeof(System.DateTime))
                return Smo.DataType.DateTime;

            if (t == typeof(System.String))
            {
                if (dc.MaxLength==1)
                    return Smo.DataType.Char(1);
                else
                    return Smo.DataType.VarChar(dc.MaxLength);
            }

            if (t == typeof(System.Double))
                return Smo.DataType.Float;

            if (t == typeof(System.Single))
                return Smo.DataType.Real;

            throw new Exception("Unsupported data type: "+t.ToString());
        }

        Smo.Column AddColumn(Smo.Table t, DataColumn dc)
        {
            Smo.DataType dt = GetDataType(dc);
            return AddColumn(t, dc.ColumnName, dt, dc.AllowDBNull);
        }

        void AddCheck(Smo.Table t, string checkName, string check)
        {
            // String.Format("ALTER TABLE {0} WITH CHECK ADD CONSTRAINT {1} CHECK ({2})",
            //                    t.Name, checkName, check);

            Smo.Check ck = new Smo.Check();
            ck.Name = checkName;
            ck.Parent = t;
            ck.Text = check;
            ck.IsEnabled = ck.IsChecked = true;
            ck.Create();
            t.Alter();
        }

        public string ConnectionString
        {
            get
            {
                return String.Format("{0};Initial Catalog={1}",
                    m_Database.Parent.ConnectionContext.ConnectionString, m_Database.Name);
            }
        }

        public SqlConnection CreateConnection()
        {
            string constr = this.ConnectionString;
            return new SqlConnection(constr);
        }
    }
}
