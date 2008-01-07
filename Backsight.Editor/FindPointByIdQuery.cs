/// <remarks>
/// Copyright 2008 - Steve Stanton. This file is part of Backsight
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

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="08-JAN-2008" />
    /// <summary>
    /// Query spatial index to obtain a point with a specific ID.
    /// </summary>
    class FindPointByIdQuery
    {
        #region Class data

        /// <summary>
        /// The ID of interest.
        /// </summary>
        private readonly string m_Key;

        /// <summary>
        /// The point found (null if nothing found).
        /// </summary>
        private PointFeature m_Result;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>FindPointByIdQuery</c> (and executes it). The result of the query
        /// can then be obtained through the <c>Result</c> property.
        /// </summary>
        /// <param name="index">The spatial index to search</param>
        /// <param name="key">The ID of interest.</param>
        internal FindPointByIdQuery(ISpatialIndex index, string key)
        {
            m_Key = key;
            m_Result = null;
            index.QueryWindow(null, SpatialType.Point, OnQueryHit);
        }

        #endregion

        /// <summary>
        /// Delegate that's called whenever the index finds an object with an extent that
        /// overlaps the query window.
        /// </summary>
        /// <param name="item">The item to process (expected to be some sort of <c>PointFeature</c>)</param>
        /// <returns>True if the query should continue. False if a matching point has been found.</returns>
        private bool OnQueryHit(ISpatialObject item)
        {
            Debug.Assert(item is PointFeature);

            PointFeature p = (PointFeature)item;
            if (p.FormattedKey == m_Key)
            {
                m_Result = p;
                return false;
            }

            return true;
        }

        /// <summary>
        /// The result of the query (null if a matching point could not be found).
        /// </summary>
        internal PointFeature Result
        {
            get { return m_Result; }
        }
    }
}
