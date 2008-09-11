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
using System.IO;

namespace Backsight.Index.Rectangle
{
    /// <summary>
    /// Information about the spatial index, for use in experimentation
    /// </summary>
    class IndexStatistics
    {
        /// <summary>
        /// The total number of <c>Node</c> instances (includes parents)
        /// </summary>
        uint m_NumNode;

        /// <summary>
        /// The total number of <c>ParentNode</c> instances
        /// </summary>
        uint m_NumParentNode;

        /// <summary>
        /// The maximum number of items in a node
        /// </summary>
        uint m_MaxItemPerNode;

        /// <summary>
        /// The total number of items
        /// </summary>
        uint m_NumItem;

        /// <summary>
        /// The smallest dimension of a node
        /// </summary>
        ulong m_MinSize;

        /// <summary>
        /// The number of empty nodes (without any items)
        /// </summary>
        uint m_NumEmptyNode;

        internal IndexStatistics()
        {
            m_MinSize = ulong.MaxValue;
        }

        internal void Add(Node n)
        {
            m_NumNode++;
            if (n is RectangleIndex)
                m_NumParentNode++;

            m_MaxItemPerNode = Math.Max(m_MaxItemPerNode, n.ItemCount);
            m_NumItem += n.ItemCount;
            if (n.ItemCount==0)
                m_NumEmptyNode++;

            m_MinSize = Math.Min(m_MinSize, n.Window.Width);
            m_MinSize = Math.Min(m_MinSize, n.Window.Height);
        }

        internal void Dump(StreamWriter sw)
        {
            sw.WriteLine("Total node count:     "+m_NumNode);
            sw.WriteLine("Number of leaf nodes: "+(m_NumNode-m_NumParentNode));
            sw.WriteLine("MaxItemPerNode:       "+m_MaxItemPerNode);
            sw.WriteLine("NumEmptyNode:         "+m_NumEmptyNode);
            sw.WriteLine("NumItems:             "+m_NumItem);
            sw.WriteLine("Avg item/node:        "+ (double)m_NumItem / (double)m_NumNode);
            sw.WriteLine("Min size:             "+m_MinSize);
        }
    }
}
