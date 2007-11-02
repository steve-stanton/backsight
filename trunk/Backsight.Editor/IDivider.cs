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
    /// <written by="Steve Stanton" on="30-OCT-2007" />
    /// <summary>
    /// Something that divides a pair of polygon rings.
    /// </summary>
    interface IDivider : IIntersectable
    {
        /// <summary>
        /// The line the divider is associated with (the divider may cover only a portion
        /// of this line).
        /// </summary>
        LineFeature Line { get; }

        /// <summary>
        /// The start position for the divider.
        /// </summary>
        ITerminal From { get; }

        /// <summary>
        /// The end position for the divider.
        /// </summary>
        ITerminal To { get; }

        /// <summary>
        /// The polygon ring on the left of the divider (may be null)
        /// </summary>
        Ring Left { get; set; }

        /// <summary>
        /// The polygon ring on the right of the divider (may be null)
        /// </summary>
        Ring Right { get; set; }
    }
}
