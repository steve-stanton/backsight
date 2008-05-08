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
    /// consists of four parts:
    /// <para/>
    /// 1. A job ID is some number that is allocated to the map model containing
    /// the object associated with an <c>InternalIdValue</c>. If you use Backsight to do some
    /// ad-hoc job (and unrelated to any other map), this ID will be zero.
    /// <para/>
    /// 2. A sequence number for the session in which something was created.
    /// Zero if the internal ID refers to the overall job.
    /// <para/>
    /// 3. A sequence number for the edit that created something.
    /// Zero if the internal ID refers to the session.
    /// <para/>
    /// 4. A sequence number for a specific feature created by an edit.
    /// Zero if the internal ID refers to the editing operation itself.
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
        /// The ID of the job for the enclosing map. Zero means the job
        /// hasn't been registered.
        /// </summary>
        uint m_JobId;

        /// <summary>
        /// The sequence number of the session (0 if the ID refers to the complete job).
        /// For each distinct job, the values start at 1.
        /// </summary>
        uint m_Session;

        /// <summary>
        /// The sequence number of the edit (0 if the ID refers to the session).
        /// For each distinct session, the values start at 1.
        /// </summary>
        uint m_Edit;

        /// <summary>
        /// The sequence number of a feature created within an edit (0 if the ID refers to
        /// the complete edit). For each distinct edit, the values start at 1.
        /// </summary>
        uint m_Feature;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>InternalIdValue</c> with the specified creation sequence, and
        /// without any job registration ID.
        /// </summary>
        /// <param name="creationSequence">The order in which an associated object was instantiated.
        /// Must be greater than zero.
        /// </param>
        //internal InternalIdValue(uint creationSequence)
        //    : this(0, creationSequence)
        //{
        //}

        /// <summary>
        /// Creates a new <c>InternalIdValue</c> for an editing session.
        /// </summary>
        /// <param name="jobId">An ID that relates an associated map with some
        /// sort of data processing job (zero if the map hasn't been registered)</param>
        /// <param name="sessionSequence">The order in which an associated session was instantiated.
        /// Must be greater than zero.
        /// </param>
        internal InternalIdValue(uint jobId, uint sessionSequence)
        {
            if (sessionSequence==0)
                throw new ArgumentOutOfRangeException("Session sequence must be greater than zero");

            m_JobId = jobId;
            m_Session = sessionSequence;
            m_Edit = 0;
            m_Feature = 0;
        }

        #endregion

        /// <summary>
        /// The ID that relates the associated map with some sort of data processing job. 
        /// </summary>
        internal uint JobId
        {
            get { return m_JobId; }
            set { m_JobId = value; }
        }

        /// <summary>
        /// The sequence number of the session (0 if the ID refers to the complete job).
        /// For each distinct job, the values start at 1.
        /// </summary>
        internal uint SessionSequence
        {
            get { return m_Session; }
            set { m_Session = value; }
        }

        /// <summary>
        /// The sequence number of the edit (0 if the ID refers to the session).
        /// For each distinct session, the values start at 1.
        /// </summary>
        internal uint EditSequence
        {
            get { return m_Edit; }
            set { m_Edit = value; }
        }

        /// <summary>
        /// The sequence number of a feature created within an edit (0 if the ID refers to
        /// the complete edit). For each distinct edit, the values start at 1.
        /// </summary>
        internal uint FeatureSequence
        {
            get { return m_Feature; }
            set { m_Feature = value; }
        }

        /// <summary>
        /// Override returns a concatenation of the job registration ID & the
        /// creation sequence value.
        /// </summary>
        /// <returns>A string taking the form #.#, where the number prior to the period is the
        /// job registration ID, and the number after the period is the creation sequence value.</returns>
        public override string ToString()
        {
            return String.Format("{0}.{1}.{2}.{3}", m_JobId, m_Session, m_Edit, m_Feature);
        }
    }
}
