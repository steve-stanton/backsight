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

using Backsight.Geometry;

namespace Backsight.Editor
{
	/// <written by="Steve Stanton" on="13-FEB-1998" was="CeXResult" />
    /// <summary>
    /// A single intersected feature found by an <c>IntersectionFinder</c>, together
    /// with information about the detected intersection(s).
    /// </summary>
    /// <see>IntersectionFinder</see>
    class IntersectionResult
    {
        #region Class data

        /// <summary>
        /// The object that is intersected.
        /// </summary>
        IIntersectable m_Line;

        /// <summary>
        /// Intersection data.
        /// </summary>
        List<IntersectionData> m_Data;

        #endregion

        #region Constructors

        internal IntersectionResult(IIntersectable line)
        {
            m_Line = line.LineGeometry;
            m_Data = new List<IntersectionData>();
        }

        /// <summary>
        /// Constructor used to combine a series of <c>IntersectResult</c> objects
        /// </summary>
        /// <param name="line">The line that was intersected</param>
        /// <param name="xsect">The result of intersecting the line with the map</param>
        internal IntersectionResult(LineFeature line, IntersectionFinder xsect)
        {
            m_Line = line.LineGeometry;
            m_Data = null;

            // Get the total number of intersections that were found (there's one
            // InteresectionResult for each intersected line)
            IList<IntersectionResult> results = xsect.Intersections;
            int nData = 0;
            foreach (IntersectionResult r in results)
                nData += r.m_Data.Count;

            // Return if no intersections.
            if (nData==0)
                return;

            // Copy over the info
            m_Data = new List<IntersectionData>(nData);
            foreach (IntersectionResult r in results)
            {
                List<IntersectionData> data = r.Intersections;
                foreach (IntersectionData d in data)
                    m_Data.Add(d);
            }

            // Reverse all context codes.
            foreach (IntersectionData d in m_Data)
                d.ReverseContext();
        }

        #endregion

        internal IIntersectable IntersectedObject
        {
            get { return m_Line; }
        }

        internal int IntersectCount
        {
            get { return (m_Data==null ? 0 : m_Data.Count); }
        }

        internal List<IntersectionData> Intersections
        {
            get { return m_Data; }
        }

        // Circle
        internal uint Intersect(IPointGeometry center, double radius)
        {
            ILength r = new Length(radius);
            ICircleGeometry circle = new CircleGeometry(center, r);
            return m_Line.LineGeometry.IntersectCircle(this, circle);
        }

        // Segment
        internal uint Intersect(IPointGeometry start, IPointGeometry end)
        {
            ILineSegmentGeometry seg = new LineSegmentGeometry(start, end);
            return m_Line.LineGeometry.IntersectSegment(this, seg);
        }
        /*
        // Multisegment
        uint Intersect(IPointGeometry[] locs)
        {
            return m_Object.Intersect(this, locs);
        }
        */

        /// <summary>
        /// Append intersection info to this object.
        /// </summary>
        /// <param name="xsect">The intersection info to append.</param>
        void Append(IntersectionData xsect)
        {
            m_Data.Add(xsect);
        }

        /// <summary>
        /// Appends a simple intersection. 
        /// </summary>
        /// <param name="xi">Easting of the intersection.</param>
        /// <param name="yi">Northing of the intersection.</param>
        /// <param name="sortval">Sort value (default=0.0).</param>
        internal void Append(double xi, double yi, double sortval)
        {
            IntersectionData xsect = new IntersectionData(xi, yi, sortval);
            Append(xsect);
        }

        /// <summary>
        /// Appends a grazing intersection.
        /// </summary>
        /// <param name="x1">Easting of the 1st intersection.</param>
        /// <param name="y1">Northing of the 1st intersection.</param>
        /// <param name="x2">Easting of the 2nd intersection.</param>
        /// <param name="y2">Northing of the 2nd intersection.</param>
        internal void Append(double x1, double y1, double x2, double y2)
        {
            IntersectionData xsect = new IntersectionData(x1, y1, x2, y2);
            Append(xsect);
        }

        /// <summary>
        /// Appends a simple intersection. 
        /// </summary>
        /// <param name="p">The position of the intersection.</param>
        internal void Append(IPosition p)
        {
            Append(p.X, p.Y, 0.0);
        }

