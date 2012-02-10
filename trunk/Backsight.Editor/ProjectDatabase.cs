﻿// <remarks>
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
using System.Globalization;
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
        /// The path for the root folder (should always refers to a folder that exists).
        /// </summary>
        readonly string m_FolderName;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectDatabase"/> class with a root folder
        /// of C:\Backsight. If the folder does not already exist, an attempt to create it will be made.
        /// </summary>
        internal ProjectDatabase()
        {
            m_FolderName = @"C:\Backsight";

            // Ensure the folder structure is complete
            if (!Directory.Exists(IndexFolderName))
                Directory.CreateDirectory(IndexFolderName);
        }

        #endregion

        /// <summary>
        /// The path for the root folder of this database.
        /// </summary>
        internal string FolderName
        {
            get { return m_FolderName; }
        }

        /// <summary>
        /// The path for the directory holding project index entries.
        /// </summary>
        string IndexFolderName
        {
            get { return Path.Combine(m_FolderName, "index"); }
        }

        /// <summary>
        /// Obtains a list of all previously created editing projects.
        /// </summary>
        /// <returns>The names of all editing projects in this database.</returns>
        /// <remarks>The result relates to projects that exist on the local file system. It excludes
        /// any published projects that have not been downloaded.</remarks>
        internal string[] FindAllProjectNames()
        {
            List<string> result = new List<string>();

            foreach (string s in Directory.GetFiles(IndexFolderName, "*.txt"))
                result.Add(Path.GetFileNameWithoutExtension(s));

            result.Sort();
            return result.ToArray();
        }

        /// <summary>
        /// Attempts to obtain the internal ID for a project
        /// </summary>
        /// <param name="projectName">The user-perceived name of the project</param>
        /// <returns>The corresponding GUID string for the project (null if not found)</returns>
        internal string FindProjectId(string projectName)
        {
            string indexFileName = Path.Combine(IndexFolderName, projectName + ".txt");
            if (File.Exists(indexFileName))
                return File.ReadAllText(indexFileName);
            else
                return null;
        }

        /// <summary>
        /// Creates an index entry for a new project.
        /// </summary>
        /// <param name="projectName">The user-perceived name for the new project.</param>
        /// <param name="projectId">The unique ID for the project.</param>
        internal void CreateIndexEntry(string projectName, Guid projectId)
        {
            string fileName = Path.Combine(IndexFolderName, projectName + ".txt");
            File.WriteAllText(fileName, projectId.ToString());
        }

        /// <summary>
        /// Creates a data folder for a project.
        /// </summary>
        /// <param name="projectId">The internal ID for the project</param>
        /// <returns>The corresponding data folder (created if necessary)</returns>
        internal string CreateDataFolder(Guid projectId)
        {
            string folderName = Path.Combine(m_FolderName, projectId.ToString());
            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);

            return folderName;
        }

        /// <summary>
        /// Obtains the number of the data files for a specific project that exist in this silo.
        /// </summary>
        /// <param name="projectId">The unique ID of the project of interest</param>
        /// <returns>The data file numbers (sorted). An empty array if the project data folder does not exist.</returns>
        internal uint[] GetFileNumbers(Guid projectId)
        {
            string dataFolder = Path.Combine(m_FolderName, projectId.ToString());
            if (!Directory.Exists(dataFolder))
                return new uint[0];

            List<uint> result = new List<uint>(100);

            foreach (string s in Directory.GetFiles(dataFolder))
            {
                string name = Path.GetFileNameWithoutExtension(s);
                uint n;
                if (name.Length == 8 && UInt32.TryParse(name, NumberStyles.HexNumber, null, out n))
                    result.Add(n);
            }

            // There's a good chance the files will already be sorted, but just in case
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
            if (FindProjectId(projectName) != null)
                throw new ArgumentException("Specified project already exists");

            // Define the event data
            NewProjectEvent e = new NewProjectEvent()
            {
                ProjectId = Guid.NewGuid(),
                ProjectName = projectName,
                LayerId = layer.Id,
                DefaultSystem = String.Empty,
                UserName = System.Environment.UserName,
                MachineName = System.Environment.MachineName
            };

            // Create the private index entry
            CreateIndexEntry(projectName, e.ProjectId);

            // Create the data folder
            string dataFolder = CreateDataFolder(e.ProjectId);

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

            // Obtain the project ID
            string projectGuid = FindProjectId(projectName);
            if (projectGuid == null)
                return null;

            // Load local project settings
            string settingsFolderName = Path.Combine(m_FolderName, projectGuid);
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

            // Obtain the project ID
            string projectGuid = FindProjectId(projectName);
            if (projectGuid == null)
                throw new ApplicationException();

            // Load the project creation event
            Guid projectId = Guid.Parse(projectGuid);
            string dataFolder = CreateDataFolder(projectId);
            string creationFileName = Path.Combine(dataFolder, NewProjectEvent.FileName);

            // Read current project settings from the private silo (even if the project is now public)
            dataFolder = CreateDataFolder(projectId);
            string settingsFileName = Path.Combine(dataFolder, "settings.txt");
            ProjectSettings ps = ProjectSettings.CreateInstance(settingsFileName);

            Settings.Default.LastProjectName = projectName;
            Settings.Default.Save();

            // Now load the data
            Project result = new Project(this, projectId, ps);

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
            };

            string sessionFile = Path.Combine(dataFolder, GetDataFileName(sessionId));
            string sessionText = EditSerializer.GetSerializedString<Change>(DataField.Edit, s);
            File.WriteAllText(sessionFile, sessionText);

            Session session = new Session(result, s, sessionFile);
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
            // Note the file numbers of the data files to load
            uint[] fileNums = GetFileNumbers(p.Id);
            if (fileNums.Length == 0)
                throw new ArgumentException("Project doesn't have any data files");

            // Now load the files
            p.LoadDataFiles(m_FolderName, fileNums);
        }

        /// <summary>
        /// Checks whether a user-perceived project name is valid.
        /// </summary>
        /// <param name="projectName">The user-perceived name for the project</param>
        /// <returns>True if the project appears to exist. False if it cannot be found on the local machine.</returns>
        internal bool CanOpen(string projectName)
        {
            string projectId = FindProjectId(projectName);
            return (projectId != null);
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
                    // Rollup the edits in the session
                    s.EndSession();
                }
            }

            p.SaveSettings();
        }
    }
}
