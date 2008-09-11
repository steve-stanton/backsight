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
    /// <summary>
    /// Tool to define a new map centre
    /// </summary>
    class NewCenterTool : SpatialDisplayTool
    {
        public NewCenterTool(MapControl mapControl) : base(mapControl)
        {
        }

        public override int Id
        {
            get { return (int)DisplayToolId.NewCentre; }
        }

        public override bool Start()
        {
            this.MapControl.SetCursor(MapResources.NewCenterCursor);
            return true;
        }

        public override void MouseDown(IPosition p, MouseButtons b)
        {
            this.MapControl.SetCenter(p);
            Finish();
        }
    }
}
