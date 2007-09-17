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
    /// Flag bits denoting line-line intersection relationships.
    /// </summary>
    /// <see>IntersectionData</see>
    [Flags]
    enum IntersectionType : ushort
    {
        TouchStart  = 0x0001,       // Touch at start of line
		TouchEnd    = 0x0002, 	    // Touch at end of line
        TouchOther  = 0x0004,       // Touch at some intermediate position
        GrazeStart  = 0x0010,       // Graze at start of line
        GrazeEnd    = 0x0020,       // Graze at end of line
        GrazeOther  = 0x0040,       // Graze along interior portion of line
        GrazeTotal  = 0x0080,       // Total graze
    }
}
