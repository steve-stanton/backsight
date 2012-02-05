// <remarks>
// Copyright 2007 - Steve Stanton. This file is part of Backsight
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
using System.IO;

namespace Backsight.Editor
{
    /// <summary>
    /// An editing session.
    /// </summary>
    class Session
    {
        #region Class data

        /// <summary>
        /// The project the session is part of.
        /// </summary>
        readonly Project m_Project;

        /// <summary>
        /// The name of the file holding the session event data.
        /// </summary>
        readonly string m_FileName;

        /// <summary>
        /// Information about the session (corresponds to a row in the <c>Sessions</c> table)
        /// </summary>
        readonly NewSessionEvent m_Data;

        /// <summary>
        /// Operations (if any) that were performed during the session. 
        /// </summary>
        readonly List<Operation> m_Operations;

        /// <summary>
        /// The item count when the session was last saved (as far as the user is concerned)
        /// </summary>
        // This should probably be in the EditingController class
        uint m_LastSavedItem;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>Session</c> and defines it as the "current" session
        /// </summary>
        /// <param name="project">The project the session is part of</param>
        /// <param name="sessionData">Information about the session</param>
        /// <param name="sessionFile">The name of the file holding the session data</param>
        internal Session(Project project, NewSessionEvent sessionData, string sessionFile)
        {
            if (sessionData == null || project == null)
                throw new ArgumentNullException();

            m_Data = sessionData;
            m_FileName = sessionFile;
            m_Project = project;
            m_Operations = new List<Operation>();
            m_LastSavedItem = sessionData.EditSequence;
        }

        #endregion

        /// <summary>
        /// Unique ID for the session.
        /// </summary>
        internal uint Id
        {
            get { return m_Data.EditSequence; }
        }

        // TODO: I think the output was dedicated to a list of sessions to be shown
        // to the user - probably not a good idea
        public override string ToString()
        {
            return String.Format("{0} ({1})", m_Data.StartTime, m_Data.UserName);
        }

        /// <summary>
        /// The model that contains this session
        /// </summary>
        internal CadastralMapModel MapModel
        {
            get { return m_Project.Model; }
        }

        /// <summary>
        /// Updates the end-time associated with this session
        /// </summary>
        internal void UpdateEndTime()
        {
            m_Data.EndTime = DateTime.Now;
            WriteFile();
        }

        /// <summary>
        /// Serializes session data to file.
        /// </summary>
        internal void WriteFile()
        {
            string s = EditSerializer.GetSerializedString<Change>(DataField.Edit, m_Data);
            File.WriteAllText(m_FileName, s);
        }

        /// <summary>
        /// Cuts an operation from this session.
        /// </summary>
        /// <param name="o">The operation to remove.</param>
        /// <returns>True if the operation was removed, false if the operation could
        /// not be found.</returns>
        bool CutOperation(Operation o)
        {
            // Traverse the list of operations from the end, since we should probably be cutting
            // in reverse sequence.

            int index = m_Operations.FindLastIndex(delegate(Operation t) { return object.ReferenceEquals(o,t); });
            if (index>=0)
                m_Operations.RemoveAt(index);

            return (index>=0);
        }

        /// <summary>
        /// The login name of the user logged on for the session. 
        /// </summary>
        internal string User
        {
            get { return m_Data.UserName; }
        }

        /// <summary>
        /// The job the session is associated with
        /// </summary>
        internal Project Project
        {
            get { return m_Project; }
        }

        /// <summary>
        /// Rolls back the last operation in this session. The operation will be removed from
        /// the session's operation list.
        /// </summary>
        /// <returns>-1 if last operation failed to roll back. 0 if no operation to rollback.
        /// Otherwise the sequence number of the edit that was rolled back.</returns>
        internal int Rollback()
        {
            // Return if there is nothing to rollback.
            if (m_Operations.Count==0)
                return 0;

            // Get the tail operation
            int index = m_Operations.Count-1;
            Operation op = m_Operations[index];

            // Remember the sequence number of the edit we're rolling back
            uint editSequence = op.EditSequence;

            // Rollback the operation & remove from list
            if (!op.Undo())
                return -1;

            m_Operations.RemoveAt(index);
            return (int)editSequence;
        }

