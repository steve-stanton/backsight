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
using System.Diagnostics;

using Backsight.Index.Point;
using Backsight.Index.Rectangle;
using Backsight.Forms;

namespace Backsight.Index
{
	/// <written by="Steve Stanton" on="15-DEC-2006" />
    /// <summary>
    /// A spatial index
    /// </summary>
    public class SpatialIndex : IEditSpatialIndex
    {
        #region Class data

        /// <summary>
        /// Index of spatial objects with point geometry
        /// </summary>
        readonly PointIndex m_Points;

        /// <summary>
        /// Index of spatial objects with line geometry
        /// </summary>
        readonly RectangleIndex m_Lines;

        /// <summary>
        /// Index of spatial objects with text geometry (annotations)
        /// </summary>
        readonly RectangleIndex m_Text;

        /// <summary>
        /// Index of polygons
        /// </summary>
        readonly RectangleIndex m_Polygons;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SpatialIndex"/> class
        /// with nothing in it.
        /// </summary>
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

        /// <summary>
        /// Adds a spatial object into the index
        /// </summary>
        /// <param name="o">The object to add to the index</param>
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
                m_Lines.AddItem(item);
            else if (t == SpatialType.Text)
                m_Text.AddItem(item);
            else if (t == SpatialType.Polygon)
                m_Polygons.AddItem(item);
            else
                throw new NotSupportedException("Unexpected object type: "+o.SpatialType);
        }

        /// <summary>
        /// Removes a spatial object from the index
        /// </summary>
        /// <param name="o">The object to remove from the index</param>
        /// <returns>
        /// True if object removed. False if it couldn't be found.
        /// </returns>
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

        // <summary>
        // Dumps out debug information relating to this index, creating
        // a file called <c>C:\Temp\index.txt</c>
        // </summary>
        //public void Dump()
        //{
        //    using (StreamWriter w = File.CreateText(@"C:\Temp\index.txt"))
        //    {
        //        w.WriteLine("Lines");
        //        m_Lines.Dump(w, 0);

        //        w.WriteLine();
        //        w.WriteLine("Text");
        //        m_Text.Dump(w, 0);

        //        w.WriteLine();
        //        w.WriteLine("Polygons");
        //        m_Polygons.Dump(w, 0);
        //    }
        //}

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

        /// <summary>
        /// Dumps out debug statistics relating to this index, creating
        /// a file called <c>C:\Temp\indexStats.txt</c>
        /// </summary>
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

        /// <summary>
        /// Locates the feature closest to a specific position. Ignores polygons.
        /// </summary>
        /// <param name="p">The search position</param>
        /// <param name="radius">The search radius</param>
        /// <param name="types">The type(s) of object to look for (if you include polygons as
        /// an applicable type, they will be quietly ignored).</param>
        /// <returns>
        /// The closest feature of the requested type (null if nothing found)
        /// </returns>
        public virtual ISpatialObject QueryClosest(IPosition p, ILength radius, SpatialType types)
        {
            return new FindClosestQuery(this, p, radius, types).Result;           
        }

        /// <summary>
        /// Process items with a covering rectangle that overlaps a query window.
        /// </summary>
        /// <param name="extent">The extent of the query window</param>
        /// <param name="types">The type(s) of object to look for</param>
        /// <param name="itemHandler">The method that should be called for each query hit. A hit
        /// is defined as anything with a covering rectangle that overlaps the query window (this
        /// does not mean the hit actually intersects the window).</param>
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

        /// <summary>
        /// Draws a representation of the internal structure of this index, for
        /// use in experimentation.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        public void Draw(ISpatialDisplay display)
        {
            DrawStyle style = new DrawStyle();
            style.Pen.Color = System.Drawing.Color.Crimson;
            //m_Points.Render(display, style);
            //m_Lines.Render(display, style, 0);
            m_Polygons.Render(display, style);
        }

        /// <summary>
        /// Is the index empty (containing nothing)?
        /// </summary>
        public bool IsEmpty
        {
            get { return (m_Points.IsEmpty && m_Lines.IsEmpty && m_Text.IsEmpty && m_Polygons.IsEmpty); }
        }

        /// <summary>
        /// Utility method that converts a signed value to unsigned (the numbering
        /// system that the spatial index is based on).
        /// </summary>
        /// <param name="val">The signed value to convert (most likely to be an X or Y
        /// value in microns)</param>
        /// <returns>The corresponding unsigned value</returns>
        internal static ulong ToUnsigned(long val)
        {
            return (ulong)(val) ^ 0x8000000000000000;
        }
    }
}
