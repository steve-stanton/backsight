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
        partial class EntityTypeDataTable
        {
            internal EntityTypeRow AddEmptyRow()
            {
                EntityTypeRow result = NewEntityTypeRow();
                result.SetDefaultValues();

                // It's more convenient if the empty row is associated with all spatial types
                // (when loading things like a combobox, it means the blank entity type will show
                // by default).
                result.IsPoint = YES;
                result.IsLine = YES;
                result.IsText = YES;
                result.IsPolygon = YES;

                AddEntityTypeRow(result);
                return result;
            }

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
                        columnEntityId.ColumnName + ">=0"
                    ,   columnIsPoint.ColumnName + YES_NO
                    ,   columnIsLine.ColumnName + YES_NO
                    ,   columnIsLineTopological.ColumnName + YES_NO
                    ,   columnIsPolygon.ColumnName + YES_NO
                    ,   columnIsText.ColumnName + YES_NO
                    ,   columnFontId.ColumnName + ">=0"
                    ,   columnGroupId.ColumnName + ">=0"
                    };
                }
            }
        }
    }
}
