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
        partial class PropertyRow : IEditProperty
        {
            public void FinishEdit()
            {
                if (IsAdded(this))
                    this.EndEdit();
                else
                    this.tableProperty.AddPropertyRow(this);
            }

            public static BacksightDataSet.PropertyRow CreatePropertyRow(BacksightDataSet ds)
            {
                PropertyRow result = ds.Property.NewPropertyRow();
                result.Name = result.Value = result.Description = String.Empty;
                return result;
            }
        }
    }
}
