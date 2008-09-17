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

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" on="01-MAY-2002" was="CePen"/>
    /// <summary>
    /// Wrapper on a <see cref="System.Drawing.Pen"/> that holds the draw scale it was defined for.
    /// </summary>
    class ScaleSpecificPen
    {
        #region Class data

        /// <summary>
        /// The wrapped pen
        /// </summary>
        Pen m_Pen;

        /// <summary>
        /// The draw scale this pen was created for.
        /// </summary>
        int m_Scale;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>ScaleSpecificPen</c>
        /// </summary>
        /// <param name="style">The style that makes reference to this pen</param>
        /// <param name="draw">The drawing tool that holds the current scale</param>
        internal ScaleSpecificPen(Style style, ISpatialDisplay draw)
        {
            m_Pen = new Pen(Color.Black);
            m_Scale = (int)draw.MapScale;
            style.DefinePen(this, draw);
        }

        #endregion

        /// <summary>
        /// The wrapped pen
        /// </summary>
        internal Pen Pen
        {
            get { return m_Pen; }
        }

        /// <summary>
        /// The color of the wrapped pen
        /// </summary>
        internal Color Color
        {
            get { return m_Pen.Color; }
        }

        /// <summary>
        /// Ensures the current pen is a solid pen with the specified width and color.
        /// </summary>
        /// <param name="width">The width of the pen (in pixels)</param>
        /// <param name="col">The color for the pen</param>
        internal void CreateSolidPen(float width, Color col)
        {
            // The pen is always solid!
            m_Pen.Width = width;
            m_Pen.Color = col;
        }

        /// <summary>
        /// The draw scale this pen was created for.
        /// </summary>
        internal int Scale
        {
            get { return m_Scale; }
        }

        /// <summary>
        /// Is this pen compatible with the supplied draw object?
        /// </summary>
        /// <param name="draw">The draw to check</param>
        /// <returns>True if the specified draw has the same scale as this pen</returns>
        internal bool IsCompatible(ISpatialDisplay draw)
        {
            return ((int)draw.MapScale == m_Scale);
        }
    }
}
