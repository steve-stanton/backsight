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
    /// <written by="Steve Stanton" on="17-NOV-1999" was="CuiFileCheck" />
    /// <summary>
    /// User interface for checking a file. Note that unlike other UI classes, this one does NOT
    //	inherit from <c>CommandUI</c>.
    /// </summary>
    class FileCheckUI
    {
        #region Class data

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// Creates a new <c>FileCheckUI</c>
        /// </summary>
        internal FileCheckUI()
        {
        }

        #endregion

        internal bool CanRollback(uint seq)
        {
            throw new NotImplementedException("FileCheckUI.CanRollback");
            //return (seq>m_OpSequence);
        }
    }
}
