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
using System.Drawing;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" on="23-NOV-2007" />
    /// <summary>
    /// An association between a map position and an icon
    /// </summary>
    class PositionedIcon
    {
        #region Class data

        /// <summary>
        /// The icon involved
        /// </summary>
        readonly Icon m_Icon;

        /// <summary>
        /// The position of the center of the icon
        /// </summary>
        readonly IPosition m_Position;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>PositionedIcon</c>
        /// </summary>
        /// <param name="p">The position of the center of the icon (not null)</param>
        /// <param name="i">The icon involved (not null)</param>
        /// <exception cref="ArgumentNullException">If either the supplied position or the icon are null</exception>
        internal PositionedIcon(IPosition p, Icon i)
        {
            if (p==null || i==null)
                throw new ArgumentNullException();

            m_Icon = i;
            m_Position = p;
        }

        #endregion

        /// <summary>
        /// Draws the icon on the specified display
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The style for doing the draw</param>
        internal void Render(ISpatialDisplay display, IDrawStyle style)
        {
            style.Render(display, m_Position, m_Icon);
        }

        /// <summary>
        /// The position of the center of the icon
        /// </summary>
        internal IPosition Position
        {
            get { return m_Position; }
        }
    }
}
