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
using System.Drawing.Drawing2D;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" on="22-MAR-2002" was="CeStyle"/>
    /// <summary>
    /// The display style for a feature.
    /// </summary>
    class Style
    {
        #region Class data

        /// <summary>
        /// The display colour
        /// </summary>
        Color m_Color;

        /// <summary>
        /// The pen used to draw lines with this style
        /// </summary>
        ScaleSpecificPen m_Pen;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>Style</c> with the specified color (does not create the pen object).
        /// </summary>
        /// <param name="col">The display colour</param>
        internal Style(Color col)
        {
            m_Color = col;
            m_Pen = null;
        }

        #endregion

        /// <summary>
        /// The display colour
        /// </summary>
        internal Color Color
        {
            get { return m_Color; }
        }

        /// <summary>
        /// Is this style dependent on scale? This implementation always returns <c>false</c> (the
        /// <see cref="LineStyle"/> class overrides).
        /// </summary>
        internal virtual bool IsScaleDependent
        {
            get { return false; }
        }

        /// <summary>
        /// Does this style match the supplied values
        /// </summary>
        /// <param name="col">The display colour</param>
        /// <returns></returns>
        internal bool Equals(Color col)
        {
            return m_Color.Equals(col);
        }

        /// <summary>
        /// Does this style match the supplied style? The scale at which any pen
        /// has been defined gets ignored.
        /// </summary>
        /// <param name="style">The style to compare with</param>
        /// <returns>True if the supplied style is identical to this one.</returns>
        internal virtual bool Equals(Style style)
        {
            if (style==null)
                return false;

            if (Object.ReferenceEquals(style, this))
                return true;

            // The display color must match
            return m_Color.Equals(style.Color);
        }

        /// <summary>
        /// Defines the supplied pen with this style This implementation just defines a solid pen
        /// with this style's color and a width of 1 pixel. The <see cref="LineStyle"/> class
        /// provides an override that does more fancy pens.
        /// </summary> 
        /// <param name="pen">The pen to define</param>
        /// <param name="draw">The definition of the draw (not used)</param>
        internal virtual void DefinePen(ScaleSpecificPen pen, ISpatialDisplay draw)
        {
            DefinePen(pen);
        }

        /// <summary>
        /// Defines the supplied pen with this style This implementation just defines a solid pen
        /// with this style's color and a width of 1 pixel.
        /// </summary> 
        /// <param name="pen">The pen to define</param>
        internal void DefinePen(ScaleSpecificPen pen)
        {
        	//pen.CreatePen(PS_SOLID,0,m_Colour);
            pen.CreateSolidPen(0, m_Color);
        }

        /// <summary>
        /// Ensures this style has a pen that is compatible with the supplied draw object.
        /// </summary>
        /// <param name="draw">The draw object</param>
        /// <returns>The corresponding pen</returns>
        internal ScaleSpecificPen GetPen(ISpatialDisplay draw)
        {
            if (IsScaleDependent)
            {
                if (m_Pen!=null && m_Pen.IsCompatible(draw))
                    return m_Pen;

                m_Pen = new ScaleSpecificPen(this, draw);
                return m_Pen;
            }

            if (m_Pen == null)
                m_Pen = new ScaleSpecificPen(this, draw);

            return m_Pen;

        }
    }
}
