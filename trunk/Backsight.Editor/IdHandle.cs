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
using System.Windows.Forms;

using Backsight.Environment;

namespace Backsight.Editor
{
	/// <written by="Steve Stanton" on="17-DEC-1998" />
    /// <summary>An "ID handle" is a transient object used by software that needs to
    /// announce the possibility that it will want to assign a specific feature ID.
    /// 
    /// For example, when a dialog containing an ID field is initially
    /// presented to a user, the default ID should be shown. However, it is possible
    /// that the user could change that ID. The problem is that in order to show the
    /// default ID at all, the ID range object needs to create an empty <c>FeatureId</c>
    /// object. So if the user changes the ID (or simply cancels the dialog), we could
    /// easily end up with an unused feature ID. To avoid that, we COULD modify all the
    /// UI code, to ensure that it cleans up any IDs that it created ... but that would
    /// involve a lot of code, and could be implemented inconsistently.
    ///
    /// An ID handle gets around this problem by acting as a mechanism that the UI can
    /// use to reserve IDs without actually creating an empty <c>FeatureId</c> object.
    /// The actual creation of an ID is done by passing the ID handle into the Execute()
    /// function that is part of the <c>Operation</c> that actually creates the
    /// associated feature. This aim is to ensure that a feature ID can never be created
    /// unless the spatial feature already exists.
    /// </summary>
    /// <devnote>06-FEB-2007: The above is the original comment from 1998. What it doesn't
    /// say is why it's bad to have an unused feature ID. Perhaps it's something to do with
    /// the old persistence mechanism. If so, this class could well be irrelevant.</devnote>
    class IdHandle : IDisposable
    {
        #region Class data
        
        /// <summary>
        /// The ID group for the ID. This could be null for previously created features,
        /// which may have been imported from an external source, or which belong to
        /// obsolete ID groups.
        /// </summary>
        private IdGroup m_Group;

        /// <summary>
        /// The ID packet that contains the ID. Will be null if the group is null.
        /// </summary>
        private IdPacket m_Packet;

        /// <summary>
        /// The entity type that the ID relates to.
        /// </summary>
        private IEntity m_Entity;

        /// <summary>
        /// The feature that this ID handle relates to.
        /// </summary>
        private Feature m_Feature;

        // An ID handle can either refer to a reserved ID (a FeatureId that
        // still needs to be created), or a previously existing ID. It
        // cannot be both. Thus, if m_FeatureId is defined, m_Id must be 0,
        // and vice versa.

        /// <summary>
        /// The reserved ID (0 if not yet reserved).
        /// </summary>
        private uint m_Id;

        /// <summary>
        /// A previously created feature ID.
        /// </summary>
        private FeatureId m_FeatureId; // was m_pId

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public IdHandle()
            : this(null)
        {
        }

        /// <summary>
        /// Constructor based on a spatial feature.
        /// </summary>
        /// <param name="feat">The existing feature (if any) that this ID handle
        /// relates to (may be null). If a feature is supplied, it may or may not
        /// have an existing ID.</param>
        internal IdHandle(Feature feat)
        {
            // Start off in a pristine state.
            m_Group = null;
            m_Packet = null;
            m_Id = 0;
            m_FeatureId = null;
            m_Entity = null;
            m_Feature = feat;

            // If we actually got a feature, define some more info.
            if (m_Feature!=null)
            {
                // Get the current ID (if any).
                m_FeatureId = m_Feature.Id;

                // Note the feature's entity type.
                m_Entity = m_Feature.EntityType;

                // If there is no ID manager (e.g. the application does not need
                // to work with "official" IDs), the group and range remain null.

                IdManager idMan = m_Feature.MapModel.IdManager;
                if (idMan!=null)
                {
                    // Try to find the ID group that applies to the feature's
                    // entity type (this will be null if the entity type was
                    // not originally listed in the IdEntity table, or the
                    // group is considered to be obsolete).
                    m_Group = idMan.GetGroup(m_Entity);

                    // If we got a group (and the ID if not foreign) try to find
                    // the ID packet that refers to the feature's ID.
                    if (m_Group != null && (m_FeatureId is NativeId))
                    {
                        NativeId nid = (m_FeatureId as NativeId);
                        m_Packet = m_Group.FindPacket(nid);
                    }
                }
            }
        }

