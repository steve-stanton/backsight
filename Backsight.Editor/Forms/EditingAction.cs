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

using Backsight.Forms;

namespace Backsight.Editor.Forms
{
	/// <written by="Steve Stanton" on="02-FEB-2007" />
    /// <summary>
    /// An editing action is a <c>UserAction</c> that's associated with a numeric ID.
    /// </summary>
    class EditingAction : UserAction, IEditingAction
    {
        #region Class data

        private readonly EditingActionId m_Id;

        #endregion

        #region Constructors

        public EditingAction(EditingActionId id, ToolStripItem[] items, IsActionEnabled isActionEnabled, DoAction doAction)
            : base(items, isActionEnabled, doAction)
        {
            m_Id = id;
        }

        #endregion

        public EditingActionId EditId
        {
            get { return m_Id; }
        }
    }
}
