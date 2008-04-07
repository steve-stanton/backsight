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
    /// <written by="Steve Stanton" on="06-APR-2008" />
    /// <summary>
    /// User action for specifying text magnification factor (for use with <see cref="NewTextContentMenu"/>).
    /// </summary>
    class TextSizeAction : UserAction
    {
        #region Class data

        /// <summary>
        /// The magnification factor (as a percentage of normal size)
        /// </summary>
        readonly int m_SizeFactor;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>TextSizeAction</c>
        /// </summary>
        /// <param name="item">The associated UI element (not null)</param>
        /// <param name="doAction">Delegate that should be called to perform the action (not null)</param>
        /// <param name="sizeFactor">The magnification factor (as a percentage of normal size)</param>
        internal TextSizeAction(ToolStripItem item, DoAction doAction, int sizeFactor)
            : base(item, doAction)
        {
            m_SizeFactor = sizeFactor;
        }

        #endregion

        /// <summary>
        /// The magnification factor (as a percentage of normal size)
        /// </summary>
        internal int SizeFactor
        {
            get { return m_SizeFactor; }
        }
    }
}
