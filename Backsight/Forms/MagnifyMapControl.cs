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
using System.Windows.Forms;

namespace Backsight.Forms
{
	/// <written by="Steve Stanton" on="16-FEB-2007" />
    /// <summary>
    /// A map control that's used by the <c>MagnifyTool</c>. This is a bit of a hack,
    /// since all it does is remove the scrollbars that are part of a normal map control,
    /// and provides overrides to ensure their removal doesn't lead to null reference
    /// exceptions.
    /// 
    /// It would be nicer to provide a MapControl that doesn't have scrollbars (which
    /// the magnifier would use), then have a derived class that also has scrollbars
    /// (for use in normal draws).
    /// </summary>
    public partial class MagnifyMapControl : MapControl
    {
        public MagnifyMapControl()
        {
            InitializeComponent();
            RemoveScrollBars(new Size(100,100));
            SetMapBackground(Color.White);
        }

        internal override void SetScrollBars()
        {
            // since the constructor removed them, it's a good idea to do nothing
        }
    }
}

