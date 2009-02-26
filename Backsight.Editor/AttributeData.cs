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
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Data.SqlClient;

using Backsight.Environment;
using Backsight.Data;
using Backsight.Editor.Database;

namespace Backsight.Editor
{
    /// <summary>
    /// Miscellaneous attributes associated with spatial features.
    /// </summary>
    static class AttributeData
    {
        /// <summary>
        /// Attaches miscellaneous attribute data to features
        /// </summary>
        /// <param name="features">The features to process (those that don't have a feature ID
        /// will be ignored)</param>
        /// <returns>The number of rows that were found (-1 if no database tables have
        /// been associated with Backsight)</returns>
        internal static int Load(Feature[] features)
        {
            List<FeatureId> fids = new List<FeatureId>(features.Length);
            foreach (Feature f in features)
            {
                FeatureId fid = f.Id;
                if (fid!=null)
                    fids.Add(fid);
            }

            if (fids.Count == 0)
                return 0;
            else
                return Load(fids.ToArray());
        }

        /// <summary>
        /// Attaches miscellaneous attribute data to the features that have been loaded.
        /// </summary>
        /// <param name="fids">The feature IDs to look for</param>
        /// <returns>The number of rows that were found (-1 if no database tables have
        /// been associated with Backsight)</returns>
        /// <remarks>
        /// The current Backsight implementation deals primarily with the
        /// definition of the geometry for spatial features. While it is intended to
        /// provide basic attribute data entry, the overall design calls for a very
        /// loose binding.
        /// <para/>
        /// To cover this design goal, there are no references to miscellaneous attributes
        /// in any editing operation. This makes it possible to manipulate the attributes
        /// using external systems, with minimal concern for the impact it could have
        /// on Backsight (the only consequence of inadvertant attribute changes is
        /// that instances of <see cref="RowTextGeometry"/> could be orphaned by
        /// removing the associated attributes).
        /// <para/>
        /// While this simplifies the overall architecture, it is advisable to
        /// make any attribute data easily available to the user, since that may well
        /// guide the user regarding the relevance of spatial edits.
        /// <para/>
        /// This method will be called after the spatial features for a job have been
        /// loaded from the database. It takes a very simple-minded approach, by attempting
        /// to match features with every table associated with Backsight via the
        /// Environment Editor application (hopefully there aren't TOO many).
        /// This could potentially be overly time-consuming as part of the loading logic.
        /// While some of this could be addressed by lazy loading, or perhaps some more
        /// definitive layer-&gt;table associations, there is no proof that there is actually
        /// an issue that needs solving. Without that proof, it is considered inappropriate
        /// to code more complicated solutions that are based on heresay.
        /// </remarks>
        internal static int Load(FeatureId[] fids)
        {
            // Locate information about the tables associated with Backsight
            ITable[] tables = EnvironmentContainer.Current.Tables;
            if (tables.Length == 0)
                return -1;

            // Copy the required keys into a temp table
            Trace.WriteLine(String.Format("Locating attributes for {0} features in {1} tables", fids.Length, tables.Length));

            // Cross-reference the supplied IDs to their formatted key (assumes that no
            // two IDs have the same formatted key)
            Dictionary<string, FeatureId> keyIds = new Dictionary<string, FeatureId>(fids.Length);
            foreach (FeatureId fid in fids)
                keyIds.Add(fid.FormattedKey, fid);

            int nFound = 0;

            using (IConnection ic = ConnectionFactory.Create())
            {
                SqlConnection c = ic.Value;
                const string KEYS_TABLE_NAME = "#Ids";
                CopyKeysToTable(c, fids, KEYS_TABLE_NAME);

                foreach (ITable t in tables)
                {
                    string sql = String.Format("SELECT * FROM {0} WHERE [{1}] IN (SELECT [FeatureId] FROM [{2}])",
                                    t.TableName, t.IdColumnName, KEYS_TABLE_NAME);
                    DataTable tab = DbUtil.ExecuteSelect(c, sql);
                    tab.TableName = t.TableName;

                    int featureIdIndex = tab.Columns[t.IdColumnName].Ordinal;
                    Debug.Assert(featureIdIndex>=0);
                    foreach (DataRow row in tab.Select())
                    {
                        string key = row[featureIdIndex].ToString().TrimEnd();
                        FeatureId fid;
                        if (keyIds.TryGetValue(key, out fid))
                        {
                            // Don't create a row if the ID is already associated with the
                            // table (this is meant to cover situations where the edit has actively
                            // formed the attributes, and is calling this method only to cover the
                            // fact that further attributes may be involved).

                            if (!fid.RefersToTable(t))
                            {
                                Row r = new Row(fid, t, row);
                                nFound++;
                            }
                        }
                        else
                        {
                            string msg = String.Format("Cannot find '{0}' in dictionary", key);
                            throw new Exception(msg);
                        }
                    }
                }
            }

            return nFound;
        }

