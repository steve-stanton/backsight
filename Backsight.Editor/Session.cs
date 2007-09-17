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
        /// The user logged on for the session. 
        /// </summary>
        Person m_Who;

        /// <summary>
        /// When was session started? 
        /// </summary>
        DateTime m_Start;

        /// <summary>
        /// When was session ended? 
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

            m_Model = model;
            m_Layer = activeLayer;
        }

        #endregion

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
            if (m_Operations.Count==0)
                m_Operations = null;

            s_CurrentSession = null;
        }

        public void UpdateEndTime()
        {
            m_End = DateTime.Now;
        }

        /// <summary>
        /// Adds an editing operation to this session.
        /// </summary>
        /// <param name="o">The operation to append to this session.</param>
        internal void Add(Operation o)
        {
            Debug.Assert(object.ReferenceEquals(s_CurrentSession,this));
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
            return m_Operations.Remove(o);
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

        public void GetFeatures(IList<Feature> features, SpatialType types)
        {
            if (m_Operations==null)
                return;

            foreach (Operation op in m_Operations)
            {
                Feature[] created = op.Features;
                foreach (Feature f in created)
                {
                    if ((f.SpatialType & types) != 0)
                        features.Add(f);
                }
            }
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
        /// Inserts data into the supplied index. This should be called shortly after a model
        /// is opened (after a prior call to <c>OnLoad</c>).
        /// </summary>
        /// <param name="index">The spatial index to add to</param>
        internal void AddToIndex(IEditSpatialIndex index)
        {
            if (m_Operations!=null)
            {
                foreach (Operation op in m_Operations)
                    op.AddToIndex(index);
            }
        }
    }
}
