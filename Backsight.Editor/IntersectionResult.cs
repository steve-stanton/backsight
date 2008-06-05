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
        readonly IIntersectable m_IntersectedObject;

        /// <summary>
        /// The concrete geometry obtained from the intersected object.
        /// </summary>
        readonly LineGeometry m_Geom;

        /// <summary>
        /// Intersection data.
        /// </summary>
        List<IntersectionData> m_Data;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>IntersectionResult</c> for the specified line. This
        /// implementation is used when intersecting ad-hoc geometry, as well as
        /// representing the results of intersecting a line feature with previously
        /// existing topology.
        /// </summary>
        /// <param name="intersectedObject"></param>
        internal IntersectionResult(IIntersectable intersectedObject)
        {
            m_IntersectedObject = intersectedObject;
            m_Geom = intersectedObject.LineGeometry;
            m_Data = new List<IntersectionData>();
        }

        /// <summary>
        /// Constructor used to combine a series of <c>IntersectResult</c> objects. This
        /// implementation is used to obtain a results object for a new topological line
        /// that has just been intersected against the map.
        /// </summary>
        /// <param name="xsect">The result of intersecting a line with the map</param>
        internal IntersectionResult(IntersectionFinder xsect)
        {
            m_IntersectedObject = xsect.Intersector;
            m_Geom = m_IntersectedObject.LineGeometry;
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
            get { return m_IntersectedObject; }
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
            ICircleGeometry circle = new CircleGeometry(center, radius);
            return m_IntersectedObject.LineGeometry.IntersectCircle(this, circle);
        }

        // Segment
        internal uint Intersect(IPointGeometry start, IPointGeometry end)
        {
            ILineSegmentGeometry seg = new LineSegmentGeometry(start, end);
            return m_IntersectedObject.LineGeometry.IntersectSegment(this, seg);
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
                m_IntersectedObject.LineGeometry.SetSortValues(m_Data);
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


        //	@parm	The point feature that correspond to the search location.
        //			Defined only if a better intersection is actually found.

        /// <summary>
        /// Calculates whether the supplied point is closer to the intersections defined
        /// in this results object.
        /// </summary>
        /// <param name="point">The point to test</param>
        /// <param name="mindsq">The best distance (squared) so far. This should be initially passed in
        /// as a very big value.</param>
        /// <param name="xsect">The intersection. This will be defined only if a better intersection is
        /// actually found.</param>
        /// <returns>True if a <paramref name="point"/> is closer to an intersection (in that case,
        /// <paramref name="mindsq"/> and <paramref name="xsect"/> will have been updated). False
        /// if the point is no closer.</returns>
        /// <remarks>This replaces the old CeArc.GetCloserPoint method</remarks>
        internal bool GetCloserPoint(IPosition point, ref double mindsq, ref IPosition xsect)
        {
            // Scan the set of intersections to find the closest, disallowing
            // any intersections that are coincident with the search location
            // (to within some very small tolerance).
            IPosition xi;
            if (!GetClosest(point, out xi, Constants.XYTOLSQ))
                return false;

            // Get the distance (squared) to the intersection,
            double dsq = Geom.DistanceSquared(xi, point);
            if (mindsq < dsq)
                return false;

            // The newly found intersection is the best so far.
            mindsq = dsq;
            xsect = xi;
            return true;
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
                d.SetContext(m_IntersectedObject.LineGeometry, line);
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
	        SetContext(m_IntersectedObject.LineGeometry);

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
            return m_IntersectedObject.LineGeometry.IntersectSegment(this, seg);
        }

        internal uint IntersectArc(ICircularArcGeometry arc)
        {
            return m_IntersectedObject.LineGeometry.IntersectArc(this, arc);
        }

        internal uint IntersectMultiSegment(IMultiSegmentGeometry line)
        {
            return m_IntersectedObject.LineGeometry.IntersectMultiSegment(this, line);
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