        /// <summary>
        /// Attempts to locate any rows of attribute data that have a specific key (this involves
        /// a select on all database tables that have been associated with Backsight).
        /// </summary>
        /// <param name="key">The key to look for</param>
        /// <returns>The rows found (may be an empty array)</returns>
        internal static DataRow[] FindByKey(string key)
        {
            // Locate information about the tables associated with Backsight
            ITable[] tables = EnvironmentContainer.Current.Tables;
            if (tables.Length == 0)
                return new DataRow[0];

            List<DataRow> result = new List<DataRow>();

            using (IConnection ic = ConnectionFactory.Create())
            {
                SqlConnection c = ic.Value;

                foreach (ITable t in tables)
                {
                    string sql = String.Format("SELECT * FROM {0} WHERE [{1}]='{2}'",
                                    t.TableName, t.IdColumnName, key);
                    DataTable tab = DbUtil.ExecuteSelect(c, sql);
                    tab.TableName = t.TableName;
                    result.AddRange(tab.Select());
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Searches a specific table to see whether it contains any rows with a specific key
        /// </summary>
        /// <param name="table">The table to select from</param>
        /// <param name="key">The key to look for</param>
        /// <returns>The rows found (may be an empty array). Normally, this array should contain
        /// no more than one row - however, it is possible that the spatial key is not the primary
        /// key of the table.</returns>
        internal static DataRow[] FindByKey(ITable table, string key)
        {
            using (IConnection ic = ConnectionFactory.Create())
            {
                string sql = String.Format("SELECT * FROM {0} WHERE [{1}]='{2}'",
                                table.TableName, table.IdColumnName, key);
                DataTable tab = DbUtil.ExecuteSelect(ic.Value, sql);
                tab.TableName = table.TableName;
                return tab.Select();
            }
        }

        /// <summary>
        /// Copies an array of keys (formatted feature IDs) into a new database table.
        /// </summary>
        /// <param name="con">The database connection to use</param>
        /// <param name="keys">The keys of the features to copy in</param>
        /// <param name="tableName">The name of the table to create and load. This will
        /// typically be a temporary table (a name starting with the "#" character
        /// in SqlServer systems).</param>
        static void CopyKeysToTable(SqlConnection con, FeatureId[] keys, string tableName)
        {
            // Stick session IDs into an array of row objects
            DataTable dt = new DataTable(tableName);
            DataColumn dc = new DataColumn("FeatureId", typeof(string));
            dc.MaxLength = 16;
            dt.Columns.Add(dc);
            DataRow[] rows = new DataRow[keys.Length];

            for (int i=0; i<rows.Length; i++)
            {
                rows[i] = dt.NewRow();
                rows[i][0] = keys[i];
            }

            // Create the temp table
            string sql = String.Format("CREATE TABLE [{0}] ([FeatureId] VARCHAR(16) NOT NULL)", tableName);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.ExecuteNonQuery();

            // Bulk copy the rows into the temp table
            SqlBulkCopy bcp = new SqlBulkCopy(con);
            bcp.BatchSize = rows.Length;
            bcp.DestinationTableName = tableName;
            bcp.WriteToServer(rows);
        }
    }
}
