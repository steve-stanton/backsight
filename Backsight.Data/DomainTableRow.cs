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

using Backsight.Environment;

namespace Backsight.Data
{
    public partial class BacksightDataSet
    {
        public partial class DomainTableRow : IEditDomainTable
        {
            public override string ToString()
            {
                return TableName;
            }

            public void FinishEdit()
            {
                if (IsAdded(this))
                    this.EndEdit();
                else
                    this.tableDomainTable.AddDomainTableRow(this);
            }

            public static DomainTableRow CreateDomainTableRow(BacksightDataSet ds)
            {
                DomainTableRow result = ds.DomainTable.NewDomainTableRow();
                result.SetDefaultValues();
                return result;
            }

            internal void SetDefaultValues()
            {
                DomainId = 0;
                TableName = String.Empty;
                LookupColumnName = "ShortValue";
                ValueColumnName = "LongValue";
            }

            public int Id
            {
                get { return DomainId; }
            }
        }
    }
}
