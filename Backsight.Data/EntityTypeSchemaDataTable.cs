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

namespace Backsight.Data
{
    public partial class BacksightDataSet
    {
        partial class EntityTypeSchemaDataTable
        {
            /// <summary>
            /// Any simple check constraints relating to columns in this table.
            /// </summary>
            public string[] Checks
            {
                get
                {
                    return new string[]
                    {
                        columnEntityId.ColumnName + ">0"
                    ,   columnSchemaId.ColumnName + ">0"
                    };
                }
            }

            /// <summary>
            /// Locates rows that refer to a specific entity type
            /// </summary>
            /// <param name="entId">The ID of the entity type to look for</param>
            /// <returns>The rows that match the specified entity type ID</returns>
            internal EntityTypeSchemaRow[] FindByEntityId(int entId)
            {
                BacksightDataSet ds = (BacksightDataSet)this.DataSet;
                string query = String.Format("{0}={1}", columnEntityId.ColumnName, entId);
                return (EntityTypeSchemaRow[])Select(query);
            }

            /// <summary>
            /// Creates a new row in the <c>EntityTypeSchema</c> table
            /// </summary>
            /// <param name="entityId">The ID of the entity type</param>
            /// <param name="schemaId">The ID of an associated table</param>
            /// <returns>The row inserted into this table</returns>
            internal EntityTypeSchemaRow AddEntityTypeSchemaRow(int entityId, int schemaId)
            {
                EntityTypeSchemaRow result = NewEntityTypeSchemaRow();
                result.EntityId = entityId;
                result.SchemaId = schemaId;
                AddEntityTypeSchemaRow(result);
                return result;
            }
        }
    }
}
