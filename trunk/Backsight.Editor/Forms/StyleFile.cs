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
using System.Collections.Generic;
using System.Drawing;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" on="20-MAR-2002" was="CeStyleFile"/>
    /// <summary>
    /// Object that holds the content of an style file (a file that holds the
    /// definition of colors and line patterns).
    /// </summary>
    class StyleFile
    {
        #region Class data

        /// <summary>
        /// The file specification of the current style file (blank if no style file
        /// has been loaded)
        /// </summary>
        string m_Spec;

        /// <summary>
        /// The key is the name of the entity type or theme, while the pointer
        /// refers to one of the instances in <c>m_Styles</c>
        /// </summary>
        Dictionary<string, Style> m_StyleLookup; // was m_pStyles

        /// <summary>
        /// The name of the last entity type or theme for which a style was requested
        /// </summary>
        string m_LastLookup;

        /// <summary>
        /// The style that was returned for <c>m_LastLookup</c> (may be null)
        /// </summary>
        Style m_LastStyle;

        /// <summary>
        /// The created styles
        /// </summary>
        Style[] m_Styles;

        /// <summary>
        /// Any dashed line styles
        /// </summary>
        DashPattern[] m_DashPatterns; 

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor. After calling this function, the
        /// object must be initialized with a call to <see cref="Create"/>
        /// </summary>
        internal StyleFile()
        {
            ResetContents();
        }

        #endregion

        /// <summary>
        /// Reset data members to their initial values.
        /// </summary>
        void ResetContents()
        {
            m_Spec = String.Empty;
            m_StyleLookup = null;
            m_LastLookup = String.Empty;
            m_LastStyle = null;
            m_Styles = null;
            m_DashPatterns = null;
        }

        /// <summary>
        /// Loads a map all standard colour names
        /// </summary>
        /// <returns>Standard colors, keyed by a lower-case version of the name</returns>
        Dictionary<string, Color> LoadColors()
        {
            string[] names = Enum.GetNames(typeof(KnownColor));

            Dictionary<string, Color> result = new Dictionary<string,Color>(names.Length);
            foreach (string name in names)
            {
                Color c = Color.FromName(name);
                result[name.ToLower()] = c;
            }

            return result;
        }
    }
}
