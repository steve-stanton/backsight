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

namespace Backsight.Forms
{
    class PanTool : SpatialDisplayTool
    {
        IPosition m_LastPanPosition;

        public PanTool(MapControl display)
            : base(display)
        {
        }

        public override int Id
        {
            get { return (int)DisplayToolId.Pan; }
        }

        public override bool Start()
        {
            this.MapControl.SetCursor(MapResources.StartPanCursor);
            m_LastPanPosition = null;
            return true;
        }

        public override void MouseDown(IPosition p, MouseButtons b)
        {
            // If doing a pan, remember the current position and switch to the moving
            // car (the pan will be cancelled with a left button up event).
            if (b == MouseButtons.Left)
            {
                m_LastPanPosition = p;
                this.MapControl.SetCursor(MapResources.PanCursor);
            }
        }

        public override void MouseUp(IPosition p, MouseButtons b)
        {
            if (b == MouseButtons.Left)
            {
                m_LastPanPosition = null;
                this.MapControl.SetCursor(MapResources.StartPanCursor);
                this.MapControl.Draw(true);
            }
            else
                Finish();
        }

        public override void MouseMove(IPosition p, MouseButtons b)
        {
            if (b == MouseButtons.Left && m_LastPanPosition!=null)
            {
                double dx = p.X - m_LastPanPosition.X;
                double dy = p.Y - m_LastPanPosition.Y;
                if (this.MapControl.MoveMap(dx, dy))
                {
                    this.MapControl.PaintNow();
                    m_LastPanPosition = p;
                }
            }
        }
    }
}
