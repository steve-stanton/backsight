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

namespace Backsight.Editor.Observations
{
	/// <written by="Steve Stanton" on="13-NOV-1997" />
    /// <summary>
    /// An offset with respect to something else. This is the base class for
    /// <see cref="OffsetDistance"/> and <see cref="OffsetPoint"/>.
    /// </summary>
    abstract class Offset : Observation, IPersistent
    {
        /// <summary>
        /// Returns the offset distance with respect to a reference direction, in meters
        /// on the ground. Offsets to the left are returned as a negated value, while
        /// offsets to the right are positive values.
        /// </summary>
        /// <param name="dir">The direction that the offset was observed with respect to.</param>
        /// <returns>The signed offset distance, in meters on the ground</returns>
        abstract internal double GetMetric(Direction dir);

        /// <summary>
        /// The offset point (if this is an instance of <see cref="OffsetPoint"/>), or
        /// null for any other type of offset.
        /// </summary>
        abstract internal PointFeature Point { get; }

        /// <summary>
        /// Cuts references to an operation that are made by any features this offset refers to.
        /// </summary>
        /// <param name="op">The operation that should no longer be referred to.</param>
        abstract internal void CutRef(Operation op);

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        abstract public void WriteData(EditSerializer editSerializer);
    }
}