        #endregion

        internal bool IsReserved
        {
            get { return (m_Id>0); }
        }

        bool IsDefined
        {
            get { return (m_FeatureId!=null || m_Id!=0); }
        }

        /// <summary>
        /// A formatted string representing the key (if any) for this ID handle.
        /// </summary>
        public string FormattedKey
        {
            get
            {
	            // If a feature ID was previously defined for this handle,
	            // get the feature ID's key to format the result.
	            if (m_FeatureId!=null)
                    return m_FeatureId.FormattedKey;

                // If an ID has been reserved, format that as a string.
                // Otherwise it's blank.

	            if (m_Id!=0 && m_Packet!=null)
                    return m_Packet.IdGroup.FormatId(m_Id);
                else
                    return String.Empty;
            }
        }

        public void Dispose()
        {
            // Ensure that any reserved ID has been cleared (if the ID
            // handle has actually been used to create a new ID, this
            // does nothing).

            if (m_Group!=null && m_Id!=0)
                m_Group.FreeId(m_Id);
        }

        /// <summary>
        /// Reserves the next available ID for the specified entity type.
        /// Any ID previously reserved by this ID handle will be released.
        /// </summary>
        /// <param name="ent">The entity type that the ID is required for.</param>
        /// <returns>True if the ID was successfully reserved.</returns>
        bool ReserveId(IEntity ent)
        {
            return ReserveId(ent, 0);
        }

        /// <summary>
        /// Reserves a feature ID. Any ID previously reserved by this ID handle will
        /// be released.
        /// </summary>
        /// <param name="ent">The entity type that the ID is required for.</param>
        /// <param name="id">The specific ID to reserve (0 if you want the next
        /// available ID).</param>
        /// <returns>True if the ID was successfully reserved.</returns>
        internal bool ReserveId(IEntity ent, uint id)
        {
            // Get the ID manager to define the results.

            IdManager idMan = CadastralMapModel.Current.IdManager;
            if (idMan==null)
            {
                this.Reset();
                return false;
            }

            if (idMan.ReserveId(this, ent, id))
            {
                m_Entity = ent;
                return true;
            }

            string errmsg;

            if (id!=0)
                errmsg = String.Format("Failed to reserve ID {0} for '{1}'", id, ent.Name);
            else
                errmsg = String.Format("No IDs for '{0}'", ent.Name);

            MessageBox.Show(errmsg);
            return false;
        }

        /// <summary>
        /// Reserves a feature ID in a specific ID packet
        /// </summary>
        /// <devnote>This function is called by LoadIdCombo</devnote>
        /// <param name="packet">The ID packet containing the available ID.</param>
        /// <param name="ent">The entity type that the ID is for.</param>
        /// <param name="id">The available ID to reserve.</param>
        /// <returns>True if the ID was successfully reserved.</returns>
        internal bool ReserveId(IdPacket packet, IEntity ent, uint id)
        {
            // Ensure that any currently reserved ID is released.
            if (!FreeId())
            {
                MessageBox.Show("IdHandle.ReserveId - Cannot free reserved ID");
                return false;
            }

            // Just get the packet to do it.
            if (packet.ReserveId(id))
            {
                m_Group = packet.IdGroup;
                m_Packet = packet;
                m_Id = id;
                m_Entity = ent;
                return true;
            }

            this.Reset();
            return false;
        }

