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

using Backsight.Xml;

namespace Backsight.Editor.Operations
{
    /// <summary>
    /// Implementation for <see cref="IDeletion"/>, for use in edit serialization.
    /// </summary>
    class DeletionAdapter : IDeletion
    {
        #region Class data

        /// <summary>
        /// The edit of interest
        /// </summary>
        readonly DeletionOperation m_Edit;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>DeletionAdapter</c> that refers to the specified edit.
        /// </summary>
        /// <param name="edit">The edit of interest</param>
        internal DeletionAdapter(DeletionOperation edit)
        {
            m_Edit = edit;
        }

        #endregion

        #region IDeletion Members

        /// <summary>
        /// The IDs of the deleted features
        /// </summary>
        public Guid[] Deletions
        {
            get
            {
                Feature[] dels = m_Edit.Deletions;
                Guid[] result = new Guid[dels.Length];
                for (int i=0; i<dels.Length; i++)
                    result[i] = new Guid(dels[i].DataId);

                return result;
            }
        }

        #endregion
    }
}
