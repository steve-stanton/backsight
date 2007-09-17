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

using Backsight.Environment;

namespace Backsight.Editor
{
	/// <written by="Steve Stanton" on="16-DEC-1998" />
    /// <summary>
    /// An ID entity, corresponding to a row that was selected from the IdEntity table.
    /// </summary>
    class IdEntity
    {
        #region Class data

        // The first 2 data members correspond to a single row in the
        // IdEntities table. Defined via a call to the Create() function ...

        /// <summary>
        /// The name of the associated ID group.
        /// </summary>
        private string m_EntityGroup;

        /// <summary>
        /// The name of the entity type that is part of the group.
        /// </summary>
        private string m_EntityType;

        // After the IdEntity table has been loaded, it gets associated
        // with an object that represents a row from the IdGroup table ...

        private IdGroup m_Group;

        // Whenever a new map is opened for editing, each IdEntity
        // object is cross-referenced to the corresponding Entity
        // object in the map (the address may vary, depending on the
        // map involved) ...

        private IEntity m_Entity;

        // The IdManager class is responsible for returning feature
        // IDs, given an entity type. To provide for efficient access,
        // IdManager maintains an array of IdEntity objects, and
        // sorts them based on the number of ID requests that are made
        // for a given entity type (frequently accessed objects are
        // sorted to the top of the list). The following data member
        // records the number of hits. It gets incremented throughout
        // an editing session.

        private uint m_NumHit;

        #endregion

        #region Constructors

        IdEntity()
        {
            m_EntityGroup = String.Empty;
            m_EntityType = String.Empty;
            m_Group = null;
            m_Entity = null;
            m_NumHit = 0;
        }

        internal IdEntity(IdGroup group, IEntity entity)
        {
            if (group==null || entity==null)
                throw new ArgumentNullException();

            m_EntityGroup = group.Name;
            m_EntityType = entity.Name;
            m_Group = group;
            m_Entity = entity;
            m_NumHit = 0;
        }

        #endregion

        /*
        //	@mfunc	Define this object using the info coming from the
        //			IdEntities table. This function should be called
        //			to initialize the info, shortly after a call to
        //			the default constructor.
        void CeIdEntity::Create ( const CtIdEntities& row ) {

	        // Pick up the name of the group and the entity type.
	        m_EntityGroup = row.m_EntityGroup;
	        m_EntityType = row.m_EntityType;

	        // And get rid of any trailing white space (in case it
	        // matters when doing subsequent comparisons).
	        m_EntityGroup.TrimRight();
	        m_EntityType.TrimRight();

	        // All the rest should have initial values.
	        m_pGroup = 0;
	        m_pEntity = 0;
	        m_NumHit = 0;

        } // end of Create
         */

        /// <summary>
        /// Reserves the next available ID.
        /// </summary>
        /// <param name="idh">The ID handle to fill in. Any ID previously reserved
        /// should have been freed (IdManager.ReserveId does this).</param>
        /// <param name="id">The specific ID to reserve (specify 0 to get the next
        /// available ID).</param>
        /// <returns>True if the ID handle was filled in.</returns>
        internal bool ReserveId(IdHandle idh, uint id)
        {
	        // Increment the number of hits.
	        m_NumHit++;

            // This ID entity cannot return an ID if it does not belong to any ID group.
	        if (m_Group==null)
                return false;

            // Get the group to reserve stuff.
	        return m_Group.ReserveId(idh, id);
        }

        internal IdGroup Group
        {
            get { return m_Group; }
            private set { m_Group = value; }
        }

        internal IEntity Entity
        {
            get { return m_Entity; }
            set { m_Entity = value; }
        }

        internal uint NumHit
        {
            get { return m_NumHit; }
        }
    }
}
