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
using System.Drawing.Drawing2D;

namespace Backsight.Forms
{
    /// <written by="Steve Stanton" on="28-SEP-2007" />
    /// <summary>
    /// Wrapper on a brush. This class exists only because the basic <see cref="Brush"/> class
    /// doesn't expose properties like <c>Color</c>.
    /// </summary>
    public class Fill : IFill
    {
        #region Class data

        /// <summary>
        /// The brush to use for the fill.
        /// </summary>
        Brush m_Brush;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>Fill</c> that uses a <see cref="SolidBrush"/>.
        /// </summary>
        /// <param name="c">The color for the fill</param>
        public Fill(Color c)
        {
            m_Brush = new SolidBrush(c);
        }

        /// <summary>
        /// Creates a new <c>Fill</c> that uses a <see cref="HatchBrush"/>.
        /// </summary>
        /// <param name="style">The hatch style for the fill</param>
        /// <param name="foreColor">The color for hatch lines</param>
        /// <param name="backColor">The color behind the hatch lines</param>
        public Fill(HatchStyle style, Color foreColor, Color backColor)
        {
            m_Brush = new HatchBrush(style, foreColor, backColor);
        }

        #endregion

        /// <summary>
        /// The brush to use for the fill.
        /// </summary>
        public Brush Brush
        {
            get { return m_Brush; }
        }

        /// <summary>
        /// The color for this fill. If the fill is a hatch-style, this relates to the
        /// foreground color.
        /// </summary>
        public Color Color
        {
            get
            {
                if (m_Brush is SolidBrush)
                    return (m_Brush as SolidBrush).Color;

                if (m_Brush is HatchBrush)
                    return (m_Brush as HatchBrush).ForegroundColor;

                throw new NotSupportedException("Unhandled brush type");
            }

            set
            {
                if (m_Brush is SolidBrush)
                    (m_Brush as SolidBrush).Color = value;
                else if (m_Brush is HatchBrush)
                {
                    HatchBrush hb = (m_Brush as HatchBrush);
                    m_Brush = new HatchBrush(hb.HatchStyle, value, hb.BackgroundColor);
                }
                else
                    throw new NotSupportedException("Unhandled brush type");
            }
        }
    }
}
