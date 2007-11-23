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
using System.Diagnostics;
using System.Collections.Generic;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="23-OCT-1997" was="CeOperation" />
    /// <summary>
    /// Base class for any sort of editing operation.
    /// </summary>
    [Serializable]
    abstract class Operation : IFeatureDependent
    {
        #region Class data

        /// <summary>
        /// The session in which this operation was originally defined.
        /// </summary>
        //[NonSerialized]
        readonly Session m_Session;

        /// <summary>
        /// Operation sequence number. Meant to provide an easy way to check for edit
        /// dependencies. It gets initially defined by the <c>Operation</c> constructor,
        /// but does not get persisted. When a map is later re-opened, the sequence number
        /// will be re-generated by the <see cref="OnLoad"/> method.
        /// </summary>
        [NonSerialized]
        uint m_Sequence;

        /// <summary>
        /// Flag bits
        /// </summary>
        OperationFlag m_Flag;

        #endregion

        #region Constructors

        protected Operation()
        {
            m_Session = Backsight.Editor.Session.CurrentSession;
            if (m_Session==null)
                throw new ArgumentNullException("Editing session is not defined");

            m_Session.Add(this);
            m_Sequence = m_Session.MapModel.ReserveNextOpSequence();
        }

        #endregion

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        abstract public string Name { get; }

        /// <summary>
        /// Finds the observed length of a line that was created by this operation.
        /// </summary>
        /// <param name="line">The line to find</param>
        /// <returns>The observed length of the line (null if this operation doesn't
        /// reference the specified line)</returns>
        abstract internal Distance GetDistance(LineFeature line);

        /// <summary>
        /// The features created by this editing operation (may be an empty array, but
        /// never null).
        /// </summary>
        /// <remarks>
        /// All features created by an operation <b>must</b> be explicitly referenced by
        /// the concrete operation class. This is necessary because the reference that
        /// the <c>Feature</c> class has to it's creating operation is not persisted
        /// (not serialized). The <c>Operation.OnLoad</c> method is responsible for
        /// restoring the <c>Feature->Operation</c> reference, and that depends on the
        /// <c>Operation.Features</c> property.
        /// </remarks>
        abstract internal Feature[] Features { get; }

        /// <summary>
        /// The circles associated with an array of features
        /// </summary>
        /// <param name="fa">The features of interest</param>
        /// <returns>The circles (if any) that are associated with the supplied features</returns>
        List<Circle> GetCreatedCircles(Feature[] fa)
        {
            if (fa.Length==0)
                return new List<Circle>();

            // The following will be overkill in most cases, but not for things like
            // bulk data imports...

            // The circles found so far will be noted in an index that's keyed by the
            // internal ID of the point at the center of the circle.
            Dictionary<string, List<Circle>> dic = new Dictionary<string, List<Circle>>();

            List<Circle> result = new List<Circle>(100);

            foreach (Feature f in fa)
            {
                if (f is ArcFeature)
                {
                    Circle c = (f as ArcFeature).Circle;

                    if (c.Creator == f.Creator)
                    {
                        string centerPointId = c.CenterPoint.DataId;
                        bool addToResult = false;
                        List<Circle> circles;

                        if (dic.TryGetValue(centerPointId, out circles))
                        {
                            if (circles.IndexOf(c)==0)
                            {
                                circles.Add(c);
                                addToResult = true;
                            }
                        }
                        else
                        {
                            circles = new List<Circle>(1);
                            circles.Add(c);
                            dic.Add(centerPointId, circles);
                            addToResult = true;
                        }

                        if (addToResult)
                            result.Add(c);
                    }
                }
            }

            result.TrimExcess();
            return result;
        }

        /// <summary>
        /// The number of features created by this edit.
        /// </summary>
        public int FeatureCount
        {
            get { return Features.Length; }
        }

        /// <summary>
        /// Operation sequence number.
        /// </summary>
        public uint EditSequence
        {
            get { return m_Sequence; }
        }

        /// <summary>
        /// Initializes this operation upon loading of the session that contains it.
        /// Any overrides should call this version up front.
        /// </summary>
        /// <param name="container">The editing session that contains this operation</param>
        internal virtual void OnLoad(Session container)
        {
            m_Sequence = container.MapModel.ReserveNextOpSequence();

            Feature[] createdFeatures = this.Features;
            foreach (Feature f in createdFeatures)
                f.OnLoad(this, true);
        }

        /// <summary>
        /// Inserts data into the supplied index. This should be called shortly after a model
        /// is opened (after a prior call to <c>OnLoad</c>).
        /// </summary>
        /// <param name="index">The spatial index to add to</param>
        internal void AddToIndex(IEditSpatialIndex index)
        {
            Feature[] createdFeatures = this.Features;
            foreach (Feature f in createdFeatures)
                f.AddToIndex(index);

            List<Circle> createdCircles = GetCreatedCircles(createdFeatures);
            foreach (Circle c in createdCircles)
                c.AddToIndex(index);
        }

        /// <summary>
        /// The session in which this operation was originally defined.
        /// </summary>        
        internal Session Session
        {
            get { return m_Session; }
        }

        /// <summary>
        /// The coordinate system of the map model this operation is part of.
        /// A shortcut to <see cref="MapModel.CoordinateSystem"/>
        /// </summary>
        internal ICoordinateSystem CoordinateSystem
        {
            get { return MapModel.CoordinateSystem; }
        }

        /// <summary>
        /// The map model this operation is part of.
        /// A shortcut to <see cref="Session.MapModel"/>
        /// </summary>
        internal CadastralMapModel MapModel
        {
            get { return (m_Session==null ? null : m_Session.MapModel); }
        }

        public void OnMove(Feature f)
        {
            throw new NotImplementedException("Operation.OnMove");
        }

        /// <summary>
        /// Convenience method that marks a line as "moved" (first checks whether
        /// the specified line is null). This is normally called by implementations
        /// of the <c>Intersect</c> method.
        /// </summary>
        /// <param name="line">The line to mark as moved (may be null)</param>
        protected void SetMoved(LineFeature line)
        {
            if (line!=null)
                line.IsMoved = true;
        }

        /// <summary>
        /// The unique identifier for this edit.
        /// </summary>
        abstract internal EditingActionId EditId { get; }

        /// <summary>
        /// Adds references to existing features referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// <para/>
        /// This should be called by the <c>Execute</c> method in derived classes, to ensure
        /// that the referenced features are cross-referenced to the editing operations
        /// that depend on them.
        /// </summary>
        abstract public void AddReferences();

        /// <summary>
        /// Rollback this operation (occurs when a user undoes the last edit).
        /// </summary>
        /// <returns>True if operation was rolled back ok</returns>
        abstract internal bool Undo();

        /// <summary>
        /// Rollback any "extra things" known to this operation. Should be called at
        /// start of implementation of the <c>Rollback</c> method.
        /// </summary>
        protected void OnRollback()
        {
            // As I recall, the only extra things were topological lines that were split
            // as a result of the edit. This should (probably) be handled some other way
            // in the new system...

            /*
	        // Return if there are no extra things.
	        if ( !m_pMore ) return;

	        // Mark the extra things for deletion.
	        CeListIter loop(m_pMore);
	        CeClass* pThing;

	        for ( pThing=(CeClass*)loop.GetHead(); pThing; pThing=(CeClass*)loop.GetNext() ) {

		        CeFeature* pFeat = dynamic_cast<CeFeature*>(pThing);
		        if ( pFeat )
			        pFeat->SetDeleted();
		        else
			        ShowMessage("CeOperation::OnRollback\nUnexpected object encountered");
	        }

	        // Delete the list.
	        delete m_pMore;
	        m_pMore = 0;
             */
        }

        /// <summary>
        /// Convenience method that marks a feature as "deleted" (first checks whether
        /// the specified feature is null). This is normally called by implementations
        /// of the <c>Rollback(void)</c> method.
        /// </summary>
        /// <param name="line">The feature to mark as deleted (may be null)</param>
        protected void Rollback(Feature f)
        {
            if (f!=null && f.Creator==this)
                f.Undo();
        }

        /// <summary>
        /// Rollforward this edit in response to some sort of update.
        /// </summary>
        /// <returns>True if operation has been re-executed successfully</returns>
        abstract internal bool Rollforward();

        /// <summary>
        /// Is a flag bit set?
        /// </summary>
        /// <param name="flag">The flag(s) to check for (may be a combination of more
        /// than one flag)</param>
        /// <returns>True if any of the supplied flag bits are set</returns>
        bool IsFlagSet(OperationFlag flag)
        {
            return ((m_Flag & flag)!=0);
        }

        /// <summary>
        /// Sets flag bit(s)
        /// </summary>
        /// <param name="flag">The flag bit(s) to set</param>
        /// <param name="setting">True to set, false to clear</param>
        void SetFlag(OperationFlag flag, bool setting)
        {
            if (setting)
                m_Flag |= flag;
            else
                m_Flag &= (~flag);
        }

        /// <summary>
        /// Has this operation been marked as changed. This indicates whether the
        /// operation needs to be re-executed as part of rollforward processing.
        /// </summary>
        protected bool IsChanged
        {
            get { return IsFlagSet(OperationFlag.Changed); }
        }

        /// <summary>
        /// Clears the "changed" status for this operation. All derived classes should
        /// call this function at the end of their implementation of <c>Rollforward</c>.
        /// </summary>
        /// <returns>True (always)</returns>
        protected bool OnRollforward()
        {
            SetFlag(OperationFlag.Changed, false);
	        return true;
        }

        #region IFeatureDependent Members

        public void OnPreMove(Feature f)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void OnPostMove(Feature f)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        /// <summary>
        /// Performs processing that should be performed at the end of each editing
        /// operation. This involves calls to <see cref="AddReferences"/> and
        /// <see cref="Intersect"/>, followed by <see cref="CadastralMapModel.CleanEdit"/>.
        /// </summary>
        protected void Complete()
        {
            // Index any circles that got created (perhaps this should have been done earlier)
            Feature[] feats = Features;
            List<Circle> circles = GetCreatedCircles(feats);
            if (circles.Count > 0)
            {
                EditingIndex index = (EditingIndex)MapModel.Index;
                foreach (Circle c in circles)
                    index.AddCircle(c);
            }

            AddReferences();
            PrepareForIntersect(feats);
            MapModel.CleanEdit();
        }

        /// <summary>
        /// Prepares the supplied features for intersect detection that should be
        /// performed by <see cref="CadastralMapModel.CleanEdit"/>. This modifies
        /// line features by setting the <see cref="LineFeature.IsMoved"/> property.
        /// </summary>
        /// <param name="fa">The features that may contain lines that need to be prepared</param>
        void PrepareForIntersect(Feature[] fa)
        {
            foreach (Feature f in fa)
            {
                LineFeature line = (f as LineFeature);
                if (line!=null && line.IsTopological)
                    line.IsMoved = true;
            }
        }
    }
}
