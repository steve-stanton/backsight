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
using System.Drawing;

using Backsight.Geometry;
using Backsight.Editor.Operations;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="08-FEB-1998" was="CeCircularSpan" />
    /// <summary>
    /// A single span in a circular leg.
    /// </summary>
    class CircularSpan
    {
        #region Class data

        // Supplied to constructor ...

        /// <summary>
        /// The leg this span relates to.
        /// </summary>
        readonly CircularLeg m_Leg;

        /// <summary>
        /// The scale factor to apply to distances on the leg.
        /// </summary>
        readonly double m_ScaleFactor;

        /// <summary>
        /// Position of the BC.
        /// </summary>
        readonly IPosition m_BC;

        /// <summary>
        /// Bearing at the BC.
        /// </summary>
        readonly double m_EntryBearing;

        // Derived / copied from the leg ...

        /// <summary>
        /// Position of center of circle.
        /// </summary>
        readonly IPosition m_Center;

        /// <summary>
        /// Bearing from center to BC.
        /// </summary>
        readonly double m_BearingToBC;

        /// <summary>
        /// Position of the EC.
        /// </summary>
        readonly IPosition m_EC;

        /// <summary>
        /// The exit bearing.
        /// </summary>
        readonly double m_ExitBearing;

        /// <summary>
        /// True if leg is clockwise.
        /// </summary>
        readonly bool m_IsClockwise;

        /// <summary>
        /// Radius of the leg (unscaled, in meters).
        /// </summary>
        readonly double m_Radius;

        /// <summary>
        /// Scaling factor for distances in cul-de-sacs (ratio of the length calculated from
        /// the CA & Radius, versus the observed distances that were actually specified). For
        /// curves that are not cul-de-sacs, this will be 1.0
        /// </summary>
        readonly double m_CulFactor;

        // Defined by a call to Get() ...

        /// <summary>
        /// Index of currently defined span (-1 if span is not defined).
        /// </summary>
        int m_Index;

        /// <summary>
        /// Position of start of span.
        /// </summary>
        IPosition m_Start;

        /// <summary>
        /// Position of end of span.
        /// </summary>
        IPosition m_End;

        /// <summary>
        /// True if the span has a line. 
        /// </summary>
        bool m_IsLine;

        /// <summary>
        /// True if there is a point at the end.
        /// </summary>
        bool m_IsEndPoint;

        // After a circle has been defined ...

        /// <summary>
        /// Persistent circle for this span.
        /// </summary>
        Circle m_Circle;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>CircularSpan</c>
        /// </summary>
        /// <param name="leg">The leg this span relates to.</param>
        /// <param name="bc">Position of the BC.</param>
        /// <param name="bcbearing">Bearing at the BC.</param>
        /// <param name="sfac">The scale factor to apply to distances on the leg.</param>
        internal CircularSpan(CircularLeg leg, IPosition bc, double bcbearing, double sfac)
        {
            // Hold on to the supplied stuff.
            m_Leg = leg;
            m_BC = bc;
            m_ScaleFactor = sfac;
            m_EntryBearing = bcbearing;

            // Use supplied stuff to derive info on the centre and EC.
            leg.GetPositions(bc, bcbearing, sfac, out m_Center, out m_BearingToBC, out m_EC, out m_ExitBearing);

            // Remember which way the leg goes
            m_IsClockwise = leg.IsClockwise;

            // What's the (unscaled) radius?
            m_Radius = leg.Radius;

            //	Initialize cul-de-sac scaling factor.
            m_CulFactor = 1.0;

            // If we are dealing with a cul-de-sac that has at least one
            // observed span, define an additional scaling factor = C/D,
            // where C is the length calculated from the central angle &
            // the (scaled) radius, and D is the total of the observed spans.

            if (leg.IsCulDeSac)
            {
                double obsv = leg.GetTotal();
                if (obsv > MathConstants.TINY)
                    m_CulFactor = leg.Length.Meters / obsv;
            }

            // Initialize values that will be defined via calls to Get().

            m_Index = -1;
            m_Start = null;
            m_End = null;
            m_IsLine = false;
            m_IsEndPoint = false;

            if (leg==null)
                m_Circle = null;
            else
                m_Circle = leg.Circle;
        }

        #endregion

        /// <summary>
        /// Position of the EC.
        /// </summary>
        internal IPosition EC
        {
            get { return m_EC; }
        }

        /// <summary>
        /// Position of center of circle.
        /// </summary>
        IPosition Center
        {
            get { return m_Center; }
        }

        /// <summary>
        /// The exit bearing.
        /// </summary>
        internal double ExitBearing
        {
            get { return m_ExitBearing; }
        }

        /// <summary>
        /// Gets info for a specific span on a leg.
        /// </summary>
        /// <param name="index">Index of the span to get.</param>
        internal void Get(int index)
        {
            // If we are dealing with a cul-de-sac that has no observed spans,
            // just return the complete curve. It MUST be visible. Note that
            // only legs that are cul-de-sacs can have no spans.
            if (m_Leg.Count==0)
            {
                m_Start = m_BC;
                m_End = m_EC;
                m_IsLine = true;
                m_IsEndPoint = true;
                return;
            }

            // Ask the leg to return the distance to the start and the
            // end of the requested span.
            double sdist, edist;
            m_Leg.GetDistances(index, out sdist, out edist);

            // See if the span has a line and a terminal point.
            m_IsLine = m_Leg.HasLine(index);
            m_IsEndPoint = m_Leg.HasEndPoint(index);

            // Define the start position (easy if it's the 1st span).
            if (index==0)
                m_Start = m_BC;
            else
                m_Start = GetPoint(sdist);

            // Define the end position.
            m_End = GetPoint(edist);

            // Remember the requested index
            m_Index = index;
        }

        /// <summary>
        /// Gets the position of a point on the circular leg.
        /// </summary>
        /// <param name="dist">The (unscaled) distance to the desired point.</param>
        /// <returns>The position.</returns>
        IPosition GetPoint(double dist)
        {
            // Get the angle subtended at the center of the circle. We use
            // unscaled values here, since the scale factors would cancel out.
            // However, we DO apply any cul-de-sac scaling factor, to account
            // for the fact that the distance may be inconsistent with the
            // curve length derived from the CA and radius. For example, it
            // is possible that the calculated curve length=200, although the
            // total of the observed spans is somehow only 100. In that case,
            // if the supplied distance is 50, we actually want to use a
            // value of 50 * (200/100) = 100.

            double angle = (dist*m_CulFactor)/m_Radius;

            // Get the bearing of the point with respect to the center of the circle.

            double bearing;

            if (m_IsClockwise)
                bearing = m_BearingToBC + angle;
            else
                bearing = m_BearingToBC - angle;

            // Calculate the position using the scaled radius.
            return Geom.Polar(m_Center, bearing, m_Radius*m_ScaleFactor);
        }

        /// <summary>
        /// Draws this span (if visible).
        /// </summary>
        internal void Draw()
        {
            EditingController ec = EditingController.Current;
            ISpatialDisplay draw = ec.ActiveDisplay;
            IDrawStyle style = ec.Style(Color.Magenta);
            IPointGeometry center = PointGeometry.Create(m_Center);
            ICircularArcGeometry arc = new CircularArcGeometry(center, m_Start, m_End, m_IsClockwise);

            if (m_IsLine)
                style.Render(draw, arc);
            else
                new DottedStyle(Color.Magenta).Render(draw, arc);

            //	Draw terminal point if it exists.
            if (m_IsEndPoint)
                style.Render(draw, m_End);
        }

        /// <summary>
        /// Adds a circle to the map, suitable for this span. Called by <see cref="CircularLeg.Save"/>.
        /// </summary>
        /// <param name="creator">The edit operation containing the leg</param>
        /// <returns>The circle for this span (may be a circle that previously existed)</returns>
        internal Circle AddCircle(Operation creator)
        {
            // If a circle was previously created, just return that.
            if (m_Circle!=null)
                return m_Circle;

            // Add the circle (checks if it's already there).
            // This will cross-reference the center point to the circle.
            CadastralMapModel map = CadastralMapModel.Current;
            PointFeature centerPoint = map.EnsurePointExists(m_Center, creator);
            m_Circle = map.AddCircle(centerPoint, new Length(m_Radius*m_ScaleFactor));
            return m_Circle;
        }

        /// <summary>
        /// Saves this span in the map.
        /// </summary>
        /// <param name="op">The editing operation this span is part of</param>
        /// <param name="insert">Reference to a new point that was inserted just before
        /// this span. Defined only during rollforward.</param>
        /// <param name="old">Pointer to the feature that was previously associated with
        /// this span. This will be not null when the span is being saved as part of
        /// rollforward processing.</param>
        /// <param name="veryEnd">The location at the very end of the connection path
        /// that this span is part of.</param>
        /// <returns>The feature (if any) that represents the span. If the span has a line,
        /// this will be a <see cref="LineFeature"/>. If the span has no line, it may be
        /// a <see cref="PointFeature"/> at the END of the span. A null is also valid,
        /// meaning that there is no line & no terminal point.</returns>
        Feature Save(Operation op, PointFeature insert, Feature old, PointFeature veryEnd)
        {
            // The circle on which this span is based should already be defined
            // (see the call that CircularLeg.Save makes to this.AddCircle).
            if (m_Circle==null)
                throw new Exception("CircularSpan.Save -- Circle has not been defined");

            // Get map info.
            CadastralMapModel map = CadastralMapModel.Current;

            // Reference to the created feature (if any).
            Feature feat = null;

            // Make sure the start and end points have been rounded to
            // the internal resolution.
            IPointGeometry sloc = PointGeometry.Create(m_Start);
            IPointGeometry eloc = PointGeometry.Create(m_End);

            // If the span was previously associated with a feature, just
            // move it. If the feature is a line, we want to move the
            // location at the end (except in a case where a new line
            // has just been inserted prior to it, in which case we
            // need to change the start location so that it matches
            // the end of the new guy).

            if (old!=null)
            {
                if (m_IsLine) // Feature should therefore be a line
                {
                    LineFeature line = (old as LineFeature);
                    if (line==null)
                        throw new Exception("CircularSpan.Save - Mismatched line");

                    if (insert!=null)
                    {
                        line.ChangeEnds(insert, line.EndPoint);
                        if (!line.EndPoint.IsCoincident(veryEnd))
                            line.EndPoint.Move(eloc);
                    }
                    else
                    {
                        if (line.EndPoint.IsCoincident(veryEnd))
                            line.StartPoint.Move(sloc);
                        else
                        {
                            throw new NotImplementedException("CircularSpan.Save");
                            //pArc->Move(sloc, eloc, m_pCircle, m_IsClockwise);
                            //line.StartPoint.Move(sloc, m_Circle, m_IsClockwise);
                            //line.EndPoint.Move(eloc, m_Circle, m_IsClockwise);
                        }
                    }
                }
                else if (m_IsEndPoint) // Feature should be a point
                {
                    PointFeature point = (old as PointFeature);
                    if (point==null)
                        throw new Exception("CircularSpan.Save - Mismatched point");

                    if (!point.IsCoincident(veryEnd))
                        point.Move(eloc);
                }

                feat = old; // SS:20080308 - Not sure if this is correct (it's not in the comparable block of StraightSpan.cs)
            }
            else
            {
                // If we have an end point, add it. If it creates something
                // new, assign an ID to it.
                if (m_IsEndPoint)
                {
                    feat = map.EnsurePointExists(eloc, op);
                    if (Object.ReferenceEquals(feat.Creator, op) && feat.Id==null)
                        feat.SetNextId();
                }

                // Add a line if we have one.
                if (m_IsLine)
                {
                    PointFeature ps = map.EnsurePointExists(sloc, op);
                    PointFeature pe = map.EnsurePointExists(eloc, op);
                    feat = map.AddCircularArc(m_Circle, ps, pe, m_IsClockwise, map.DefaultLineType, op);
                }
            }

            return feat;
        }

        /// <summary>
        /// Records the circle for this span. This also ensures that the circle has the
        /// correct radius. However, it does NOT alter the circle's center position, since
        /// that is dependent on higher level stuff (see <see cref="CircularLeg.Save"/>).
        /// </summary>
        /// <param name="circle">The circle for this span (may be null).</param>
        void SetCircle(Circle circle)
        {
            // Remember the circle.
            m_Circle = circle;

            // Ensure the radius is correct.
            if (m_Circle!=null)
                m_Circle.Radius = new Length(m_Radius*m_ScaleFactor);
        }

        /// <summary>
        /// Saves a newly inserted span.
        /// </summary>
        /// <param name="index">The index of the new span.</param>
        /// <param name="creator">The operation that the new span should be referred to.</param>
        /// <param name="isLast">Is the new span going to be the very last span in the last
        /// leg of a connection path?</param>
        /// <returns>The line that was created.</returns>
        LineFeature SaveInsert(int index, PathOperation creator, bool isLast)
        {
            // SS:20080314 - Most of what follows is identical to the corresponding method
            // in StraightSpan. The only difference is right at the end, where the line
            // gets created...

            // Get the end positions for the new span.
            Get(index);

            // Make sure the start and end points have been rounded to
            // the internal resolution.
            IPointGeometry sloc = PointGeometry.Create(m_Start);
            IPointGeometry eloc = PointGeometry.Create(m_End);

            // Get the location at the start of the span (in most cases,
            // it should be there already -- the only exception is a
            // case where the point was omitted).
            CadastralMapModel map = CadastralMapModel.Current;
            PointFeature pS = map.EnsurePointExists(sloc, creator);

            // If the insert is going to be the very last span in the
            // enclosing connection path, just pick up the terminal
            // location of the path.
            PointFeature pE = null;

            if (isLast)
            {
                // Pick up the end of the path.
                pE = creator.EndPoint;

                // And ensure there has been no roundoff in the end position.
                eloc = pE;
            }
            else
            {
                // Add a point at the end of the span. Do NOT attempt to re-use any existing
                // point that happens to fall there. If you did, we could be re-using a location
                // that comes later in the connection path (i.e. it may later be moved again!).
                pE = map.AddPoint(eloc, map.DefaultPointType, creator);
                m_IsEndPoint = true;

                // Assign the next available ID to the point
                pE.SetNextId();
            }

            // Add a line (this will cross-reference the points and the circle to the new line).
            m_IsLine = true;
            return map.AddCircularArc(m_Circle, pS, pE, m_IsClockwise, map.DefaultLineType, creator);
        }
    }
}
