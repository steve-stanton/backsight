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
using System.Text;
using System.Diagnostics;
using Backsight.Editor.Properties;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="18-NOV-1999" was="CeArcCheck" />
    /// <summary>
    /// The result of a check on a polygon ring divider.
    /// </summary>
    class DividerCheck : CheckItem
    {
        #region Static

        /// <summary>
        /// Performs checks on the supplied divider.
        /// </summary>
        /// <param name="d">The divider to check.</param>
        /// <returns>The problem(s) that were found.</returns>
        internal static CheckType Check(IDivider d)
        {
            // Ignore invisible dividers (the result of trimming dangling lines)
            if (!d.IsVisible)
                return CheckType.Null;

            CheckType types = CheckType.Null;

            // Is the divider extremely small (smaller than 1mm on the mapping plane)?

            double len = d.LineGeometry.Length.Meters;
            if (len < 0.001)
                types |= CheckType.SmallLine;

            // Always flag overlaps. For all other dividers, check if
	        // dangling, floating, or a bridge.

            if (d.IsOverlap)
                types |= CheckType.Overlap;
            else
                types |= CheckNeighbours(d);

            return types;
        }

        /// <summary>
        /// Checks if a divider is a dangle, floating, or a bridge.
        /// </summary>
        /// <param name="d">The divider to check</param>
        /// <returns>Bit mask of the check(s) the divider failed.</returns>
        static CheckType CheckNeighbours(IDivider d)
        {
            Debug.Assert(!d.IsOverlap);

	        // Return if the divider has different polygons on both sides.
            if (d.Left != d.Right)
                return CheckType.Null;

            bool sDangle = Topology.IsDangle(d, d.From);
            bool eDangle = Topology.IsDangle(d, d.To);

            if (sDangle && eDangle)
                return CheckType.Floating;

            if (sDangle || eDangle)
                return CheckType.Dangle;

            return CheckType.Bridge;
        }

        #endregion

        #region Class data

        /// <summary>
        /// The divider the check relates to (never null).
        /// </summary>
        readonly IDivider m_Divider;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>DividerCheck</c> that relates to the specified divider.
        /// </summary>
        /// <param name="divider">The divider the check relates to (not null).</param>
        /// <param name="types">The type(s) of check this item corresponds to</param>
        /// <exception cref="ArgumentNullException">If <paramref name="divider"/> is null</exception>
        internal DividerCheck(IDivider divider, CheckType types)
            : base(types)
        {
            if (divider==null)
                throw new ArgumentNullException();

            m_Divider = divider;
        }

        #endregion

        /// <summary>
        /// The divider the check relates to (never null).
        /// </summary>
        internal IDivider Divider
        {
            get { return m_Divider; }
            //set { m_Divider = value; }
        }

        /// <summary>
        /// Repaint icon(s) representing this check result.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The style for the drawing</param>
        internal override void Render(ISpatialDisplay display, IDrawStyle style)
        {
            // Return if the line has been de-activated.
            if (m_Divider.Line.IsInactive)
                return;

            // Draw half way along the divider.
            PaintAt(display, style, this.Position);
        }

        /// <summary>
        /// Repaints icon(s) representing this check result.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="gpos">The position for the draw</param>
        void PaintAt(ISpatialDisplay display, IDrawStyle style, IPosition gpos)
        {
            // Remember the reference position.
            Place = gpos;

            // Define position for first icon (a little to the left of the divider,
            // assuming an icon size of 32 pixels)
            double shift = display.DisplayToLength(16);
            Position p = new Position(gpos.X-shift, gpos.Y);

            // Define shift for any further icons
            shift = display.DisplayToLength(36);

            // Draw icon(s).
            CheckType types = Types;
            if ((types & CheckType.SmallLine)!=0)
            {
                style.Render(display, p, Resources.CheckSmallLineIcon);
                p = new Position(p.X-shift, p.Y);
            }

            if ((types & CheckType.Dangle)!=0)
            {
                style.Render(display, p, Resources.CheckDanglingIcon);
                p = new Position(p.X-shift, p.Y);
            }

            if ((types & CheckType.Overlap)!=0)
            {
                style.Render(display, p, Resources.CheckOverlapIcon);
                p = new Position(p.X-shift, p.Y);
            }

            if ((types & CheckType.Floating)!=0)
            {
                style.Render(display, p, Resources.CheckFloatingIcon);
                p = new Position(p.X-shift, p.Y);
            }

            if ((types & CheckType.Bridge)!=0)
            {
                style.Render(display, p, Resources.CheckBridgeIcon);
                p = new Position(p.X-shift, p.Y);
            }
        }

        /// <summary>
        /// Paints out those results that no longer apply.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="newTypes">The new results</param>
        internal override void PaintOut(ISpatialDisplay display, CheckType newTypes)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        /*
        void CeArcCheck::PaintOut ( const UINT4 newTypes
                                  , CeDC& gdc
                                  , HICON* icons ) const {

            // Get the reference position last used for painting.
            const CeVertex& gpos = GetPlace();

            // And express in logical units.
            CPoint pt;
            gdc.GetLogPoint(gpos,pt);

            // Get the Windows device context so we can draw directly.
            CDC* pDC = gdc.GetDC();

            // Shift a bit so the icon is a bit to the left of the
            // mid-point (assumes MM_TEXT, and an icon size of 16
            // pixels).
            pt.x -= 8;
            pt.y -= 8;

            // The icons to paint out are those that we originally
            // had, but which are not in the new results.
            UINT4 oldTypes = GetTypes();

            if ( (oldTypes & CHB_LSMALL) ) {
                if ( (newTypes & CHB_LSMALL)==0 )
                    pDC->DrawIcon(pt.x,pt.y,icons[CHI_LIGNORE]);
                pt.x -= 20;
            }

            if ( (oldTypes & CHB_DANGLE) ) {
                if ( (newTypes & CHB_DANGLE)==0 )
                    pDC->DrawIcon(pt.x,pt.y,icons[CHI_LIGNORE]);
                pt.x -= 20;
            }

            if ( (oldTypes & CHB_OVERLAP) ) {
                if ( (newTypes & CHB_OVERLAP)==0 )
                    pDC->DrawIcon(pt.x,pt.y,icons[CHI_LIGNORE]);
                pt.x -= 20;
            }

            if ( (oldTypes & CHB_FLOAT) ) {
                if ( (newTypes & CHB_FLOAT)==0 )
                    pDC->DrawIcon(pt.x,pt.y,icons[CHI_LIGNORE]);
                pt.x -= 20;
            }

            if ( (oldTypes & CHB_BRIDGE) ) {
                if ( (newTypes & CHB_BRIDGE)==0 )
                    pDC->DrawIcon(pt.x,pt.y,icons[CHI_LIGNORE]);
                pt.x -= 20;
            }

        } // end of PaintOut
         */

        /// <summary>
        /// Rechecks this result.
        /// </summary>
        /// <returns>The result(s) that still apply.</returns>
        internal override CheckType ReCheck()
        {
            // No errors if the line has been deleted.
            if (m_Divider.Line.IsInactive)
                return CheckType.Null;

            // Clear the errors if the line is now non-topological.
            if (!m_Divider.Line.IsTopological)
                return CheckType.Null;

            // Return if the result has been cleared.
            if (Types == CheckType.Null)
                return CheckType.Null;

            // Check the divider
            return Check(m_Divider);
        }

        /// <summary>
        /// A textual explanation about this check result.
        /// </summary>
        internal override string Explanation
        {
            get
            {
                if (m_Divider.Line.IsInactive)
                    return "(line deleted)";

                CheckType types = Types;
                StringBuilder sb = new StringBuilder();

                if ((types & CheckType.SmallLine)!=0)
                    sb.Append("Very small line section" + System.Environment.NewLine);

                if ((types & CheckType.Dangle)!=0)
                    sb.Append("Dangling at one end" + System.Environment.NewLine);

                if ((types & CheckType.Overlap)!=0)
                    sb.Append("Precise overlap" + System.Environment.NewLine);

                if ((types & CheckType.Floating)!=0)
                    sb.Append("Dangling at both ends" + System.Environment.NewLine);

                if ((types & CheckType.Bridge) != 0)
                {
                    Polygon pol = (m_Divider.Left as Polygon);
                    TextFeature label = (pol == null ? null : pol.Label);
                    if (label == null)
                        sb.Append("Same polygon on both sides");
                    else
                        sb.Append(String.Format("Same polygon ({0}) on both sides", label.FormattedKey));

                    sb.Append(System.Environment.NewLine);
                }

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
                // Return a position half way along the divider.
                LineGeometry geom = m_Divider.LineGeometry;
                double len = geom.Length.Meters;
                IPosition pos;
                geom.GetPosition(new Length(len*0.5), out pos);
                return pos;
            }
        }

        /// <summary>
        /// Selects the object that this check relates to.
        /// </summary>
        internal override void Select()
        {
            SpatialSelection ss = new SpatialSelection();
            ss.Add(m_Divider.Line);

            // If the divider has the same polygon on both sides, select
            // the polygon as well.
            if ((Types & CheckType.Bridge)!=0 && m_Divider.Left!=null)
                ss.Add(m_Divider.Left);

            CadastralEditController.Current.SetSelection(ss);
            CadastralEditController.Current.ActiveDisplay.MapPanel.Focus();
        }
    }
}
