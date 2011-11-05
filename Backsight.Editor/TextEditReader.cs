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
using System.IO;
using System.Reflection;

using Backsight.Editor.Xml;
using Backsight.Environment;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="01-NOV-2011" />
    /// <summary>
    /// Implementation of <see cref="IEditReader"/> that loads edits using an instance of
    /// <see cref="System.IO.TextReader"/>.
    /// </summary>
    class TextEditReader : IEditReader
    {
        #region Class data

        /// <summary>
        /// The mechanism for reading the text for the current edit.
        /// </summary>
        TextReader m_Reader;

        /// <summary>
        /// Index of the constructors that accept an instance of <see cref="IEditReader"/> (and which
        /// belong to classes that implement <see cref="IPersistent"/>), keyed by the short type name.
        /// This is restricted to the current assembly, and excludes abstract classes, as well as miscellaneous
        /// mystery classes that seem to be produced by NET (with type names that start with the
        /// "&lt;" character).
        /// </summary>
        readonly Dictionary<string, ConstructorInfo> m_Constructors;

        /// <summary>
        /// The line of text AFTER the current line (used to peek ahead).
        /// </summary>
        string m_NextLine;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TextEditReader"/> class.
        /// </summary>
        /// <exception cref="ApplicationException">If no suitable persistent classes
        /// could be found in the current assembly.</exception>
        internal TextEditReader()
        {
            m_Reader = null;
            m_Constructors = LoadConstructors();

            if (m_Constructors.Count == 0)
                throw new ApplicationException("Cannot find any persistent classes");
        }

        #endregion

        /// <summary>
        /// Specifies the current source of the text data.
        /// </summary>
        /// <param name="reader">The text storage medium to read from</param>
        internal void SetReader(TextReader reader)
        {
            m_Reader = reader;
            ReadNextLine();
        }

        /// <summary>
        /// Loads constructor information for persistent classes.
        /// </summary>
        /// <returns>The constructors for each persistent class in this assembly, keyed by the
        /// short type name.</returns>
        static Dictionary<string, ConstructorInfo> LoadConstructors()
        {
            Dictionary<string, Type> all = DataFactory.GetTypeIndex();
            Dictionary<string, ConstructorInfo> result = new Dictionary<string, ConstructorInfo>();
            string iName = typeof(IPersistent).Name; // do it this way in case I rename the interface

            foreach (KeyValuePair<string, Type> kvp in all)
            {
                Type t = kvp.Value;
                if (t.GetInterface(iName) != null)
                {
                    ConstructorInfo ci = t.GetConstructor(BindingFlags.Public |
                                                          BindingFlags.NonPublic |
                                                          BindingFlags.Instance |
                                                          BindingFlags.DeclaredOnly, null,
                                                          new Type[] { typeof(DeserializationFactory) }, null);

                    if (ci == null)
                        throw new ApplicationException("Class " + t.Name + " implements IPersistent but does not provide deserialization constructor");

                    result.Add(t.Name, ci);
                }
            }

            return result;
        }

        /// <summary>
        /// Reads any text that precedes the data values for an object.
        /// </summary>
        void ReadBeginObject()
        {
            ReadLiteralLine("{");
        }

        /// <summary>
        /// Reads any text that should follow the data values for an object.
        /// </summary>
        void ReadEndObject()
        {
            ReadLiteralLine("}");
        }

        /// <summary>
        /// Reads a text string that should appear on the next line of the storage medium.
        /// </summary>
        /// <param name="requiredString">The string that should appear next.</param>
        /// <exception cref="ArgumentException">If the required string was not found.</exception>
        void ReadLiteralLine(string requiredString)
        {
            string s = ReadNextLine();
            if (s != requiredString)
                throw new ArgumentException(String.Format("Expected {0}, found {1}", requiredString, s));
        }

        /// <summary>
        /// Attempts to read the next line from the storage medium.
        /// </summary>
        /// <remarks>
        /// The constructor should have made an initial call to obtain the very first line of text.
        /// Thereafter, calling this method will read the subsequent line and return the one that was
        /// previously read!
        /// </remarks>
        /// <returns>The next line of text, with any leading and trailing whitespace trimmed off
        /// (null if there is no more text).</returns>
        string ReadNextLine()
        {
            string result = m_NextLine;

            // You get back null on reaching the end
            m_NextLine = m_Reader.ReadLine();

            if (m_NextLine != null)
                m_NextLine = m_NextLine.Trim();

            return result;
        }

        /// <summary>
        /// Reads the next data value from the text storage medium (but does not attempt
        /// to parse it).
        /// </summary>
        /// <param name="name">A name tag associated with the value</param>
        /// <returns>The text following the name tag (null if there's nothing there)</returns>
        string ReadValue(string name)
        {
            string s = ReadNextLine();
            if (!s.StartsWith(name))
                throw new ArgumentException(String.Format("Expected {0} but found {1}", name, s));

            int eqIndex = s.IndexOf('=');
            if (eqIndex < 0)
                return null;

            string result = s.Substring(eqIndex + 1);
            return (result.Length == 0 ? null : result);
        }

        #region IEditReader Members

        /// <summary>
        /// Reads the next byte.
        /// </summary>
        /// <param name="name">A name tag associated with the value</param>
        /// <returns>The byte value that was read.</returns>
        public byte ReadByte(string name)
        {
            return Convert.ToByte(ReadValue(name));
        }

        /// <summary>
        /// Reads a 4-byte signed integer.
        /// </summary>
        /// <param name="name">A name tag associated with the value</param>
        /// <returns>The 4-byte value that was read.</returns>
        public int ReadInt32(string name)
        {
            return Convert.ToInt32(ReadValue(name));
        }

        /// <summary>
        /// Reads an entity type for a spatial feature.
        /// </summary>
        /// <param name="name">A name tag associated with the value</param>
        /// <returns>The entity type that was read.</returns>
        public IEntity ReadEntity(string name)
        {
            int id = Convert.ToInt32(ReadValue(name));
            return EnvironmentContainer.FindEntityById(id);
        }

        /// <summary>
        /// Reads a 4-byte unsigned integer.
        /// </summary>
        /// <param name="name">A name tag associated with the value</param>
        /// <returns>The 4-byte unsigned value that was read.</returns>
        public uint ReadUInt32(string name)
        {
            return Convert.ToUInt32(ReadValue(name));
        }

        /// <summary>
        /// Reads an eight-byte floating-point value.
        /// </summary>
        /// <param name="name">A name tag associated with the value</param>
        /// <returns>
        /// The 8-byte floating-point value that was read.
        /// </returns>
        public double ReadDouble(string name)
        {
            return Convert.ToDouble(ReadValue(name));
        }

        /// <summary>
        /// Reads a four-byte floating-point value.
        /// </summary>
        /// <param name="name">A name tag associated with the value</param>
        /// <returns>
        /// The 4-byte floating-point value that was read.
        /// </returns>
        public float ReadSingle(string name)
        {
            return Convert.ToSingle(ReadValue(name));
        }

        /// <summary>
        /// Reads a one-byte boolean value.
        /// </summary>
        /// <param name="name">A name tag associated with the value</param>
        /// <returns>The boolean value that was read.</returns>
        public bool ReadBool(string name)
        {
            byte b = Convert.ToByte(ReadValue(name));
            return (b == 0 ? false : true);
        }

        /// <summary>
        /// Reads a string.
        /// </summary>
        /// <param name="name">A name tag associated with the value</param>
        /// <returns>The string that was read.</returns>
        public string ReadString(string name)
        {
            return ReadValue(name);
        }

        /// <summary>
        /// Reads a value in radians.
        /// </summary>
        /// <param name="name">A name tag associated with the value</param>
        /// <returns>The radian value that was read.</returns>
        public RadianValue ReadRadians(string name)
        {
            string s = ReadString(name);
            double d = RadianValue.Parse(s);
            return new RadianValue(d);
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
        public T ReadObject<T>(string name) where T : IPersistent
        {
            string typeName = ReadValue(name);

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

            // Read opening bracket
            ReadBeginObject();

            try
            {
                return (T)ci.Invoke(new object[] { this });
            }

            finally
            {
                // Read the closing bracket
                ReadEndObject();
            }
        }

        /// <summary>
        /// Reads a reference to a spatial feature, using that reference to obtain the
        /// corresponding feature.
        /// </summary>
        /// <typeparam name="T">The type of spatial feature expected by the caller</typeparam>
        /// <param name="name">A name tag associated with the value</param>
        /// <returns>
        /// The feature that was read (null if not found, or the reference was undefined). May
        /// actually have a type that is derived from the supplied type.
        /// </returns>
        /// <remarks>This does not create a brand new feature. Rather, it uses a reference
        /// to try to obtain a feature that should have already been created.
        /// </remarks>
        public T ReadFeature<T>(string name) where T : Feature
        {
            string dataId = ReadValue(name);
            if (dataId == null)
                return default(T);

            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether the next line of text refers to a specific name tag. Make a call to
        /// <see cref="ReadNextLine"/> to actually advance.
        /// </summary>
        /// <param name="name">The name tag to check for</param>
        /// <returns>True if the next line refers to the specified name tag</returns>
        public bool IsNextName(string name)
        {
            if (String.IsNullOrEmpty(m_NextLine))
                return false;

            int eqIndex = m_NextLine.IndexOf('=');
            if (eqIndex < 0)
                return false;

            if (eqIndex != name.Length)
                return false;

            string result = m_NextLine.Substring(0, name.Length);
            return (result == name);
        }

        #endregion
    }
}
