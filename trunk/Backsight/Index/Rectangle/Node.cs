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
using System.IO;

namespace Backsight.Index.Rectangle
{
	/// <written by="Steve Stanton" on="15-DEC-2006" />
    /// <summary>A node in an index tree. This is the base class
    /// for <see cref="RectangleIndex"/>.</summary>
    class Node
    {
        #region Class data

        /// <summary>
        /// The items (if any) associated with this node. May be null.
        /// </summary>
        private List<Item> m_Items;

        /// <summary>
        /// The covering rectangle of this node
        /// </summary>
        private readonly Extent m_Window;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a node that isn't associated with any items
        /// </summary>
        /// <param name="w">The covering rectangle of the node</param>
        internal Node(Extent w)
        {
            m_Items = null;
            m_Window = w;
        }

        #endregion

        /// <summary>
        /// Queries the specified search extent.
        /// </summary>
        /// <param name="searchExtent">The search extent.</param>
        /// <param name="itemHandler">The method that should be called for each query hit. A hit
        /// is defined as anything with a covering rectangle that overlaps the query window (this
        /// does not mean the hit actually intersects the window).</param>
        internal virtual void Query(Extent searchExtent, ProcessItem itemHandler)
        {
            if (m_Items!=null)
            {
                foreach (Item i in m_Items)
                {
                    if (i.Window.IsOverlap(searchExtent))
                    {
                        bool keepGoing = itemHandler(i.SpatialObject);
                        if (!keepGoing)
                            return;
                    }
                }
            }
        }

        /// <summary>
        /// The covering rectangle of this node
        /// </summary>
        internal Extent Window
        {
            get { return m_Window; }
        }

        /// <summary>
        /// Adds an item to this indexing node
        /// </summary>
        /// <param name="item">The item to reference as part of this node</param>
        internal virtual void AddItem(Item item)
        {
            if (m_Items==null)
                m_Items = new List<Item>(1);

            m_Items.Add(item);
        }

        /// <summary>
        /// Calls <see cref="RemoveItem"/> if the supplied item is 
        /// enclosed by the extent of this node.
        /// </summary>
        /// <param name="item">The item that may be inside the spatial extent
        /// of this node</param>
        /// <returns>True if <see cref="RemoveItem"/> was called. False if the
        /// item is not completely enclosed by this node.</returns>
        internal virtual bool Remove(Item item)
        {
            if (item.Window.IsEnclosedBy(this.m_Window))
                return RemoveItem(item);
            else
                return false;
        }

        /// <summary>
        /// Removes the index item that refers to a specific spatial object
        /// </summary>
        /// <param name="item">The item referencing the spatial object that needs
        /// to be removed</param>
        /// <returns>True if an item referring to the same spatial object was
        /// removed. False if this node does not contain any items that refer
        /// to the spatial object to be removed.</returns>
        protected bool RemoveItem(Item item)
        {
            if (m_Items==null)
                return false;

            foreach (Item i in m_Items)
            {
                if (Object.ReferenceEquals(i.SpatialObject, item.SpatialObject))
                {
                    m_Items.Remove(i);
                    if (m_Items.Count==0)
                        m_Items = null;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Dumps out information about this indexing node.
        /// </summary>
        /// <param name="sw">The output stream to write to</param>
        /// <param name="depth">The current node depth</param>
        internal virtual void Dump(StreamWriter sw, int depth)
        {
            string prefix = new String(' ', depth*2);
            sw.WriteLine(prefix+"Node: "+m_Window.ToString());
            sw.WriteLine(prefix+"#items: "+(m_Items==null ? 0 : m_Items.Count));

            if (m_Items!=null)
            {
                foreach (Item item in m_Items)
                    sw.WriteLine(prefix+item.Window.ToString());
            }
        }

        /// <summary>
        /// Collects statistics relating to this indexing node, for use
        /// in experimentation.
        /// </summary>
        /// <param name="stats">The statistics to add to</param>
        internal virtual void CollectStats(IndexStatistics stats)
        {
            stats.Add(this);
        }

        /// <summary>
        /// The number of items associated with this node.
        /// </summary>
        internal uint ItemCount
        {
            get { return (m_Items==null ? 0 : (uint)m_Items.Count); }
        }

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

        /// <summary>
        /// Does this indexing node contain any items (checks whether
        /// the <see cref="ItemCount"/> property has a value of 0).
        /// </summary>
        internal virtual bool IsEmpty
        {
            get { return (this.ItemCount==0); }
        }
    }
}
