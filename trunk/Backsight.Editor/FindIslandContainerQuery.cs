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
using System.Collections.Generic;

using Backsight.Geometry;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="23-AUG-2007" />
    /// <summary>
    /// Query spatial index to obtain the polygon (if any) that encloses an island.
    /// This is used during polygon formation, so island topology may not be entirely
    /// resolved when this class is used.
    /// </summary>
    /// <seealso cref="FindPointContainerQuery"/>
    class FindIslandContainerQuery
    {
        #region Class data

        /// <summary>
        /// The island you want the container for.
        /// </summary>
        readonly Island m_Island;

        /// <summary>
        /// The most easterly position of the island (the search point).
        /// </summary>
        readonly PointGeometry m_EastPoint;

        /// <summary>
        /// The enclosing polygon (null if nothing has been found).
        /// </summary>
        Polygon m_Result;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>FindIslandContainerQuery</c> (and executes it). The result of the query
        /// can then be obtained through the <c>Result</c> property.
        /// </summary>
        /// <param name="index">The spatial index to search</param>
        /// <param name="island">The island you want the container for</param>
        internal FindIslandContainerQuery(ISpatialIndex index, Island island)
        {
            m_Island = island;
            m_Result = null;

            // Get the most easterly point in the island.
            IPosition ep = m_Island.GetEastPoint();

            // Shift the east point ONE MICRON further to the east, to ensure
            // we don't pick up the interior of the island!
            PointGeometry eg = PointGeometry.Create(ep);
            m_EastPoint = new PointGeometry(eg.Easting.Microns+1, eg.Northing.Microns);
            IWindow w = new Window(m_EastPoint, m_EastPoint);

            index.QueryWindow(w, SpatialType.Polygon, OnQueryHit);
        }

        #endregion

        /// <summary>
        /// Delegate that's called whenever the index finds an object with an extent that
        /// overlaps the query window.
        /// </summary>
        /// <param name="item">The item to process (expected to be some sort of <c>Ring</c>)</param>
        /// <returns>True if the query should continue. False if the enclosing polygon has been found.</returns>
        private bool OnQueryHit(ISpatialObject item)
        {
            // We're only interested in real polygons (not islands)
            if (!(item is Polygon))
                return true;

            // The area of the polygon MUST be bigger than the area of the island.
            Polygon p = (Polygon)item;
            if (p.Area < m_Island.Area)
                return true;

            // Skip if the window does not actually overlap the search point.
            if (!p.Extent.IsOverlap(m_EastPoint))
                return true;

            // The window of the island must be ENTIRELY within the window
            // of the candidate polygon.
            if (!m_Island.Extent.IsEnclosedBy(p.Extent))
                return true;

            // Skip if the polygon doesn't enclose the east point.
            if (!p.IsRingEnclosing(m_EastPoint))
                return true;

            // Skip if we previously found something, and the area of the candidate
            // polygon is bigger. Take care here - the polygons may not yet know about
            // all their islands (which might lead to an incorrect enclosing polygon),
            // so this test should EXCLUDE the area of any islands that they already
            // know about).
            if (m_Result!=null && p.AreaExcludingIslands > m_Result.AreaExcludingIslands)
                return true;

            m_Result = p;

            // Keep going after finding a result, since a different (smaller) polygon
            // may be found further on in the search.
            return true;
        }

        /// <summary>
        /// The result of the query (null if no features were found within the query region).
        /// </summary>
        internal Polygon Result
        {
            get { return m_Result; }
        }
    }
}
