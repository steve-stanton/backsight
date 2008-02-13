/// <remarks>
/// Copyright 2008 - Steve Stanton. This file is part of Backsight
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
using System.Collections.Generic;
using System.Drawing;

using Backsight.Forms;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="01-NOV-1999" was="CeSelection" />
    /// <summary>
    /// Performs the selection of spatial features in a map.
    /// </summary>
    class SelectionTool
    {
        #region Class data

        /// <summary>
        /// The points defining the limits for selection area (if any).
        /// </summary>
        List<IPosition> m_Limit;

        /// <summary>
        /// The last mouse position.
        /// </summary>
        IPosition m_Mouse;

        /// <summary>
        /// True if the last position is currently being rubber-banded.
        /// </summary>
        //bool m_IsBand;

        /// <summary>
        /// True if limit is currently drawn.
        /// </summary>
        //bool m_IsLimit;

        /// <summary>
        /// The current selection obtained via m_Limit. This will be included
        /// in the selection when the limit line is completed.
        /// </summary>
        List<ISpatialObject> m_LimSel;

        /// <summary>
        /// The currently selected items.
        /// </summary>
        readonly List<ISpatialObject> m_Selection;

        #endregion

        #region Constructors

        internal SelectionTool()
            : base()
        {
            m_Limit = null;
            m_Mouse = null;
            //m_IsBand = false;
            //m_IsLimit = false;
            m_LimSel = null;
            m_Selection = new List<ISpatialObject>();
        }

        #endregion

        /// <summary>
        /// Adds a specific item to the current selection.
        /// </summary>
        /// <param name="thing">The item to append to the selection</param>
        /// <returns>True if item was appended. False if it's already in the selection</returns>
        bool Append(ISpatialObject thing)
        {
            if (m_Selection.Contains(thing))
                return false;

            m_Selection.Add(thing);
            return true;
        }

        /// <summary>
        /// Replaces the current selection with a specific object.
        /// </summary>
        /// <param name="thing">The object to select</param>
        internal void ReplaceWith(ISpatialObject thing)
        {
            // Clear out the current selection.
            RemoveSel();

            // Append the thing to the selection (so long as it
            // has a defined value)
            if (thing != null)
            {
                Append(thing);
                ErasePainting();
                //ReHighlight();
            }
        }

        /// <summary>
        /// Removes everything from this selection.
        /// </summary>
        internal void RemoveSel()
        {
            // Unhighlight everything.
            UnHighlight();

            // Clear out the saved selection
            m_Selection.Clear();

            // Make sure we have no limit-line selection either.
            DiscardLimit();
        }

        /// <summary>
        /// Checks if a single line is selected.
        /// </summary>
        /// <returns>The currently selected line (null if a line isn't selected,
        /// or the selection refers to more than one thing)</returns>
        LineFeature GetLine() // was GetArc
        {
            return (this.Item as LineFeature);
        }

        /// <summary>
        /// Checks if a single item of text is selected.
        /// </summary>
        /// <returns>The currently selected text (null if a text label isn't selected,
        /// or the selection refers to more than one thing)</returns>
        TextFeature GetText() // was GetLabel
        {
            return (this.Item as TextFeature);
        }

        /// <summary>
        /// Checks if a single point is selected.
        /// </summary>
        /// <returns>The currently selected point (null if a point isn't selected,
        /// or the selection refers to more than one thing)</returns>
        PointFeature GetPoint()
        {
            return (this.Item as PointFeature);
        }

        /// <summary>
        /// Checks if a single polygon is selected.
        /// </summary>
        /// <returns>The currently selected polygon (null if a polygon isn't selected,
        /// or the selection refers to more than one thing)</returns>
        Polygon GetPolygon()
        {
            return (this.Item as Polygon);
        }

        /// <summary>
        /// Checks if a single spatial object is selected.
        /// </summary>
        /// <returns>The currently selected object (null if the selection refers to
        /// more than one thing)</returns>
        ISpatialObject GetObject()
        {
            return (this.Item as ISpatialObject);
        }

        /// <summary>
        /// Returns the feature ID (if any) that is associated with the currently
        /// selected feature. Multi-selections don't return anything.
        /// </summary>
        /// <returns>The selected ID.</returns>
        FeatureId GetId() // was SelPId
        {
            // Has to be just ONE object selected.
            ISpatialObject thing = this.Item;
            if (thing == null)
                return null;

            // Points, lines, and labels are all handled by the Feature class
            Feature feat = (thing as Feature);
            if (feat != null)
                return feat.Id;

            // Polygons aren't.
            Polygon pol = (thing as Polygon);
            if (pol != null)
                return pol.GetId();

            // We SHOULD have got something, but...
            return null;
        }

        /// <summary>
        /// Adds or removes an object to this selection. It will be added if it is
        /// not currently in the list. It will be removed if it was previously in
        /// the list.
        /// </summary>
        /// <param name="thing">The object to add or remove.</param>
        /// <returns>True if the object is in the selection at return.</returns>
        internal bool AddOrRemove(ISpatialObject thing)
        {
            // We don't accept null things.
            if (thing==null)
                return false;

            // A specific section can be drawn differently only
            // if it's a simple selection (handled via ReplaceWith).
            //base.Section = null;

            // If the thing is currently in the list, remove it.
            if (m_Selection.Remove(thing))
                return false;

            // If we currently have a single-item selected, un-highlight
            // it now. If we originally have a single line selected, it's
            // end points will be drawn in shades of blue. When we append
            // the extra object and go to re-highlight, however, the
            // end points will NOT get repainted (since end points don't
            // get drawn for multi-selects).

            if (IsSingle())
                UnHighlight(); //(null);

            // Append the thing to the list.
            Append(thing);
            ErasePainting();
            //ReHighlight();
            return true;
        }

        /// <summary>
        /// Has a limit line been started?
        /// </summary>
        internal bool HasLimit
        {
            get { return (m_Limit!=null && m_Limit.Count>0); }
        }

        /// <summary>
        /// Accepts a position that the user specified via a left click,
        /// while holding the CTRL key down.
        /// </summary>
        /// <param name="pos">Where did the user click?</param>
        internal void CtrlMouseDown(IPosition pos) // was LButton
        {
            // If we don't have any positions, just remember the supplied position
            // and create an empty limit line selection.
            if (m_Limit==null)
            {
                m_Limit = new List<IPosition>();
                m_Limit.Add(pos);
                m_LimSel = new List<ISpatialObject>();
                return;
            }

            // Erase any rubber band.
            //EraseBand();

            // Erase the current limit.
            //EraseLimit();

            // Append the new position to our list.
            m_Limit.Add(pos);

            // Draw the current limit.
            //DrawLimit();

            // But don't redraw any rubber banding. It comes back
            // as soon as the mouse moves.

            // Select stuff within the current limit.
            List<ISpatialObject> cutsel = new List<ISpatialObject>();
            SelectLimit(cutsel);

            // Undo the highlighting of any objects that are no longer selected.
            // Draw the ends, since we'll be re-highlighting.
            //CutHighlight(cutsel, null, true);

            // Ensure the current selection is highlighted.
            //ReHighlight();
            ErasePainting();
        }

        /// <summary>
        /// Accepts a mouse position while the user has the CTRL key pressed down.
        /// </summary>
        /// <param name="pos">The position of the mouse</param>
        internal void CtrlMouseMoveTo(IPosition pos) // was MouseMoveTo
        {
            // Just return if a left click hasn't been done yet.
            if (m_Limit==null)
                return;

            // Erase any previously drawn rubber banding.
            //EraseBand();

            // Hold on to the current mouse position.
            m_Mouse = pos;

            // Draw any rubber banding.
            //DrawBand();
            ErasePainting();
        }

        /// <summary>
        /// Frees any limit that has been specified. This is called whenever
        /// the user presses a mouse button without having the CTRL key
        /// pressed down.
        /// </summary>
        void FreeLimit()
        {
            // Nothing to do if no defined limit.
            if (m_Limit == null)
                return;

            // Erase any limit line we've drawn.
            //EraseLimit();

            // And erase any rubber banding.
            //EraseBand();

            // Get rid of the limit positions.
            m_Limit = null;

            // Reset last mouse position.
            m_Mouse = null;

            ErasePainting();
        }

        /// <summary>
        /// Draws any select limit.
        /// </summary>
        /*
        void Paint()
        {
            // Draw any limit line.
            DrawLimit();

            // Draw any rubber banding too.
            DrawBand();
        }
        */

        /// <summary>
        /// Draws any rubber band.
        /// </summary>
        /*
        void DrawBand()
        {
            // If there is no rubber band at the moment, invert it. But
            // take care to set m_IsBand true only if something actually
            // gets drawn.

            if (!m_IsBand)
                m_IsBand = InvertBand();
        }
        */

        /// <summary>
        /// Erases any rubber band.
        /// </summary>
        /*
        void EraseBand()
        {
            if (m_IsBand)
            {
                //InvertBand();
                EditingController.Current.ActiveDisplay.RestoreLastDraw();
                m_IsBand = false;
            }
        }
         */

        void ErasePainting()
        {
            EditingController.Current.ActiveDisplay.RestoreLastDraw();
        }

        /// <summary>
        /// Inverts rubber banding. If it was previously drawn, it will be erased. To get
        /// more positive control, make calls to DrawBand or EraseBand instead, since they
        /// ensure that the band doesn't get inverted when you don't expect it to invert.
        /// </summary>
        /// <returns></returns>
        /*
        bool InvertBand()
        {
            if (m_Limit!=null && m_Mouse!=null)
            {
                ISpatialDisplay draw = EditingController.Current.ActiveDisplay;
                IDrawStyle style = new DottedStyle();

                // Get the 2 ends of the current limit line
                int nVertex = m_Limit.Count;
                IPosition spt = m_Limit[0];
                IPosition ept = m_Limit[nVertex-1];

                style.Render(draw, new IPosition[] { spt, m_Mouse });

                if (nVertex > 1)
                    style.Render(draw, new IPosition[] { m_Mouse, ept });

                return true;
            }

            return false;
        }
         */

        /// <summary>
        /// Draws any limit line.
        /// </summary>
        /*
        void DrawLimit()
        {
            // If there is no limit line at the moment, invert it. But
            // take care to set m_IsLimit true only if something actually
            // gets drawn.

            if (!m_IsLimit)
                m_IsLimit = InvertLimit();
        }
         */

        /// <summary>
        /// Erases any limit line.
        /// </summary>
        /*
        void EraseLimit()
        {
            if (m_IsLimit)
            {
                //InvertLimit();
                EditingController.Current.ActiveDisplay.RestoreLastDraw();
                m_IsLimit= false;
            }
        }
         */

        /// <summary>
        /// Inverts the limit line. If it was previously drawn, it will be erased.
        /// To get more positive control, make calls to DrawLimit or EraseLimit instead,
        /// since they ensure that the limit doesn't get inverted when you don't expect
        /// it to invert.
        /// </summary>
        /// <returns></returns>
        bool InvertLimit()
        {
            if (m_Limit==null)
                return false;

            return true;
        }
        /*
	// Convert the positions into logical units.

	const UINT4 nVertex = m_pLimit->GetCount();
	CeDraw* pDraw = GetpDraw();
	CPoint* draw = new CPoint[nVertex+1];
	CPoint* pd = draw;

	for ( UINT4 i=0; i<nVertex; pd++, i++ ) {
		pDraw->GroundToLP(m_pLimit->GetPosition(i),pd);
	}

	// Always repeat the first position so the limit is closed.
	draw[nVertex] = draw[0];

	// Fill the limit with hatching.
	CClientDC dc(pDraw);
	dc.SetROP2(R2_NOT);
	if ( nVertex>2 )
		dc.Polyline(draw,nVertex+1);
	else
		dc.Polyline(draw,nVertex);

	// Get rid of the draw buffer
	delete [] draw;
	return TRUE;

} // end of InvertLimit
         */

        /// <summary>
        /// Selects stuff within the current limit line. This makes a private selection
        /// over and above any previous selection.
        /// </summary>
        /// <param name="cutsel">List of the objects that were removed from the the current
        /// limit selection as a consequence of making the new selection.</param>
        void SelectLimit(List<ISpatialObject> cutsel)
        {
            // Ensure the list of cut objects is initially clear.
            cutsel.Clear();

            // Nothing to do if there is no limit line.
            if (m_Limit==null)
                return;

            // Ensure list of objects to cut initially matches our
            // current limit selection.
            if (m_LimSel!=null)
                cutsel.AddRange(m_LimSel);

            // Empty out the current limit selection.
            m_LimSel = new List<ISpatialObject>();

            // Nothing to do if there's only one position.
            if (m_Limit.Count<=1)
                return;

            // If we have just 2 positions, select everything that
            // intersects the line. Otherwise select inside the shape.

            try
            {
                // Close the limit line.
                m_Limit.Add(m_Limit[0]);

                // Select only lines if the limit line consists of only 2 points (otherwise select
                // whatever is currently visible on the active display)
                SpatialType types = (m_Limit.Count==2 ? SpatialType.Line : EditingController.Current.VisibleFeatureTypes);

                // Make the selection.
                ISpatialIndex index = CadastralMapModel.Current.Index;
                List<ISpatialObject> res = new FindOverlapsQuery(index, m_Limit.ToArray(), types).Result;
                m_LimSel.AddRange(res);
            }

            catch
            {
            }

            finally
            {
                // Remove the closing point.
                int lastIndex = m_Limit.Count-1;
                m_Limit.RemoveAt(lastIndex);
            }

            // Go through the things we've got selected now, removing
            // them from the copy that we made up top of the original
            // selection. While we're at it accumulate a list of those
            // features that are not actually visible (the Select call
            // might have given us points and labels, even though they
            // might not be drawn by the view).

            //ISpatialDisplay view = EditingController.Current.ActiveDisplay;
        }
        /*
	CeObjectList invis;
	CeListIter loop(m_pLimSel);
	CeObject* pThing;

	CeView* pView = GetpView();
	const LOGICAL areLabelsDrawn = pView->AreLabelsDrawn();
	const LOGICAL arePointsDrawn = pView->ArePointsDrawn();

	for ( pThing = (CeObject*)loop.GetHead();
		  pThing;
		  pThing = (CeObject*)loop.GetNext() ) {

		// If both labels and points are drawn, what we
		// have is fine.

		if ( areLabelsDrawn && arePointsDrawn )
			cutsel.CutRef(*pThing);
		else {

			CeObject* pSel = pThing;

			// If we're not drawing labels, and we've got one,
			// get rid of it.
			if ( !areLabelsDrawn ) {
				CeLabel* pLabel = dynamic_cast<CeLabel*>(pSel);
				if ( pLabel ) {
					invis.Append(pLabel);
					pSel = 0;
				}
			}

			if ( pSel && !arePointsDrawn ) {
				CePoint* pPoint = dynamic_cast<CePoint*>(pSel);
				if ( pPoint ) {
					invis.Append(pPoint);
					pSel = 0;
				}
			}

			// If we didn't cut the thing, cut it from the
			// original list.
			if ( pSel ) cutsel.CutRef(*pSel);
		}
	} 

	// Remove any items from the selection that were invisible.
	*m_pLimSel -= invis;

} // end of SelectLimit
         */

        /// <summary>
        /// Arbitrarily discards any limit line (including any selection
        /// that has been made).
        /// </summary>
        void DiscardLimit()
        {
            // Free the limit itself.
            FreeLimit();

            if (m_LimSel!=null)
            {
                // Clear out the limit selection.
                //m_LimSel.Clear();
                m_LimSel = null;

                // Rehighlight the base selection (if any)
                //ReHighlight();
                ErasePainting();
            }
        }

        /// <summary>
        /// Grabs the current limit line selection and discards what we have here.
        /// </summary>
        internal Selection UseLimit()
        {
            // If there isn't any limit selection, just ensure that
            // the limit line has been freed.
            if (m_LimSel==null)
            {
                FreeLimit();
                return new Selection();
            }

            // Specific arc sections apply only to simple selections.
            //m_pSection = 0;

            // Add the limit selection to the base class.
            Selection result = new Selection(m_LimSel);

            // Discard the limit line and its selection,
            DiscardLimit();
            return result;
        }

        /*
//	@mfunc	Check if the selection is empty. This overrides the
//			version in the base class, to also take account of
//			any limit line selection that has not yet been used.

LOGICAL CeSelection::IsEmpty ( void ) const {

	// If we have a limit line selection and it's not empty,
	// that's the answer.
	if ( m_pLimSel && !m_pLimSel->IsEmpty() ) return FALSE;

	// The base class might have something though.
	return CeObjectList::IsEmpty();

} // end of IsEmpty
*/

        /// <summary>
        /// Do we just have one object selected? This refers to both the base
        /// class selection, as well as any limit line selection.
        /// </summary>
        /// <returns></returns>
        bool IsSingle()
        {
            // Get base class count. If more than one, that's us done.
            /*
            if (this.Item==null)
                return false;


            // If we've got a limit line selection, add that to the total.
            if (m_LimSel==null || m_LimSel.Count==0)
                return true;
            else
                return false;
             */
            return false;
        }

        void UnHighlight()
        {
        }
        /*
//	@mfunc	Make sure nothing is currently highlighted.
//
//	@parm	Pointer to the object (if any) that is about to
//			be selected. If this corresponds to something
//			that is currently highlighted, the current
//			highlighting will NOT be removed. Default=0,
//			meaning you definitely want to unhighlight
//			everything.

void CeSelection::UnHighlight ( const CeObject* const pNewSel ) const {

	// Return if nothing is selected.
	if ( this->IsEmpty() ) return;

	// Prepare a device context.
	CeView* pView = GetpView();
	CClientDC dc(pView);
	pView->OnPrepareDC(&dc);

	// If we're not going to be selecting anything new,
	// ensure that line end points get drawn. If you don't
	// do this, you get white shadows on top of the points
	// when you cancel a multi-select.
	LOGICAL drawEnds = (pNewSel==0);

	// Cut the highlighting of any limit line selection.
	CutHighlight(m_pLimSel,pNewSel,pView,dc,drawEnds);

	// Cut the base selection.
	CutHighlight(this,pNewSel,pView,dc,drawEnds);

} // end of UnHighlight
         */

        /*
        void CutHighlight(List<ISpatialObject> cutsel, ISpatialObject newSel, bool drawEnds)
        {
        }
         */
        /*
//	@mfunc	Eliminate highlighting. Does not change the
//			actual selection in any way.
//
//	@parm	The thing to cut the highlighting for. Could
//			either be a single object, or an objectlist.
//	@parm	Pointer to the object (if any) that is about to
//			be selected. If this corresponds to something
//			that is currently highlighted, the current
//			highlighting will NOT be removed. Default=0,
//			meaning you definitely want to unhighlight
//			everything.
//	@parm	Should line end points be redrawn normally?

void CeSelection::CutHighlight ( const CeClass* const pWhat
							   , const CeObject* const pNewSel 
							   , const LOGICAL drawEnds ) const {

	// Return if nothing has been specified.
	if ( !pWhat ) return;

	// Prepare a device context.
	CeView* pView = GetpView();
	CClientDC dc(pView);
	pView->OnPrepareDC(&dc);

	// Cut the highlighting.
	CutHighlight(pWhat,pNewSel,pView,dc,drawEnds);

} // end of CutHighlight
         */

        /*
//	@mfunc	Eliminate highlighting. Does not change the
//			actual selection in any way.
//
//	@parm	The thing to cut the highlighting for. Could
//			either be a single object, or an objectlist.
//	@parm	Pointer to the object (if any) that is about to
//			be selected. If this corresponds to something
//			that is currently highlighted, the current
//			highlighting will NOT be removed. Default=0,
//			meaning you definitely want to unhighlight
//			everything.
//	@parm	The view that will do the actual un-highlighting.
//	@parm	Device context to draw to.
//	@parm	Should line end points be redrawn normally?
//			Applies only to lines and polygons.

void CeSelection::CutHighlight ( const CeClass* const pWhat
							   , const CeObject* const pNewSel
							   , CeView* pView
							   , CDC& dc
							   , const LOGICAL drawEnds ) const {

	// Return if nothing has been specified.
	if ( !pWhat ) return;

	// Loop through everything, eliminating the highlighting.

	CeListIter loop(pWhat);
	CeObject* pThing;

	for ( pThing = (CeObject*)loop.GetHead();
		  pThing;
		  pThing = (CeObject*)loop.GetNext() ) {

		// Skip if it will be selected afterwards.
		if ( pThing == pNewSel ) continue;

		CePolygon* pPol = dynamic_cast<CePolygon*>(pThing);
		if ( pPol ) {

			// The 'drawEnds' is actually unused here.
			pView->UnHighlight((CClientDC*)&dc,pPol,drawEnds);
			continue;
		}

		CeArc* pArc = dynamic_cast<CeArc*>(pThing);
		if ( pArc ) {
			pView->UnHighlight((CClientDC*)&dc,pArc,drawEnds);
			continue;
		}

		CePoint* pPoint = dynamic_cast<CePoint*>(pThing);
		if ( pPoint ) {
			pView->UnHighlight((CClientDC*)&dc,pPoint);
			continue;
		}

		CeLabel* pLabel = dynamic_cast<CeLabel*>(pThing);
		if ( pLabel ) {
			pView->UnHighlight((CClientDC*)&dc,pLabel);
			continue;
		}
	}
} // end of CutHighlight
         */

        /// <summary>
        /// Ensures the current selection is highlighted.
        /// </summary>
        /*
        void ReHighlight()
        {
        }
         */
        /*
//	@parm	Device context that may already be know (by
//			CeView::OnDraw for example). May be NULL.
void CeSelection::ReHighlight ( CDC* pDC ) const {

	if ( pDC ) {

		// Need to pass down the view.
		CeView* pView = GetpView();

		// Do the re-highlighting of the base selection.
		ReHighlight(this,pView,*pDC);

		// As well as any limit line selection.
		ReHighlight(m_pLimSel,pView,*pDC);

		// Draw any arc section in yellow.
		if ( m_pSection ) m_pSection->DrawThis(COL_YELLOW);
	}
	else {

		// Prepare a device context.
		CeView* pView = GetpView();
		CClientDC dc(pView);
		pView->OnPrepareDC(&dc);

		// Do the re-highlighting of the base selection.
		ReHighlight(this,pView,dc);

		// As well as any limit line selection.
		ReHighlight(m_pLimSel,pView,dc);

		// Draw any arc section in yellow.
		if ( m_pSection ) m_pSection->DrawThis(COL_YELLOW);
	}

} // end of ReHighlight
         */

        /*
//	@mfunc	Ensure the current selection is highlighted.
//
//	@parm	The thing to re-highlight for. Could either
//			be a single object, or an objectlist.
//	@parm	The view that will do the actual un-highlighting.
//	@parm	Device context for the draw.

void CeSelection::ReHighlight ( const CeClass* const pWhat
							  , CeView* pView
							  , CDC& dc ) const {

	// Return if nothing has been specified.
	if ( !pWhat ) return;

	// Highlight any lines, labels, and polygons.

	CeListIter loop(pWhat);
	CeClass* pThing;

	for ( pThing = (CeClass*)loop.GetHead();
		  pThing;
		  pThing = (CeClass*)loop.GetNext() ) {

		CeArc* pArc = dynamic_cast<CeArc*>(pThing);
		if ( pArc ) {
			pView->Highlight((CClientDC*)&dc,pArc);
			continue;
		}

		CeLabel* pLabel = dynamic_cast<CeLabel*>(pThing);
		if ( pLabel ) {
			pView->Highlight((CClientDC*)&dc,pLabel);
			continue;
		}

		CePolygon* pPol = dynamic_cast<CePolygon*>(pThing);
		if ( pPol ) {
			pView->Highlight((CClientDC*)&dc,pPol);
			continue;
		}

	} // next item in selection

	// Now do any points. We do points seperately, since
	// a highlighted line can occlude the highlighting of a
	// point if the point comes first.

	for ( pThing = (CeClass*)loop.GetHead();
		  pThing;
		  pThing = (CeClass*)loop.GetNext() ) {

		CePoint* pPoint = dynamic_cast<CePoint*>(pThing);
		if ( pPoint ) {
			pView->Highlight((CClientDC*)&dc,pPoint);
			continue;
		}
	}

} // end of ReHighlight
         */

        /*
//	@mfunc	Arbitrarily discard everything that this
//			selection refers to. Don't even un-highlight.
//			This is called by <mf CeView::ResetContents>
//			when the user goes to open a different map.
void CeSelection::ResetContents ( void ) {

	// Get rid of any limit line stuff.
	delete m_pLimit;
	delete m_pLimSel;
	delete m_pMouse;

	// Clear out the base class.
	Remove();

	// Assign initial values to everything.
	SetZeroValues();

} // end of ResetContents
         */

        /*
//	@mfunc	Assign initial values to everything.
void CeSelection::SetZeroValues ( void ) {

	m_pLimit = 0;
	m_pMouse = new CeVertex();
	m_IsBand = FALSE;
	m_IsLimit = FALSE;
	m_pLimSel = 0;
	m_pSection = 0;

} // end of SetZeroValues
         */

        /// <summary>
        /// The one and only item in this selection (null if the selection is empty, or
        /// it contains more than one item).
        /// </summary>
        ISpatialObject Item
        {
            get { return (m_Selection.Count==1 ? m_Selection[0] : null); }
        }

        internal void Render(ISpatialDisplay display)
        {
            if (m_Limit==null || m_Limit.Count==0 || m_Mouse==null)
                return;

            // Draw dotted line from the last point on the limit line to the last known mouse position
            int lastIndex = m_Limit.Count-1;
            IPosition last = m_Limit[lastIndex];
            DottedStyle dottedLine = new DottedStyle(Color.Gray);
            dottedLine.Render(display, new IPosition[] { last, m_Mouse });

            // If we have two or more positions, draw an additional dotted line to the start of
            // the limit line.
            if (m_Limit.Count>=2)
                dottedLine.Render(display, new IPosition[] { m_Mouse, m_Limit[0] });

            // Draw the limit line
            dottedLine.Render(display, m_Limit.ToArray());

            // Draw any limit line selection
            if (m_LimSel!=null)
            {
                HighlightStyle style = new HighlightStyle();
                new SpatialSelection(m_LimSel).Render(display, style);
            }
        }
    }
}
