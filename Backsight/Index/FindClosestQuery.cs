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

using Backsight.Geometry;

namespace Backsight.Index
{
	/// <written by="Steve Stanton" on="15-DEC-2006" />
    /// <summary>
    /// Query spatial index to obtain the feature closest to a specific position. This
    /// type of search only locates spatial "features" (which excludes polygons).
    /// </summary>
    class FindClosestQuery
    {
        #region Class data

        /// <summary>
        /// The search position.
        /// </summary>
        private readonly IPointGeometry m_Position;

        /// <summary>
        /// The search tolerance (expected to be greater than zero).
        /// </summary>
        private readonly ILength m_Radius;

        /// <summary>
        /// The type of objects to look for.
        /// </summary>
        private readonly SpatialType m_Types;

        /// <summary>
        /// The distance to the closest feature found so far (never greater than m_Radius.Meters)
        /// </summary>
        private double m_Distance;

        /// <summary>
        /// The closest feature (null if nothing has been found).
        /// </summary>
        private ISpatialObject m_Result;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>FindClosestQuery</c> (and executes it). The result of the query
        /// can then be obtained through the <c>Result</c> property.
        /// </summary>
        /// <param name="index">The spatial index to search</param>
        /// <param name="p">The search position.</param>
        /// <param name="radius">The search tolerance (expected to be greater than zero).</param>
        /// <param name="types">The type of objects to look for.</param>
        internal FindClosestQuery(ISpatialIndex index, IPosition p, ILength radius, SpatialType types)
        {
            if (types==0)
                throw new ArgumentNullException("Spatial type(s) not specified");

            // If the user hasn't been specific, ensure we don't search for polygons!
            SpatialType useTypes = (types==SpatialType.All ? SpatialType.Feature : types);
            Debug.Assert((useTypes & SpatialType.Polygon)==0);

            // It's important to round off to the nearest micron. Otherwise you might not
            // get the desired results in situations where the search radius is zero.
            m_Position = PositionGeometry.Create(p);

            m_Radius = radius;
            m_Types = types;
            m_Result = null;
            m_Distance = m_Radius.Meters;

            // The query will actually involve a square window, not a circle.
            IWindow x = new Window(m_Position, radius.Meters * 2.0);
            index.QueryWindow(x, useTypes, OnQueryHit);
        }

        #endregion

        /// <summary>
        /// Delegate that's called whenever the index finds an object with an extent that
        /// overlaps the query window.
        /// </summary>
        /// <param name="item">The item to process (expected to be some sort of <c>IFeature</c>)</param>
        /// <returns>True if the query should continue. False if the item is exactly coincident with
        /// the query position.</returns>
        private bool OnQueryHit(ISpatialObject item)
        {
            double d = item.Distance(m_Position).Meters;
            if (d<=m_Distance)
            {
                m_Distance = d;
                m_Result = item;
                return (m_Distance > Double.Epsilon);
            }

            return true;
        }

        /// <summary>
        /// The result of the query (null if no features were found within the query region).
        /// </summary>
        internal ISpatialObject Result
        {
            get { return m_Result; }
        }
    }
}
