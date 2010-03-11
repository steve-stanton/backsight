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
using Backsight.Forms;

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
        }

        #endregion

        /// <summary>
        /// Position of start of span.
        /// </summary>
        internal IPosition Start
        {
            get { return m_Start; }
        }

        /// <summary>
        /// Position of end of span.
        /// </summary>
        internal IPosition End
        {
            get { return m_End; }
        }

        /// <summary>
        /// Position of the EC.
        /// </summary>
        internal IPosition EC
        {
            get { return m_EC; }
        }

        /// <summary>
        /// True if the span has a line. 
        /// </summary>
        internal bool HasLine
        {
            get { return m_IsLine; }
            set { m_IsLine = value; }
        }

        /// <summary>
        /// True if there is a point at the end.
        /// </summary>
        internal bool HasEndPoint
        {
            get { return m_IsEndPoint; }
            set { m_IsEndPoint = value; }
        }

        /// <summary>
        /// Position of center of circle.
        /// </summary>
        internal IPosition Center
        {
            get { return m_Center; }
        }

        internal double ScaledRadius
        {
            get { return (m_Radius * m_ScaleFactor); }
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
        /// <param name="draw">The display to draw to</param>
        internal void Render(ISpatialDisplay draw)
        {
            EditingController ec = EditingController.Current;
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
    }
}
