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
using Backsight.Environment;

namespace Backsight.Data
{
    public partial class BacksightDataSet
    {
        partial class EntityTypeSchemaRow
        {
            /// <summary>
            /// Checks an array of <c>EntityTypeSchemaRow</c> to see if any element refers
            /// to a specific table.
            /// </summary>
            /// <param name="t">The table to look for</param>
            /// <param name="rows">The rows to check</param>
            /// <returns>True if any row refers to the table</returns>
            /// <remarks>This helps to avoid the clumsy syntax of Array.Find, making
            /// calling code a bit more readable.</remarks>
            internal static bool IsTableInArray(ITable t, EntityTypeSchemaRow[] rows)
            {
                int tableId = t.Id;

                foreach (EntityTypeSchemaRow row in rows)
                {
                    if (row.SchemaId == tableId)
                        return true;
                }

                return false;
            }
        }
    }
}
