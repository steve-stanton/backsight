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

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" on="23-MAR-2002" was="CeStyleEntry"/>
    /// <summary>
    /// Object that holds the content of a single line read from a style file. This parses
    /// the line into the various tokens that any entry can contain.
    /// </summary>
    class StyleEntry
    {
        #region Class data

        /// <summary>
        /// What follows ENTITY=
        /// </summary>
        string m_EntityToken;

        /// <summary>
        /// What follows THEME=
        /// </summary>
        string m_ThemeToken;

        /// <summary>
        /// What follows COL=
        /// </summary>
        string m_ColToken;

        /// <summary>
        /// What follows RGB=
        /// </summary>
        string m_RGBToken;

        /// <summary>
        /// What follows WT=
        /// </summary>
        string m_WtToken;

        /// <summary>
        /// What follows DASH_STYLE=
        /// </summary>
        string m_DashToken;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor creates an empty entry
        /// </summary>
        internal StyleEntry()
        {
            Reset();
        }

        #endregion

        /// <summary>
        /// What follows ENTITY=
        /// </summary>
        internal string EntityToken
        {
            get { return m_EntityToken; }
        }

        /// <summary>
        /// What follows THEME=
        /// </summary>
        internal string ThemeToken
        {
            get { return m_ThemeToken; }
        }

        /// <summary>
        /// What follows COL=
        /// </summary>
        internal string ColToken
        {
            get { return m_ColToken; }
        }

        /// <summary>
        /// What follows RGB=
        /// </summary>
        internal string RGBToken
        {
            get { return m_RGBToken; }
        }

        /// <summary>
        /// What follows WT=
        /// </summary>
        internal string WtToken
        {
            get { return m_WtToken; }
        }

        /// <summary>
        /// What follows DASH_STYLE=
        /// </summary>
        internal string DashToken
        {
            get { return m_DashToken; }
        }

        /// <summary>
        /// Attempts to parses the supplied string into some sort of style entry.
        /// </summary>
        /// <param name="str">The string to parse</param>
        /// <returns>The number of tokens that were parsed (-1 if a token was found, but
        /// a dependent token was not)</returns>
        internal int Parse(string str)
        {
            Reset();

            // The string must contain at least one '=' character
            if (str.IndexOf('=') <= 0)
                return 0;

            // Get rid of any leading and trailing white space
            string s = str.Trim();
            if (s.Length==0)
                return 0;

            // Look for the various tokens
            int nToken = 0;

            nToken += GetToken(s, "THEME=", ref m_ThemeToken);
            nToken += GetToken(s, "ENTITY=", ref m_EntityToken);
            nToken += GetToken(s, "COL=", ref m_ColToken);
            nToken += GetToken(s, "RGB=", ref m_RGBToken);
            nToken += GetToken(s, "WT=", ref m_WtToken);
            nToken += GetToken(s, "DASH_STYLE=", ref m_DashToken);

            if (nToken == 0)
                return 0;

            // If it has an entity token, it must have either
            // a color name or an RGB value and must not also
            // have a theme token

            if (m_EntityToken.Length>0)
            {
                if ((m_ColToken.Length==0 && m_RGBToken.Length==0)
                        || m_ThemeToken.Length!=0)
                    return -1;
            }

            return nToken;
        }

        /// <summary>
        /// Resets all style entry tokens to blank values.
        /// </summary>
        void Reset()
        {
            m_EntityToken = String.Empty;
            m_ThemeToken = String.Empty;
            m_ColToken = String.Empty;
            m_RGBToken = String.Empty;
            m_WtToken = String.Empty;
            m_DashToken = String.Empty;
        }

        /// <summary>
        /// Does this entry represent the definition of an entity? If so, the relevant color token
        /// can be obtained via either the <see cref="ColToken"/> or <see cref="RGBToken"/> property
        /// (one of them will be non-blank).
        /// <para/>
        /// For entity types that represent lines, the value of the <see cref="WtToken"/> property
        /// may also be defined.
        /// </summary>
        internal bool IsEntityEntry
        {
            get
            {
                // Must have the entity name token
                if (m_EntityToken.Length==0)
                    return false;

                // Must have either the colour name or RGB token
                if (m_ColToken.Length==0 && m_RGBToken.Length==0)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Does this entry represent the definition of a theme? If so, the relevant color token
        /// can be obtained via either the <see cref="ColToken"/> or <see cref="RGBToken"/> property
        /// (one of them will be non-blank).
        /// </summary>
        internal bool IsThemeEntry
        {
            get
            {
                // Must have the theme name token
                if (m_ThemeToken.Length==0)
                    return false;

                // Must have either the colour name or RGB token
                if (m_ColToken.Length==0 && m_RGBToken.Length==0)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Does this entry represent the definition of a dash style?
        /// </summary>
        internal bool IsDashStyle
        {
            get
            {
                // The definition is different from it's usage in
                // theme or entity entries
                if (IsEntityEntry || IsThemeEntry)
                    return false;

                return (m_DashToken.Length>0);
            }
        }

        /// <summary>
        /// Attempts to locate a specific token in the supplied string
        /// </summary>
        /// <param name="str">The string that may contain the desired token</param>
        /// <param name="token">The token to look for</param>
        /// <param name="value">The value to define</param>
        /// <returns>0 if the token was not found (in the case, the value to define is
        /// returned as a blank string). 1 if the token was found</returns>
        int GetToken(string str, string token, ref string value)
        {
            // See if the required token is there. If not, ensure
            // the value is returned as a blank string
            int tokIndex = str.IndexOf(token);
            if (tokIndex < 0)
            {
                value = String.Empty;
                return 0;
            }

            // Grab the stuff that follows the token
            string rest = str.Substring(tokIndex+token.Length);
            rest.TrimStart();

            // Get rid of any additional token that follows
            int len = rest.Length;
            int index;

            if ((index = rest.IndexOf("COL=")) > 0)
                len = Math.Min(index, len);
            if ((index = rest.IndexOf("RGB=")) > 0)
                len = Math.Min(index, len);
            if ((index = rest.IndexOf("WT=")) > 0)
                len = Math.Min(index, len);
            if ((index = rest.IndexOf("ENTITY=")) > 0)
                len = Math.Min(index, len);
            if ((index = rest.IndexOf("THEME=")) > 0)
                len = Math.Min(index, len);
            if ((index = rest.IndexOf("DASH_STYLE=")) > 0)
                len = Math.Min(index, len);

            value = rest.Substring(0, len).TrimEnd();
            return (value.Length==0 ? 0 : 1);
        }
    }
}
