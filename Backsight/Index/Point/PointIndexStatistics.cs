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
using System.IO;

namespace Backsight.Index.Point
{
	/// <written by="Steve Stanton" on="16-JAN-2007" />
    /// <summary>
    /// Information about the spatial index, for use in experimentation
    /// </summary>
    class PointIndexStatistics
    {
        /// <summary>
        /// The number of <c>DataNode</c> instances
        /// </summary>
        uint m_NumDataNode;

        /// <summary>
        /// The number of <c>IndexNode</c> instances
        /// </summary>
        uint m_NumIndexNode;

        /// <summary>
        /// The maximum number of items in a node
        /// </summary>
        uint m_MaxItemInDataNode;

        /// <summary>
        /// The maximum number of items in an index node
        /// </summary>
        uint m_MaxItemInIndexNode;

        /// <summary>
        /// The total number of point features in the index.
        /// </summary>
        uint m_NumItem;

        /// <summary>
        /// The smallest dimension of a node
        /// </summary>
        ulong m_MinSize;

        internal PointIndexStatistics()
        {
            m_MinSize = ulong.MaxValue;
        }

        internal void Add(Node n)
        {
            if (n is DataNode)
            {
                m_NumDataNode++;
                m_MaxItemInDataNode = Math.Max(m_MaxItemInDataNode, n.Count);
                m_NumItem += n.Count;
            }
            else
            {
                m_NumIndexNode++;
                m_MaxItemInIndexNode = Math.Max(m_MaxItemInIndexNode, n.Count);
            }

            m_MinSize = Math.Min(m_MinSize, n.Window.Width);
        }

        internal void Dump(StreamWriter sw)
        {
            sw.WriteLine("Number of index nodes: "+m_NumIndexNode);
            sw.WriteLine("MaxItemInIndexNode:    "+m_MaxItemInIndexNode);
            sw.WriteLine("Number of data nodes:  "+m_NumDataNode);
            sw.WriteLine("MaxItemInDataNode:     "+m_MaxItemInDataNode);
            sw.WriteLine("Numbe or points:       "+m_NumItem);
            sw.WriteLine("Avg points/data node:  "+ (double)m_NumItem / (double)m_NumDataNode);
            sw.WriteLine("Min size:              "+m_MinSize);
        }
    }
}
