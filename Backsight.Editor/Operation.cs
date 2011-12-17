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

using Backsight.Editor.Observations;
using Backsight.Editor.Operations;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="23-OCT-1997" was="CeOperation" />
    /// <summary>
    /// Base class for any sort of editing operation.
    /// </summary>
    abstract class Operation : IFeatureDependent, IPersistent
    {
        #region Static

        /// <summary>
        /// Loads the content of an editing operation.
        /// <para/>
        /// Deprecated -- 
        /// Prior to call, the current editing session
        /// must be defined using the <see cref="Session.CurrentSession"/> property.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        /// <returns>The created editing object</returns>
        static internal Operation Deserialize(EditDeserializer editDeserializer)
        {
            Operation result = editDeserializer.ReadPersistent<Operation>(DataField.Edit);

            // Note that calculated geometry is NOT defined at this stage. That happens
            // when the model is asked to index the data.

            // Associate referenced features with the edit
            result.AddReferences();

            // If we're dealing with an update, exchange update items
            UpdateOperation upo = (result as UpdateOperation);
            if (upo != null)
                upo.ApplyChanges();

            return result;
        }

        #endregion

        #region Class data

        /// <summary>
        /// The session in which this operation was originally defined (not null)
        /// </summary>
        readonly ISession m_Session;

        /// <summary>
        /// The item sequence number of this operation (starts at 1 for each session).
        /// </summary>
        readonly uint m_Sequence;

        /// <summary>
        /// Flag bits
        /// </summary>
        OperationFlag m_Flag;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Operation"/> class that will be
        /// included in the current editing session.
        /// </summary>
        protected Operation()
        {
            m_Session = CadastralMapModel.Current.WorkingSession;
            if (m_Session == null)
                throw new ArgumentNullException();

            // The edit is now added to the session at the very end of SaveOperation
            //m_Session.Add(this);

            m_Sequence = m_Session.AllocateNextItem();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Operation"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        protected Operation(EditDeserializer editDeserializer)
        {
            editDeserializer.CurrentEdit = this;
            m_Session = editDeserializer.MapModel.LastSession;
            Debug.Assert(m_Session != null);

            string id = editDeserializer.ReadString(DataField.Id);
            uint sessionId;
            InternalIdValue.Parse(id, out sessionId, out m_Sequence);

            // Consistency check (mainly for debugging).
            if (m_Session.Id != sessionId)
                throw new ApplicationException();
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
        /// The session in which this operation was originally defined (never null).
        /// </summary>        
        internal ISession Session
        {
            get { return m_Session; }
        }

        /// <summary>
        /// The coordinate system of the map model this operation is part of.
        /// A shortcut to <see cref="MapModel.SpatialSystem"/>
        /// </summary>
        internal ISpatialSystem SpatialSystem
        {
            get { return MapModel.SpatialSystem; }
        }

        /// <summary>
        /// The map model this operation is part of.
        /// A shortcut to <see cref="Session.MapModel"/>
        /// </summary>
        internal CadastralMapModel MapModel
        {
            get { return m_Session.MapModel; }
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
        /// <returns>True if operation has been re-executed successfully</returns>
        [Obsolete]
        internal virtual bool Rollforward()
        {
            return false;
        }

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

        /*
        /// <summary>
        /// Has this operation been marked as changed. This indicates whether the
        /// operation needs to be re-executed as part of rollforward processing.
        /// </summary>
        internal bool IsChanged
        {
            get { return IsFlagSet(OperationFlag.Changed); }
            set { SetFlag(OperationFlag.Changed, value); }
        }
        */

        /*
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
        */

        #region IFeatureDependent Members

        public void OnPreMove(Feature f)
        {
        }

        public void OnPostMove(Feature f)
        {
            SetFlag(OperationFlag.Changed, true);
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
            bool isStartup = !Object.ReferenceEquals(m_Session, CadastralMapModel.Current.WorkingSession);

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

            //// Assign 1-based creation sequence to each created feature
            //uint numItem = m_Session.NumItem;
            //for (uint i = 0; i < feats.Length; i++)
            //{
            //    numItem++;
            //    feats[i].SessionSequence = numItem;

            //    // Remember the feature as part of the map model
            //    MapModel.AddFeature(feats[i]);
            //}

            //// Ensure the item count for the session has been updated
            //m_Session.NumItem = numItem;

            // Point referenced features to this editing operation
            AddReferences();

            // Mark any new topological lines as "moved" so that they will be
            // intersected with the map
            PrepareForIntersect(feats);

            if (!isStartup)
            {
                // Ensure the map structure has been updated to account for the new data.
                MapModel.CleanEdit();

                // Save the edit
                m_Session.SaveOperation(this);
            }
        }

        /// <summary>
        /// Represents this editing operation as a text string.
        /// </summary>
        /// <returns>A string that can be used to save a persistent version of this edit.</returns>
        internal string GetEditString()
        {
            EditSerializer es = new EditSerializer();
            es.WritePersistent<Operation>(DataField.Edit, this);
            return es.ToSerializedString();
        }

        /*
    /// <summary>
    /// Saves an editing operation in the database. This writes to the <c>Edits</c>
    /// table and updates the timestamp in the <c>Sessions</c> table.
    /// </summary>
    /// <param name="op">The edit to save</param>
    internal void SaveOperation()
    {
        Trace.Write("Saving to database");

        // Save the last edit in the database
        string editString = GetEditString();

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
            c.Parameters[1].Value = m_Sequence;
            c.Parameters[2].Value = editString;
            c.ExecuteNonQuery();

            // Update the end-time associated with the session
            CadastralMapModel.Current.WorkingSession.UpdateEndTime();
        });

        // Remember the edit as part of the session
        CadastralMapModel.Current.WorkingSession.Add(this);
    }
         */

        /// <summary>
        /// Adds references to existing features referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// <para/>
        /// This is called by the <see cref="Complete"/> method, to ensure
        /// that the referenced features are cross-referenced to the editing operations
        /// that depend on them.
        /// </summary>
        /// <remarks>
        /// In the case of deletions, the deleted features are <b>not</b> cross-referenced to
        /// the deletion operation. Instead, a special flag bit gets set in each feature.
        /// This is perhaps a bit inconsistent.
        /// <para/>
        /// The <see cref="DeletionOperation"/> and <see cref="UpdateOperation"/> classes provide overrides.
        /// </remarks>
        public virtual void AddReferences()
        {
            Feature[] feats = GetRequiredFeatures();

            foreach (Feature f in feats)
                f.AddReference(this);
        }

        /// <summary>
        /// Obtains the features that are referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        /// <returns>The referenced features (never null, but may be an empty array).</returns>
        abstract public Feature[] GetRequiredFeatures();

        /// <summary>
        /// Obtains the edits that must be completed before this edit.
        /// </summary>
        /// <returns>The edits that must be finished before this edit can be executed.</returns>
        /// <remarks>
        /// If no updates have ever been performed, the result will always refer to
        /// earlier edits. If updates have been applied to this edit, the result
        /// may also include future edits. This arises because an update can make
        /// use of subsequently created features (provided there is no dependency).
        /// <para/>
        /// The <see cref="UpdateOperation"/> class provides an override.
        /// </remarks>
        internal virtual Operation[] GetRequiredEdits()
        {
            List<Operation> result = new List<Operation>();

            Feature[] fa = GetRequiredFeatures();
            foreach (Feature f in fa)
            {
                Operation op = f.Creator;
                if (!result.Contains(op))
                    result.Add(op);
            }

            return result.ToArray();
        }

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
                {
                    line.IsMoved = true;

                    // Ensure any polygons in the vicinity have been marked for a rebuild - this
                    // covers new lines that are incident on existing points (where no lines will
                    // get divided).
                    line.MarkPolygons();
                }
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
        /// Performs data processing that involves creating or retiring spatial features.
        /// Newly created features will not have any definition for their geometry - a
        /// subsequent call to <see cref="CalculateGeometry"/> is needed to to that.
        /// </summary>
        /// <param name="ff">The factory class for generating any spatial features</param>
        /// <remarks>This implementation does nothing. Derived classes that need to are
        /// expected to provide a suitable override.</remarks>
        internal virtual void ProcessFeatures(FeatureFactory ff)
        {
            // Do nothing
        }

        /// <summary>
        /// Calculates the geometry for any spatial features that were created by
        /// this editing operation.
        /// </summary>
        /// <param name="ctx">The context in which the geometry is being calculated.</param>
        /// <remarks>This implementation does nothing. Derived classes that need to are
        /// expected to provide a suitable override.</remarks>
        internal virtual void CalculateGeometry(EditingContext ctx)
        {
            // Do nothing
        }

        /// <summary>
        /// Is the <see cref="OperationFlag.ToCalculate"/> flag bit set for this edit? 
        /// This does not actually mean that <see cref="CalculateGeometry"/> needs to be
        /// called - it is used to help determine the order in which edits should be calculated,
        /// which may need to be reworked whenever updates are performed.
        /// </summary>
        internal bool ToCalculate
        {
            get { return IsFlagSet(OperationFlag.ToCalculate); }
            set { SetFlag(OperationFlag.ToCalculate, value); }
        }

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
        /// Executes a brand new editing operation that is part of the working session
        /// (as opposed to a historical session that is being deserialized from the database).
        /// </summary>
        /// <param name="op">The edit to execute</param>
        /// <param name="ff">The factory for creating new spatial features (specify null if
        /// the edit is not expected to create anything).</param>
        internal void Execute(FeatureFactory ff)
        {
            Debug.Assert(m_Session == CadastralMapModel.Current.WorkingSession);
            CadastralMapModel mapModel = this.MapModel;

            // Create the spatial features
            ProcessFeatures(ff);

            // Calculate any geometry for spatial features
            CalculateGeometry(null);

            // Index features that were created (and ensure the map extent has been
            // expanded to include the new features)
            Feature[] feats = ff.CreatedFeatures;

            if (feats.Length > 0)
            {
                // Attempt to associate new features with database attributes
                AttributeData.Load(feats);
                mapModel.AddToIndex(feats);

                // Ensure user-perceived ID objects have been indexed too
                mapModel.AddFeatureIds(feats);
            }

            // Point referenced features to this editing operation
            AddReferences();

            // Mark any new topological lines as "moved" so that they will be
            // intersected with the map
            PrepareForIntersect(feats);

            // Ensure the map structure has been updated to account for the new data.
            mapModel.CleanEdit();

            // Save the edit to the database
            m_Session.SaveOperation(this);
        }

        public virtual void ExchangeData(UpdateItemCollection data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The edit that created this dependent.
        /// </summary>
        /// <value><c>this</c> edit</value>
        public Operation Creator
        {
            get { return this; }
        }

        /// <summary>
        /// Adds the features created by this edit to the model's spatial index.
        /// </summary>
        internal void AddToIndex()
        {
            EditingIndex index = this.MapModel.EditingIndex;
            Feature[] fa = this.Features;

            foreach (Feature f in fa)
                index.AddFeature(f);
        }

        /// <summary>
        /// Removes the features created by this edit from the model's spatial index.
        /// </summary>
        internal void RemoveFromIndex()
        {
            EditingIndex index = this.MapModel.EditingIndex;
            Feature[] fa = this.Features;

            foreach (Feature f in fa)
                index.RemoveFeature(f);
        }

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public virtual void WriteData(EditSerializer editSerializer)
        {
            editSerializer.WriteString(DataField.Id, DataId);
        }
    }
}
