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
using System.Windows.Forms;
using System.Drawing;
using System.IO;

using Backsight.Editor.Properties;

namespace Backsight.Editor
{
    static class EditorResources
    {
        internal static Cursor PenCursor
        {
            get
            {
                Cursor c = new Cursor(new MemoryStream(Resources.PenCursor));
                c.Tag = "PenCursor"; // test
                return c;
            }
        }

        internal static Cursor PolygonSubdivisionCursor
        {
            get
            {
                Cursor c = new Cursor(new MemoryStream(Resources.PolygonSubdivisionCursor));
                c.Tag = "PolygonSubdivisionCursor"; // test
                return c;
            }
        }

        internal static Cursor AttachPointCursor
        {
            get
            {
                Cursor c = new Cursor(new MemoryStream(Resources.AttachPointCursor));
                c.Tag = "AttachPointCursor";
                return c;
            }
        }

        internal static Cursor DiagonalCursor
        {
            get
            {
                Cursor c = new Cursor(new MemoryStream(Resources.DiagonalCursor));
                c.Tag = "DiagonalCursor";
                return c;
            }
        }

        internal static Cursor ReverseArrowCursor
        {
            get
            {
                Cursor c = new Cursor(new MemoryStream(Resources.ReverseArrowCursor));
                c.Tag = "ReverseArrowCursor";
                return c;
            }
        }
    }
}
