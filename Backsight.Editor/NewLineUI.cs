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
using System.Drawing;

using Backsight.Editor.Forms;
using Backsight.Forms;
using Backsight.Geometry;
using Backsight.Editor.Operations;

namespace Backsight.Editor
{
	/// <written by="Steve Stanton" on="03-JUL-2007" />
    /// <summary>
    /// User interface for defining a new line.
    /// </summary>
    class NewLineUI : SimpleCommandUI
    {
        #region Class data

        /// <summary>
        /// The point at the start of the new line.
        /// </summary>
        PointFeature m_Start;

        /// <summary>
        /// The point the mouse is currently close to (may end up being either the start or
        /// the end of the new line).
        /// </summary>
        PointFeature m_CurrentPoint;

        /// <summary>
        /// The last mouse position
        /// </summary>
        IPointGeometry m_End;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cc">Object for holding any displayed dialogs</param>
        /// <param name="action">The action that initiated this command</param>
        /// <param name="start">Point initially selected at start of command</param>
        internal NewLineUI(IControlContainer cc, IUserAction action, PointFeature start)
            : base(cc, action)
        {
            m_Start = start;
            m_End = null;
            m_CurrentPoint = start;
        }

        #endregion

        internal override bool Run()
        {
            // Ensure any initial selection has been cleared (if the user clicks in space
            // to cancel the line-add command, the selection needs to be clear).
            Controller.ClearSelection();

            SetCommandCursor();
            return true;
        }

        internal override void SetCommandCursor()
        {
            ActiveDisplay.MapPanel.Cursor = EditorResources.PenCursor;
        }

        /// <summary>
        /// Override indicates that this command performs painting. This means that the
        /// controller will periodically call the <see "Paint"/> method (probably during
        /// idle time).
        /// </summary>
        internal override bool PerformsPainting
        {
            get { return true; }
        }

        internal override void Paint(PointFeature point)
        {
            ISpatialDisplay display = ActiveDisplay;
            IDrawStyle style = Controller.DrawStyle;
            style.FillColor = style.LineColor = Color.Magenta;

            if (m_CurrentPoint!=null)
                style.Render(display, m_CurrentPoint);

            if (m_Start!=null && m_End!=null)
                style.Render(display, new IPosition[] { m_Start, m_End }); 
        }

        internal override void MouseMove(IPosition p)
        {
            CadastralMapModel map = CadastralMapModel.Current;
            ILength size = new Length(map.PointHeight.Meters * 0.5);
            m_CurrentPoint = (map.QueryClosest(p, size, SpatialType.Point) as PointFeature);

            if (m_Start==null)
                return;

            m_End = PointGeometry.Create(p);
            if (m_End.IsCoincident(m_Start))
            {
                m_End = null;
                return;
            }

            ErasePainting();
        }

        /*
void CeView::ShowX ( const CeLine* const pLine ) {

//	There is no point showing intersections if the current
//	default entity type for lines is non-topological (this
//	function is only used when you are adding a new line, and
//	a new line uses the current default entity type for lines).
	
//	Note that I was originally going to skip the CeXObject
//	stuff in that case. However, if you do that, the lines that
//	you intersect get rubbed out as the rubber banding moves
//	over them. So we do intersect detection regardless, and
//	just use the topological status to tell CeXObject::Draw
//	whether it should draw the intersection points or not.

	LOGICAL drawpts = (IsShowX() && this->AddingTopology());

	if ( drawpts && m_Op != ID_LINE_CURVE ) {

		static CeXObject* pX = 0;
		static CeSegment* pXseg = 0;

	//	Check for erase. We'll have a CeXObject if doing a topological
	//	line, and just a CeLine if non-topological.

		if ( pLine==0 && pX ) pX->Erase(AreLabelsDrawn(),drawpts);
		
	//	Get rid of any previous intersection info.
		delete pX;
		delete pXseg;
		pX = 0;
		pXseg = 0;

	//	If not erasing, work out intersections & draw them.
		if ( pLine ) {

			CeLayerList layers(GetActiveTheme());

			pXseg = new CeSegment( *pLine->GetpStart()
								 , *pLine->GetpEnd() );
			pX = new CeXObject(*pXseg,&layers);

			pX->Draw(COL_BLACK,drawpts);
		}

		// Ensure any selected point has priority (there is no need
		// to worry about polygons, lines, or label, since they
		// do not get selected while adding a new line (see CeView::
		// OnSelect())

		CePoint* pSelPoint = m_Sel.GetPoint();
		if ( pSelPoint ) {
			CClientDC aDC(this);
			OnPrepareDC(&aDC);
			this->Highlight(&aDC,pSelPoint);
		}
	}
	else {

		// Not drawing intersection points, so just erase rubber banding.

		if ( m_pStart && m_pEnd && *m_pEnd!=*m_pStart ) {
			if ( m_Op == ID_LINE_CURVE ) {

				// Erase is done with solid pen, while draw is done
				// with a transparent dotted pen.
				DrawCircles(FALSE);
				DrawCircles(TRUE);
				if ( pLine ) pLine->Draw(COL_BLACK);
			}
			else {
				CClientDC aDC(this);
				OnPrepareDC(&aDC);
				aDC.SetROP2(R2_NOTXORPEN);
				CeSegment oldseg(*m_pStart,*m_pEnd);
				oldseg.Draw(this,(CDC*)&aDC);
			}
		}
	}

} // end of ShowX
         */

