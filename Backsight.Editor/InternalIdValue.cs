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

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="22-AUG-2007" />
    /// <summary>
    /// An internal ID is used by Backsight as a persistent handle for objects. For each project, ID
    /// values start with number 1.
    /// <para/>
    /// An internal ID should not be confused with the <see cref="FeatureId"/> class. An
    /// internal ID will generally be hidden from users, whereas <c>FeatureId</c> is a user-
    /// specified ID.
    /// </summary>
    struct InternalIdValue : IEquatable<InternalIdValue>, IComparable<InternalIdValue>
    {
        /// <summary>
        /// An undefined internal ID.
        /// </summary>
        public static readonly InternalIdValue Empty = new InternalIdValue(0);

        #region data

        /// <summary>
        /// The sequence number of the item (0 if the ID is undefined).
        /// For each editing project, the values start at 1.
        /// </summary>
        uint m_Item;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>InternalIdValue</c>.
        /// </summary>
        /// <param name="itemSequence">A sequence number for an item within an editing project</param>
        internal InternalIdValue(uint itemSequence)
        {
            m_Item = itemSequence;
        }

        /// <summary>
        /// Creates a new <c>InternalIdValue</c> by parsing the supplied string
        /// </summary>
        /// <param name="s">The string that represents an ID (in the same format produced
        /// by a call to <see cref="InternalIdValue.ToString"/>)</param>
        internal InternalIdValue(string s)
        {
            m_Item = uint.Parse(s);
        }

        #endregion

        /// <summary>
        /// The creation sequence number of the item (increasing through the lifetime of a project).
        /// </summary>
        internal uint ItemSequence
        {
            get { return m_Item; }
            set { m_Item = value; }
        }

        /// <summary>
        /// Is this an "empty" ID (with item sequence of zero).
        /// </summary>
        internal bool IsEmpty
        {
            get { return (m_Item==0); }
        }

        /// <summary>
        /// Override returns the creation sequence value.
        /// </summary>
        /// <returns>A string version of the item sequence number.</returns>
        public override string ToString()
        {
            return m_Item.ToString();
        }

        /// <summary>
        /// Hash code (for use with Dictionary) is the value of the internal ID, cast to an int.
        /// </summary>
        /// <returns>The value to use for indexing IDs</returns>
        public override int GetHashCode()
        {
            return (int)m_Item;
        }

        #region IEquatable<InternalIdValue> Members

        public bool Equals(InternalIdValue that)
        {
            return (this.m_Item == that.m_Item);
        }

        #endregion

        #region IComparable<InternalIdValue> Members

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="that">An object to compare with this object.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared.
        /// The return value has the following meanings: Value Meaning Less than zero This object
        /// is less than the other parameter.Zero This object is equal to other.
        /// Greater than zero This object is greater than other.
        /// </returns>
        public int CompareTo(InternalIdValue that)
        {
            return this.m_Item.CompareTo(that.m_Item);
        }

        #endregion

        /// <summary>
        /// Implements the &lt; operator
        /// </summary>
        /// <param name="a">The first ID</param>
        /// <param name="b">The second ID</param>
        /// <returns>True if the first ID precedes the second ID. False if the first ID is greater than
        /// or equal to the second ID.</returns>
        public static bool operator<(InternalIdValue a, InternalIdValue b)
        {
            return (a.m_Item < b.m_Item);
        }

        /// <summary>
        /// Implements the &gt; operator
        /// </summary>
        /// <param name="a">The first ID</param>
        /// <param name="b">The second ID</param>
        /// <returns>True if the first ID comes after the second ID. False if the first ID is less than
        /// or equal to the second ID.</returns>
        public static bool operator>(InternalIdValue a, InternalIdValue b)
        {
            return (a.m_Item > b.m_Item);
        }
    }
}
