using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Backsight.Editor
{
    /// <summary>
    /// A collection of Backsight projects.
    /// </summary>
    class ProjectSilo
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
        internal ProjectSilo(string folderName)
        {
            m_FolderName = folderName;

            // Ensure the folder structure is complete
            if (!Directory.Exists(IndexFolderName))
                Directory.CreateDirectory(IndexFolderName);
        }

        #endregion

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
        /// <remarks>The result relates to projects that exist on the local file system. It exlcudes
        /// any published projects that have not been downloaded.</remarks>
        internal string[] FindAllProjectNames()
        {
            string[] result = Directory.GetFiles(IndexFolderName, "*.txt");

            // Strip off the path
            for (int i = 0; i < result.Length; i++)
                result[i] = Path.GetFileName(result[i]);

            return result;
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
    }
}
