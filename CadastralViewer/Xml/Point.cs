// <remarks>
// Copyright 2010 - Steve Stanton. This file is part of Backsight
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
using Backsight;
using Backsight.Geometry;

namespace CadastralViewer.Xml
{
    /// <written by="Steve Stanton" on="09-MAR-2010" />
    /// <summary>
    /// A point feature
    /// </summary>
    public partial class Point : IPoint, IPosition
    {
        #region IPoint Members

        /// <summary>
        /// The geometry for this point.
        /// </summary>
        public IPointGeometry Geometry
        {
            get { return new PositionGeometry(this.x, this.y); }
        }

        #endregion

        #region ISpatialObject Members

        /// <summary>
        /// Value denoting the spatial object type.
        /// </summary>
        /// <value>SpatialType.Point</value>
        public SpatialType SpatialType
        {
            get { return SpatialType.Point; }
        }

        /// <summary>
        /// Draws this object on the specified display
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The drawing style</param>
        public void Render(ISpatialDisplay display, IDrawStyle style)
        {
            style.Render(display, this);
        }

        /// <summary>
        /// The spatial extent of this object.
        /// </summary>
        public IWindow Extent
        {
            get { return new Window(this); }
        }

        /// <summary>
        /// The shortest distance between this object and the specified position.
        /// </summary>
        /// <param name="point">The position of interest</param>
        /// <returns>
        /// The shortest distance between the specified position and this object
        /// </returns>
        public ILength Distance(IPosition point)
        {
            return new Length(BasicGeom.Distance(this, point));
        }

        #endregion

        #region IPosition Members

        /// <summary>
        /// The easting value for this position.
        /// </summary>
        public double X
        {
            get { return this.x; }
        }

        /// <summary>
        /// The northing value for this position.
        /// </summary>
        public double Y
        {
            get { return this.y; }
        }

        /// <summary>
        /// Does this position coincide with another one?
        /// </summary>
        /// <param name="p">The position to compare with</param>
        /// <param name="tol">Tolerance to use for comparison</param>
        /// <returns>
        /// True if this position is at the same position (within
        /// the specified tolerance)
        /// </returns>
        public bool IsAt(IPosition p, double tol)
        {
            return Position.IsCoincident(this, p, tol);
        }

        #endregion
    }
}
