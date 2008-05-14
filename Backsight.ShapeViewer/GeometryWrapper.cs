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

using NTS=GisSharpBlog.NetTopologySuite.Geometries;

namespace Backsight.ShapeViewer
{
    /// <written by="Steve Stanton" on="04-OCT-2006" />
    /// <summary>
    /// Wraps a NetTopologySuite <c>Geometry</c> object so that it can be treated
    /// as a Backsight <c>ISpatialObject</c>. Not expected to work with instances
    /// of <c>GeometryCollection</c>.
    /// </summary>
    /// <remarks>This may not be the most efficient class, since it converts
    /// NTS <c>Coordinate</c> objects into Backsight <c>Position</c> objects
    /// whenever it's asked to do something.</remarks>
    class GeometryWrapper : ISpatialObject
    {
        #region Class data

        /// <summary>
        /// The wrapped geometry
        /// </summary>
        private readonly NTS.Geometry m_Geometry;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>GeometryWrapper</c> that wraps the supplied geometry.
        /// </summary>
        /// <param name="g">The geometry to wrap</param>
        /// <exception cref="ArgumentException">If the geometry can't be mapped to one of the
        /// types defined by the <c>SpatialType</c> enum.</exception>
        internal GeometryWrapper(NTS.Geometry g)
        {
            m_Geometry = g;

            // Throw exception on unexpected type
            SpatialType type = this.SpatialType;
        }

        #endregion

        /// <summary>
        /// Any data attached to the wrapped NTS geometry. If the geometry was loaded using
        /// the <c>ShapeFile</c> class that's part of the ShapeViewer project, this should be
        /// the attribute data associated with the geometry.
        /// </summary>
        internal object UserData
        {
            get { return m_Geometry.UserData; }
        }

        /// <summary>
        /// Converts coordinates of the wrapped geometry into an array of Backsight position objects. 
        /// </summary>
        /// <remarks>The NTS <c>Coordinate</c> class already implements a portion of
        /// the <c>IPosition</c> interface, and could easily implement the remainder.
        /// However, I am not going to tweak any NTS classes for the sake of this
        /// sample application.
        /// </remarks>
        IPosition[] PositionArray
        {
            get
            {
                NTS.Coordinate[] cos = m_Geometry.Coordinates;
                IPosition[] pts = new Position[cos.Length];
                for (int i=0; i<cos.Length; i++)
                {
                    NTS.Coordinate c = cos[i];
                    pts[i] = new Position(c.X, c.Y);
                }
                return pts;
            }
        }

        /// <summary>
        /// The spatial type of the wrapped geometry.
        /// </summary>
        /// <exception cref="ArgumentException">If the geometry can't be mapped to one of the
        /// types defined by the <c>SpatialType</c> enum.</exception>
        public SpatialType SpatialType
        {
            get
            {
                if (m_Geometry is NTS.LineString || m_Geometry is NTS.MultiLineString || m_Geometry is NTS.Polygon)
                    return SpatialType.Line;
                else if (m_Geometry is NTS.Point)
                    return SpatialType.Point;

                throw new ArgumentException("Unexpected geometry type: "+m_Geometry.GetType().Name);
            }
        }

        /// <summary>
        /// Draws this object on the specified display
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The drawing style</param>
        public void Render(ISpatialDisplay display, IDrawStyle style)
        {
            style.Render(display, PositionArray);
        }

        /// <summary>
        /// The spatial extent of this object.
        /// </summary>
        public IWindow Extent
        {
            get { return new Window(PositionArray); }
        }

        /// <summary>
        /// The shortest distance between this object and the specified position.
        /// </summary>
        /// <param name="point">The position of interest</param>
        /// <returns>The shortest distance between the specified position & this object</returns>
        public ILength Distance(IPosition point)
        {
            double dsq = BasicGeom.MinDistanceSquared(PositionArray, point);
            return new Length(Math.Sqrt(dsq));
        }
    }
}