        /// <summary>
        /// Creates a feature ID from this ID handle. In order for this to work, a
        /// prior call to <c>IdHandle.ReserveId</c> is needed. This function should
        /// be called ONLY by <c>Operation.CreateId</c>.
        /// </summary>
        /// <param name="feature">The persistent feature that should get the created
        /// feature ID.</param>
        /// <returns>The created feature ID (if any).</returns>
        internal FeatureId CreateId(Feature feature)
        {
            // Ensure the feature is persistent.
            /*
	        if (feature.IsTransient)
            {
		        MessageBox.Show("IdHandle.CreateId - Attempt to assign ID to transient feature.");
		        return null;
	        }
             */

            // You can't create a new feature ID if it already existed.
            if (m_FeatureId!=null)
            {
                MessageBox.Show("IdHandle.CreateId - ID previously defined");
                return m_FeatureId;
            }

            // The packet has to be known.
            if (m_Group==null || m_Packet==null)
            {
                MessageBox.Show("IdHandle.CreateId - No ID group or range");
                return null;
            }

            // Get the ID group to do it.
            return m_Group.CreateId(m_Id, m_Packet, feature);
        }

        /// <summary>
        /// Frees an ID. The ID handle can either refer to a reserved ID, or a persistent ID.
        ///
        /// The only function that should attempt to free a persistent ID is <c>Feature.Clean</c>,
        /// which is called if a user-perceived deletion is performed. In that case, this
        /// function will only free the ID if the ID range has not been released.
        /// </summary>
        /// <returns>True if ID was found. False if not found (it could be a foreign ID).</returns>
        internal bool FreeId()
        {
            // If we're referring to an existing (persistent) ID
            if (m_Feature!=null)
            {
                // Return if there's no ID to free!
                if (m_FeatureId==null)
                    return false;

                // Return if it's a foreign ID.
                NativeId nid = (m_FeatureId as NativeId);
                if (nid==null)
                    return false;

                // The ID range SHOULD be known (see the constructor that
                // accepts a Feature). It might not be if one of three
                // things happened:

                // 1. Some software frigged around with the entity type
                //	  without changing the ID.
                // 2. The ID group was made obsolete after the ID was
                //    created.
                // 3. The calling function did not check whether the ID
                //    was foreign.

                if (m_Packet==null)
                    m_Packet = CadastralMapModel.Current.IdManager.FindPacket(nid); 
                /*
                if (m_Range==null)
                {
                    List<IdRange> ranges = CadastralMapModel.Current.IdRanges;
                    m_Range = ranges.Find(delegate(IdRange r) { return r.IsReferredTo(m_FeatureId); });
                }
                */

                // If we still don't know the ID packet, issue an error message and return.
                if (m_Packet==null)
                {
                    string errmsg = String.Format("Cannot free ID '{0}' (not found)", m_FeatureId.FormattedKey);
                    MessageBox.Show(errmsg);
                    return false;
                }

                // If the range has not been released, tell it to "delete"
                // the pointer it has to the ID.
                m_Packet.DeleteId(nid);
                //if (!m_Range.IsReleased)
                //    m_Range.DeleteId(m_FeatureId);
            }
            else
            {
                // The ID was just reserved, so tell the ID group to turf the reserved ID.
                if (m_Group!=null)
                    m_Group.FreeId(m_Id);
            }

            m_Group = null;
            m_Packet = null;
            m_Id = 0;
            m_Entity = null;

            return true;
        }

        /// <summary>
        /// Defines a newly reserved ID.
        /// </summary>
        /// <param name="packet">The ID packet.</param>
        /// <param name="ent">The entity type for the ID.</param>
        /// <param name="id">The raw ID number.</param>
        /// <returns>True if the ID was valid.</returns>
        internal bool Define(IdPacket packet, IEntity ent, uint id)
        {
            // You can NEVER use this handle to refer simultaneously to
            // an existing feature ID as well as a reserved ID.
            if (m_FeatureId!=null)
                return false;

            // The ID has to be valid.
            if (id==0)
                return false;

            // Remember the supplied info.
            m_Group = packet.IdGroup;
            m_Packet = packet;
            m_Id = id;
            m_Entity = ent;

            return true;
        }

