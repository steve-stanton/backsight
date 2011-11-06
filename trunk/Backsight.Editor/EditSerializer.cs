using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Backsight.Environment;

namespace Backsight.Editor
{
    class EditSerializer
    {
        #region Static

        /// <summary>
        /// Serializes an object as a text string.
        /// </summary>
        /// <typeparam name="T">The type of object being written (as it is known to the instance
        /// that contains it)</typeparam>
        /// <param name="name">A name tag for the item</param>
        /// <param name="value">The object to write (may be null)</param>
        /// <returns>The result of serializing the supplied object</returns>
        internal static string GetSerializedString<T>(string name, T value) where T : IPersistent
        {
            EditSerializer es = new EditSerializer();
            es.WriteObject(name, value);
            return es.Writer.ToString();
        }

        #endregion

        #region Class data

        IEditWriter m_Writer;

        #endregion

        #region Constructors

        internal EditSerializer()
            : this(new TextEditWriter())
        {
        }

        internal EditSerializer(IEditWriter writer)
        {
            m_Writer = writer;
        }

        #endregion

        internal IEditWriter Writer
        {
            get { return m_Writer; }
            set { m_Writer = value; }
        }

        /// <summary>
        /// Writes the content of an object to a storage medium.
        /// </summary>
        /// <typeparam name="T">The type of object being written (as it is known to the instance
        /// that contains it)</typeparam>
        /// <param name="name">A name tag for the item</param>
        /// <param name="value">The object to write (may be null)</param>
        internal void WriteObject<T>(string name, T value) where T : IPersistent
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
        /// <param name="name">A name tag for the item</param>
        /// <param name="value">The entity type to write. </param>
        internal void WriteEntity(string name, IEntity entity)
        {
            m_Writer.WriteInt32(name, entity.Id);
        }

        /// <summary>
        /// Write a value in radians to a storage medium.
        /// </summary>
        /// <param name="name">A name tag for the item</param>
        /// <param name="value">The radian value to write</param>
        internal void WriteRadians(string name, RadianValue value)
        {
            m_Writer.WriteString(name, value.AsShortString());
        }

        /// <summary>
        /// Writes a reference to a spatial feature to a storage medium.
        /// </summary>
        /// <typeparam name="T">The type of spatial feature being written</typeparam>
        /// <param name="name">A name tag for the item</param>
        /// <param name="feature">The feature that is referenced.</param>
        internal void WriteFeature<T>(string name, T feature) where T : Feature
        {
            m_Writer.WriteString(name, feature.DataId);
        }
    }
}
