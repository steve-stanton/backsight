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

using Backsight.Editor.Properties;
using System.Text;
using Backsight.Forms;
using System.Drawing;
using System.Diagnostics;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="18-NOV-1999" was="CePolygonCheck" />
    /// <summary>
    /// The result of a check on a polygon ring.
    /// </summary>
    class RingCheck : CheckItem
    {
        #region Static

        /// <summary>
        /// Performs checks for a polygon ring.
        /// </summary>
        /// <param name="ring">The ring to check.</param>
        /// <returns>The problem(s) that were found.</returns>
        internal static CheckType CheckRing(Ring ring)
        {
            CheckType types = CheckType.Null;

            // Is it really small? Use a hard-coded size of 0.01
	        // square metres (a 10cm square).
            double area = Math.Abs(ring.Area);
            if (area < 0.01)
                types |= CheckType.SmallPolygon;

            if (ring is Island)
            {
                // Does the polygon have an enclosing polygon?
                Island island = (ring as Island);
                if (island.Container==null)
                    types |= CheckType.NotEnclosed;
            }
            else
            {
        		// Does the polygon have a label?
                Polygon pol = (Polygon)ring;
                if (pol.LabelCount==0)
                    types |= CheckType.NoLabel;
            }

            return types;
        }

        #endregion

        #region Class data

        /// <summary>
        /// The ring the check relates to (never null).
        /// </summary>
        readonly Ring m_Ring;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>RingCheck</c> that relates to the specified polygon ring.
        /// </summary>
        /// <param name="ring">The polygon ring the check relates to (not null).</param>
        /// <param name="types">The type(s) of check this item corresponds to</param>
        /// <exception cref="ArgumentNullException">If <paramref name="ring"/> is null</exception>
        internal RingCheck(Ring ring, CheckType types)
            : base(types)
        {
            if (ring==null)
                throw new ArgumentNullException();

            m_Ring = ring;
        }

        #endregion

        /// <summary>
        /// Repaint icon(s) representing this check result.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The style for the drawing</param>
        internal override void Render(ISpatialDisplay display, IDrawStyle style)
        {
            // Return if the polygon doesn't overlap the draw.
            IWindow win = m_Ring.Extent;
            if (!win.IsOverlap(display.Extent))
                return;

            // Is the polygon real small? If so, we'll use a display
            // position that's just to the left of the polygon's window.
            IPosition gpos = null;
            Position p = null;
            CheckType types = Types;
            bool isSmall = ((types & CheckType.SmallPolygon)!=0);

            if (isSmall)
            {
                // Get the north-west corner of the window.
                gpos = new Position(win.Min.X, win.Max.Y); // NW corner

                // Draw the icon
                double shift = IconSize(display);
                p = new Position(gpos.X-shift, gpos.Y);
                style.Render(display, p, Resources.CheckSmallPolygonIcon);

                // And shift a bit more in case we draw more.
                p.X -= shift;
            }

            if (m_Ring is Island)
            {
                // If the polygon is a phantom that has no enclosing polygon,
                // display an icon at the east point (unless we previously
                // found that the polygon was real small).

                if (!isSmall && (types & CheckType.NotEnclosed)!=0)
                {
                    if (gpos==null)
                    {
                        gpos = m_Ring.GetEastPoint();
                        p = new Position(gpos);
                    }

                    style.Render(display, p, Resources.CheckNotEnclosedIcon);
                }
            }
            else
            {
                // The other two possibilities relate to regular (non-phantom) polygons.

                // If the polygon has no label, get a suitable position (if the polygon is
                // real small, just use the position we already have).

                if ((types & CheckType.NoLabel)!=0)
                {
                    // If the position hasn't been defined (because the polygon is NOT small),
                    // figure out a good spot. If that fails, fall back on the east point.

                    if (gpos==null)
                    {
                        Debug.Assert(!isSmall);

                        double size = IconSize(display);
                        gpos = (m_Ring as Polygon).GetLabelPosition(size, size);
                        if (gpos==null)
                            gpos = m_Ring.GetEastPoint();

                        // The shift here is not ideal if the east point got used, However,
                        // it simplifies the logic in PaintOut to use the same offset.
                        p = new Position(gpos.X-size/2, gpos.Y+size/2);
                    }

                    style.Render(display, p, Resources.CheckNoLabelIcon);
                }
            }

            // Remember the reference position we used.
            Place = gpos;
        }

        /// <summary>
        /// Draws the checked item in a way that highlights it during a check review.
        /// For polygons that are small, this draws the perimeter in orange. This helps
        /// to identify polygons that are actually unclosed lines or networks.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        internal override void HighlightCheckedItem(ISpatialDisplay display)
        {
            if ((Types & CheckType.SmallPolygon)!=0)
            {
                IDrawStyle style = new DrawStyle(Color.Orange);
                m_Ring.RenderOutline(display, style);
            }
        }

        /// <summary>
        /// Paints out those results that no longer apply.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The style for the drawing</param>
        /// <param name="newTypes">The new results</param>
        internal override void PaintOut(ISpatialDisplay display, IDrawStyle style, CheckType newTypes)
        {
            IPosition p = this.Place;
            double shift = IconSize(display);
            CheckType oldTypes = Types;

            if (IsPaintOut(CheckType.SmallPolygon, oldTypes, newTypes))
            {
                p = new Position(p.X-shift, p.Y);
                style.Render(display, p, Resources.CheckPolygonIgnoreIcon);

                // And shift a bit more in case we draw more.
                p = new Position(p.X-shift, p.Y);
            }

            if (IsPaintOut(CheckType.NotEnclosed, oldTypes, newTypes))
                style.Render(display, p, Resources.CheckPolygonIgnoreIcon);

            if (IsPaintOut(CheckType.NoLabel, oldTypes, newTypes))
            {
                if ((oldTypes & CheckType.SmallPolygon)==0)
                    p = new Position(p.X-shift/2, p.Y+shift/2);

                style.Render(display, p, Resources.CheckPolygonIgnoreIcon);
            }
        }

        /// <summary>
        /// Rechecks this result.
        /// </summary>
        /// <returns>The result(s) that still apply.</returns>
        internal override CheckType ReCheck()
        {
            // Return if the result has been cleared.
            CheckType types = Types;
            if (types == CheckType.Null)
                return CheckType.Null;

            // Recheck the polygon involved
            return CheckRing(m_Ring);
        }

        /// <summary>
        /// A textual explanation about this check result.
        /// </summary>
        internal override string Explanation
        {
            get
            {
                CheckType types = Types;
                StringBuilder sb = new StringBuilder();

                if ((types & CheckType.SmallPolygon)!=0)
                {
                    // If the area is absolute zero, it could well be
                    // a floating line or a network.

                    if (m_Ring.Area < Double.Epsilon)
                        sb.Append("Polygon has zero area");
                    else
                    {
                        // Format a sliver length.
                        double perim = m_Ring.GetEdgeLength().Meters;
                        string lenstr = String.Format("{0:0.00}", perim*0.5);
                        string msg;

                        // If it looks like "0.00", just show the dimensions in millimeters.
                        if (lenstr == "0.00")
                        {
                            IWindow w = m_Ring.Extent;
                            msg = String.Format("Tiny area ({0:0.000} x {1:0.000} mm)", w.Width*1000.0, w.Height*1000.0);
                        }
                        else
                            msg = String.Format("Tiny sliver area ({0} meters long)", lenstr);

                        sb.Append(msg);
                    }

                    sb.Append(System.Environment.NewLine);
                }

                if ((types & CheckType.NotEnclosed)!=0)
                    sb.Append("Polygon not enclosed by any polygon" + System.Environment.NewLine);

                if ((types & CheckType.NoLabel)!=0)
                    sb.Append("Polygon has no label" + System.Environment.NewLine);

                return sb.ToString();
            }
        }

        /// <summary>
        /// Returns a display position for this check. This is the position to
        /// use for auto-centering the draw. It is not necessarily the same as
        /// the position of the problem icon.
        /// </summary>
        internal override IPosition Position
        {
            get
            {
                // If the polygon is supposedly real small, just use the center of the polygon's window.
                CheckType types = Types;
                if ((types & CheckType.SmallPolygon)!=0)
                    return m_Ring.Extent.Center;

                // If the polygon is an island, use the east point (because that's roughly where the marker is).
                if (m_Ring is Island)
                    return m_Ring.GetEastPoint();

                // Calculate the position for a label that has the same size as an icon. If that fails for
                // any reason, use the east point of the polygon.
                double size = IconSize(CadastralEditController.Current.ActiveDisplay);
                IPosition p = (m_Ring as Polygon).GetLabelPosition(size, size);
                return (p==null ? m_Ring.GetEastPoint() : p);
            }
        }

        /// <summary>
        /// Selects the object that this check relates to.
        /// </summary>
        internal override void Select()
        {
            SpatialSelection ss = new SpatialSelection();
            ss.Add(m_Ring);
            CadastralEditController.Current.SetSelection(ss);

            // CadastralEditController.Current.ActiveDisplay.MapPanel.Focus();

            // @devnote Don't set the focus to the active display! When we do this in DividerCheck
            // and TextCheck, it makes it easy to do stuff like editing the problem item (e.g. delete
            // it with a keystroke). You can't do that with a polygon. If you switch the focus now,
            // hitting RETURN to move on to the next problem has no effect, when there's a
            // tendency to think that it will.
        }
    }
}
