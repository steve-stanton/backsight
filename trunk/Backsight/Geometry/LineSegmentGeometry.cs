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

namespace Backsight.Geometry
{
	/// <written by="Steve Stanton" on="14-JUN-2007" />
    /// <summary>
    /// The geometry for a line segment.
    /// </summary>
    public class LineSegmentGeometry : ILineGeometry, ILineSegmentGeometry
    {
        #region Class data

        /// <summary>
        /// The position of the start of the line.
        /// </summary>
        private readonly IPointGeometry m_Start;

        /// <summary>
        /// The position of the end of the line.
        /// </summary>
        private readonly IPointGeometry m_End;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates new <c>SegmentGeometry</c> that corresponds to the supplied positions.
        /// </summary>
        /// <param name="start">The position of the start of the line.</param>
        /// <param name="end">The position of the end of the line.</param>
        public LineSegmentGeometry(IPosition start, IPosition end)
        {
            m_Start = PointGeometry.Create(start);
            m_End = PointGeometry.Create(end);
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="g">The geometry to copy</param>
        public LineSegmentGeometry(ILineSegmentGeometry g)
        {
            m_Start = g.Start;
            m_End = g.End;
        }

        #endregion

        /// <summary>
        /// The position of the start of the line (implements ILineGeometry)
        /// </summary>
        public IPointGeometry Start
        {
            get { return m_Start; }
        }

        /// <summary>
        /// The position of the end of the line (implements ILineGeometry)
        /// </summary>
        public IPointGeometry End
        {
            get { return m_End; }
        }

        public void Render(ISpatialDisplay display, IDrawStyle style)
        {
            LineSegmentGeometry.Render(this, display, style);
        }

        public static void Render(ILineSegmentGeometry g, ISpatialDisplay display, IDrawStyle style)
        {
            style.Render(display, new IPosition[] { g.Start, g.End });
        }

        public static void Render(IPosition start, IPosition end, ISpatialDisplay display, IDrawStyle style)
        {
            style.Render(display, new IPosition[] { start, end });
        }

        public IWindow Extent
        {
            get { return LineSegmentGeometry.GetExtent(this); }
        }

        public static IWindow GetExtent(ILineSegmentGeometry g)
        {
            return new Window(g.Start, g.End);
        }

        public ILength Distance(IPosition point)
        {
            return LineSegmentGeometry.GetDistance(this, point);
        }

        public static ILength GetDistance(ILineSegmentGeometry g, IPosition point)
        {
            double xp, yp;
            BasicGeom.GetPerpendicular(point.X, point.Y, g.Start.X, g.Start.Y, g.End.X, g.End.Y, out xp, out yp);
            double d = BasicGeom.Distance(new Position(xp, yp), point);
            return new Length(d);
        }

        /// <summary>
        /// The length of the line (on the map projection). (implements ILineGeometry)
        /// </summary>
        public ILength Length
        {
            get { return LineSegmentGeometry.GetLength(this); }
        }

        public static ILength GetLength(ILineSegmentGeometry g)
        {
            double d = BasicGeom.Distance(g.Start, g.End);
            return new Length(d);
        }

        /// <summary>
        /// Gets the position that is a specific distance from the start of a line segment.
        /// </summary>
        /// <param name="g">The line segment</param>
        /// <param name="dist">The distance from the start of the segment.</param>
        /// <param name="result">The position found</param>
        /// <returns>True if the distance is somewhere ON the line. False if the distance
        /// was less than zero, or more than the line length (in that case, the position
        /// found corresponds to the corresponding terminal point).</returns>
        public static bool GetPosition(ILineSegmentGeometry g, double d, out IPosition result)
        {
            return GetPosition(g.Start, g.End, d, out result);
        }

        public static bool GetPosition(IPosition start, IPosition end, double d, out IPosition result)
        {
            // Check for distance that is real close to the start (less
            // than 1 micron or so, since that's the smallest number we
            // can store).
            if (d < 0.000002)
            {
                result = start;
                return (d>=0.0);
            }

            // Get length of the line
            double len = BasicGeom.Distance(start, end);

            // If the required distance is within the limiting tolerance
            // of the end of the line (or beyond it), return the end.
            if (d>len || (len-d)<0.000002)
            {
                result = end;
                return (d<=len);
            }

            // How far up the line do we need to go?
            double ratio = d/len;

            // Figure out the position
            double x1 = start.X;
            double y1 = start.Y;
            double x2 = end.X;
            double y2 = end.Y;
            double dx = x2-x1;
            double dy = y2-y1;
            result = new Position(x1+ratio*dx, y1+ratio*dy);

            return true;
        }
    }
}
