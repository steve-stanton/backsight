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
    /// Query spatial index to locate line intersections. This class is used
    /// to detect topological intersections, so it ignores plain (non-topological) lines.
    /// <para/>
    /// Lines that are marked as "moved" (a flag used to force line re-intersect calculations)
    /// are also ignored. Consider an edit that creates two lines that intersect each other.
    /// When the edit is being concluded, the lines get intersected against the map, and that
    /// software assumes that each new line starts out without any splits. To ensure this
    /// assumption holds, the software first intersects the 1st line against the map, totally
    /// ignoring the 2nd line that was also created. After making any splits on line 1, it's
    /// IsMoved flag gets cleared. On processing the 2nd line, we'll detect the intersection
    /// with line 1.
    /// </summary>
    class FindIntersectionsQuery
    {
        #region Class data

        /// <summary>
        /// The line (if any) that is being intersected. Null if raw geometry is
        /// being intersected.
        /// </summary>
        private readonly LineFeature m_Feature;

        /// <summary>
        /// The geometry to intersect
        /// </summary>
        private readonly UnsectionedLineGeometry m_Geom;

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
        /// Creates a new <c>FindIntersectionsQuery</c> (and executes it). The result of the query
        /// can then be obtained through the <c>Result</c> property.
        /// <para/>
        /// Use this constructor when intersecting something that has already been added to
        /// the spatial index. This ensures that the line is not intersected with itself.
        /// </summary>
        /// <param name="index">The spatial index to search</param>
        /// <param name="line">The line feature to intersect.</param>
        /// <param name="wantEndEnd">Specify true if you want end-to-end intersections in the results.</param>
        internal FindIntersectionsQuery(ISpatialIndex index, LineFeature line, bool wantEndEnd)
        {
            m_Feature = line;
            m_Geom = GetUnsectionedLineGeometry(line.LineGeometry);
            m_WantEndEnd = wantEndEnd;
            m_Result = new List<IntersectionResult>(100);

            FindIntersections(index);
        }

        /// <summary>
        /// Creates a new <c>FindIntersectionsQuery</c> (and executes it). The result of the query
        /// can then be obtained through the <c>Result</c> property.
        /// <para/>
        /// Use this constructor when intersecting with geometry that has been created ad-hoc.
        /// Note that if you are looking to intersect a line feature (already part of
        /// the spatial index), you should use the constructor that accepts the <c>LineFeature</c>.
        /// </summary>
        /// <param name="index">The spatial index to search</param>
        /// <param name="geom">The geometry to intersect.</param>
        /// <param name="wantEndEnd">Specify true if you want end-to-end intersections in the results.</param>
        internal FindIntersectionsQuery(ISpatialIndex index, LineGeometry geom, bool wantEndEnd)
        {
            m_Feature = null;
            m_Geom = GetUnsectionedLineGeometry(geom);
            m_WantEndEnd = wantEndEnd;
            m_Result = new List<IntersectionResult>(100);

            FindIntersections(index);
        }

        #endregion

        /// <summary>
        /// Obtains line geometry that isn't an instance of <see cref="SectionGeometry"/>
        /// </summary>
        /// <param name="geom">The geometry that could be a section</param>
        /// <returns>Concrete (unsectioned) geometry that corresponds to <paramref name="geom"/></returns>
        private UnsectionedLineGeometry GetUnsectionedLineGeometry(LineGeometry geom)
        {
            if (geom is UnsectionedLineGeometry)
                return (geom as UnsectionedLineGeometry);

            if (geom is SectionGeometry)
                return (geom as SectionGeometry).Make();

            throw new NotImplementedException("Unexpected line geometry: "+geom.GetType().Name);
        }

        /// <summary>
        /// Detects intersections
        /// </summary>
        /// <param name="index">The spatial index to search</param>
        private void FindIntersections(ISpatialIndex index)
        {
            // Get the window of the candidate object and add on a 2mm buffer (on the ground). This is
            // intended to help cover the fact that some circular arcs may be inaccurate by that much.
            Window searchwin = new Window(m_Geom.Extent);
            ILength dim = new Length(0.002);
            searchwin.Expand(dim);

            index.QueryWindow(searchwin, SpatialType.Line, OnQueryHit);
            m_Result.TrimExcess();
        }

        /// <summary>
        /// Delegate that's called whenever the index finds an object with an extent that
        /// overlaps the query window.
        /// </summary>
        /// <param name="item">The item to process (expected to be some sort of <c>LineFeature</c>)</param>
        /// <returns>True (always), indicating that the query should continue.</returns>
        private bool OnQueryHit(ISpatialObject item)
        {
            Debug.Assert(item is LineFeature);

            // Ignore if we're intersecting a line feature & the line we've found is that line
            LineFeature f = (LineFeature)item;
            if (Object.ReferenceEquals(m_Feature, f))
                return true;

            // Ignore lines that don't have any topology
            Topology t = f.Topology;
            if (t == null)
                return true;

            // Ignore lines that are marked as "moved"
            if (f.IsMoved)
                return true;

            // Intersect each divider
            foreach (IDivider d in t)
            {
                // Ignore divider overlaps (regarded as non-topological)
                if (d.IsOverlap)
                    continue;

                // Search for intersections
                IntersectionResult other = new IntersectionResult(d);
                m_Geom.Intersect(other);

                if (other.IntersectCount>0)
                {
                    // Determine the context of each intersection.
                    other.SetContext(m_Geom);

                    // If end-to-end simple intersections are not required, weed them out.
                    if (!m_WantEndEnd)
                        other.CutEndEnd();

                    if (other.IntersectCount>0)
                        m_Result.Add(other);
                }
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
