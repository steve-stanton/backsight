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
    /// Performs the selection of spatial features in a map. This basically
    /// farms out selection-related stuff so that the controller class is
    /// a bit less cluttered.
    /// </summary>
    /// <remarks>For the time being, this just handles CTRL style selections,
    /// though it would make sense to also extend this to the simpler style of
    /// selections.</remarks>
    class SelectionTool
    {
        #region Class data

        /// <summary>
        /// The controller that is making use of this tool.
        /// </summary>
        readonly EditingController m_Controller;

        /// <summary>
        /// The points defining the limits for selection area (if any).
        /// A new instance is created when a CTRL+MouseDown event initially occurs.
        /// On each further CTRL+MouseDown, a further position is appended to the
        /// list, and features are selected from the map. The features don't get
        /// instantly added to the "official" selection, since it's possible
        /// that further limit line positions will invalidate some of the initially
        /// selected features (consider a situation where the user ends up defining
        /// a concave shape). Instead, we hold on to the features until the limit
        /// line has been completed (which occurs when a MouseMove event occurs
        /// while the CTRL key is NOT pressed).
        /// </summary>
        List<IPosition> m_Limit;

        /// <summary>
        /// The last mouse position.
        /// </summary>
        IPosition m_Mouse;

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

        internal SelectionTool(EditingController controller)
            : base()
        {
            if (controller == null)
                throw new ArgumentNullException();

            m_Controller = controller;
            m_Limit = null;
            m_Mouse = null;
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
            }
        }

        /// <summary>
        /// Removes everything from this selection.
        /// </summary>
        internal void RemoveSel()
        {
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

            // Append the thing to the list.
            Append(thing);
            ErasePainting();
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

            // Append the new position to our list.
            m_Limit.Add(pos);

            // Select stuff within the current limit.
            List<ISpatialObject> cutsel = new List<ISpatialObject>();
            SelectLimit(cutsel);

            // Ensure the current selection is highlighted.
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

            // Hold on to the current mouse position.
            m_Mouse = pos;

            // Ensure current position is (apparently) rubber banded
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

            // Get rid of the limit positions.
            m_Limit = null;

            // Reset last mouse position.
            m_Mouse = null;

            ErasePainting();
        }

        void ErasePainting()
        {
            m_Controller.ActiveDisplay.RestoreLastDraw();
        }

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
                SpatialType types = (m_Limit.Count==2 ? SpatialType.Line : m_Controller.VisibleFeatureTypes);

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
        }

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
            m_Mouse = null;
            return result;
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

            // Draw the limit line (but not if it's just one point)
            if (m_Limit.Count>1)
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
