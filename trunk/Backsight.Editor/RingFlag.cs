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
    /// <written by="Steve Stanton" on="20-JUL-1997" />
    /// <summary>
    /// Flag bits relating to a polygon ring.
    /// </summary>
    [Flags]
    enum RingFlag : byte
    {
        /// <summary>
        /// Ring is due for deletion.
        /// </summary>
        Deleted = 0x01,

        /// <summary>
        /// Island is floating
        /// </summary>
        Floating = 0x02,

        /// <summary>
        /// Ring to left of 1st arc (not used)
        /// </summary>
        Left = 0x04,

        /// <summary>
        /// Ring overlaps another ring in a theme
        /// </summary>
        /// <remarks>Hopefully obsolete</remarks>
        Overlap=0x08,

        /// <summary>
        /// Ring created system-generated lines in order to create itself.
        /// </summary>
        /// <remarks>Hopefully obsolete</remarks>
        LineOwner = 0x10,

        /// <summary>
        /// Ring has been spatially indexed.
        /// </summary>
        Indexed = 0x20,
    }
}
