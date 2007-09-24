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
using System.Diagnostics;

using Microsoft.SqlServer.Management.Common;
using Smo=Microsoft.SqlServer.Management.Smo;

using Backsight.Data;
using Backsight.Environment;

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

        /// <summary>
        /// Creates a new <c>TableFactory</c> for the database identified via the supplied
        /// connection string.
        /// </summary>
        /// <param name="connectionString">The connection string, identifying the server and
        /// database that the factory is for.</param>
        public TableFactory(string connectionString)
        {
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder(connectionString);

            // For some reason, the following doesn't initialize the Database object completely
            // (even calling SetDefaultInitFields). But it works as expected if you first connect
            // using a ServerConnection.
      
            //Smo.Server s = new Smo.Server(csb.DataSource);
            //s.SetDefaultInitFields(typeof(Smo.Table), true);
            //m_Database = new Smo.Database(s, csb.InitialCatalog);

            ServerConnection sc = new ServerConnection();
            sc.ServerInstance = csb.DataSource;
            sc.ConnectTimeout = 5;
            sc.LoginSecure = csb.IntegratedSecurity;
            if (!sc.LoginSecure)
            {
                sc.Login = csb.UserID;
                sc.Password = csb.Password;
            }

            sc.Connect();
            string dbName = csb.InitialCatalog;
            m_Database = new Smo.Server(sc).Databases[dbName];
        }

        /// <summary>
        /// Creates a new <c>TableFactory</c> for the database that is defined through
        /// the <see cref="AdapterFactory.ConnectionString"/> property.
        /// </summary>
        public TableFactory()
            : this(AdapterFactory.ConnectionString)
        {
        }

        #endregion

        /// <summary>
        /// Checks whether the database contains Backsight system tables.
        /// </summary>
        /// <returns>True if the entity table exists</returns>
        public bool DoTablesExist()
        {
            string testTable = new BacksightDataSet().Entity.TableName;
            return m_Database.Tables.Contains(testTable);
        }

        /// <summary>
        /// Creates a database table for each DataTable that's part of a BacksightDataSet,
        /// inserts initial rows, and defines constraints.
        /// </summary>
        /// <param name="logger">Something to display progress messages (not null)</param>
        public void CreateTables(ILog logger)
        {
            if (logger==null)
                throw new ArgumentNullException();

            DropForeignKeyConstraints();

            BacksightDataSet ds = new BacksightDataSet();
            foreach (DataTable dt in ds.Tables)
            {
                logger.LogMessage("CREATE TABLE "+dt.TableName);
                CreateTable(dt);
            }

            // Add simple checks (unfortunately, this info isn't held as part of the
            // generated DataSet, so need to explicitly identify each table involved)

            AddSimpleChecks(logger, ds.Archive, ds.Archive.Checks);
            AddSimpleChecks(logger, ds.Checkout, ds.Checkout.Checks);
            AddSimpleChecks(logger, ds.DomainList, ds.DomainList.Checks);
            AddSimpleChecks(logger, ds.Domain, ds.Domain.Checks);
            AddSimpleChecks(logger, ds.Entity, ds.Entity.Checks);
            AddSimpleChecks(logger, ds.EntitySchema, ds.EntitySchema.Checks);
            AddSimpleChecks(logger, ds.Field, ds.Field.Checks);
            AddSimpleChecks(logger, ds.Font, ds.Font.Checks);
            AddSimpleChecks(logger, ds.IdAllocation, ds.IdAllocation.Checks);
            AddSimpleChecks(logger, ds.IdFree, ds.IdFree.Checks);
            AddSimpleChecks(logger, ds.IdGroup, ds.IdGroup.Checks);
            AddSimpleChecks(logger, ds.Job, ds.Job.Checks);
            AddSimpleChecks(logger, ds.Layer, ds.Layer.Checks);
            AddSimpleChecks(logger, ds.Map, ds.Map.Checks);
            AddSimpleChecks(logger, ds.MapUser, ds.MapUser.Checks);
            AddSimpleChecks(logger, ds.Schema, ds.Schema.Checks);
            AddSimpleChecks(logger, ds.SchemaField, ds.SchemaField.Checks);
            AddSimpleChecks(logger, ds.SchemaTemplate, ds.SchemaTemplate.Checks);
            AddSimpleChecks(logger, ds.Template, ds.Template.Checks);
            AddSimpleChecks(logger, ds.Theme, ds.Theme.Checks);

            // Insert initial rows & save to database
            logger.LogMessage("Inserting initial rows");
            ds.AddInitialRows();
            ds.Save(ConnectionString);

            // Define foreign key constraints
            AddForeignKeyConstraints(logger);
        }

        void AddSimpleChecks(ILog logger, DataTable dt, string[] checks)
        {
            Smo.Table t = m_Database.Tables[dt.TableName];
            if (t==null)
                throw new Exception("Cannot locate table "+dt.TableName);

            for (int i=0; i<checks.Length; i++)
            {
                string checkName = String.Format("{0}Check{1}", t.Name, i+1);
                logger.LogMessage("ADD CHECK "+checkName);
                AddCheck(t, checkName, checks[i]);
            }
        }

        void AddForeignKeyConstraints(ILog logger)
        {
            BacksightDataSet ds = new BacksightDataSet();

            foreach (DataRelation dr in ds.Relations)
            {
                DataTable parent = dr.ParentTable;
                DataTable child = dr.ChildTable;

                Smo.Table parentTable = m_Database.Tables[parent.TableName];
                Smo.Table childTable = m_Database.Tables[child.TableName];

                if (parentTable!=null && childTable!=null)
                {
                    if (logger!=null)
                        logger.LogMessage("ADD CONSTRAINT "+dr.RelationName);

                    AddForeignKeyConstraint(dr);
                }
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

        /// <summary>
        /// The connection string for the database this factory is for.
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return String.Format("{0};Initial Catalog={1}",
                    m_Database.Parent.ConnectionContext.ConnectionString, m_Database.Name);
            }
        }

        /// <summary>
        /// Enable or disable foreign key constraints for Backsight system tables.
        /// </summary>
        /// <param name="enable">Specify <c>false</c> to enable constraints, <c>true</c>
        /// to re-enable.</param>
        public void EnableForeignKeys(bool enable)
        {
            // Do it the heavy-handed way, since I'm still getting foreign key errors
            // after setting ForeignKey.IsEnabled to false.

            if (enable)
                AddForeignKeyConstraints(null);
            else
                DropForeignKeyConstraints();

            /*
            BacksightDataSet ds = new BacksightDataSet();

            foreach (DataTable dt in ds.Tables)
            {
                Smo.Table t = m_Database.Tables[dt.TableName];
                if (t!=null)
                {
                    foreach (Smo.ForeignKey fk in t.ForeignKeys)
                        fk.IsEnabled = enable;
                }
            }
            */
        }

        /// <summary>
        /// Removes all data from all Backsight tables. This is done when a new environment
        /// is being imported. You will probably need to bracket this call with calls
        /// to <see cref="EnableForeignKeys"/> (disable foreign key constraints, then
        /// remove, then import, then re-enable constraints).
        /// </summary>
        public void RemoveAll()
        {
            BacksightDataSet ds = new BacksightDataSet();

            using (IConnection ic = AdapterFactory.GetConnection())
            {
                SqlConnection c = ic.Value;

                foreach (DataTable dt in ds.Tables)
                {
                    // Not sure why, but it complains if you don't specify the table schema
                    // at this point. Assume "dbo", since I don't see any DataTable.SchemaName
                    // property.
                    string sql = String.Format("DELETE FROM [{0}].[{1}]", "dbo", dt.TableName);
                    SqlCommand cmd = new SqlCommand(sql, c);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Imports environment data from the specified dataset. Prior to call, you should
        /// first call <see cref="RemoveAll"/>.
        /// </summary>
        /// <param name="ds">The dataset to import</param>
        public void Import(BacksightDataSet ds)
        {
            using (IConnection ic = AdapterFactory.GetConnection())
            {
                SqlConnection c = ic.Value;

                foreach (DataTable dt in ds.Tables)
                {
                    if (dt.Rows.Count > 0)
                    {
                        SqlBulkCopy bc = new SqlBulkCopy(c);
                        bc.DestinationTableName = dt.TableName;
                        bc.BatchSize = dt.Rows.Count;
                        bc.WriteToServer(dt);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the names of all tables in the database this factory is associated with.
        /// </summary>
        /// <returns>The names of all tables in the database</returns>
        public string[] GetTableNames()
        {
            Smo.TableCollection tc = m_Database.Tables;
            List<string> names = new List<string>(tc.Count);

            foreach (Smo.Table t in tc)
                names.Add(t.Name);

            return names.ToArray();
        }
    }
}