        /// <summary>
        /// Appends a grazing intersection.
        /// </summary>
        /// <param name="p">The position of the 1st intersection.</param>
        /// <param name="q">The position of the 2nd intersection.</param>
        internal void Append(IPosition p, IPosition q)
        {
            Append(p.X, p.Y, q.X, q.Y);
        }

        internal void SplitWhereIntersected(List<LineFeature> retrims)
        {
            SplitWhereIntersected(retrims, true);
        }

        /// <summary>
        /// Splits the features attached to the primitive that these
        /// intersection results refer to. Only does something if the results refer
        /// to a line primitive. Anything else will be quietly ignored.
        /// </summary>
        /// <param name="retrims">List of lines that will need to be retrimmed.</param>
        /// <param name="needsort">Does data need to be sorted along the line (default=TRUE).
        /// Specify FALSE only if the intersection results were obtained via self-intersection
        /// of a multisegment</param>
        internal void SplitWhereIntersected(List<LineFeature> retrims, bool needsort)
        {
            // Get a list of the topological arcs that are attached to the
            // line primitive, and that have a theme that overlays the
            // required layers.
            throw new NotImplementedException("IntersectionResult.SplitX");
            /*
	        CeObjectList xarcs;
	        UINT4 nxarc = pLine->GetArcs(xarcs,&layers);

	        // Return if there are no suitable arcs (this should really have
	        // been caught before now, but we'll let it pass).
	        if ( nxarc==0 ) return;

	        // Sort the intersections along the line we intersected.
	        if ( needsort ) this->Sort();

	        // Loop through each attached arc, cutting them up at the
	        // intersections.

	        CeListIter loop(&xarcs);
	        CeArc* pArc;

	        for ( pArc = (CeArc*)loop.GetHead();
		          pArc;
		          pArc = (CeArc*)loop.GetNext() ) {

		        // Ignore arcs that are marked as moved. This may occur
		        // in cases where a line has more than one arc attached
		        // to it. In this case, one of the arcs has been processed
		        // by CeMap::Intersect, thereby making the line primitive
		        // visible to subsequent arcs. However, not all the arcs
		        // may have been revealed just yet.
		        if ( pArc->IsMoved() ) continue;

		        // Get a split object for the arc.
		        CeSplit* pSplit = dynamic_cast<CeSplit*>(pArc->GetpCreator());

		        // If the arc is inactive, it means that it is the hidden
		        // portion of a trimmed line. Add the parent line in to
		        // the list of things that will need to be retrimmed. And
		        // revert the arc section to a normal line.

		        if ( pArc->IsInactive() ) {
			        assert(pSplit);
			        pArc->UnTrim();
			        if ( pSplit ) retrims.AddReference(pSplit->GetpArc());
		        }

		        // If there is no split, get one.
		        if ( !pSplit ) pSplit = pArc->GetpSplit(TRUE);

		        // Make additional splits. If the split object ends up
		        // referring to NOTHING (possible if the intersections
		        // are only at the end points), get rid of it.
		        if ( !pSplit->Create(*pArc,*this,&layers) ) {
			        if ( pSplit->IsEmpty() ) {
				        pArc->CutOp(*pSplit);
				        delete pSplit;
			        }
		        }
	        }
             */
        }

