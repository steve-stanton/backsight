/// <remarks>
/// Copyright 2008 - Steve Stanton. This file is part of Backsight
///
/// Backsight is free software; you can redistribute it and/or modify it under the terms
/// of the GNU Lesser General Public License as published by the Free Software Foundation;
/// either version 3 of the License, or (at your option) any later version.
///
/// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
/// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU Lesser General Public License for more details.
///
/// You should have received a copy of the GNU Lesser General Public License
/// along with this program. If not, see <http://www.gnu.org/licenses/>.
/// </remarks>

using System;
using System.Xml;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics;

using Backsight.Editor.Database;

namespace Backsight.Editor
{
    /// <summary>
    /// Reads back XML data that was previously created using the
    /// <see cref="XmlContentWriter"/> class.
    /// </summary>
    public class XmlContentReader
    {
        #region Class data

        /// <summary>
        /// The object that actually does the reading.
        /// </summary>
        XmlReader m_Reader;

        /// <summary>
        /// Cross-reference of type names to the corresponding constructor.
        /// </summary>
        readonly Dictionary<string, ConstructorInfo> m_Types;

        /// <summary>
        /// Spatial features that have been loaded.
        /// </summary>
        readonly Dictionary<InternalIdValue, Feature> m_Features;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>XmlContentReader</c> that wraps the supplied
        /// reading tool.
        /// </summary>
        /// <param name="numItem">The estimated number of items that will be
        /// loaded (used to initialize an index of the loaded data)</param>
        internal XmlContentReader(uint numItem)
        {
            m_Reader = null;
            m_Types = new Dictionary<string, ConstructorInfo>();
            m_Features = new Dictionary<InternalIdValue, Feature>((int)numItem);
        }

        #endregion

        //internal void LoadSession(SessionData session, 

        /// <summary>
        /// Attempts to locate the <c>Type</c> corresponding to the supplied type name
        /// </summary>
        /// <param name="typeName">The name of the type (unqualified with any assembly stuff)</param>
        /// <returns>The correspond class type (null if not found in application domain)</returns>
        /// <remarks>This stuff is in pretty murky territory for me. I would guess it's possible
        /// that the same type name could appear in more than one assembly, so there could be
        /// some ambiguity.</remarks>
        Type FindType(string typeName)
        {
            // The type is most likely part of the assembly that holds this class.
            // Note: I thought it might be sufficient to just call Type.GetType("Backsight.Editor."+typeName),
            // but that doesn't find Operation classes (it only works if you specify the sub-folder name too).
            Type result = FindType(GetType().Assembly, typeName);
            if (result != null)
                return result;

            // If things get moved about though, it's a bit more complicated...
            Assembly[] aa = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly a in aa)
            {
                result = FindType(a, typeName);
                if (result != null)
                    return result;
            }

            return null;
        }

        /// <summary>
        /// Searches the supplied assembly for a <c>Type</c> corresponding to the
        /// supplied type name
        /// </summary>
        /// <param name="a">The assembly to search</param>
        /// <param name="typeName">The name of the type (unqualified with any assembly stuff)</param>
        /// <returns>The corresponding type (null if not found)</returns>
        Type FindType(Assembly a, string typeName)
        {
            foreach (Type type in a.GetTypes())
            {
                if (type.Name == typeName)
                    return type;
            }

            return null;
        }

        public int ReadInt(string name)
        {
            string s = m_Reader[name];
            return (s==null ? 0 : Int32.Parse(s));
        }

        public uint ReadUnsignedInt(string name)
        {
            string s = m_Reader[name];
            return (s == null ? 0 : UInt32.Parse(s));
        }

        public long ReadLong(string name)
        {
            string s = m_Reader[name];
            return (s == null ? 0 : Int64.Parse(s));
        }

        public double ReadDouble(string name)
        {
            string s = m_Reader[name];
            return (s == null ? 0 : Double.Parse(s));
        }

        public string ReadString(string name)
        {
            return m_Reader[name];
        }

        internal InternalIdValue ReadId(string name)
        {
            string s = m_Reader[name];
            if (s==null)
                return new InternalIdValue();
            else
                return new InternalIdValue(s);
        }

