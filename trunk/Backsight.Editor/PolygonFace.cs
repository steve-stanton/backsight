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
	/// <written by="Steve Stanton" on="03-DEC-2002" />
    /// <summary>Information about points along a boundary that forms the perimeter of polygon,
    /// for use in the automatic subdivision of polygons. This accommodates the fact that
    /// any one line may contain a secondary face that contains additional points.
    /// </summary>
    class PolygonFace
    {
        #region Class data

        /// <summary>
        /// The polygon involved.
        /// </summary>
        Polygon m_Polygon;

        /// <summary>
        /// The boundary that forms part of the polygon boundary.
        /// </summary>
        Boundary m_Boundary;

        /// <summary>
        /// The location at the start of the face. It'll be the start of the line if the
        /// polygon is to the right of the line. Otherwise it's the end of the line.
        /// </summary>
        PointGeometry m_Begin;

        /// <summary>
        /// The location at the end of the face.
        /// </summary>
        PointGeometry m_End;

        /// <summary>
        /// The extra points along this face (frequently null). If there are two or mor
        /// extra points, they will be arranged in clockwise order around the polygon.
        /// </summary>
        PointFeature[] m_ExtraPoints;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        internal PolygonFace()
        {
            m_Polygon = null;
            m_Boundary = null;
            m_Begin = null;
            m_End = null;
            m_ExtraPoints = null;
        }

        #endregion

        /// <summary>
        /// One-time definition of the topological boundary that this face corresponds to.
        /// </summary>
        /// <param name="pol">The polygon this face is part of</param>
        /// <param name="line">The topological line that acts as the primary face for
        /// some portion of the polygon boundary</param>
        /// <returns>The number of points for this face (one for the start of the face,
        /// plus any intermediate points along the face).</returns>
        internal uint SetBoundary(Polygon pol, Boundary line)
        {
            // Expect that this method should only ever get called once in the
            // lifetime of an instance
            Debug.Assert(m_ExtraPoints==null);

            // Define any extra points
            m_Polygon = pol;
            m_Boundary = line;
            SetExtraPoints();
            return (uint)(1+m_ExtraPoints.Length);
        }

        /// <summary>
        /// The topological boundary that this face corresponds to (a prior call to
        /// <c>SetBoundary</c> is required for this to return a non-null value).
        /// </summary>
        Boundary Line
        {
            get { return m_Boundary; }
        }

        /// <summary>
        /// The polygon involved
        /// </summary>
        internal Polygon Polygon
        {
            get { return m_Polygon; }
        }

        /// <summary>
        /// Defines any extra points that might coincide with this face.
        /// <c>m_Polygon</c> and <c>m_Line</c> must both be defined prior to call
        /// (via a call to <c>SetLine</c>).
        /// </summary>
        void SetExtraPoints()
        {
            Debug.Assert(m_Polygon!=null);
            Debug.Assert(m_Boundary!=null);

            // Determine whether the order needs to be reversed
            // (the points should follow a clockwise cycle round
            // the polygon)
            bool reverse = (m_Boundary.Left == m_Polygon);

            // Define the initial position
            if (reverse)
            {
                m_Begin = new PointGeometry(m_Boundary.End);
                m_End = new PointGeometry(m_Boundary.Start);
            }
            else
            {
                m_Begin = new PointGeometry(m_Boundary.Start);
                m_End = new PointGeometry(m_Boundary.End);
            }

            // Trash any old info kicking around (we shouldn't really have any)
            m_ExtraPoints = null;

            // Get any points along the boundary (excluding end points)
            throw new NotImplementedException();
            /*
	        const CeTheme& theme = GetActiveTheme();
	        CeObjectList xp;
	        pLine->GetCoincidentPoints(xp,theme,FALSE);
             */
            List<PointFeature> xp = new List<PointFeature>();
            if (xp.Count==0)
                return;

            // If we've got more than one extra point, ensure they're sorted in
            // the same direction as the line.
            if (xp.Count==1)
                m_ExtraPoints = xp.ToArray();
            else
            {
                // Sort the points so they're arranged along the boundary
                List<TaggedObject<PointFeature, double>> pts =
                    new List<TaggedObject<PointFeature, double>>(xp.Count);

                foreach (PointFeature p in xp)
                {
                    double value = m_Boundary.GetLength(p).Meters;
                    if (reverse)
                        value = -value;
                    pts.Add(new TaggedObject<PointFeature, double>(p, value));
                }

                pts.Sort(delegate(TaggedObject<PointFeature, double> a,
                                  TaggedObject<PointFeature, double> b)
                                  {
                                      return a.Tag.CompareTo(b.Tag);
                                  });

                // Retrieve the points in their sorted order, ignoring any coincident points
                // (this assumes that it doesn't matter which PointFeature we actually pick up)

                List<PointFeature> extraPoints = new List<PointFeature>(pts.Count);
                PointFeature prev = null;
                foreach (TaggedObject<PointFeature,double> tp in pts)
                {
                    PointFeature p = tp.Thing;

                    if (prev==null || !p.IsCoincident(prev))
                    {
                        extraPoints.Add(p);
                        prev = p;
                    }
                }

                // Allocate array of distinct extra positions
                m_ExtraPoints = extraPoints.ToArray();
            }
        }

        /// <summary>
        /// Defines links for this face. A prior call to <c>SetLine</c> is required.
        /// </summary>
        /// <param name="prev">The preceding face (after it has been processed via a call to
        /// <c>SetLine</c>, Must refer to the same polygon as this face.</param>
        /// <returns>The links (the number of array elements will equal the value that came
        /// back from the <c>SetLine</c> call)</returns>
        internal PolygonLink[] CreateLinks(PolygonFace prev)
        {
            Debug.Assert(m_Polygon!=null);
            Debug.Assert(m_Boundary!=null);
            Debug.Assert(prev.Line!=null);
            Debug.Assert(Object.ReferenceEquals(prev.Polygon, m_Polygon));

            // Try to obtain a point feature at the beginning of this face.
            // We need a point with a theme that is consistent with
            // that of the polygon (this could conceivably be different
            // from that of the line).

            throw new NotImplementedException();
            //ILineGeometry thisLine = null;
            PointFeature beginPoint = null;
            /*
            const CeLayerList layers(*m_pPolygon);
	        const CeLine* const pThisLine = m_pArc->GetpLine();
	        const CePoint* const pBeginPoint = m_pBegin->GetpPoint(pThisLine,&layers);
             */

            // If the polygon to the left of the this face? (if so, curve-related
            // things may need to be reversed below)
            bool isPolLeft = (m_Boundary.Left == m_Polygon);

            // Default geometric info we need to define
            bool isCurveEnd = false;
            bool isRadial = false;
            double bearing = 0.0;
            double angle = 0.0;

            // Is the position at the beginning of this face the start
            // or end of a circular arc (points between 2 curves (on
            // compound curve) are NOT considered to be curve ends). Note
            // that the line could either be a CircularArc, or a topological
            // section based on a circular arc.

            ICircularArcGeometry prevArc = (prev as ICircularArcGeometry);
            ICircularArcGeometry thisArc = (m_Boundary as ICircularArcGeometry);
            bool isPrevCurve = (prevArc!=null);
            bool isThisCurve = (thisArc!=null);
            isCurveEnd = (isPrevCurve!=isThisCurve);

            // If we are dealing with a point between two curves try to
            // set the curve parameters and, if successful, we will treat
            // the point as a radial point.

            if (isPrevCurve && isThisCurve)
            {
                isRadial = true;

                // Get the curve parameters...

                IPosition prevcen = prevArc.Circle.Center;
                double prevrad = prevArc.Circle.Radius.Meters;

                IPosition thiscen = thisArc.Circle.Center;
                double thisrad = thisArc.Circle.Radius.Meters;

                // We only need to know the sense for one of the arcs
                bool iscw = thisArc.IsClockwise;

                // If the 2 curves do not have the same centre and radius,
                // see if they are really close (to the nearest centimeter
                // on the ground). In that case, use the average values for
                // the 2 curves. If they really are different curves, don't
                // treat this as a radial curve point -- treat it as a
                // curve end instead.

                IPosition centre;	    // The centre to actually use

                if (!(prevcen==thiscen && Math.Abs(prevrad-thisrad)<Constants.TINY))
                {
                    double xc1 = prevcen.X;
                    double yc1 = prevcen.Y;
                    double xc2 = thiscen.X;
                    double yc2 = thiscen.Y;
                    double dxc = xc2-xc1;
                    double dyc = yc2-yc1;

                    if (Math.Abs(dxc)<0.01 &&
				        Math.Abs(dyc)<0.01 &&
				        Math.Abs(prevrad-thisrad)<0.01)
                    {
                        // Close enough
                        centre = new Position(xc1+dxc*0.5, yc1+dyc*0.5);
                    }
                    else
                    {
                        isRadial = false;
                        isCurveEnd = true;
                    }
                }
                else
                    centre = prevcen;

                // If the centre and radius were the same (or close enough),
                // define the radial bearing from the centre of the circle.
                // If the polygon is actually on the other side, reverse the
                // bearing so that it's directed INTO the polygon.

                if (isRadial)
                {
                    bearing = Geom.Bearing(centre, m_Begin).Radians;
                    angle = 0.0;
                    if (iscw  != isPolLeft)
                        bearing += Constants.PI;
                }
            }

            // If we're not dealing with a radial curve (or we have curves that
            // don't appear to be radial)
            if (!isRadial)
            {
                // Get the clockwise angle from the last position of the
                // preceding face to the first position after the start
                // of this face. Since info is held in a clockwise cycle
                // around the polygon, this will always give us the exterior
                // angle.
                Turn reference = new Turn(m_Begin, prev.TailReference);
                angle = reference.GetAngle(this.HeadReference).Radians;

                // Define the bearing to use for projecting the point. It's
                // in the middle of the angle, but projected into the polygon.
                bearing = reference.Bearing.Radians + angle*0.5 + Constants.PI;
            }

            // Initialize the link at the start of the face
            List<PolygonLink> links = new List<PolygonLink>();
            PolygonLink link = new PolygonLink(beginPoint, isCurveEnd, isRadial, bearing, angle);
            links.Add(link);

            // Initialize links for any extra points
            if (m_ExtraPoints!=null)
            {
                // Intermediate points can never be curve ends
                isCurveEnd = false;

                // If the face is a curve, they're radial points
                isRadial = isThisCurve;

                // Note any curve info
                double radius;
                IPosition centre;
                bool iscw;

                if (isRadial)
                {
                    Debug.Assert(m_Boundary is ICircularArcGeometry);
                    ICircularArcGeometry arc = (m_Boundary as ICircularArcGeometry);
                    centre = arc.Circle.Center;
                    radius = arc.Circle.Radius.Meters;
                    iscw = arc.IsClockwise;
                    angle = 0.0;
                }

                for (uint i=0; i<m_ExtraPoints.Length; i++)
                {
                    //IPointGeometry loc = m_ExtraPoints[i].Geometry;
                    PointFeature loc = m_ExtraPoints[i];

                    // Figure out the orientation bearing for the point
                    if (isRadial)
                    {
                        bearing = Geom.Bearing(centre, loc).Radians;
                        if (iscw  != isPolLeft)
                            bearing += Constants.PI;
                    }
                    else
                    {
                        // Get the exterior clockwise angle 

                        IPointGeometry back;
                        IPointGeometry fore;

                        if (i==0)
                            back = m_Begin;
                        else
                            back = m_ExtraPoints[i-1].PointGeometry;

                        if (i==(m_ExtraPoints.Length-1))
                            fore = m_End;
                        else
                            fore = m_ExtraPoints[i+1].PointGeometry;


                        Turn reference = new Turn(loc, back);
                        angle = reference.GetAngle(fore).Radians;

                        // Define the bearing to use for projecting the point. It's
                        // in the middle of the angle, but projected into the polygon.
                        bearing = reference.Bearing.Radians + angle*0.5 + Constants.PI;
                    }

                    link = new PolygonLink(m_ExtraPoints[i], isCurveEnd, isRadial, bearing, angle);
                    links.Add(link);
                }
            }

            return links.ToArray();
        }

        /// <summary>
        /// Returns a reference position from the end of this face. If there aren't any
        /// extra points along this face, you get the location at <c>m_Begin</c>. If there
        /// are extra points, you get the location of the last one.
        /// </summary>
        IPointGeometry TailReference
        {
            get
            {
                if (m_ExtraPoints==null)
                    return m_Begin;

                PointFeature tail = m_ExtraPoints[m_ExtraPoints.Length-1];
                return tail.PointGeometry;
            }
        }

        /// <summary>
        /// Returns a reference position from the start of this face. If there aren't any
        /// extra points along this face, you get the location at <c>m_End</c>. If there
        /// are extra points, you get the location of the first one.
        /// </summary>
        IPointGeometry HeadReference
        {
            get
            {
                if (m_ExtraPoints==null)
                    return m_End;

                PointFeature head = m_ExtraPoints[0];
                return head.PointGeometry;
            }
        }
    }
}
