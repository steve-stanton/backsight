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
	/// <written by="Steve Stanton" on="08-JUN-2007" />
    /// <summary>
    /// Something that imports spatial data from a file.
    /// </summary>
    abstract class FileImportSource
    {
        #region Class data

        /// <summary>
        /// The file specification of the import source.
        /// </summary>
        readonly string m_FileName;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>FileImportSource</c> that refers to the specified file.
        /// </summary>
        /// <param name="fileName">The file specification for the import source.</param>
        protected FileImportSource(string fileName)
        {
            m_FileName = fileName;
        }

        #endregion

        /// <summary>
        /// Loads the file on behalf of the supplied editing operation.
        /// </summary>
        /// <param name="creator">The editing operation performing the import</param>
        /// <returns>The loaded features</returns>
        internal Feature[] Load(Operation creator)
        {
            if (creator==null)
                throw new ArgumentNullException();

            return LoadFeatures(m_FileName, creator);
        }

        /// <summary>
        /// Creates features that correspond to the data in a file.
        /// </summary>
        /// <param name="fileName">The file containing data to import.</param>
        /// <param name="creator">The editing operation creating the features.</param>
        /// <returns>The features that correspond to the content of the file</returns>
        abstract internal Feature[] LoadFeatures(string fileName, Operation creator);
    }
}
