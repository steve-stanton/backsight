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
using System.IO;

namespace Backsight.Index.Rectangle
{
	/// <written by="Steve Stanton" on="15-DEC-2006" />
    /// <summary>
    /// A spatial index represented by a tree where each level divides space into 16
    /// vertical or horizontal strips (as you drill down, the axis we split will alternate
    /// between X and Y). Each pair of adjacent strips is additionally covered by a
    /// "bridging strip" that covers half of both strips, leading to a total of 31 strips
    /// per level.
    /// <para/>
    /// This aims to be an "adequate" spatial index that is easy to create and
    /// easy to update. While it may not be the most efficient index, it should
    /// provide acceptable results for an in-memory dataset.
    /// <para/>
    /// This index is not intended for points. Use <see cref="PointIndex"/> instead.
    /// </summary>
    class RectangleIndex : Node
    {
        #region Constants

        /// <summary>
        /// The max number of child nodes (basic strips + bridging strips)
        /// </summary>
        const int MAX_CHILD_COUNT = 31; // 15

        /// <summary>
        /// The smallest dimension for a strip
        /// </summary>
        const ulong SMALL = 0x0000000001000000;

        /// <summary>
        /// The width of a root-level strip
        /// </summary>
        const ulong ROOT_STRIP_SIZE = 0x0FFFFFFFFFFFFFFF; //0x1FFFFFFFFFFFFFFF;

        /// <summary>
        /// The number of bits consumed per level
        /// </summary>
        const int BITS_PER_STRIP = 4; //3;

        #endregion

        #region Class data

        /// <summary>
        /// Child nodes (if any), in no particular order.
        /// The maximum number of child nodes is <c>MAX_CHILD_COUNT</c>.
        /// </summary>
        private List<Node> m_Children;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a parent node with no children, and no associated items.
        /// </summary>
        /// <param name="w">The covering rectangle of the node</param>
        internal RectangleIndex(Extent w)
            : base(w)
        {
            m_Children = null;
        }

        #endregion

        internal override void Query(Extent searchExtent, ProcessItem itemHandler)
        {
            base.Query(searchExtent, itemHandler);

            if (m_Children!=null)
            {
                foreach (Node n in m_Children)
                {
                    if (n.Window.IsOverlap(searchExtent))
                        n.Query(searchExtent, itemHandler);
                }
            }
        }

        internal override void Add(Item item)
        {
            // If this node hasn't reached it's provisional capacity, just hold
            // the item here (it will be re-located if we subsequently find that
            // the node is too busy).

            /*
            if (base.IsAcceptingProvisionalPlacements)
            {
                item.HasProvisionalPlacement = true;
                base.Add(item);
            }
            else
            {
                AddItem(item);
            }
            */
            AddItem(item);
        }

        internal override void AddItem(Item item)
        {
            // Obtain a window that represents a child strip
            Extent strip = CreateChildStrip();

            // If the item is bigger than a strip, just add into this node
            if (strip.Width < item.Window.Width || strip.Height < item.Window.Height)
            {
                base.AddItem(item);
                return;
            }

            // Check whether any existing children will accept the item. If not,
            // generate a new child node & add into that.

            Node child = FindChild(item.Window);
            if (child==null)
                child = AddChild(strip, item.Window);

            if (child==null)
                base.AddItem(item);
            else
                child.Add(item);
        }

        internal override bool Remove(Item item)
        {
            if (!item.Window.IsEnclosedBy(this.Window))
                return false;

            if (base.RemoveItem(item))
                return true;

            if (m_Children!=null)
            {
                foreach (Node n in m_Children)
                {
                    if (n.Remove(item))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Attempts to create a child node that encloses the specified window
        /// </summary>
        /// <param name="childStrip">The coverage of the first child of this node</param>
        /// <param name="itemWindow">The window of an item that is being added into this tree</param>
        /// <returns>The created child (null if the item cannot be accepted by any child node)</returns>
        private Node AddChild(Extent childStrip, Extent itemWindow)
        {
            for (int i=0; i<MAX_CHILD_COUNT; i++)
            {
                if (itemWindow.IsEnclosedBy(childStrip))
                    return CreateChild(childStrip);

                ShiftStrip(childStrip);
            }

            return null;
        }

        private void ShiftStrip(Extent w)
        {
            // Shift vertical strips east; shift horizontal strips north

            if (w.Height > w.Width)
                w.Increase((w.Width+1)/2, 0);
            else
                w.Increase(0, (w.Height+1)/2);
        }

        private Node CreateChild(Extent childWindow)
        {
            // If the child is quite small, make it a leaf node (the number here
            // means we effectively ignore the 3 low-order bytes in X & Y, meaning
            // that a leaf node will cover approx 16x16 metres on the ground).

            Node child;

            if (childWindow.Width < SMALL && childWindow.Height < SMALL)
                child = new Node(childWindow);
            else
                child = new RectangleIndex(childWindow);

            if (m_Children==null)
                m_Children = new List<Node>(1);

            m_Children.Add(child);
            return child;
        }

        /// <summary>
        /// Returns a window with a coverage that corresponds to the first child strip
        /// </summary>
        /// <returns></returns>
        private Extent CreateChildStrip()
        {
            Extent w = this.Window;

            ulong minx = w.MinX;
            ulong miny = w.MinY;

            // If this node covers a square area of space, we'll be subdividing along
            // the X-axis (produce a vertical strip). Otherwise subdivide in Y.

            ulong maxx, maxy;

            ulong delta = w.Height;
            delta = (delta==UInt64.MaxValue ? ROOT_STRIP_SIZE : ((delta+1) >> BITS_PER_STRIP)-1);

            if (w.IsSquare)
            {
                // Create vertical strip

                maxx = minx + delta;
                maxy = w.MaxY;
            }
            else
            {
                // Create horizontal strip

                maxx = w.MaxX;
                maxy = miny + delta;
            }

            // Could we have gone past UInt64.MaxValue?
            Debug.Assert(minx<maxx);
            Debug.Assert(miny<maxy);

            return new Extent(minx, miny, maxx, maxy);
        }

        /// <summary>
        /// Attempt to find an existing child node that encloses the specified window
        /// </summary>
        /// <param name="w">The window of interest</param>
        /// <returns>The first child node that entirely encloses the window (null
        /// if nothing found)</returns>
        private Node FindChild(Extent w)
        {
            if (m_Children==null)
                return null;

            foreach (Node n in m_Children)
            {
                if (w.IsEnclosedBy(n.Window))
                    return n;
            }

            return null;
        }

        internal override void Dump(StreamWriter sw, int depth)
        {
            base.Dump(sw, depth);

            string prefix = new String(' ', depth*2);
            if (m_Children!=null)
            {
                sw.WriteLine(prefix+"#children: "+m_Children.Count);
                foreach (Node n in m_Children)
                    n.Dump(sw, depth+1);
            }
        }

        internal override void CollectStats(IndexStatistics stats)
        {
            base.CollectStats(stats);

            if (m_Children!=null)
            {
                foreach (Node n in m_Children)
                    n.CollectStats(stats);
            }
        }

        internal override void Render(ISpatialDisplay display, IDrawStyle style)
        {
            base.Render(display, style);

            if (m_Children!=null)
            {
                foreach (Node n in m_Children)
                    n.Render(display, style);
            }
        }

        internal override bool IsEmpty
        {
            get { return (this.ItemCount==0 && this.ChildCount==0); }
        }

        private uint ChildCount
        {
            get { return (m_Children==null ? 0 : (uint)m_Children.Count); }
        }
    }
}
