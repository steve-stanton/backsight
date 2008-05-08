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

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="22-AUG-2007" />
    /// <summary>
    /// An internal ID is used by Backsight as a persistent handle for objects, in situations
    /// where the data needs to be stored in something like a relational database. It is expected
    /// to be globally unique (within the world known to Backsight). To allow for this, the ID
    /// consists of two parts:
    /// <para/>
    /// 1. A job registration ID is some number that is allocated to the map model containing
    /// the object associated with an <c>InternalIdValue</c>. If you use Backsight to do some
    /// ad-hoc job (and unrelated to any other map), this ID will be zero.
    /// <para/>
    /// 2. A creation sequence number defines the order in which the associated object was
    /// instantiated. Since creation of an object does not guarantee that it will actually
    /// be saved, the collection of internal IDs for a map may well have gaps in this sequence.
    /// <para/>
    /// An internal ID should not be confused with the <see cref="FeatureId"/> class. An
    /// internal ID will generally be hidden from users, whereas <c>FeatureId</c> is a user-
    /// specified ID.
    /// </summary>
    [Serializable]
    struct InternalIdValue
    {
        #region Class data

        /// <summary>
        /// The ID of the job registration for the enclosing map. Zero means the job
        /// hasn't been registered.
        /// </summary>
        readonly uint m_JobRegistrationId;

        /// <summary>
        /// The sequence in which the associated object was instantiated. Not zero.
        /// </summary>
        readonly uint m_CreationSequence;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>InternalIdValue</c> with the specified creation sequence, and
        /// without any job registration ID.
        /// </summary>
        /// <param name="creationSequence">The order in which an associated object was instantiated.
        /// Must be greater than zero.
        /// </param>
        internal InternalIdValue(uint creationSequence)
            : this(0, creationSequence)
        {
        }

        /// <summary>
        /// Creates a new <c>InternalIdValue</c> with the specified job registration ID and
        /// creation sequence.
        /// </summary>
        /// <param name="jobRegistrationId">An ID that relates an associated map with some
        /// sort of data processing job (zero if the map hasn't been registered)</param>
        /// <param name="creationSequence">The order in which an associated object was instantiated.
        /// Must be greater than zero.
        /// </param>
        internal InternalIdValue(uint jobRegistrationId, uint creationSequence)
        {
            if (creationSequence==0)
                throw new ArgumentOutOfRangeException("Creation sequence must be greater than zero");

            m_JobRegistrationId = jobRegistrationId;
            m_CreationSequence = creationSequence;
        }

        #endregion

        /// <summary>
        /// Is the job registration defined (with any value greater than zero).
        /// </summary>
        internal bool IsRegistered
        {
            get { return (m_JobRegistrationId>0); }
        }

        /// <summary>
        /// The ID that relates the associated map with some sort of data processing job. 
        /// </summary>
        internal uint JobRegistrationId
        {
            get { return m_JobRegistrationId; }
        }

        /// <summary>
        /// The sequence in which the associated object was instantiated. Not zero.
        /// </summary>
        internal uint CreationSequence
        {
            get { return m_CreationSequence; }
        }

        /// <summary>
        /// Override returns a concatenation of the job registration ID & the
        /// creation sequence value.
        /// </summary>
        /// <returns>A string taking the form #.#, where the number prior to the period is the
        /// job registration ID, and the number after the period is the creation sequence value.</returns>
        public override string ToString()
        {
            return String.Format("{0}.{1}", m_JobRegistrationId, m_CreationSequence);
        }
    }
}
