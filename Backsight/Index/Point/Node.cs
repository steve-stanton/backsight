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

namespace Backsight.Index.Point
{
	/// <written by="Steve Stanton" on="11-JAN-2007" />
    /// <summary>A node in a point index tree</summary>
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

        internal static int MAX_DEPTH = (SIZES.Length-1);

        #endregion

        #region Class data

        /// <summary>
        /// The covering rectangle of this node
        /// </summary>
        private readonly Extent m_Window;

        #endregion

        #region Constructors

        protected Node(Extent w)
        {
            m_Window = w;
        }

        #endregion

        internal Extent Window
        {
            get { return m_Window; }
        }

        internal abstract Node Add(IPoint p, int depth);
        internal abstract bool Remove(IPoint p, int depth);
        internal abstract bool IsEmpty { get; }
        internal abstract void Query(Extent searchExtent, ProcessItem itemHandler);
        internal abstract void PassAll(ProcessItem itemHandler);

        /// <summary>
        /// The number of items in this node. What consititutes an "item" depends on the
        /// derived class.
        /// </summary>
        internal abstract uint Count { get; }

        /// <summary>
        /// Collects statistics for this node (for use in experimentation)
        /// </summary>
        /// <param name="stats">The stats to update</param>
        internal abstract void CollectStats(PointIndexStatistics stats);

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
