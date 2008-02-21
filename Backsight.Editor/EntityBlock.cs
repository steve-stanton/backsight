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
using System.IO;
using System.Collections.Generic;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="05-FEB-1999" was="CeEntityBlock"/>
    /// <summary>
    /// A block of entity translations read from an entity file.
    /// </summary>
    class EntityBlock
    {
        #region Class data

        /// <summary>
        /// The name of the entity type that this block refers to
        /// </summary>
        string m_EntityName;

        /// <summary>
        /// The name of the theme that this block refers to (blank for all themes)
        /// </summary>
        string m_ThemeName;
        
        /// <summary>
        /// The defined translations
        /// </summary>
        readonly List<EntityTranslation> m_Translations;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor. After calling this, you must make a call
        /// to <see cref="Create"> in order to initialize the object.
        /// </summary>
        internal EntityBlock()
        {
            m_EntityName = String.Empty;
            m_ThemeName = String.Empty;
            m_Translations = new List<EntityTranslation>();
        }

        #endregion

        /// <summary>
        /// Initializes this entity block with the info from an entity file.
        /// </summary>
        /// <param name="sr">The stream that the info is being read from. The file
        /// must already be open, and positioned immediately after a "BEGIN" record.
        /// It will be read as far as a record containing "END".</param>
        /// <returns>True if block was successfully loaded.</returns>
        /// <exception cref="Exception">If the end of file was encountered before
        /// finding the "END" keyword.</exception>
        internal void Create(StreamReader sr)
        {
            string line;

            while ((line=sr.ReadLine())!=null)
            {
                // Trim off any leading and trailing whitespace.
                string str = line.Trim();

                // Skip blank records.
                if (str.Length == 0)
                    continue;

                // Return if we have reached the end of the block.
                if (String.Compare(str, "END", true) == 0)
                {
                    EndBlock();
                    return;
                }

                // Make an upper-case version of the buffer so that we
                // can check for keywords.
                string upstr = str.ToUpper();

                if (upstr.Contains("ENTITY"))
                {
                    // Parse the entity type.
                    AddEntity(str);
                }
                else if (upstr.Contains("THEME"))
                {
                    // Parse the theme.
                    AddTheme(str);
                }
                else
                {
                    // No keyword, so treat the record as a translation entry.
                    AddTranslation(str);
                }
            }

            // We reached the end of file without encountering the "END"
            // keyword. Something is wrong.
            throw new Exception("EntityBlock.Create - Missing END keyword.");
        }

        /// <summary>
        /// Checks whether this translation block refers to a specific
        /// entity type (and theme).
        /// </summary>
        /// <param name="entName">The name of the candidate entity type.</param>
        /// <param name="theName">The name of the candidate theme.</param>
        /// <returns>True if we have a match.</returns>
        bool IsMatch(string entName, string theName)
        {
            // Return if this entity block does not refer to the entity
            // type that is being translated.
            if (m_EntityName != entName)
                return false;

            // If the block is restricted to a specific theme, ensure we
            // have a match.
            if (m_ThemeName.Length==0)
                return true;

            if (m_ThemeName != theName)
                return false;

            return true;
        }

        /// <summary>
        /// Tries to return the name of a derived entity type from this entity
        /// translation block. A prior call to <see cref="IsMatch"/> is required,
        /// to confirm that this translation block actually applies.
        /// </summary>
        /// <param name="entName1">The name of the 1st related entity type.</param>
        /// <param name="entName2">The name of the 2nd related entity type.</param>
        /// <returns>The name of the derived entity type (if one applies).
        /// Otherwise null.</returns>
        string GetDerivedType(string entName1, string entName2)
        {
            // Scan through the array of entity translations, looking for a match
            // with the specified entity types.
            foreach (EntityTranslation et in m_Translations)
            {
                if (et.IsMatch(entName1, entName2))
                    return et.Translation;
            }

            // Translation not found.
            return null;
        }

        /// <summary>
        /// Parses a record that contains the "THEME" keyword.
        /// </summary>
        /// <param name="str">The record to parse.</param>
        /// <exception cref="Exception">If the theme could
        /// not be found, or a theme has already been parsed for this
        /// entity block.</exception>
        void AddTheme(string str)
        {
            // Return with error if a theme pointer has already been obtained
            // for this entity block.
            if (m_ThemeName.Length>0)
                throw new Exception("Multiple THEME keywords encountered.");

            // Find the "=" character that should appear somewhere after
            // the THEME keyword.
            int eqpos = str.IndexOf('=');
            if (eqpos<0)
                throw new Exception("THEME keyword is not followed by '=' character");

            // Grab the stuff that follows the "=" character, and ensure
            // there is no leading or trailing white space.
            m_ThemeName = str.Substring(eqpos + 1).Trim();
        }

        /// <summary>
        /// Parses a record that contains the "ENTITY" keyword.
        /// </summary>
        /// <param name="str">The record to parse.</param>
        /// <exception cref="Exception">If the entity could not be found, or
        /// an entity type has already been parsed for this entity block.
        /// </exception>
        void AddEntity(string str)
        {
            // Return with error if an entity pointer has already been obtained
            // for this entity block.
            if (m_EntityName.Length>0)
                throw new Exception("Multiple ENTITY keywords encountered.");

            // Find the "=" character that should appear somewhere after
            // the ENTITY keyword.
            int eqpos = str.IndexOf('=');
            if (eqpos < 0)
                throw new Exception("ENTITY keyword is not followed by '=' character");

            // Grab the stuff that follows the "=" character, and ensure
            // there is no leading or trailing white space.
            m_EntityName = str.Substring(eqpos + 1).Trim();
        }

        /// <summary>
        /// Parses a record that refers to an entity translation.
        /// </summary>
        /// <param name="str">The record to parse.</param>
        /// <returns>True if record parsed ok. False if the record could
        /// not be parsed.</returns>
        bool AddTranslation(string str)
        {
            // Create an entity translation object for the current map.
            EntityTranslation entran = new EntityTranslation();
            if (!entran.Create(str))
                return false;

            m_Translations.Add(entran);
            return true;
        }

        /// <summary>
        /// Performs validation at the end of an entity block.
        /// </summary>
        /// <returns>True if entity block is ok.</returns>
        void EndBlock()
        {
            // Confirm that the entity type for this block has been
            // defined. And that we have at least one translation!

            if (m_EntityName.Length == 0 || m_Translations.Count == 0)
                throw new Exception("No entity type or translations for block");
        }

        /// <summary>
        /// Tries to return the name of an un-derived entity type associated with an
        /// entity type that may be derived.
        /// </summary>
        /// <param name="dervEntName">The name of the (maybe) derived entity type.</param>
        /// <returns>The un-derived entity type (null if it is not derived, as far
        /// as this translation block is concerned).</returns>
        internal string GetUnderivedType(string dervEntName)
        {
            // Go through each translation, looking for a translation that
            // matches the specified entity type. If we find a match, the
            // un-derived type is the entity type that this translation
            // block refers to.

            foreach (EntityTranslation et in m_Translations)
            {
                if (et.Translation == dervEntName)
                    return m_EntityName;
            }

            // Specified entity type is not derived.
            return null;
        }
    }
}
