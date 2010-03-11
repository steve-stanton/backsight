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
using System.Drawing.Drawing2D;

namespace Backsight.Forms
{
    /// <summary>
    /// A drawing style in which lines are drawn with a dotted pattern.
    /// </summary>
    public class DottedStyle : DrawStyle
    {
        public DottedStyle()
            : this(Color.Magenta)
        {
        }

        public DottedStyle(Color col)
            : base(col)
        {
            base.Pen.DashPattern = new float[] { 5.0F, 5.0F };
        }
    }
}
