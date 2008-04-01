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

using Backsight.Editor.Forms;
using Backsight.Forms;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="28-MAR-1999" was="CuiNewText"/>
    /// <summary>
    /// User interface for adding a new item of miscellaneous text.
    /// </summary>
    class NewTextUI : SimpleCommandUI
    {
        #region Class data

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>NewTextUI</c>
        /// </summary>
        /// <param name="cc"></param>
        /// <param name="action"></param>
        internal NewTextUI(IControlContainer cc, IUserAction action)
            : base(cc, action)
        {
            //m_Dialog = null;
        }

        #endregion

        internal override bool Run()
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
