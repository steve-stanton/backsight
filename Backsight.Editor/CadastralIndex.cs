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
using System.Collections.Generic;

using Backsight.Index;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="12-AUG-2007" />
    /// <summary>
    /// Spatial index utilized by the Cadastral Editor. In addition to the usual
    /// stuff, this provides an additional index for circles.
    /// </summary>
    class CadastralIndex : SpatialIndex
    {
        #region Class data

        /// <summary>
        /// Spatial index of all defined circles (instances of <see "Circle"/>). The
        /// only spatial type that this index will contain is <see "SpatialType.Line"/>
        /// </summary>
        readonly SpatialIndex m_Circles;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>CadastralIndex</c> with nothing in it.
        /// </summary>
        internal CadastralIndex()
        {
            m_Circles = new SpatialIndex();
        }

        #endregion

        /// <summary>
        /// Includes a circle in this index.
        /// </summary>
        /// <param name="c">The circle to add to the index</param>
        internal void AddCircle(Circle c)
        {
            Debug.Assert(c.SpatialType==SpatialType.Line);
            m_Circles.Add(c);
        }

        /// <summary>
        /// Attempts to locate the circle closest to a position of interest.
        /// </summary>
        /// <param name="p">The search position (on the circumference of the circle)</param>
        /// <param name="tol">The search tolerance</param>
        /// <returns>The circle closest to the search position (null if nothing found)</returns>
        internal Circle QueryClosestCircle(IPosition p, ILength tol)
        {
            ISpatialObject so = m_Circles.QueryClosest(p, tol, SpatialType.Line);
            if (so==null)
                return null;

            Debug.Assert(so is Circle);
            return (so as Circle);
        }

        /// <summary>
        /// Locates circles close to a specific position.
        /// </summary>
        /// <param name="p">The search position.</param>
        /// <param name="tol">The search tolerance (expected to be greater than zero).</param>
        /// <returns>The result of the query (may be an empty list).</returns>
        internal List<Circle> QueryCircles(IPosition p, ILength tol)
        {
            return new FindCirclesQuery(this, p, tol).Result;
        }

        /// <summary>
        /// Processes circles that overlap the specified window.
        /// Used by <c>FindCirclesQuery</c> to execute the query.
        /// </summary>
        /// <param name="extent">The query extent</param>
        /// <param name="itemHandler">Delegate for processing each query hit</param>
        internal void FindCircles(IWindow extent, ProcessItem itemHandler)
        {
            m_Circles.QueryWindow(extent, SpatialType.Line, itemHandler);
        }
    }
}
