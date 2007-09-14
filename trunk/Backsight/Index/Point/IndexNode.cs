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
        private readonly List<Node> m_Children;

        #endregion

        #region Constructors

        internal IndexNode(Extent w, List<IPoint> points, int depth) : base(w)
        {
            Debug.Assert(depth>=0 && depth<=Node.MAX_DEPTH);
            m_Children = new List<Node>(1);
            foreach(IPoint p in points)
                this.Add(p, depth);
        }

        #endregion

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

        internal override bool Remove(IPoint p, int depth)
        {
            Extent w = CreateWindow(p, depth+1);
            Node n = FindNode(w);
            if (n==null)
                return false;

            return n.Remove(p, depth+1);
        }

        private Node FindNode(Extent w)
        {
            foreach (Node n in m_Children)
            {
                if (n.Window.Equals(w))
                    return n;
            }

            return null;
        }

        private Extent CreateWindow(IPoint p, int depth)
        {
            if (depth==0)
                return new Extent();

            ulong x = SpatialIndex.ToUnsigned(p.PointGeometry.Easting.Microns);
            ulong y = SpatialIndex.ToUnsigned(p.PointGeometry.Northing.Microns);

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

        internal override bool IsEmpty
        {
            get { return (m_Children.Count==0); }
        }

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

        internal override void PassAll(ProcessItem itemHandler)
        {
            foreach (Node n in m_Children)
                n.PassAll(itemHandler);
        }

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

        internal override void CollectStats(PointIndexStatistics stats)
        {
            stats.Add(this);

            foreach (Node child in m_Children)
                child.CollectStats(stats);
        }
    }
}
