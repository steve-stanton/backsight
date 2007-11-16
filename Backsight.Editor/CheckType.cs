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

namespace Backsight.Editor
{
    /// <summary>
    /// Bit masks denoting specific checks. The low-order 8 bits relate to line checks.
    /// The high-order bits relate to polygon and label checks.
    /// </summary>
    /// <remarks>Each flag bit defined here should have a corresponding code letter. These
    /// code letters are currently defined as constants in the <see cref="CheckItem"/> class.
    /// </remarks>
    [Flags]
    enum CheckType : ushort
    {
        /// <summary>
        /// Not a check
        /// </summary>
        Null = 0x0000,

        /// <summary>
        /// Very small line
        /// </summary>
        SmallLine = 0x0001, // CHB_LSMALL

        /// <summary>
        /// Line is dangling
        /// </summary>
        Dangle = 0x0002, // CHB_DANGLE

        /// <summary>
        /// Line overlap
        /// </summary>
        Overlap = 0x0004, // CHB_OVERLAP

        /// <summary>
        /// Line is floating in space (same polygon on both sides, end points un-connected)
        /// </summary>
        Floating = 0x0008, // CHB_FLOAT

        /// <summary>
        /// Bridging line (same polygon on both sides)
        /// </summary>
        Bridge = 0x0010, // CHB_BRIDGE

        /// <summary>
        /// Very small polygon
        /// </summary>
        SmallPolygon = 0x0100, // CHB_PSMALL

        /// <summary>
        /// Island ring is not enclosed by any polygon
        /// </summary>
        NotEnclosed = 0x0200, // CHB_NOPOLENCPOL

        /// <summary>
        /// Polygon has no label
        /// </summary>
        NoLabel = 0x0400, // CHB_NOLABEL

        /// <summary>
        /// Label has no enclosing polygon
        /// </summary>
		NoPolygonForLabel = 0x0800,	// CHB_NOPOLENCLAB

        /// <summary>
        /// No attributes associated with polygon label
        /// </summary>
        NoAttributes = 0x1000, // CHB_NOATTR

        /// <summary>
        /// More than one label inside a single polygon
        /// </summary>
        MultiLabel = 0x2000, // CHB_MULTILAB
    }
}
