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
        /// The tag that identifies the item (may be repeated across different types of editing operations).
        /// </summary>
        DataField m_Field;

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
            m_Field = DataField.Empty;
            m_Value = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateItem&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="field">The tag that identifies the item.</param>
        /// <param name="value">The value associated with the item.</param>
        internal UpdateItem(DataField field, object value)
        {
            m_Field = field;
            m_Value = value;
        }

        #endregion

        /// <summary>
        /// The tag that identifies the item (may be repeated across different types of editing operations).
        /// </summary>
        public DataField Field
        {
            get { return m_Field; }
            set { m_Field = value; }
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

        /// <summary>
        /// Obtains the features that are referenced by this item (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        /// <returns>The referenced features (never null, but may be an empty array).</returns>
        internal Feature[] GetReferences()
        {
            if (m_Value is Feature)
                return new Feature[] { (Feature)m_Value };

            Observation o = (m_Value as Observation);
            if (o == null)
                return new Feature[0];
            else
                return o.GetReferences();
        }
    }
}
