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

namespace Backsight.Editor
{
    public class Constants : MathConstants
    {
        /// <summary>
        /// Resolution of data (1 micron)
        /// </summary>
        public const double XYRES = 0.000001;

        /// <summary>
        /// Tolerance for intersections. This is established by supposing that any location has a
        /// circle of uncertainty that has a 1 micron radius. This means that a segment has a corridor
        /// of uncertainty that is up to 1.414 units wide. When dealing with 2 line segments, it may
        /// be possible that the uncertainty is compounded to give us 2.828 units (I'm no mathematician,
        /// maybe it doesn't). Then we add on a bit for luck to give us 3 microns.
        /// </summary>
        public const double XYTOL = 0.000003;

        /// <summary>
        /// The tolerance squared (handy during calculations)
        /// </summary>
        public const double XYTOLSQ = XYTOL*XYTOL;
}
}
