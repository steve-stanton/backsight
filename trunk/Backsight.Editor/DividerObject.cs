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

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="22-NOV-2007" />
    /// <summary>
    /// Wrapper on an instance of <see cref="IDivider"/> that implements <see cref="ISpatialObject"/>.
    /// This makes it possible to include dividers in things like a <see cref="SpatialSelection"/>.
    /// </summary>
    class DividerObject : ISpatialObject
    {
        #region Class data

        /// <summary>
        /// The divider that this instance wraps.
        /// </summary>
        readonly IDivider m_Divider;

        /// <summary>
        /// The geometry for the divider. While this is also available via <c>m_Divider</c>, this can
        /// hide the fact that getting the geometry may involve quite a number of steps (e.g. getting
        /// the geometry for a divider that is a section on a multi-segment). Since the intention is
        /// that the <c>DividerObject</c> class will only be used for special handling of dividers,
        /// it seems reasonable to cache the geometry here.
        /// </summary>
        readonly LineGeometry m_Geom;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>DividerObject</c> that wraps the supplied divider.
        /// </summary>
        /// <param name="d">The divider to wrap</param>
        internal DividerObject(IDivider d)
        {
            if (d==null)
                throw new ArgumentNullException();

            m_Divider = d;
            m_Geom = m_Divider.LineGeometry;
        }

        #endregion

        #region ISpatialObject

        /// <summary>
        /// Value denoting the spatial object type.
        /// </summary>
        public SpatialType SpatialType
        {
            get { return SpatialType.Line; }
        }

        /// <summary>
        /// Draws this object on the specified display
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The drawing style</param>
        public void Render(ISpatialDisplay display, IDrawStyle style)
        {
            m_Geom.Render(display, style);
        }

        /// <summary>
        /// The spatial extent of this object.
        /// </summary>
        public IWindow Extent
        {
            get { return m_Geom.Extent; }
        }

        /// <summary>
        /// The shortest distance between this object and the specified position.
        /// </summary>
        /// <param name="point">The position of interest</param>
        /// <returns>The shortest distance between the specified position & this object</returns>
        public ILength Distance(IPosition point)
        {
            return m_Geom.Distance(point);
        }

        #endregion
    }
}
