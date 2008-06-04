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
    /// 1. A session ID is a number that identifies a specific editing session.
    /// <para/>
    /// 2. An item number provides the item creation sequence within the session.
    /// The session itself is regarded as item number 0.
    /// <para/>
    /// An internal ID should not be confused with the <see cref="FeatureId"/> class. An
    /// internal ID will generally be hidden from users, whereas <c>FeatureId</c> is a user-
    /// specified ID.
    /// </summary>
    struct InternalIdValue
    {
        #region Class data

        /// <summary>
        /// The 1-based sequence number of the session.
        /// </summary>
        uint m_Session;

        /// <summary>
        /// The sequence number of the item (0 if the ID refers to the session).
        /// For each distinct session, the values start at 1.
        /// </summary>
        uint m_Item;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>InternalIdValue</c> for an editing session.
        /// </summary>
        /// <param name="sessionId">An ID for the editing session</param>
        /// <param name="itemSequence">The order in which an associated session was instantiated.
        /// Must be greater than zero.
        /// </param>
        internal InternalIdValue(uint sessionId, uint itemSequence)
        {
            if (sessionId==0)
                throw new ArgumentOutOfRangeException("Session ID must be greater than zero");

            m_Session = sessionId;
            m_Item = itemSequence;
        }

        /// <summary>
        /// Creates a new <c>InternalIdValue</c> by parsing the supplied string
        /// </summary>
        /// <param name="s">The string that represents an ID (in the same format produced
        /// by a call to <see cref="InternalIdValue.ToString"/>)</param>
        internal InternalIdValue(string s)
        {
            int dotPos = s.IndexOf('.');
            if (dotPos<=0)
                throw new ArgumentException();

            m_Session = UInt32.Parse(s.Substring(0, dotPos));
            m_Item = UInt32.Parse(s.Substring(dotPos+1));
        }

        #endregion

        /// <summary>
        /// The sequence number of the session
        /// </summary>
        internal uint SessionId
        {
            get { return m_Session; }
            set { m_Session = value; }
        }

        /// <summary>
        /// The creation sequence number of the item (0 if the ID refers to the session).
        /// </summary>
        internal uint ItemSequence
        {
            get { return m_Item; }
            set { m_Item = value; }
        }

        /// <summary>
        /// Is this an "empty" ID (with session ID and item sequence both zero).
        /// </summary>
        internal bool IsEmpty
        {
            get { return (m_Session==0 && m_Item==0); }
        }

        /// <summary>
        /// Override returns a concatenation of the job registration ID & the
        /// creation sequence value.
        /// </summary>
        /// <returns>A string taking the form #.#, where the number prior to the period is the
        /// session ID, and the number after the period is the item sequence.</returns>
        public override string ToString()
        {
            return String.Format("{0}.{1}", m_Session, m_Item);
        }

        /// <summary>
        /// Hash code (for use with Dictionary) is the session ID shifted over 16 bits,
        /// OR'd with the 16 low-order bits of the item number. This will produce unique
        /// values so long as the item number and session ID are both less than 65536.
        /// </summary>
        /// <returns>The value to use for indexing IDs</returns>
        public override int GetHashCode()
        {
            return ((int)(m_Session<<16) | (int)(m_Item&0x0000FFFF));
        }
    }
}
