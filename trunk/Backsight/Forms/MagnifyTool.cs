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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Backsight.Forms
{
	/// <written by="Steve Stanton" on="15-FEB-2007" />
    /// <summary>
    /// Tool for showing a small zoomed area at the current cursor position.
    /// </summary>
    class MagnifyTool : SpatialDisplayTool
    {
        #region Class data

        /// <summary>
        /// The window that will hold the magnified display
        /// </summary>
        private MagnifyForm m_Window;

        //private Point[] m_Connection;

        /// <summary>
        /// The current magnification scale
        /// </summary>
        private double m_Scale;

        private bool m_IsStarted;

        /// <summary>
        /// The position where the last right click occurred.
        /// </summary>
        private IPosition m_RightClickPosition;

        #endregion

        public MagnifyTool(MapControl mapControl) : base(mapControl)
        {
            m_Scale = mapControl.MapScale/20.0;
            m_IsStarted = false;
        }

        public override int Id
        {
            get { return (int)DisplayToolId.Magnify; }
        }

        public override bool Start()
        {
            m_Window = new MagnifyForm(this, m_Scale);
            m_Window.Size = new Size(this.MapControl.Width/5, this.MapControl.Height/5);
            m_Window.StartPosition = FormStartPosition.Manual;
            Point screenPos = Cursor.Position;
            AdjustLocation(ref screenPos);
            m_Window.Location = screenPos;
            m_Window.Visible = true;
            this.MapControl.SetCursor(MapResources.HollowSquareCursor);
            m_IsStarted = true;
            return true;
        }

        private void CreateConnection()
        {
            // I keep getting little shadows near the corners of the MagnifyForm,
            // so do without the connecting lines for now...

            /*
            Point from = Cursor.Position; // centre of cursor
            Point to = m_Window.Location; // top left corner of display

            m_Connection = new Point[6];

            m_Connection[0].X = from.X - 16;
            m_Connection[0].Y = from.Y - 16;
            m_Connection[1].X = to.X - 3;
            m_Connection[1].Y = to.Y;

            m_Connection[2].X = from.X - 16;
            m_Connection[2].Y = from.Y + 16;
            m_Connection[3].X = to.X - 3;
            m_Connection[3].Y = to.Y + m_Window.Height + 3;

            m_Connection[4].X = from.X + 16;
            m_Connection[4].Y = from.Y + 16;
            m_Connection[5].X = to.X + m_Window.Width;
            m_Connection[5].Y = to.Y + m_Window.Height + 3;
             */
        }

        /*
        private void DrawConnection()
        {
            if (m_Connection!=null)
            {
                Color col = this.MapControl.BackColor;

                for (int i=1; i<m_Connection.Length; i+=2)
                    ControlPaint.DrawReversibleLine(m_Connection[i-1], m_Connection[i], col);
            }
        }
        */

        public override void MouseMove(IPosition p, MouseButtons b)
        {
            if (!m_IsStarted)
                return;

            //DrawConnection();
            MapControl display = this.MapControl;
            int x = (int)display.EastingToDisplay(p.X);
            int y = (int)display.NorthingToDisplay(p.Y);
            Point screenPos = display.PointToScreen(new Point(x, y));
            AdjustLocation(ref screenPos);
            m_Window.Location = screenPos;
            m_Window.Draw(p, m_Scale);
            CreateConnection();
            //DrawConnection();
        }

        private void AdjustLocation(ref Point screenPos)
        {
            screenPos.X += 25; // right
            screenPos.Y -= (m_Window.Height + 35); // above
        }

        public override void MouseDown(IPosition p, MouseButtons b)
        {
            if (b == MouseButtons.Right)
            {
                // Remember the position where the right click occurred so that
                // it can be used when reacting to menu events (as things stand,
                // I believe mouse move events still get intercepted somewhere,
                // meaning we might have the wrong position when it comes to
                // things like a ZoomIn command).

                m_RightClickPosition = p;
                MapControl.ShowContextMenu(p, m_Window.ContextMenuStrip);
            }
            else
            {
                // Escape first; otherwise the magnifier window may be saved as
                // part of the screen display when we redraw.
                Escape();
                this.MapControl.SetCenterAndScale(p, m_Scale, true);
            }
        }

        public override void MouseWheel(int delta, Keys k)
        {
            //bool isAlt = (k & Keys.Alt)!=0;
            m_Scale = m_Window.MouseWheel(delta);
        }

        internal void ZoomIn(double factor)
        {
            m_Scale = m_Window.ZoomIn(factor);
        }

        internal void ZoomOut(double factor)
        {
            m_Scale = m_Window.ZoomOut(factor);
        }

        public override void Escape()
        {
            if (m_Window!=null)
            {
                //DrawConnection();
                m_Window.Close();
                m_Window.Dispose();
                m_Window = null;
            }

            base.Escape();
        }

        internal IPosition RightClickPosition
        {
            get { return m_RightClickPosition; }
        }
    }
}
