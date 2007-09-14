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

namespace Backsight
{
    /// <summary>
    /// A controller that's suitable for use in Visual Studio design mode.
    /// </summary>
    public class DesignTimeController : ISpatialController
    {
        private readonly ISpatialModel m_Model;

        public DesignTimeController()
        {
            m_Model = new DesignTimeMapModel();
        }

        public ISpatialModel MapModel { get { return m_Model; } }
        public void Register(ISpatialDisplay display) { }
        public void Unregister(ISpatialDisplay display) { }
        public void MouseDown(ISpatialDisplay sender, IPosition p, MouseButtons b) { }
        public void MouseUp(ISpatialDisplay sender, IPosition p, MouseButtons b) { }
        public void MouseMove(ISpatialDisplay sender, IPosition p, MouseButtons b) { }
        public ISpatialSelection Selection { get { return null; } }
        public IDrawStyle DrawStyle { get { return null; } }
        public IDrawStyle HighlightStyle { get { return null; } }
        public ISpatialDisplay ActiveDisplay { get { return null; } }
        public void OnSetExtent(ISpatialDisplay sender) { }
        public void KeyDown(ISpatialDisplay sender, KeyEventArgs k) {}
    }
}
