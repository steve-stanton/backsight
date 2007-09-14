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
    /// Something that processes an item in the index (for use with implementations
    /// of the <c>ISpatialIndex.QueryWindow</c> method).
    /// </summary>
    /// <param name="item">An object associated with the spatial index</param>
    /// <returns>True if the query should be continued. False if the query should be
    /// terminated (e.g. a result may have been obtained).</returns>
    public delegate bool ProcessItem(ISpatialObject item);


	/// <written by="Steve Stanton" on="31-OCT-2006" />
    /// <summary>
    /// A spatial index.
    /// </summary>
    public interface ISpatialIndex
    {
        /// <summary>
        /// Is the index empty (containing nothing)?
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Locates the feature closest to a specific position. Ignores polygons.
        /// </summary>
        /// <param name="p">The search position</param>
        /// <param name="radius">The search radius</param>
        /// <param name="types">The type(s) of object to look for (if you include polygons as
        /// an applicable type, they will be quietly ignored).</param>
        /// <returns>The closest feature of the requested type (null if nothing found)</returns>
        ISpatialObject QueryClosest(IPosition p, ILength radius, SpatialType types);

        /// <summary>
        /// Process items with a covering rectangle that overlaps a query window.
        /// </summary>
        /// <param name="extent">The extent of the query window</param>
        /// <param name="types">The type(s) of object to look for</param>
        /// <param name="itemHandler">The method that should be called for each query hit. A hit
        /// is defined as anything with a covering rectangle that overlaps the query window (this
        /// does not mean the hit actually intersects the window).</param>
        void QueryWindow(IWindow extent, SpatialType types, ProcessItem itemHandler);
    }
}
