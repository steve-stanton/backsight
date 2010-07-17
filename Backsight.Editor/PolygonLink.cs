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
using Backsight.Geometry;

namespace Backsight.Editor
{
	/// <written by="Steve Stanton" on="26-FEB-1998" />
    /// <summary>
    /// Used by the <c>PolygonSub</c> class to hold information about a specific point
    /// on the edge of a polygon, along with the info on another point it connects to.
    /// </summary>
    class PolygonLink
    {
        #region Class data

        /// <summary>
        /// Point on polygon perimeter (may be null for positions at corners).
        /// </summary>
        PointFeature m_Point;

        /// <summary>
        /// The linked object.
        /// </summary>
	    PolygonLink m_Link;

        /// <summary>
        /// Sequence number for the side on which the point sits (0 if the point
        /// should not be connected).
        /// </summary>
	    uint m_SideNumber;

        /// <summary>
        /// The angle between the adjacent lines.
        /// </summary>
	    double m_Angle;

        /// <summary>
        /// The bearing in the middle of the angle.
        /// </summary>
	    double m_Bearing;

        /// <summary>
        /// True if the point is at the BC or EC of a circular arc.
        /// </summary>
	    bool m_IsCurveEnd;

        /// <summary>
        /// True if this is the end of a specific link.
        /// </summary>
        bool m_IsEnd;

        /// <summary>
        /// True if this link is supposed to be a radial on a curve (in that
        /// case <c>m_Bearing</c> is perpendicular to the tangent of the curve).
        /// </summary>
        bool m_IsRadial;

        /// <summary>
        /// The angular difference between <c>m_Bearing</c>, and the bearing to
        /// <c>m_Link</c>. If no link has been set, this value will be <c>Constants.PIMUL2</c>.
        /// </summary>
	    double m_LinkAngle;

        /// <summary>
        /// The number of intersections this link has with other links.
        /// </summary>
        uint m_NumIntersect;

        #endregion

        #region Constructors

        PolygonLink()
        {
            ResetContent();
        }

        internal PolygonLink(PointFeature point, bool isCurveEnd, bool isRadial, double bearing, double angle)
        {
            ResetContent();
            Define(point, isCurveEnd, isRadial, bearing, angle);
        }

        #endregion

        /// <summary>
        /// Assigns initial values to everything
        /// </summary>
        void ResetContent()
        {
            m_Point = null;
            m_Link = null;
            m_SideNumber = 0;
            m_Angle = 0.0;
            m_Bearing = 0.0;
            m_IsCurveEnd = false;
            m_IsEnd = false;
            m_IsRadial = false;
            m_LinkAngle = Constants.PIMUL2;
            m_NumIntersect = 0;
        }

        /// <summary>
        /// Checks whether the point is at the end of a curve (possibly a compound curve).
        /// Note that if 2 compound curves meet end to end, the BC/EC will NOT be regarded
        /// as a curve end point. The test is based on whether a curve leads into something
        /// that is not a curve (<see>Create</see>).
        /// </summary>
        internal bool IsCurveEnd
        {
            get { return m_IsCurveEnd; }
        }

        /// <summary>
        /// Is this the end of a specific link?
        /// </summary>
        bool IsEnd
        {
            get { return m_IsEnd; }
            set { m_IsEnd = value; }
        }

        /// <summary>
        /// Calls <c>IsCorner</c> with the default tolerance of 20 degrees.
        /// </summary>
        /// <returns></returns>
        internal bool IsCorner()
        {
            double cornerTolerance = (20.0 * Constants.DEGTORAD);
            return IsCorner(cornerTolerance);
        }

        /// <summary>
        /// Checks whether a point is at a corner. This function assumes the stored angle has
        /// been reduced to the range [0,PI) (<see>Create</see>)
        /// </summary>
        /// <param name="tol">How close to 90 degrees (PIDIV2) does the angle need to be, in radians.
        /// </param>
        /// <returns>True if we've got a corner</returns>
        bool IsCorner(double tol)
        {
            return ( m_Angle>(Constants.PIDIV2-tol) && m_Angle<(Constants.PIDIV2+tol) );
        }

        /// <summary>
        /// The polygon side number associated with this object. This number is expected
        /// to refer to a specific side (or face) in a polygon, although it could
        ///	conceivably be used for other stuff (...not sure what I meant by that!)
        /// </summary>
        internal uint Side
        {
            get { return m_SideNumber; }
            set { m_SideNumber = value; }
        }

        /// <summary>
        /// The ideal bearing for this point.
        /// </summary>
        double Bearing
        {
            get { return m_Bearing; }
        }

        /// <summary>
        /// Is this point already linked to something?
        /// </summary>
        internal bool IsLinked
        {
            get { return (m_Link!=null); }
        }

