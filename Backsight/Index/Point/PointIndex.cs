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

namespace Backsight.Index.Point
{
	/// <written by="Steve Stanton" on="12-JAN-2007" />
    /// <summary>
    /// Spatial index for 2D point features, where position is expressed to the nearest
    /// micron on the ground (in an unsigned 64-bit coordinate space). At the most general
    /// level, a grid of 256x256 cells is used. The number of cells used becomes progressively
    /// smaller as you drill down, until you eventually end up with a quad tree. The smallest
    /// cell has a coverage of approx 16x16 meters on the ground (this corresponds to the
    /// 3 low-order bytes in X & Y).
    /// </summary>
    class PointIndex
    {
        #region Class data

        /// <summary>
        /// The root node for the spatial index, corresponding to the complete
        /// 64-bit coordinate space.
        /// </summary>
        Node m_Root;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a <c>PointIndex</c> that contains nothing.
        /// </summary>
        internal PointIndex()
        {
            Extent all = new Extent();
            m_Root = new DataNode(all);
        }

        #endregion

        /// <summary>
        /// Adds a point into this index.
        /// </summary>
        /// <param name="p">The index to add</param>
        internal void Add(IPoint p)
        {
            m_Root = m_Root.Add(p, 0);
        }

        /// <summary>
        /// Removes a point from this index.
        /// </summary>
        /// <param name="p">The point to remove</param>
        /// <returns>True if the point was removed. False if it couldn't be found</returns>
        internal bool Remove(IPoint p)
        {
            return m_Root.Remove(p, 0);
        }

        /// <summary>
        /// Is this index empty (containing no points whatsoever)?
        /// </summary>
        internal bool IsEmpty
        {
            get { return m_Root.IsEmpty; }
        }

        /// <summary>
        /// Queries this index
        /// </summary>
        /// <param name="searchExtent">The area to search for</param>
        /// <param name="itemHandler">The method to call for every point that
        /// falls within the search extent</param>
        internal void Query(Extent searchExtent, ProcessItem itemHandler)
        {
            m_Root.Query(searchExtent, itemHandler);
        }

        /// <summary>
        /// Draws the extent of the nodes in this index (excluding the higher level nodes).
        /// For experimentation. 
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The drawing tools to use</param>
        internal void Render(ISpatialDisplay display, IDrawStyle style)
        {
            m_Root.Render(display, style);
        }

        /// <summary>
        /// Collects statistical information about this index. For experimentation.
        /// </summary>
        /// <param name="stats">The stats to fill in.</param>
        internal void CollectStats(PointIndexStatistics stats)
        {
            m_Root.CollectStats(stats);
        }
    }
}
