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
    abstract class Operation : Change, IFeatureDependent
    {
        #region Class data

        /// <summary>
        /// The session in which this operation was originally defined (not null)
        /// </summary>
        readonly Session m_Session;

        /// <summary>
        /// Flag bits
        /// </summary>
        OperationFlag m_Flag;

        /// <summary>
        /// The number of the data file that was used to initially store this edit.
        /// This will be defined only for edits in the current working session (for
        /// edits that are part of historic sessions, this will always be zero).
        /// </summary>
        /// <remarks>The file number is the same as the internal ID of the change, plus
        /// the number of further IDs used by the edit.</remarks>
        internal uint FileNumber { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Operation"/> class that will be
        /// included in the current editing session.
        /// </summary>
        protected Operation()
            : base()
        {
            m_Session = CadastralMapModel.Current.WorkingSession;
            if (m_Session == null)
                throw new ArgumentNullException();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Operation"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        protected Operation(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
            editDeserializer.CurrentEdit = this;
            m_Session = editDeserializer.MapModel.LastSession;
            Debug.Assert(m_Session != null);
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
        /// The session in which this operation was originally defined (never null).
        /// </summary>        
        internal Session Session
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
        abstract internal void Undo();

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

        #region IFeatureDependent Members

        /// <summary>
        /// Performs any processing that needs to be done just before the position of
        /// a referenced feature is changed.
        /// </summary>
        /// <param name="f">The feature that is about to be moved (not null).</param>
        /// <param name="ctx">The context in which the move is being made (not null).</param>
        public void OnFeatureMoving(Feature f, UpdateEditingContext context)
        {
            // Do nothing
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
        public override void AddReferences()
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
                if (f == null)
                {
                    // TESTING - allow nulls for now
                    int junk = 0;
                    //continue;
                }

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

                // Define user-perceived IDs if necessary
                CreateIds(feats);

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

        /// <summary>
        /// Creates user-perceived IDs for features that should have one.
        /// </summary>
        /// <param name="features">The features to assign IDs to</param>
        /// <remarks>This implementation does nothing. The <see cref="LineSubdivisionOperation"/>
        /// and <see cref="PathOperation"/> class override.</remarks>
        internal virtual void CreateIds(Feature[] features)
        {
            // Do nothing
        }

        /// <summary>
        /// Exchanges update items that were previously generated.
        /// </summary>
        /// <param name="data">The update data to apply to this edit (modified to
        /// hold the values that were previously defined for this edit)</param>
        /// <exception cref="NotImplementedException">Thrown always. Derived classes that are
        /// update capable must override (I'm not sure why this isn't part of the IRevisable
        /// interface)</exception>
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
        public override void WriteData(EditSerializer editSerializer)
        {
            base.WriteData(editSerializer);
        }

        /// <summary>
        /// Obtains a position for re-centering a draw so that new features created by this operation will show.
        /// Any redraw must be performed at the current draw scale, so this may be impossible to achieve.
        /// </summary>
        /// <param name="currentWindow">The current draw window</param>
        /// <returns>The position for the new center (null if no re-centering is needed)</returns>
        /// <remarks>
        /// This implementation just uses the window of any features created by this operation.
        /// Derived classes may override (e.g. see <see cref="IntersectOperation"/>).
        /// </remarks>
        internal virtual IPosition GetRecenter(IWindow currentWindow)
        {
            // Get the window of the features that were created by this operation.
            IWindow editExtent = GetFeatureExtent();

            // No recenter if the feature window is enclosed by the supplied draw window
            if (editExtent.IsEnclosedBy(currentWindow))
                return null;
            else
                return editExtent.Center;
        }

        /// <summary>
        /// Obtains the spatial extent of the features created by this edit.
        /// </summary>
        /// <returns>The extent of features created by this edit (null if no features were created)</returns>
        IWindow GetFeatureExtent()
        {
            Window result = new Window();

            foreach (Feature f in this.Features)
                result.Union(f.Extent);

            return (result.IsEmpty ? null : result);
        }
    }
}
