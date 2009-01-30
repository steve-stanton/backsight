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
using System.Diagnostics;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;

using Backsight.Data;

namespace Backsight.Editor
{
    /// <summary>
    /// Test class for working with positions in a database table
    /// </summary>
    class PositionLoader
    {
        readonly CadastralMapModel m_Map;
        readonly DataTable m_Table;
        List<DataRow> m_Data;

        internal PositionLoader(CadastralMapModel map)
        {
            m_Map = map;
            m_Table = new DataTable("[ced].[Positions]");
            m_Table.Columns.Add(new DataColumn("PositionId", typeof(int)));
            m_Table.Columns.Add(new DataColumn("Lat", typeof(double)));
            m_Table.Columns.Add(new DataColumn("Lon", typeof(double)));
        }

        internal IDictionary<int, IPosition> ReadPositions()
        {
            Stopwatch sw = Stopwatch.StartNew();
            IDictionary<int, IPosition> result = new Dictionary<int, IPosition>();
            //ICoordinateSystem sys = m_Map.CoordinateSystem;

            using (IConnection ic = ConnectionFactory.Create())
            {
                SqlCommand cmd = new SqlCommand("SELECT [PositionId], [Lat], [Lon] FROM [ced].[Positions]",
                                                    ic.Value);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        int id = rdr.GetInt32(0);
                        double lat = rdr.GetDouble(1);
                        double lon = rdr.GetDouble(2);

                        // Don't have a method to convert lat/long ??
                        result.Add(id, new Position(lon, lat));
                    }
                }
            }

            sw.Stop();
            MessageBox.Show("That took " + sw.ElapsedMilliseconds + " millisecs (" + result.Count + " points)");

            return result;
        }

        internal void LoadDatabase()
        {
            Stopwatch sw = Stopwatch.StartNew();
            m_Data = new List<DataRow>(10000);
            ISpatialIndex index = CadastralMapModel.Current.Index;
            index.QueryWindow(null, SpatialType.Point, LoadData);

            using (IConnection ic = ConnectionFactory.Create())
            {
                SqlBulkCopy bcp = new SqlBulkCopy(ic.Value);
                bcp.DestinationTableName = m_Table.TableName;
                bcp.BatchSize = m_Data.Count;
                bcp.WriteToServer(m_Data.ToArray());
            }

            // 2.3 secs for 4800 points
            // 0.17 secs on 2nd run
            sw.Stop();
            MessageBox.Show("That took " + sw.ElapsedMilliseconds + " millisecs ("+m_Data.Count+" points)");
        }

        bool LoadData(ISpatialObject so)
        {
            IPointGeometry pg = (so as IPoint).Geometry;
            ICoordinateSystem cs = CadastralMapModel.Current.CoordinateSystem;
            double lat, lon;
            cs.GetLatLong(pg, out lat, out lon);

            DataRow row = m_Table.NewRow();
            row[0] = m_Data.Count + 1;
            row[1] = lat * MathConstants.RADTODEG;
            row[2] = lon * MathConstants.RADTODEG;
            m_Data.Add(row);

            return true;
        }

    }
}
