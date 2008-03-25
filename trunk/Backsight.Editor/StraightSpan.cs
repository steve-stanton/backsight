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
using System.Diagnostics;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="08-FEB-1998" was="CeStraightSpan" />
    /// <summary>
    /// A single span in a straight leg.
    /// </summary>
    class StraightSpan
    {
        #region Class data

        /// <summary>
        /// The leg this span relates to.
        /// </summary>
        IStraightLeg m_Leg;

        /// <summary>
        /// Position of start of leg.
        /// </summary>
        double m_LegStartN;

        /// <summary>
        /// Position of start of leg.
        /// </summary>
        double m_LegStartE;

        /// <summary>
        /// The sin(bearing) of the leg.
        /// </summary>
        double m_SinBearing;

        /// <summary>
        /// The cos(bearing) of the leg.
        /// </summary>
        double m_CosBearing;

        /// <summary>
        /// The scale factor to apply to distances on the leg.
        /// </summary>
        double m_ScaleFactor;

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

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>StraightSpan</c>
        /// </summary>
        /// <param name="leg">The leg that this span falls on.</param>
        /// <param name="start">The position of the start of the leg.</param>
        /// <param name="bearing">The bearing of the leg.</param>
        /// <param name="sfac">The scale factor to apply to distances on the leg.</param>
        internal StraightSpan(IStraightLeg leg, IPosition start, double bearing, double sfac)
        {
            // Hold on to the supplied stuff.
            m_Leg = leg;
            m_LegStartN = start.Y;
            m_LegStartE = start.X;
            m_SinBearing = Math.Sin(bearing);
            m_CosBearing = Math.Cos(bearing);
            m_ScaleFactor = sfac;

            // Initialize values that will be defined via calls to Get().
            m_Index = -1;
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
        /// Gets info for a specific span on a leg.
        /// </summary>
        /// <param name="index">Index of the span to get.</param>
        internal void Get(int index)
        {
            // Ask the leg to return the distance to the start and the
            // end of the requested span.
            double sdist, edist;
            m_Leg.GetDistances(index, out sdist, out edist);

            // See if the span has a line and a terminal point.
            m_IsLine = m_Leg.HasLine(index);
            m_IsEndPoint = m_Leg.HasEndPoint(index);

            // Define the start position.
            if (index==0)
                m_Start = new Position(m_LegStartE, m_LegStartN);
            else
            {
                sdist *= m_ScaleFactor;
                m_Start = new Position(m_LegStartE + (sdist*m_SinBearing),
                                       m_LegStartN + (sdist*m_CosBearing));
            }

            // Define the end position.
            edist *= m_ScaleFactor;
            m_End = new Position(m_LegStartE + (edist*m_SinBearing),
                                 m_LegStartN + (edist*m_CosBearing));

            //	Remember the requested index
            m_Index = index;
        }

        /// <summary>
        /// Draws this span (if visible).
        /// </summary>
        internal void Draw()
        {
            EditingController ec = EditingController.Current;
            ISpatialDisplay draw = ec.ActiveDisplay;
            IDrawStyle style = ec.Style(Color.Magenta);
            IPosition[] line = new IPosition[] { m_Start, m_End };

            if (m_IsLine)
                style.Render(draw, line);
            else
                new DottedStyle(Color.Magenta).Render(draw, line);

            //	Draw terminal point if it exists.
            if (m_IsEndPoint)
                style.Render(draw, m_End);
        }
    }
}
