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
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;

using Backsight.Data;
using Backsight.Editor.Database;

namespace Backsight.Editor
{
    /// <summary>
    /// An editing session.
    /// </summary>
    class Session : ISession
    {
        #region Static

        /// <summary>
        /// The current editing session
        /// </summary>
        //static Session s_CurrentSession = null;

        /*
        /// <summary>
        /// The current editing session. During initial data loading, this may actually refer
        /// to a historical session. To obtain the session to which brand new edits should
        /// be appended, you should use the <see cref="WorkingSession"/> property.
        /// </summary>
        internal static Session CurrentSession
        {
            get { return s_CurrentSession; }
        }
        */

        /*
        internal static ISession WorkingSession
        {
            get { return CadastralMapModel.Current.WorkingSession; }
        }
        */

        /*
        /// <summary>
        /// Creates a new session and remembers it as the "current" session.
        /// </summary>
        /// <param name="model">The object model containing this session. The newly created session will
        /// be appended to this model.</param>
        /// <param name="sessionData">The information selected from the database</param>
        /// <param name="user">The user who performed the session</param>
        /// <param name="job">The job the session is associated with</param>
        /// <returns>The created session (can also be subsequently accessed through the
        /// <see cref="CurrentSession"/> property</returns>
        internal static Session CreateCurrentSession(CadastralMapModel model, SessionData sessionData, IUser user, JobFile job)
        {
            s_CurrentSession = new Session(model, sessionData, user, job);
            model.AddSession(s_CurrentSession);
            return s_CurrentSession;
        }
        */

        /*
        /// <summary>
        /// Nulls out the <see cref="CurrentSession"/> property. This should be called only when
        /// the editing application is shuting down.
        /// </summary>
        internal static void ClearCurrentSession()
        {
            s_CurrentSession = null;
        }
        */

        #endregion

        #region Class data

        /// <summary>
        /// The model that contains this session
        /// </summary>
        readonly CadastralMapModel m_Model;

        /// <summary>
        /// Information about the session (corresponds to a row in the <c>Sessions</c> table)
        /// </summary>
        readonly SessionData m_Data;

        /// <summary>
        /// The user logged on for the session. 
        /// </summary>
        readonly User m_Who;

        /// <summary>
        /// The job the session is associated with
        /// </summary>
        readonly JobFile m_Job;

        /// <summary>
        /// Operations (if any) that were performed during the session. 
        /// </summary>
        readonly List<Operation> m_Operations;

        /// <summary>
        /// The item count when the session was last saved
        /// </summary>
        uint m_LastSavedItem;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>Session</c> and defines it as the "current" session
        /// </summary>
        /// <param name="model">The object model containing this session</param>
        /// <param name="sessionData">The information selected from the database</param>
        /// <param name="user">The user who performed the session</param>
        /// <param name="job">The job the session is associated with</param>
        /// <remarks>To be called only by <see cref="CreateCurrentSessoon"/></remarks>
        Session(CadastralMapModel model, SessionData sessionData, User user, JobFile job)
        {
            if (sessionData == null || user == null || job == null)
                throw new ArgumentNullException();

            Debug.Assert(user.UserId == sessionData.UserId);
            //Debug.Assert(job.JobId == sessionData.JobId);

            m_Model = model;
            m_Data = sessionData;
            m_Who = user;
            m_Job = job;
            m_Operations = new List<Operation>();
            m_LastSavedItem = 0;
        }

        #endregion

        /// <summary>
        /// Unique ID for the session.
        /// </summary>
        uint ISession.Id
        {
            get { return m_Data.Id; }
        }

        // TODO: I think the output was dedicated to a list of sessions to be shown
        // to the user - probably not a good idea
        public override string ToString()
        {
            return String.Format("{0} ({1})", m_Data.StartTime, m_Who);
        }

        /// <summary>
        /// The model that contains this session
        /// </summary>
        CadastralMapModel ISession.MapModel
        {
            get { return m_Model; }
        }

        /// <summary>
        /// Deletes information about this session from the database.
        /// </summary>
        void ISession.Delete()
        {
            m_Data.Delete();
        }

        /// <summary>
        /// Updates the end-time (and item count) associated with this session
        /// </summary>
        void ISession.UpdateEndTime()
        {
            m_Data.UpdateEndTime();
        }

        /*
        /// <summary>
        /// Adds an editing operation to this session.
        /// </summary>
        /// <param name="o">The operation to append to this session.</param>
        void Add(Operation o) // move to SaveOperation?
        {
            Debug.Assert(object.ReferenceEquals(s_CurrentSession,this));
            m_Operations.Add(o);
        }
        */

