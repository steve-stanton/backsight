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
using System.IO;
using System.Text;

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
        /// The key is the name of the entity type or layer, while the pointer
        /// refers to one of the instances in <c>m_Styles</c>
        /// </summary>
        Dictionary<string, Style> m_StyleLookup; // was m_pStyles

        /// <summary>
        /// The name of the last entity type or layer for which a style was requested
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
        /// Initialize this style file object with the content of a file.
        /// </summary>
        /// <param name="fileName">The name of the file to read the definition from.</param>
        /// <returns>The number of styles that were loaded.</returns>
        internal int Create(string fileName)
        {
            // Remember the file specification
            m_Spec = fileName;

            using (StreamReader sr = File.OpenText(fileName))
            {
                return Create(sr);
            }
        }

        /// <summary>
        /// Initialize this style file object with the content of a file.
        /// </summary>
        /// <param name="file">The file to read the definition from.</param>
        /// <returns>The number of styles that were loaded.</returns>
        int Create(StreamReader file)
        {
            // If we previously created stuff, get rid of it.
            ResetContents();

            // Load an index for standard colour names
            Dictionary<string, Color> colors = LoadColors();

            // Create the index of entity and/or layer name vs it's style
            m_StyleLookup = new Dictionary<string, Style>(100);

            string line;
            StyleEntry entry = new StyleEntry();
            StringBuilder errors = new StringBuilder(1000);
            List<DashPattern> dashPatterns = new List<DashPattern>(100);
            List<Style> styles = new List<Style>(100);

            while ((line=file.ReadLine())!=null)
            {
                int nToken = entry.Parse(line);
                if (nToken == 0)
                    continue;

                if (nToken < 0)
                {
                    errors.AppendLine(String.Format("Invalid style entry: {0}", line));
                    continue;
                }

                // If we've got the definition of a dash style,
                // parse it and add to the list of styles
                if (entry.IsDashStyle)
                {
                    DashPattern dp = new DashPattern();
                    if (dp.Create(entry))
                        dashPatterns.Add(dp);
                    else
                        errors.AppendLine(String.Format("Cannot parse style definition: {0}", line));

                }
                else
                {
                    // Only entity & layer entries are currently supported

                    string itemName;

                    if (entry.IsEntityEntry)
                        itemName = entry.EntityToken;
                    else if (entry.IsLayerEntry)
                        itemName = entry.LayerToken;
                    else
                        continue;

                    // Both entity & layer entries should have some sort of colour (either a
                    // named colour, or an RGB value)

                    Color col = Color.Black;
                    bool colDefined = false;
                    string colName = entry.ColToken;

                    if (colName.Length==0)
                    {
                        string rgb = entry.RGBToken;
                        colDefined = TryParseRGB(rgb, out col);
                    }
                    else
                    {
                        string fmtcol = FormatColorName(colName);
                        colDefined = colors.ContainsKey(fmtcol);
                        if (colDefined)
                            col = colors[fmtcol];
                    }

                    if (!colDefined)
                    {
                        errors.AppendLine(String.Format("Invalid colour in: {0}", line));
                        continue;
                    }

                    // Get any line pattern
                    DashPattern dp = null;
                    string pat = entry.DashToken;
                    if (pat.Length > 0)
                    {
                        dp = dashPatterns.Find(delegate(DashPattern d) { return d.Name==pat; });
                        if (dp==null)
                        {
                            errors.AppendLine(String.Format("Cannot find dash pattern for: {0}", line));
                            continue;
                        }
                    }

                    // Get any line weight (sanity check forces it
                    // to be in the range (0.01,10.0))

                    float fwt = -1.0F;
                    string wt = entry.WtToken;
                    if (wt.Length>0)
                    {
                        if (!Single.TryParse(wt, out fwt) || fwt<0.01F || fwt>10.0F)
                        {
                            errors.AppendLine(String.Format("Invalid line weight: {0}", line));
                            continue;
                        }
                    }

                    // Create a suitable style
                    Style style;

                    if (fwt < 0.01F && dp==null)
                        style = new Style(col);
                    else
                        style = new LineStyle(col, fwt, dp);

                    // If we already have an identical style, use that instead
                    Style prevStyle = styles.Find(delegate(Style s) { return s.Equals(style); });
                    if (prevStyle!=null)
                        m_StyleLookup[itemName] = prevStyle;
                    else
                    {
                        styles.Add(style);
                        m_StyleLookup[itemName] = style;
                    }
                }
            }

            m_DashPatterns = dashPatterns.ToArray();
            m_Styles = styles.ToArray();

            if (errors.Length>0)
                throw new Exception(errors.ToString());

            // Return the number of styles that we loaded.
            return m_Styles.Length;
        }

        /// <summary>
        /// Attempts to parse a string that holds an RGB triplet (where each colour
        /// value is expressed in base 10, delimited by commas).
        /// </summary>
        /// <param name="rgb">The string to parse</param>
        /// <param name="col">The corresponding colour (defined as Color.Black if the
        /// supplied string could not be parsed)</param>
        /// <returns>True if string was parsed successfully. False if any color value
        /// could not be parsed as an integer value.</returns>
        bool TryParseRGB(string rgb, out Color col)
        {
            col = Color.Black;

            string[] cols = rgb.Split(',');
            if (cols.Length!=3)
                return false;

            int r, g, b;

            if (!Int32.TryParse(cols[0], out r) ||
                !Int32.TryParse(cols[1], out g) ||
                !Int32.TryParse(cols[2], out b))
                return false;

            col = Color.FromArgb(r, g, b);
            return true;
        }

        /// <summary>
        /// Returns the style associated with the specified name (which is expected to
        /// refer either to the name of an entity type, or the name of a layer)
        /// </summary>
        /// <param name="itemName">The name of the entity type or layer</param>
        /// <returns>The corresponding style (null if the supplied name could not be found
        /// in the style mapping, or a style file has not been loaded).</returns>
        internal Style GetStyle(string itemName)
        {
            if (m_StyleLookup == null)
                return null;

            if (m_LastLookup == itemName)
                return m_LastStyle;

            m_LastStyle = (m_StyleLookup.ContainsKey(itemName) ? m_StyleLookup[itemName] : null);
            m_LastLookup = itemName;

            return m_LastStyle;
        }

        /// <summary>
        /// The file specification of the current style file (blank if one hasn't been loaded).
        /// </summary>
        internal string Spec
        {
            get { return m_Spec; }
        }

        /// <summary>
        /// Loads a map all standard colour names
        /// </summary>
        /// <returns>Standard colors, keyed by a lower-case version of the name</returns>
        Dictionary<string, Color> LoadColors()
        {
            string[] names = Enum.GetNames(typeof(KnownColor));

            Dictionary<string, Color> result = new Dictionary<string, Color>(names.Length);
            foreach (string name in names)
            {
                Color c = Color.FromName(name);
                result[name.ToLower()] = c;
            }

            return result;
        }

        /// <summary>
        /// Ensures a color name (as specified by a user) is in a standard form. All white
        /// space will be removed, as will any "-" character. The result is always in lower case.
        /// </summary>
        /// <param name="col">The string to standardize</param>
        /// <returns>The standardized string</returns>
        string FormatColorName(string col)
        {
            // Get rid of any leading and trailing white space
            string t = col.Trim().ToLower();
            if (t.Length==0)
                return String.Empty;

            StringBuilder result = new StringBuilder(t.Length);

            for (int i=0; i<t.Length; i++)
            {
                if (!(Char.IsWhiteSpace(t, i) || t[i]=='-'))
                    result.Append(t[i]);
            }

            return result.ToString();
        }
    }
}
