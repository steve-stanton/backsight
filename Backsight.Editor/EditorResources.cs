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
            get { return CreateCursor(Resources.PenCursor, "PenCursor"); }
        }

        internal static Cursor PolygonSubdivisionCursor
        {
            get { return CreateCursor(Resources.PolygonSubdivisionCursor, "PolygonSubdivisionCursor"); }
        }

        internal static Cursor AttachPointCursor
        {
            get { return CreateCursor(Resources.AttachPointCursor, "AttachPointCursor"); }
        }

        internal static Cursor DiagonalCursor
        {
            get { return CreateCursor(Resources.DiagonalCursor, "DiagonalCursor"); }
        }

        internal static Cursor ReverseArrowCursor
        {
            get { return CreateCursor(Resources.ReverseArrowCursor, "ReverseArrowCursor"); }
        }

        internal static Cursor GrayReverseArrowCursor
        {
            get { return CreateCursor(Resources.GrayReverseArrowCursor, "GrayReverseArrowCursor"); }
        }

        internal static Cursor Point1Cursor
        {
            get { return CreateCursor(Resources.Point1Cursor, "Point1Cursor"); }
        }

        internal static Cursor Point2Cursor
        {
            get { return CreateCursor(Resources.Point2Cursor, "Point2Cursor"); }
        }

        internal static Cursor WandCursor
        {
            get { return CreateCursor(Resources.WandCursor, "WandCursor"); }
        }

        internal static Cursor GrayWandCursor
        {
            get { return CreateCursor(Resources.GrayWandCursor, "GrayWandCursor"); }
        }

        static Cursor CreateCursor(byte[] cursorData, string tag)
        {
            Stream s = new MemoryStream(cursorData);
            Cursor result = new Cursor(s);
            result.Tag = tag;
            return result;
        }
    }
}
