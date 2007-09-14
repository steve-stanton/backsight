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

namespace Backsight.Forms
{
	/// <written by="Steve Stanton" on="04-OCT-2006" />
    /// <summary>A user action that initiates one of the display tools provided by the
    /// <c>MapControl</c> class</summary>
    /// <see>MapControl</see>
    public class MapControlAction : IUserAction
    {
        #region Class data

        private readonly MapControl m_Control;
        private readonly DisplayToolId m_ToolId;
        private readonly UserActionSupport m_Elements;

        #endregion

        #region Constructors

        public MapControlAction(MapControl control, DisplayToolId id, ToolStripItem[] items)
        {
            m_Control = control;
            m_ToolId = id;
            m_Elements = new UserActionSupport(items);
            m_Elements.SetHandler(Do);
        }

        #endregion

        public void Update()
        {
            m_Elements.Enabled = m_Control.IsEnabled(m_ToolId);            
        }

        private void Do(object sender, EventArgs e)
        {
            m_Control.Do(m_ToolId);
        }

        public string Title { get { return m_Elements.Title; } }
    }
}
