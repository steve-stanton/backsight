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
        public XmlContentReader(uint numItem)
        {
            m_Reader = null;
            m_Types = new Dictionary<string, ConstructorInfo>();
            m_Features = new Dictionary<InternalIdValue, Feature>((int)numItem);
        }

        #endregion

        //internal void LoadSession(SessionData session, 

        /// <summary>
        /// Loads the next content element (if any) from the XML content
        /// </summary>
        /// <returns>The next content element (null if there's nothing further)</returns>
        public IXmlContent ReadContent()
        {
            // Read the next node if we're at the start
            if (m_Reader.NodeType == XmlNodeType.None)
            {
                if (!m_Reader.Read())
                    return null;
            }

            // Assume we've reached the next element
            Debug.Assert(m_Reader.NodeType == XmlNodeType.Element);

            // Empty elements are placeholders for nulls.
            //if (m_Reader.IsEmptyElement)
            //    return null;

            // If there's no type declaration, treat as empty element (since XmlReader.IsEmptyElement
            // is always returning true)
            string typeName = m_Reader["xsi:type"];
            if (String.IsNullOrEmpty(typeName))
                return null;
                //throw new Exception("Content does not contain xsi:type attribute");

            // If we haven't previously encountered the type, look up an appropriate
            // constructor (where possible, go for a constructor that accepts an
            // XmlContentReader parameter).
            if (!m_Types.ContainsKey(typeName))
            {
                Type t = FindType(typeName);
                if (t==null)
                    throw new Exception("Cannot create object with type: "+typeName);

                // Confirm that the type implements IXmlContent
                if (t.FindInterfaces(Module.FilterTypeName, "IXmlContent").Length==0)
                    throw new Exception("IXmlContent not implemented by type: "+t.FullName);

                ConstructorInfo ci = t.GetConstructor(new Type[] { typeof(XmlContentReader) });
                if (ci==null)
                    ci = t.GetConstructor(Type.EmptyTypes);

                if (ci==null)
                    throw new Exception("Cannot locate suitable constructor for type: "+t.FullName);

                m_Types.Add(typeName, ci);
            }

            ConstructorInfo c = m_Types[typeName];
            IXmlContent result;
            if (c.GetParameters().Length==0)
            {
                result = (IXmlContent)c.Invoke(new object[0]);
                //throw new NotImplementedException("Need to uncomment IXmlContent.ReadContent");
                result.ReadContent(this);
            }
            else
            {
                result = (IXmlContent)c.Invoke(new object[] { this });
            }

            return result;
        }

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
            Assembly[] aa = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly a in aa)
            {
                foreach (Type type in a.GetTypes())
                {
                    if (type.Name == typeName)
                        return type;
                }
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
        public T ReadElement<T>(string name)
        {
            // Read the next node if we're at the start
            if (m_Reader.NodeType == XmlNodeType.None)
            {
                if (!m_Reader.Read())
                    return default(T);
            }

            m_Reader.Read();

            // I'm not 100% sure what to expect, this is what I think I'll have...
            Debug.Assert(m_Reader.NodeType == XmlNodeType.Element);
            //Console.WriteLine("Name=" + m_Reader.Name);
            //Debug.Assert(m_Reader.Name == name);

            return (T)ReadContent();
        }

        public T[] ReadArray<T>(string arrayName, string itemName) where T : IXmlContent
        {
            Debug.Assert(m_Reader.Name == arrayName);
            List<T> result = new List<T>(1000);

            while (m_Reader.NodeType != XmlNodeType.EndElement)
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
    }
}
