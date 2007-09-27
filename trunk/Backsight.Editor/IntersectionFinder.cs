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
	    LineGeometry m_Line;

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
        /// Constructor
        /// </summary>
        /// <param name="line">The primitive being intersected.</param>
        /// <param name="wantEndEnd">Should end-to-end intersections (simple ones) be included
        /// in the results. Default=TRUE.</param>
        internal IntersectionFinder(LineGeometry line, bool wantEndEnd)
        {
            m_Line = line;
            Load(wantEndEnd);
        }

        /// <summary>
        /// Constructor for a feature
        /// </summary>
        /// <param name="f">The feature involved</param>
        /// <param name="obj">The geometry that needs to be intersected.</param>
        /// <param name="wantEndEnd">Should end-to-end intersections (simple ones) be included
        /// in the results. Default=TRUE.</param>
        /*
        IntersectionFinder(IFeature f, IIntersectable obj, bool wantEndEnd)
        {
            m_Object = obj;
            m_Layers = new LayerList(f);
            Load(wantEndEnd);
        }
         */

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
        internal LineGeometry Geometry
        {
            get { return m_Line; }
        }

        /// <summary>
        /// Loads all intersections with this object. This is called by each constructor.
        /// </summary>
        /// <param name="wantEndEnd">Should end-to-end intersections (simple ones) be included
        /// in the results. Applies only when the primitive being intersected is a line.</param>
        void Load(bool wantEndEnd)
        {
            ISpatialIndex index = CadastralMapModel.Current.Index;
            m_Intersects = new FindIntersectionsQuery(index, m_Line, wantEndEnd).Result;

        }
        /*

//	Get a list of the tiles that the primitive passes through.
	CeTileList tlist(*m_pObject);

	// Get reference to the spatial index.
	const CeSpace& space = CeMap::GetpMap()->GetSpace();

	// Get the spatial index to return a list of all the candidates
	// that fall within these tiles (this returns ONLY line primitives).
	CeFixedArray<CeLine*> xlines;
	UINT4 ncand = space.GetXCandidates(xlines,tlist,searchwin,m_pLayers,m_pObject);
	if ( ncand==0 ) return;

//	The list of lines refers to those that have a window
//	that overlaps the window of the candidate object. Now do
//	some more intensive calculation to get actual intersections.

	for ( UINT4 i=0; i<ncand; i++ ) {
		const CeLine* const pxLine = xlines[i];
		if ( (CePrimitive*)pxLine==m_pObject ) continue;
		CeXResult other(*pxLine);
		m_pObject->Intersect(other);

		// Skip if we didn't get anything.
		UINT4 nx = other.GetCount();
		if ( nx==0 ) continue;

		// If we just intersected a line, determine the context
		// of each intersection. If end-to-end simple intersections
		// are not required, weed them out.
		if ( pLine ) {
			other.SetContext(*pLine);
			if ( !wantEndEnd ) nx = other.CutEndEnd();
		}

		if ( nx ) this->Append(other);
	}

} // end of Load
*/
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
        bool IsGrazing
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
        bool IsSplitNeeded
        {
            get
            {
                // Go through each object we intersected with, looking for an intersection
                // that does not occur at the ends of the line primitive.

                foreach(IntersectionResult r in m_Intersects)
                {
                    if (r.IsSplitOn(m_Line))
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Cuts up the topological arcs that are referred to by this intersection object.
        /// This will only do something if a set of applicable layers is defined.
        /// </summary>
        /// <param name="splitter">The line that is causing the split (the same as a call to <c>this.Line</c>).</param>
        /// <param name="retrims">List of intersected lines that will need to be retrimmed.</param>
        /*
        void SplitX(LineFeature splitter, List<LineFeature> retrims)
        {
            Debug.Assert(Object.ReferenceEquals(m_Line, splitter));

	        // Return if no intersections.
	        if (m_Intersects.Count==0)
                return;

	        // Cut up the things that were intersected, making grazing
	        // portions non-topological.
            foreach (IntersectionResult r in m_Intersects)
            {
                r.SplitX(retrims);
	        }

	        // Combine the results and get the splitter to cut itself up.
            IntersectionResult xres = new IntersectionResult(this);
	        //splitter.Split(xres);
            throw new NotImplementedException();
        }
         */
	}
}
