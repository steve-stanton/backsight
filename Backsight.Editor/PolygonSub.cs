// <remarks>
// Copyright 2007 - Steve Stanton. This file is part of Backsight
//
// Backsight is free software; you can redistribute it and/or modify it under the terms
// of the GNU Lesser General Public License as published by the Free Software Foundation;
// either version 3 of the License, or (at your option) any later version.
//
// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
// </remarks>

using System;
using System.Diagnostics;

namespace Backsight.Editor
{
	/// <written by="Steve Stanton" on="26-FEB-1998" />
    /// <revision author="Steve Stanton" date="03-DEC-2002">
    /// Major revision to accomodate polygons in which the divider may contain
    /// points on secondary faces.
    /// </revision>
    /// <summary>
    /// Information that can be used to automatically subdivide a polygon.
    /// </summary>
    class PolygonSub
    {
        #region Class data

        /// <summary>
        /// The polygon being subdivided.
        /// </summary>
        Polygon m_Polygon;

        /// <summary>
        /// Information about each face of the polygon
        /// </summary>
        PolygonFace[] m_Faces;

        /// <summary>
        /// Information about each significant point
        /// </summary>
        PolygonLink[] m_Links;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor (that does a lot of stuff)
        /// </summary>
        /// <param name="polygon">The polygon being subdivided.</param>
        internal PolygonSub(Polygon polygon)
        {
            m_Polygon = polygon;
            m_Faces = null;
            m_Links = null;

            // Get the boundaries defining the exterior edge of the polygon
            IDivider[] lines = polygon.Edge;
            if (lines==null || lines.Length==0)
                return;

            // Allocate an array for holding info on each polygon face,
            // and associate each one with the divider that acts as the primary face.
            uint numLink = 0;
            m_Faces = new PolygonFace[lines.Length];
            for (int i=0; i<lines.Length; i++)
            {
                m_Faces[i] = new PolygonFace();
                numLink += m_Faces[i].SetDivider(m_Polygon, lines[i]);
            }

            // Create an array of link objects (one for each point)
            if (numLink==0)
                return;

            m_Links = new PolygonLink[numLink];

            // Initialize the links for first face
            int nDone = 0;
            PolygonLink[] links = m_Faces[0].CreateLinks(m_Faces[m_Faces.Length-1]);
            Array.Copy(links, m_Links, links.Length);
            nDone += links.Length;

            // Initialize links for each subsequent face
            for (int i=1; i<m_Faces.Length; i++)
            {
                links = m_Faces[i].CreateLinks(m_Faces[i-1]);
                for (int j=0; j<links.Length; j++)
                    m_Links[nDone+j] = links[j];

                nDone += links.Length;
            }


            Debug.Assert(nDone==m_Links.Length);

            // Assign a side number to each link position

            uint nside=0;		// No side number assigned so far.
            uint ncorn=0;		// Number of corners

            for (int i=0; i<m_Links.Length; i++)
            {

                // Skip if we are dealing with a corner.
                if (m_Links[i].IsCorner())
                {
                    ncorn++;
                    continue;
                }

                // Skip if the side number has already been assigned.
                if (m_Links[i].Side!=0)
                    continue;

                // Assign next side number.
                nside++;

                // Continue till we reach a corner, assigning intermediate
                // faces the same side number.
                for (int j=i; j<m_Links.Length; j++)
                {
                    if (m_Links[j].IsCorner())
                        break;
                    m_Links[j].Side = nside;
                }

                // If we just assigned a side number to the very first face,
                //  loop back from the end of the array.
                if (i==0)
                {
                    for (int k=m_Links.Length-1; k>0; k--)
                    {
                        if (m_Links[k].IsCorner())
                            break;
                        m_Links[k].Side = nside;
                    }
                }
            }

            // Return if we only have one side (or less)
            // if ( nside<2 ) return;

            // Define the max angular difference (10 degrees, but in radians)
            double ANGTOL = (10.0 * Constants.DEGTORAD);

            // Process all the radial faces first.

            for (uint i=0; i<m_Links.Length; i++)
            {
                if (m_Links[i].IsRadial)
                    this.SetLink(i, ANGTOL);
            }

            // For each point that is not a corner or a curve end, cycle
            // through all the subsequent points, looking for a point to
            // connect to. Don't try to do curve end points just yet.

            for (uint i=0; i<m_Links.Length; i++)
            {
                if (!m_Links[i].IsRadial && !m_Links[i].IsCurveEnd)
                    this.SetLink(i, ANGTOL);
            }

            // Process any points at the end of curves.

            for (uint i=0; i<m_Links.Length; i++)
            {
                if (m_Links[i].IsCurveEnd)
                    this.SetLink(i, ANGTOL);
            }

            // Eliminate self-intersections ...

            // Start by counting the number of intersections each link has.
            uint maxnx=0;

            for (uint i=0; i<m_Links.Length; i++)
            {
                uint nx = this.SetIntersectionCount(i);
                maxnx = Math.Max(maxnx, nx);
            }

            uint nloop=0;		// Number of loops (just in case of infinite loop).

            while (maxnx > 0)
            {
                // Find a link with the max number of intersections.
                int rem=-1;
                for (int i=0; i<m_Links.Length; i++)
                {
                    if (m_Links[i].NumIntersect == maxnx)
                    {
                        rem = i;
                        break;
                    }
                }

                // We SHOULD have found something.
                if (rem<0)
                    throw new Exception("PolygonSub: Unexpected intersection count");

                // If the count was greater than one, just remove the link. Otherwise get the
                // thing that was intersected. If one is a radial or a curve end, & the other isn't,
                // drop the radial. Otherwise drop the one with the poorer link angle.

                if (maxnx > 1)
                    m_Links[rem].SetLink(null);
                else
                {
                    PolygonLink pLink1 = m_Links[rem];
                    PolygonLink pLink2 = null;
                    for (int i=0; pLink2!=null && i<m_Links.Length; i++)
                    {
                        if (pLink1.IsIntersected(m_Links[i]))
                            pLink2 = m_Links[i];
                    }

                    if (pLink2==null)
                        throw new Exception("PolygonSub: Intersection not found");

                    // Check if radial.
                    bool rad1 = pLink1.IsRadial || pLink1.Link.IsRadial;
                    bool rad2 = pLink2.IsRadial || pLink2.Link.IsRadial;

                    // Check if end of curves.
                    bool end1 = pLink1.IsCurveEnd || pLink1.Link.IsCurveEnd;
                    bool end2 = pLink2.IsCurveEnd || pLink2.Link.IsCurveEnd;

                    // We treat radials and end of curves the same.
                    bool curve1 = (rad1 || end1);
                    bool curve2 = (rad2 || end2);

                    // If one is a curve-related and the other isn't, drop
                    // the one that's curve-related.
                    if (curve1 != curve2)
                    {
                        if (curve1)
                            pLink1.SetLink(null);
                        else
                            pLink2.SetLink(null);
                    }
                    else
                    {
                        // Neither is curve related, or both are. Drop the
                        // one with the poorer link angle.

                        if (pLink1.LinkAngle > pLink2.LinkAngle)
                            pLink1.SetLink(null);
                        else
                            pLink2.SetLink(null);
                    }
                }

                // Rework the intersect counts where necessary.

                maxnx = 0;
                for (uint i=0; i<m_Links.Length; i++)
                {
                    if (m_Links[i].NumIntersect > 0)
                        maxnx = Math.Max(maxnx, this.SetIntersectionCount(i));
                }

                // Make sure we don't go into an infinite loop.
                nloop++;
                if (nloop > m_Links.Length)
                    throw new Exception("PolygonSub: Breaking from infinite loop");

            } // end while
        }

