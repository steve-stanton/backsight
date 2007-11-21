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
            IPosition p = new Position(win.Min.X, win.Max.Y); // NW corner
            CheckType types = Types;
            bool isSmall = ((types & CheckType.SmallPolygon)!=0);

            if (isSmall)
            {
                // Get the north-west corner of the window.
                double shift = display.DisplayToLength(32);
                p = new Position(p.X-shift, p.Y);

                // Draw the icon
                style.Render(display, p, Resources.CheckSmallPolygonIcon);

                // And shift a bit more in case we draw more.
                p = new Position(p.X-32, p.Y);
            }

            if (m_Ring is Island)
            {
                // If the polygon is a phantom that has no enclosing polygon,
                // display an icon at the east point (unless we previously
                // found that the polygon was real small).

                if (!isSmall && (types & CheckType.NotEnclosed)!=0)
                {
                    p = m_Ring.GetEastPoint();
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

                    if (!isSmall)
                    {
                        double size = display.DisplayToLength(32);
                        IPosition pp = (m_Ring as Polygon).GetLabelPosition(size, size);
                        p = (pp==null ? m_Ring.GetEastPoint() : pp);

                        // The shift here is not ideal if the east point got used, However,
                        // it simplifies the logic in PaintOut to use the same offset.
                        double shift = display.DisplayToLength(16);
                        p = new Position(p.X-shift, p.Y+shift);
                    }

                    style.Render(display, p, Resources.CheckNoLabelIcon);
                }
            }

            // Remember the reference position we used.
            Place = p;
        }

        /*
//	@mfunc	Do any check-specific paint of the object associated
//			with this object.
//
//////////////////////////////////////////////////////////////////////

void CePolygonCheck::Paint ( void ) const {

	// Return if the polygon isn't real small.
	if ( (GetTypes() & CHB_PSMALL)==0 ) return;

	// Locate the polygon in question.
	const CeTheme& theme = GetActiveTheme();
	const CePolygon* const pPol = m_pFace->GetPolygon(theme);
	if ( !pPol ) return;

	// Draw its perimeter in yellow.
	CeDraw* pDraw = GetpDraw();
	CClientDC dc(pDraw);
	CPen yellow(PS_SOLID,0,RGB(255,255,0));
	dc.SelectObject(&yellow);
	pPol->Draw(pDraw,&dc);

} // end of Paint
         */

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
//	@mfunc	Paint out those results that no longer apply.
//
//	@parm	The new results.
//	@parm	The thing we're drawing to.
//	@parm	Array of icon handles.
//
//////////////////////////////////////////////////////////////////////

