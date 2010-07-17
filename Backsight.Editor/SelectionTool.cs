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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Backsight.Forms;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="01-NOV-1999" was="CeSelection" />
    /// <summary>
    /// Performs line-based and area-based selection of spatial features.
    /// This basically farms out the more complex selection stuff so that
    /// the controller class is a bit less cluttered.
    /// </summary>
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
        /// line has been completed (which occurs when the CTRL key is released).
        /// </summary>
        List<IPosition> m_Limit;

        /// <summary>
        /// The last mouse position.
        /// </summary>
        IPosition m_Mouse;

        /// <summary>
        /// The current selection obtained via m_Limit. This will be included
        /// in the selection when the limit line is completed. Never null.
        /// </summary>
        readonly List<ISpatialObject> m_LimSel;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>SelectionTool</c> for performing line- and area-
        /// based selections.
        /// </summary>
        /// <param name="controller">The controller making use of this tool (not null)</param>
        /// <exception cref="ArgumentNullException">If <paramref name="controller"/> is null</exception>
        internal SelectionTool(EditingController controller)
        {
            if (controller == null)
                throw new ArgumentNullException();

            m_Controller = controller;
            m_Limit = null;
            m_Mouse = null;
            m_LimSel = new List<ISpatialObject>();
        }

        #endregion

        /// <summary>
        /// The cursor that should be used when using the <c>SelectionTool</c> class.
        /// </summary>
        internal static Cursor Cursor
        {
            get { return EditorResources.DiagonalCursor; }
        }

        /// <summary>
        /// The features inside the limit line
        /// </summary>
        internal Selection Selection
        {
            get { return new Selection(m_LimSel); }
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
                m_LimSel.Clear();
            }
            else
            {
                // Append the new position to our list.
                m_Limit.Add(pos);

                // Select stuff within the current limit.
                SelectLimit();

                // Ensure the current selection is highlighted.
                ErasePainting();
            }
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
        /// Indicates that any painting previously done by this selection tool should be erased. This
        /// tells the controller's active display that it should revert the display buffer to
        /// the way it was at the end of the last draw from the map model. The controller should
        /// end up calling the <see cref="Render"/> method during idle time.
        /// </summary>
        void ErasePainting()
        {
            m_Controller.ActiveDisplay.RestoreLastDraw();
        }

        /// <summary>
        /// Selects stuff within the current limit line. This makes a private selection
        /// over and above any previous selection.
        /// </summary>
        void SelectLimit()
        {
            // Nothing to do if there is no limit line.
            if (m_Limit==null)
                return;

            // Empty out the current limit selection.
            m_LimSel.Clear();

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
        /// Draws the limit line (if it contains at least 2 positions)
        /// </summary>
        /// <param name="display">The display to draw to</param>
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
            if (m_Limit.Count>1)
                dottedLine.Render(display, m_Limit.ToArray());

            // Draw any limit line selection
            if (m_LimSel.Count>0)
            {
                HighlightStyle style = new HighlightStyle();
                style.ShowLineEndPoints = false;
                new SpatialSelection(m_LimSel).Render(display, style);
            }
        }
    }
}
