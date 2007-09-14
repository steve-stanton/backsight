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

namespace Backsight.Data
{
    public partial class BacksightDataSet
    {
        partial class MapDataTable
        {
            /// <summary>
            /// Any simple check constraints relating to columns in this table.
            /// </summary>
            public string[] Checks
            {
                get
                {
                    const string YES_NO = " IN ('y', 'n')";

                    return new string[]
                    {
                        columnMapId.ColumnName + ">=0"
                    ,   columnVersion.ColumnName + ">=0"
                    ,   columnJobId.ColumnName + ">=0"
                    ,   columnUserId.ColumnName + ">=0"
                    ,   columnArchiveId.ColumnName + ">=0"
                    ,   columnIsActive.ColumnName + YES_NO
                    };
                }
            }
        }
    }
}