        /// <summary>
        /// The last editing operation in this session (null if no edits have been performed)
        /// </summary>
        internal Operation LastOperation
        {
            get
            {
                int numOp = m_Operations.Count;
                return (numOp==0 ? null : m_Operations[numOp-1]);
            }
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
        internal bool IsSaved
        {
            get { return m_LastSavedItem == m_Project.LastItemId; }
        }

        /// <summary>
        /// Records the fact that this project has been "saved". This doesn't actually
        /// save anything, since that happens each time you perform an edit.
        /// </summary>
        internal void SaveChanges()
        {
            // Update the number of the last saved item (as far as the user's session
            // is concerned). Note that m_Data.NumItem corresponds to what's already
            // been saved in the database (well, it should).
            Project p = EditingController.Current.Project;
            m_LastSavedItem = p.LastItemId;

            // Save the project settings for good measure.
            p.SaveSettings();
        }

        /// <summary>
        /// Gets rid of edits that the user has not explicitly saved.
        /// </summary>
        internal void DiscardChanges()
        {
            string dataFolder = Path.GetDirectoryName(m_FileName);
            string[] files = Directory.GetFiles(dataFolder, "*.txt");

            foreach (string s in files)
            {
                string name = Path.GetFileNameWithoutExtension(s);
                uint fileNum;
                if (UInt32.TryParse(name, out fileNum) && fileNum > m_LastSavedItem)
                    File.Delete(s);
            }

            // Go back to the old item count (and update session time)
            UpdateEndTime();
            m_Project.SetLastItem(m_LastSavedItem);
        }

        /// <summary>
        /// Reserves an item number for use with the current session. It is a lightweight
        /// request, because it just increments a counter. The database gets updated
        /// when an edit completes.
        /// </summary>
        /// <returns>The reserved item number</returns>
        internal uint AllocateNextItem()
        {
            return m_Project.AllocateId();
        }

        /// <summary>
        /// Scans this session to note the native IDs that have been used.
        /// Before calling this method, you must make a call to <see cref="Load"/>
        /// to obtain the relevant allocations.
        /// </summary>
        /// <param name="m">The ID manager, previously initialized with relevant
        /// ID allocations</param>
        internal void LoadUsedIds(IdManager m)
        {
            IdPacket p = null;

            foreach (Operation op in m_Operations)
            {
                Feature[] fa = op.Features;
                foreach (Feature f in fa)
                    p = m.AddUsedId(f, p);
            }
        }

        /// <summary>
        /// The number of edits performed in this session
        /// </summary>
        internal int OperationCount
        {
            get { return m_Operations.Count; }
        }

        /// <summary>
        /// When was session started? 
        /// </summary>
        internal DateTime StartTime
        {
            get { return m_Data.StartTime; }
        }

        /// <summary>
        /// When was the last edit performed?
        /// </summary>
        internal DateTime EndTime
        {
            get { return m_Data.EndTime; }
        }

        /// <summary>
        /// Attempts to locate an edit within this session
        /// </summary>
        /// <param name="editSequence">The sequence number of the edit to look for</param>
        /// <returns>The corresponding editing operation (null if not found)</returns>
        internal Operation FindOperation(uint editSequence)
        {
            return m_Operations.Find(delegate(Operation o) { return o.EditSequence == editSequence; });
        }

        /// <summary>
        /// Obtains dependent edits within this session.
        /// </summary>
        /// <param name="deps">The dependent edits.</param>
        /// <param name="startOp">The first operation that should be touched (specify null
        /// for all edits in this session).</param>
        internal void Touch(List<Operation> deps, Operation startOp)
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

        /// <summary>
        /// The edits performed in this session.
        /// </summary>
        internal Operation[] Edits
        {
            get { return m_Operations.ToArray(); }
        }

        /// <summary>
        /// Saves an editing operation as part of this session.
        /// </summary>
        /// <param name="edit">The edit to save</param>
        internal void SaveOperation(Operation edit)
        {
            Trace.Write("Saving edit");

            // Save the last edit in a file
            string editString = edit.GetEditString();

            string dataFolder = Path.GetDirectoryName(m_FileName);
            string editFileName = Path.Combine(dataFolder, ProjectDatabase.GetDataFileName(m_Project.LastItemId));
            File.WriteAllText(editFileName, editString);

            // Update the end-time associated with the session
            UpdateEndTime();

            // Remember the edit as part of the session
            m_Operations.Add(edit);
        }
    }
}
