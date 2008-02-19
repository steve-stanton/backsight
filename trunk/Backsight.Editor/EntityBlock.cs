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
        EntityTranslation[] m_Translations;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>EntityBlock</c>
        /// </summary>
        internal EntityBlock()
        {
        }

        #endregion
    }
}
