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
using System.Collections.Generic;

namespace Backsight.Editor
{
	/// <written by="Steve Stanton" on="24-JUL-1997" was="CeConnection" />
    /// <summary>
    /// Information about the network topology for one end of a polygon ring divider.
    /// </summary>
    class ConnectionFinder
    {
        #region Class data

        /// <summary>
        /// The next anticlockwise divider in nodal cycle
        /// </summary>
        IDivider m_Next;

        /// <summary>
        /// True if we are dealing with the start of the next divider
        /// </summary>
        bool m_IsStart;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor 
        /// </summary>
        internal ConnectionFinder()
        {
            m_Next = null;
            m_IsStart = false;
        }

        /// <summary>
        /// Creates a new <c>ConnectionFinder</c> at a specific end of a divider.
        /// Defines the next divider in the nodal cycle (reckoned anticlockwise),
        /// along with a flag to indicate whether it is the start or end of the divider.
        /// 
        /// If the parameters supplied refer to a dangling divider, the result will
        /// be the SAME as the supplied info.
        /// 
        /// This constructor is normally used during polygon formation.
        /// </summary>
        /// <param name="d">The divider entering the node.</param>
        /// <param name="isStart">True if it's the start of the divider.</param>
        internal ConnectionFinder(IDivider d, bool isStart)
            : this(d, isStart, null)
        {
        }

        /// <summary>
        /// Creates a new <c>ConnectionFinder</c> at a specific end of a divider.
        /// Defines the next divider in the nodal cycle (reckoned anticlockwise),
        /// along with a flag to indicate whether it is the start or end of the divider.
        /// <para/>
        /// If the parameters supplied refer to a dangling divider, the result will
        /// be the SAME as the supplied info.
        /// <para/>
        /// This constructor is normally used during polygon formation. It is also used
        /// to resolve a special case in point in polygon tests, where the directed
        /// line (a <c>HorizontalRay</c> object) intersects at a node. In that case,
        /// the supplied divider refers to any one of the dividers incident on the
        /// node, and the connection to be formed is actually the connection for
        /// the <c>HorizontalRay</c> object.
        /// </summary>
        /// <param name="from">The divider entering the node.</param>
        /// <param name="isFromStart">True if it's the start of the divider.</param>
        /// <param name="ray">Optional reference line (used when doing special point
        /// in polygon tests).</param>
        internal ConnectionFinder(IDivider from, bool isFromStart, HorizontalRay ray)
        {
            Debug.Assert(from!=null);

            // Initialize with default values
            m_Next = null;
            m_IsStart = false;

            // Get the location involved
            ITerminal loc = (isFromStart ? from.From : from.To);

            // Get the dividers incident on the terminal.
            IDivider[] da = loc.IncidentDividers();

            // Get orientation info for each divider (the list could conceivably
            // grow if dividers start at the terminal, then loop round to also
            // finish at the same terminal).
            List<Orientation> orient = new List<Orientation>(da.Length);

            // If we are doing special stuff for point in polygon, add an extra orientation
            // object to refer to the HorizontalRay object.
            if (ray!=null)
            {
                Orientation o = new Orientation(ray);
                orient.Add(o);
            }

            foreach (IDivider d in da)
            {
                // 03-APR-2003: Try ignoring boundaries that have zero-length
                //if (!b.HasLength)
                //    continue;

                // Check if the start of the divider is incident on the node.
                // If so, append the orientation info to the list.
                if (d.From.IsCoincident(loc))
                    orient.Add(new Orientation(d, true));

                // Likewise for the end of the divider (it could start AND end at the node).
                if (d.To.IsCoincident(loc))
                    orient.Add(new Orientation(d, false));
            }

            // We MUST have at least one orientation point (two if a horizontal ray is also included)
            Debug.Assert(orient.Count>(ray==null ? 0 : 1));

            // If we have only one orientation point, we have a dangle,
            // so return the information we were supplied.
            if (ray==null && orient.Count==1)
            {
                m_Next = from;
                m_IsStart = isFromStart;
                return;
            }

            // Get a reference to the orientation info for the source divider (if
            // we are doing point in polygon stuff, we stashed the reference
            // segment in the first slot).

            Orientation source = null;

            if (ray!=null)
                source = orient[0];
            else
            {
                source = orient.Find(o => (o.Divider==from && o.IsStart==isFromStart));
                Debug.Assert(source!=null);
            }

            // If we have only two orientation points, the one we want
            // if the one which does not match the source.
            if (orient.Count==2)
            {
                Orientation o = (source==orient[0] ? orient[1] : orient[0]);
                m_Next = o.Divider;
                m_IsStart = o.IsStart;
                return;
            }

            // Handle any curves.
            if (HasAnyCurves(orient))
                source.SetCurves(orient);

            // If there are any other boundaries in the source quadrant,
            // try to find an anticlockwise divider (with the highest
            // orientation angle which is less than that of the source).
            Orientation next = FindPrevMax(source, orient);

            // If we got something, we're all done.
            if (next!=null)
            {
                m_Next = next.Divider;
                m_IsStart = next.IsStart;
                return;
            }

            // Loop through the quadrants, looking for the next divider. We
            // scan the quadrants anticlockwise, including the source
            // quadrant we may have just processing (possibility that the
            // next divider is further on in the source quadrant).

            Quadrant search = NextQuadrant(source.Quadrant);
            for (int i=0; i<4; i++, search=NextQuadrant(search))
            {
                next = FindMax(search, source, orient);
                if (next!=null)
                {
                    m_Next = next.Divider;
                    m_IsStart = next.IsStart;
                    return;
                }
            }

            throw new Exception("ConnectionFinder: network error");
        }

