/// <remarks>
/// Copyright 2008 - Steve Stanton. This file is part of Backsight
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

using Backsight.Environment;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="14-APR-2008" />
    /// <summary>
    /// Implementation of <c>IFont</c> that represents information about the default font.
    /// </summary>
    class FontInfo : IFont
    {
        #region Static

        /// <summary>
        /// Information for the default font
        /// </summary>
        static FontInfo s_DefaultFont = new FontInfo();

        #endregion

        #region Class data

        /// <summary>
        /// The name of the type face
        /// </summary>
        readonly string m_Typeface;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor creates a new <c>FontInfo</c> that represents the default font.
        /// </summary>
        FontInfo()
        {
            m_Typeface = FontFamily.GenericSansSerif.Name;
        }

        #endregion

        /// <summary>
        /// Information about the default font.
        /// </summary>
        internal static IFont Default
        {
            get { return s_DefaultFont; }
        }

        #region IFont Members

        /// <summary>
        /// The name of the font family (e.g. "Arial").
        /// </summary>
        public string TypeFace
        {
            get { return m_Typeface; }
        }

        /// <summary>
        /// The point-size of the font (always 10.0)
        /// </summary>
        public float PointSize
        {
            get { return 10.0F; }
        }

        /// <summary>
        /// Flag bits defining font modifiers (always <c>FontStyle.Regular</c>)
        /// </summary>
        public FontStyle Modifiers
        {
            get { return FontStyle.Regular; }
        }

        #endregion

        #region IEnvironmentItem Members

        /// <summary>
        /// The item ID (always 0, signifying "default")
        /// </summary>
        public int Id
        {
            get { return 0; }
        }

        #endregion
    }
}
