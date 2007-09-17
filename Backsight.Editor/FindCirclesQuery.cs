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
using System.Collections.Generic;
using System.Diagnostics;

namespace Backsight.Editor
{
	/// <written by="Steve Stanton" on="04-JUN-2007" />
    /// <summary>
    /// Query spatial index to obtain circles close to a specific position.
    /// </summary>
    class FindCirclesQuery
    {
        #region Class data

        /// <summary>
        /// The search position.
        /// </summary>
        private readonly IPosition m_Position;

        /// <summary>
        /// The search tolerance, in meters on the ground (expected to be greater than zero).
        /// </summary>
        private readonly double m_Tolerance;

        /// <summary>
        /// The circles found so far.
        /// </summary>
        private readonly List<Circle> m_Result;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>FindCirclesQuery</c> (and executes it). The result of the query
        /// can then be obtained through the <c>Result</c> property.
        /// </summary>
        /// <param name="index">The spatial index to search</param>
        /// <param name="p">The search position.</param>
        /// <param name="tol">The search tolerance (expected to be greater than zero).</param>
        internal FindCirclesQuery(CadastralIndex index, IPosition p, ILength tol)
        {
            m_Position = p;
            m_Tolerance = tol.Meters;
            m_Result = new List<Circle>();

            // The query will actually involve a square window, not a circle.
            IWindow x = new Window(p, m_Tolerance * 2.0);
            index.FindCircles(x, OnQueryHit);
        }

        #endregion

        /// <summary>
        /// Delegate that's called whenever the index finds a line with an extent that
        /// overlaps the query window.
        /// </summary>
        /// <param name="item">The item to process (expected to be some sort of <c>IFeature</c>)</param>
        /// <returns>True (always), meaning the query should continue.</returns>
        private bool OnQueryHit(ISpatialObject item)
        {
            if (item is Circle)
            {
                // Confirm the circle is truly within tolerance
                Circle c = (item as Circle);
                double rad = c.Radius.Meters;
                double dist = Geom.Distance(c.Center, m_Position);
                if (Math.Abs(rad-dist) < m_Tolerance)
                    m_Result.Add(c);
            }

            return true;
        }

        /// <summary>
        /// The result of the query (may be an empty list).
        /// </summary>
        internal List<Circle> Result
        {
            get { return m_Result; }
        }
    }
}
