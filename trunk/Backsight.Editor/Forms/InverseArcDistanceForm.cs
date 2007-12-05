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
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

using Backsight.Geometry;
using Backsight.Forms;

namespace Backsight.Editor.Forms
{
    /// <summary>
    /// Inverse distance calculator for circular arcs.
    /// </summary>
    partial class InverseArcDistanceForm : InverseForm
    {
        #region Class data

        /// <summary>
        /// The first point for the distance calculation.
        /// </summary>
        PointFeature m_Point1;

        /// <summary>
        /// The second point for the distance calculation.
        /// </summary>
        PointFeature m_Point2;

        /// <summary>
        /// Circles for first point
        /// </summary>
        Circle[] m_Cir1;

        /// <summary>
        /// Circles for second point
        /// </summary>
        Circle[] m_Cir2;

        /// <summary>
        /// Circles common to points 1 and 2. May be null. If not null, it
        /// will contain at least one element.
        /// </summary>
        Circle[] m_CommCir;

        /// <summary>
        /// True if the short arc distance should be shown.
        /// </summary>
        bool m_WantShort;

        /// <summary>
        /// The geometry that corresponds to the distance that's currently shown (null
        /// if no distance is currently shown)
        /// </summary>
        ICircularArcGeometry m_CurrentDistanceArc;

        #endregion

        internal InverseArcDistanceForm()
        {
            InitializeComponent();
            color1Button.BackColor = InverseColors[0];
            color2Button.BackColor = InverseColors[1];

            m_Point1 = m_Point2 = null;
            m_Cir1 = m_Cir2 = m_CommCir = null;
            m_WantShort = true;
            m_CurrentDistanceArc = null;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        internal override void InitializeUnits(DistanceUnit units)
        {
            if (units.UnitType == DistanceUnitType.Feet)
                fRadioButton.Checked = true;
            else if (units.UnitType == DistanceUnitType.Chains)
                cRadioButton.Checked = true;
            else
                mRadioButton.Checked = true;
        }

        private void shortRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (shortRadioButton.Checked)
            {
                m_WantShort = true;
                ErasePainting();
                ShowResult();
            }
        }

        private void longRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (longRadioButton.Checked)
            {
                m_WantShort = false;
                ErasePainting();
                ShowResult();
            }
        }