        /// <summary>
        /// Checks whether any intersection is a graze.
        /// </summary>
        internal bool IsGrazing
        {
            get
            {
                foreach (IntersectionData d in m_Data)
                {
                    if (d.IsGraze)
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Checks whether any intersection is a graze at the start of a line.
        /// </summary>
        internal bool IsStartGrazing
        {
            get
            {
                foreach (IntersectionData d in m_Data)
                {
                    if (d.IsStartGraze)
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Checks whether any intersection is a graze at the end of a line.
        /// </summary>
        internal bool IsEndGrazing
        {
            get
            {
                foreach (IntersectionData d in m_Data)
                {
                    if (d.IsEndGraze)
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Checks whether any intersections occur at positions that do not coincide
        /// with the end points of a line.
        /// </summary>
        /// <param name="line">The line to check. This should be the same line that
        /// was supplied to the <c>IntersectionFinder</c> constructor that created THIS results
        /// object (doing otherwise doesn't make any sense).</param>
        /// <returns>TRUE if any intersection does not coincide with the end points
        /// of the specified line.</returns>
        internal bool IsSplitOn(ILineGeometry line)
        {
            // Get the locations of the line end points.
            IPointGeometry start = line.Start;
            IPointGeometry end = line.End;

            // Go through each intersection, looking for one that does
            // not correspond to the line ends.

            foreach (IntersectionData d in m_Data)
            {
                IPointGeometry loc1 = new PointGeometry(d.P1);
                if (!start.IsCoincident(loc1) && !end.IsCoincident(loc1))
                    return true;

                if (d.IsGraze)
                {
                    /*
                     * Huh? This was the original, but it always ended up returning true.
                     * 
			        IPointGeometry loc2 = PointGeometry.New(d.P2);
                    if (!start.IsCoincident(loc2) && !end.IsCoincident(loc2))
                        return true;

                    return true;
                     */
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// The number of intersection that are grazes.
        /// </summary>
        uint NumGraze
        {
            get
            {
                uint ngraze=0;
                foreach (IntersectionData d in m_Data)
                {
                    if (d.IsGraze)
                        ngraze++;
                }
                return ngraze;
            }
        }

        /// <summary>
        /// Ensures that intersections have been sorted in the same direction as
        /// the line to which these intersection results relate.
        /// </summary>
        /// <param name="setsort">TRUE (the default) means sort values need to be defined.
        /// Specify FALSE only if you have supplied sort values when intersections were
        /// appended.</param>
        internal void Sort(bool setsort)
        {
            // Assign sort values to each intersection.
            if (setsort)
            {
                m_Line.LineGeometry.SetSortValues(m_Data);
            }

            m_Data.Sort(delegate(IntersectionData a, IntersectionData b)
                            { return a.CompareTo(b); });
        }

        /// <summary>
        /// Perform self-intersection on the primitive that this object refers to.
        /// <para/>
        /// NOTE: Only works for multi-segments. You will get an error message if you
        /// try to do it with anything else. Even sections based on multi-segments
        /// won't work.
        /// <todo>This is messy & should be changed</todo>
        /// </summary>
        /// <returns>The number of self-intersections that were detected.</returns>
        /*
        uint SelfIntersect()
        {
            return m_Line.LineGeometry.SelfIntersect(m_Data);
        }
        */

        /// <summary>
        /// Checks if these intersection results refer to a specific position. The match
        /// has to be exact.
        /// </summary>
        /// <param name="p">The position to check for.</param>
        /// <returns>True if the position was found.</returns>
        bool IsReferredTo(IPosition p)
        {
            foreach (IntersectionData d in m_Data)
            {
                if (d.IsReferredTo(p))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the position of the intersection that is closest to a specific position.
        /// </summary>
        /// <param name="posn">The position to search for.</param>
        /// <param name="xsect">The closest intersection.</param>
        /// <param name="tolsq">The smallest allowable distance (squared) between the
        /// search position and the closest intersection (default=0.0).</param>
        /// <returns>TRUE if the position was found.</returns>
        internal bool GetClosest(IPosition posn, out IPosition xsect, double tolsq)
        {
            xsect = null;

            // Go through each intersection, looking for the closest
            // intersection that is not TOO close.

            double mindsq = Double.MaxValue;
            double dsq;

            foreach (IntersectionData d in m_Data)
            {
                dsq = BasicGeom.DistanceSquared(posn, d.P1);
                if (dsq<mindsq && dsq>tolsq)
                {
                    mindsq = dsq;
                    xsect = d.P1;
                }

                // If the intersection is a graze, check the 2nd
                // intersection too.
                if (d.IsGraze)
                {
                    dsq = BasicGeom.DistanceSquared(posn, d.P2);
                    if (dsq<mindsq && dsq>tolsq)
                    {
                        mindsq = dsq;
                        xsect = d.P2;
                    }
                }
            }

            return (xsect!=null);
        }

        /// <summary>
        /// Eliminates all simple intersections that refer to lines that only meet end
        /// to end. A prior call to <c>SetContext</c> is required.
        /// </summary>
        /// <returns>The number of intersections after elimination of unwanted intersects.</returns>
        internal uint CutEndEnd()
        {
            // Return if there are no intersections.
            if (m_Data==null || m_Data.Count==0)
                return 0;

            // Go through each simple intersection, undefining those
            // results that are redundant.
            int nLeft = m_Data.Count;
            foreach (IntersectionData d in m_Data)
            {
                if (d.IsEndEnd)
                {
                    d.Reset();
                    nLeft--;
                }
            }

            return Squeeze(nLeft);
        }

        /// <summary>
        /// Marks intersection data with flags that signify the relationship of these
        /// intersection results to the position of a specific line (presumably the
        /// line that caused the generation of these results).
        /// </summary>
        /// <param name="line">The line to compare with.</param>
        /// <returns>TRUE if any context settings were made. It will be FALSE if there
        /// are no intersections, or these results do not refer to a line.</returns>
        internal bool SetContext(ILineGeometry line)
        {
            // Return if there are no intersections.
            if (m_Data==null || m_Data.Count==0)
                return false;

            // Go through each intersected object. For those that are
            // lines, figure out the relationship to the supplied line.
            foreach (IntersectionData d in m_Data)
            {
                d.SetContext(m_Line.LineGeometry, line);
            }

            return true;
        }

        /// <summary>
        /// Simplify these intersection results by converting any grazing intersections
        /// into simple intersections. Given that there are any grazes, the results will be
        /// re-sorted so that the end of each graze comes at it's proper sequence along the
        /// line that they relate to (this covers the fact that grazes may overlap). Any
        /// consecutive duplicates will be removed too (covering the fact that the intersected
        /// line may have passed through a node). Intersections at the end of the line will be
        /// eliminated.
        /// </summary>
        /// <returns>The number of intersection after simplification.</returns>
        internal uint Simplify()
        {
            // Return if there are no intersections.
            if (m_Data==null || m_Data.Count==0)
                return 0;

            // If we have any grazes, copy each intersection to a new results array (and
		    // for each graze, stick in the end of the graze as well).
	        uint ngraze = this.NumGraze;
            if (ngraze>0)
            {
                List<IntersectionData> newres = new List<IntersectionData>((int)(m_Data.Count + ngraze));
                foreach(IntersectionData d in m_Data)
                {
                    if (d.IsGraze)
                    {
                        newres.Add(new IntersectionData(d.P1));
                        newres.Add(new IntersectionData(d.P2));
                    }
                    else
                        newres.Add(d);
                }

        		m_Data = newres;
            }

            // Ensure the intersections are sorted properly.
	        this.Sort(true);

            // Ensure context is set properly.
	        SetContext(m_Line.LineGeometry);

            // Null out any consecutive duplicates, as well as intersections
	        // at the end of the line.

	        int nLeft = m_Data.Count;		// How many have we got left?

        	// Reset any consecutive duplicates.

            IntersectionData prev = m_Data[0];
            IntersectionData cur = null;

            for(int i=1; i<m_Data.Count; i++, prev=cur)
            {
                cur = m_Data[i];

                // Reset the previous intersection if it has the same sort value
                // as the current one.
                if (Math.Abs(cur.GetDeltaSort(prev)) < Constants.XYRES)
                {
                    prev.Reset();
                    nLeft--;
                }
            }

        	// Reset anything at either end of the line.
            foreach(IntersectionData d in m_Data)
            {
                if (d.IsEnd)
                {
                    d.Reset();
                    nLeft--;
                }
            }

    	    // Squeeze out the redundant intersections.
            return Squeeze(nLeft);
        }

        /// <summary>
        /// Squeezes out any undefined intersections.
        /// </summary>
        /// <param name="nLeft">The number of intersections that should be left after the
        /// squeeze. Must be greater than zero.</param>
        /// <returns>The number of intersections after the squeeze.</returns>
        uint Squeeze(int nLeft)
        {
            Debug.Assert(nLeft>=0);
            if (nLeft==m_Data.Count)
                return (uint)m_Data.Count;

            // Squeeze out the redundant intersections.
            if (nLeft==0)
            {
                m_Data = null;
                return 0;
            }

            List<IntersectionData> newData = new List<IntersectionData>(nLeft);
            foreach (IntersectionData d in m_Data)
            {
                if (d.IsDefined)
                    newData.Add(d);
            }
            m_Data = newData;
            return (uint)m_Data.Count;
        }

        internal uint IntersectSegment(ILineSegmentGeometry seg)
        {
            return m_Line.LineGeometry.IntersectSegment(this, seg);
        }

        internal uint IntersectArc(ICircularArcGeometry arc)
        {
            return m_Line.LineGeometry.IntersectArc(this, arc);
        }

        internal uint IntersectMultiSegment(IMultiSegmentGeometry line)
        {
            return m_Line.LineGeometry.IntersectMultiSegment(this, line);
        }

        /// <summary>
        /// Draws intersections on the specified display
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The drawing style</param>
        internal void Render(ISpatialDisplay display, IDrawStyle style)
        {
            foreach (IntersectionData d in m_Data)
                d.Render(display, style);
        }
    }
}
