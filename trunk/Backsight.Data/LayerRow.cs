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

using Backsight.Environment;

namespace Backsight.Data
{
    public partial class BacksightDataSet
    {
        partial class LayerRow : IEditLayer
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
                    this.tableLayer.AddLayerRow(this);
            }

            public static BacksightDataSet.LayerRow CreateLayerRow(BacksightDataSet ds)
            {
                LayerRow result = ds.Layer.NewLayerRow();
                result.SetDefaultValues();
                return result;
            }

            internal void SetDefaultValues()
            {
                LayerId = 0;
                Name = String.Empty;
                ThemeId = 0;
                ThemeSequence = 0;
                DefaultPointId = 0;
                DefaultLineId = 0;
                DefaultPolygonId = 0;
                DefaultTextId = 0;
            }

            public int Id
            {
                get { return LayerId; }
            }

            public IEntity DefaultPointType
            {
                get { return this.EntityTypesRowByFK_Layer_EntityType1; }
                set { DefaultPointId = (value==null ? 0 : value.Id); }
            }

            public IEntity DefaultLineType
            {
                get { return this.EntityTypesRowByFK_Layer_EntityType2; }
                set { DefaultLineId = (value==null ? 0 : value.Id); }
            }

            public IEntity DefaultTextType
            {
                get { return this.EntityTypesRowByFK_Layer_EntityType4; }
                set { DefaultTextId = (value==null ? 0 : value.Id); }
            }

            public IEntity DefaultPolygonType
            {
                get { return this.EntityTypesRowByFK_Layer_EntityType3; }
                set { DefaultPolygonId = (value==null ? 0 : value.Id); }
            }

            /// <summary>
            /// Any theme associated with this layer (may be null).
            /// </summary>
            public ITheme Theme
            {
                get { return this.ThemeRow; }
                set
                {
                    ThemeId = (value==null ? 0 : value.Id);
                    if (ThemeId==0)
                        ThemeSequence = 0;
                }
            }
        }
    }
}
