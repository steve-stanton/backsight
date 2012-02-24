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
using System.Diagnostics;
using System.Globalization;
using System.IO;

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
        readonly ProjectDatabase m_Container;

        /// <summary>
        /// The unique ID for the project
        /// </summary>
        readonly Guid m_Id;

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

        /// <summary>
        /// The data model for the map.
        /// </summary>
        readonly CadastralMapModel m_MapModel;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Project"/> class
        /// upon creation of a brand new project.
        /// </summary>
        /// <param name="container">The container for this project.</param>
        /// <param name="projectId">The unique ID for the project.</param>
        /// <param name="ps">The initial project settings.</param>
        internal Project(ProjectDatabase container, Guid projectId, ProjectSettings ps)
        {
            m_Container = container;
            m_Id = projectId;
            m_Settings = ps;
            m_MapModel = new CadastralMapModel();
        }

        #endregion

        /// <summary>
        /// Loads a new map model
        /// </summary>
        /// <param name="dataFolder">The folder containing the data files</param>
        internal void LoadEdits(string dataFolder)
        {
            // Note the file numbers of the data files to load
            uint[] fileNums = GetFileNumbers(dataFolder);
            if (fileNums.Length == 0)
                throw new ArgumentException("Project doesn't have any data files");

            // Now load the files
            LoadDataFiles(dataFolder, fileNums);
        }

        /// <summary>
        /// Obtains the number of the data files in the project data folder.
        /// </summary>
        /// <param name="dataFolder">The folder containing the data files</param>
        /// <returns>The data file numbers (sorted). An empty array if the project data folder does not exist.</returns>
        uint[] GetFileNumbers(string dataFolder)
        {
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
        /// Records the highest internal ID for this project.
        /// </summary>
        /// <param name="maxItemId">The last internal ID used by this project</param>
        internal void SetLastItem(uint maxItemId)
        {
            m_LastItemId = maxItemId;
        }

        /// <summary>
        /// The last internal ID value assigned by the project.
        /// </summary>
        internal uint LastItemId
        {
            get { return m_LastItemId; }
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
            string dataFolder = m_Container.CreateDataFolder(m_Id);
            string settingsFileName = Path.Combine(dataFolder, "settings.txt");
            m_Settings.WriteXML(settingsFileName);
        }

        /// <summary>
        /// The user-perceived project name
        /// </summary>
        internal string Name
        {
            get
            {
                if (m_ProjectInfo == null)
                    return "Unknown";
                else
                    return m_ProjectInfo.ProjectName;
            }
        }

        /// <summary>
        /// A unique ID for the project.
        /// </summary>
        internal Guid Id
        {
            get { return m_Id; }
        }

        /// <summary>
        /// The ID of the map layer the project is associated with.
        /// </summary>
        internal int LayerId
        {
            get
            {
                if (m_ProjectInfo == null)
                    return 0;
                else
                    return m_ProjectInfo.LayerId;
            }
        }

        /// <summary>
        /// The data model for the map.
        /// </summary>
        internal CadastralMapModel Model
        {
            get { return m_MapModel; }
        }

        void LoadDataFiles(string folderName, uint[] fileNums)
        {
            Trace.Write("Reading data...");
            EditDeserializer ed = new EditDeserializer(m_MapModel);
            Session lastSession = null;
            IdManager idMan = m_MapModel.IdManager;

            foreach (uint fileNum in fileNums)
            {
                string editFile = Path.Combine(folderName, ProjectDatabase.GetDataFileName(fileNum));

                using (TextReader tr = File.OpenText(editFile))
                {
                    TextEditReader er = new TextEditReader(tr);

                    // Ignore any empty files altogether
                    while (er.HasNext)
                    {
                        ed.SetReader(er);
                        Change edit = Change.Deserialize(ed);

                        if (edit is NewProjectEvent)
                        {
                            m_ProjectInfo = (NewProjectEvent)edit;
                        }
                        else if (edit is NewSessionEvent)
                        {
                            lastSession = new Session(this, (NewSessionEvent)edit, editFile);
                            m_MapModel.AddSession(lastSession);
                        }
                        else if (edit is EndSessionEvent)
                        {
                            Debug.Assert(lastSession != null);
                            lastSession.EndTime = edit.When;
                        }
                        else if (edit is IdAllocation)
                        {
                            if (idMan != null)
                            {
                                IdAllocation alloc = (IdAllocation)edit;
                                IdGroup g = idMan.FindGroupById(alloc.GroupId);
                                g.AddIdPacket(alloc);

                                // Remember that allocations have been made in the session (bit of a hack
                                // to ensure the session isn't later removed if no edits are actually
                                // performed).
                                lastSession.AddAllocation(alloc);
                            }

                        }
                        else
                        {
                            Debug.Assert(edit is Operation);
                            Debug.Assert(lastSession != null);
                            lastSession.AddOperation((Operation)edit);
                        }
                    }
                }
            }

            if (m_ProjectInfo == null)
                throw new ApplicationException("Could not locate the project creation event");

            // Remember the highest internal ID used by the project
            SetLastItem(fileNums[fileNums.Length - 1]);
        }

        /// <summary>
        /// The data folder for this project.
        /// </summary>
        internal string ProjectFolder
        {
            get { return Path.Combine(m_Container.FolderName, m_Id.ToString().ToUpper()); }
        }

        /// <summary>
        /// The project undo folder (may not exist).
        /// </summary>
        internal string UndoFolder
        {
            get { return Path.Combine(ProjectFolder, "undo"); }
        }

        /// <summary>
        /// Obtains the path of the project undo folder (creating it if it does not already exist).
        /// </summary>
        /// <returns></returns>
        internal string GetUndoFolder()
        {
            string undoFolder = this.UndoFolder;
            if (!Directory.Exists(undoFolder))
                Directory.CreateDirectory(undoFolder);

            return undoFolder;
        }

        /// <summary>
        /// Ensures any undo folder for this project has been removed (usually done when an
        /// editing session is being closed).
        /// </summary>
        internal void DeleteUndoFolder()
        {
            string undoFolder = this.UndoFolder;
            if (Directory.Exists(undoFolder))
                Directory.Delete(undoFolder, true);
        }

        /// <summary>
        /// Writes the data for some sort of change to this project's data folder.
        /// </summary>
        /// <param name="c">The change to write</param>
        internal void WriteChange(Change c)
        {
            string dataFolder = Path.Combine(m_Container.FolderName, m_Id.ToString().ToUpper());
            string dataFile = Path.Combine(dataFolder, ProjectDatabase.GetDataFileName(c.EditSequence));
            string changeText = EditSerializer.GetSerializedString<Change>(DataField.Edit, c);
            File.WriteAllText(dataFile, changeText);
        }
    }
}
