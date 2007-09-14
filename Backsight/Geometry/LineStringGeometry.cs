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
using System.Diagnostics;

namespace Backsight.Geometry
{
	/// <written by="Steve Stanton" on="14-JUN-2007" />
    /// <summary>
    /// The geometry for a series of connected line segments.
    /// </summary>
    public class LineStringGeometry : ILineGeometry, IMultiSegmentGeometry
    {
        #region Class data

        /// <summary>
        /// The positions defining the line.
        /// </summary>
        private readonly IPointGeometry[] m_Data;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates new <c>LineStringGeometry</c> that corresponds to the supplied positions.
        /// </summary>
        /// <param name="data">The positions defining the line.</param>
        public LineStringGeometry(IPointGeometry[] data)
        {
            if (data==null || data.Length<2)
                throw new ArgumentException();

            m_Data = data;
        }

        #endregion

        public IPointGeometry[] Data
        {
            get { return m_Data; }
        }

        public void Render(ISpatialDisplay display, IDrawStyle style)
        {
            LineStringGeometry.Render(this, display, style);
        }

        public static void Render(IMultiSegmentGeometry g, ISpatialDisplay display, IDrawStyle style)
        {
            style.Render(display, g.Data);
        }

        public IWindow Extent
        {
           get { return GetExtent(this); }
        }

        public static IWindow GetExtent(IMultiSegmentGeometry g)
        {
            return new Window(g.Data);
        }

        public ILength Distance(IPosition point)
        {
            return GetDistance(this, point);
        }

        public static ILength GetDistance(IMultiSegmentGeometry g, IPosition p)
        {
            double dsq = BasicGeom.MinDistanceSquared(g.Data, p);
            return new Length(Math.Sqrt(dsq));
        }

        /*

        public ILength GetLength(IPosition asFarAs)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool GetPosition(ILength dist, out IPosition pos)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        */

        /// <summary>
        /// The position of the start of the line (implements ILineGeometry).
        /// </summary>
        public IPointGeometry Start
        {
            get { return m_Data[0]; }
        }

        /// <summary>
        /// The position of the end of the line (implements ILineGeometry).
        /// </summary>
        public IPointGeometry End
        {
            get { return m_Data[m_Data.Length-1]; }
        }

        /// <summary>
        /// The length of the line (on the map projection). (implements ILineGeometry)
        /// </summary>
        public ILength Length
        {
            get { return GetLength(this, null, null); }
        }

        /// <summary>
        /// The length of this line, up to a specific position.
        /// </summary>
        /// <param name="g">The geometry for the line</param>
        /// <param name="asFarAs">Optional position on the line that the distance should
        /// be calculated to (null means return the length for the complete line). If this
        /// position doesn't actually coincide with the line, you'll get the length of the
        /// complete line.</param>
        /// <param name="tol">The tolerance for matching <c>asFarAs</c> with the line. Ignored
        /// if <c>asFarAs</c> is null.</param>
        /// <returns>The length of the specified line geometry</returns>
        public static ILength GetLength(IMultiSegmentGeometry g, IPosition asFarAs, ILength tol)
        {
            IPosition[] line = g.Data;
            double tsq = (tol.Meters * tol.Meters);

            double tx = (asFarAs==null ? 0.0 : asFarAs.X);
            double ty = (asFarAs==null ? 0.0 : asFarAs.Y);

            double x1 = line[0].X;
            double y1 = line[0].Y;

            double length=0.0;		// Total length so far
            bool finish=false;	    // True to break early

            for (int i=1; i<line.Length && !finish; i++)
            {
                double x2 = line[i].X;
                double y2 = line[i].Y;

                if (asFarAs!=null && IsCoincident(tx, ty, x1, y1, x2, y2, tsq))
                {
                    x2 = tx;
                    y2 = ty;
                    finish = true;
                }

                double dx = x2-x1;
                double dy = y2-y1;
                length += Math.Sqrt(dx*dx+dy*dy);
                x1 = x2;
                y1 = y2;
            }

            return new Length(length);
        }

        /// <summary>
        /// Checks whether a position is coincident with a line segment, using a tolerance that's
        /// consistent with the resolution of data.
        /// </summary>
        /// <param name="testX">The position to test</param>
        /// <param name="testY"></param>
        /// <param name="xs">Start of line segment.</param>
        /// <param name="ys"></param>
        /// <param name="xe">End of line segment.</param>
        /// <param name="ye"></param>
        /// <param name="tsq">The tolerance (squared) to use for checking whether the test point is
        /// coincident with the segment.</param>
        /// <returns>True if the distance from the test position to the line segment is less
        /// than the specified tolerance.</returns>
        private static bool IsCoincident(double testX, double testY, double xs, double ys, double xe, double ye, double tsq)
        {
            return (BasicGeom.DistanceSquared(testX, testY, xs, ys, xe, ye) < tsq);
        }

        /// <summary>
        /// Checks an array of positions that purportedly correspond to a circular arc, to see
        /// whether the data is directed clockwise or not. No checks are made to confirm that
        /// the data really does correspond to a circular arc.
        /// </summary>
        /// <param name="pts">The positions defining an arc</param>
        /// <param name="center">The position of the centre of the circle that the arc lies on.</param>
        /// <returns>True if the data is ordered clockwise.</returns>
        public static bool IsClockwise(IPointGeometry[] pts, IPointGeometry center)
        {
            // To determine the direction, we must locate two successive
            // vertices that lie in the same quadrant (with respect to the
            // circle center).

            QuadVertex start = new QuadVertex(center, pts[0]);
            QuadVertex end = null;

            for (int i=1; i<pts.Length; i++, start=end)
            {
                // Pick up the position at the end of the line segment.
                end = new QuadVertex(center, pts[i]);

                // If both ends of the segment are in the same quadrant,
                // see which one comes first. The result tells us whether
                // the curve is clockwise or not.

                if (start.Quadrant == end.Quadrant)
                {
                    return (start.GetTanAngle() < end.GetTanAngle());
                }
            }

            // Something has gone wrong if we got here!
            Debug.Assert(1==2);
	        return true;
        }
    }
}
