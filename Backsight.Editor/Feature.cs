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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;

using Backsight.Environment;
using Backsight.Forms;

namespace Backsight.Editor
{
	/// <written by="Steve Stanton" on="13-JUL-1997" />
    /// <summary>
    /// Some sort of spatial feature.
    /// <para/>
    /// Note that a polygon is not considered to be a spatial feature, but the
    /// polygon label <em>is</em> a feature. The label indicates what a polygon means
    /// in the real world, whereas a polygon in itself means nothing (consider the
    /// fact that an unlabelled polygon may often be created inadvertently during
    /// editing). Features are meant to be things that the user explicitly creates.
    /// </summary>
    [DefaultProperty("EntityType")]
    abstract class Feature : ISpatialObject, IPossibleList<Feature>, IFeature, IExpandablePropertyItem, IPersistent
    {
        #region Class data

        /// <summary>
        /// The editing operation that created this feature.
        /// </summary>
        readonly Operation m_Creator;

        /// <summary>
        /// The internal ID of this feature (holds the 1-based creation sequence
        /// of this feature within the project that created it).
        /// </summary>
        /// <remarks>The sequence value could be 0 if not yet defined (not sure if that still applies).</remarks>
        readonly InternalIdValue m_InternalId;

        /// <summary>
        /// The type of real-world object that the feature corresponds to.
        /// </summary>
        readonly IEntity m_What;

        /// <summary>
        /// Objects that reference this feature (either directly, or
        /// indirectly through some sort of <c>Observation</c> object).
        /// Null if there aren't any references.
        /// </summary>
        /// <remarks>The way this is used needs to be re-visited. As it stands, it gets used for two
        /// things: 1) For holding references from other features and edits, and 2) for holding
        /// lines that are either terminate at a point, or which pass through the point. This
        /// tends to confuse the handling of dependencies (particularly with regard to the handling
        /// of updates). It would be better to hive off the point-line relationship to the PointFeature
        /// class. The m_References list might then be built using the logic of Operation.GetRequiredEdits
        /// (for each required edit, the calling edit is a dependency).
        /// <para/>
        /// As things stand, if you do something like add a new line segment, the two end points will be
        /// referenced both to the new line, and to the editing operation that defined the line. This seems
        /// a bit verbose.
        /// </remarks>
        List<IFeatureDependent> m_References;

        /// <summary>
        /// The ID of this feature (may be shared by multiple features).
        /// </summary>
        FeatureId m_Id;

