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

using Backsight.Editor.Operations;
using Backsight.Environment;
using System.Collections.ObjectModel;

namespace Backsight.Editor
{
    [Serializable]
    class Session
    {
        #region Static

        /// <summary>
        /// The current editing session. Initialized on a call to <c>Start</c>, cleared
        /// on a call to <c>End</c>.
        /// </summary>
        private static Session s_CurrentSession = null;

        public static Session CurrentSession
        {
            get { return s_CurrentSession; }
        }

        #endregion

        #region Class data

        /// <summary>
        /// A unique ID for this session
        /// </summary>
        readonly InternalIdValue m_SessionId;

        /// <summary>
        /// The user logged on for the session. 
        /// </summary>
        Person m_Who;

        /// <summary>
        /// When was session started? 
        /// </summary>
        DateTime m_Start;

        /// <summary>
        /// When was the last edit performed?
        /// </summary>
        DateTime m_End;

        /// <summary>
        /// Operations (if any) that were performed during the session. 
        /// </summary>
        List<Operation> m_Operations;

        /// <summary>
        /// The map layer that was being edited throughout this session.
        /// </summary>
        LayerFacade m_Layer;

        /// <summary>
        /// The range of layers (with a base that corresponds to <c>m_Layer</c>)
        /// </summary>
        [NonSerialized]
        private LayerRange m_Layers;

        /// <summary>
        /// The model that contains this session
        /// </summary>
        [NonSerialized]
        private CadastralMapModel m_Model;

        #endregion

        #region Constructors

        internal Session(CadastralMapModel model, LayerFacade activeLayer)
        {
            if (model==null || activeLayer==null)
                throw new ArgumentNullException();

            m_SessionId = model.CreateSessionId();
            m_Model = model;
            m_Layer = activeLayer;
        }

        #endregion

        public uint Id
        {
            get { return m_SessionId.SessionSequence; }
        }

        public InternalIdValue InternalId
        {
            get { return m_SessionId; }
        }

        public override string ToString()
        {
            return String.Format("{0} ({1})", m_Start, m_Who);
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
            get { return m_Layer; }
        }

        internal LayerRange ActiveLayerRange
        {
            get { return m_Layers; }
        }

        public bool IsEmpty
        {
            get { return (m_Operations==null || m_Operations.Count==0); }
        }

        internal void Start(Person p)
        {
            m_Who = p;
            m_Start = DateTime.Now;
            m_End = m_Start;
            m_Operations = new List<Operation>();

            s_CurrentSession = this;
        }

        public void End()
        {
            UpdateEndTime();
            if (IsEmpty)
                m_Operations = null;

            s_CurrentSession = null;
        }

        public void UpdateEndTime()
        {
            m_End = DateTime.Now;
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
            m_Layers = LayerRange.CreateRange(m_Layer);

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
        /// When was session started? 
        /// </summary>
        public DateTime StartTime
        {
            get { return m_Start; }
        }

        /// <summary>
        /// When was the last edit performed?
        /// </summary>
        public DateTime EndTime
        {
            get { return m_End; }
        }

        /// <summary>
        /// The user logged on for the session. 
        /// </summary>
        public Person User
        {
            get { return m_Who; }
        }

        /// <summary>
        /// The number of edits performed during the session.
        /// </summary>
        public int OperationCount
        {
            get { return (m_Operations==null ? 0 : m_Operations.Count); }
        }

        /// <summary>
        /// The map layer that was being edited throughout this session.
        /// </summary>
        public ILayer Layer
        {
            get { return m_Layer; }
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
