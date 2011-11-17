// <remarks>
// Copyright 2010 - Steve Stanton. This file is part of Backsight
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

using Backsight.Environment;


namespace Backsight.Editor
{
    /// <summary>
    /// Basic information for a spatial feature, used during deserialization from the database.
    /// </summary>
    class FeatureStub : IFeature, IPersistent
    {
        #region Class data

        /// <summary>
        /// The editing operation that created the feature (not null).
        /// </summary>
        readonly Operation m_Creator;

        /// <summary>
        /// The 1-based creation sequence of the feature within the session that created it.
        /// </summary>
        readonly uint m_SessionSequence;

        /// <summary>
        /// The type of real-world object that the feature corresponds to.
        /// </summary>
        readonly IEntity m_What;

        /// <summary>
        /// The ID of the feature (may be shared by multiple features).
        /// </summary>
        readonly FeatureId m_Id;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureStub"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal FeatureStub(EditDeserializer editDeserializer)
        {
            m_Creator = editDeserializer.CurrentEdit;
            Debug.Assert(m_Creator != null);
            ReadData(editDeserializer, out m_SessionSequence, out m_What, out m_Id);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureStub"/> class with the
        /// next available item sequence number.
        /// </summary>
        /// <param name="creator">The editing operation that created the feature.</param>
        /// <param name="ent">The entity type for the feature (not null)</param>
        /// <param name="fid">The (optional) user-perceived ID for the feature.</param>
        /// <exception cref="ArgumentNullException">If either <paramref name="ent"/> or
        /// <paramref name="creator"/> is null.</exception>
        internal FeatureStub(Operation creator, IEntity ent, FeatureId fid)
            : this(creator, Session.ReserveNextItem(), ent, fid)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureStub"/> class
        /// </summary>
        /// <param name="creator">The editing operation that created the feature.</param>
        /// <param name="sessionSequence">The 1-based creation sequence of the feature within
        /// the session that created it.</param>
        /// <param name="ent">The entity type for the feature (not null)</param>
        /// <param name="fid">The (optional) user-perceived ID for the feature.</param>
        /// <exception cref="ArgumentNullException">If either <paramref name="ent"/> or
        /// <paramref name="creator"/> is null.</exception>
        internal FeatureStub(Operation creator, uint sessionSequence, IEntity ent, FeatureId fid)
        {
            if (creator == null || ent == null)
                throw new ArgumentNullException();

            m_Creator = creator;
            m_SessionSequence = sessionSequence;
            m_What = ent;
            m_Id = fid;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureStub"/> class
        /// that contains a copy of the properties of a feature.
        /// </summary>
        /// <param name="f">The feature containing the properties to copy (not null).</param>
        /// <exception cref="ArgumentNullException">If the supplied feature is null.</exception>
        internal FeatureStub(IFeature f)
        {
            if (f == null)
                throw new ArgumentNullException();

            m_Creator = f.Creator;
            m_SessionSequence = f.SessionSequence;
            m_What = f.EntityType;
            m_Id = f.FeatureId;
        }

        #endregion

        #region IFeature Members

        /// <summary>
        /// The editing operation that created the feature (not null).
        /// </summary>
        public Operation Creator
        {
            get { return m_Creator; }
        }

        /// <summary>
        /// The 1-based creation sequence of the feature within the session that created it.
        /// </summary>
        public uint SessionSequence
        {
            get { return m_SessionSequence; }
        }

        /// <summary>
        /// The type of real-world object that the feature corresponds to (not null).
        /// </summary>
        public IEntity EntityType
        {
            get { return m_What; }
        }

        /// <summary>
        /// The ID of the feature (may be shared by multiple features).
        /// </summary>
        public FeatureId FeatureId
        {
            get { return m_Id; }
        }

        #endregion

        /// <summary>
        /// The formatted ID of the unique identifier for the feature
        /// </summary>
        public string DataId
        {
            get
            {
                uint sessionId = m_Creator.Session.Id;
                return InternalIdValue.Format(sessionId, m_SessionSequence);
            }
        }

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public void WriteData(EditSerializer editSerializer)
        {
            editSerializer.WriteUInt32("Id", m_SessionSequence);
            editSerializer.WriteEntity("Entity", m_What);
            editSerializer.WriteFeatureId(m_Id);
        }

        /// <summary>
        /// Reads data that was previously written using <see cref="WriteData"/>
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        /// <param name="ss">The 1-based creation sequence of the feature within the session that created it.</param>
        /// <param name="entity">The type of real-world object that the feature corresponds to.</param>
        /// <param name="fid">The ID of the feature (may be null).</param>
        static void ReadData(EditDeserializer editDeserializer, out uint ss, out IEntity entity, out FeatureId fid)
        {
            ss = editDeserializer.ReadUInt32("Id");
            entity = editDeserializer.ReadEntity("Entity");
            fid = editDeserializer.ReadFeatureId();
        }
    }
}
