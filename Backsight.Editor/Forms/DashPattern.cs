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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" on="07-MAY-2002" was="CeDashPattern"/>
    /// <summary>
    /// The definition of a pattern that can be used to produce dashed lines.
    /// </summary>
    class DashPattern
    {
        #region Class data

        /// <summary>
        /// The name of the pattern
        /// </summary>
        string m_Name;

        /// <summary>
        /// The sequence of dashes and spaces (starting with a dash). All values
        /// are in meters on the map projection.
        /// </summary>
        float[] m_Pattern;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor creates an undefined pattern. A subsequent call to
        /// <see cref="Create"/> is required to define the pattern.
        /// </summary>
        internal DashPattern()
        {
            m_Name = String.Empty;
            m_Pattern = null;
        }

        #endregion

        /// <summary>
        /// The name of the pattern
        /// </summary>
        internal string Name
        {
            get { return m_Name; }
        }

        /// <summary>
        /// Defines this pattern using the string obtained from a style file.
        /// </summary>
        /// <param name="entry">The entry that defines the pattern</param>
        /// <returns>True if parsed ok. False if something looks bad.</returns>
        internal bool Create(StyleEntry entry)
        {
            // The entry takes the form <name> = <pattern>, so if there's no "=" character
            // (or it's right at the very start), the entry is no good
            string token = entry.DashToken;
            int index = token.IndexOf('=');
            if (index<=0)
                return false;

            // Pick up the name for the pattern
            m_Name = token.Substring(0, index).Trim();
            if (m_Name.Length==0)
                return false;

            // Pick up the definition of the dashes and the intervening spaces
            string rest = token.Substring(index+1).Trim();
            if (rest.Length==0)
                return false;

            // There has to be at least one comma
            if (rest.IndexOf(',')<=0)
                return false;

            // Parse the values
            string[] pats = rest.Split(',');
            m_Pattern = new float[pats.Length];

            for (int i=0; i<pats.Length; i++)
            {
                float f;
                if (!Single.TryParse(pats[i], out f))
                    return false;

                // All values must be greater than 1mm on the ground.
                // This is basically a sanity check that's meant to
                // catch confusion about the units in which the dash
                // pattern is defined.
                if (f < 0.001F)
                    return false;

                m_Pattern[i] = f;
            }

            return true;

            // SS 18-FEB-2008: The following was commented out in the old CEdit implementation,
            // not sure if it has any continuing relevance...

            // If the very first element is a space, prepend a dash
            // that has no length. This is meant to accommodate the
            // fact that CPen::CreatePen expects the first value to
            // relate to a dash, then a space, etc.

            // Be more restrictive and force the first value to be
            // a dash. When I tried inserting a leading zero, it
            // looked like the width of the 0.1m dash was twice what
            // it should be. It's possible that MFC gets confused by
            // the zero (or possibly the fact that there were an odd
            // number of elements in the pattern).
            //if ( m_Pattern[0] > 0.0 )
            //{
            //    FLOAT4* array = new FLOAT4[m_NumElement+1];
            //    array[0] = 0.0;
            //    memcpy(&array[1],m_Pattern,m_NumElement*sizeof(FLOAT4));
            //    delete [] m_Pattern;
            //    m_NumElement++;
            //    m_Pattern = array;
            //}

            //// Ensure they're all positive numbers
            //for ( i=0; i<m_NumElement; i++ )
            //{
            //    m_Pattern[i] = fabs(m_Pattern[i]);
            //}

            //if ( ok )
            //{
            //    CString msg;
            //    msg.Format("%s=",m_Name);
            //    for ( i=0; i<m_NumElement; i++ )
            //    {
            //        if ( i>0 ) msg+=",";
            //        CString extra;
            //        extra.Format("%g",m_Pattern[i]);
            //        msg += extra;
            //    }
            //    AfxMessageBox(msg);
            //}	return ok;
        }

        /// <summary>
        /// Defines the supplied pen with this pattern
        /// </summary>
        /// <param name="pen">The pen to define</param>
        /// <param name="draw">The definition of the draw</param>
        /// <param name="logWidth">The width of the pen, in logical units</param>
        /// <param name="b">The brush defining the pen colour</param>
        internal void DefinePen(Pen pen, ISpatialDisplay draw, float logWidth, Brush b)
        {
            float[] style = new float[m_Pattern.Length];
            for (int i=0; i<m_Pattern.Length; i++)
            {
                style[i] = draw.LengthToDisplay((double)m_Pattern[i]);

                // Ensure dashes are at least one pixel in size
                style[i] = Math.Max(style[i], 1.0F);
            }

            // The default is PS_ENDCAP_ROUND, but if the line is thick, the dashes bleed into
            // each other. I also tried PS_ENDCAP_SQUARE, which seems to bleed even more.
            // Using PS_ENDCAP_FLAT means you can have really thin dashes along with thick lines,
            // which looks fine for simple line segments (it might be a poor choice for
            // convoluted lines however).

            pen.DashPattern = style;
            pen.EndCap = LineCap.Flat;
            pen.Brush = b;
            pen.Width = logWidth;

            //pen.CreatePen(PS_GEOMETRIC | PS_USERSTYLE | PS_ENDCAP_FLAT
            //             , logWidth
            //             , &b
            //             , m_NumElement
            //             , style);
        }
    }
}