        #endregion

        /// <summary>
        /// Loop through an array of orientation data to try to find the max angle
        /// prior to a specific one.
        /// </summary>
        /// <param name="source">Source orientation (a reference to one of the
        /// items in the <c>orient</c> array.</param>
        /// <param name="orient">Orientation data for node.</param>
        /// <returns>The item in the supplied array that's immediately prior to
        /// the source orientation (next anti-clockwise). Null if the supplied array
        /// contains only one item (in that case, it is assumed to refer to the
        /// source orientation).</returns>
        /// <remarks>
        /// Since this function deals exclusively with <c>Orientation</c> stuff,
        /// it may be better to include this function in the <c>Orientation</c> class.
        /// </remarks>
        Orientation FindPrevMax(Orientation source, List<Orientation> orient)
        {
            Debug.Assert(orient.Count>0);

            // Return if there is nothing to process (if 1, it is assumed
            // to be the source orientation).
            if (orient.Count==1)
                return null;

            // If there is only one other to process, grab the one which
            // does not match the source orientation.
            if (orient.Count==2)
            {
                if (Object.ReferenceEquals(source, orient[0]))
                    return orient[1];
                else
                    return orient[0];
            }

            // Loop through the list of orientation data, looking for the
            // highest orientation which is in the same quadrant, but
            // which has an angle less than that of the source.

	        double srcangle = source.Angle;
	        double maxangle = -1.0;
            Orientation found = null;

            foreach(Orientation o in orient)
            {
                if (o.Quadrant==source.Quadrant && !Object.ReferenceEquals(o, source))
                {
                    // If the orientation is identical to the source orientation,
                    // note an exact match ... TODO

        		    double angle = o.Angle;
		            if (angle<=srcangle && angle>maxangle)
                    {
			            maxangle = angle;
			            found = o;
		            }
                }
            }

            // Return whatever we found (if anything)
	        return found;
        }

        /// <summary>
        /// Finds the orientation with the maximum angle in a specific search quadrant.
        /// </summary>
        /// <param name="squad">The search quadrant.</param>
        /// <param name="source">Source orientation (a reference to one of the items
        /// in the <c>orient</c> array).</param>
        /// <param name="orient">Orientation data for node.</param>
        /// <returns></returns>
        /// <remarks>
        /// If the search quadrant corresponds to the source quadrant, care must be
        /// taken to ignore the item referring to the source orientation.
        /// </remarks>
        Orientation FindMax(Quadrant squad, Orientation source, List<Orientation> orient)
        {
            // Count the number of orientations for the specified search
            // quadrant. Return if there's nothing here.
            uint nsq = CountOrient(squad, orient);
            if (nsq==0)
                return null;

            // If we only have one, and we are not in the quadrant for the
            // source orientation, return it (if we ARE in the source
            // quadrant, something is screwed up, because we previously
            // made an explicit check for this).

	        if (nsq==1)
            {
		        Debug.Assert(squad!=source.Quadrant);

                foreach(Orientation o in orient)
                {
                    if (o.Quadrant==squad)
                        return o;
		        }
	        }

            // We have more than one orientation in the search quadrant.
            // Find the one with the maximum angle.

            double maxangle = -1.0;
            Orientation found = null;

            foreach (Orientation o in orient)
            {
                if (o.Quadrant==squad && !Object.ReferenceEquals(o, source))
                {
                    double angle = o.Angle;
                    if (angle>maxangle)
                    {
                        maxangle = angle;
                        found = o;
                    }
                }
            }

            // Return whatever we found (we should have got SOMETHING).
	        Debug.Assert(found!=null);
	        return found;
        }

        /// <summary>
        /// Returns the next (anticlockwise) quadrant.
        /// </summary>
        /// <param name="current">The current quadrant</param>
        /// <returns>The next (anticlockwise) quadrant.</returns>
        Quadrant NextQuadrant(Quadrant current)
        {
            switch (current)
            {
                case Quadrant.NE:
                    return Quadrant.NW;

                case Quadrant.NW:
                    return Quadrant.SW;

                case Quadrant.SW:
                    return Quadrant.SE;

                case Quadrant.SE:
                    return Quadrant.NE;
            }

            return Quadrant.All; // error!
        }

