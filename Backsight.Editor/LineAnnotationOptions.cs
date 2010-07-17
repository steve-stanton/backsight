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
    /// <summary>
    /// Options relating to line annotation (as noted by <see cref="LineAnnotationStyle"/>)
    /// </summary>
    [Flags]
    enum LineAnnotationOptions : byte
    {
        /// <summary>
        /// Should the adjusted length of lines be displayed?
        /// (the units used for the display is determined via another display preference)
        /// </summary>
        ShowAdjustedLengths = 0x01,

        /// <summary>
        /// Should the observed length of lines be displayed?
        /// (the units used for the display is determined via another display preference)
        /// </summary>
        ShowObservedLengths = 0x02,

        /// <summary>
        /// Should observed angles be displayed?
        /// </summary>
        ShowObservedAngles = 0x04,
    }
}
