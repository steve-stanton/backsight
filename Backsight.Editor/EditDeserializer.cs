// <remarks>
// Copyright 2011 - Steve Stanton. This file is part of Backsight
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
using System.Diagnostics;
using System.Reflection;

using Backsight.Environment;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="06-NOV-2011" />
    /// <summary>
    /// Deserializes instances of <see cref="IPersistent"/> that have been saved using
    /// an instance of <see cref="EditSerializer"/>.
    /// </summary>
    class EditDeserializer
    {
        #region Static

        /// <summary>
        /// Loads constructor information for persistent classes.
        /// </summary>
        /// <returns>The constructors for each persistent class in this assembly, keyed by the
        /// short type name.</returns>
        static Dictionary<string, ConstructorInfo> LoadConstructors()
        {
            Dictionary<string, ConstructorInfo> result = new Dictionary<string, ConstructorInfo>();
            string iName = typeof(IPersistent).Name; // do it this way in case I rename the interface
            Assembly a = Assembly.GetExecutingAssembly();
            Type[] types = a.GetTypes();

            foreach (Type t in types)
            {
                if (t.Name.StartsWith("<") == false && t.IsAbstract == false && t.IsInterface == false && t.GetInterface(iName) != null)
                {
                    ConstructorInfo ci = t.GetConstructor(BindingFlags.Public |
                                                          BindingFlags.NonPublic |
                                                          BindingFlags.Instance |
                                                          BindingFlags.DeclaredOnly, null,
                                                          new Type[] { typeof(EditDeserializer) }, null);

                    if (ci == null)
                        throw new ApplicationException("Class " + t.Name + " implements IPersistent but does not provide deserialization constructor");

                    result.Add(t.Name, ci);
                }
            }

            return result;
        }

        #endregion

        #region Class data

        IEditReader m_Reader;

        /// <summary>
        /// Index of the constructors that accept an instance of <see cref="EditDeserializer"/> (and which
        /// belong to classes that implement <see cref="IPersistent"/>), keyed by the short type name.
        /// This is restricted to the current assembly, and excludes abstract classes, as well as miscellaneous
        /// mystery classes that seem to be produced by NET (with type names that start with the
        /// "&lt;" character).
        /// </summary>
        readonly Dictionary<string, ConstructorInfo> m_Constructors;

        /// <summary>
        /// The map model that deserialized data will be loaded into (not null).
        /// </summary>
        readonly CadastralMapModel m_MapModel;

        /// <summary>
        /// The edit (if any) that is currently being deserialized (defined as a result
        /// of calling <see cref="Operation(EditDeserializer)"/>). 
        /// </summary>
        Operation m_CurrentEdit;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EditDeserializer"/> class
        /// with an empty map model.
        /// </summary>
        /// <remarks>
        /// While this constructor is good for testing, the empty map model that gets utilized may not
        /// be complete unless the environment has been properly initialized (e.g. have ID groups been
        /// loaded from the database?).
        /// </remarks>
        internal EditDeserializer()
            : this(new CadastralMapModel())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditDeserializer"/> class by
        /// scanning the current assembly to look for constructors that accept an instance of
        /// <see cref="EditDeserializer"/>. Before attempting to deserialize anything,
        /// you must also define a reader using <see cref="SetReader"/>.
        /// </summary>
        /// <param name="mapModel">The map model to load deserialized data into (not null).</param>
        /// <exception cref="ArgumentNullException">If the supplied map model is null.</exception>
        /// <exception cref="ApplicationException">If no deserialization constructors can be found
        /// in the current assembly.</exception>
        internal EditDeserializer(CadastralMapModel mapModel)
        {
            if (mapModel == null)
                throw new ArgumentNullException();

            m_MapModel = mapModel;
            m_CurrentEdit = null;
            m_Constructors = LoadConstructors();

            if (m_Constructors.Count == 0)
                throw new ApplicationException("Cannot find any deserialization constructors");
        }

        #endregion

        /// <summary>
        /// Records the reader that should be used when deserializing data.
        /// </summary>
        /// <param name="reader">The mechanism for loading persistent data (not null).</param>
        internal void SetReader(IEditReader reader)
        {
            m_Reader = reader;
        }

        /// <summary>
        /// The map model that deserialized data is loaded into (not null).
        /// </summary>
        internal CadastralMapModel MapModel
        {
            get { return m_MapModel; }
        }

        /// <summary>
        /// The edit (if any) that is currently being deserialized (defined as a result
        /// of calling the <see cref="Operation(EditDeserializer)"/> constructor).
        /// </summary>
        internal Operation CurrentEdit
        {
            get { return m_CurrentEdit; }

            set
            {
                if (value != null && m_CurrentEdit != null)
                    throw new ApplicationException("Edits appear to be nested");

                m_CurrentEdit = value;
            }
        }

        /// <summary>
        /// Reads the content of an array containing elements that implement <see cref="IPersistent"/>.
        /// </summary>
        /// <typeparam name="T">The type of each array element expected by the caller.</typeparam>
        /// <param name="field">A tag associated with the array</param>
        /// <returns>The array that was read</returns>
        internal T[] ReadPersistentArray<T>(DataField field) where T : IPersistent
        {
            // The array itself should not have a type name unless it happens to be "null" (denoting
            // an array reference that is null).

            string typeName = m_Reader.ReadString(field.ToString());
            if (typeName != null)
            {
                if (typeName == "null")
                    return null;

                throw new ApplicationException("Unexpected type name for array element " + field);
            }

            m_Reader.ReadBeginObject();

            try
            {
                uint nItem = m_Reader.ReadUInt32("Length");
                T[] result = new T[nItem];

                for (int i = 0; i < result.Length; i++)
                {
                    string itemName = string.Format("[{0}]", i);
                    result[i] = ReadPersistent<T>(itemName);
                }

                return result;
            }

            finally
            {
                m_Reader.ReadEndObject();
            }
        }

        /// <summary>
        /// Reads an array of feature stubs.
        /// </summary>
        /// <param name="field">A tag associated with the array</param>
        /// <returns>The array that was read</returns>
        internal FeatureStub[] ReadFeatureStubArray(DataField field)
        {
            // Just read an array the usual way (ReadFeatureStubArray exists only to provide symmetry with WriteFeatureStubArray)

            return ReadPersistentArray<FeatureStub>(field);
        }

        /// <summary>
        /// Reads an array of simple types to a storage medium.
        /// </summary>
        /// <typeparam name="T">The type of each array element expected by the caller.</typeparam>
        /// <param name="field">A tag associated with the array</param>
        /// <returns>The array that was read</returns>
        internal T[] ReadSimpleArray<T>(DataField field) where T : IConvertible
        {
            // The array itself should not have a type name unless it happens to be "null" (denoting
            // an array reference that is null).

            string data = m_Reader.ReadString(field.ToString());
            if (data == "null")
                return null;

            string[] items = data.Split(';');
            T[] result = new T[items.Length];

            for (int i=0; i<items.Length; i++)
                result[i] = (T)Convert.ChangeType(items[i], typeof(T));

            return result;
        }

        /// <summary>
        /// Reads the content of an object that implements <see cref="IPersistent"/>.
        /// </summary>
        /// <typeparam name="T">The type of object expected by the caller.</typeparam>
        /// <param name="field">A tag associated with the object</param>
        /// <returns>
        /// The object that was read (may actually have a type that is derived
        /// from the supplied type).
        /// </returns>
        /// <remarks>
        /// In addition to implementing <see cref="IPersistent"/>, the Backsight implementation
        /// assumes that the created type will also provide a constructor that accepts an
        /// instance of the <see cref="IEditReader"/>.
        /// </remarks>
        /// <exception cref="ApplicationException">If the type name read from storage does
        /// not correspond to a suitable type within this assembly.</exception>
        internal T ReadPersistent<T>(DataField field) where T : IPersistent
        {
            return ReadPersistent<T>(field.ToString());
        }

        /// <summary>
        /// Reads the content of an object that implements <see cref="IPersistent"/>.
        /// </summary>
        /// <typeparam name="T">The type of object expected by the caller.</typeparam>
        /// <param name="name">A name tag associated with the object</param>
        /// <returns>
        /// The object that was read (may actually have a type that is derived
        /// from the supplied type).
        /// </returns>
        /// <remarks>
        /// In addition to implementing <see cref="IPersistent"/>, the Backsight implementation
        /// assumes that the created type will also provide a constructor that accepts an
        /// instance of the <see cref="IEditReader"/>.
        /// </remarks>
        /// <exception cref="ApplicationException">If the type name read from storage does
        /// not correspond to a suitable type within this assembly.</exception>
        T ReadPersistent<T>(string name) where T : IPersistent
        {
            string typeName = m_Reader.ReadString(name);

            // A string of "null" is used to denote a null
            if (typeName == "null")
                return default(T);

            // Getting back an *actual* null means there was nothing after the name tag, meaning
            // that the type known to the caller is what we want to create.
            if (typeName == null)
                typeName = typeof(T).Name;

            ConstructorInfo ci;
            if (!m_Constructors.TryGetValue(typeName, out ci))
                throw new ApplicationException("Cannot locate constructor for type: " + typeName);


            m_Reader.ReadBeginObject(); // Read opening bracket
            T result = default(T);

            try
            {
                result = (T)ci.Invoke(new object[] { this });
            }

            catch
            {
                throw new ApplicationException("Failed to create instance of " + typeName);
            }

            finally
            {
                m_Reader.ReadEndObject(); // Read the closing bracket

                // The current edit is defined by the Operation constructor that accepts this
                // EditDeserializer instance. Edits should never be nested, so make sure we
                // clear the current edit when necessary.
                if (result is Operation)
                {
                    Debug.Assert(m_CurrentEdit != null);
                    Debug.Assert(Object.ReferenceEquals(m_CurrentEdit, result));

                    m_CurrentEdit = null;
                }
            }

            return result;
        }

        /// <summary>
        /// Reads the content of an object that implements <see cref="IPersistent"/>, 
        /// having confirmed that the specified name tag comes next.
        /// </summary>
        /// <typeparam name="T">The type of object expected by the caller.</typeparam>
        /// <param name="field">A tag associated with the object</param>
        /// <returns>
        /// The object that was read (if the specified name tag was there), or null (if
        /// the next name tag is something else).
        /// </returns>
        internal T ReadPersistentOrNull<T>(DataField field) where T : IPersistent
        {
            if (IsNextField(field))
                return ReadPersistent<T>(field);
            else
                return default(T);
        }

        /// <summary>
        /// Reads an entity type for a spatial feature.
        /// </summary>
        /// <param name="field">A tag associated with the value</param>
        /// <returns>The entity type that was read.</returns>
        internal IEntity ReadEntity(DataField field)
        {
            int id = m_Reader.ReadInt32(field.ToString());
            return EnvironmentContainer.FindEntityById(id);
        }

        /// <summary>
        /// Reads a distance unit type.
        /// </summary>
        /// <param name="field">A tag associated with the item</param>
        /// <returns>The distance unit that was read.</returns>
        internal DistanceUnit ReadDistanceUnit(DataField field)
        {
            DistanceUnitType unitType = (DistanceUnitType)m_Reader.ReadInt32(field.ToString());
            return EditingController.GetUnits(unitType);
        }

        /// <summary>
        /// Reads a value in radians.
        /// </summary>
        /// <param name="field">A tag associated with the value</param>
        /// <returns>The radian value that was read.</returns>
        internal RadianValue ReadRadians(DataField field)
        {
            string s = m_Reader.ReadString(field.ToString());
            double d = RadianValue.Parse(s);
            return new RadianValue(d);
        }

        /// <summary>
        /// Reads a 2D position.
        /// </summary>
        /// <param name="xField">A tag for the easting value</param>
        /// <param name="yField">A tag for the northing value</param>
        /// <param name="value">The position to write</param>
        /// <returns>The position that was read</returns>
        internal PointGeometry ReadPointGeometry(DataField xField, DataField yField)
        {
            long x = m_Reader.ReadInt64(xField.ToString());
            long y = m_Reader.ReadInt64(yField.ToString());
            return new PointGeometry(x, y);
        }

        /// <summary>
        /// Reads a reference to a spatial feature, using that reference to obtain the
        /// corresponding feature.
        /// </summary>
        /// <typeparam name="T">The type of spatial feature expected by the caller</typeparam>
        /// <param name="field">A tag associated with the value</param>
        /// <returns>
        /// The feature that was read (null if not found, or the reference was undefined). May
        /// actually have a type that is derived from the supplied type.
        /// </returns>
        /// <remarks>This does not create a brand new feature. Rather, it uses a reference
        /// to try to obtain a feature that should have already been created.
        /// </remarks>
        internal T ReadFeatureRef<T>(DataField field) where T : Feature
        {
            string dataId = m_Reader.ReadString(field.ToString());
            if (dataId == null)
                return default(T);

            return m_MapModel.Find<T>(dataId);
        }

        /// <summary>
        /// Reads an array of spatial features (using their unique IDs the read them from the map model).
        /// </summary>
        /// <typeparam name="T">The type of spatial feature expected by the caller</typeparam>
        /// <param name="field">A tag associated with the array</param>
        /// <returns>The features that were read (should all be not null).</returns>
        internal T[] ReadFeatureRefArray<T>(DataField field) where T : Feature
        {
            string[] ids = ReadSimpleArray<string>(field);
            T[] result = new T[ids.Length];

            for (int i=0; i<result.Length; i++)
            {
                result[i] = m_MapModel.Find<T>(ids[i]);
                Debug.Assert(result[i] != null);                 
            }

            return result;
        }

        /// <summary>
        /// Reads a user-perceived feature ID using the standard naming convention.
        /// </summary>
        /// <returns>The user-perceived ID that was read (may be null)</returns>
        internal FeatureId ReadFeatureId()
        {
            return ReadFeatureId(DataField.Key, DataField.ForeignKey);
        }

        /// <summary>
        /// Reads a user-perceived feature ID using the specified naming tags.
        /// </summary>
        /// <param name="nativeField">The tag to use for a native ID.</param>
        /// <param name="foreignField">The tag to use for a foreign ID.</param>
        /// <returns>The user-perceived ID that was read (may be null)</returns>
        internal FeatureId ReadFeatureId(DataField nativeField, DataField foreignField)
        {
            if (IsNextField(nativeField))
            {
                uint nativeKey = m_Reader.ReadUInt32(nativeField.ToString());
                NativeId nid = m_MapModel.FindNativeId(nativeKey);

                if (nid == null)
                    return m_MapModel.AddNativeId(nativeKey);
                else
                    return nid;
            }

            if (IsNextField(foreignField))
            {
                string key = m_Reader.ReadString(foreignField.ToString());
                ForeignId fid = m_MapModel.FindForeignId(key);

                if (fid == null)
                    return m_MapModel.AddForeignId(key);
                else
                    return fid;
            }

            return null;
        }

        /// <summary>
        /// Reads a miscellaneous value.
        /// </summary>
        /// <typeparam name="T">The type of simple value that is known to the caller.</typeparam>
        /// <param name="field">A tag for the item</param>
        internal T ReadValue<T>(DataField field) where T : IConvertible
        {
            object o = ReadValueAsObject<T>(field);
            return (T)o;
        }

        /// <summary>
        /// Reads a miscellaneous value as an <c>object</c>
        /// </summary>
        /// <typeparam name="T">The type of simple value that is known to the caller.</typeparam>
        /// <param name="field">A tag for the item</param>
        /// <returns>The object obtained from the input stream</returns>
        object ReadValueAsObject<T>(DataField field) where T : IConvertible
        {
            string name = field.ToString();
            TypeCode typeCode = Type.GetTypeCode(typeof(T));

            switch (typeCode)
            {
                case TypeCode.Byte:
                    return m_Reader.ReadByte(name);

                case TypeCode.Int32:
                    return m_Reader.ReadInt32(name);

                case TypeCode.UInt32:
                    return m_Reader.ReadUInt32(name);

                case TypeCode.Int64:
                    return m_Reader.ReadInt64(name);

                case TypeCode.Double:
                    return m_Reader.ReadDouble(name);

                case TypeCode.Single:
                    return m_Reader.ReadSingle(name);

                case TypeCode.Boolean:
                    return m_Reader.ReadBool(name);

                case TypeCode.String:
                    return m_Reader.ReadString(name);
            }

            throw new NotSupportedException("Unexpected value type: " + typeof(T).Name);
        }

        /// <summary>
        /// Reads the next byte.
        /// </summary>
        /// <param name="field">A tag associated with the value</param>
        /// <returns>The byte value that was read.</returns>
        internal byte ReadByte(DataField field)
        {
            return m_Reader.ReadByte(field.ToString());
        }

        /// <summary>
        /// Reads a 4-byte signed integer.
        /// </summary>
        /// <param name="field">A tag associated with the value</param>
        /// <returns>The 4-byte value that was read.</returns>
        internal int ReadInt32(DataField field)
        {
            return m_Reader.ReadInt32(field.ToString());
        }

        /// <summary>
        /// Reads a 4-byte unsigned integer.
        /// </summary>
        /// <param name="field">A tag associated with the value</param>
        /// <returns>The 4-byte unsigned value that was read.</returns>
        internal uint ReadUInt32(DataField field)
        {
            return m_Reader.ReadUInt32(field.ToString());
        }

        /// <summary>
        /// Reads an 8-byte signed integer.
        /// </summary>
        /// <param name="field">A tag associated with the value</param>
        /// <returns>The 8-byte value that was read.</returns>
        internal long ReadInt64(DataField field)
        {
            return m_Reader.ReadInt64(field.ToString());
        }

        /// <summary>
        /// Reads an eight-byte floating-point value.
        /// </summary>
        /// <param name="field">A tag associated with the value</param>
        /// <returns>The 8-byte floating-point value that was read.</returns>
        internal double ReadDouble(DataField field)
        {
            return m_Reader.ReadDouble(field.ToString());
        }

        /// <summary>
        /// Reads a four-byte floating-point value.
        /// </summary>
        /// <param name="field">A tag associated with the value</param>
        /// <returns>The 4-byte floating-point value that was read.</returns>
        internal float ReadSingle(DataField field)
        {
            return m_Reader.ReadSingle(field.ToString());
        }

        /// <summary>
        /// Reads a one-byte boolean value.
        /// </summary>
        /// <param name="field">A tag associated with the value</param>
        /// <returns>The boolean value that was read.</returns>
        internal bool ReadBool(DataField field)
        {
            return m_Reader.ReadBool(field.ToString());
        }

        /// <summary>
        /// Reads a string.
        /// </summary>
        /// <param name="field">A tag associated with the value</param>
        /// <returns>The string that was read (null if nothing follows the name)</returns>
        internal string ReadString(DataField field)
        {
            return m_Reader.ReadString(field.ToString());
        }

        /// <summary>
        /// Checks whether the next data item has a specific field tag. Make a call to any
        /// <c>Read</c> method to actually advance.
        /// </summary>
        /// <param name="field">The field to look for</param>
        /// <returns>True if the next data item has the specified field tag</returns>
        internal bool IsNextField(DataField field)
        {
            return m_Reader.IsNextField(field.ToString());
        }
    }
}
