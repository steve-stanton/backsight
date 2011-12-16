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
        internal SessionFolder(uint sessionId, string folderName)
        {
            m_SessionId = sessionId;
            m_FolderName = folderName;
            m_Operations = new List<Operation>();
            m_LastSavedItem = 0;

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
        /// Loads the edits in this session folder.
        /// </summary>
        /// <param name="editDeserializer"></param>
        internal void LoadEdits(EditDeserializer editDeserializer)
        {
            foreach (string editFile in Directory.EnumerateFiles(m_FolderName))
            {
                // Only consider those files that have names that are numbers (the edit sequence)
                string name = Path.GetFileNameWithoutExtension(editFile);
                uint seqNum;
                if (UInt32.TryParse(name, out seqNum))
                {
                    using (TextReader tr = File.OpenText(editFile))
                    {
                        editDeserializer.SetReader(new TextEditReader(tr));
                        Operation edit = Operation.Deserialize(editDeserializer);
                        Debug.Assert(seqNum == edit.EditSequence);
                        m_Operations.Add(edit);
                    }
                }
            }
        }

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
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            settings.OmitXmlDeclaration = true;

            // Create the XmlWriter object and write some content.
            using (XmlWriter writer = XmlWriter.Create(GetInfoSpec(), settings))
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
        /// Deletes information about this session from the database.
        /// </summary>
        void ISession.Delete()
        {
            if (Directory.Exists(m_FolderName))
                Directory.Delete(m_FolderName, true);
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

        /*
        /// <summary>
        /// Adds an editing operation to this session.
        /// </summary>
        /// <param name="o">The operation to append to this session.</param>
        void ISession.Add(Operation o)
        {
            m_Operations.Add(o);
        }
        */

        /// <summary>
        /// Saves an editing operation as part of this session.
        /// </summary>
        /// <param name="edit">The edit to save</param>
        void ISession.SaveOperation(Operation edit)
        {
            Trace.Write("Saving edit");

            // Save the last edit in a file
            string editString = edit.GetEditString();
            string fileName = GetEditFileName(edit.EditSequence);
            File.WriteAllText(fileName, editString);

            // Remember current time as part of the info file
            m_Info.EndTime = DateTime.Now;
            WriteInfo();

            m_Operations.Add(edit);
        }

        string GetEditFileName(uint editSequence)
        {
            string editName = String.Format("{0:000000}", editSequence);
            return Path.Combine(m_FolderName, editName + ".txt");
        }

        /// <summary>
        /// Have edits performed as part of this session been "saved" (as far as
        /// the user is concerned). Each time a user performs an edit, the database
        /// will always get updated -- this is done mainly to ensure that work will
        /// not get lost due to things like power failures, or unexpected crashes.
        /// <para/>
        /// In an attempt to simulate a file-based system (which users will probably
        /// be more familiar with), the Editor checks whether the user has actually
        /// used the File-Save command to save changes. If they have not, the application
        /// is expected to ask the user whether they want to keep their changes or not.
        /// If no, the data already saved needs to be discarded.
        /// <para/>
        /// This isn't exactly thorough (e.g. if the Editor really does crash, your
        /// changes will have been saved - so be glad).
        /// </summary>
        bool ISession.IsSaved
        {
            get { return m_LastSavedItem == m_Info.NumItem; }
        }

        /// <summary>
        /// Records the fact that this session has been "saved". This doesn't actually
        /// save anything, since that happens each time you perform an edit.
        /// </summary>
        void ISession.SaveChanges()
        {
            // Update the number of the last saved item (as far as the user's session
            // is concerned). Note that m_Info.NumItem corresponds to what's already
            // been saved in the session folder (well, it should).
            m_LastSavedItem = m_Info.NumItem;

            // Save the job file for good measure.
            EditingController.Current.JobInfo.Save();
        }

        /// <summary>
        /// Rolls back the last operation in this session. The operation will be removed from
        /// the session's operation list.
        /// </summary>
        /// <returns>-1 if last operation failed to roll back. 0 if no operation to rollback.
        /// Otherwise the sequence number of the edit that was rolled back.</returns>
        /// <exception cref="InvalidOperationException">If the session has been published</exception>
        int ISession.Rollback()
        {
            // Return if there is nothing to rollback.
            if (m_Operations.Count == 0)
                return 0;

            // Get the tail operation
            int index = m_Operations.Count - 1;
            Operation op = m_Operations[index];

            // Remember the sequence number of the edit we're rolling back
            uint editSequence = op.EditSequence;

            // Rollback the operation & remove from list
            if (!op.Undo())
                return -1;

            m_Operations.RemoveAt(index);

            // Get rid of the file that contains a description of the edit (if a redo capability
            // is introduced, it's probably best to recreate the file as part of that logic).
            DeleteEditFile(editSequence);

            return (int)editSequence;
        }

        void DeleteEditFile(uint editSequence)
        {
            string fileName = GetEditFileName(editSequence);

            if (File.Exists(fileName))
                File.Delete(fileName);
        }

        /// <summary>
        /// Gets rid of edits that the user has not explicitly saved.
        /// </summary>
        void ISession.DiscardChanges()
        {
            // Get rid of edits subsequent to the last user-perceived save
            for (uint i = m_LastSavedItem + 1; i <= m_Info.NumItem; i++)
                DeleteEditFile(i);

            // Go back to the old item count (and update session time)
            m_Info.NumItem = m_LastSavedItem;
            (this as ISession).UpdateEndTime();
        }

        /// <summary>
        /// Attempts to locate an edit within this session
        /// </summary>
        /// <param name="editSequence">The sequence number of the edit to look for</param>
        /// <returns>The corresponding editing operation (null if not found)</returns>
        Operation ISession.FindOperation(uint editSequence)
        {
            return m_Operations.Find(delegate(Operation o) { return o.EditSequence == editSequence; });
        }

        /// <summary>
        /// Obtains dependent edits within this session.
        /// </summary>
        /// <param name="deps">The dependent edits.</param>
        /// <param name="startOp">The first operation that should be touched (specify null
        /// for all edits in this session).</param>
        void ISession.Touch(List<Operation> deps, Operation startOp)
        {
            // If a starting op has been specified, process only from there
            if (startOp != null)
            {
                int opIndex = m_Operations.FindIndex(delegate(Operation o)
                {
                    return object.ReferenceEquals(o, startOp);
                });

                if (opIndex < 0)
                    throw new Exception("Cannot locate starting edit within session");

                try
                {
                    // Touch the starting edit
                    startOp.SetTouch();

                    // Process the edits within this session, starting with the specified edit
                    for (int i = opIndex; i < m_Operations.Count; i++)
                        m_Operations[i].Touch(deps);
                }

                finally
                {
                    startOp.UnTouch();
                }
            }
            else
            {
                // Process all edits performed within this session
                foreach (Operation op in m_Operations)
                    op.Touch(deps);
            }
        }
    }
}
