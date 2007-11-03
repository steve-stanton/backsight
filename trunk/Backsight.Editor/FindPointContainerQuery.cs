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
    /// <written by="Steve Stanton" on="28-AUG-2007" />
    /// <summary>
    /// Query spatial index to obtain the polygon (if any) that encloses a point.
    /// This class assumes that polygon topology is completely up to date.
    /// </summary>
    /// <seealso cref="FindIslandContainerQuery"/>
    class FindPointContainerQuery
    {
        #region Class data

        /// <summary>
        /// The position you want the container for.
        /// </summary>
        readonly IPointGeometry m_Point;

        /// <summary>
        /// The enclosing polygon (null if nothing has been found).
        /// </summary>
        Polygon m_Result;

        /// <summary>
        /// Candidates that we'll come back to.
        /// </summary>
        List<Polygon> m_Candidates;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>FindPointContainerQuery</c> (and executes it). The result of the query
        /// can then be obtained through the <c>Result</c> property.
        /// </summary>
        /// <param name="index">The spatial index to search</param>
        /// <param name="point">The position you want the container for</param>
        internal FindPointContainerQuery(ISpatialIndex index, IPointGeometry p)
        {
            m_Point = p;
            m_Result = null;
            IWindow w = new Window(p, p);
            index.QueryWindow(w, SpatialType.Polygon, OnQueryHit);

            // If we didn't get a result, but we skipped some candidates, check them now.
            if (m_Result==null && m_Candidates!=null)
            {
                // If NONE of the polygon's islands enclose the search position, that's
                // the result we want.

                foreach (Polygon cand in m_Candidates)
                {
                    Debug.Assert(cand.HasAnyIslands);

                    if (!cand.HasIslandEnclosing(m_Point))
                    {
                        m_Result = cand;
                        return;
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Delegate that's called whenever the index finds an object with an extent that
        /// overlaps the query window.
        /// </summary>
        /// <param name="item">The item to process (expected to be some sort of <c>Ring</c>)</param>
        /// <returns>True if the query should continue. False if the enclosing polygon has been found.</returns>
        private bool OnQueryHit(ISpatialObject item)
        {
            // We're only interested in real polygons (not islands)
            if (!(item is Polygon))
                return true;

            // The window of the polygon has to overlap.
            Polygon p = (Polygon)item;
            if (!p.Extent.IsOverlap(m_Point))
                return true;

            // Skip if it doesn't enclose the search position
            if (!p.IsRingEnclosing(m_Point))
                return true;

            // If the polygon contains any islands, remember the polygon
            // for a further look. Things like an unclosed street network
            // can have MANY islands, so checking whether the position falls
            // inside any of them is a bit laborious. We'll comes back to
            // this polygon if we can't find an easy match.

            if (p.HasAnyIslands)
            {
                if (m_Candidates==null)
                    m_Candidates = new List<Polygon>(1);

                m_Candidates.Add(p);
                return true;
            }

            m_Result = p;
            return false;
        }

        /// <summary>
        /// The result of the query (null if no features were found within the query region).
        /// </summary>
        internal Polygon Result
        {
            get { return m_Result; }
        }
    }
}
