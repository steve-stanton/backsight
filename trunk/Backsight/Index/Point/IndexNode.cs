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
using System.Diagnostics;

namespace Backsight.Index.Point
{
	/// <written by="Steve Stanton" on="12-JAN-2007" />
    /// <summary>
    /// A node that represents a split in the index tree.
    /// </summary>
    class IndexNode : Node
    {
        #region Class data

        /// <summary>
        /// Child nodes. The maximum number of nodes in this list depends on the tree
        /// depth (at more general levels, the potential number of children is greater).
        /// </summary>
        readonly List<Node> m_Children;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="IndexNode"/> (called when an
        /// instance of <see cref="DataNode"/> reaches its split threshold).
        /// </summary>
        /// <param name="w">The spatial extent of the node</param>
        /// <param name="points">The points that need to be indexed</param>
        /// <param name="depth">The 0-based depth of this node (0 for root)</param>
        internal IndexNode(Extent w, List<IPoint> points, int depth)
            : base(w)
        {
            Debug.Assert(depth>=0 && depth<=Node.MAX_DEPTH);
            m_Children = new List<Node>(1);
            foreach(IPoint p in points)
                this.Add(p, depth);
        }

        #endregion

        /// <summary>
        /// Adds a point to this indexing node. This will either add the point
        /// to a new child node, or add to an existing child (and, if the
        /// child is an index node, that may cause a node split).
        /// </summary>
        /// <param name="p">The point to add to the index</param>
        /// <param name="depth">The 0-based depth of the current node (specify 0
        /// when dealing with the root node)</param>
        /// <returns>The node containing the node in which the point was stored -
        /// always <c>this</c> instance.</returns>
        internal override Node Add(IPoint p, int depth)
        {
            Extent w = CreateWindow(p, depth+1);
            Node n = FindNode(w);

            if (n==null)
            {
                n = new DataNode(w, p);
                m_Children.Add(n);
            }
            else
            {
                Node result = n.Add(p, depth+1);
                if (!Object.ReferenceEquals(n, result))
                {
                    m_Children.Remove(n);
                    m_Children.Add(result);
                }
            }

            return this;
        }

        /// <summary>
        /// Removes a point from a child of this indexing node
        /// </summary>
        /// <param name="p">The point to remove from the index</param>
        /// <param name="depth">The 0-based depth of the current node (specify 0
        /// when dealing with the root node)</param>
        /// <returns>
        /// True if a point was removed. False if the supplied
        /// instance was not found in any child.
        /// </returns>
        internal override bool Remove(IPoint p, int depth)
        {
            Extent w = CreateWindow(p, depth+1);
            Node n = FindNode(w);
            if (n==null)
                return false;

            return n.Remove(p, depth+1);
        }

        /// <summary>
        /// Attempts to locate the child node with a specific extent
        /// </summary>
        /// <param name="w">The spatial extent to look for</param>
        /// <returns>The child node with an extent that is precisely coincident
        /// with the supplied extent (null if not found).</returns>
        Node FindNode(Extent w)
        {
            foreach (Node n in m_Children)
            {
                if (n.Window.Equals(w))
                    return n;
            }

            return null;
        }

        /// <summary>
        /// Creates the covering rectangle of the node that encloses
        /// a point at a given depth in the index tree.
        /// </summary>
        /// <param name="p">The point of interest</param>
        /// <param name="depth">The 0-based tree depth (specify 0 when dealing
        /// with the root node)</param>
        /// <returns>The covering rectangle enclosing the point at the requested depth</returns>
        Extent CreateWindow(IPoint p, int depth)
        {
            if (depth==0)
                return new Extent();

            ulong x = SpatialIndex.ToUnsigned(p.Geometry.Easting.Microns);
            ulong y = SpatialIndex.ToUnsigned(p.Geometry.Northing.Microns);

            // The lower-left corner is given by masking out the low-order bits (neat)
            ulong size = Node.SIZES[depth];
            ulong mask = size ^ 0xFFFFFFFFFFFFFFFF;

            ulong minx = (x & mask);
            ulong miny = (y & mask);
            ulong maxx = minx + size;
            ulong maxy = miny + size;

            Debug.Assert(maxx>minx);
            Debug.Assert(maxy>miny);

            return new Extent(minx, miny, maxx, maxy);
        }

        /// <summary>
        /// Is this node empty (containing no child nodes)
        /// </summary>
        internal override bool IsEmpty
        {
            get { return (m_Children.Count==0); }
        }

        /// <summary>
        /// Queries the specified search extent. For child nodes that are completely
        /// enclosed by the search extent, the <see cref="PassAll"/> method gets
        /// called to process all descendants with no further spatial checks. For
        /// child nodes that partially overlap, this method will be called recursively.
        /// </summary>
        /// <param name="searchExtent">The search extent.</param>
        /// <param name="itemHandler">The method that should be called for each query hit (any
        /// node with an extent that overlaps the query window)</param>
        internal override void Query(Extent searchExtent, ProcessItem itemHandler)
        {
            foreach (Node n in m_Children)
            {
                if (n.Window.IsEnclosedBy(searchExtent))
                    n.PassAll(itemHandler);
                else
                    n.Query(searchExtent, itemHandler);
            }
        }

        /// <summary>
        /// Processes every child node associated with this index node.
        /// This will be called by the <see cref="Query"/> method in a situation
        /// where the node is completely enclosed by a search extent.
        /// </summary>
        /// <param name="itemHandler">The method that should be called for every point</param>
        /// <remarks>This method should really return a <c>bool</c>, by being sensitive to
        /// the return value from the item handler (as it is, we may end up processing more
        /// data than we need to)</remarks>
        internal override void PassAll(ProcessItem itemHandler)
        {
            foreach (Node n in m_Children)
                n.PassAll(itemHandler);
        }

        /// <summary>
        /// Draws the outline of all child nodes (if the child is also an
        /// instance of <see cref="IndexNode"/>, this will just cause further
        /// drilldown - something will only get drawn if the child is an
        /// instance of some other class).
        /// </summary>
        /// <param name="display">The display to draw to.</param>
        /// <param name="style">The style for the draw.</param>
        internal override void Render(ISpatialDisplay display, IDrawStyle style)
        {
            foreach (Node n in m_Children)
                n.Render(display, style);
        }

        /// <summary>
        /// The number of child nodes associated with this node.
        /// </summary>
        internal override uint Count
        {
            get { return (uint)m_Children.Count; }
        }

        /// <summary>
        /// Collects statistics for this node and all children (for use in experimentation)
        /// </summary>
        /// <param name="stats">The stats to update</param>
        internal override void CollectStats(PointIndexStatistics stats)
        {
            stats.Add(this);

            foreach (Node child in m_Children)
                child.CollectStats(stats);
        }
    }
}
