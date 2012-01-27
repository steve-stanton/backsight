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
   }
}
