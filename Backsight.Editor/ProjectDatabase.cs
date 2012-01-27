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
        /// <returns>The names of all editing projects in this database.</returns>
        /// <remarks>The result relates to projects that exist on the local file system. It exlcudes
        /// any published projects that have not been downloaded.</remarks>
        internal string[] FindAllProjectNames()
        {
            List<string> result = new List<string>();

            result.AddRange(m_Public.FindAllProjectNames());
            result.AddRange(m_Private.FindAllProjectNames());

            return result.ToArray();
        }

        /// <summary>
        /// Creates a brand new project.
        /// </summary>
        /// <param name="projectName">The user-perceived name for the project.</param>
        /// <param name="layer">The map layer the project is for.</param>
        /// <returns>Information describing the state of the project.</returns>
        internal IProjectInfo CreateProject(string projectName, ILayer layer)
        {
            return null;
        }

        /// <summary>
        /// Opens an editing project that was previously created.
        /// </summary>
        /// <param name="projectName">The name of the project</param>
        /// <returns>Information describing the state of the project (null if it could not be found).</returns>
        internal IProjectInfo OpenProject(string projectName)
        {
            return null;
        }
    }
}