        /// <summary>
        /// Returns the number of orientation points in a specific quadrant.
        /// </summary>
        /// <param name="squad">The quadrant to search for.</param>
        /// <param name="orient">Array of orientation info.</param>
        /// <returns></returns>
        uint CountOrient(Quadrant squad, List<Orientation> orient)
        {
            uint nsq=0;

            foreach(Orientation o in orient)
            {
                if (o.Quadrant == squad)
                    nsq++;
            }

            return nsq;
        }

        /// <summary>
        /// Determine the next arc in a polygon cycle. This does pretty well the same
        /// as one of the constructors. However, this function is intended for use
        /// when creating polygons.
        /// </summary>
        /// <param name="from">The divider we're coming from.</param>
        /// <param name="isFromStart">Are we coming from the start of the divider?</param>
        /// <returns>True if connection found.</returns>
        internal bool Create(IDivider from, bool isFromStart)
        {
            // Initialize with default values
            m_Next = null;
            m_IsStart = false;

            // Get the location involved
            ITerminal loc = (isFromStart ? from.From : from.To);

            // Get the dividers incident on the terminal
            IDivider[] divs = loc.IncidentDividers();

            // Get orientation info for each divider (the list could conceivably
            // grow if boundaries start at the terminal, then loop round to also
            // finish at the same terminal).
            List<Orientation> orient = new List<Orientation>(divs.Length);
            Orientation source = null;

            foreach (IDivider d in divs)
            {
                // Check if the start of the divider is incident on the node.
                // If so, append the orientation info to the list.
                if (d.From.IsCoincident(loc))
                {
                    Orientation o = new Orientation(d, true);
                    orient.Add(o);

                    if (d==from && isFromStart)
                    {
                        Debug.Assert(source==null);
                        source = o;
                    }
                }

                // Likewise for the end of the divider (it could start AND end at the node).
                if (d.To.IsCoincident(loc))
                {
                    Orientation o = new Orientation(d, false);
                    orient.Add(o);

                    if (d==from && !isFromStart)
                    {
                        Debug.Assert(source==null);
                        source = o;
                    }
                }
            }

            // We MUST have at least one orientation point.
            Debug.Assert(orient.Count>0);

            // If we have only one orientation point, we have a dangle,
            // so return the information we were supplied.
            if (orient.Count==1)
            {
                m_Next = from;
                m_IsStart = isFromStart;
                return true;
            }

            // Get a reference to the orientation info for the source divider.
            Debug.Assert(source!=null);

            // If we have only two orientation points, the one we want
            // if the one which does not match the source.
            if (orient.Count==2)
            {
                Orientation o = (source==orient[0] ? orient[1] : orient[0]);
                m_Next = o.Divider;
                m_IsStart = o.IsStart;
                return true;
            }

            // Handle any curves.
            if (HasAnyCurves(orient))
                source.SetCurves(orient);

            // If there are any other boundaries in the source quadrant,
            // try to find an anticlockwise divider (with the highest
            // orientation angle which is less than that of the source).
            Orientation next = FindPrevMax(source, orient);

            // At this stage, if there is more than one choice (i.e 2 or
            // more boundaries with EXACTLY the same orientation info), we want
            // to go for the one that does not already have a polygon
            // pointer on the applicable side.

            // If we got something, we're all done.
            if (next!=null)
            {
                m_Next = next.Divider;
                m_IsStart = next.IsStart;
                return true;
            }

            // Loop through the quadrants, looking for the next divider. We
            // scan the quadrants anticlockwise, including the source
            // quadrant we may have just processing (possibility that the
            // next divider is further on in the source quadrant).

            Quadrant search = NextQuadrant(source.Quadrant);
            for (int i=0; i<4; i++, search=NextQuadrant(search))
            {
                next = FindMax(search, source, orient);
                if (next!=null)
                {
                    m_Next = next.Divider;
                    m_IsStart = next.IsStart;
                    return true;
                }
            }

            throw new Exception("ConnectionFinder: network error");
        }

        /// <summary>
        /// Checks if any orientations refer to a circular arc.
        /// </summary>
        /// <param name="orient">The orientations to check.</param>
        /// <returns>True if there is at least one circular arc.</returns>
        bool HasAnyCurves(List<Orientation> orient)
        {
            foreach (Orientation o in orient)
            {
                if (o.IsDividerArc)
                    return true;
            }

            return false;
        }

        internal bool IsStart
        {
            get { return m_IsStart; }
        }

        internal IDivider Next
        {
            get { return m_Next; }
        }
    }
}
