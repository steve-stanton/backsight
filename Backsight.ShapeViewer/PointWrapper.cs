// <remarks>
// Copyright 2009 - Steve Stanton. This file is part of Backsight
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

using NTS=GisSharpBlog.NetTopologySuite.Geometries;
using Backsight.Geometry;

namespace Backsight.ShapeViewer
{
    /// <summary>
    /// Wrapper on an <c>NTS.Point</c>
    /// </summary>
    class PointWrapper : GeometryWrapper, IPoint
    {
        #region Static

        static ILength HEIGHT = new Length(100.0);

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>PointWrapper</c> that wraps the supplied geometry.
        /// </summary>
        /// <param name="g">The geometry to wrap</param>
        internal PointWrapper(NTS.Point p)
            : base(p)
        {
        }

        #endregion


        #region IPoint Members

        /// <summary>
        /// The geometry for this point.
        /// </summary>
        public IPointGeometry Geometry
        {
            get
            {
                NTS.Point p = (NTS.Point)NtsGeometry;
                return new PositionGeometry(p.X, p.Y);
            }
        }

        #endregion

        /// <summary>
        /// Draws this object on the specified display
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The drawing style</param>
        public override void Render(ISpatialDisplay display, IDrawStyle style)
        {
            style.PointHeight = HEIGHT;
            style.RenderPlus(display, Geometry);
        }
    }
}