        /// <summary>
        /// The angular difference between the desired bearing for this link point,
        /// and the direction to the object that this has been linked to. The return
        ///	value only has meaning if <c>IsLinked</c> returns true.
        /// </summary>
        internal double LinkAngle
        {
            get { return m_LinkAngle; }
            set { m_LinkAngle = value; }
        }

        /// <summary>
        /// The point this link object corresponds to.
        /// </summary>
        PointFeature Point
        {
            get { return m_Point; }
        }

        /// <summary>
        /// Is this a radial link?
        /// </summary>
        internal bool IsRadial
        {
            get { return m_IsRadial; }
        }

        /// <summary>
        /// The number of intersections this link has.
        /// </summary>
        internal uint NumIntersect
        {
            get { return m_NumIntersect; }
            set { m_NumIntersect = value; }
        }

        /// <summary>
        /// The object this is linked to.
        /// </summary>
        internal PolygonLink Link
        {
            get { return m_Link; }
            set { m_Link = value; }
        }

        /// <summary>
        /// Defines geometry for this link (and undefines everything else). For use by <c>PolygonFace</c>.
        /// </summary>
        /// <param name="point">The point that coincides with this link position (may be null at
        /// system-generated split location)</param>
        /// <param name="isCurveEnd">Does the link location coincide with the BC/EC of a circular
        /// arc? If the link is regarded as a radial position, this must be false.</param>
        /// <param name="isRadial">Does the link fall somewhere along a circular curve?</param>
        /// <param name="bearing">The bearing of the bisector of the angle formed with adjacent
        /// link objects</param>
        /// <param name="angle">The clockwise angle formed with adjacent link positions (irrelevant
        /// if the link is a radial).</param>
        internal void Define(PointFeature point, bool isCurveEnd, bool isRadial, double bearing, double angle)
        {
	        // Ensure everything is in it's default state
	        ResetContent();

	        // Assign the supplied values
	        m_Point = point;
	        m_IsCurveEnd = isCurveEnd;
	        m_IsRadial = isRadial;

	        // If we're dealing with a radial, the angle is n/a. Otherwise
	        // pick up a value that's less than 180 degrees (this simplifies
	        // the test to see if we're dealing with a corner)
	        if ( m_IsRadial )
		        m_Angle = 0.0;
	        else
	        {
		        m_Angle = angle;
		        if ( m_Angle > Constants.PI )
                    m_Angle -= Constants.PI;
	        }

            // Important that the bearing is in range [0,PIMUL2), for use with Turn.GetAngle
	        m_Bearing = bearing;
	        if ( m_Bearing >= Constants.PIMUL2 )
                m_Bearing -= Constants.PIMUL2;
        }

        /// <summary>
        /// Returns any link information.
        /// </summary>
        /// <param name="from">The point where the link starts.</param>
        /// <param name="to">The point where the link ends.</param>
        /// <returns>True if a link has been returned. False if no link (in that case,
        /// the 2 return parameters come back as nulls).</returns>
        internal bool GetLink(out PointFeature from, out PointFeature to)
        {
	        if ( m_Link!=null && !m_IsEnd )
            {
		        from = m_Point;
		        to = m_Link.Point;
		        return true;
	        }
	        else
            {
		        from = to = null;
		        return false;
	        }
        }

        internal void SetLink(PolygonLink link)
        {
            SetLink(link, Constants.PIMUL2, Constants.PIMUL2);
        }

        /// <summary>
        /// Refers this object to another one. Once done, the <c>Link</c> function will
        /// return the link when called for THIS object (but not the other object).
        /// </summary>
        /// <param name="link">The object to link to. Must not point to this object. Note that
        /// if the object to link to already has a link, that link will be severed; you can
        /// only link to ONE other object.</param>
        /// <param name="thisdiff">The value to assign to <c>this.LinkAngle</c></param>
        /// <param name="othrdiff">The value to assign to <c>link.LinkAngle</c></param>
        internal void SetLink(PolygonLink link, double thisdiff, double othrdiff)
        {
            // CAN'T point to itself.
            if (Object.ReferenceEquals(link,this))
                throw new ArgumentException("PolygonLink.SetLink: Attempt to form null connection");

            // The link shouldn't have been made already.
            if (link!=null && (Object.ReferenceEquals(m_Link,link) || Object.ReferenceEquals(link.Link,this)))
                throw new ArgumentException("PolygonLink.SetLink: Attempt to re-form link");

            // If this object already has a link, update the object that pointed back.
            if (m_Link!=null)
            {
                m_Link.Link = null;
                m_Link.IsEnd = false;
                m_Link.LinkAngle = Constants.PIMUL2;
            }

            // If the object we are linking to has a link, break it too.
            if (link!=null && link.Link!=null)
                link.SetLink(null);

            // Define the link.
            m_Link = link;
            m_IsEnd = false;
            m_LinkAngle = thisdiff;

            // If the link is real, point the other object back, marking it as a back pointer.

            if (link!=null)
            {
                link.Link = this;
                link.IsEnd = true;
                link.LinkAngle = othrdiff;
            }
        }

