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
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;

namespace Backsight.Editor.FileStore
{
    class SessionFolder : ISession
    {
        #region Class data

        /// <summary>
        /// The path to the folder where the edits were loaded from.
        /// </summary>
        readonly string m_FolderName;

        /// <summary>
        /// Operations (if any) that were performed during the session. 
        /// </summary>
        readonly List<Operation> m_Operations;

        /// <summary>
        /// Unique ID for the session.
        /// </summary>
        readonly uint m_SessionId;

        /// <summary>
        /// Information about the session.
        /// </summary>
        readonly SessionInfo m_Info;

        /// <summary>
        /// The item count when the session was last saved
        /// </summary>
        uint m_LastSavedItem;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionFolder"/> class.
        /// </summary>
        /// <param name="sessionId">A numeric ID for the session</param>
        /// <param name="folderName">The path to the folder where the edits were loaded from.</param>
        /// <param name="edits">Operations (if any) that were performed during the session.</param>
        internal SessionFolder(uint sessionId, string folderName, Operation[] edits)
        {
            m_SessionId = sessionId;
            m_FolderName = folderName;
            m_Operations = new List<Operation>(edits);

            // Attempt to load info file (if not present, create one)
            string fileName = GetInfoSpec();
            if (File.Exists(fileName))
            {
                m_Info = ReadInfo();
            }
            else
            {
                m_Info = new SessionInfo()
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now,
                    UserName = System.Environment.UserName
                };

                WriteInfo();
            }
        }

        #endregion

        /// <summary>
        /// The edits performed in this session.
        /// </summary>
        Operation[] ISession.Edits
        {
            get { return m_Operations.ToArray(); }
        }

        /// <summary>
        /// Unique ID for the session.
        /// </summary>
        uint ISession.Id
        {
            get { return m_SessionId; }
        }

        /// <summary>
        /// The number of edits performed in this session
        /// </summary>
        int ISession.OperationCount
        {
	        get { return m_Operations.Count; }
        }

        /// <summary>
        /// When was session started?
        /// </summary>
        DateTime ISession.StartTime
        {
	        get { return m_Info.StartTime; }
        }

        /// <summary>
        /// When was the last edit performed?
        /// </summary>
        DateTime ISession.EndTime
        {
	        get { return m_Info.EndTime; }
        }

        /// <summary>
        /// The model that contains this session
        /// </summary>
        CadastralMapModel ISession.MapModel
        {
            get { return CadastralMapModel.Current; }
        }

        /// <summary>
        /// The user logged on for the session. 
        /// </summary>
        IUser ISession.User
        {
            get { return new User(m_Info.UserName); }
        }

        SessionInfo ReadInfo()
        {
            using (TextReader reader = new StreamReader(GetInfoSpec()))
            {
                XmlSerializer xs = new XmlSerializer(typeof(SessionInfo));
                return (SessionInfo)xs.Deserialize(reader);
            }
        }

        void WriteInfo()
        {
            using (XmlWriter writer = XmlWriter.Create(GetInfoSpec()))
            {
                XmlSerializer xs = new XmlSerializer(typeof(SessionInfo));
                xs.Serialize(writer, m_Info);
            }
        }

        string GetInfoSpec()
        {
            return Path.Combine(m_FolderName, "info.txt");
        }

        /// <summary>
        /// Records the fact that this session has been "saved". This doesn't actually
        /// save anything, since that happens each time you perform an edit.
        /// </summary>
        void ISession.SaveChanges()
        {
            throw new NotImplementedException();
            /*
            // Update the number of the last saved item (as far as the user's session
            // is concerned). Note that m_Data.NumItem corresponds to what's already
            // been saved in the database (well, it should).
            m_LastSavedItem = m_Data.NumItem;

            // Save the job file for good measure. If the user looks at the file
            // timestamp, this will reassure them that something really has been done!
            EditingController.Current.JobInfo.Save();
             */
        }

        /// <summary>
        /// Deletes information about this session from the database.
        /// </summary>
        void ISession.Delete()
        {
            throw new NotImplementedException();
            //m_Data.Delete();
        }

        /// <summary>
        /// Reserves an item number for use with the current session. It is a lightweight
        /// request, because it just increments a counter. The database gets updated
        /// when an edit completes.
        /// </summary>
        /// <returns>The reserved item number</returns>
        uint ISession.AllocateNextItem()
        {
            m_Info.NumItem++;
            return m_Info.NumItem;
        }

        /// <summary>
        /// The last editing operation in this session (null if no edits have been performed)
        /// </summary>
        Operation ISession.LastOperation
        {
            get
            {
                int numOp = m_Operations.Count;
                return (numOp == 0 ? null : m_Operations[numOp - 1]);
            }
        }

        /// <summary>
        /// Updates the end-time (and item count) associated with this session. This should be
        /// called at the end of each editing operation.
        /// </summary>
        void ISession.UpdateEndTime()
        {
            m_Info.EndTime = DateTime.Now;
            WriteInfo();
        }
    }
}
