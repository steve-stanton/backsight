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
using System.Drawing;

using Backsight.Environment;
using System.Text;

namespace Backsight.Data
{
    public partial class BacksightDataSet
    {
        partial class FontRow : IEditFont
        {
            /// <summary>
            /// A user-perceived title for this font.
            /// </summary>
            /// <returns>The type face (font family name), its points size, and
            /// any modifiers. The result could be supplied to the <c>Font</c>
            /// constructor that accepts a font title.</returns>
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder(100);

                sb.AppendFormat("{0} - {1}", TypeFace, PointSize);
                if (IsBold==YES)
                    sb.Append(" Bold");
                if (IsItalic == YES)
                    sb.Append(" Italic");

                // Underline was formerly ignored, so that's the way it stays

                return sb.ToString();
            }

            public void FinishEdit()
            {
                if (IsAdded(this))
                    this.EndEdit();
                else
                    this.tableFont.AddFontRow(this);
            }

            public static FontRow CreateFontRow(BacksightDataSet ds)
            {
                FontRow result = ds.Font.NewFontRow();
                result.SetDefaultValues();
                return result;
            }

            internal void SetDefaultValues()
            {
                FontId = 0;
                TypeFace = String.Empty;
                PointSize = 0.0F;
                IsBold = NO;
                IsItalic = NO;
                IsUnderline = NO;
                FontFile = String.Empty;
            }

            public int Id
            {
                get { return FontId; }
            }

            public FontStyle Modifiers
            {
                get
                {
                    FontStyle result = 0;
                    if (IsBold==YES)
                        result |= FontStyle.Bold;

                    if (IsItalic==YES)
                        result |= FontStyle.Italic;

                    if (IsUnderline==YES)
                        result |= FontStyle.Underline;

                    return result;
                }

                set
                {
                    IsBold = ((value & FontStyle.Bold)==0 ? NO : YES);
                    IsItalic = ((value & FontStyle.Italic)==0 ? NO : YES);
                    IsUnderline = ((value & FontStyle.Underline)==0 ? NO : YES);
                }
            }
        }
    }
}
