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
using System.Collections.Generic;
using System.Windows.Forms;

using Backsight.Forms;

namespace Backsight
{
    public class SpatialController : ISpatialController
    {
        #region Statics

        /// <summary>
        /// The one (and only) controller.
        /// </summary>
        private static ISpatialController s_Controller = new DesignTimeController();

        public static ISpatialController Current
        {
            get { return s_Controller; }
        }

        #endregion

        #region Class data

        private ISpatialModel m_Data;

        /// <summary>
        /// The currently selected elements (may be empty, but never null)
        /// </summary>
        private ISpatialSelection m_Selection;

        /// <summary>
        /// Map displays that have been registered with this controller (see <c>Register</c> method)
        /// </summary>
        private readonly List<ISpatialDisplay> m_Displays;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a <c>SpatialController</c> with a map model that's suitable
        /// for use at design time.
        /// </summary>
        public SpatialController()
        {
            s_Controller = this;

            m_Data = new DesignTimeMapModel();
            m_Selection = new SpatialSelection();
            m_Displays = new List<ISpatialDisplay>();

            // Initialize map model in case any of the prelim stuff needs it (the
            // running application needs to replace it with something more appropriate).
            SetMapModel(m_Data, null);
        }

        #endregion

        public virtual void Close()
        {
            m_Data = null;
            m_Selection = new SpatialSelection();
        }

        public ISpatialModel MapModel
        {
            get
            {
                if (m_Data==null)
                    m_Data = new DesignTimeMapModel();

                return m_Data;
            }

            set { SetMapModel(value, null); }
        }

        protected void SetMapModel(ISpatialModel model, IWindow initialDrawExtent)
        {
            m_Data = model;
            SetSelection(null);

            foreach (ISpatialDisplay display in m_Displays)
                display.ReplaceMapModel(initialDrawExtent);

            // Ensure the active display has focus (so that it reacts to any mouse-wheel events)
            ISpatialDisplay ad = ActiveDisplay;
            if (ad!=null)
                ad.MapPanel.Focus();
        }

        public void Register(ISpatialDisplay display)
        {
            if (!m_Displays.Contains(display))
                m_Displays.Add(display);

            // Redraw the display now (ensures the background has the expected colour)
            //display.Redraw();
        }

        public void Unregister(ISpatialDisplay display)
        {
            m_Displays.Remove(display);
        }

        public virtual void MouseDown(ISpatialDisplay sender, IPosition p, MouseButtons b)
        {
            if (b==MouseButtons.Right)
                ShowContextMenu(sender, p);
            else
                Select(sender, p, SpatialType.All);
        }

        /// <summary>
        /// Placeholder that displays a context menu. This implementation does nothing, since
        /// the relevant context menu depends on the functionality provided by the application
        /// that utilizes this controller. Derived classes (ones that know something about the
        /// application) are expected to override.
        /// </summary>
        /// <param name="where">The display where the context menu should appear</param>
        /// <param name="p">The preferred position for the menu (may not be honoured if
        /// the context menu would be obscured)</param>
        public virtual void ShowContextMenu(ISpatialDisplay where, IPosition p)
        {
        }

        public virtual void MouseUp(ISpatialDisplay sender, IPosition p, MouseButtons b)
        {
        }

        public virtual void MouseMove(ISpatialDisplay sender, IPosition p, MouseButtons b)
        {
        }

        public virtual void KeyDown(ISpatialDisplay sender, KeyEventArgs k)
        {
        }

        public virtual void Select(ISpatialDisplay display, IPosition p, SpatialType spatialType)
        {
            if (m_Data==null)
                return;

            // Use a tolerance of 2mm at the map scale of the supplied display
            ILength size = new Length(0.002 * display.MapScale);

            ISpatialObject so = m_Data.QueryClosest(p, size, spatialType);
            SetSelection(new SpatialSelection(so));
        }

        /// <summary>
        /// The currently selected elements (may be empty, but never null)
        /// </summary>
        public ISpatialSelection SpatialSelection
        {
            get { return m_Selection; }
        }

        /// <summary>
        /// Remembers a new selection
        /// </summary>
        /// <param name="newSel">The new selection (specify null to clear any current selection)</param>
        public virtual void SetSelection(ISpatialSelection newSel)
        {
            ISpatialSelection ss = (newSel==null ? new SpatialSelection() : newSel);

            if (!m_Selection.Equals(ss))
            {
                // Meant to ensure that any previous selection will be unhighlighted (and
                // the new one highlighted)
                foreach (ISpatialDisplay d in m_Displays)
                    d.OnSelectionChanging(m_Selection, ss);

                m_Selection = new SpatialSelection(ss.Items);
            }
        }

        public virtual IDrawStyle DrawStyle
        {
            get { return new DrawStyle(); }
        }

        public virtual IDrawStyle HighlightStyle
        {
            get { return new HighlightStyle(); }
        }

        public virtual void RefreshAllDisplays()
        {
            foreach (ISpatialDisplay d in m_Displays)
                d.Redraw();
        }

        public ISpatialDisplay ActiveDisplay
        {
            get { return (m_Displays.Count==0 ? null : m_Displays[0]); }
        }

        /// <summary>
        /// Perform any processing whenever a display has changed the drawn extent
        /// of a map. This implementation does nothing, derived classes may override.
        /// </summary>
        /// <param name="sender">The display that has changed</param>
        public virtual void OnSetExtent(ISpatialDisplay sender)
        {
            // do nothing
        }

        protected void RedrawSelection()
        {
            if (m_Selection.Count>0)
            {
                foreach (ISpatialDisplay d in m_Displays)
                    d.OnSelectionChanging(m_Selection, m_Selection);
            }
        }
    }
}
