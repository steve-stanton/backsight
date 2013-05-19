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
using System.Globalization;

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
            return String.Format("{0} ({1})", m_Data.When, m_Data.UserName);
        }

        /// <summary>
        /// The model that contains this session
        /// </summary>
        internal CadastralMapModel MapModel
        {
            get { return m_Project.Model; }
        }

        /// <summary>
        /// The login name of the user logged on for the session. 
        /// </summary>
        internal string User
        {
            get { return m_Data.UserName; }
        }

        /// <summary>
        /// The project the session is associated with
        /// </summary>
        internal Project Project
        {
            get { return m_Project; }
        }

        /// <summary>
        /// Rolls back the last operation in this session. The operation will be removed from
        /// the session's operation list.
        /// </summary>
        /// <returns>True if an edit was undone.</returns>
        internal bool Rollback()
        {
            // Return if there is nothing to rollback.
            if (m_Operations.Count==0)
                return false;

            // Get the tail operation
            int index = m_Operations.Count-1;
            Operation op = m_Operations[index];

            // Move the data file to the project undo folder
            uint fileNum = op.FileNumber;
            if (fileNum == 0)
                throw new ApplicationException("Edit does not appear to belong to the current editing session");

            string name = ProjectDatabase.GetDataFileName(fileNum);
            string sourceFileName = Path.Combine(m_Project.ProjectFolder, name);
            string destFileName = Path.Combine(m_Project.GetUndoFolder(), name);
            File.Move(sourceFileName, destFileName);

            // Remember the sequence number of the edit we're rolling back
            uint editSequence = op.EditSequence;

            // Remove the edit and undo it
            m_Operations.RemoveAt(index);
            this.MapModel.RemoveEdit(op);
            m_Project.SetLastItem(op.EditSequence - 1);
            op.Undo();

            return true;
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
                if (UInt32.TryParse(name, NumberStyles.HexNumber, null, out fileNum) && fileNum > m_LastSavedItem)
                    File.Delete(s);
            }

            // Locate the objects that represent the edits to discard, and remove them from the model.
            List<Operation> discards = m_Operations.FindAll(o => o.EditSequence > m_LastSavedItem);
            foreach (Operation edit in discards)
                MapModel.RemoveEdit(edit);

            // And remove from the session
            m_Operations.RemoveAll(o => o.EditSequence > m_LastSavedItem);

            // Go back to the old item count
            m_Project.SetLastItem(m_LastSavedItem);
        }

        /// <summary>
        /// Concludes this editing session by combining all data files, finishing off with an
        /// instance of <see cref="EndSessionEvent"/>.
        /// </summary>
        internal void EndSession()
        {
            // Pick up the files that relate to the session
            string endFolder = Path.GetDirectoryName(m_FileName);
            uint[] fileNumbers = GetFileNumbers(endFolder, m_Data.EditSequence);

            // Create an end session event
            EndSessionEvent endEvent = new EndSessionEvent(m_Project.AllocateId());
            string endFile = Path.Combine(endFolder, ProjectDatabase.GetDataFileName(endEvent.EditSequence));

            using (StreamWriter sw = File.CreateText(endFile))
            {
                foreach (uint fileNum in fileNumbers)
                {
                    string fileName = Path.Combine(endFolder, ProjectDatabase.GetDataFileName(fileNum));
                    string s = File.ReadAllText(fileName);
                    sw.Write(s);
                }

                // And finish off with the end event
                string endText = EditSerializer.GetSerializedString<Change>(DataField.Edit, endEvent);
                sw.Write(endText);
            }

            // Get rid of the files that we've just combined
            foreach (uint fileNum in fileNumbers)
            {
                string fileName = Path.Combine(endFolder, ProjectDatabase.GetDataFileName(fileNum));
                File.Delete(fileName);
            }
        }

        /// <summary>
        /// Obtains the numbers of data files in a specific folder
        /// </summary>
        /// <param name="folderName">The name of the folder to look in</param>
        /// <param name="startFileNumber">The earliest file number to pick up</param>
        /// <returns>The numbers of data files in the specified folder, starting with the specified starting
        /// file (sorted ascending).</returns>
        uint[] GetFileNumbers(string folderName, uint startFileNumber)
        {
            List<uint> result = new List<uint>(100);

            foreach (string s in Directory.GetFiles(folderName))
            {
                string name = Path.GetFileNameWithoutExtension(s);
                uint n;
                //if (name.Length == 8 && UInt32.TryParse(name, NumberStyles.HexNumber, null, out n) && n >= startFileNumber)
                if (UInt32.TryParse(name, out n) && n >= startFileNumber)
                    result.Add(n);
            }

            result.Sort();
            return result.ToArray();
        }

        /// <summary>
        /// Allocates the next available internal ID for the project that this session belongs to.
        /// This is a lightweight request, because it just increments a counter. The database gets updated
        /// when an edit completes.
        /// </summary>
        /// <returns>The allocated internal ID</returns>
        internal InternalIdValue AllocateNextId()
        {
            return new InternalIdValue(m_Project.AllocateId());
        }

        /// <summary>
        /// The number of edits performed in this session
        /// </summary>
        /// <remarks>Caution: An ID allocation is not currently regarded as an edit (perhaps it should be).</remarks>
        internal int OperationCount
        {
            get { return m_Operations.Count; }
        }

        /// <summary>
        /// The number of changes in this session (the sum of the <see cref="OperationCount"/> and
        /// <see cref="AllocationCount"/> property values).
        /// </summary>
        internal int ChangeCount
        {
            get { return m_Operations.Count + (int)AllocationCount; }
        }

        /// <summary>
        /// The number of ID allocations that were made during this session.
        /// </summary>
        internal uint AllocationCount { get; private set; }

        /// <summary>
        /// Remembers that an allocation has been made during this session.
        /// </summary>
        /// <param name="alloc">The allocation that was made</param>
        internal void AddAllocation(IdAllocation alloc)
        {
            AllocationCount++;
            m_LastSavedItem = Math.Max(m_LastSavedItem, alloc.EditSequence);
        }

        /// <summary>
        /// When was session started? 
        /// </summary>
        internal DateTime StartTime
        {
            get { return m_Data.When; }
        }

        /// <summary>
        /// When was the last edit performed?
        /// </summary>
        internal DateTime EndTime { get; set; }

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
                int opIndex = m_Operations.FindIndex(o => object.ReferenceEquals(o, startOp));
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
            string editString = edit.GetEditString();
            SaveEditString(edit, editString);
        }

        /// <summary>
        /// Saves an editing operation as part of this session (writes a file to the project folder).
        /// </summary>
        /// <param name="edit">The edit to save</param>
        /// <param name="editString">A serialized version of the edit to save</param>
        /// <remarks>This method is called directly when dealing with instances of <see cref="UpdateOperation"/>,
        /// as the serialized string must be obtained before the update is applied. In all other cases, you
        /// should use the <see cref="SaveOperation"/> to save an edit.
        /// </remarks>
        internal void SaveEditString(Operation edit, string editString)
        {
            string dataFolder = Path.GetDirectoryName(m_FileName);
            string editFileName = Path.Combine(dataFolder, ProjectDatabase.GetDataFileName(m_Project.LastItemId));
            File.WriteAllText(editFileName, editString);

            // Remember the edit as part of the session
            edit.FileNumber = m_Project.LastItemId;
            AddOperation(edit);
        }

        /// <summary>
        /// Remembers an edit created as part of this editing session (and index as part of
        /// the map model that contains this session).
        /// </summary>
        /// <param name="edit"></param>
        internal void AddOperation(Operation edit)
        {
            m_Operations.Add(edit);
            MapModel.AddEdit(edit);
        }

        /// <summary>
        /// The name of the file holding the session event data.
        /// </summary>
        internal string FileName
        {
            get { return m_FileName; }
        }
    }
}
