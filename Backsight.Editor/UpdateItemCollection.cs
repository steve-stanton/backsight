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
    class UpdateItemCollection
    {
        #region Class data

        /// <summary>
        /// The change items
        /// </summary>
        readonly Dictionary<string, UpdateItem> m_Changes;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateItemCollection"/> class
        /// that contains no changes.
        /// </summary>
        internal UpdateItemCollection()
        {
            m_Changes = new Dictionary<string, UpdateItem>();
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="copy">The collection to copy</param>
        internal UpdateItemCollection(UpdateItemCollection copy)
        {
            m_Changes = new Dictionary<string, UpdateItem>(copy.m_Changes.Count);

            foreach (UpdateItem item in copy.m_Changes.Values)
                m_Changes.Add(item.Name, new UpdateItem(item.Name, item.Value));
        }

        #endregion

        /// <summary>
        /// Remembers an additional change as part of this collection.
        /// </summary>
        /// <param name="item">The item to add (not null)</param>
        internal void Add(UpdateItem item)
        {
            m_Changes.Add(item.Name, item);
        }

        /// <summary>
        /// Records an update item that refers to a spatial feature.
        /// </summary>
        /// <typeparam name="T">The spatial feature class</typeparam>
        /// <param name="name">The name for the change item</param>
        /// <param name="oldValue">Tne current value (may be null)</param>
        /// <param name="newValue">The modified value (may be null)</param>
        /// <returns>True if item added. False if there's no change.</returns>
        internal bool AddFeature<T>(string name, T oldValue, T newValue) where T : Feature
        {
            if (oldValue != null || newValue != null)
            {
                if (oldValue == null || newValue == null || oldValue.DataId.Equals(newValue.DataId) == false)
                {
                    Add(new UpdateItem(name, newValue));
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Unconditionally records an update item that simply refers to a specific spatial feature.
        /// </summary>
        /// <typeparam name="T">The spatial feature class</typeparam>
        /// <param name="name">The name for the change item</param>
        /// <param name="oldValue">Tne current value (may be null)</param>
        /// <returns>True (always)</returns>
        internal bool AddFeature<T>(string name, T value) where T : Feature
        {
            Add(new UpdateItem(name, value));
            return true;
        }

        /// <summary>
        /// Writes a feature reference to a storage medium if the item is present as part of this collection.
        /// </summary>
        /// <typeparam name="T">The type of feature being referenced (as it is known to the edit
        /// that contains it)</typeparam>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        /// <param name="name">A name tag for the item</param>
        /// <returns>True if a feature reference was written, false if the item with the specific name is not present
        /// in this collection.</returns>
        internal bool WriteFeature<T>(EditSerializer editSerializer, string name) where T : Feature
        {
            UpdateItem item;
            if (m_Changes.TryGetValue(name, out item))
            {
                editSerializer.WriteFeatureRef<T>(name, (T)item.Value);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Loads a feature reference back from a storage medium (so long as it comes next in the supplied
        /// deserialization stream).
        /// </summary>
        /// <typeparam name="T">The type of feature expected by the caller.</typeparam>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        /// <param name="name">A name tag associated with the object</param>
        /// <returns>True if a feature reference was loaded, false if the next item in the deserialization
        /// stream does not have the specified name.</returns>
        internal bool ReadFeature<T>(EditDeserializer editDeserializer, string name) where T : Feature
        {
            if (editDeserializer.IsNextName(name))
            {
                T result = editDeserializer.ReadFeatureRef<T>(name);
                Add(new UpdateItem(name, result));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Records an update item that refers to some sort of observation.
        /// </summary>
        /// <typeparam name="T">The observation class</typeparam>
        /// <param name="name">The name for the change item</param>
        /// <param name="oldValue">Tne current value (may be null)</param>
        /// <param name="newValue">The modified value (may be null)</param>
        /// <returns>True if item added. False if there's no change.</returns>
        internal bool AddObservation<T>(string name, T oldValue, T newValue) where T : Observation
        {
            if (oldValue != null || newValue != null)
            {
                if (oldValue == null || newValue == null || IsEqual(oldValue, newValue) == false)
                {
                    Add(new UpdateItem(name, newValue));
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Writes an observation to a storage medium if the item is present as part of this collection.
        /// </summary>
        /// <typeparam name="T">The type of object being written (as it is known to the edit
        /// that contains it)</typeparam>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        /// <param name="name">A name tag for the item</param>
        /// <returns>True if an observation was written, false if the item with the specific name is not present
        /// in this collection.</returns>
        internal bool WriteObservation<T>(EditSerializer editSerializer, string name) where T : Observation
        {
            UpdateItem item;
            if (m_Changes.TryGetValue(name, out item))
            {
                editSerializer.WritePersistent<T>(name, (T)item.Value);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Loads an observation back from a storage medium (so long as it comes next in the supplied
        /// deserialization stream).
        /// </summary>
        /// <typeparam name="T">The type of object expected by the caller.</typeparam>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        /// <param name="name">A name tag associated with the object</param>
        /// <returns>True if an observation was loaded, false if the next item in the deserialization
        /// stream does not have the specified name.</returns>
        internal bool ReadObservation<T>(EditDeserializer editDeserializer, string name) where T : Observation
        {
            if (editDeserializer.IsNextName(name))
            {
                T result = editDeserializer.ReadPersistent<T>(name);
                Add(new UpdateItem(name, result));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks whether two observations are the same. By rights, this should be
        /// included as part of <c>Observation</c> classes. It's here only because
        /// the method for making the comparison is a bit dumb, and I don't want to
        /// make use of it more generally.
        /// </summary>
        /// <param name="a">The first observation</param>
        /// <param name="b">The observation to compare with</param>
        /// <returns></returns>
        bool IsEqual(Observation a, Observation b)
        {
            if (a == null || b == null)
            {
                return (a == null && b == null);
            }
            else
            {
                if (Object.ReferenceEquals(a, b))
                    return true;

                // The following is kind of heavy-handed
                string sa = EditSerializer.GetSerializedString<Observation>("Test", a);
                string sb = EditSerializer.GetSerializedString<Observation>("Test", b);
                return sa.Equals(sb);
            }
        }

        /// <summary>
        /// Records an update item that refers to a miscellaneous value.
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="name">The name for the change item</param>
        /// <param name="oldValue">Tne current value (may be null)</param>
        /// <param name="newValue">The modified value (may be null)</param>
        /// <returns>True if item added. False if there's no change.</returns>
        internal bool AddItem<T>(string name, T oldValue, T newValue) where T : IEquatable<T>
        {
            if (oldValue != null || newValue != null)
            {
                if (oldValue == null || newValue == null || oldValue.Equals(newValue) == false)
                {
                    Add(new UpdateItem(name, newValue));
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Writes a miscellaneous value to a storage medium if the item is present as part of this collection.
        /// </summary>
        /// <typeparam name="T">The type of value being written (as it is known to the edit
        /// that contains it)</typeparam>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        /// <param name="name">A name tag for the item</param>
        /// <returns>True if an item was written, false if the item with the specific name is not present
        /// in this collection.</returns>
        internal bool WriteItem<T>(EditSerializer editSerializer, string name) where T : IConvertible
        {
            UpdateItem item;
            if (m_Changes.TryGetValue(name, out item))
            {
                editSerializer.WriteValue<T>(name, (T)item.Value);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Loads a miscellaneous value back from a storage medium (so long as it comes next in the supplied
        /// deserialization stream).
        /// </summary>
        /// <typeparam name="T">The type of value expected by the caller.</typeparam>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        /// <param name="name">A name tag associated with the value</param>
        /// <returns>True if an item was loaded, false if the next item in the deserialization
        /// stream does not have the specified name.</returns>
        internal bool ReadItem<T>(EditDeserializer editDeserializer, string name) where T : IConvertible
        {
            if (editDeserializer.IsNextName(name))
            {
                T result = editDeserializer.ReadValue<T>(name);
                Add(new UpdateItem(name, result));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Replaces the spatial feature referenced by a specific change item.
        /// </summary>
        /// <typeparam name="T">The spatial feature class</typeparam>
        /// <param name="edit">The edit these updates relate to</param>
        /// <param name="name">The name of the change item</param>
        /// <param name="value">The value to save as part of this collection</param>
        /// <returns>The value that was previously recorded in this collection. If
        /// the named item isn't recorded as part of this collection, you get
        /// back the supplied value.</returns>
        internal T ExchangeFeature<T>(Operation edit, string name, T value) where T : Feature
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
                value.CutOp(edit);

            if (result != null)
                result.AddOp(edit);

            // Replace the value we had with the supplied value, and return
            // the value we had.
            item.Value = value;
            return result;
        }

        /// <summary>
        /// Replaces the observation referenced by a specific change item.
        /// </summary>
        /// <typeparam name="T">The observation class</typeparam>
        /// <param name="edit">The edit these updates relate to</param>
        /// <param name="name">The name of the change item</param>
        /// <param name="value">The value to save as part of this collection</param>
        /// <returns>The value that was previously recorded in this collection. If
        /// the named item isn't recorded as part of this collection, you get
        /// back the supplied value.</returns>
        internal T ExchangeObservation<T>(Operation edit, string name, T value) where T : Observation
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
                value.OnRollback(edit);

            if (result != null)
                result.AddReferences(edit);

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
        internal T ExchangeValue<T>(string name, T value) where T : IEquatable<T>
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
        /// Obtains the value of the item with a specific name.
        /// </summary>
        /// <typeparam name="T">The object class</typeparam>
        /// <param name="name">The name of the item</param>
        /// <returns>The value associated with the item (null if not found)</returns>
        internal T GetValue<T>(string name)
        {
            UpdateItem item;
            if (m_Changes.TryGetValue(name, out item))
                return (T)item.Value;
            else
                return default(T);
        }

        /// <summary>
        /// Exchanges the currently stored change values with the revised edit.
        /// </summary>
        /// <param name="edit">The edit the changes relate to (not null).</param>
        internal void ExchangeData(IRevisable edit)
        {
            edit.ExchangeData(this);
        }

        /// <summary>
        /// Obtains the features that are referenced by the items in this collection (including
        /// features that are indirectly referenced by observation classes).
        /// </summary>
        /// <returns>The referenced features (never null, but may be an empty array).</returns>
        internal Feature[] GetReferences()
        {
            List<Feature> result = new List<Feature>();

            foreach (UpdateItem item in m_Changes.Values)
                result.AddRange(item.GetReferences());

            return result.ToArray();
        }

        /// <summary>
        /// Creates an array that contains the items in this collection.
        /// </summary>
        /// <returns>The items in this collection.</returns>
        internal UpdateItem[] ToArray()
        {
            UpdateItem[] result = new UpdateItem[m_Changes.Count];
            m_Changes.Values.CopyTo(result, 0);
            return result;
        }

        /// <summary>
        /// The number of items in this collection.
        /// </summary>
        internal int Count
        {
            get { return m_Changes.Count; }
        }

        /*
        public void WriteData(EditSerializer editSerializer)
        {
            IEditWriter writer = editSerializer.Writer;
            writer.WriteInt32("Length", m_Changes.Count);

            foreach (KeyValuePair<string, object> kvp in m_Changes)
            {
                object o = kvp.Value;

                if (o is Feature)
                {
                    editSerializer.WriteFeatureRef<Feature>(kvp.Key, (Feature)o);
                }
                else if (o is Observation)
                {
                    editSerializer.WritePersistent<Observation>(kvp.Key, (Observation)o);
                }
                else if (o is IConvertible)
                {
                    // Just write it as a string
                    TypeCode typeCode = Type.GetTypeCode(o.GetType());

                    switch (typeCode)
                    {
                    }
                }
            }
        }
             */
    }
}
