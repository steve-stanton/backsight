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
    abstract class SpatialDisplayTool : ISpatialDisplayTool
    {
        private readonly MapControl m_MapControl;

        protected MapControl MapControl { get { return m_MapControl; } }

        protected SpatialDisplayTool(MapControl mapControl)
        {
            if (mapControl==null)
                throw new ArgumentNullException();

            m_MapControl = mapControl;
        }

        abstract public int Id { get; }
        abstract public bool Start();

        public virtual bool Finish()
        {
            m_MapControl.Finish(this);
            return true;
        }

        public virtual void MouseDown(IPosition p, MouseButtons b)
        {
        }

        public virtual void MouseUp(IPosition p, MouseButtons b)
        {
        }

        public virtual void MouseMove(IPosition p, MouseButtons b)
        {
        }

        public virtual void MouseWheel(int delta, Keys k)
        {
        }

        public virtual void Escape()
        {
            m_MapControl.Escape(this);
        }
    }
}
