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
    /// <summary>A node in an index tree</summary>
    /// <seealso cref="ParentNode"/>
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
        /// <param name="itemHandler">The item handler.</param>
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

        [Obsolete("Use AddItem instead")]
        internal virtual void Add(Item item)
        {
            AddItem(item);
        }

        /// <summary>
        /// Add an item to this indexing node
        /// </summary>
        /// <param name="item">The item to reference as part of this node</param>
        internal virtual void AddItem(Item item)
        {
            if (m_Items==null)
                m_Items = new List<Item>(1);

            m_Items.Add(item);
        }

        internal virtual bool Remove(Item item)
        {
            if (item.Window.IsEnclosedBy(this.m_Window))
                return RemoveItem(item);
            else
                return false;
        }

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

        internal virtual void CollectStats(IndexStatistics stats)
        {
            stats.Add(this);
        }

        internal uint ItemCount
        {
            get { return (m_Items==null ? 0 : (uint)m_Items.Count); }
        }

        internal virtual void Render(ISpatialDisplay display, IDrawStyle style)
        {
            if (m_Window.Width < 0x0000010000000000)
            {
                IPosition[] outline = m_Window.Outline;
                style.Render(display, outline);
            }
        }

        internal virtual bool IsEmpty
        {
            get { return (this.ItemCount==0); }
        }
    }
}
