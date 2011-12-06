// <remarks>
// Copyright 2011 - Steve Stanton. This file is part of Backsight
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
using System.Linq;
using System.Text;

namespace Backsight.Editor.FileStore
{
    class SessionFolder : ISession
    {
        #region Class data

        /// <summary>
        /// The path to the folder where the edits were loaded from.
        /// </summary>
        readonly string m_FolderName;

        /// <summary>
        /// Operations (if any) that were performed during the session. 
        /// </summary>
        readonly List<Operation> m_Operations;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionFolder"/> class.
        /// </summary>
        /// <param name="folderName">The path to the folder where the edits were loaded from.</param>
        /// <param name="edits">Operations (if any) that were performed during the session.</param>
        internal SessionFolder(string folderName, Operation[] edits)
        {
            m_FolderName = folderName;
            m_Operations = new List<Operation>(edits);
        }

        #endregion

        /// <summary>
        /// The edits performed in this session.
        /// </summary>
        Operation[] ISession.Edits
        {
            get { return m_Operations.ToArray(); }
        }
    }
}
