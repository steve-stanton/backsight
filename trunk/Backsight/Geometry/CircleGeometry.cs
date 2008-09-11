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

namespace Backsight.Geometry
{
	/// <written by="Steve Stanton" on="14-JUN-2007" />
    /// <summary>
    /// The geometry for a circle.
    /// </summary>
    public class CircleGeometry : ILineGeometry, ICircleGeometry
    {
        #region Class data

        /// <summary>
        /// The radius of the circle, in meters
        /// </summary>
        private readonly double m_Radius;

        /// <summary>
        /// The center of the circle.
        /// </summary>
        private readonly IPointGeometry m_Center;

        #endregion

        #region Constructors

        public CircleGeometry(IPointGeometry center, double radius)
        {
            m_Center = center;
            m_Radius = radius;
        }

        public CircleGeometry(ICircleGeometry g)
        {
            m_Center = g.Center;
            m_Radius = g.Radius;
        }

        #endregion

        public IPointGeometry Center
        {
            get { return m_Center; }
        }

        public double Radius
        {
            get { return m_Radius; }
        }

        public void Render(ISpatialDisplay display, IDrawStyle style)
        {
            CircleGeometry.Render(this, display, style);
        }

        public static void Render(ICircleGeometry g, ISpatialDisplay display, IDrawStyle style)
        {
            style.Render(display, g.Center, g.Radius);
        }

        public IWindow Extent
        {
            get { return CircleGeometry.GetExtent(this); }
        }

        public static IWindow GetExtent(ICircleGeometry g)
        {
            double xc = g.Center.X;
            double yc = g.Center.Y;
            double r = g.Radius;
            return new Window(xc-r, yc-r, xc+r, yc+r);
        }

        /// <summary>
        /// The distance from the specified position to the perimeter of this circle.
        /// </summary>
        /// <param name="p">The position of interest</param>
        /// <returns>The distance from the specified position to the perimeter of
        /// this circle.</returns>
        public ILength Distance(IPosition p)
        {
            return CircleGeometry.Distance(this, p);
        }

        public static ILength Distance(ICircleGeometry g, IPosition p)
        {
            double d = BasicGeom.Distance(g.Center, p);
            return new Length(Math.Abs(d - g.Radius));
        }

        /// <summary>
        /// Do a pair of circles coincide?
        /// </summary>
        /// <param name="a">The first circle</param>
        /// <param name="b">The second circle</param>
        /// <param name="radiusTol">The tolerance for a radius match, in meters on the ground</param>
        /// <returns>True if the two circles have the same center point, and the same radius (within
        /// the specified tolerance)</returns>
        public static bool IsCoincident(ICircleGeometry a, ICircleGeometry b, double radiusTol)
        {
            return (a.Center.IsCoincident(b.Center) &&
                    Math.Abs(a.Radius - b.Radius)<radiusTol);
        }

        /// <summary>
        /// The "start" of the circle is the position at the top (implements ILineGeometry)
        /// </summary>
        public IPointGeometry Start
        {
            get { return Top; }
        }

        /// <summary>
        /// The "end" of the circle is the position at the top (implements ILineGeometry)
        /// </summary>
        public IPointGeometry End
        {
            get { return Top; }
        }

        /// <summary>
        /// The most northerly position of this circle.
        /// </summary>
        IPointGeometry Top
        {
            get
            {
                return new PositionGeometry(m_Center.Easting.Meters,
                                            m_Center.Northing.Meters + m_Radius);
            }
        }

        /// <summary>
        /// The perimeter length of this circle. (implements ILineGeometry)
        /// </summary>
        public ILength Length
        {
            get { return GetLength(this); }
        }

        /// <summary>
        /// Calculates the perimeter length of a circle.
        /// </summary>
        /// <param name="g">The circle of interest</param>
        /// <returns>The perimeter length of the circle.</returns>
        public static ILength GetLength(ICircleGeometry g)
        {
            return new Length(g.Radius * MathConstants.PIMUL2);
        }

        /// <summary>
        /// Calculates the position on a circle that is closest to the specified position
        /// (i.e. takes the supplied position and drop a perpendicular to the circle).
        /// </summary>
        /// <param name="g">The circle of interest</param>
        /// <param name="p">The position to process</param>
        /// <returns>The position on the circle closest to the supplied position. Null
        /// if the supplied position coincides with the center of the circle.</returns>
        public static IPosition GetClosestPosition(ICircleGeometry g, IPosition p)
        {
            // Get the deltas of the supplied position with respect to the circle center
            IPosition c = g.Center;
            double dx = p.X - c.X;
            double dy = p.Y - c.Y;
            double dist = Math.Sqrt(dx*dx + dy*dy);

            // No result if it's too close to the center.
            if (dist<MathConstants.TINY)
                return null;

            // Get the factor for projecting the position.
            double factor = g.Radius/dist;

            // Figure out the position on the circle.
            return new Position(c.X + dx*factor, c.Y + dy*factor);
        }
    }
}
