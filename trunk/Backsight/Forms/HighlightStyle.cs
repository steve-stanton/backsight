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

namespace Backsight.Forms
{
    /// <written by="Steve Stanton" on="02-OCT-2006" />
    /// <summary>
    /// Drawing of highlighted features (typically map features the user has selected)
    /// </summary>
    public class HighlightStyle : DrawStyle
    {
        #region Class data

        /// <summary>
        /// Should the end points of lines be highlighted (given that points are
        /// currently drawn).
        /// </summary>
        bool m_ShowLineEndPoints;

        #endregion

        #region Constructors

        public HighlightStyle()
        {
            base.Pen.Color = Color.Red;
            base.Pen.Width = 3.0F;

            (base.Brush as SolidBrush).Color = Color.Red;
            m_ShowLineEndPoints = true;
        }

        #endregion

        /// <summary>
        /// Should the end points of lines be highlighted (given that points are
        /// currently drawn). This is true by default. You may wish to turn it off
        /// when highlighting selections containing more than one feature, since
        /// highlighting the end points in that situation can give the impression
        /// that they too are selected.
        /// </summary>
        public bool ShowLineEndPoints
        {
            get { return m_ShowLineEndPoints; }
            set { m_ShowLineEndPoints = value; }
        }
    }
}