        private void mRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (mRadioButton.Checked)
            {
                base.SetMeters();
                ShowResult();
            }
        }

        private void fRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (fRadioButton.Checked)
            {
                base.SetFeet();
                ShowResult();
            }
        }

        private void cRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (cRadioButton.Checked)
            {
                base.SetChains();
                ShowResult();
            }
        }

        internal virtual void ShowResult()
        {
            // If we have two points, get the distance between them,
            // format the result, and display it.
            if (m_Point1!=null && m_Point2!=null && m_CommCir!=null)
            {
                // It's conceivable that the two points share more than
                // one common circle. For now, just pick off the first
                // common circle and use that.
                Circle circle = m_CommCir[0];

                // Get the centre of the circle.
                IPosition c = circle.Center;

                // Get the clockwise angle from point 1 to point 2.
                Turn reft = new Turn(c, m_Point1);
                double angle = reft.GetAngle(m_Point2).Radians;
                bool iscw = true;

                // Make sure the angle is consistent with whether we want
                // the short or the long arc distance.
                bool isshort = (angle < Constants.PI);
                if (isshort != m_WantShort)
                {
                    angle = Constants.PIMUL2 - angle;
                    iscw = false;
                }

                // Get the arc distance and display it.
                double metric = angle * circle.Radius.Meters;
                distanceTextBox.Text = Format(metric, m_Point1, m_Point2);

                // Remember the geometry that corresponds to the displayed distance (this
                // will be drawn when the controller periodically calls the Draw method).
                m_CurrentDistanceArc = new CircularArcGeometry(circle, m_Point1, m_Point2, iscw);
    	    }
	        else
            {
                distanceTextBox.Text = "<no distance>";
                m_CurrentDistanceArc = null;
    	    }
        }

        protected PointFeature Point1
        {
            get { return m_Point1; }
        }

        protected PointFeature Point2
        {
            get { return m_Point2; }
        }

        protected Circle FirstCommonCircle
        {
            get { return (m_CommCir==null ? null : m_CommCir[0]); }
        }

        internal override void Draw()
        {
            ISpatialDisplay display = EditingController.Current.ActiveDisplay;

            // Redraw the arcs that are suitable for the next pointing operation.
		    DrawCurves();

            // Draw a solid line to represent displayed distance (if any)
            if (m_CurrentDistanceArc!=null)
            {
                DrawStyle style = new DrawStyle(Color.Magenta);
                style.Pen.Width = 3.0F;
                style.Render(display, m_CurrentDistanceArc);
            }

            // Draw the points.
            if (m_Point1!=null)
                m_Point1.Draw(display, InverseColors[0]);

            if (m_Point2!=null)
                m_Point2.Draw(display, InverseColors[1]);
        }

        internal override void OnSelectPoint(PointFeature point)
        {
            // Ignore undefined points.
            if (point==null)
                return;

            // Get a list of the circles related to the incoming point
            // If we don't get any, tell the user and ignore the point.
            Circle[] circles = GetCircles(point);
            if (circles==null)
            {
                MessageBox.Show("Point is not on a circular arc.");
                return;
            }

            Debug.Assert(circles.Length>0);

            // If we have a list of circles from a previous point
            if (m_Point1!=null)
            {
                // If BOTH points were previously defined, compare the circles
                // for the selected point with the circles for the second point
                // (which will become the first point if there is at least one
                // common circle is involved). If we don't yet have a second
                // point, compare with the first set of circles.

                Circle[] compare = (m_Point2==null ? m_Cir1 : m_Cir2);

                // Intersect the circles for the new point with what we
                // already have. If there is no intersection, issue a
                // message and get ready to treat the new point as the
                // initial point (if you don't do this, you'll be eternally
                // stuck with the first curve you pointed at).            }

                Circle[] commcir = GetCommonCircles(circles, compare);
                if (commcir==null)
                {
                    MessageBox.Show("Switching to a different circular arc.");
                    m_Point1 = m_Point2 = null;
                }
            }

            // Hold on to the new point, along with its associated circles.
            if (m_Point1==null)
            {
                m_Point1 = point;
                m_Cir1 = circles;
            }
            else
            {
                // If we have a second point, shift it back to occupy
                // the first slot
                if (m_Point2!=null)
                {
                    m_Point1 = m_Point2;
                    m_Cir1 = m_Cir2;
                }

                // Hold the new info as the 2nd point.
                m_Point2 = point;
                m_Cir2 = circles;

                // Define the common circles
                m_CommCir = GetCommonCircles(m_Cir1, m_Cir2);
            }

            // Make sure the edit boxes reflect what we now have
            point1TextBox.Text = GetPointText(m_Point1);
            point2TextBox.Text = GetPointText(m_Point2);

            // Display the distance if we've got 2 points.
            ShowResult();
        }

        /// <summary>
        /// Draws the circular arcs that are suitable for the next pointing operation.
        /// As dotted lines. This is based on the list of circles that are associated
        /// with the last point that was specified.
        /// </summary>
        void DrawCurves()
        {
            // Use the circles associated with the last point entered (once the
            // 2nd point has been defined, that's the one we always use).

            Circle[] circles = (m_Point2!=null ? m_Cir2 : m_Cir1);
            if (circles==null)
                return;

            // Highlight the arcs associated with each circle

            ISpatialDisplay display = EditingController.Current.ActiveDisplay;
            IDrawStyle dottedBlack = new DottedStyle(Color.Black);
            IDrawStyle white = new DrawStyle(Color.White);

            foreach(Circle c in circles)
            {
                foreach(ArcFeature arc in c.Arcs)
                {
                    if (!arc.IsInactive)
                    {
                        arc.Render(display, white);
                        arc.Render(display, dottedBlack);
                    }
                }
            }
        }

        /// <summary>
        /// Get a list of the circles related to a point. These are the <see cref="Circle"/>
        /// objects that any incident curved lines are based on (which means either
        /// <see cref="ArcGeometry"/> objects, or sections that are based on arcs).
        /// </summary>
        /// <param name="point">The point on a curve.</param>
        /// <returns>The circles that were found (null if nothing found)</returns>
        Circle[] GetCircles (PointFeature point)
        {
            EditingIndex index = CadastralMapModel.Current.EditingIndex;
            ILength tol = new Length(0.001);
            List<Circle> circles = index.QueryCircles(point, tol);
            return (circles.Count==0 ? null : circles.ToArray());
        }

        /// <summary>
        /// Returns the intersection of two circle arrays.
        /// </summary>
        /// <param name="a">The first array</param>
        /// <param name="b">The second array</param>
        /// <returns>The circles that exist in both arrays. Null if either of the
        /// supplied arrays is null (or empty), or there are no common circles.</returns>
        Circle[] GetCommonCircles(Circle[] a, Circle[] b)
        {
            if (a==null || a.Length==0)
                return null;

            if (b==null || b.Length==0)
                return null;

            if (Object.ReferenceEquals(a, b))
                return a;

            List<Circle> result = new List<Circle>(Math.Min(a.Length, b.Length));

            foreach (Circle c in a)
            {
                if (Array.IndexOf<Circle>(b, c)>=0)
                    result.Add(c);
            }

            return (result.Count==0 ? null : result.ToArray());
        }
    }
}