        #endregion

        /// <summary>
        /// The polygon being subdivided.
        /// </summary>
        internal Polygon Polygon
        {
            get { return m_Polygon; }
        }

        /// <summary>
        /// Sets the number of intersections for one of the polygon link points.
        /// </summary>
        /// <param name="index">The array index of the link point to process.</param>
        /// <returns>The number of intersections assigned.</returns>
        uint SetIntersectionCount(uint index)
        {
            // No intersections so far
            uint nx=0;

            //	Point to the thing we want to set the count for.
            PolygonLink link = m_Links[index];

            // Go through all link points searching for intersections.
            for (uint i=0; i<m_Links.Length; i++)
            {
                if (link.IsIntersected(m_Links[i]))
                    nx++;
            }

            // Set the intersect count.
            link.NumIntersect = nx;
            return nx;
        }

        /// <summary>
        /// Tries to set the link for one of the link points.
        /// </summary>
        /// <param name="start">The array index of the link point to process.</param>
        /// <param name="angtol">Angular tolerance (in radians) for seeing if a link
        /// candidate has a bearing that's close enough to the desired bearing.</param>
        /// <returns>True if a link was set.</returns>
        bool SetLink(uint start, double angtol)
        {
            // Note our lucky contestant and it's side number.
            PolygonLink from = m_Links[start];
            uint fromside = from.Side;

            // Nothing to do if no side number (corner point).
            if (fromside==0)
                return false;

            // Skip if already linked.
            if (from.IsLinked)
                return false;

            // Are we doing a connection for a radial point? In that case, we are
            // allowed to connect to corner points. We do radial points FIRST.
            bool radial = from.IsRadial;


            // Are we doing a curve end point? We do curve end points LAST.
            bool curveend = from.IsCurveEnd;

            // Initialize info on best link so far.
            PolygonLink to = null;
            double bestangle = Constants.PIMUL2;
            double thisdiff = Constants.PIMUL2;
            double othrdiff = Constants.PIMUL2;

            double MINDSQ = 4.0;		// Min distance (squared) between points.

            // Loop through every link point, looking for a match.
            for (uint j=0; j<m_Links.Length; j++)
            {
                // Skip if this is the main candidate.
                if (j==start)
                    continue;

                // Skip if no side number (corners), or it's the same side
                // as the main candidate. We do permit an extension to a corner
                // if we're doing a radial link.
                uint curside = m_Links[j].Side;
                //	if ( curside==0 || curside==fromside ) continue;
                if (curside==0 && !radial)
                    continue;

                // Skip if already linked.
                // if (m_Links[j].IsLinked) continue;

                // Only consider curve end points if the main candidate is a
                // curve end point too, or a radial.
                if (m_Links[j].IsCurveEnd && !(curveend || radial))
                    continue;

                // Skip if the distance to the other point is smaller
                // than 2 meters (squared) on the ground (the number's just a stab)
                if (from.DistanceSquared(m_Links[j]) < MINDSQ)
                    continue;

                // Get the angle formed between the candidate's reference
                // bearing, and the position of the current link point. If
                // it exceeds the angular tolerance, skip to next.
                double angle1 = from.GetAngle(m_Links[j]);
                if (angle1 > angtol)
                    continue;

                // Do the reverse test so long as the main candidate is
                // not a radial (when a radial hits a straight edge, the
                // ideal bearing for that point is likely to be close to
                // a perpendicular to the straight edge, so it probably
                // won't be within tolerance of the radial).

                double angle2;

                if (radial || m_Links[j].IsRadial)
                    angle2 = angle1;
                else
                    angle2 = m_Links[j].GetAngle(from);

                if (angle2 > angtol)
                    continue;

                // Skip if the candidate was previously linked, and the angle
                // we have now is worse.
                if (m_Links[j].IsLinked && angle2 > m_Links[j].LinkAngle)
                    continue;

                // If we are NOT doing a radial link, confirm that the proposed
                // link does not cross any radial links we may have formed	(we
                // did all radial links first).

                // If the main candidate is a curve end point, it can't intersect
                // ANYTHING else. If it's not a curve end point or a radial, all
                // we should check are radials. We process data in the order:
                // 1. radials
                // 2. not radials or curve ends
                // 3. curve ends

                // if ( curveend ) {
                //   if ( this->IsIntersected(*pFrom,m_Links[j],FALSE) ) continue;
                // }
                // else if ( !radial ) {
                //   if ( this->IsIntersected(*pFrom,m_Links[j],TRUE) ) continue;
                // }

                // Use the best angular deviation.
                double angle = Math.Min(angle1, angle2);

                // If the deviation is better than anything we already have,
                // and the link is valid, remember the current link point
                // as the best one so far.

                if (angle<bestangle && from.IsLinkValid(m_Links[j], m_Polygon))
                {
                    bestangle = angle;
                    thisdiff = angle1;
                    othrdiff = angle2;
                    to = m_Links[j];
                }

            } // next point

            // Set the link if we got one.
            if (to!=null)
            {
                from.SetLink(to, thisdiff, othrdiff);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to get a link between two points.

        /// </summary>
        /// <remarks>
        /// You can cycle through the links as soon as the constructor has been called, using
        /// a loop like:
        /// <code>
        /// 
        ///   PolygonSub sub = new PolygonSub(pol);
        ///   PointFeature ps, pe;
        ///   for (int i=0; sub.GetLink(i,out ps, out pe); i++);
        /// 
        /// </code>
        /// </remarks>
        /// <param name="index">The index number of the link you want.</param>
        /// <param name="start">The point at the start of the link.</param>
        /// <param name="end">The point at the end of the link.</param>
        /// <returns>True if a link was returned</returns>
        internal bool GetLink(int index, out PointFeature start, out PointFeature end)
        {
            // Initialize return variables.
            start = end = null;

            // Loop through each point, checking to see whether it has a link. If so,
            // check if we have reached the desired index, and increment link count. This
            // may not be particularly efficient, but there shouldn't be that many links.

            PointFeature s, e; // Start and end of link
            int nLink = 0;     // No links so far

            for (int i=0; i<m_Links.Length; i++)
            {
                if (m_Links[i].GetLink(out s, out e))
                {
                    if (nLink==index)
                    {
                        start = s;
                        end = e;
                        return true;
                    }
                    nLink++;
                }
            }

            // Specified index is greater than the number of links we actually have.
            return false;
        }

        /// <summary>
        /// The number of links that have been formed.
        /// </summary>
        internal int NumLink
        {
            get
            {
                PointFeature start, end;
                int nLink=0;
                for (; GetLink(nLink, out start, out end); nLink++);
                return nLink;
            }
        }
    }
}
