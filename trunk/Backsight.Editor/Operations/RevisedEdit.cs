// <remarks>
// Copyright 2012 - Steve Stanton. This file is part of Backsight
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

namespace Backsight.Editor.Operations
{
    /// <summary>
    /// The changes to a specific edit (included as part of the <see cref="UpdateOperation"/> class).
    /// </summary>
    class RevisedEdit : IPersistent
    {
        #region Class data

        /// <summary>
        /// The edit being updated (not null).
        /// </summary>
        readonly Operation m_Edit;

        /// <summary>
        /// Information about the update (not null). 
        /// </summary>
        UpdateItemCollection m_Changes;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RevisedEdit"/> class.
        /// </summary>
        /// <param name="revisedEdit">The edit being updated (not null). Must implement
        /// <see cref="IRevisable"/>.</param>
        /// <exception cref="ArgumentNullException">If either <paramref name="edit"/> or
        /// <paramref name="changes"/> is null.</exception>
        /// <exception cref="ArgumentException">If <paramref name="revisedEdit"/> does not
        /// implement <see cref="IRevisable"/>.</exception>
        internal RevisedEdit(Operation revisedEdit, UpdateItemCollection changes)
            : base()
        {
            if (revisedEdit == null || changes == null)
                throw new ArgumentNullException();

            if (!(revisedEdit is IRevisable))
                throw new ArgumentException();

            m_Changes = changes;
            m_Edit = revisedEdit;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RevisedEdit"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal RevisedEdit(EditDeserializer editDeserializer)
        {
            InternalIdValue id = editDeserializer.ReadInternalId(DataField.RevisedEdit);
            m_Edit = editDeserializer.MapModel.FindOperation(id);
            m_Changes = (m_Edit as IRevisable).ReadUpdateItems(editDeserializer);
        }

        #endregion

        /// <summary>
        /// Exchanges changes with the revised edit.
        /// </summary>
        internal void ApplyChanges()
        {
            ((IRevisable)m_Edit).ExchangeData(m_Changes);
        }

        /// <summary>
        /// The edit being updated (not null). Must implement <see cref="IRevisable"/>.
        /// </summary>
        internal Operation RevisedOperation
        {
            get { return m_Edit; }
        }

        /// <summary>
        /// Information about the update (not null). 
        /// </summary>
        internal UpdateItemCollection Changes
        {
            get { return m_Changes; }
            set { m_Changes = value; }
        }

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public void WriteData(EditSerializer editSerializer)
        {
            editSerializer.WriteInternalId(DataField.RevisedEdit, m_Edit.InternalId);
            (m_Edit as IRevisable).WriteUpdateItems(editSerializer, m_Changes);
        }
    }
}