        ///// <summary>
        ///// Removes an editing operation from this session. This should be called
        ///// if a new edit has failed to execute as expected.
        ///// </summary>
        ///// <param name="o">The edit that has failed</param>
        ///// <returns>True if operation removed</returns>
        //internal bool Remove(Operation o)
        //{
        //    return m_Operations.Remove(o);
        //}

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
        /// The user logged on for the session. 
        /// </summary>
        User ISession.User
        {
            get { return m_Who; }
        }

        /// <summary>
        /// The job the session is associated with
        /// </summary>
        internal JobFile Job
        {
            get { return m_Job; }
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
        Operation ISession.LastOperation
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
        bool ISession.IsSaved
        {
            get { return m_LastSavedItem == m_Data.NumItem; }
        }

        /// <summary>
        /// Records the fact that this session has been "saved". This doesn't actually
        /// save anything, since that happens each time you perform an edit.
        /// </summary>
        void ISession.SaveChanges()
        {
            // Update the number of the last saved item (as far as the user's session
            // is concerned). Note that m_Data.NumItem corresponds to what's already
            // been saved in the database (well, it should).
            m_LastSavedItem = m_Data.NumItem;

            // Save the job file for good measure. If the user looks at the file
            // timestamp, this will reassure them that something really has been done!
            EditingController.Current.JobInfo.Save();
        }

        /// <summary>
        /// Gets rid of edits that the user has not explicitly saved.
        /// </summary>
        void ISession.DiscardChanges()
        {
            m_Data.DiscardEdits(m_LastSavedItem);
        }

        /*
        /// <summary>
        /// Reserves an item number for use with the current session. It is a lightweight
        /// request, because it just increments a counter. The database gets updated
        /// when an edit completes.
        /// </summary>
        /// <returns>The reserved item number</returns>
        internal static uint ReserveNextItem()
        {
            return s_CurrentSession.m_Data.ReserveNextItem();
        }
        */

        /// <summary>
        /// Reserves an item number for use with the current session. It is a lightweight
        /// request, because it just increments a counter. The database gets updated
        /// when an edit completes.
        /// </summary>
        /// <returns>The reserved item number</returns>
        uint ISession.AllocateNextItem()
        {
            return m_Data.ReserveNextItem();
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
        int ISession.OperationCount
        {
            get { return m_Operations.Count; }
        }

        /// <summary>
        /// When was session started? 
        /// </summary>
        DateTime ISession.StartTime
        {
            get { return m_Data.StartTime; }
        }

        /// <summary>
        /// When was the last edit performed?
        /// </summary>
        DateTime ISession.EndTime
        {
            get { return m_Data.EndTime; }
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

        /// <summary>
        /// The edits performed in this session.
        /// </summary>
        Operation[] ISession.Edits
        {
            get { return m_Operations.ToArray(); }
        }

        /// <summary>
        /// Saves an editing operation as part of this session.
        /// </summary>
        /// <param name="edit">The edit to save</param>
        void ISession.SaveOperation(Operation edit)
        {
            Trace.Write("Saving edit");

            // Save the last edit in a file
            string editString = edit.GetEditString();

            // Dump the file out (to help with debugging)
            using (StreamWriter sw = File.CreateText(@"C:\Temp\LastEdit.txt"))
            {
                sw.Write(editString);
            }

            Transaction.Execute(delegate
            {
                // Insert the edit
                SqlCommand c = new SqlCommand();
                c.Connection = Transaction.Connection.Value;
                c.CommandText = "INSERT INTO [ced].[Edits] ([SessionId], [EditSequence], [Data])" +
                                    " VALUES (@sessionId, @editSequence, @data)";
                c.Parameters.Add(new SqlParameter("@sessionId", SqlDbType.Int));
                c.Parameters.Add(new SqlParameter("@editSequence", SqlDbType.Int));
                c.Parameters.Add(new SqlParameter("@data", SqlDbType.Text));
                c.Parameters[0].Value = CadastralMapModel.Current.WorkingSession.Id;
                c.Parameters[1].Value = edit.EditSequence;
                c.Parameters[2].Value = editString;
                c.ExecuteNonQuery();

                // Update the end-time associated with the session
                m_Data.UpdateEndTime();
            });

            // Remember the edit as part of the session
            m_Operations.Add(edit);
        }
    }
}
