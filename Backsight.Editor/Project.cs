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

using Backsight.Environment;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="23-JAN-2012"/>
    /// <summary>
    /// An editing project
    /// </summary>
    /// <remarks>Should be blind to the way the project is actually stored</remarks>
    class Project
    {
        #region Class data

        /// <summary>
        /// The database that contains this project.
        /// </summary>
        readonly ProjectSilo m_Container;

        /// <summary>
        /// The data for the original project creation event.
        /// </summary>
        NewProjectEvent m_ProjectInfo;

        /// <summary>
        /// The current user's project settings.
        /// </summary>
        readonly ProjectSettings m_Settings;

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
        /// <param name="container">The container for this project.</param>
        /// <param name="ps">The initial project settings.</param>
        internal Project(ProjectSilo container, ProjectSettings ps)
        {
            m_Container = container;
            m_Settings = ps;
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

        /// <summary>
        /// The current user's project settings.
        /// </summary>
        internal ProjectSettings Settings
        {
            get { return m_Settings; }
        }

        /// <summary>
        /// Saves user settings for this project.
        /// </summary>
        internal void SaveSettings()
        {
            if (m_ProjectInfo != null)
            {
                string dataFolder = m_Container.CreateDataFolder(m_ProjectInfo.ProjectId);
                string settingsFileName = Path.Combine(dataFolder, "settings.txt");
                m_Settings.WriteXML(settingsFileName);
            }
        }

        /// <summary>
        /// The user-perceived project name
        /// </summary>
        internal string Name
        {
            get { return m_ProjectInfo.ProjectName; }
        }

        /// <summary>
        /// The ID of the map layer the project is associated with.
        /// </summary>
        internal int LayerId
        {
            get { return m_ProjectInfo.LayerId; }
        }
    }
}
