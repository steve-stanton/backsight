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
using System.Collections.Generic;
using System.IO;

using Backsight.Data;
using Backsight.Editor.Properties;
using Backsight.Environment;


namespace Backsight.Editor
{
    /// <summary>
    /// A collection of Backsight projects (stored on the local file system).
    /// </summary>
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
        /// Creates a brand new project. If this completes without any exception, you can
        /// call <see cref="OpenProject"/> to activate the project.
        /// </summary>
        /// <param name="projectName">The user-perceived name for the project.</param>
        /// <param name="layer">The map layer the project is for (not null)</param>
        internal void CreateProject(string projectName, ILayer layer)
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

            // Serialize the event data to the data folder. Specify <Change> so that
            // the class name will be included in the output file.
            string s = EditSerializer.GetSerializedString<Change>(DataField.Edit, e);
            string fileName = Path.Combine(dataFolder, GetDataFileName(e.EditSequence));
            File.WriteAllText(fileName, s);

            // Write initial project settings to the data folder
            ProjectSettings ps = new ProjectSettings();
            ps.ConnectionString = ConnectionFactory.ConnectionString;

            // Turn off auto-number if there's no database connection string
            if (String.IsNullOrEmpty(ps.ConnectionString))
                ps.IsAutoNumber = false;

            // Remember default entity types for points, lines, text, polygons
            ps.DefaultPointType = layer.DefaultPointType.Id;
            ps.DefaultLineType = layer.DefaultLineType.Id;
            ps.DefaultPolygonType = layer.DefaultPolygonType.Id;
            ps.DefaultTextType = layer.DefaultTextType.Id;

            // Save the settings
            string settingsFileName = Path.Combine(dataFolder, "settings.txt");
            ps.WriteXML(settingsFileName);
        }

        /// <summary>
        /// Obtains the name of the data file that corresponds to a file number.
        /// </summary>
        /// <param name="fileNumber">The file number</param>
        /// <returns>The corresponding file name (without any directory specification).</returns>
        internal static string GetDataFileName(uint fileNumber)
        {
            return String.Format("{0:X8}.txt", fileNumber);
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

            // Read current project settings from the private silo (even if the project is now public)
            dataFolder = m_Private.CreateDataFolder(projectId);
            string settingsFileName = Path.Combine(dataFolder, "settings.txt");
            ProjectSettings ps = ProjectSettings.CreateInstance(settingsFileName);

            Settings.Default.LastProjectName = projectName;
            Settings.Default.Save();

            // Now load the data
            bool isPublic = (silo == m_Public ? true : false);
            Project result = new Project(silo, isPublic, projectId, ps);

            // The Load method will end up calling software that requires access to the
            // current map model, so we need to set it no
            // -- not sure if this is still true
            //EditingController.Current.SetMapModel(result.Model, null);
            EditingController.Current.SetProject(result);

            // Doing it here versus there is historical...
            LoadEdits(result);
            EditingController.Current.SetMapModel(result.Model, null);
            result.Model.Load();

            // Get rid of any empty sessions
            result.Model.RemoveEmptySessions();

            // Need to set it again (need to find out why)... if you don't you get a null
            // ref on opening last project at startup
            EditingController.Current.SetMapModel(result.Model, null);

            // Create a new editing session
            uint sessionId = result.AllocateId();
            NewSessionEvent s = new NewSessionEvent(sessionId)
            {
                UserName = System.Environment.UserName,
                MachineName = System.Environment.MachineName,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now
            };

            string sessionFile = Path.Combine(dataFolder, GetDataFileName(sessionId));
            Session session = new Session(result, s, sessionFile);
            session.WriteFile();
            result.Model.AddSession(session);
            result.Model.WorkingSession = session;
            result.SetLastItem(session.Id);

            return result;
        }

        /// <summary>
        /// Loads a new map model
        /// </summary>
        /// <param name="p">The project containing the model</param>
        void LoadEdits(Project p)
        {
            // If the project is public, we need to first load from the public silo (but no further than
            // the last data file that existed on publication).
            uint[] privateFiles = m_Private.GetFileNumbers(p.Id);
            if (privateFiles.Length == 0)
                throw new ArgumentException("Project doesn't have any private data files");

            // If the first file isn't 00000001.txt, it means the project has been published, so we
            // need to initially load stuff from the public silo.

            List<string> files = new List<string>();

            if (privateFiles[0] > 1)
            {
                uint[] publicFiles = m_Public.GetFileNumbers(p.Id);
                string publicFolder = m_Public.CreateDataFolder(p.Id);

                foreach (uint fileNum in publicFiles)
                {
                    if (fileNum < privateFiles[0])
                    {
                        string fileName = Path.Combine(publicFolder, GetDataFileName(fileNum));
                        files.Add(fileName);
                    }
                }
            }

            // Append all private files
            string privateFolder = m_Private.CreateDataFolder(p.Id);
            foreach (uint fileNum in privateFiles)
            {
                string fileName = Path.Combine(privateFolder, GetDataFileName(fileNum));
                files.Add(fileName);
            }

            // Now load the files
            p.LoadDataFiles(files.ToArray());

            // Remember the highest internal ID used by the project
            p.SetLastItem(privateFiles[privateFiles.Length - 1]);
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

        /// <summary>
        /// Closes a open project. If no edits have been performed, this will delete the file that
        /// records the session. Otherwise the timestamp stored in the session file will be updated.
        /// Local project settings will also be saved for luck.
        /// </summary>
        /// <param name="p">The project to close</param>
        internal void CloseProject(Project p)
        {
            Session s = p.Model.WorkingSession;

            if (s != null)
            {
                if (s.OperationCount == 0)
                {
                    string sessionFile = s.FileName;
                    File.Delete(sessionFile);
                }
                else
                {
                    s.UpdateEndTime();
                }
            }

            p.SaveSettings();
        }
    }
}
