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
    /// <written by="Steve Stanton" on="05-FEB-1999" was="CeEntityFile"/>
    /// <summary>
    /// Object that holds the content of an entity file (a file that holds the
    /// definition of derived entity types).
    /// </summary>
    class EntityFile
    {
        #region Class data

        /// <summary>
        /// Translation blocks
        /// </summary>
        EntityBlock[] m_Blocks;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// Creates a new <c>EntityFile</c>
        /// </summary>
        internal EntityFile()
        {
        }

        #endregion
    }
}
