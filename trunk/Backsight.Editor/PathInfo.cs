// <remarks>
// Copyright 2008 - Steve Stanton. This file is part of Backsight
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
using System.Drawing;

using Backsight.Editor.Operations;
using Backsight.Forms;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="26-MAR-2008"/>
    /// <summary>
    /// Information about a connection path. This acts as a helper for the <see cref="PathForm"/> dialog.
    /// It's sort of a half-way between the fairly unstructured world of the dialog class, and the
    /// regimented world of the operation class.
    /// </summary>
    class PathInfo
    {
        #region Class data

        /// <summary>
        /// The point where the path starts.
        /// </summary>
        readonly PointFeature m_From;

        /// <summary>
        /// The point where the path ends.
        /// </summary>
        readonly PointFeature m_To;

        /// <summary>
        /// The legs that make up the path
        /// </summary>
        readonly Leg[] m_Legs;

        /// <summary>
        /// Has the <see cref="Adjust"/> method been successfully called. If true, the values
        /// for <see cref="m_Rotation"/> and <see cref="m_ScaleFactor"/> are meaningful.
        /// </summary>
        bool m_IsAdjusted;

        /// <summary>
        /// Rotation for path (in radians)
        /// </summary>
        double m_Rotation;

        /// <summary>
        /// Scaling to apply to path distances
        /// </summary>
        double m_ScaleFactor;

        /// <summary>
        /// The precision denominator (0 for a perfect match).
        /// </summary>
        double m_Precision;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>PathInfo</c> object
        /// </summary>
        /// <param name="from">The point where the path starts.</param>
        /// <param name="to">The point where the path ends.</param>
        internal PathInfo(PointFeature from, PointFeature to, Leg[] legs)
        {
            m_From = from;
            m_To = to;
            m_Legs = legs;

            m_IsAdjusted = false;
            m_Rotation = 0.0;
            m_ScaleFactor = 0.0;
        }

        /// <summary>
        /// Create a new <c>PathInfo</c> object that corresponds to a previously
        /// saved connection path. For consistency with the other constructor, this
        /// does not attempt to adjust the path (the Rotation and ScaleFactory properties
        /// will retain zero values unless a call is made to Adjust).
        /// </summary>
        /// <param name="pop">The saved connection path</param>
        internal PathInfo(PathOperation pop)
        {
            m_From = pop.StartPoint;
            m_To = pop.EndPoint;
            m_Legs = pop.GetLegs();

            m_IsAdjusted = false;
            m_Rotation = 0.0;
            m_ScaleFactor = 0.0;
            m_Precision = 0.0;
        }

        #endregion

        /// <summary>
        /// The point where the path starts.
        /// </summary>
        internal PointFeature FromPoint
        {
            get { return m_From; }
        }

        /// <summary>
        /// The point where the path ends.
        /// </summary>
        internal PointFeature ToPoint
        {
            get { return m_To; }
        }

        /// <summary>
        /// Draws the path on the specified display
        /// </summary>
        /// <param name="display">The display to draw to</param>
        internal void Render(ISpatialDisplay display)
        {
            EnsureAdjusted();

            // Do nothing if the scale factor is undefined.
            if (Math.Abs(m_ScaleFactor) < MathConstants.TINY)
                return;

            // Initialize position to the start of the path.
            IPosition gotend = new Position(m_From);

            // Initial bearing is whatever the rotation is.
            double bearing = m_Rotation;

            for (int i = 0; i < m_Legs.Length; i++)
            {
                Leg leg = m_Legs[i];

                // Include any angle specified at the start of the leg
                StraightLeg sLeg = (leg as StraightLeg);
                if (sLeg != null)
                    bearing = sLeg.AddStartAngle(bearing);

                // Determine exit bearing for circular leg (do it now, in case an extra leg complicates matters below)
                double exitBearing = bearing;
                CircularLeg cLeg = (leg as CircularLeg);
                if (cLeg != null)
                    exitBearing = cLeg.GetExitBearing(gotend, bearing, m_ScaleFactor);

                // Obtain geometry for each span and draw
                SpanInfo[] spans = leg.PrimaryFace.Spans;
                ILineGeometry[] sections = leg.GetSpanSections(gotend, bearing, m_ScaleFactor, spans);
                DrawSpans(display, spans, sections);

                // If we're dealing with the first face of a staggered leg, process the second face
                if (leg.AlternateFace != null)
                {
                    spans = leg.AlternateFace.Spans;
                    sections = leg.GetSpanSections(gotend, bearing, m_ScaleFactor, spans);
                    DrawSpans(display, spans, sections);
                }

                // Get to the end of the leg
                gotend = sections[sections.Length - 1].End;
                bearing = exitBearing;
            }

            // Re-draw the terminal points to ensure that their color is on top.
            DrawEnds(display);
        }

        /// <summary>
        /// Renders the geometry for the spans along a leg.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="spans">Information about each observed span</param>
        /// <param name="sections">The geometry that corresponds to each span</param>
        void DrawSpans(ISpatialDisplay display, SpanInfo[] spans, ILineGeometry[] sections)
        {
            Debug.Assert(spans.Length == sections.Length);
            IDrawStyle solidStyle = EditingController.Current.Style(Color.Magenta);
            IDrawStyle dottedStyle = new DottedStyle(Color.Magenta);

            for (int i = 0; i < spans.Length; i++)
            {
                ILineGeometry geom = sections[i];
                IDrawStyle style = (spans[i].HasLine ? solidStyle : dottedStyle);

                if (geom is IClockwiseCircularArcGeometry)
                    style.Render(display, (IClockwiseCircularArcGeometry)geom);
                else
                    style.Render(display, new IPosition[] { geom.Start, geom.End });

                if (spans[i].HasEndPoint)
                    solidStyle.Render(display, geom.End);
            }
        }

        /// <summary>
        /// Draws the end points for this path.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        void DrawEnds(ISpatialDisplay display)
        {
            if (m_From!=null)
                m_From.Draw(display, Color.DarkBlue);

            if (m_To!=null)
                m_To.Draw(display, Color.Cyan);
        }

        /// <summary>
        /// Ensures the Adjust method has been called.
        /// </summary>
        void EnsureAdjusted()
        {
            if (!m_IsAdjusted)
            {
                double de;				// Misclosure in eastings
                double dn;				// Misclosure in northings
                double prec;			// Precision
                double length;			// Total observed length
                double rotation;		// Rotation for adjustment
                double sfac;			// Adjustment scaling factor

                Adjust(out dn, out de, out prec, out length, out rotation, out sfac);
            }
        }

        /// <summary>
        /// Adjusts the path (Helmert adjustment).
        /// </summary>
        /// <param name="dN">Misclosure in northing.</param>
        /// <param name="dE">Misclosure in easting.</param>
        /// <param name="precision">Precision denominator (zero if no adjustment needed).</param>
        /// <param name="length">Total observed length.</param>
        /// <param name="rotation">The clockwise rotation to apply (in radians).</param>
        /// <param name="sfac">The scaling factor to apply.</param>
        void Adjust(out double dN, out double dE, out double precision, out double length,
                    out double rotation, out double sfac)
        {
            dN = dE = precision = length = rotation = 0.0;
            sfac = 1.0;

            // Initialize position to the start of the path, corresponding to the initial
            // un-adjusted end point.
            IPosition start = m_From;
            IPosition gotend = new Position(m_From);

            // Initial bearing is due north.
            double bearing = 0.0;

            // Go through each leg, updating the end position, and getting
            // the total path length.
            foreach (Leg leg in m_Legs)
            {
                length += leg.Length.Meters;
                leg.Project(ref gotend, ref bearing, sfac);
            }

            // Get the bearing and distance of the end point we ended up with.
            double gotbear = Geom.BearingInRadians(m_From, gotend);
            double gotdist = Geom.Distance(m_From, gotend);

            // Get the bearing and distance we want.
            double wantbear = Geom.BearingInRadians(m_From, m_To);
            double wantdist = Geom.Distance(m_From, m_To);

            // Figure out the rotation.
            rotation = wantbear-gotbear;

            // Rotate the end point we got.
            gotend = Geom.Rotate(m_From, gotend, new RadianValue(rotation));

            // Calculate the line scale factor.
            double linefac = m_From.MapModel.SpatialSystem.GetLineScaleFactor(m_From, gotend);

            // Figure out where the rotated end point ends up when we apply the line scale factor.
            gotend = Geom.Polar(m_From, wantbear, gotdist*linefac);

            // What misclosure do we have?
            dN = gotend.Y - m_To.Y;
            dE = gotend.X - m_To.X;
            double delta = Math.Sqrt(dN*dN + dE*dE);

            // What's the precision denominator (use a value of 0 to denote an exact match).
            if (delta > MathConstants.TINY)
                precision = wantdist/delta;
            else
                precision = 0.0;

            // Figure out the scale factor for the adjustment (use a value of 0 if the start and end
            // points are coincident). The distances here have NOT been adjusted for the line scale factor.
            if (gotdist > MathConstants.TINY)
                sfac = wantdist/gotdist;
            else
                sfac = 0.0;

            // Remember the rotation and scaling factor
            m_IsAdjusted = true;
            m_Rotation = rotation;
            m_ScaleFactor = sfac;
            m_Precision = precision;
        }

        /// <summary>
        /// Rotation for path (in radians)
        /// </summary>
        internal double RotationInRadians
        {
            get
            {
                EnsureAdjusted();
                return m_Rotation;
            }
        }

        /// <summary>
        /// Scaling to apply to path distances
        /// </summary>
        internal double ScaleFactor
        {
            get
            {
                EnsureAdjusted();
                return m_ScaleFactor;
            }
        }

        /// <summary>
        /// The precision denominator (0 for a perfect match).
        /// </summary>
        internal double Precision
        {
            get
            {
                EnsureAdjusted();
                return m_Precision;
            }
        }

        /// <summary>
        /// Obtains line sections for a specific face in this path.
        /// </summary>
        /// <param name="face">The face of interest</param>
        /// <returns>The corresponding sections</returns>
        internal ILineGeometry[] GetSections(LegFace face)
        {
            EnsureAdjusted();

            // Initialize position to the start of the path.
            IPosition p = new Position(m_From);

            // Initial bearing is whatever the rotation is.
            double bearing = m_Rotation;

            // Get the position at the start of the required leg.
            foreach (Leg leg in m_Legs)
            {
                if (leg == face.Leg)
                    break;

                leg.Project(ref p, ref bearing, m_ScaleFactor);
            }

            // We've now got the position at the start of the required leg, and the bearing of the previous leg.
            // If the leg we actually want if a straight leg (or an extra leg layered on a straight), add on any
            // initial angle.
            StraightLeg sLeg = (face.Leg as StraightLeg);
            if (sLeg != null)
                bearing = sLeg.AddStartAngle(bearing);

            return face.Leg.GetSpanSections(p, bearing, m_ScaleFactor, face.Spans);
        }
    }
}
