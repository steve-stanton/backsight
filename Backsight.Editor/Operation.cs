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
using System.Diagnostics;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Data;
using System.Xml;
using System.Text;
using System.Xml.Serialization;

using Backsight.Editor.Database;
using Backsight.Data;
using Backsight.Editor.Observations;
using Backsight.Editor.Xml;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="23-OCT-1997" was="CeOperation" />
    /// <summary>
    /// Base class for any sort of editing operation.
    /// </summary>
    abstract class Operation : IFeatureDependent
    {
        #region Class data

        /// <summary>
        /// The session in which this operation was originally defined.
        /// </summary>
        Session m_Session;

        /// <summary>
        /// The item sequence number of this operation (starts at 1 for each session).
        /// </summary>
        uint m_Sequence;

        /// <summary>
        /// Flag bits
        /// </summary>
        OperationFlag m_Flag;

        /// <summary>
        /// A previous edit that this one is based on (null if this edit has never been revised).
        /// </summary>
        Operation m_Previous;

        /// <summary>
        /// The next revision of this edit (null if this edit has never been revised).
        /// </summary>
        Operation m_Next;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new editing operation as part of the current default session. This constructor
        /// should be called during de-serialization from the database.
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        /// <param name="t">The serialized version of this instance</param>
        protected Operation(Session s, OperationData t)
        {
            if (s==null || t==null)
                throw new ArgumentNullException();

            uint sessionId;
            InternalIdValue.Parse(t.Id, out sessionId, out m_Sequence);
            Debug.Assert(s.Id == sessionId);

            m_Session = s;
            m_Session.Add(this);

            // If this edit represents a revision to a previous edit, chain the
            // edits together
            if (t.PreviousId != null)
            {
                Operation prev = s.MapModel.FindOperation(t.PreviousId);
                Debug.Assert(prev!=null);
                prev.SetNextRevision(this);
            }
        }

        /// <summary>
        /// Creates a new editing operation as part of the supplied session. This constructor
        /// should be called during regular editing work.
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        protected Operation(Session s)
        {
            if (s==null)
                throw new ArgumentNullException();

            m_Session = s;
            m_Session.Add(this);
            m_Sequence = Session.ReserveNextItem();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Operation"/> class.
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        /// <param name="sequence">The sequence number of the edit within the session (specify 0 if
        /// a new sequence number should be reserved). A non-zero value is specified during
        /// deserialization from the database.</param>
        protected Operation(Session s, uint sequence)
        {
            if (s==null)
                throw new ArgumentNullException();

            m_Session = s;
            m_Session.Add(this);

            if (sequence == 0)
                m_Sequence = Session.ReserveNextItem();
            else
                m_Sequence = sequence;
        }

        #endregion

        /// <summary>
        /// Chains this editing operation with an edit that represents a revision of this edit
        /// </summary>
        /// <param name="next">The revised version of this edit</param>
        void SetNextRevision(Operation next)
        {
            Debug.Assert(m_Next==null);
            Debug.Assert(next.m_Previous==null);

            m_Next = next;
            next.m_Previous = this;
        }

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
        /// The number of features created (or deleted) by this edit. This implementation
        /// returns the number of created features - the <see cref="DeletionOperation"/>
        /// class overrides
        /// </summary>
        /// <remarks>This property is used by <see cref="SessionForm"/> to show
        /// the number of feature involved in each edit.</remarks>
        public virtual uint FeatureCount
        {
            get { return (uint)Features.Length; }
        }

        /// <summary>
        /// Operation sequence number.
        /// </summary>
        public uint EditSequence
        {
            get { return m_Sequence; }
        }

        /// <summary>
        /// The external ID of this edit is a concatenation of the
        /// <see cref="Session.Id"/> and <see cref="EditSequence"/> properties
        /// (seperated with a colon).
        /// </summary>
        internal string DataId
        {
            get { return InternalIdValue.Format(m_Session.Id, m_Sequence); }
        }

        /// <summary>
        /// Parses a string that was returned by the <see cref="DataId"/> property.
        /// </summary>
        /// <param name="s">The string to parse</param>
        /// <param name="sessionId">The ID of the session</param>
        /// <param name="sequence">The creation sequence of the edit within the session</param>
        void ParseDataId(string s, out uint sessionId, out uint sequence)
        {
            InternalIdValue.Parse(s, out sessionId, out sequence);
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

        //public void OnMove(Feature f)
        //{
        //    throw new NotImplementedException("Operation.OnMove");
        //}

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
        /// Convenience method that marks a feature as "being undone" (first checks whether
        /// the specified feature is null). This is normally called by implementations
        /// of the <c>Rollback(void)</c> method.
        /// </summary>
        /// <param name="line">The feature to mark for undo (may be null)</param>
        protected void Rollback(Feature f)
        {
            if (f!=null && f.Creator==this)
                f.Undo();
        }

        /// <summary>
        /// Rollforward this edit in response to some sort of update.
        /// </summary>
        /// <param name="uc">The context in which editing revisions are being made (not null).
        /// Used to hold a record of any positional changes.</param>
        /// <returns>True if operation has been re-executed successfully</returns>
        abstract internal bool Rollforward(UpdateContext uc);

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
        internal bool IsChanged
        {
            get { return IsFlagSet(OperationFlag.Changed); }
            set { SetFlag(OperationFlag.Changed, value); }
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

        public bool OnPreMove(Feature f)
        {
            throw new NotImplementedException("Operation.OnPreMove");
        }

        public bool OnPostMove(Feature f)
        {
            SetFlag(OperationFlag.Changed, true);
            return true;
        }

        #endregion

        /// <summary>
        /// Performs processing that should be performed to conclude an editing operation
        /// (either during initial execution, or when loading during application startup).
        /// Features that were created will initially get indexed via a call to <see cref="CadastralMapModel.AddToIndex"/>.
        /// Followed by calls to <see cref="AddReferences"/> and <see cref="PrepareForIntersect"/>.
        /// If the call is being made as part of an editing session (not application startup), a call to
        /// <see cref="CadastralMapModel.CleanEdit"/> will be made, and the edit will be saved to the database.
        /// </summary>
        protected void Complete()
        {
            // Is this method being called as part of application startup?
            bool isStartup = !Object.ReferenceEquals(m_Session, Session.WorkingSession);

            // Index features that were created (and ensure the map extent has been
            // expanded to include the new features)
            Feature[] feats = Features;

            // If we're not doing startup, attempt to associate new features with database attributes (when
            // we're doing startup, we do all the deserialzation, then do all attribute matching in one big swoop)
            if (!isStartup)
            {
                AttributeData.Load(feats);
                MapModel.AddToIndex(feats);

                // Ensure user-perceived ID objects have been indexed too
                MapModel.AddFeatureIds(feats);
            }

            // Assign 1-based creation sequence to each created feature
            uint numItem = m_Session.NumItem;
            for (uint i = 0; i < feats.Length; i++)
            {
                numItem++;
                feats[i].CreatorSequence = numItem;

                // Remember the feature as part of the map model
                MapModel.AddFeature(feats[i]);
            }

            // Ensure the item count for the session has been updated
            m_Session.NumItem = numItem;

            // Point referenced features to this editing operation
            AddReferences();

            // Mark any new topological lines as "moved" so that they will be
            // intersected with the map
            PrepareForIntersect(feats);

            if (!isStartup)
            {
                // Ensure the map structure has been updated to account for the new data.
                MapModel.CleanEdit();

                // Save the edit to the database
                SaveOperation();
            }
        }

        /// <summary>
        /// Saves an editing operation in the database. This writes to the <c>Edits</c>
        /// table and updates the timestamp in the <c>Sessions</c> table.
        /// </summary>
        /// <param name="op">The edit to save</param>
        void SaveOperation()
        {
            Trace.Write("Saving to database");

            // Save the last edit in the database
            string x = DataFactory.Instance.ToXml(this);

            // Dump the file out (to help with debugging)
            using (StreamWriter sw = File.CreateText(@"C:\Temp\LastEdit.txt"))
            {
                sw.Write(x);
            }

            // Validate the xml against the schema
            Content.Validate(x);

            Transaction.Execute(delegate
            {
                // Insert the edit
                SqlCommand c = new SqlCommand();
                c.Connection = Transaction.Connection.Value;
                c.CommandText = "INSERT INTO [ced].[Edits] ([SessionId], [EditSequence], [Data])" +
                                    " VALUES (@sessionId, @editSequence, @data)";
                c.Parameters.Add(new SqlParameter("@sessionId", SqlDbType.Int));
                c.Parameters.Add(new SqlParameter("@editSequence", SqlDbType.Int));
                c.Parameters.Add(new SqlParameter("@data", SqlDbType.Xml));
                c.Parameters[0].Value = Session.WorkingSession.Id;
                c.Parameters[1].Value = m_Sequence;
                c.Parameters[2].Value = x;
                c.ExecuteNonQuery();

                // Update the end-time associated with the session
                Session.WorkingSession.UpdateEndTime();
            });
        }

        /// <summary>
        /// Adds references to existing features referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// <para/>
        /// This is called by the <see cref="Complete"/> method, to ensure
        /// that the referenced features are cross-referenced to the editing operations
        /// that depend on them.
        /// </summary>
        abstract public void AddReferences();

        /// <summary>
        /// Prepares the supplied features for intersect detection that should be
        /// performed by <see cref="CadastralMapModel.CleanEdit"/>. This modifies
        /// line features by setting the <see cref="LineFeature.IsMoved"/> property.
        /// Called by the <see cref="Complete"/> method
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

        /// <summary>
        /// Cuts any reference made to this operation for an observed distance that was
        /// specified using an <see cref="OffsetPoint"/> object.
        /// </summary>
        /// <param name="dist">The distance observation. If this is not an <c>OffsetPoint</c>
        /// object, this function does nothing.</param>
        protected void CutOffsetRef(Observation dist)
        {
            // Return if the distance observation is not for an offset point
            OffsetPoint off = (dist as OffsetPoint);
            if (off == null)
                return;

            // Get the offset point and cut the reference it has to this operation.
            PointFeature point = off.Point;
            if (point != null)
                point.CutOp(this);
        }

        /// <summary>
        /// Performs the data processing associated with this editing operation.
        /// Edits that involve the creation of new spatial features should calculate and
        /// assign the geometry for the features.
        /// </summary>
        abstract internal void RunEdit();

        /// <summary>
        /// Attempts to locate a superseded (inactive) line that was the parent of
        /// a specific line.
        /// </summary>
        /// <param name="line">The line of interest</param>
        /// <returns>A line that was superseded by this edit in order to produce
        /// the line of interest.</returns>
        abstract internal LineFeature GetPredecessor(LineFeature line);

        /// <summary>
        /// Draws the features that were created by this editing operation
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The drawing style</param>
        /// <param name="drawInactive">Should features that are currently inactive be drawn too?</param>
        internal void Render(ISpatialDisplay display, IDrawStyle style, bool drawInactive)
        {
            foreach (Feature f in this.Features)
            {
                if (!drawInactive && f.IsInactive)
                    continue;

                f.Render(display, style);
            }
        }

        /// <summary>
        /// Ensures that all features created by this edit have been recorded as part of the map model.
        /// </summary>
        /// <remarks>
        /// This is a bit of a hack. It covers the fact that only one of the Feature constructors
        /// will add to the model (the other constructor cannot add to the model, because the creation
        /// sequence number may not be available at that stage). Edits that use the "bad" constructor,
        /// and that do not call the <see cref="Complete"/> method as part of their deserialization
        /// logic, will need to call this method to ensure the model is complete.
        /// </remarks>
        protected void EnsureFeaturesAreIndexed()
        {
            CadastralMapModel mapModel = this.MapModel;

            foreach (Feature f in this.Features)
            {
                if (mapModel.Find<Feature>(f.InternalId)==null)
                    mapModel.AddFeature(f);
            }
        }

        /// <summary>
        /// Sets the flag bit which indicates that this operation needs to be processed
        /// if <see cref="Touch"/> is called.
        /// </summary>
        internal void SetTouch()
        {
            SetFlag(OperationFlag.Touched, true);
        }

        /// <summary>
        /// Clears the flag bit which indicates that this operation needs to be processed
        /// if <see cref="Touch"/> is called.
        /// </summary>
        internal void UnTouch()
        {
            SetFlag(OperationFlag.Touched, false);
        }

        /// <summary>
        /// Has this edit been marked as "touched" (via a prior call to <see cref="SetTouch"/>).
        /// </summary>
        bool IsTouched
        {
            get { return IsFlagSet(OperationFlag.Touched); }
        }

        /// <summary>
        /// Touches this operation for rollforward preview.
        /// </summary>
        /// <param name="deps">The dependent edits (those that have been touched). This edit
        /// will be appended so long as <see cref="SetTouch"/> has been called.</param>
        /// <returns>True if this edit was appended to the list of dependents. False if this edit
        /// was not marked via a prior call to <see cref="SetTouch"/>.</returns>
        internal bool Touch(List<Operation> deps)
        {
            // Return if there is no need to touch this op.
            if (!IsTouched)
                return false;

            // Touch the features that were created by this op.
            foreach (Feature f in this.Features)
                f.Touch(this);

            // Append this operation to the list of dependent edits and clear the touch flag
            deps.Add(this);
            UnTouch();
            return true;
        }

        /// <summary>
        /// Does this edit come after the supplied edit?
        /// </summary>
        /// <param name="that">The edit to compare with</param>
        /// <returns>True if this edit was performed after the supplied edit</returns>
        internal bool IsAfter(Operation that)
        {
            if (this.m_Session.Id > that.m_Session.Id)
                return true;

            if (this.m_Session.Id < that.m_Session.Id)
                return false;

            Debug.Assert(this.m_Session.Id == that.m_Session.Id);
            return (this.m_Sequence > that.m_Sequence);
        }

        /// <summary>
        /// Applies an update to this edit (by modifying the parameters that describe
        /// the edit). To re-execute the edit and propogate any positional changes, you
        /// must make a subsequent call to <see cref="CadastralMapModel.Rollforward"/>.
        /// </summary>
        /// <param name="ut">Information about the update (expected to have a type that is
        /// consistent with this editing operation)</param>
        /// <returns>The parameters this editing operation originally had (before the
        /// supplied information was applied). Holding on to this information makes it
        /// possible to later revert things to the way they were originally.</returns>
        public virtual UpdateData ApplyUpdate(UpdateData ut)
        {
            // This method should be declared by the IRevisable interface once it
            // has been implemented by derived classes. At that time, this implementation
            // should be discarded.

            return null;
        }

        /// <summary>
        /// A previous edit that this one is based on (null if this edit has never been revised).
        /// </summary>
        internal Operation Previous
        {
            get { return m_Previous; }
        }
    }
}
