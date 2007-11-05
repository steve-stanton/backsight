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
	/// <written by="Steve Stanton" on="13-FEB-1998" was="CeXObject" />
    /// <summary>
    /// Detects intersections of a line with the map. The things that it
    /// intersects are held in a series of <see cref="IntersectionResult"/> objects.
    /// </summary>
    class IntersectionFinder
    {
        #region Class data

        /// <summary>
        /// The thing being intersected
        /// </summary>
	    IIntersectable m_Line;

        /// <summary>
        /// The things that are intersected
        /// </summary>
        List<IntersectionResult> m_Intersects;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        IntersectionFinder()
        {
            m_Line = null;
            m_Intersects = new List<IntersectionResult>();
        }

        /// <summary>
        /// Creates a new <c>IntersectionFinder</c> for the specified line feature.
        /// Use this constructor when intersecting something that has already been added to
        /// the map model. This ensures that the line is not intersected with itself.
        /// </summary>
        /// <param name="line">The line feature to intersect.</param>
        /// <param name="wantEndEnd">Specify true if you want end-to-end intersections in the results.</param>
        internal IntersectionFinder(LineFeature line, bool wantEndEnd)
        {
            m_Line = line;
            ISpatialIndex index = CadastralMapModel.Current.Index;
            m_Intersects = new FindIntersectionsQuery(index, line, wantEndEnd).Result;
        }

        /// <summary>
        /// Creates a new <c>IntersectionFinder</c> for the specified geometry.
        /// Use this constructor when intersecting geometry that has been created ad-hoc.
        /// </summary>
        /// <param name="geom">The geometry to intersect.</param>
        /// <param name="wantEndEnd">Specify true if you want end-to-end intersections in the results.</param>
        internal IntersectionFinder(LineGeometry geom, bool wantEndEnd)
        {
            m_Line = geom;
            ISpatialIndex index = CadastralMapModel.Current.Index;
            m_Intersects = new FindIntersectionsQuery(index, geom, wantEndEnd).Result;
        }

        #endregion

        internal uint Count
        {
            get { return (uint)m_Intersects.Count; }
        }

        /// <summary>
        /// The list of things that <c>Geometry</c> intersects
        /// </summary>
        internal IList<IntersectionResult> Intersections
        {
            get { return m_Intersects; }
        }

        /// <summary>
        /// The thing being intersected
        /// </summary>
        internal IIntersectable Intersector
        {
            get { return m_Line; }
        }

        /// <summary>
        /// Appends intersection info to this object.
        /// </summary>
        /// <param name="xsect">The intersection info to append.</param>
        void Append(IntersectionResult xsect)
        {
            m_Intersects.Add(xsect);
        }

        /// <summary>
        /// Checks whether intersection results graze anything.
        /// </summary>
        internal bool IsGrazing
        {
            get
            {
                foreach(IntersectionResult r in m_Intersects)
                {
                    if (r.IsGrazing)
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Checks whether any intersection refers to a position that requires a split
        /// on the primitive that THIS object refers to. This does not count the primitives
        /// that we actually intersected with.
        ///
        /// The result is true if a split is required. If this intersection object was for
        /// a point or a circle, the result will always be FALSE.
        /// </summary>
        internal bool IsSplitNeeded
        {
            get
            {
                // Go through each object we intersected with, looking for an intersection
                // that does not occur at the ends of the line primitive.

                foreach(IntersectionResult r in m_Intersects)
                {
                    if (r.IsSplitOn(m_Line.LineGeometry))
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Cuts up the topological lines that are referred to by this intersection object.
        /// </summary>
        /// <param name="splitter">The line that is causing the split (the same as a call to <c>this.Line</c>).</param>
        /// <param name="retrims">List of intersected lines that will need to be retrimmed.</param>
        internal void SplitX(LineFeature splitter, List<LineFeature> retrims)
        {
            Debug.Assert(Object.ReferenceEquals(m_Line, splitter.LineGeometry));

            // Return if no intersections.
            if (m_Intersects.Count==0)
                return;

            // Cut up the things that were intersected, making grazing
	        // portions non-topological.
            foreach (IntersectionResult r in m_Intersects)
            {
                SplitData sd = new SplitData(r);
                //if (sd.RequiresRetrim)
                //    retrims.Add(r.IntersectedObject);
            }

            // Combine the results and get the splitter to cut itself up.
            splitter.SplitAtIntersections(this);
        }

        /// <summary>
        /// Draws intersections on the specified display
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The drawing style</param>
        internal void Render(ISpatialDisplay display, IDrawStyle style)
        {
            foreach (IntersectionResult r in m_Intersects)
                r.Render(display, style);
        }
    }
}
