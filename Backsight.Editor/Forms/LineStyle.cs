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
    /// <written by="Steve Stanton" on="22-MAR-2002" was="CeLineStyle"/>
    /// <summary>
    /// The display style for a line feature.
    /// </summary>
    class LineStyle : Style
    {
        #region Class data

        /// <summary>
        /// Built-in MFC style (e.g. PS_DOT). If m_Pattern is not
        /// null, this is always set to PS_SOLID
        /// </summary>
        readonly int m_Style;

        /// <summary>
        /// Line weight (in meters on the ground). Any value .le. 0
        /// means it should be displayed with a width of 1 pixel.
        /// </summary>
        readonly double m_Weight;

        /// <summary>
        /// Any dashed line pattern that applies (null if the
        /// style implied by <c>m_Style</c> should prevail)
        /// </summary>
        readonly DashPattern m_Pattern;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for a line style with the specified color that
        /// will be displayed with a width of 1 pixel.
        /// </summary>
        /// <param name="col">The display color</param>
        /// <param name="style">The line style (e.g. PS_SOLID)</param>
        internal LineStyle(Color col, int style)
            : base(col)
        {
            m_Style = style;
            m_Weight = -1.0;
            m_Pattern = null;
        }

        /// <summary>
        /// Constructor for a style where lines will be displayed with the
        /// specified line weight.
        /// </summary>
        /// <param name="col">The display colour</param>
        /// <param name="wt">The line weight (in meters on the ground)</param>
        /// <param name="dp">Any dashed line pattern that should be used (null
        /// for a solid line</param>
        internal LineStyle(Color col, double wt, DashPattern dp)
            : base(col)
        {
            m_Style = 0; // PS_SOLID
            m_Weight = wt;
            m_Pattern = dp;
        }

        #endregion

        /// <summary>
        /// Is this style dependent on scale? (true if a line weight or pattern
        /// is part of this style).
        /// </summary>
        internal override bool IsScaleDependent
        {
            get { return (m_Weight > 0.0 || m_Pattern != null); }
        }

        /// <summary>
        /// Does this style match the supplied values
        /// </summary>
        /// <param name="col">The display color</param>
        /// <param name="style">The line style (default is PS_SOLID)</param>
        /// <returns></returns>
        internal bool Equals(Color col, int style)
        {
            return (style == m_Style && base.Equals(col));
        }

        /// <summary>
        /// Does this style match the supplied style?
        /// </summary>
        /// <param name="style">The style to compare with</param>
        /// <returns>True if the supplied style is identical to this one.</returns>
        internal override bool Equals(Style style)
        {
            if (!base.Equals(style))
                return false;

            // No match if the other style isn't also a line style
            LineStyle that = (style as LineStyle);
            if (that == null)
                return false;

            // Any dash pattern must have the same address in memory
            return (this.m_Style == that.m_Style &&
                    Object.ReferenceEquals(this.m_Pattern, that.m_Pattern) &&
                    Math.Abs(this.m_Weight-that.m_Weight) < 0.0001);
        }

        /// <summary>
        /// Defines the supplied pen with this style
        /// </summary>
        /// <param name="pen">The pen to define</param>
        /// <param name="draw">The definition of the draw</param>
        internal override void  DefinePen(ScaleSpecificPen pen, ISpatialDisplay draw)
        {
            float fwt = (m_Weight > 0.0 ? draw.LengthToDisplay(m_Weight) : 0.0F);

            if (m_Pattern != null)
                m_Pattern.DefinePen(pen.Pen, draw, fwt, new SolidBrush(this.Color));

            // MUST be solid for anything that has non-zero weight
            else if ((int)fwt != 0)
                pen.CreateSolidPen(fwt, this.Color);
            else
        		//pen.CreatePen(m_Style,0,GetColour());
                pen.CreateSolidPen(0.0F, this.Color);
        }
    }
}
