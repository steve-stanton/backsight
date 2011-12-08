// <remarks>
// Copyright 2011 - Steve Stanton. This file is part of Backsight
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

using Backsight.Editor.Properties;

namespace Backsight.Editor.FileStore
{
    /// <summary>
    /// A container for editing jobs that corresponds to an operating system folder.
    /// </summary>
    class JobFolder : IJobContainer
    {
        #region Class data

        /// <summary>
        /// The path for the folder that contains editing jobs (should always refers to a folder
        /// that exists).
        /// </summary>
        readonly string m_FolderName;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JobFolder"/> class that corresponds to C:\Backsight.
        /// If the folder does not already exist, an attempt to create it will be made.
        /// </summary>
        internal JobFolder()
        {
            m_FolderName = @"C:\Backsight";
            if (!Directory.Exists(m_FolderName))
                Directory.CreateDirectory(m_FolderName);
        }

        #endregion

        #region IJobContainer Members

        /// <summary>
        /// Creates a brand new job.
        /// </summary>
        /// <param name="jobName">The user-perceived name for the job.</param>
        /// <returns>
        /// Information describing the state of the job.
        /// </returns>
        public IJobInfo CreateJob(string jobName)
        {
            string jobFolder = Path.Combine(m_FolderName, jobName);
            if (Directory.Exists(jobFolder))
                throw new ArgumentException("Specified editing job already exists");

            Directory.CreateDirectory(jobFolder);

            // Save the information in a job file
            string jobFileName = Path.Combine(jobFolder, jobName + ".cedx");
            JobFileInfo jfi = new JobFileInfo();
            IJobInfo result = JobFile.SaveJobFile(jobFileName, jfi);

            Settings.Default.LastMap = jobName;
            Settings.Default.Save();

            return result;
        }

        /// <summary>
        /// Opens an editing job that was previously created.
        /// </summary>
        /// <param name="jobName">The name of the job</param>
        /// <returns>
        /// Information describing the state of the job.
        /// </returns>
        public IJobInfo OpenJob(string jobName)
        {
            string jobFolder = Path.Combine(m_FolderName, jobName);
            if (!Directory.Exists(jobFolder))
                throw new ArgumentException("Editing job does not exist");

            Settings.Default.LastMap = jobName;
            Settings.Default.Save();

            return null;
        }

        /// <summary>
        /// Obtains a list of all previously created editing jobs.
        /// </summary>
        /// <returns>
        /// The names of all editing jobs in this container.
        /// </returns>
        public string[] FindAllJobNames()
        {
            return Directory.GetDirectories(m_FolderName);
        }

        /// <summary>
        /// Loads a map model with the content of this container.
        /// </summary>
        /// <param name="jobName">The name of the job to load</param>
        /// <param name="mapModel">The model to load</param>
        void IJobContainer.LoadModel(string jobName, CadastralMapModel mapModel)
        {
            throw new NotImplementedException();
        }

        public ISession[] LoadSessions(string jobName, CadastralMapModel model)
        {
            EditDeserializer editDeserializer = new EditDeserializer(model);

            List<ISession> result = new List<ISession>();
            string jobFolder = Path.Combine(m_FolderName, jobName);

            foreach (string sFolder in Directory.GetDirectories(jobFolder, "s.*"))
            {
                Operation[] edits = LoadEdits(sFolder, editDeserializer);
                ISession s = new SessionFolder(sFolder, edits);
            }

            return result.ToArray();
        }

        Operation[] LoadEdits(string sFolder, EditDeserializer editDeserializer)
        {
            List<Operation> edits = new List<Operation>();

            foreach (string editFile in Directory.GetFiles(sFolder))
            {
                using (TextReader tr = File.OpenText(editFile))
                {
                    editDeserializer.SetReader(new TextEditReader(tr));
                    Operation edit = Operation.Deserialize(editDeserializer);
                    edits.Add(edit);
                }
            }

            return edits.ToArray();
        }

        #endregion
    }
}
