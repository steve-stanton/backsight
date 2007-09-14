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

using Backsight.Properties;

namespace Backsight.Forms
{
    public static class MapResources
    {
        public static Cursor NewCenterCursor
        {
            get { return new Cursor(new MemoryStream(Resources.NewCenterCursor)); }
        }

        public static Cursor StartPanCursor
        {
            get { return new Cursor(new MemoryStream(Resources.CarCursor)); }
        }

        public static Cursor PanCursor
        {
            get { return new Cursor(new MemoryStream(Resources.MovingCarCursor)); }
        }

        public static Cursor MagnifyingGlassCursor
        {
            get { return new Cursor(new MemoryStream(Resources.MagnifyingGlass)); }
        }

        public static Cursor HollowSquareCursor
        {
            get { return new Cursor(new MemoryStream(Resources.HollowSquareCursor)); }
        }
    }
}