        /// <summary>
        /// Loads a list with all the IDs that are available for a specific entity type.
        /// </summary>
        /// <param name="ent">The entity type to search for.</param>
        /// <param name="avail">The list to load.</param>
        /// <returns>The number of IDs that were added to the array.</returns>
        uint GetAvailIds(IEntity ent, List<uint> avail)
        {
            IdManager idMan = CadastralMapModel.Current.IdManager;
            return (idMan==null ? 0 : idMan.GetAvailIds(ent, avail));
        }

        /// <summary>
        /// Checks whether this ID handle is valid for a specific entity type.
        /// </summary>
        /// <param name="ent">The entity type to check.</param>
        /// <returns>True if this ID handle is suitable for the entity type.</returns>
        internal bool IsValidFor(IEntity ent)
        {
            IdManager idMan = CadastralMapModel.Current.IdManager;
            if (idMan==null)
                return true;

            // If this ID handle refers to an existing feature that has a
            // foreign ID, the entity type is always valid.
            if (m_Feature!=null && m_FeatureId!=null && m_Feature.IsForeignId)
                return true;

            // Try to find the ID group for the specified entity type.
            IIdGroup group = ent.IdGroup;

            // If we actually found an a group, it has to match the one
            // that we already know about.

            // If we didn't find a group, this ID is valid only if it is undefined!

            if (group!=null)
                return Object.ReferenceEquals(group, m_Group);
            else
                return (m_Id==0 && m_FeatureId==null);
        }

        /// <summary>
        /// Resets everything in the class to null values.
        /// </summary>
        void Reset()
        {
            m_Group = null;
            m_Packet = null;
            m_Entity = null;
            m_Id = 0;
            m_FeatureId = null;
        }

        /// <summary>
        /// The entity type for this ID handle. When setting, it is assumed that the entity
        /// type is consistent with the ID (as tested via a call to <c>IdHandle.IsValidFor</c>.
        /// </summary>
        internal IEntity Entity
        {
            get { return m_Entity; }
            set { m_Entity = value; }
        }

        /// <summary>
        /// Creates an ID out of some miscellaneous string. This function should be called
        /// only when importing data from an externally defined file format.
        ///
        /// This is needed because there is no guarantee that a foreign key is numeric. It
        /// might even be completely alphabetic, so it would be impossible to relate it
        /// to any ID range.
        ///
        /// In order to use this function, you must initially construct the ID handle with
        /// a reference to the feature that the ID will be for. If you don't, you'll get an
        /// error message. Given that you have done things correctly, the feature will be
        /// modified in 2 ways:
        ///
        /// 1. It will be marked as having a foreign ID.
        /// 2. It will be modified to point to the ID.
        /// </summary>
        /// <param name="keystr">The foreign key to assign.</param>
        /// <returns>The feature ID that has been created (null on error).</returns>
        internal FeatureId CreateForeignId(string keystr)
        {
            // If the feature was not supplied to the constructor, say it can't be done!
            if (m_Feature==null)
            {
                MessageBox.Show("IdHandle.CreateForeignId - Attempt to create un-attached ID");
                return null;
            }

            m_Id = 0;	// NOT a reserved ID.

            // Create the new ID (and point it to the feature).
            m_FeatureId = new ForeignId(keystr);
            m_FeatureId.Add(m_Feature);

            // Make sure we have the latest entity type for the feature (it
            // could conceivably have changed since the constructor was
            // called).
            m_Entity = m_Feature.EntityType;

            IdManager idMan = CadastralMapModel.Current.IdManager;
            if (idMan==null)
                m_Group = null;
            else
            {
                // Try to find the ID group that applies to the feature's
                // entity type (this will be null if the entity type was
                // not originally listed in the IdEntity table, or the
                // group is considered to be obsolete).
                m_Group = idMan.GetGroup(m_Entity);
            }

            // Foreign IDs cannot fall in ANY packet.
            m_Packet = null;

            return m_FeatureId;
        }