        internal override void LButtonDown(IPosition p)
        {
            // Cancel the new line if there is no point selected.
            if (m_CurrentPoint==null)
            {
                DialAbort(null);
                return;
            }

            // If we don't have the first point yet, remember the start location.
            // Otherwise remember the end point & add the line.
            AppendToLine(m_CurrentPoint);
        }

        /// <summary>
        /// Appends a point to the new line. If it's the second point, the new line
        /// will be added to the map.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        bool AppendToLine(PointFeature p)
        {
            // If the start point is not defined, just remember it.
            if (m_Start==null)
            {
                m_Start = p;
                return true;
            }

		    // Get the location of the end point and confirm that it's
		    // different from the start.
            if (p.IsCoincident(m_Start))
            {
			    MessageBox.Show("End point cannot match the start point.");
			    return false;
		    }

		    // Add the new line.
            AddNewLine(p);

            DialFinish(null);
	        return true;
        }
        /*
LOGICAL CeView::AppendToLine ( const CePoint& point ) {

	// If the start point is not defined, just remember it. For
	// lines that are expected to be curves, ensure that the
	// point coincides with at least one circle.
	if ( !m_pStart ) {
		if ( m_Op == ID_LINE_CURVE ) {
			if ( !GetCircles(point) ) {
				AfxMessageBox("Selected point does not coincide with any circles.");
				return FALSE;
			}
		}
		m_pStart = point.GetpVertex();
		return TRUE;
	}

	if ( m_Op == ID_LINE_CURVE ) {

		// Get the circles that pass through the selected point.

		CeObjectList clist;
		CeMap* pMap = CeMap::GetpMap();
		CeVertex posn(point);
		UINT4 ncircle = pMap->FindCircles(clist,posn);

		if ( !ncircle ) {
			AfxMessageBox("Selected point does not coincide with any circles.");
			SetLineCursor();
			return FALSE;
		}

		// The point MUST coincide with one of the circles that
		// were found at the start point.

		CeObjectList comlist;
		ncircle = comlist.Intersect(clist,m_Circles);
		if ( !ncircle ) {
			CString msg;
			msg.Format("%s\n%s"
				, "Selected end point does not coincide with any of"
				, "the circles that pass through the start point." );
			AfxMessageBox(msg);
			SetLineCursor();
			return FALSE;
		}

		// Could we have more than 1 to choose from?
		if ( ncircle > 1 ) {
			CString msg;
			msg.Format("%s\n%s"
				, "More than one circle is common to the start"
				, "and the end point. Don't know what to do." );
			AfxMessageBox(msg);
			SetLineCursor();
			return FALSE;
		}

		// Get the location of the end point and confirm that it's
		// different from the start.
		m_pEnd = point.GetpVertex();
		if ( m_pEnd==m_pStart ) {
			AfxMessageBox("End point cannot match the start point.");
			SetLineCursor();
			return FALSE;
		}

		// Add the new arc.
		CeCircle* pCircle = (CeCircle*)comlist.GetpFirst();
		this->AddNewArc(*m_pStart,*m_pEnd,pCircle);
	}
	else {

	}

	OnFinishCommand();
	return TRUE;

} // end of AppendToLine
         */

        /// <summary>
        /// Adds a new line segment feature.
        /// </summary>
        /// <param name="end"></param>
        void AddNewLine(PointFeature end)
        {
            // Create the persistent edit (adds to current session)
            NewLineOperation op = new NewLineOperation();
            bool ok = op.Execute(m_Start, end);

            if (!ok)
                Session.CurrentSession.Remove(op);
        }

        internal override void DialAbort(Control wnd)
        {
            AbortCommand();
        }

        internal override bool DialFinish(Control wnd)
        {
            FinishCommand();
            return true;
        }
    }
}
