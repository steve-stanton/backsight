// <remarks>
// Copyright 2007 - Steve Stanton. This file is part of Backsight
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
using System.Windows.Forms;
using System.Drawing;


namespace Backsight.Forms
{
    class ZoomRectangleTool : SpatialDisplayTool
    {
        /// <summary>
        /// First corner of the rectangle (anchor point)
        /// </summary>
        IPosition m_FirstCorner;

        /// <summary>
        /// Second corner of the rectangle
        /// </summary>
        IPosition m_SecondCorner;

        /// <summary>
        /// Rectange corresponding to the current corners, in screen units (empty if
        /// rectangle is currently invisible)
        /// </summary>
        Rectangle m_Rect;


        public ZoomRectangleTool(MapControl display)
            : base(display)
        {
        }

        public override int Id
        {
            get { return (int)DisplayToolId.ZoomRectangle; }
        }

        public override bool Start()
        {
            this.MapControl.SetCursor(MapResources.MagnifyingGlassCursor);
            return true;
        }

        public override void MouseDown(IPosition p, MouseButtons b)
        {
            RectBegin(p);
        }

        /// <summary>
        /// Remember the anchor point for a zoom by rectangle.
        /// </summary>
        /// <param name="p">The position where the left click occurred</param>
        void RectBegin(IPosition p)
        {
            m_FirstCorner = p;
        }

        public override void MouseMove(IPosition p, MouseButtons b)
        {
            if (b == MouseButtons.Left)
            {
                RectUpdate(p);
            }
        }

        /// <summary>
        /// Remember the latest point defining a zoom by rectangle.
        /// This should be called by a MouseMove handler.
        /// </summary>
        /// <param name="p">The updated position</param>
        void RectUpdate(IPosition p)
        {
            MapControl display = this.MapControl;

            // Remember the second corner for the next draw (the first
            // corner was defined by RectBegin).
            m_SecondCorner = p;

            // If we previously drew a rectangle, get rid of it by
            // re-drawing it so that it disappears
            if (!m_Rect.IsEmpty)
            {
                display.DrawReversibleFrame(m_Rect);
                m_Rect = Rectangle.Empty;
            }

            // Create new rectangle and draw it
            Window x = new Window(m_FirstCorner, m_SecondCorner);
            m_Rect = display.DrawReversibleFrame(x);
        }

        public override void MouseUp(IPosition p, MouseButtons b)
        {
            if (b == MouseButtons.Left)
            {
                RectFinish();
            }
        }

        /// <summary>
        /// Finishes off a zoom by rectangle
        /// </summary>
        void RectFinish()
        {
            // Only do stuff if a rectangle has been defined
            if (!m_Rect.IsEmpty)
            {
                // Make the rectangle invisible
                MapControl display = this.MapControl;
                display.DrawReversibleFrame(m_Rect);
                m_Rect = Rectangle.Empty;

                // Define the new draw window
                display.SetNewWindow(new Window(m_FirstCorner, m_SecondCorner), true);
            }

            Finish();
        }
    }
}
