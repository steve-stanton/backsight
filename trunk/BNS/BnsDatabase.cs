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
using System.Data.SqlClient;

using Backsight;
using Backsight.Index;

namespace BNS
{
    class BnsDatabase : ISpatialData
    {
        #region Class data

        /// <summary>
        /// The database connection string
        /// </summary>
        readonly string m_ConnectionString;

        /// <summary>
        /// Shot points
        /// </summary>
        readonly List<SurveyPoint> m_Shots;

        /// <summary>
        /// Receiver points
        /// </summary>
        readonly List<SurveyPoint> m_Receivers;

        readonly IWindow m_Extent;

        readonly ISpatialIndex m_Index;

        #endregion

        #region Constructors

        internal BnsDatabase(string serverName, string dbName)
        {
            m_ConnectionString = String.Format("Data Source={0};Initial Catalog={1};Persist Security Info={2};User ID={3};Password={4}",
                serverName, dbName, "True", "fireflydb", "fireflydb");

            // Get the data from the database

            using (SqlConnection c = new SqlConnection(m_ConnectionString))
            {
                m_Shots = LoadPoints(ExecuteSelect(c, "select * from survey.shot_points_ac"));
                m_Receivers = LoadPoints(ExecuteSelect(c, "select * from survey.rx_points_ac"));
            }

            // Get the overall extent of the data and index it

            Window w = new Window();
            IEditSpatialIndex index = new SpatialIndex();

            foreach (SurveyPoint p in m_Shots)
            {
                w.Union(p.Geometry);
                index.Add(p);
            }

            foreach (SurveyPoint p in m_Receivers)
            {
                w.Union(p.Geometry);
                index.Add(p);
            }

            m_Extent = w;
            m_Index = index;
        }

        #endregion

        List<SurveyPoint> LoadPoints(DataTable table)
        {
            int xIndex = table.Columns["easting"].Ordinal;
            int yIndex = table.Columns["northing"].Ordinal;

            List<SurveyPoint> result = new List<SurveyPoint>(table.Rows.Count);

            foreach (DataRow row in table.Select())
            {
                double x = (double)row[xIndex];
                double y = (double)row[yIndex];
                IPosition p = new Position(x, y);
                result.Add(new SurveyPoint(row, p));
            }

            return result;
        }

        /// <summary>
        /// Executes a SELECT statement using the specified connection 
        /// </summary>
        /// <param name="c">The database connection to use</param>
        /// <param name="sql">The select statement</param>
        /// <returns>The retrieved data</returns>
        DataTable ExecuteSelect(SqlConnection c, string sql)
        {
            SqlCommand cmd = new SqlCommand(sql, c);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            adapter.Fill(dt);
            return dt;
        }



        #region ISpatialData Members

        public string Name
        {
            get { return "BNS"; }
        }

        public void Render(ISpatialDisplay display, IDrawStyle style)
        {
            new DrawQuery(m_Index, display, style);
        }

        public bool IsEmpty
        {
            get { return false; }
        }

        public IWindow Extent
        {
            get { return m_Extent; }
        }

        public ISpatialObject QueryClosest(IPosition p, ILength radius, SpatialType types)
        {
            return m_Index.QueryClosest(p, radius, types);
        }

        #endregion
    }
}
