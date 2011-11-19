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
using System.Text;

using Backsight.Environment;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="06-NOV-2011" />
    /// <summary>
    /// Serializes instances of <see cref="IPersistent"/>. Objects that have been saved in
    /// this way can be loaded back using an instance of <see cref="EditDeserializer"/>.
    /// </summary>
    class EditSerializer
    {
        #region Static

        /// <summary>
        /// Serializes an object as a text string.
        /// </summary>
        /// <typeparam name="T">The type of object being written (as it is known to the instance
        /// that contains it)</typeparam>
        /// <param name="field">The tag that identifies the item.</param>
        /// <param name="value">The object to write (may be null)</param>
        /// <returns>The result of serializing the supplied object</returns>
        internal static string GetSerializedString<T>(DataField field, T value) where T : IPersistent
        {
            EditSerializer es = new EditSerializer();
            es.WritePersistent(field.ToString(), value);
            return es.ToSerializedString();
        }

        #endregion

        #region Class data

        /// <summary>
        /// The mechanism for writing out data.
        /// </summary>
        IEditWriter m_Writer;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EditSerializer"/> class
        /// that makes use of a <see cref="TextEditWriter"/>.
        /// </summary>
        internal EditSerializer()
            : this(new TextEditWriter())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditSerializer"/> class
        /// that makes use of the supplied writer.
        /// </summary>
        /// <param name="writer">The mechanism for writing out data.</param>
        internal EditSerializer(IEditWriter writer)
        {
            m_Writer = writer;
        }

        #endregion

        /// <summary>
        /// Writes an array of objects to a storage medium.
        /// </summary>
        /// <typeparam name="T">The type of objects within the array (as it is known to the instance
        /// that refers to it)</typeparam>
        /// <param name="field">The tag that identifies the item.</param>
        /// <param name="array">The array to write (may be null)</param>
        internal void WritePersistentArray<T>(DataField field, T[] array) where T : IPersistent
        {
            string name = field.ToString();

            if (array == null)
            {
                m_Writer.WriteString(name, "null");
            }
            else
            {
                m_Writer.WriteString(name, null);
                m_Writer.WriteBeginObject();
                m_Writer.WriteUInt32(DataField.Length.ToString(), (uint)array.Length);

                for (int i=0; i<array.Length; i++)
                {
                    string itemName = string.Format("[{0}]", i);
                    WritePersistent<T>(itemName, array[i]);
                }

                m_Writer.WriteEndObject();
            }
        }

        /// <summary>
        /// Writes an array of <see cref="FeatureStub"/>, creating them from the supplied
        /// feature array.
        /// </summary>
        /// <param name="field">The tag that identifies the item.</param>
        /// <param name="features">The features to convert into stubs before writing them out.</param>
        internal void WriteFeatureStubArray(DataField field, Feature[] features)
        {
            var stubs = new FeatureStub[features.Length];

            for (int i = 0; i < stubs.Length; i++)
                stubs[i] = new FeatureStub(features[i]);

            WritePersistentArray<FeatureStub>(field, stubs);
        }

        /// <summary>
        /// Writes an array of simple types to a storage medium.
        /// </summary>
        /// <typeparam name="T">The type of objects within the array (as it is known to the instance
        /// that refers to it)</typeparam>
        /// <param name="field">The tag that identifies the array.</param>
        /// <param name="array">The array to write (may be null)</param>
        internal void WriteSimpleArray<T>(DataField field, T[] array) where T : IConvertible
        {
            string name = field.ToString();

            if (array == null)
            {
                m_Writer.WriteString(name, "null");
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < array.Length; i++)
                {
                    if (i > 0)
                        sb.Append(";");

                    sb.Append(array[i].ToString());
                }

                m_Writer.WriteString(name, sb.ToString());
            }
        }

        /// <summary>
        /// Writes the content of an object to a storage medium.
        /// </summary>
        /// <typeparam name="T">The type of object being written (as it is known to the instance
        /// that contains it)</typeparam>
        /// <param name="field">The tag that identifies the item.</param>
        /// <param name="value">The object to write (may be null)</param>
        internal void WritePersistent<T>(DataField field, T value) where T : IPersistent
        {
            WritePersistent(field.ToString(), value);
        }

        /// <summary>
        /// Writes the content of an object to a storage medium.
        /// </summary>
        /// <typeparam name="T">The type of object being written (as it is known to the instance
        /// that contains it)</typeparam>
        /// <param name="name">The tag that identifies the item.</param>
        /// <param name="value">The object to write (may be null)</param>
        void WritePersistent<T>(string name, T value) where T : IPersistent
        {
            if (value == null)
            {
                m_Writer.WriteString(name, "null");
            }
            else
            {
                // Output the data type of the object only if it's a derived type
                string parentTypeName = typeof(T).Name;
                string valueTypeName = value.GetType().Name;

                if (parentTypeName == valueTypeName)
                    m_Writer.WriteString(name, null);
                else
                    m_Writer.WriteString(name, valueTypeName);

                m_Writer.WriteBeginObject();
                value.WriteData(this);
                m_Writer.WriteEndObject();
            }
        }

        /// <summary>
        /// Writes an entity type to a storage medium.
        /// </summary>
        /// <param name="field">The tag that identifies the item.</param>
        /// <param name="value">The entity type to write</param>
        internal void WriteEntity(DataField field, IEntity entity)
        {
            m_Writer.WriteInt32(field.ToString(), entity.Id);
        }

        /// <summary>
        /// Writes a distance unit type to a storage medium.
        /// </summary>
        /// <param name="field">The tag that identifies the item.</param>
        /// <param name="value">The distance unit type to write</param>
        internal void WriteDistanceUnit(DataField field, DistanceUnit value)
        {
            m_Writer.WriteInt32(field.ToString(), (int)value.UnitType);
        }

        /// <summary>
        /// Write a value in radians to a storage medium.
        /// </summary>
        /// <param name="field">The tag that identifies the item.</param>
        /// <param name="value">The radian value to write</param>
        internal void WriteRadians(DataField field, RadianValue value)
        {
            m_Writer.WriteString(field.ToString(), value.AsShortString());
        }

        /// <summary>
        /// Writes a 2D position to a storage medium.
        /// </summary>
        /// <param name="xField">The tag that identifies the easting value.</param>
        /// <param name="yField">The tag that identifies the northing value.</param>
        /// <param name="value">The position to write</param>
        internal void WritePointGeometry(DataField xField, DataField yField, PointGeometry value)
        {
            m_Writer.WriteInt64(xField.ToString(), value.Easting.Microns);
            m_Writer.WriteInt64(yField.ToString(), value.Northing.Microns);
        }

        /// <summary>
        /// Writes a reference to a spatial feature to a storage medium.
        /// </summary>
        /// <typeparam name="T">The type of spatial feature being written</typeparam>
        /// <param name="field">The tag that identifies the item.</param>
        /// <param name="feature">The feature that is referenced.</param>
        internal void WriteFeatureRef<T>(DataField field, T feature) where T : Feature
        {
            m_Writer.WriteString(field.ToString(), feature.DataId);
        }

        /// <summary>
        /// Writes an array of references to spatial features.
        /// </summary>
        /// <typeparam name="T">The type of spatial feature being written</typeparam>
        /// <param name="field">The tag that identifies the item.</param>
        /// <param name="features">The features that are referenced.</param>
        internal void WriteFeatureRefArray<T>(DataField field, T[] features) where T : Feature
        {
            string[] ids = new string[features.Length];
            for (int i=0; i<ids.Length; i++)
                ids[i] = features[i].DataId;

            WriteSimpleArray<string>(field, ids);
        }

        /// <summary>
        /// Writes a user-perceived feature ID to a storage medium using a standard naming
        /// convention.
        /// </summary>
        /// <param name="featureId">The ID to write (if null, nothing will be written)</param>
        internal void WriteFeatureId(FeatureId featureId)
        {
            WriteFeatureId(DataField.Key, DataField.ForeignKey, featureId);
        }

        /// <summary>
        /// Writes a user-perceived feature ID to a storage medium using the specified naming tags.
        /// convention.
        /// </summary>
        /// <param name="nativeField">The tag to use for a native ID.</param>
        /// <param name="foreignField">The tag to use for a foreign ID.</param>
        /// <param name="featureId">The ID to write (if null, nothing will be written)</param>
        void WriteFeatureId(DataField nativeField, DataField foreignField, FeatureId featureId)
        {
            if (featureId != null)
            {
                if (featureId is NativeId)
                    m_Writer.WriteUInt32(nativeField.ToString(), featureId.RawId);
                else
                    m_Writer.WriteString(foreignField.ToString(), featureId.FormattedKey);
            }
        }

        /// <summary>
        /// Writes a miscellaneous value to a storage medium.
        /// </summary>
        /// <typeparam name="T">The type of simple value that is known to the caller.</typeparam>
        /// <param name="field">The tag that identifies the item.</param>
        /// <param name="value">The value to write</param>
        internal void WriteValue<T>(DataField field, T value) where T : IConvertible
        {
            string name = field.ToString();
            TypeCode typeCode = Type.GetTypeCode(typeof(T));
            object o = (object)value;

            switch (typeCode)
            {
                case TypeCode.Byte:
                    m_Writer.WriteByte(name, (byte)o);
                    return;

                case TypeCode.Int32:
                    m_Writer.WriteInt32(name, (int)o);
                    return;

                case TypeCode.UInt32:
                    m_Writer.WriteUInt32(name, (uint)o);
                    return;

                case TypeCode.Int64:
                    m_Writer.WriteInt64(name, (long)o);
                    return;

                case TypeCode.Double:
                    m_Writer.WriteDouble(name, (double)o);
                    return;

                case TypeCode.Single:
                    m_Writer.WriteSingle(name, (float)o);
                    return;

                case TypeCode.Boolean:
                    m_Writer.WriteBool(name, (bool)o);
                    return;

                case TypeCode.String:
                    m_Writer.WriteString(name, (string)o);
                    return;
            }

            throw new NotSupportedException("Unexpected value type: " + typeof(T).Name);
        }

        /// <summary>
        /// Writes an unsigned byte to a storage medium.
        /// </summary>
        /// <param name="field">The tag that identifies the item.</param>
        /// <param name="value">The unsigned byte to write.</param>
        internal void WriteByte(DataField field, byte value)
        {
            m_Writer.WriteByte(field.ToString(), value);
        }

        /// <summary>
        /// Writes a four-byte signed integer to a storage medium.
        /// </summary>
        /// <param name="field">The tag that identifies the item.</param>
        /// <param name="value">The four-byte signed integer to write.</param>
        internal void WriteInt32(DataField field, int value)
        {
            m_Writer.WriteInt32(field.ToString(), value);
        }

        /// <summary>
        /// Writes a four-byte unsigned integer to a storage medium.
        /// </summary>
        /// <param name="field">The tag that identifies the item.</param>
        /// <param name="value">The four-byte unsigned integer to write.</param>
        internal void WriteUInt32(DataField field, uint value)
        {
            m_Writer.WriteUInt32(field.ToString(), value);
        }

        /// <summary>
        /// Writes an eight-byte signed integer to a storage medium.
        /// </summary>
        /// <param name="field">The tag that identifies the item.</param>
        /// <param name="value">The eight-byte signed integer to write.</param>
        internal void WriteInt64(DataField field, long value)
        {
            m_Writer.WriteInt64(field.ToString(), value);
        }

        /// <summary>
        /// Writes an eight-byte floating-point value to a storage medium.
        /// </summary>
        /// <param name="field">The tag that identifies the item.</param>
        /// <param name="value">The eight-byte floating-point value to write.</param>
        internal void WriteDouble(DataField field, double value)
        {
            m_Writer.WriteDouble(field.ToString(), value);
        }

        /// <summary>
        /// Writes an four-byte floating-point value to a storage medium.
        /// </summary>
        /// <param name="field">The tag that identifies the item.</param>
        /// <param name="value">The four-byte floating-point value to write.</param>
        internal void WriteSingle(DataField field, float value)
        {
            m_Writer.WriteSingle(field.ToString(), value);
        }

        /// <summary>
        /// Writes a one-byte boolean value to a storage medium.
        /// </summary>
        /// <param name="field">The tag that identifies the item.</param>
        /// <param name="value">The boolean value to write (0 or 1).</param>
        internal void WriteBool(DataField field, bool value)
        {
            m_Writer.WriteBool(field.ToString(), value);
        }

        /// <summary>
        /// Writes a string to a storage medium.
        /// </summary>
        /// <param name="field">The tag that identifies the item.</param>
        /// <param name="value">The string to write (if a null is supplied, just the name tag will be written).</param>
        internal void WriteString(DataField field, string value)
        {
            m_Writer.WriteString(field.ToString(), value);
        }

        /// <summary>
        /// Expresses the content produced by this serializer as a string.
        /// </summary>
        /// <returns>The content produced by this serializer</returns>
        internal string ToSerializedString()
        {
            return m_Writer.ToString();
        }
    }
}