        /// <summary>
        /// Flag bits
        /// </summary>
        FeatureFlag m_Flag;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Feature"/> class, and records it
        /// as part of the map model.
        /// </summary>
        /// <param name="creator">The editing operation that created the feature (never null).</param>
        /// <param name="id">The internal of this feature within the
        /// project that created it. Specify an internal ID value of 0 for a temporary feature that should not be added
        /// to the model.</param>
        /// <param name="entityType">The type of real-world object that the feature corresponds to.</param>
        /// <param name="featureId">The user-perceived ID (if any) for the feature. This is the ID that
        /// is used to associate the feature with any miscellaneous attributes
        /// that may be held in a database.</param>
        protected Feature(Operation creator, InternalIdValue id, IEntity entityType, FeatureId featureId)
        {
            if (creator == null)
                throw new ArgumentNullException("Creating operation must be defined");

            if (entityType == null)
                throw new ArgumentNullException("Entity type must be defined");

            //if (sessionSequence == 0)
            //    throw new ArgumentException("Session sequence must be defined");

            m_Creator = creator;
            m_InternalId = id;
            m_What = entityType;
            m_References = null;
            m_Flag = 0;

            // If a user-defined ID is present, ensure it knows about this feature, and vice versa
            m_Id = featureId;
            if (m_Id != null)
                m_Id.AddReference(this);

            // Remember this feature as part of the model
            if (!m_InternalId.IsEmpty)
                m_Creator.MapModel.AddFeature(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Feature"/> class, and records it
        /// as part of the map model.
        /// </summary>
        /// <param name="f">Basic information for the feature (not null)</param>
        protected Feature(IFeature f)
            : this(f.Creator, f.InternalId, f.EntityType, f.FeatureId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Feature"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        protected Feature(EditDeserializer editDeserializer)
        {
            m_Creator = editDeserializer.CurrentEdit;
            Debug.Assert(m_Creator != null);
            ReadData(editDeserializer, out m_InternalId, out m_What, out m_Id);
            m_References = null;
            m_Flag = 0;

            // If a user-defined ID is present, ensure it knows about this feature, and vice versa
            if (m_Id != null)
                m_Id.AddReference(this);

            // Remember this feature as part of the model
            m_Creator.MapModel.AddFeature(this);
        }

        #endregion

        /// <summary>
        /// A code that identifies the spatial object type of this feature.
        /// </summary>
        abstract public SpatialType SpatialType { get; }

        /// <summary>
        /// Override returns a formatted version of this handle's unique ID.
        /// </summary>
        /// <returns>The <c>DataId</c> property</returns>
        public override string ToString()
        {
            return m_InternalId.ToString();
        }

        /// <summary>
        /// The internal ID of this feature (holds the 1-based creation sequence
        /// of this feature within the project that created it).
        /// </summary>
        public InternalIdValue InternalId
        {
            get { return m_InternalId; }
        }

        /// <summary>
        /// Hash code (for use with Dictionary) is the hash code of the <see cref="InternalId"/> property
        /// (unique within an editing project).
        /// </summary>
        /// <returns>The value to use for indexing IDs</returns>
        public override int GetHashCode()
        {
            return m_InternalId.GetHashCode();
        }

        /// <summary>
        /// The user-perceived ID (if any) for the feature. This is the ID that
        /// is used to associate the feature with any miscellaneous attributes
        /// that may be held in a database.
        /// </summary>
        [Description("Unique ID")]
        public FeatureId FeatureId
        {
            get { return m_Id; }
        }

        /// <summary>
        /// The spatial reference system for this feature (is the system associated
        /// with the editing operation that created this feature).
        /// </summary>
        [DisplayName("Coordinate system")]
        [Description("Spatial reference for the geometry")]
        public ISpatialSystem SpatialSystem
        {
            get { return (m_Creator == null ? null : m_Creator.SpatialSystem); }
        }

        /// <summary>
        /// The map model of this feature (is the model that contains the editing session
        /// in which this feature was created).
        /// </summary>
        internal CadastralMapModel MapModel
        {
            get { return (m_Creator==null ? null : (CadastralMapModel)m_Creator.Session.MapModel); }
        }

        public void RemoveIndex(Row row)
        {
            throw new NotImplementedException("Feature.RemoveIndex");
        }

        public void AddIndex(Row row)
        {
            throw new NotImplementedException("Feature.AddIndex");
        }

        #region Implement IPossibleList<Feature>

        [Browsable(false)]
        public int Count { get { return 1; } }

        public Feature this[int index]
        {
            get
            {
                if (index!=0)
                    throw new ArgumentOutOfRangeException();

                return this;
            }
        }

        public IEnumerator<Feature> GetEnumerator()
        {
            yield return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator(); // not this one, the other one
        }

        public IPossibleList<Feature> Add(Feature thing)
        {
            return new BasicList<Feature>(this, thing);
        }

        public IPossibleList<Feature> Remove(Feature thing)
        {
            if (!Object.ReferenceEquals(this, thing))
                throw new ArgumentException();

            return null;
        }

        #endregion

        /// <summary>
        /// Cuts a reference from this feature to an operation.
        /// </summary>
        /// <param name="op">The operation that makes reference to this feature.</param>
        public void CutOp(Operation op)
        {
            CutReference(op);
        }

        /// <summary>
        /// Removes a reference to a dependent.
        /// </summary>
        /// <param name="d">The object that no longer depends on this feature</param>
        internal void CutReference(IFeatureDependent d)
        {
            if (m_References!=null)
            {
                m_References.Remove(d);
                if (m_References.Count==0)
                    m_References = null;
            }
        }

        /// <summary>
        /// References this feature to the specified operation.
        /// </summary>
        /// <param name="op">The operation that makes reference to this feature.</param>
        public void AddOp(Operation op)
        {
            AddReference(op);
        }

        /// <summary>
        /// References this feature to the specified dependent.
        /// </summary>
        /// <param name="d">The object that depends on this feature.</param>
        internal void AddReference(IFeatureDependent d)
        {
            if (m_References==null)
                m_References = new List<IFeatureDependent>(1);

            if (!m_References.Contains(d))
                m_References.Add(d);
        }

        /// <summary>
        /// Initializes this feature upon loading of an editing operation that involves
        /// this feature.
        /// </summary>
        /// <param name="op">The operation involved</param>
        /// <param name="isCreator">Is the operation the one that originally created this feature?</param>
        internal virtual void OnLoad(Operation op, bool isCreator)
        {
            /*
            if (isCreator)
            {
                Debug.Assert(m_Creator==null);
                m_Creator = op;
            }
            else
            {
                if (m_References==null)
                    m_References = new List<IFeatureDependent>(1);

                m_References.Add(op);
            }
             */
        }

        /// <summary>
        /// Inserts this feature into the supplied index. This should be called shortly after a model
        /// is opened (after a prior call to <c>OnLoad</c>).
        /// </summary>
        /// <param name="index">The spatial index to add to</param>
        /// <returns>True if the feature was indexed. False if the feature is currently inactive (not
        /// added to the index)</returns>
        internal virtual bool AddToIndex(EditingIndex index)
        {
            if (IsInactive)
                return false;

            index.AddFeature(this);
            return true;
        }

        /// <summary>
        /// The editing operation that created this feature.
        /// </summary>
        public Operation Creator
        {
            get { return m_Creator; }
        }

        /// <summary>
        /// The type of real-world object that the feature corresponds to.
        /// </summary>
        public IEntity EntityType
        {
            get { return m_What; }
        }

        /// <summary>
        /// Does this feature have a foreign ID (imported from some external source).
        /// </summary>
        internal bool IsForeignId
        {
            get { return IsFlagSet(FeatureFlag.ForeignId); }
            private set { SetFlag(FeatureFlag.ForeignId, value); }
        }

        /// <summary>
        /// Records the feature ID for this feature. This function is called by
        /// <c>IdPacket.CreateId</c> and <c>IdHandle.DeleteId</c>.
        /// <para/>
        /// If a not-null ID is supplied, it will throw an exception if the feature
        /// already has an ID. To avoid that, the original ID must be successfully
        /// deleted via a prior call to <c>IdHandle.DeleteId</c>.
        /// </summary>
        /// <param name="fid">The ID to remember (may be null).</param>
        internal void SetId(FeatureId fid)
        {
        	if (fid!=null)
            {
		        // Throw exception if the feature already has an ID (a
		        // prior call to CeIdHandle::DeleteId should have been made).
        		if (m_Id!=null)
                    throw new InvalidOperationException("Feature.SetId - Attempt to re-define ID");

		        // Record the new ID, and remember whether it is regarded as
		        // a foreign ID or not.

		        m_Id = fid;
                this.IsForeignId = (fid is ForeignId);
        	}
	        else
            {
		        m_Id = null;
                this.IsForeignId = false;
        	}
        }

        /// <summary>
        /// Assigns the next available ID to this feature. The feature cannot
        /// already have an ID.
        /// </summary>
        internal void SetNextId()
        {
            if (CadastralMapModel.Current.WorkingSession != null)
            {
                // Disallow if this feature already has an ID.
                if (m_Id!=null)
                    throw new InvalidOperationException("Feature already has an ID");

                // If we can reserve a new ID, create it.
                IdHandle idh = new IdHandle();
                if (idh.ReserveId(m_What, 0))
                    idh.CreateId(this);
            }
        }

        /// <summary>
        /// Checks if this feature is topological or not.
        /// </summary>
        internal bool IsTopological
        {
            get { return IsFlagSet(FeatureFlag.Topol); }
        }

        /// <summary>
        /// Sets or clears topological status. If topology is being turned off,
        /// <c>FeatureFlag.Built</c> will also be set. If topology is being turned on
        /// <c>FeatureFlag.Built</c> will be cleared.
        /// </summary>
        /// <param name="topol">True to mark this feature as topological. False to clear topology flag.</param>
        internal virtual void SetTopology(bool topol)
        {
            SetBuilt(!topol);
            SetFlag(FeatureFlag.Topol, topol);
        }

        /// <summary>
        /// Marks a feature as "built" (meaning that all topological relationships have been defined).
        /// </summary>
        /// <param name="setting">True (default) to set the status. False to clear the status.</param>
        internal void SetBuilt(bool setting)
        {
            SetFlag(FeatureFlag.Built, setting);
        }

        /// <summary>
        /// Have topological relationships (if any) been defined for this feature?
        /// </summary>
        internal bool IsBuilt
        {
            get { return IsFlagSet(FeatureFlag.Built); }
        }

        /// <summary>
        /// Is a flag bit set?
        /// </summary>
        /// <param name="flag">The flag(s) to check for (may be a combination of more
        /// than one flag)</param>
        /// <returns>True if any of the supplied flag bits are set</returns>
        bool IsFlagSet(FeatureFlag flag)
        {
            return ((m_Flag & flag)!=0);
        }

        /// <summary>
        /// Sets flag bit(s)
        /// </summary>
        /// <param name="flag">The flag bit(s) to set</param>
        /// <param name="setting">True to set, false to clear</param>
        void SetFlag(FeatureFlag flag, bool setting)
        {
            if (setting)
                m_Flag |= flag;
            else
                m_Flag &= (~flag);
        }

        /// <summary>
        /// Has this feature been moved. This occurs during rollforward (and during rollback
        /// of a deletion). You can also mark a feature as moved to force re-calculation of
        /// topological relationships.
        /// </summary>
        internal bool IsMoved
        {
            get { return IsFlagSet(FeatureFlag.Moved); }
            set { SetFlag(FeatureFlag.Moved, value); }
        }

        /// <summary>
        /// Has this feature been marked as "trimmed".
        /// </summary>
        /// <remarks>In the past, this attribute related only to line features, and meant that
        /// a portion of the line would be flagged with a "hidden" attribute. These sections
        /// were not selectable.
        /// <para/>
        /// In the current implementation, trim status may be set for both lines and points.
        /// It acts only as a drawing hint, since visibility is determined dynamically at draw
        /// time. Sections that have been trimmed can still be selected by the user (if you know
        /// where to point).
        /// </remarks>
        /// <seealso cref="TrimLineOperation"/>
        public bool IsTrimmed
        {
            get { return IsFlagSet(FeatureFlag.Trim); }
            internal set { SetFlag(FeatureFlag.Trim, value); }
        }

        /// <summary>
        /// A string representing the key of this feature. If the feature does not
        /// have an ID, you get a blank string.
        /// </summary>
        internal string FormattedKey
        {
            get { return (m_Id==null ? String.Empty : m_Id.FormattedKey); }
        }

        /// <summary>
        /// Is this feature inactive? A feature is considered to be inactive if
        /// it has been superseded by something else (or its creating operation is being undone)
        /// </summary>
        internal bool IsInactive
        {
            get { return (IsFlagSet(FeatureFlag.Inactive) || IsFlagSet(FeatureFlag.Undoing)); }
            set { SetFlag(FeatureFlag.Inactive, value); }
        }

        /// <summary>
        /// Deactivates this feature.
        /// </summary>
        /// <remarks>This method is currently called when the model is getting loaded from
        /// the database, and it hits a DeletionOperation. I'm not sure whether it should
        /// be called during a "live" DeletionOperation</remarks>
        internal virtual void Deactivate()
        {
            IsInactive = true;
            IsMoved = false;
            SetBuilt(false);

            // Remove from spatial index
            EditingIndex index = m_Creator.MapModel.EditingIndex;
            if (index != null)
                index.RemoveFeature(this);

            // In the case of lines, this will first call the override that gets
            // rid of any topological attachments
            Clean();
        }

        /// <summary>
        /// Is the operation that created this feature being undone (see the <see cref="Undo"/> and
        /// <see cref="Restore"/> methods).
        /// </summary>
        internal bool IsUndoing
        {
            get { return IsFlagSet(FeatureFlag.Undoing); }
            set { SetFlag(FeatureFlag.Undoing, value); }
        }

        /// <summary>
        /// Marks this feature for removal (in a situation where the editing operation that
        /// created it is getting undone).
        /// </summary>
        /// <remarks>This merely sets a flag bit. The actual changes won't happen until
        /// <see cref="Clean"/> gets called.</remarks>
        internal void Undo()
        {
            SetFlag(FeatureFlag.Undoing, true);
        }

        /// <summary>
        /// Objects that reference this feature (either directly, or
        /// indirectly through some sort of <c>Observation</c> object).
        /// May be null.
        /// </summary>
        public List<IFeatureDependent> Dependents
        {
            get { return m_References; }
        }

        /// <summary>
        /// Does this feature have any dependents?
        /// </summary>
        internal bool HasDependents
        {
            get { return (m_References!=null && m_References.Count>0); }
        }

        /// <summary>
        /// The number of dependents for this feature.
        /// </summary>
        public int DependentCount
        {
            get { return (m_References==null ? 0 : m_References.Count); }
        }

        /// <summary>
        /// Draws this object on the specified display
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The drawing style</param>
        abstract public void Render(ISpatialDisplay display, IDrawStyle style);

        /// <summary>
        /// The covering rectangle that encloses this feature.
        /// </summary>
        abstract public IWindow Extent { get; }

        /// <summary>
        /// The shortest distance between this object and the specified position.
        /// </summary>
        /// <param name="point">The position of interest</param>
        /// <returns>The shortest distance between the specified position and this object</returns>
        abstract public ILength Distance(IPosition point);

        /// <summary>
        /// Performs any processing that needs to be done just before the position of
        /// a referenced feature is changed.
        /// </summary>
        /// <param name="f">The feature that is about to be moved  - something that
        /// the <c>IFeatureDependent</c> is dependent on (not null).</param>
        /// <param name="ctx">The context in which the move is being made (not null).</param>
        public virtual void OnFeatureMoving(Feature f, UpdateEditingContext ctx)
        {
            RemoveIndex();
        }

        /// <summary>
        /// Adds this feature to the spatial index, and sets the flag bit indicating
        /// that this feature is indexed.
        /// </summary>
        internal void AddToIndex()
        {
            EditingIndex index = MapModel.EditingIndex;
            if (index != null)
                index.AddFeature(this);
        }

        /// <summary>
        /// Removes spatial indexing for this feature, and clears the flag bit indicating
        /// that this feature is indexed.
        /// </summary>
        internal void RemoveIndex()
        {
            // The spatial index may be null while data is being deserialized from the
            // database during application startup

            EditingIndex index = MapModel.EditingIndex;
            if (index != null)
                index.RemoveFeature(this);
        }

        public string TypeName
        {
            get { return GetType().Name; }
        }

        /// <summary>
        /// Draws this feature on a specific map display. Not intended for bulk draws, since
        /// it creates a drawing style object on each call.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="col">The colour to use for the draw</param>
        public void Draw(ISpatialDisplay display, Color col)
        {
            IDrawStyle style = EditingController.Current.DrawStyle;
            style.LineColor = style.FillColor = col;
            Render(display, style);
        }

        public void Draw(ISpatialDisplay display, HatchStyle hs, Color foreColor)
        {
            IDrawStyle style = EditingController.Current.DrawStyle;
            style.Fill = new Fill(hs, foreColor, display.MapPanel.BackColor);
            Render(display, style);
        }

        /// <summary>
        /// Restores (un-deletes) this feature.
        /// </summary>
        /// <returns>True if feature restored. False if the feature wasn't marked as inactive.</returns>
        internal virtual bool Restore()
        {
            // Return if this feature doesn't currently have the "inactive" state
            if (!IsInactive)
                return false;

            // If this feature referred to an ID, restore it.
            if (m_Id!=null)
            {
                // Ensure that the ID is cross-referenced to this feature.
                m_Id.AddReference(this);

                // That's pretty well it. When a feature is de-activated, only the back
                // pointer from the ID is nulled out. The feature retained it's pointer to the ID,
                // and the ID itself was left in place as part of the IdPacket to which it belongs.
                // For native IDs, the packet needs to be told to decrement the number of free
                // IDs.

                // TODO: This may be irrelevant (need to also review the logic when something is
                // de-activated).

                if (m_Id is NativeId)
                {
                    IdManager idMan = MapModel.IdManager;
                    if (idMan != null)
                    {
                        // Find the ID group that applies to the feature's
                        // entity type (this will be null if the entity type was
                        // not originally listed in the IdEntity table, or the
                        // group is considered to be obsolete).
                        IdGroup g = idMan.GetGroup(this.EntityType);

                        // If we got a group (and the ID if not foreign) try to find
                        // the ID packet that refers to the feature's ID.
                        if (g != null)
                        {
                            NativeId nid = (m_Id as NativeId);
                            IdPacket p = g.FindPacket(nid);
                            p.RestoreId(m_Id);
                        }
                    }
                }
            }

            // Add back into the map index.
            MapModel.EditingIndex.AddFeature(this);

            // Remember that the feature is now active
            IsInactive = false;

            return true;
        }

        /// <summary>
        /// Ensures this feature is clean after some sort of edit. If this feature has been marked
        /// for deletion (either deletion through a <see cref="DeletionOperation"/>, or because
        /// the creating edit is being rolled back), this ensures that any associated ID object
        /// no longer refers to this feature.
        /// <para/>
        /// Any override should first do it's stuff, then call this implementation.
        /// </summary>
        internal virtual void Clean()
        {
            // Return if this feature hasn't been marked for deletion
            if (!IsUndoing)
                return;

            // If this feature is active, but it doesn't appear to be indexed,
            // do it now. This is a bit of a kludge, meant to cover the fact
            // that lines are erroneously dropping out of the index after making
            // updates.
            //if (this.IsInactive == false && this.IsIndexed == false)
            //    this.MapModel.EditingIndex.AddFeature(this);

            // Return if there is no feature ID.
            if (m_Id==null)
                return;

            // Cut the reference that the ID makes to this feature (the ID
            // may continue to point to other features).
            m_Id.CutReference(this);

            // Remove the the ID from its enclosing ID packet
            NativeId nid = (m_Id as NativeId);
            if (nid != null)
                nid.IdGroup.ReleaseId(nid);
        }

        /// <summary>
        /// Defines a new feature so it is like this one.
        /// </summary>
        /// <param name="newFeature">The new feature (fresh out of its constructor).</param>
        protected void DefineFeature(Feature newFeature)
        {
            // Same ID (if any) as this feature.
            if (m_Id!=null)
            {
                newFeature.m_Id = m_Id;
                m_Id.AddReference(newFeature);
            }

            // Those flag bits that relate to the initial state of a new feature.

            FeatureFlag newmask = ( FeatureFlag.Trim
                                  | FeatureFlag.Topol
                                  | FeatureFlag.ForeignId
                                  | FeatureFlag.Locked
                                  | FeatureFlag.Void );
            newFeature.m_Flag = (newmask & m_Flag);

            /*
	        // Hold on. If we're copying an arc that has system-defined
	        // topological sections, clear that bit and set topological
	        // bit instead.
	        if ( (m_Flag & FFL_SYSTOPOL) ) {
		        newfeat.m_Flag &= (~FFL_SYSTOPOL);
		        newfeat.m_Flag |= FFL_TOPOL;
	        }
             */

            // Non-topological stuff is always "built"
            if (!newFeature.IsTopological)
                newFeature.SetBuilt(true);
        }

        /*
        /// <summary>
        /// The layer that was active when this feature was created
        /// </summary>
        internal ILayer BaseLayer
        {
            get
            {
                // SS20090401 - The entity type can no longer be used, since entity types
                // associated with layer 0 are now considered to apply to all layers.
                // return (m_What==null ? null : m_What.Layer);

                int layerId = BaseLayerId;
                return EnvironmentContainer.FindLayerById(layerId);
            }
        }
        */

        /// <summary>
        /// Is this feature considered to be "void". This status is used to mark
        /// lines along staggered faces in a connection path. Lines that are marked
        /// thus do not get exported to AutoCad.
        /// </summary>
        internal bool IsVoid
        {
            get { return IsFlagSet(FeatureFlag.Void); }
            set { SetFlag(FeatureFlag.Void, value); }
        }

        /// <summary>
        /// Touches this feature for rollforward preview.
        /// </summary>
        /// <param name="afterOp">The edit causing the change (only edits that were performed after this
        /// edit are considered relevant).</param>
        /// <remarks>The <see cref="PointFeature"/> class overrides</remarks>
        internal virtual void Touch(Operation afterOp)
        {
            if (m_Creator.IsAfter(afterOp))
                m_Creator.SetTouch();

            if (m_References != null)
            {
                foreach (IFeatureDependent fd in m_References)
                {
                    if (fd is Operation)
                    {
                        Operation depOp = (fd as Operation);
                        if (depOp.IsAfter(afterOp))
                            depOp.SetTouch();
                    }
                }
            }
        }

        /// <summary>
        /// Has this feature been spatially indexed?
        /// </summary>
        internal bool IsIndexed
        {
            get { return IsFlagSet(FeatureFlag.Indexed); }
            set { SetFlag(FeatureFlag.Indexed, value); }
        }

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public virtual void WriteData(EditSerializer editSerializer)
        {
            editSerializer.WriteInternalId(DataField.Id, m_InternalId);
            editSerializer.WriteEntity(DataField.Entity, m_What);
            editSerializer.WriteFeatureId(m_Id);
        }

        /// <summary>
        /// Reads data that was previously written using <see cref="WriteData"/>
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        /// <param name="id">The internal ID of the feature within the project that created it.</param>
        /// <param name="entity">The type of real-world object that the feature corresponds to.</param>
        /// <param name="fid">The ID of the feature (may be null).</param>
        static void ReadData(EditDeserializer editDeserializer, out InternalIdValue id, out IEntity entity, out FeatureId fid)
        {
            id = editDeserializer.ReadInternalId(DataField.Id);
            entity = editDeserializer.ReadEntity(DataField.Entity);
            fid = editDeserializer.ReadFeatureId();
        }
    }
}
