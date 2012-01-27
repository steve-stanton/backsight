// <remarks>
// Copyright 2012 - Steve Stanton. This file is part of Backsight
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
using System.IO;

using Backsight.Editor.ChangeEvents;
using Backsight.Environment;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="23-JAN-2012"/>
    /// <summary>
    /// An editing project
    /// </summary>
    class Project
    {
        #region Static

        /// <summary>
        /// Creates a brand new project (and saves it).
        /// </summary>
        /// <param name="projectName">The user-perceived name of the project.</param>
        /// <param name="layer"> The map layer the project is associated with.</param>
        /// <param name="defaultSystem">The name of the default coordinate system.</param>
        /// <returns>The newly created project</returns>
        static internal Project CreateNewProject(string projectName, ILayer layer, string defaultSystem)
        {
            // Define the event data
            NewProjectEvent e = new NewProjectEvent()
            {
                ProjectId = Guid.NewGuid(),
                ProjectName = projectName,
                CreationTime = DateTime.Now,
                LayerId = layer.Id,
                DefaultSystem = defaultSystem,
                UserName = System.Environment.UserName,
                MachineName = System.Environment.MachineName
            };

            // Create the private index entry
            CreateIndexEntry(projectName, e.ProjectId);

            // Create the private data folder
            string dataFolder = GetDataPath(e.ProjectId, true);

            // Serialize the event data to the data folder
            string s = EditSerializer.GetSerializedString<NewProjectEvent>(DataField.Edit, e);
            string fileName = Path.Combine(dataFolder, String.Format("{0:X8}.txt", e.EditSequence));
            File.WriteAllText(fileName, s);

            // Wrap the event data
            return new Project(e);
        }

        /// <summary>
        /// Creates a private index entry for a new project.
        /// </summary>
        /// <param name="projectName">The user-perceived name for the new project.</param>
        /// <param name="projectId">The unique ID for the project.</param>
        static void CreateIndexEntry(string projectName, Guid projectId)
        {
            string indexFolder = GetIndexPath(true);
            string fileName = Path.Combine(indexFolder, projectName + ".txt");
            File.WriteAllText(fileName, projectId.ToString());
        }

        /// <summary>
        /// Obtains the path of the index folder (and ensures that it exists).
        /// </summary>
        /// <param name="wantPrivate">Specify <c>true</c> for the private index, or <c>false</c> for the
        /// public index.</param>
        /// <returns>The path of the index folder</returns>
        static string GetIndexPath(bool wantPrivate)
        {
            string s = (wantPrivate ? "private" : "public");
            string folderName = String.Format(@"C:\Backsight\{0}\index", s);

            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);

            return folderName;
        }

        /// <summary>
        /// Obtains the path of the data folder (and ensures that it exists).
        /// </summary>
        /// <param name="projectId">The unique project ID.</param>
        /// <param name="wantPrivate">Specify <c>true</c> for the private data folder, <c>false</c> for the
        /// public data folder</param>
        /// <returns>The path of the data folder</returns>
        static string GetDataPath(Guid projectId, bool wantPrivate)
        {
            string s = (wantPrivate ? "private" : "public");
            string folderName = String.Format(@"C:\Backsight\{0}\{1}", s, projectId.ToString());

            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);

            return folderName;
        }


        #endregion

        #region Class data

        /// <summary>
        /// The data for the original project creation event.
        /// </summary>
        readonly NewProjectEvent m_ProjectInfo;

        /// <summary>
        /// The last internal ID value assigned by the project.
        /// </summary>
        uint m_LastItemId;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Project"/> class
        /// upon creation of a brand new project.
        /// </summary>
        /// <param name="e">The data for the original project creation event.</param>
        Project(NewProjectEvent e)
        {
            m_ProjectInfo = e;
            m_LastItemId = 1;
        }

        #endregion

        /// <summary>
        /// Allocates a single internal ID for this project. This is a lightweight request that
        /// just increments a counter. It does not persist anything to disk.
        /// </summary>
        /// <returns>The next available ID.</returns>
        internal uint AllocateId()
        {
            m_LastItemId++;
            return m_LastItemId;
        }
    }
}
