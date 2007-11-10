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

using Backsight.Index.Point;
using Backsight.Index.Rectangle;
using System.Diagnostics;

namespace Backsight.Index
{
	/// <written by="Steve Stanton" on="15-DEC-2006" />
    /// <summary>
    /// A spatial index
    /// </summary>
    public class SpatialIndex : IEditSpatialIndex
    {
        #region Class data

        private readonly PointIndex m_Points;
        private readonly RectangleIndex m_Lines;
        private readonly RectangleIndex m_Text;
        private readonly RectangleIndex m_Polygons;

        #endregion

        #region Constructors

        public SpatialIndex()
        {
            Extent all = new Extent();

            m_Points = new PointIndex();
            m_Lines = new RectangleIndex(all);
            m_Text = new RectangleIndex(all);
            m_Polygons = new RectangleIndex(all);
        }

        #endregion

        #region IEditSpatialIndex Members

        public void Add(ISpatialObject o)
        {
            // Points are handled using their own index. Unfortunately, they
            // must implement IPoint in order for it to work (something
            // that should really be reflected in the parameter list).

            // Could cover this by using the fact that an ISpatialObject that is
            // a point must be able to return an IWindow (from which we can
            // pull out the point position). Just a bit more convoluted.

            if (o is IPoint)
            {
                m_Points.Add(o as IPoint);
                return;
            }

            Item item = new Item(o);
            SpatialType t = o.SpatialType;

            if (t == SpatialType.Line)
                m_Lines.Add(item);
            else if (t == SpatialType.Text)
                m_Text.Add(item);
            else if (t == SpatialType.Polygon)
                m_Polygons.Add(item);
            else
                throw new NotSupportedException("Unexpected object type: "+o.SpatialType);
        }

        public bool Remove(ISpatialObject o)
        {
            if (o is IPoint)
                return m_Points.Remove(o as IPoint);

            Item item = new Item(o);
            SpatialType t = o.SpatialType;

            if (t == SpatialType.Line)
                return m_Lines.Remove(item);

            if (t == SpatialType.Text)
            {
                bool isRemoved = m_Text.Remove(item);
                Debug.Assert(isRemoved);
                return isRemoved;
            }

            if (t == SpatialType.Polygon)
                return m_Polygons.Remove(item);

            throw new NotSupportedException("Unexpected object type: "+o.SpatialType);
        }

        #endregion

        public void Dump()
        {
            using (StreamWriter w = File.CreateText(@"C:\Temp\index.txt"))
            {
                w.WriteLine("Lines");
                m_Lines.Dump(w, 0);

                w.WriteLine();
                w.WriteLine("Text");
                m_Text.Dump(w, 0);

                w.WriteLine();
                w.WriteLine("Polygons");
                m_Polygons.Dump(w, 0);
            }
        }

        /// <summary>
        /// Obtains the number of points in this index
        /// </summary>
        /// <returns>The number of points</returns>
        public uint GetPointCount()
        {
            PointIndexStatistics statPoints = new PointIndexStatistics();
            m_Points.CollectStats(statPoints);
            return statPoints.PointCount;
        }

        public void DumpStats()
        {            
            using (StreamWriter w = File.CreateText(@"C:\Temp\indexStats.txt"))
            {
                PointIndexStatistics statPoints = new PointIndexStatistics();
                w.WriteLine("Points");
                m_Points.CollectStats(statPoints);
                statPoints.Dump(w);

                IndexStatistics statLines = new IndexStatistics();
                w.WriteLine();
                w.WriteLine("Lines");
                m_Lines.CollectStats(statLines);
                statLines.Dump(w);

                IndexStatistics statText = new IndexStatistics();
                w.WriteLine();
                w.WriteLine("Text");
                m_Text.CollectStats(statText);
                statText.Dump(w);

                IndexStatistics statPol = new IndexStatistics();
                w.WriteLine();
                w.WriteLine("Polygons");
                m_Polygons.CollectStats(statPol);
                statPol.Dump(w);
            }
        }

        public virtual ISpatialObject QueryClosest(IPosition p, ILength radius, SpatialType types)
        {
            return new FindClosestQuery(this, p, radius, types).Result;           
        }

        public void QueryWindow(IWindow extent, SpatialType types, ProcessItem itemHandler)
        {
            Extent w = (extent==null || extent.IsEmpty ? new Extent() : new Extent(extent));

            if ((types & SpatialType.Point)!=0)
                m_Points.Query(w, itemHandler);

            if ((types & SpatialType.Line)!=0)
                m_Lines.Query(w, itemHandler);

            if ((types & SpatialType.Text)!=0)
                m_Text.Query(w, itemHandler);

            if ((types & SpatialType.Polygon)!=0)
                m_Polygons.Query(w, itemHandler);
        }

        // For experimentation
        public void Draw(ISpatialDisplay display)
        {
            Backsight.Forms.DrawStyle style = new Backsight.Forms.DrawStyle();
            style.Pen.Color = System.Drawing.Color.Crimson;
            //m_Points.Render(display, style);
            //m_Lines.Render(display, style, 0);
            m_Polygons.Render(display, style);
        }

        public bool IsEmpty
        {
            get { return (m_Points.IsEmpty && m_Lines.IsEmpty && m_Text.IsEmpty && m_Polygons.IsEmpty); }
        }

        internal static ulong ToUnsigned(long val)
        {
            return (ulong)(val) ^ 0x8000000000000000;
        }
    }
}
