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
using System.Diagnostics;
using System.Data;

using Backsight.Environment;

namespace Backsight.Data
{
    public partial class BacksightDataSet
    {
        public partial class EntityRow : IEditEntity
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
                    this.tableEntity.AddEntityRow(this);
            }

            public static EntityRow CreateEntityRow(BacksightDataSet ds)
            {
                EntityRow result = ds.Entity.NewEntityRow();
                result.SetDefaultValues();
                return result;
            }

            internal void SetDefaultValues()
            {
                EntityId = 0;
                Name = String.Empty;
                IsPoint = NO;
                IsLine = NO;
                IsLineTopological = NO;
                IsPolygon = NO;
                IsText = NO;
                FontId = 0;
                LayerId = 0;
                GroupId = 0;
            }

            public int Id
            {
                get { return EntityId; }
            }

            public bool IsPointValid
            {
                get { return (IsPoint==YES); }
                set { IsPoint = AsString(value); }
            }

            public bool IsLineValid
            {
                get { return (IsLine==YES); }
                set { IsLine = AsString(value); }
            }

            public bool IsPolygonValid
            {
                get { return (IsPolygon==YES); }
                set { IsPolygon = AsString(value); }
            }

            public bool IsPolygonBoundaryValid
            {
                get { return (IsLineTopological==YES); }
                set { IsLineTopological = AsString(value); }
            }

            public bool IsTextValid
            {
                get { return (IsText==YES && IsPolygon==NO); }
                set { IsText = AsString(value); }
            }

            public bool IsValid(SpatialType t)
            {
                return (((t & SpatialType.Point)!=0 && IsPointValid) ||
                        ((t & SpatialType.Line) !=0 && IsLineValid) ||
                        ((t & SpatialType.Text) !=0 && IsTextValid) ||
                        ((t & SpatialType.Polygon)!=0 && IsPolygonValid));
            }

            /// <summary>
            /// The table(s) that are usually associated with this entity type.
            /// </summary>
            public ITable[] DefaultTables
            {
                get
                {
                    // Grab the rows that associate this entity type with schemas (tables)
                    BacksightDataSet ds = GetDataSet(this);
                    EntitySchemaRow[] entSchemas = ds.EntitySchema.FindByEntityId(this.EntityId);

                    // Cover easy case where there isn't anything
                    if (entSchemas.Length == 0)
                        return new ITable[0];

                    // Pick out those tables that are part of the association
                    ITable[] tables = (ITable[])ds.Schema.Select();
                    return Array.FindAll<ITable>(tables, delegate(ITable t)
                    {
                        return EntitySchemaRow.IsTableInArray(t, entSchemas);
                    });
                }

                set
                {
                    // Grab the current entity type -> schema (table) associations
                    BacksightDataSet ds = GetDataSet(this);
                    EntitySchemaDataTable tab = ds.EntitySchema;
                    EntitySchemaRow[] entSchemas = tab.FindByEntityId(this.EntityId);

                    // Insert new associations
                    foreach (ITable t in value)
                    {
                        if (!EntitySchemaRow.IsTableInArray(t, entSchemas))
                            tab.AddEntitySchemaRow(this.EntityId, t.Id);
                    }

                    // Remove any associations that no longer apply
                    foreach (EntitySchemaRow row in entSchemas)
                    {
                        if (!Array.Exists<ITable>(value, delegate(ITable t) { return t.Id==row.SchemaId; }))
                            row.Delete();
                    }
                }
            }

            public IIdGroup IdGroup
            {
                get { return (GroupId==0 ? null : this.IdGroupRow); }
                set { GroupId = (value==null ? 0 : value.Id); }
            }

            public ILayer Layer
            {
                get { return (LayerId==0 ? null : this.LayerRow); }
                set { LayerId = (value==null ? 0 : value.Id); }
            }

            public IFont Font
            {
                get { return (FontId == 0 ? null : this.FontRow); }
                set { FontId = (value == null ? 0 : value.Id); }
            }
        }
    }
}