        /// <summary>
        /// Deletes the ID for the feature that was specified in this ID handle's
        /// constructor. The ID will only be deleted if nothing else is currently
        /// referenced by the ID.
        /// </summary>
        /// <returns>True if the ID has been successfully deleted. False if it
        /// could not be deleted.</returns>
        bool DeleteId()
        {
            // Confirm that the ID refers to an existing feature.
            if (m_Feature==null)
            {
                MessageBox.Show("IdHandle.DeleteId - Attempt to delete non-persistent ID");
                return false;
            }

            // Confirm that the feature's ID has not changed somehow.
            if (!Object.ReferenceEquals(m_FeatureId, m_Feature.Id))
            {
                MessageBox.Show("IdHandle.DeleteId - Inconsistent ID");
                return false;
            }

            // Ensure that the reference the ID makes to the feature
            // has been cut (it may have been done already).
            m_FeatureId.CutReference(m_Feature);

            // And null the pointer from the feature to the ID, declaring
            // that it no longer has "foreign ID" status.
            bool isForeign = m_Feature.IsForeignId;
            m_Feature.SetId(null);

            // If the ID still refers to anything, it can't be deleted.
            if (!m_FeatureId.IsInactive)
                return false;

            // If the ID is not foreign, get the ID manager to get rid
            // of the pointer to it.
            if (!isForeign)
            {
                IdManager idMan = CadastralMapModel.Current.IdManager;
                if (idMan==null)
                    return false;

                if (!idMan.DeleteId((m_FeatureId as NativeId), m_Entity))
                    return false;
            }

            // Delete the ID ... this can only be done for foreign IDs.
            // For official CED-style IDs, you can NEVER delete a feature
            // ID, because it may have been in use before the current
            // feature made use of it. So if you rollback the operation
            // that re-used the ID, and then rolled back the operation
            // that had originally used it, it would be pointing to an
            // area of deleted memory!

            //if (isForeign) delete m_pId;

            m_FeatureId = null;
            return true;
        }

        /// <summary>
        /// Restores the ID for the feature attached to this handle. This function is
        /// called when a user-perceived deletion is being rolled back.
        ///
        /// You should not call this function for features that have foreign IDs.
        /// </summary>
        /// <returns>True if the ID has been successfully restored. False if the ID
        /// involved is a foreign ID.</returns>
        internal bool RestoreId()
        {
	        // Confirm that the ID refers to an existing feature.
	        if (m_Feature==null)
            {
		        MessageBox.Show("IdHandle.RestoreId - Attempt to restore non-persistent ID");
		        return false;
	        }

	        // Confirm that the feature's ID has not changed somehow.
	        if (!Object.ReferenceEquals(m_FeatureId, m_Feature.Id))
            {
		        MessageBox.Show("IdHandle.RestoreId - Inconsistent ID");
		        return false;
	        }

	        // Ensure that the ID is cross-referenced to the feature.
	        m_FeatureId.AddReference(m_Feature);

	        // That's it. When a feature is de-activated, only the back
	        // pointer from the ID is nulled out. The feature retained
	        // it's pointer to the ID, and the ID itself was left in
	        // place as part of the IdRange object ... oops, the
	        // range needs to be told to decrement the number of free
	        // IDs. It should already be known via the constructor.

	        // Foreign IDs don't count.
	        if (m_Feature.IsForeignId)
                return true;

	        if (m_Packet==null)
                throw new Exception("IdHandle.RestoreId - ID range not found");

	        // Tell the packet that the ID has been restored (this just
	        // decrements the free count).
	        return m_Packet.RestoreId(m_FeatureId);
        }
    }
}
