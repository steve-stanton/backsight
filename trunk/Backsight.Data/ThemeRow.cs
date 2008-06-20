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
using System.Diagnostics;

namespace Backsight.Data
{
    public partial class BacksightDataSet
    {
        partial class ThemeRow : IEditTheme
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
                    this.tableTheme.AddThemeRow(this);
            }

            public static BacksightDataSet.ThemeRow CreateThemeRow(BacksightDataSet ds)
            {
                ThemeRow result = ds.Theme.NewThemeRow();
                result.SetDefaultValues();
                return result;
            }

            internal void SetDefaultValues()
            {
                ThemeId = 0;
                Name = String.Empty;
            }

            public int Id
            {
                get { return ThemeId; }
            }

            /// <summary>
            /// The layers associated with the theme, ordered so that the base layer
            /// comes first.
            /// </summary>
            public ILayer[] Layers
            {
                get
                {
                    LayerRow[] result = this.GetLayersRows();
                    Array.Sort<LayerRow>(result, delegate(LayerRow a, LayerRow b)
                                { return a.ThemeSequence.CompareTo(b.ThemeSequence); });
                    return result;
                }
            }
        }
    }
}
