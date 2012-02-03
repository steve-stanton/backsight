// <remarks>
// Copyright 2008 - Steve Stanton. This file is part of Backsight
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
using System.Diagnostics;
using System.IO;

namespace Backsight.Editor
{
    /// <summary>
    /// The file holding project information.
    /// </summary>
    class ProjectFile
    {
        #region Class data

        /// <summary>
        /// The full name of the project file (including directory)
        /// </summary>
        string m_FileName;

        /// <summary>
        /// The information read from the file.
        /// </summary>
        readonly ProjectSettings m_Info;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>ProjectFile</c> that refers to an existing file
        /// </summary>
        /// <param name="fileName">The name of the project file to load</param>
        internal ProjectFile(string fileName)
        {
            m_FileName = fileName;
            m_Info = ProjectSettings.CreateInstance(m_FileName);
        }

        #endregion

        /// <summary>
        /// The user-perceived name of the project.
        /// </summary>
        internal string Name
        {
            get { return Path.GetFileNameWithoutExtension(m_FileName); }
        }

        /// <summary>
        /// Have modified project setting been saved?
        /// </summary>
        internal bool IsSaved
        {
            get { return m_Info.IsSaved; }
        }

        /// <summary>
        /// Saves the project settings as part of a persistent storage area.
        /// </summary>
        internal void Save()
        {
            m_Info.WriteXML(m_FileName);
        }

        /// <summary>
        /// Identifies a map layer associated with the project.
        /// </summary>
        //internal int LayerId
        //{
        //    get { return m_Info.LayerId; }
        //}

        /// <summary>
        /// Loads a map model with the content of this project.
        /// </summary>
        /// <param name="mapModel">The model to load</param>
        internal void LoadModel(CadastralMapModel mapModel)
        {
            //SessionDataFactory.Load(mapModel, this);
            Trace.Write("Reading data...");

            EditDeserializer editDeserializer = new EditDeserializer(mapModel);

            // Look for a Sessions folder in the same place as this project file
            // (not sure if m_FileName really has the full path when loaded from MRU)
            string jobFolder = Path.GetDirectoryName(m_FileName);
            string sessionsFolder = Path.Combine(jobFolder, "Sessions");

            // If the sessions folder doesn't exist, just create it and return (there's
            // nothing to load).
            if (!Directory.Exists(sessionsFolder))
            {
                Directory.CreateDirectory(sessionsFolder);
                return;
            }

            // Load each session
            /*
            foreach (string sessionFolderName in Directory.EnumerateDirectories(sessionsFolder))
            {
                // Only those folder names that are numbers are valid
                string subFolderName = Path.GetFileName(sessionFolderName);
                uint sessionId;
                if (UInt32.TryParse(subFolderName, out sessionId))
                {
                    // Append the session to the model (the Operation constructor will need this upon deserialization)
                    SessionFolder sf = new SessionFolder(sessionId, sessionFolderName);
                    mapModel.AddSession(sf);

                    sf.LoadEdits(editDeserializer);
                }
            }
             */
        }

        /// <summary>
        /// Creates a brand new session for this project.
        /// </summary>
        /// <param name="sessionId">The ID to assign to the new session</param>
        /// <returns>The newly created session</returns>
        /*
        internal Session AppendWorkingSession(uint sessionId)
        {
            string jobFolder = Path.GetDirectoryName(m_FileName);
            string sessionsFolder = Path.Combine(jobFolder, "Sessions");
            string newFolder = Path.Combine(sessionsFolder, String.Format("{0:000000}", sessionId));

            if (Directory.Exists(newFolder))
                throw new ArgumentException("Session folder already exists: " + newFolder);

            // Create the directory and add the session info
            Directory.CreateDirectory(newFolder);
            return new SessionFolder(sessionId, newFolder);
        }
        */

        /// <summary>
        /// Obtains information about ID allocations.
        /// </summary>
        /// <returns></returns>
        internal IdAllocationInfo[] GetIdAllocations()
        {
            throw new NotImplementedException();
            /*
            // Look for a Ids folder in the same place as this project file
            // (not sure if m_FileName really has the full path when loaded from MRU)
            if (m_IdFolder == null)
            {
                string jobFolder = Path.GetDirectoryName(m_FileName);
                string idFolder = Path.Combine(jobFolder, "Ids");
                m_IdFolder = new IdFolder(idFolder);
            }

            return m_IdFolder.GetIdAllocations();
             */
        }

        internal ProjectSettings Settings
        {
            get { return m_Info; }
        }
    }
}