void CePolygonCheck::PaintOut ( const UINT4 newTypes
							  , CeDC& gdc
							  , HICON* icons ) const {

	// Get the reference position last used to paint stuff,
	// and express in logical units.
	const CeVertex& gpos = GetPlace();
	CPoint pt;
	gdc.GetLogPoint(gpos,pt);

	// Get the Windows device context so we can draw directly.
	CDC* pDC = gdc.GetDC();

	// Note the problems that were last painted.
	UINT4 oldTypes = GetTypes();

	if ( (oldTypes & CHB_PSMALL) ) {

		// Draw the icon
		pt.x -= 16;
		if ( (newTypes & CHB_PSMALL)==0 )
			pDC->DrawIcon(pt.x,pt.y,icons[CHI_PIGNORE]);

		// And shift a bit more in case we draw more.
		pt.x -= 16;
	}

	if ( (oldTypes & CHB_NOPOLENCPOL) &&
		 (newTypes & CHB_NOPOLENCPOL)==0 )
			pDC->DrawIcon(pt.x,pt.y,icons[CHI_PIGNORE]);

	if ( (oldTypes & CHB_NOLABEL) &&
		 (newTypes & CHB_NOLABEL)==0 ) {

		if ( (oldTypes & CHB_PSMALL)==0 ) {
			pt.x -= 8;
			pt.y -= 8;
		}

		pDC->DrawIcon(pt.x,pt.y,icons[CHI_PIGNORE]);
	}

} // end of PaintOut
         */

        /// <summary>
        /// Rechecks this result.
        /// </summary>
        /// <returns>The result(s) that still apply.</returns>
        internal override CheckType ReCheck()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /*
//	@mfunc	Recheck this result.
//
//	@rdesc	The result(s) that still apply.
//
//////////////////////////////////////////////////////////////////////

UINT4 CePolygonCheck::ReCheck ( void ) {

	// Return if the result has been cleared.
	UINT4 types = GetTypes();
	if ( types==0 ) return 0;

	// Get the polygon involved.
	const CeTheme& theme = GetActiveTheme();
	const CePolygon* const pPol = m_pFace->GetPolygon(theme);

	// Recheck.
	if ( pPol )
		return CheckPolygon(*pPol,theme);
	else
		return 0;

} // end of ReCheck
         */

        /// <summary>
        /// A textual explanation about this check result.
        /// </summary>
        internal override string Explanation
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        /*
//	@mfunc	Provide a textual explanation about this check result.
//
//	@parm	The string to define.
//	@parm	The theme the check was done for.
//
//////////////////////////////////////////////////////////////////////

void CePolygonCheck::Explain ( CString& str
							 , const CeTheme& theme ) const {

	// Start out with an empty string.
	str.Empty();

	// Get the problem type(s).
	UINT4 types = GetTypes();

	if ( (types & CHB_PSMALL) ) {
		const CePolygon* const pPol = m_pFace->GetPolygon(theme);
		if ( pPol ) {

			// If the area is absolute zero, it could well be
			// a floating line or a network.

			const FLOAT8 area = fabs(pPol->GetArea(theme));

			if ( area < TINY )
				str += "Polygon has zero area";
			else {

				CString msg;
				CString lenstr;

				// Format a sliver length.
				const FLOAT8 perim = pPol->GetPerimeter();
				lenstr.Format("%.2lf",perim*0.5);

				// If it looks like "0.00", just show the dimensions
				// in millimetres.

				if ( lenstr=="0.00" ) {

					// Get the actual dimensions of the polygon.
					FLOAT8 dx;
					FLOAT8 dy;
					pPol->GetDimensions(dx,dy);
			
					msg.Format("Tiny area (%.3lf x %.3lf mm)\n"
						, dx*1000.0, dy*1000.0 );
				}
				else
					msg.Format("Tiny sliver area (%s metres long)\n"
						, lenstr );

				str += msg;
			}
		}
		else
			str += "Tiny polygon (seems to have vanished)\n";
	}

	if ( (types & CHB_NOPOLENCPOL) )
		str += "Polygon not enclosed by any polygon\n";

	if ( (types & CHB_NOLABEL) )
		str += "Polygon has no label\n";

} // end of Explain
         */
        /// <summary>
        /// Returns a display position for this check. This is the position to
        /// use for auto-centering the draw. It is not necessarily the same as
        /// the position of the problem icon.
        /// </summary>
        internal override IPosition Position
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        /*
//	@mfunc	Return a display position for this check. This is
//			the position to use for auto-centring the draw. It
//			is not necessarily the same as the position of
//			the problem icon.
//
//	@parm	The position to define.
//
//	@rdesc	TRUE if position was defined ok.
//
//////////////////////////////////////////////////////////////////////

#include "CdUtil.h"
#include "CeDraw.h"

LOGICAL CePolygonCheck::GetPosition ( CeVertex& pos ) const {

	// Get the polygon involved.
	const CeTheme& theme = GetActiveTheme();
	const CePolygon* const pPol = m_pFace->GetPolygon(theme);
	if ( !pPol ) return FALSE;

	// If the polygon is supposedly real small, just use the
	// centre of the polygon's window.
	UINT4 types = GetTypes();
	if ( (types & CHB_PSMALL) ) {
		const CeWindow* const pWin = pPol->GetpWindow();
		pWin->GetCentre(&pos);
		return pos.IsDefined();
	}

	// If the polygon is an island, use the east point (because
	// that's roughly where the marker is).
	if ( pPol->IsIsland() ) {
		pPol->GetEastPoint(&pos);
		return pos.IsDefined();
	}

	// Calculate the position for a label that has the same
	// size as an icon. If that fails for any reason, use the
	// east point of the polygon.
	CeDraw* pDraw = GetpDraw();
	const FLOAT8 size = pDraw->LPToLength(UINT4(SM_CXICON));
	if ( !pPol->GetLabelPosition(size,size,pos) )
		pPol->GetEastPoint(&pos);

	return pos.IsDefined();

} // end of GetPosition
         */
        /// <summary>
        /// Selects the object that this check relates to.
        /// </summary>
        internal override void Select()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /*
//	@mfunc	Select the object that this check relates to.
//
//////////////////////////////////////////////////////////////////////

#include "CdUtil.h"
#include "CeView.h"
#include "CeSelection.h"

void CePolygonCheck::Select ( void ) const {

	// Find the relevant polygon.
	const CeTheme& theme = GetActiveTheme();
	const CePolygon* const pPol = m_pFace->GetPolygon(theme);
	if ( !pPol ) return;

	// And select it.
	CeView* pView = GetpView();
	CeSelection& sel = pView->GetSel();
	sel.ReplaceWith(pPol);

	// pView->SetFocus();

	// @devnote Don't set the focus to the view! When we do
	// this in CeArcCheck and CeLabelCheck, it makes it easy
	// to do stuff like editing the problem item (e.g. delete
	// it with a keystroke). You can't do that with a polygon.
	// If you switch the focus now, hitting RETURN to move
	// on to the next problem has no effect, when there's a
	// tendency to think that it will.

} // end of Select
         */
    }
}
