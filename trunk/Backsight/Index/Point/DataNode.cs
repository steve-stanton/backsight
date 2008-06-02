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

namespace Backsight.Index.Point
{
	/// <written by="Steve Stanton" on="12-JAN-2007" />
    class DataNode : Node
    {
        private readonly List<IPoint> m_Items;

        internal DataNode(Extent w) : base(w)
        {
            m_Items = new List<IPoint>();
        }

        internal DataNode(Extent w, IPoint p) : base(w)
        {
            m_Items = new List<IPoint>(1);
            m_Items.Add(p);
        }

        internal override Node Add(IPoint p, int depth)
        {
            m_Items.Add(p);
            if (m_Items.Count < 100 || depth==Node.MAX_DEPTH)
                return this;

            return new IndexNode(this.Window, m_Items, depth);
        }

        internal override bool Remove(IPoint p, int depth)
        {
            return m_Items.Remove(p);
        }

        internal override bool IsEmpty
        {
            get { return (m_Items.Count==0); }
        }

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

        internal override void CollectStats(PointIndexStatistics stats)
        {
            stats.Add(this);
        }
    }
}
