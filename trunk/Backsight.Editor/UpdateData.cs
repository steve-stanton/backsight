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
using System.Collections.Generic;
using Backsight.Editor.Operations;

namespace Backsight.Editor
{
    /// <summary>
    /// A collection of <see cref="UpdateItem"/>, indexed by the item name.
    /// </summary>
    class UpdateData
    {
        #region Class data

        /// <summary>
        /// The edit the changes relate to (not null).
        /// </summary>
        readonly IRevisable m_Edit;

        /// <summary>
        /// The change items
        /// </summary>
        readonly Dictionary<string, UpdateItem> m_Changes;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateData"/> class
        /// that contains no changes.
        /// </summary>
        /// <param name="edit">The edit the changes relate to (not null).</param>
        internal UpdateData(IRevisable edit)
        {
            if (edit == null)
                throw new ArgumentNullException();

            if (!(edit is Operation))
                throw new ArgumentException("IRevisable is not an edit");

            m_Edit = edit;
            m_Changes = new Dictionary<string, UpdateItem>();
        }

        #endregion

        /// <summary>
        /// The edit the changes relate to (not null).
        /// </summary>
        internal Operation RevisedEdit
        {
            get { return (Operation)m_Edit; }
        }

        /// <summary>
        /// Remembers an additional change as part of this collection.
        /// </summary>
        /// <param name="item">The item to add (not null)</param>
        void Add(UpdateItem item)
        {
            m_Changes.Add(item.Name, item);
        }

        /// <summary>
        /// Records an update item that refers to a spatial feature.
        /// </summary>
        /// <typeparam name="T">The spatial feature class</typeparam>
        /// <param name="name">The name for the change item</param>
        /// <param name="value">Tne value to record</param>
        internal void AddFeature<T>(string name, T value) where T : Feature
        {
            Add(new UpdateItem(name, value));
        }

        /// <summary>
        /// Records an update item that refers to some sort of observation.
        /// </summary>
        /// <typeparam name="T">The observation class</typeparam>
        /// <param name="name">The name for the change item</param>
        /// <param name="value">Tne value to record</param>
        internal void AddObservation<T>(string name, T value) where T : Observation
        {
            Add(new UpdateItem(name, value));
        }

        /// <summary>
        /// Records an update item that refers to a miscellaneous value.
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="name">The name for the change item</param>
        /// <param name="value">Tne value to record</param>
        internal void AddItem<T>(string name, T value) where T : IComparable
        {
            Add(new UpdateItem(name, value));
        }

        /// <summary>
        /// Replaces the spatial feature referenced by a specific change item.
        /// </summary>
        /// <typeparam name="T">The spatial feature class</typeparam>
        /// <param name="name">The name of the change item</param>
        /// <param name="value">The value to save as part of this collection</param>
        /// <returns>The value that was previously recorded in this collection. If
        /// the named item isn't recorded as part of this collection, you get
        /// back the supplied value.</returns>
        internal T ExchangeFeature<T>(string name, T value) where T : Feature
        {
            // If the specified item isn't in the change list, just return
            // the value that was supplied.
            UpdateItem item;
            if (!m_Changes.TryGetValue(name, out item))
                return value;

            // Do nothing if the before and after values are the same
            T result = (T)item.Value;
            if (Object.ReferenceEquals(result, value))
                return value;

            // Cut reference that the old feature has to the edit, and ensure
            // the new feature is referenced to the edit.
            if (value != null)
                value.CutOp(this.RevisedEdit);

            if (result != null)
                result.AddOp(this.RevisedEdit);

            // Replace the value we had with the supplied value, and return
            // the value we had.
            item.Value = value;
            return result;
        }

        /// <summary>
        /// Replaces the observation referenced by a specific change item.
        /// </summary>
        /// <typeparam name="T">The observation class</typeparam>
        /// <param name="name">The name of the change item</param>
        /// <param name="value">The value to save as part of this collection</param>
        /// <returns>The value that was previously recorded in this collection. If
        /// the named item isn't recorded as part of this collection, you get
        /// back the supplied value.</returns>
        internal T ExchangeObservation<T>(string name, T value) where T : Observation
        {
            // If the specified item isn't in the change list, just return
            // the value that was supplied.
            UpdateItem item;
            if (!m_Changes.TryGetValue(name, out item))
                return value;

            // Do nothing if the before and after values are the same
            T result = (T)item.Value;
            if (Object.ReferenceEquals(result, value))
                return value;

            // Cut any references made by the supplied observation.
            if (value != null)
                value.OnRollback(this.RevisedEdit);

            if (result != null)
                result.AddReferences(this.RevisedEdit);

            // Replace the value we had with the supplied value, and return
            // the value we had.
            item.Value = value;
            return result;
        }

        /// <summary>
        /// Replaces a miscellaneous object referenced by a specific change item.
        /// </summary>
        /// <typeparam name="T">The object class</typeparam>
        /// <param name="name">The name of the change item</param>
        /// <param name="value">The value to save as part of this collection</param>
        /// <returns>The value that was previously recorded in this collection. If
        /// the named item isn't recorded as part of this collection, you get
        /// back the supplied value.</returns>
        internal T ExchangeValue<T>(string name, T value) where T : IComparable
        {
            // If the specified item isn't in the change list, just return
            // the value that was supplied.
            UpdateItem item;
            if (!m_Changes.TryGetValue(name, out item))
                return value;

            // Replace the value we had with the supplied value, and return
            // the value we had.
            T result = (T)item.Value;
            item.Value = value;
            return result;
        }

        /// <summary>
        /// Exchanges the currently stored change values with the revised edit.
        /// </summary>
        internal void ExchangeData()
        {
            m_Edit.ExchangeData(this);
        }
    }
}
