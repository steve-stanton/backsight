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

using System.Collections.Generic;

using Backsight.Environment;
using System;
using System.IO;
using Backsight.Data;
using Backsight.Editor.Properties;


namespace Backsight.Editor
{
    /// <summary>
    /// A collection of Backsight projects (stored on the local file system).
    /// </summary>
    /// c.f. JobCollectionFolder
    class ProjectDatabase
    {
        #region Class data

        /// <summary>
        /// The folder structure for public projects.
        /// </summary>
        readonly ProjectSilo m_Public;

        /// <summary>
        /// The folder structure for private projects.
        /// </summary>
        readonly ProjectSilo m_Private;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectDatabase"/> class with a root folder
        /// of C:\Backsight. If the folder does not already exist, an attempt to create it will be made.
        /// </summary>
        internal ProjectDatabase()
        {
            m_Public = new ProjectSilo(@"C:\Backsight\public");
            m_Private = new ProjectSilo(@"C:\Backsight\private");
        }

        #endregion

        /// <summary>
        /// Obtains a list of all previously created editing projects.
        /// </summary>
        /// <returns>The names of all editing projects in this database (sorted alphabetically).</returns>
        /// <remarks>The result relates to projects that exist on the local file system. It excludes
        /// any published projects that have not been downloaded.</remarks>
        internal string[] FindAllProjectNames()
        {
            List<string> result = new List<string>();

            result.AddRange(m_Public.FindAllProjectNames());
            result.AddRange(m_Private.FindAllProjectNames());

            result.Sort();
            return result.ToArray();
        }

        /// <summary>
        /// Creates a brand new project.
        /// </summary>
        /// <param name="projectName">The user-perceived name for the project.</param>
        /// <param name="layer">The map layer the project is for (not null)</param>
        /// <returns>Information describing the state of the project.</returns>
        internal Project CreateProject(string projectName, ILayer layer)
        {
            if (String.IsNullOrWhiteSpace(projectName) || layer == null)
                throw new ArgumentNullException();

            // Confirm that the project name is unique
            if (m_Public.FindProjectId(projectName) != null || m_Private.FindProjectId(projectName) != null)
                throw new ArgumentException("Specified project already exists");

            // Define the event data
            NewProjectEvent e = new NewProjectEvent()
            {
                ProjectId = Guid.NewGuid(),
                ProjectName = projectName,
                CreationTime = DateTime.Now,
                LayerId = layer.Id,
                DefaultSystem = String.Empty,
                UserName = System.Environment.UserName,
                MachineName = System.Environment.MachineName
            };

            // Create the private index entry
            m_Private.CreateIndexEntry(projectName, e.ProjectId);

            // Create the private data folder
            string dataFolder = m_Private.CreateDataFolder(e.ProjectId);

            // Serialize the event data to the data folder
            string s = EditSerializer.GetSerializedString<NewProjectEvent>(DataField.Edit, e);
            string fileName = Path.Combine(dataFolder, String.Format("{0:X8}.txt", e.EditSequence));
            File.WriteAllText(fileName, s);

            // Write initial project settings to the data folder
            ProjectSettings ps = new ProjectSettings();
            ps.ConnectionString = ConnectionFactory.ConnectionString;
            ps.LayerId = layer.Id;

            // Turn off auto-number if there's no database connection string
            if (String.IsNullOrEmpty(ps.ConnectionString))
                ps.IsAutoNumber = false;

            // Remember default entity types for points, lines, text, polygons
            ps.DefaultPointType = layer.DefaultPointType.Id;
            ps.DefaultLineType = layer.DefaultLineType.Id;
            ps.DefaultPolygonType = layer.DefaultPolygonType.Id;
            ps.DefaultTextType = layer.DefaultTextType.Id;

            Project result = new Project(m_Private, e, ps);
            result.SaveSettings();

            // Remember the newly created project as the default for the application
            Settings.Default.LastProjectId = e.ProjectId;
            Settings.Default.Save();

            // Wrap the event data along with the initial project settings
            return result;
        }

        /// <summary>
        /// Attempts to load local settings for a specific project.
        /// </summary>
        /// <param name="projectName">The user-perceived name for the project</param>
        /// <returns>The local settings for the specified project (null if the project could not be found).</returns>
        internal ProjectSettings GetProjectSettings(string projectName)
        {
            if (String.IsNullOrWhiteSpace(projectName))
                return null;

            ProjectSilo silo = FindProjectSilo(projectName);
            if (silo == null)
                return null;

            // Read the project ID from the silo
            string projectGuid = silo.FindProjectId(projectName);
            if (projectGuid == null)
                return null;

            // Load local project settings
            string settingsFolderName = Path.Combine(m_Private.FolderName, projectGuid);
            if (!Directory.Exists(settingsFolderName))
                return null;

            string settingsFileName = Path.Combine(settingsFolderName, "settings.txt");

            try
            {
                return ProjectSettings.CreateInstance(settingsFileName);
            }

            catch { }
            return null;
        }

        /// <summary>
        /// Opens an editing project that was previously created.
        /// </summary>
        /// <param name="projectName">The user-perceived name of the project</param>
        /// <returns>Information describing the state of the project (null if it could not be found).</returns>
        internal Project OpenProject(string projectName)
        {
            if (String.IsNullOrWhiteSpace(projectName))
                throw new ArgumentNullException();

            // Are we dealing with a public or a private project?
            ProjectSilo silo = FindProjectSilo(projectName);
            if (silo == null)
                throw new ArgumentException("Cannot find project " + projectName);

            // Read the project ID from the silo
            string projectGuid = silo.FindProjectId(projectName);
            if (projectGuid == null)
                throw new ApplicationException();

            // Load the project creation event
            Guid projectId = Guid.Parse(projectGuid);
            string dataFolder = silo.CreateDataFolder(projectId);
            string creationFileName = Path.Combine(dataFolder, NewProjectEvent.FileName);
            //EditDeserializer ed = new EditDeserializer();
            //new TextEditReader();
            NewProjectEvent e = null;

            // Read current project settings from the private silo (even if the project is now public)
            dataFolder = m_Private.CreateDataFolder(projectId);
            string settingsFileName = Path.Combine(dataFolder, "settings.txt");
            ProjectSettings ps = ProjectSettings.CreateInstance(settingsFileName);

            Settings.Default.LastProjectId = projectId;
            Settings.Default.Save();

            return new Project(silo, e, ps);
        }

        /// <summary>
        /// Attempts to locate the silo for a specific project.
        /// </summary>
        /// <param name="projectName">The user-perceived name for the project</param>
        /// <returns>The corresponding silo (null if not found)</returns>
        ProjectSilo FindProjectSilo(string projectName)
        {
            string projectId = m_Private.FindProjectId(projectName);
            if (projectId != null)
                return m_Private;

            projectId = m_Public.FindProjectId(projectName);
            if (projectId != null)
                return m_Public;

            return null;
                throw new ArgumentException("Cannot find project " + projectName);
        }

        /// <summary>
        /// Checks whether a user-perceived project name is valid.
        /// </summary>
        /// <param name="projectName">The user-perceived name for the project</param>
        /// <returns>True if the project appears to exist. False if it cannot be found on the local machine.</returns>
        internal bool CanOpen(string projectName)
        {
            ProjectSilo silo = FindProjectSilo(projectName);
            return (silo != null);
        }
    }
}
