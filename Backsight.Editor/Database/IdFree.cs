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
using System.Collections.Generic;
using System.Data.SqlClient;

using Backsight.Data;

namespace Backsight.Editor.Database
{
    /// <summary>
    /// Database access to the <c>IdFree</c> table.
    /// </summary>
    class IdFree
    {
        #region Static

        /// <summary>
        /// Locates rows that refer to a specific ID group
        /// </summary>
        /// <param name="groupId">The ID of the ID group of interest</param>
        /// <returns>The rows identifying free ranges for the group (may be an empty array if
        /// the group has never been used)</returns>
        internal static IdFree[] FindByGroupId(int groupId)
        {
            using (IConnection ic = AdapterFactory.GetConnection())
            {
                List<IdFree> result = new List<IdFree>(1000);
                string sql = String.Format("{0} WHERE [GroupId]={1}", GetSelectString(), groupId);
                SqlCommand cmd = new SqlCommand(sql, ic.Value);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    IdFree item = ParseSelect(reader);
                    result.Add(item);
                }
                return result.ToArray();
            }
        }

        /// <summary>
        /// Inserts a free range that refers to the complete range of an ID group
        /// </summary>
        /// <param name="idGroup">The new ID group</param>
        /// <returns>The inserted free range entry</returns>
        internal static IdFree Insert(IdGroup idGroup)
        {
            using (IConnection ic = AdapterFactory.GetConnection())
            {
                string sql = String.Format("INSERT INTO [dbo].[IdFree] ([GroupId], [LowestId], [HighestId])" +
                                            " VALUES ({0}, {1}, {2})", idGroup.Id, idGroup.LowestId, idGroup.HighestId);
                SqlCommand cmd = new SqlCommand(sql, ic.Value);
                cmd.ExecuteNonQuery();

                return new IdFree(idGroup.Id, idGroup.LowestId, idGroup.HighestId);
            }
        }

        #endregion

        #region Class data

        /// <summary>
        /// The unique ID of the ID group
        /// </summary>
        int m_GroupId;

        /// <summary>
        /// The lowest value in the free range
        /// </summary>
        int m_LowestId;

        /// <summary>
        /// The highest value in the free range
        /// </summary>
        int m_HighestId;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>IdFree</c> using information selected from the database
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="lowestId"></param>
        /// <param name="highestId"></param>
        IdFree(int groupId, int lowestId, int highestId)
        {
            m_GroupId = groupId;
            m_LowestId = lowestId;
            m_HighestId = highestId;
        }

        #endregion

        /// <summary>
        /// Obtains a select statement that can be used to select all columns from the
        /// database table. Rows returned using this select can be subsequently parsed
        /// with a call to <see cref="ParseSelect"/>.
        /// </summary>
        /// <returns>The SQL select statement (with no where clause)</returns>
        static string GetSelectString()
        {
            return "SELECT [GroupId], [LowestId], [HighestId] FROM [dbo].[IdFree]";
        }

        /// <summary>
        /// Parses a selection that refers to the columns identified via a prior call to
        /// <see cref="GetSelectString"/>
        /// </summary>
        /// <param name="reader">The database reader, positioned at the row that needs to
        /// be parsed</param>
        /// <returns>Data corresponding to the content of the row</returns>
        static IdFree ParseSelect(SqlDataReader reader)
        {
            int groupId = reader.GetInt32(0);
            int lowestId = reader.GetInt32(1);
            int highestId = reader.GetInt32(2);

            return new IdFree(groupId, lowestId, highestId);
        }

        /// <summary>
        /// The unique ID of the ID group
        /// </summary>
        internal int GroupId
        {
            get { return m_GroupId; }
        }

        /// <summary>
        /// The lowest value in the free range
        /// </summary>
        internal int LowestId
        {
            get { return m_LowestId; }
            set
            {
                using (IConnection ic = AdapterFactory.GetConnection())
                {
                    string sql = String.Format("UPDATE [dbo].[IdFree] SET [LowestId]={0} WHERE [GroupId]={1} AND [LowestId]={2}",
                                                value, m_GroupId, m_LowestId);
                    SqlCommand cmd = new SqlCommand(sql, ic.Value);
                    cmd.ExecuteNonQuery();
                }

                m_LowestId = value;
            }
        }

        /// <summary>
        /// The highest value in the free range
        /// </summary>
        internal int HighestId
        {
            get { return m_HighestId; }
        }

        /// <summary>
        /// Removes this row from the database
        /// </summary>
        internal void Delete()
        {
            using (IConnection ic = AdapterFactory.GetConnection())
            {
                string sql = String.Format("DELETE FROM [dbo].[IdFree] WHERE [GroupId]={0} AND [LowestId]={1}",
                                            m_GroupId, m_LowestId);
                SqlCommand cmd = new SqlCommand(sql, ic.Value);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
