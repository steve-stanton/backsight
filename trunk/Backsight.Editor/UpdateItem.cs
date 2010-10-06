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

namespace Backsight.Editor
{
    /// <summary>
    /// An updated item for an editing operation.
    /// </summary>
    class UpdateItem
    {
        #region Class data

        /// <summary>
        /// A name for the item (may be repeated across different types of editing operations).
        /// </summary>
        string m_Name;

        /// <summary>
        /// The value associated with the item. This may either refer to the value prior
        /// to a change, or the modified value that was applied.
        /// </summary>
        object m_Value;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor (for serialization mechanism).
        /// </summary>
        public UpdateItem()
        {
            m_Name = String.Empty;
            m_Value = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateItem&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="name">A name for the item</param>
        /// <param name="value">The value associated with the item.</param>
        internal UpdateItem(string name, object value)
        {
            m_Name = name;
            m_Value = value;
        }

        #endregion

        /// <summary>
        /// A name for the item (may be repeated across different types of editing operations).
        /// </summary>
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        /// <summary>
        /// The value associated with the item. This may either refer to the value prior
        /// to a change, or the modified value that was applied.
        /// </summary>
        public object Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }
    }
}