        /// <summary>
        /// Returns the distance (squared) between 2 link points.
        /// </summary>
        /// <param name="other">The other link to get the distance to.</param>
        /// <returns></returns>
        internal double DistanceSquared(PolygonLink other)
        {
            // 22-MAR-00 There may be no point at a corner.

            if (m_Point==null || other.Point==null)
                return 0.0;
            else
                return Geom.DistanceSquared(m_Point, other.Point);
        }

        /// <summary>
        /// Returns the angle between the ideal bearing for this link, and the
        /// position of another link point. The caller can test the angle against
        /// an angular tolerance to see if a link is suitable.
        /// 
        /// Note that this function only makes a one-way test, making use only of
        /// the POSITION of the other link point. The caller may also want to do the
        /// reverse test as well, to see if the link is within tolerance from the
        /// perspective of both link points.
        /// </summary>
        /// <param name="other">The other link point.</param>
        /// <returns>The angle between this link point's reference bearing, and a
        /// line radiating to the other link point. The angle will always be less
        /// (or equal to) PI radians.</returns>
        internal double GetAngle(PolygonLink other)
        {
            // 22-MAR-00 There may be no point at a corner.
            if (m_Point==null || other.Point==null)
                return Constants.PIDIV2;

            // Treat the direction to the other link point as the reference direction.
            Turn reference = new Turn(this.Point, other.Point);

            // Get the clockwise angle formed by the reference direction and the
            // ideal bearing for this link point.
            double angle = reference.GetAngleInRadians(m_Bearing);

            // Force the angle to be less than 180 degrees.
	        if (angle > Constants.PI)
            {
		        angle = Constants.PIMUL2 - angle;
		        if (angle < Constants.TINY)
                    angle = 0.0;
	        }
	
	        return angle;
        }

        /// <summary>
        /// Confirms that a potential link is valid. For a link to be valid, the
        /// midpoint of the link must fall inside the specified polygon, and the
        /// line connecting the 2 points cannot intersect anything.
        /// </summary>
        /// <param name="other">The end of the proposed point to link to.</param>
        /// <param name="pol">The polygon the link must fall completely inside.</param>
        /// <returns>True if link is valid.</returns>
        internal bool IsLinkValid(PolygonLink other, Polygon pol)
        {
            // 22-MAR-00 There may be no point at a corner.
            if (m_Point==null || other.Point==null)
                return false;

            // Get the midpoint of the link.
            IPosition mid = Geom.MidPoint(m_Point, other.Point);

            // Confirm the midpoint falls inside the right polygon.
        	if (!pol.IsRingEnclosing(mid))
                return false;

            // Locate any intersections formed by the proposed link (considering
	        // only the currently active theme).
            LineGeometry seg = new SegmentGeometry(m_Point, other.m_Point);
            IntersectionFinder xsect = new IntersectionFinder(seg, true);

            // The proposed link is not valid if it intersects anything
            // along the length of the segment, or has any grazes.
	        if (xsect.IsGrazing || xsect.IsSplitNeeded)
                return false;

            return true;
        }

        /// <summary>
        /// The position (if any) for the start of this link object.
        /// </summary>
        IPosition From
        {
            get { return m_Point; }
        }

        /// <summary>
        /// Checks whether this link object intersects another one.
        /// </summary>
        /// <param name="other">The other link to check.</param>
        /// <returns>
        /// True if there is an intersection. False if:
        ///
        /// 1. The other link is THIS link.
        /// 2. This link is the end of a link.
        /// 3. This link has no link.
        /// 4. The other link is the end of any link.
        /// 5. The other link is not linked.
        /// 6. There is no intersection.
        /// </returns>
        internal bool IsIntersected(PolygonLink other)
        {
            // The other object can't be THIS object.
	        if (Object.ReferenceEquals(other, this))
                return false;

            // The other can't be linked to this.
	        if (Object.ReferenceEquals(other, m_Link))
                return false;

            // Both objects must be linked.
	        if (m_Link==null || other.Link==null)
                return false;

            // It must be the START of both links.
	        if (m_IsEnd || other.IsEnd)
                return false;
    
            // Get the position of this link.
	        double xk = m_Point.X;
	        double yk = m_Point.Y;
	        double xl = m_Link.Point.X;
	        double yl = m_Link.Point.Y;

            // Get the position of the other link.
	        double xm = other.Point.X;
	        double ym = other.Point.Y;
	        double xn = other.Link.Point.X;
	        double yn = other.Link.Point.Y;

            // Return if there is any intersection along the length of the test segments.
            double xi, yi;
            return (Geom.CalcIntersect(xk, yk, xl, yl,
                                       xm, ym, xn, yn,
                                       out xi, out yi, true) != 0);
        }
    }
}
