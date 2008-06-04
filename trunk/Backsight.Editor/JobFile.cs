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

namespace Backsight.Editor
{
    /// <summary>
    /// The file holding job information (with the "cedx" file extension).
    /// </summary>
    class JobFile
    {
        #region Static

        /// <summary>
        /// Writes a new job file
        /// </summary>
        /// <param name="fileName">The name of the job file to create</param>
        /// <param name="info">The information to write out</param>
        /// <returns>An object representing the resultant file</returns>
        internal static JobFile SaveJobFile(string fileName, JobFileInfo info)
        {
            // Write out the info, then read it back in
            info.WriteXML(fileName);
            return new JobFile(fileName);
        }

        #endregion

        #region Class data

        /// <summary>
        /// The full name of the job file (including directory)
        /// </summary>
        string m_FileName;

        /// <summary>
        /// The information read from the file.
        /// </summary>
        JobFileInfo m_Info;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>JobFile</c> that refers to an existing file
        /// </summary>
        /// <param name="fileName">The name of the job file to load</param>
        internal JobFile(string fileName)
        {
            m_FileName = fileName;
            m_Info = JobFileInfo.CreateInstance(m_FileName);
        }

        #endregion

        /// <summary>
        /// The information read from the file.
        /// </summary>
        internal JobFileInfo Data
        {
            get { return m_Info; }
        }

        /// <summary>
        /// The full name of the job file (including directory)
        /// </summary>
        internal string Name
        {
            get { return m_FileName; }
        }

        /// <summary>
        /// Saves the job file to disk
        /// </summary>
        internal void Save()
        {
            m_Info.WriteXML(m_FileName);
        }
    }
}
