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

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="05-FEB-1999" was="CeEntityTranslation"/>
    /// <summary>
    /// An entity translation record that has been read from an entity file. Each entity translation
    ///	object forms part of an <see cref="EntityBlock"/> object.
    /// </summary>
    class EntityTranslation
    {
        #region Class data

        /// <summary>
        /// Name of 1st entity type
        /// </summary>
        string m_EntName1;

        /// <summary>
        /// Name of 2nd entity type (an empty string means anything will match)
        /// </summary>
        string m_EntName2;

        /// <summary>
        /// The resulting translation.
        /// </summary>
        string m_Translation;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor. After calling this, you must make a call to
        /// <see cref="Create"/> in order to initialize the object.
        /// </summary>
        internal EntityTranslation()
        {
            m_Translation = String.Empty;
            m_EntName1 = String.Empty;
            m_EntName2 = String.Empty;
        }

        #endregion

        /// <summary>
        /// The resulting translation.
        /// </summary>
        internal string Translation
        {
            get { return m_Translation; }
        }

        /// <summary>
        /// Initializes this entity translation object based on a record that has been
        /// read from an entity translation block in an entity file.
        /// </summary>
        /// <param name="str">The string to parse.</param>
        /// <returns>True if record initialized ok.</returns>
        internal bool Create(string str)
        {
            // The translation preceded the first "=" character in the supplied string.
            int pos = str.IndexOf('=');
            if (pos<0)
                return false;

            // Grab the stuff prior to the "=" character and trim off any surrounding white space.
            m_Translation = str.Substring(0, pos).Trim();

            // Grab the stuff after the "=" character.
            string rest = str.Substring(pos+1);

            // The rest should contain a backslash character somewhere.
            pos = rest.IndexOf('\\');
            if (pos<0)
                return false;

            // Get the names of the two entity types seperated by the backslash, and ensure
            // surrounding white space has been removed.
            m_EntName1 = rest.Substring(0, pos).Trim();
            m_EntName2 = rest.Substring(pos+1).Trim();

            // Ensure that the first entity type is always defined in
            // preference to the 2nd entity type.
            if (m_EntName1.Length==0 && m_EntName2.Length>0)
            {
                m_EntName1 = m_EntName2;
                m_EntName2 = String.Empty;
            }

            // We succeeded if we got at least one entity type and the
            // derived entity type.
            return (m_Translation.Length>0 && m_EntName1.Length>0);
        }

        /// <summary>
        /// Checks whether this entity translation matches a pair of entity types.
        /// </summary>
        /// <param name="e1">First entity type name</param>
        /// <param name="e2">Second entity type name</param>
        /// <returns>True if we've got a match.</returns>
        internal bool IsMatch(string e1, string e2)
        {
            // Do a comparison depending on whether we need to match just
            // one or both entity types.

            if (m_EntName2.Length==0)
                return (m_EntName1==e1 || m_EntName1==e2);
            else
                return ((m_EntName1==e1 && m_EntName2==e2) ||
                        (m_EntName1==e2 && m_EntName2==e1));
        }
    }
}

