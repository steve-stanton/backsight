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

using Backsight.Geometry;
using Backsight.Editor.Operations;

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
        StraightLeg m_Leg;

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
        StraightSpan(StraightLeg leg, IPosition start, double bearing, double sfac)
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

        internal IPosition End
        {
            get { return m_End; }
        }

        /// <summary>
        /// Gets info for a specific span on a leg.
        /// </summary>
        /// <param name="index">Index of the span to get.</param>
        void Get(int index)
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
        void Draw()
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
                        throw new Exception("StraightSpan.Save - Mismatched line");

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
                            line.StartPoint.Move(sloc);
                            line.EndPoint.Move(eloc);
                        }
                    }
                }
                else if (m_IsEndPoint) // Feature should be a point
                {
                    PointFeature point = (old as PointFeature);
                    if (point==null)
                        throw new Exception("StraightSpan.Save - Mismatched point");

                    if (!point.IsCoincident(veryEnd))
                        point.Move(eloc);
                }
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
                    feat = map.AddLine(ps, pe, map.DefaultLineType, op);
                }
            }

            return feat;
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

            // Add a line.
            m_IsLine = true;
            return map.AddLine(pS, pE, map.DefaultLineType, creator);
        }
    }
}
