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

namespace Backsight
{
    /// <summary>
    /// Some sort of rectangular window on the ground. This version is const, to change
    /// the window, use <c>IEditWindow</c>.
    /// </summary>
    public interface IWindow : IExpandablePropertyItem
    {
        /// <summary>
        /// The position of the south-west corner.
        /// </summary>
        IPosition Min { get; }

        /// <summary>
        /// The position of the north-east corner.
        /// </summary>
        IPosition Max { get; }

        /// <summary>
        /// The height of this window, in meters (<c>Double.NaN</c> if the <see cref="IsEmpty"/> property is true).
        /// </summary>
        double Height { get; }

        /// <summary>
        /// The width of this window, in meters (<c>Double.NaN</c> if the <see cref="IsEmpty"/> property is true).
        /// </summary>
        double Width { get; }

        /// <summary>
        /// The position at the center of this window (null if the <see cref="IsEmpty"/> property is true).
        /// </summary>
        IPosition Center { get; }

        /// <summary>
        /// Is this window empty. An empty window is an undefined window.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Does this window only cover a point in space?
        /// </summary>
        bool IsPoint { get; }

        /// <summary>
        /// Checks whether this window is entirely enclosed by another window.
        /// </summary>
        /// <param name="other">The window to compare with this one</param>
        /// <returns>True is this window is entirely enclosed</returns>
        bool IsEnclosedBy(IWindow other);

        /// <summary>
        /// Checks if two windows overlap (or touch).
        /// </summary>
        /// <param name="e">The extent to compare with</param>
        /// <returns>True if this extent overlaps (or touches) the supplied extent</returns>
        bool IsOverlap(IWindow e);

        /// <summary>
        /// Checks whether a position falls inside (or on) this extent.
        /// </summary>
        /// <param name="p">The position to test</param>
        /// <returns>True if the supplied position is inside (or on the edge of) this extent</returns>
        bool IsOverlap(IPosition p);
    }
}