        /*
        public IXmlContent ReadElement(string name)
        {
            // Read the next node if we're at the start
            if (m_Reader.NodeType == XmlNodeType.None)
            {
                if (!m_Reader.Read())
                    return null;
            }

            m_Reader.Read();

            // I'm not 100% sure what to expect, this is what I think I'll have...
            Debug.Assert(m_Reader.NodeType == XmlNodeType.Element);
            Console.WriteLine("Name="+m_Reader.Name);
            //Debug.Assert(m_Reader.Name == name);

            return ReadContent();
        }
        */
        public T ReadElement<T>(string name) where T : IXmlContent
        {
            // Ensure we're at an element
            while (m_Reader.NodeType != XmlNodeType.Element && m_Reader.Name != name)
            {
                if (!m_Reader.Read())
                    return default(T);
            }

            // If there's no type declaration, treat as empty element (since XmlReader.IsEmptyElement
            // is always returning true)
            string typeName = m_Reader["xsi:type"];
            if (String.IsNullOrEmpty(typeName))
                return default(T);
            //throw new Exception("Content does not contain xsi:type attribute");

            // If we haven't previously encountered the type, look up an appropriate
            // constructor (where possible, go for a constructor that accepts an
            // XmlContentReader parameter).
            if (!m_Types.ContainsKey(typeName))
            {
                Type t = FindType(typeName);
                if (t == null)
                    throw new Exception("Cannot create object with type: " + typeName);

                // Confirm that the type implements IXmlContent
                if (t.FindInterfaces(Module.FilterTypeName, "IXmlContent").Length == 0)
                    throw new Exception("IXmlContent not implemented by type: " + t.FullName);

                ConstructorInfo ci = t.GetConstructor(new Type[] { typeof(XmlContentReader) });
                if (ci == null)
                    ci = t.GetConstructor(Type.EmptyTypes);

                if (ci == null)
                    throw new Exception("Cannot locate suitable constructor for type: " + t.FullName);

                m_Types.Add(typeName, ci);
            }

            ConstructorInfo c = m_Types[typeName];
            T result;
            if (c.GetParameters().Length == 0)
            {
                result = (T)c.Invoke(new object[0]);
                //throw new NotImplementedException("Need to uncomment IXmlContent.ReadContent");
                result.ReadContent(this);
            }
            else
            {
                result = (T)c.Invoke(new object[] { this });
            }

            return result;
        }

        /*
         * Consider the following sample:
         * 
            <?xml version="1.0"?>
            <Edit xsi:type="Import" xmlns="Backsight" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
              <FeatureArray>
                <Feature xsi:type="PointFeature" CreationSequence="1" EntityId="27" Key="">
                  <Geometry xsi:type="PointGeometry" X="427567152600" Y="5521630899900" />
                </Feature>
                <Feature xsi:type="PointFeature" CreationSequence="5" EntityId="27" Key="">
                  <Geometry xsi:type="PointGeometry" X="427588490200" Y="5522161261200" />
                </Feature>
              </FeatureArray>
            </Edit>         

         * As you read through the above, you hit the following nodes (name and type),
         * (they'll possibly be interspersed with Whitespace nodes)

            xml XmlDeclaration
            Edit Element
              FeatureArray Element
                Feature Element
                  Geometry Element
                Feature EndElement
                Feature Element
                  Geometry Element
                Feature EndElement
              FeatureArray EndElement
            Edit EndElement
         
         * Note that there's no EndElement node for the geometry (where everything
         * consists of attributes)
         */
        public T[] ReadArray<T>(string arrayName, string itemName) where T : IXmlContent
        {
            // Ensure we're at an element
            while (m_Reader.NodeType != XmlNodeType.Element || m_Reader.Name != arrayName)
            {
                if (!m_Reader.Read())
                    throw new Exception("Array element not found");
            }

            // Pick up array size (optional nicety, may be inaccurate)
            uint capacity = 1000;
            if (HasAttribute("Capacity"))
                capacity = ReadUnsignedInt("Capacity");

            List<T> result = new List<T>((int)capacity);

            m_Reader.Read();
            while (m_Reader.NodeType != XmlNodeType.EndElement || m_Reader.Name != arrayName)
            {
                T item = ReadElement<T>(itemName);
                result.Add(item);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Obtains the spatial feature associated with a reference that was
        /// originally output using <see cref="XmlContentWriter.WriteFeatureReference"/>
        /// </summary>
        /// <param name="name">The local name of the attribute</param>
        /// <returns>The corresponding feature</returns>
        internal T ReadFeatureByReference<T>(string name) where T : Feature
        {
            // Get the internal ID of the feature
            InternalIdValue iid = ReadId(name);
            if (iid.IsEmpty)
                return null;

            Debug.Assert(m_Features.ContainsKey(iid));
            return (T)m_Features[iid];
        }

        /// <summary>
        /// Loads the content of an editing operation
        /// </summary>
        /// <param name="session">The session the edit is part of</param>
        /// <param name="editSequence">The item sequence for the edit</param>
        /// <param name="data">The data that describes the edit</param>
        /// <returns>The created editing object</returns>
        internal Operation LoadOperation(SessionData session, uint editSequence, XmlReader data)
        {
            // Remember the session and sequence number of the edit that's being loaded
            SessionData.CurrentSession = session;
            Operation.CurrentEditSequence = editSequence;

            try
            {
                m_Reader = data;
                return ReadElement<Operation>("Edit");
            }

            finally
            {
                m_Reader = null;
            }
        }

        /// <summary>
        /// Checks whether an attribute with the specified name is part of
        /// the current element.
        /// </summary>
        /// <param name="name">The local name of the attribute</param>
        /// <returns>True if the attribute is defined</returns>
        internal bool HasAttribute(string name)
        {
            return (m_Reader[name] != null);
        }
    }
}
