/// <remarks>
/// Copyright 2007 - Steve Stanton. This file is part of Backsight
///
/// Backsight is free software; you can redistribute it and/or modify it under the terms
/// of the GNU Lesser General Public License as published by the Free Software Foundation;
/// either version 3 of the License, or (at your option) any later version.
///
/// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
/// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU Lesser General Public License for more details.
///
/// You should have received a copy of the GNU Lesser General Public License
/// along with this program. If not, see <http://www.gnu.org/licenses/>.
/// </remarks>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections.ObjectModel;

using Backsight.Editor.Operations;
using Backsight.Environment;
using Backsight.Editor.Database;

namespace Backsight.Editor
{
    /// <summary>
    /// An editing session.
    /// </summary>
    class Session
    {
        #region Static

        /// <summary>
        /// The current editing session
        /// </summary>
        static Session s_CurrentSession = null;

        /// <summary>
        /// The current editing session
        /// </summary>
        internal static Session CurrentSession
        {
            get { return s_CurrentSession; }
            set { s_CurrentSession = value; }
        }

        #endregion

        #region Class data

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
        readonly Job m_Job;

        /// <summary>
        /// The map layer associated with the job (null until the <see cref="ActiveLayer"/>
        /// property is accessed).
        /// </summary>
        ILayer m_Layer;

        /// <summary>
        /// Operations (if any) that were performed during the session. 
        /// </summary>
        List<Operation> m_Operations;

        /// <summary>
        /// The model that contains this session
        /// </summary>
        private CadastralMapModel m_Model;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a <c>Session</c> using information retrieved from the database.
        /// This is called when historical sessions are loaded during loading of
        /// the editing model.
        /// </summary>
        /// <param name="sessionData">The information selected from the database</param>
        /// <param name="user">The user who performed the session</param>
        /// <param name="job">The job the session is associated with</param>
        internal Session(SessionData sessionData, User user, Job job)
        {
            if (sessionData == null || user == null || job == null)
                throw new ArgumentNullException();

            Debug.Assert(user.UserId == sessionData.UserId);
            Debug.Assert(job.JobId == sessionData.JobId);

            m_Data = sessionData;
            m_Who = user;
            m_Job = job;
            m_Layer = null;
        }

        #endregion

        /// <summary>
        /// Unique ID for the session.
        /// </summary>
        public uint Id
        {
            get { return m_Data.Id; }
        }

        public override string ToString()
        {
            return String.Format("{0} ({1})", m_Data.StartTime, m_Who);
        }

        internal CadastralMapModel MapModel
        {
            get { return m_Model; }
        }

        /// <summary>
        /// The map layer that was active throughout the session.
        /// </summary>
        public ILayer ActiveLayer
        {
            get
            {
                if (m_Layer == null)
                    m_Layer = EnvironmentContainer.FindLayerById(m_Job.LayerId);

                return m_Layer;
            }
        }

        /// <summary>
        /// Have any persistent objects been created via this editing session?
        /// </summary>
        public bool IsEmpty
        {
            get { return m_Data.NumItem==0; }
        }

        /// <summary>
        /// Concludes this editing session. If nothing has been created, the
        /// row in the database will be deleted. Otherwise it gets updated
        /// with the current time.
        /// </summary>
        public void End()
        {
            if (m_Data.NumItem==0)
                m_Data.Delete();
            else
                m_Data.UpdateEndTime();

            s_CurrentSession = null;
        }

        /// <summary>
        /// Updates the end-time (and item count) associated with this session
        /// </summary>
        public void UpdateEndTime()
        {
            m_Data.UpdateEndTime();
        }

        /// <summary>
        /// Adds an editing operation to this session. This is called by the constructor
        /// for the <see cref="Operation"/> class, so the operation will not necessarily
        /// know about the features involved at this stage (things like indexing the
        /// content of the operation will usually be done when the operation actually
        /// instantiates features).
        /// </summary>
        /// <param name="o">The operation to append to this session.</param>
        internal void Add(Operation o)
        {
            Debug.Assert(object.ReferenceEquals(s_CurrentSession,this));
            if (m_Operations==null)
                m_Operations = new List<Operation>();

            m_Operations.Add(o);
        }

        /// <summary>
        /// Removes an editing operation from this session. This should be called
        /// if a new edit has failed to execute as expected.
        /// </summary>
        /// <param name="o">The edit that has failed</param>
        /// <returns>True if operation removed</returns>
        internal bool Remove(Operation o)
        {
            if (m_Operations!=null)
                return m_Operations.Remove(o);
            else
                return false;
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
        /// Initializes this session upon loading of the model that contains it.
        /// </summary>
        /// <param name="container">The map model that contains this session</param>
        internal void OnLoad(CadastralMapModel container)
        {
            Debug.Assert(m_Model==null);
            m_Model = container;

            if (m_Operations!=null)
            {
                foreach (Operation op in m_Operations)
                    op.OnLoad(this);
            }
        }

        /// <summary>
        /// Inserts data into the spatial index of the map model associated with this
        /// session. This should be called shortly after a model is opened (after a prior
        /// call to <c>OnLoad</c>).
        /// </summary>
        internal void AddToIndex()
        {
            if (m_Operations!=null)
            {
                foreach (Operation op in m_Operations)
                {
                    Feature[] createdFeatures = op.Features;
                    m_Model.AddToIndex(createdFeatures);
                }
            }
        }

        /// <summary>
        /// The user logged on for the session. 
        /// </summary>
        public User User
        {
            get { return m_Who; }
        }

        /// <summary>
        /// Rolls back the last operation in this session. The operation will be removed from
        /// the session's operation list.
        /// </summary>
        /// <returns>-1 if last operation failed to roll back. 0 if no operation to rollback.
        /// Otherwise the code number that specifies the operation type.</returns>
        internal int Rollback()
        {
            // Return if there is nothing to rollback.
            if (m_Operations==null || m_Operations.Count==0)
                return 0;

            // Get the tail operation
            int index = m_Operations.Count-1;
            Operation op = m_Operations[index];

            // What sort of thing are we rolling back?
            int type = (int)op.EditId;

            // Rollback the operation & remove from list
            if (!op.Undo())
                return -1;

            m_Operations.RemoveAt(index);
            if (m_Operations.Count==0)
                m_Operations = null;

            return type;
        }

        /// <summary>
        /// Returns the editing operations recorded as part of this session.
        /// </summary>
        /// <param name="reverse">Should the list be reversed (latest edit first)</param>
        /// <returns>The edits associated with this session (never null, but may be
        /// an empty array)</returns>
        internal Operation[] GetOperations(bool reverse)
        {
            if (m_Operations==null)
                return new Operation[0];

            if (!reverse)
                return m_Operations.ToArray();

            Operation[] result = m_Operations.ToArray();
            for (int i=0, j=result.Length-1; i<j; i++, j--)
            {
                Operation temp = result[i];
                result[i] = result[j];
                result[j] = temp;
            }

            return result;
        }
    }
}
