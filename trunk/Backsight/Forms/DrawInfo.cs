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

namespace Backsight.Forms
{
	/// <written by="Steve Stanton" on="14-SEP-2006" />
    /// <summary>The center and scale used for a map display</summary>
    public struct DrawInfo
    {
        public double CenterX;
        public double CenterY;
        public double MapScale;

        public DrawInfo(double centerX, double centerY, double mapScale)
        {
            CenterX = centerX;
            CenterY = centerY;
            MapScale = mapScale;
        }

        public DrawInfo(IWindow extent, double mapScale)
        {
            IPosition c = extent.Center;
            CenterX = c.X;
            CenterY = c.Y;
            MapScale = mapScale;
        }
    }
}
