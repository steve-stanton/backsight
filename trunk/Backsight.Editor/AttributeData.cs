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
    class AttributeData
    {
        #region Class data

        /// <summary>
        /// The database rows associated with spatial features.
        /// The key is the formatted version of the feature ID (expected to be no longer than
        /// 16 characters), while the value is the collection of database rows with that key.
        /// <para/>
        /// Spatial features that have no associated attributes will not appear in this
        /// collection. For a spatial feature that does have attributes, rows can come from any
        /// number of tables. Multiple rows may also come from a single database table,
        /// since there is no constraint that says the feature ID column has to be a unique
        /// key. Use the <see "System.Data.DataRow.Table.Name"/> property to determine where
        /// the data came from.
        /// </summary>
        Dictionary<string, List<DataRow>> m_Data;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeData"/> class that
        /// has no associated attributes.
        /// </summary>
        internal AttributeData()
        {
            m_Data = null;
        }

        #endregion

        /// <summary>
        /// Attaches miscellaneous attribute data to the features that have been loaded.
        /// </summary>
        /// <param name="keys">The keys (formatted feature IDs) to look for</param>
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
        internal int Load(string[] keys)
        {
            // Locate information about the tables associated with Backsight
            ITable[] tables = EnvironmentContainer.Current.Tables;
            if (tables.Length == 0)
                return -1;

            // Copy the required keys into a temp table
            Trace.WriteLine(String.Format("Locating attributes for {0} features in {1} tables", keys.Length, tables.Length));

            // Form the master table that combines the lot
            m_Data = new Dictionary<string, List<DataRow>>(keys.Length);
            int nFound = 0;

            using (IConnection ic = ConnectionFactory.Create())
            {
                SqlConnection c = ic.Value;
                const string KEYS_TABLE_NAME = "#Keys";
                CopyKeysToTable(c, keys, KEYS_TABLE_NAME);

                foreach (ITable t in tables)
                {
                    string sql = String.Format("SELECT * FROM [{0}] WHERE [{1}] IN (SELECT [FeatureId] FROM [{2}])",
                                    t.TableName, t.FeatureIdColumnName, KEYS_TABLE_NAME);
                    DataTable tab = DbUtil.ExecuteSelect(c, sql);
                    tab.TableName = t.TableName;

                    int featureIdIndex = tab.Columns[t.FeatureIdColumnName].Ordinal;
                    Debug.Assert(featureIdIndex>=0);
                    foreach (DataRow row in tab.Select())
                    {
                        string key = row[featureIdIndex].ToString();
                        List<DataRow> rows;
                        if (!m_Data.TryGetValue(key, out rows))
                        {
                            rows = new List<DataRow>(1);
                            m_Data.Add(key, rows);
                        }

                        rows.Add(row);
                        nFound++;
                    }
                }
            }

            return nFound;
        }

        /// <summary>
        /// Copies an array of keys (formatted feature IDs) into a new database table.
        /// </summary>
        /// <param name="con">The database connection to use</param>
        /// <param name="keys">The keys of the features to copy in</param>
        /// <param name="tableName">The name of the table to create and load. This will
        /// typically be a temporary table (a name starting with the "#" character
        /// in SqlServer systems).</param>
        void CopyKeysToTable(SqlConnection con, string[] keys, string tableName)
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

        /// <summary>
        /// Attempts to locate the attributes associated with a feature
        /// </summary>
        /// <param name="fid">The ID of interest</param>
        /// <returns>The corresponding attributes (null if the <see cref="Load"/> method has not been
        /// called, or no database tables are associated with Backsight). An empty array if no attributes
        /// were found.</returns>
        DataRow[] Find(FeatureId fid)
        {
            if (m_Data==null)
                return null;

            string key = fid.FormattedKey;
            List<DataRow> result;
            if (m_Data.TryGetValue(key, out result))
                return result.ToArray();

            return new DataRow[0];
        }
    }
}
