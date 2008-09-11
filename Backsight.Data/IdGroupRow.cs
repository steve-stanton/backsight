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
        partial class IdGroupRow : IEditIdGroup
        {
            public override string ToString()
            {
                return Name;
            }

            public void FinishEdit()
            {
                if (IsAdded(this))
                    this.EndEdit();
                else
                    this.tableIdGroup.AddIdGroupRow(this);
            }

            public static BacksightDataSet.IdGroupRow CreateIdGroupRow(BacksightDataSet ds)
            {
                IdGroupRow result = ds.IdGroup.NewIdGroupRow();
                result.SetDefaultValues();
                return result;
            }

            internal void SetDefaultValues()
            {
                GroupId = 0;
                Name = String.Empty;
                LowestId = 0;
                HighestId = 0;
                PacketSize = 0;
                CheckDigit = NO;
                KeyFormat = "{0}";
            }

            public int Id
            {
                get { return GroupId; }
            }

            public bool HasCheckDigit
            {
                get { return CheckDigit==YES; }
                set { CheckDigit = AsString(value); }
            }

            public IEntity[] EntityTypes
            {
                get { return (IEntity[])this.GetEntityTypesRows(); }
            }
        }
    }
}
