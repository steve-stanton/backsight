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

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="01-NOV-1999" was="CeSelection" />
    /// <summary>
    /// Performs the selection of spatial features in a map.
    /// </summary>
    class SelectionTool : Selection
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
        bool m_IsBand;

        /// <summary>
        /// True if limit is currently drawn.
        /// </summary>
        bool m_IsLimit;

        /// <summary>
        /// The current selection obtained via m_Limit. This will be included
        /// in the base class when the limit line is completed.
        /// </summary>
        List<ISpatialObject> m_LimSel;

        #endregion

        #region Constructors

        internal SelectionTool()
            : base()
        {
            m_Limit = null;
            m_Mouse = null;
            m_IsBand = false;
            m_IsLimit = false;
            m_LimSel = null;
        }

        #endregion

        /// <summary>
        /// Replaces the current selection with a specific object.
        /// </summary>
        /// <param name="thing">The object to select</param>
        internal void ReplaceWith(ISpatialObject thing)
        {
            ReplaceWith(thing, null);
        }

        /// <summary>
        /// Replaces the current selection with a specific object (possibly
        /// a section of a line)
        /// </summary>
        /// <param name="thing">The object to select</param>
        /// <param name="section">The section of a line object to select (null if the
        /// complete object should be selected)</param>
        internal void ReplaceWith(ISpatialObject thing, IDivider section)
        {
            // Clear out the current selection.
            RemoveSel();

            // Append the thing to the selection (so long as it
            // has a defined value)
            if (thing != null)
            {
                Add(thing);
                base.Section = section;
            }
        }

        /// <summary>
        /// Removes everything from this selection.
        /// </summary>
        void RemoveSel()
        {
            // Clear out the saved selection (in the base class).
            base.Clear();

            // Make sure we have no limit-line selection either.
            DiscardLimit();
        }

        /// <summary>
        /// Checks if a single line is selected.
        /// </summary>
        /// <returns>The currently selected line (null if a line isn't selected,
        /// or the selection refers to more than one thing)</returns>
        LineFeature GetLine()
        {
            return (this.Item as LineFeature);
        }

        /// <summary>
        /// Checks if a single item of text is selected.
        /// </summary>
        /// <returns>The currently selected text (null if a text label isn't selected,
        /// or the selection refers to more than one thing)</returns>
        TextFeature GetText()
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
            base.Section = null;

            // If the thing is currently in the list, remove it.
            if (Remove(thing))
                return false;

            // If we currently have a single-item selected, un-highlight
            // it now. If we originally have a single line selected, it's
            // end points will be drawn in shades of blue. When we append
            // the extra object and go to re-highlight, however, the
            // end points will NOT get repainted (since end points don't
            // get drawn for multi-selects).
            //if (IsSingle())

            return true;
        }
        /*

	if ( IsSingle() ) UnHighlight(0);

	// Append the thing to the list.
	if ( Append(pThing)==0 ) return FALSE;

	// Ensure everything is highlighted (bit of brute force).
	ReHighlight();
	return TRUE;

} // end of AddOrRemove
         */

        /// <summary>
        /// Arbitrarily discards any limit line (including any selection
        /// that has been made).
        /// </summary>
        void DiscardLimit()
        {
            // Free the limit itself.
            FreeLimit();

            // Clear out the limit selection.
            m_LimSel.Clear();
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
            EraseLimit();

            // And erase any rubber banding.
            EraseBand();

            // Get rid of the limit positions.
            m_Limit = null;

            // Reset last mouse position.
            m_Mouse = null;
        }

        /// <summary>
        /// Erases any rubber band.
        /// </summary>
        void EraseBand()
        {
            if (m_IsBand)
            {
                EditingController.Current.ActiveDisplay.RestoreLastDraw();
                m_IsBand = false;
            }
        }

        /// <summary>
        /// Erases any limit line.
        /// </summary>
        void EraseLimit()
        {
            if (m_IsLimit)
            {
                EditingController.Current.ActiveDisplay.RestoreLastDraw();
                m_IsLimit = false;
            }
        }

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
    }
}
