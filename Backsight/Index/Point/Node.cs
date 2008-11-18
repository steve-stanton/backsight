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
	/// <written by="Steve Stanton" on="11-JAN-2007" />
    /// <summary>A node in a point index tree. Base class for
    /// <see cref="IndexNode"/> and <see cref="DataNode"/></summary>
    abstract class Node
    {
        #region Constants

        /// <summary>
        /// The size of a node window at a given depth. 64-bit space
        /// is initially divided into 256x256 (65536) cells, but as you drill
        /// down, the division becomes less and less prolific. On reaching a
        /// ground coverage of approx 64x64m, it becomes a quad tree.
        /// </summary>
        internal static ulong[] SIZES =
            {   0xFFFFFFFFFFFFFFFF
            ,   0x00FFFFFFFFFFFFFF // 8 bits => 256x256
            ,   0x0001FFFFFFFFFFFF // 7 bits => 128x128
            ,   0x000007FFFFFFFFFF // 6 bits => 64x64  
            ,   0x0000003FFFFFFFFF // 4 bits => 16x16
            ,   0x00000003FFFFFFFF // 3 bits => 8x8   
            ,   0x000000007FFFFFFF // 2 bits => 4x4  
            ,   0x000000001FFFFFFF // 2 bits => 4x4
            ,   0x0000000007FFFFFF // 1 bit  => 2x2
            ,   0x0000000003FFFFFF // 1 bit  => 2x2
            ,   0x0000000001FFFFFF // 1 bit  => 2x2
            ,   0x0000000000FFFFFF // 1 bit  => 2x2
            };

        /// <summary>
        /// The array index of the last element in the <c>SIZES</c> array
        /// </summary>
        internal static int MAX_DEPTH = (SIZES.Length-1);

        #endregion

        #region Class data

        /// <summary>
        /// The covering rectangle of this node
        /// </summary>
        private readonly Extent m_Window;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        /// <param name="w">The covering rectangle of the node</param>
        protected Node(Extent w)
        {
            m_Window = w;
        }

        #endregion

        /// <summary>
        /// The covering rectangle of this node
        /// </summary>
        internal Extent Window
        {
            get { return m_Window; }
        }

        /// <summary>
        /// Adds a point to this indexing node
        /// </summary>
        /// <param name="p">The point to add to the index</param>
        /// <param name="depth">The 0-based depth of the current node (specify 0
        /// when dealing with the root node)</param>
        /// <returns>The node containing the node in which the point was stored (at
        /// the next level of the spatial index).</returns>
        internal abstract Node Add(IPoint p, int depth);

        /// <summary>
        /// Removes a point from this indexing node (or a child of this node)
        /// </summary>
        /// <param name="p">The point to remove from the index</param>
        /// <param name="depth">The 0-based depth of the current node (specify 0
        /// when dealing with the root node)</param>
        /// <returns>True if a point was removed. False if the supplied
        /// instance was not found.</returns>
        internal abstract bool Remove(IPoint p, int depth);

        /// <summary>
        /// Is this node empty (containing no items)
        /// </summary>
        internal abstract bool IsEmpty { get; }

        /// <summary>
        /// Queries the specified search extent.
        /// </summary>
        /// <param name="searchExtent">The search extent.</param>
        /// <param name="itemHandler">The method that should be called for each query hit (any
        /// node with an extent that overlaps the query window)</param>
        internal abstract void Query(Extent searchExtent, ProcessItem itemHandler);

        /// <summary>
        /// Processes every item related to this node (including any child nodes).
        /// This will be called by the <see cref="Query"/> method in a situation
        /// where the node is completely enclosed by a search extent.
        /// </summary>
        /// <param name="itemHandler">The method that should be called for every point</param>
        /// <remarks>This method should really return a <c>bool</c>, by being sensitive to
        /// the return value from the item handler (as it is, we may end up processing more
        /// data than we need to)</remarks>
        internal abstract void PassAll(ProcessItem itemHandler);

        /// <summary>
        /// The number of items in this node. What consititutes an "item" depends on the
        /// derived class (it's either the number of points, or the number of child nodes).
        /// </summary>
        internal abstract uint Count { get; }

        /// <summary>
        /// Collects statistics for this node (for use in experimentation)
        /// </summary>
        /// <param name="stats">The stats to update</param>
        internal abstract void CollectStats(PointIndexStatistics stats);

        /// <summary>
        /// Draws the outline of this indexing node to a specific display (so
        /// long as the extent of the node isn't too big)
        /// </summary>
        /// <param name="display">The display to draw to.</param>
        /// <param name="style">The style for the draw.</param>
        internal virtual void Render(ISpatialDisplay display, IDrawStyle style)
        {
            if (m_Window.Width < 0x0000010000000000)
            {
                IPosition[] outline = m_Window.Outline;
                style.Render(display, outline);
            }
        }
    }
}
