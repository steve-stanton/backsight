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
    /// <written by="Steve Stanton" on="27-SEP-2007" />
    /// <summary>
    /// Query spatial index to locate line intersections.
    /// </summary>
    class FindIntersectionsQuery
    {
        #region Class data

        /// <summary>
        /// The line of interest.
        /// </summary>
        private readonly LineGeometry m_Line;

        /// <summary>
        /// Should lines that intersect end-to-end be included in the results?
        /// </summary>
        private readonly bool m_WantEndEnd;

        /// <summary>
        /// The intersections found so far.
        /// </summary>
        private readonly List<IntersectionResult> m_Result;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>FindPointsOnLineQuery</c> (and executes it). The result of the query
        /// can then be obtained through the <c>Result</c> property.
        /// </summary>
        /// <param name="index">The spatial index to search</param>
        /// <param name="line">The line to intersect.</param>
        /// <param name="wantEndEnd">Specify true if you want end-to-end intersections in the results.</param>
        internal FindIntersectionsQuery(ISpatialIndex index, LineGeometry line, bool wantEndEnd)
        {
            m_Line = line;
            m_WantEndEnd = wantEndEnd;
            m_Result = new List<IntersectionResult>(100);

            // Get the window of the candidate object and add on a 2mm buffer (on the ground). This is
            // intended to help cover the fact that some circular arcs may be inaccurate by that much.
            Window searchwin = new Window(m_Line.Extent);
            ILength dim = new Length(0.002);
            searchwin.Expand(dim);

            index.QueryWindow(searchwin, SpatialType.Line, OnQueryHit);
            m_Result.TrimExcess();
        }

        #endregion

        /// <summary>
        /// Delegate that's called whenever the index finds an object with an extent that
        /// overlaps the query window.
        /// </summary>
        /// <param name="item">The item to process (expected to be some sort of <c>LineFeature</c>)</param>
        /// <returns>True (always), indicating that the query should continue.</returns>
        private bool OnQueryHit(ISpatialObject item)
        {
            Debug.Assert(item is LineFeature);

            LineFeature f = (LineFeature)item;
            LineGeometry g = f.LineGeometry;

            // Don't bother considering the line we're intersecting
            if (object.ReferenceEquals(m_Line, g))
                return true;

            // Search for intersections
            IntersectionResult other = new IntersectionResult(f);
            
            // Unfortunately, doing the following would mean that I have to promote
            // IntersectionResult (+ associated parephanelia) to the main Backsight
            // project. Even trying to conceal things with a few interfaces isn't
            // very satisfactory. To get around it, look 

            // Incidentally, the reason m_Line is an instance of ILineGeometry (as
            // opposed to LineGeometry) is because I want to use this class for doing

            m_Line.Intersect(other);

            if (other.IntersectCount>0)
            {
                // Determine the context of each intersection.
                other.SetContext(m_Line);

                // If end-to-end simple intersections are not required, weed them out.
                if (!m_WantEndEnd)
                    other.CutEndEnd();

                if (other.IntersectCount>0)
                    m_Result.Add(other);
            }

            return true;
        }

        /// <summary>
        /// The result of the query (may be an empty list).
        /// </summary>
        internal List<IntersectionResult> Result
        {
            get { return m_Result; }
        }
    }
}
