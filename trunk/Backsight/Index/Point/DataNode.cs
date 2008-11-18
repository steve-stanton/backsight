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
using System.Collections.Generic;

namespace Backsight.Index.Point
{
	/// <written by="Steve Stanton" on="12-JAN-2007" />
    /// <summary>
    /// A node in a <see cref="PointIndex"/> that is associated with
    /// a list of <see cref="IPoint"/>. Nodes of this type normally contain no
    /// more than 100 points (on reaching a count of 100, the node is replaced
    /// with an instance of <see cref="IndexNode"/>). The exception is a situation
    /// where we have more than 100 points in a node at the maximum depth of the
    /// index tree - however, given that the coverage of a low-level node is
    /// approx 16x16 meters on the ground, it is highly unlikely that this will
    /// arise, especially in a cadastral mapping application.
    /// </summary>
    class DataNode : Node
    {
        #region Class data

        /// <summary>
        /// The points enclosed by this node.
        /// </summary>
        readonly List<IPoint> m_Items;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataNode"/> class
        /// that contains no points.
        /// </summary>
        /// <param name="w">The covering rectangle of the node</param>
        internal DataNode(Extent w) : base(w)
        {
            m_Items = new List<IPoint>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataNode"/> class
        /// that contains a single point.
        /// </summary>
        /// <param name="w">The covering rectangle of the node</param>
        /// <param name="p">The point to remember as part of the node</param>
        internal DataNode(Extent w, IPoint p)
            : base(w)
        {
            m_Items = new List<IPoint>(1);
            m_Items.Add(p);
        }

        #endregion

        /// <summary>
        /// Adds a point to this node
        /// </summary>
        /// <param name="p">The point to add to the index</param>
        /// <param name="depth">The 0-based depth of the current node (specify 0
        /// when dealing with the root node)</param>
        /// <returns>
        /// The node containing the node in which the point was stored (either
        /// <c>this</c> node, or a new instance of <see cref="IndexNode"/>).
        /// The current node is used so long as the number of items in the
        /// node is less than 100 (or this node is at the maximum depth of
        /// the spatial indexing tree).
        /// </returns>
        internal override Node Add(IPoint p, int depth)
        {
            m_Items.Add(p);
            if (m_Items.Count < 100 || depth==Node.MAX_DEPTH)
                return this;

            return new IndexNode(this.Window, m_Items, depth);
        }

        /// <summary>
        /// Removes a point from this indexing node
        /// </summary>
        /// <param name="p">The point to remove from the index</param>
        /// <param name="depth">The 0-based depth of the current node (specify 0
        /// when dealing with the root node)</param>
        /// <returns>
        /// True if a point was removed. False if the supplied
        /// instance was not found.
        /// </returns>
        internal override bool Remove(IPoint p, int depth)
        {
            return m_Items.Remove(p);
        }

        /// <summary>
        /// Is this node empty (containing no points)
        /// </summary>
        internal override bool IsEmpty
        {
            get { return (m_Items.Count==0); }
        }

        /// <summary>
        /// Queries the specified search extent. If this node is completely
        /// enclosed by the search extent, the <see cref="PassAll"/> method gets
        /// called to process all points with no further spatial checks. For
        /// nodes that partially overlap, each point is checked against the
        /// search extent.
        /// </summary>
        /// <param name="searchExtent">The search extent.</param>
        /// <param name="itemHandler">The method that should be called for each query hit (any
        /// node with an extent that overlaps the query window)</param>
        internal override void Query(Extent searchExtent, ProcessItem itemHandler)
        {
            if (this.Window.IsOverlap(searchExtent))
            {
                if (this.Window.IsEnclosedBy(searchExtent))
                {
                    PassAll(itemHandler);
                }
                else
                {
                    foreach (IPoint p in m_Items)
                    {
                        if (searchExtent.IsOverlap(p.Geometry))
                        {
                            bool keepGoing = itemHandler(p);
                            if (!keepGoing)
                                return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Processes every point associated with this node.
        /// This will be called by the <see cref="Query"/> method in a situation
        /// where the node is completely enclosed by a search extent.
        /// </summary>
        /// <param name="itemHandler">The method that should be called for every point</param>
        /// <remarks>This method should really return a <c>bool</c>, by being sensitive to
        /// the return value from the item handler (as it is, we may end up processing more
        /// data than we need to)</remarks>
        internal override void PassAll(ProcessItem itemHandler)
        {
            foreach (IPoint p in m_Items)
            {
                bool keepGoing = itemHandler(p);
                if (!keepGoing)
                    return;
            }
        }

        /// <summary>
        /// The number of point features cross-referenced to this node.
        /// </summary>
        internal override uint Count
        {
            get { return (uint)m_Items.Count; }
        }

        /// <summary>
        /// Collects statistics for this node (for use in experimentation)
        /// </summary>
        /// <param name="stats">The stats to update</param>
        internal override void CollectStats(PointIndexStatistics stats)
        {
            stats.Add(this);
        }
    }
}
