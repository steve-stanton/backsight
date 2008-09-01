// <remarks>
// Copyright 2008 - Steve Stanton. This file is part of Backsight
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
using System.Xml;
using System.Reflection;

namespace Backsight.Editor
{
    /// <summary>
    /// Controls deserialization of data that was previously written using the
    /// <see cref="ContentWriter"/> class.
    /// </summary>
    public class ContentReader
    {
        #region Class data

        /// <summary>
        /// The object that does the low-level reading.
        /// </summary>
        XmlReader m_Reader;

        /// <summary>
        /// Cross-reference of type names to the corresponding constructor.
        /// </summary>
        readonly Dictionary<string, ConstructorInfo> m_Types;

        /// <summary>
        /// The elements that are being read
        /// </summary>
        readonly Stack<ContentElement> m_Elements;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>ContentReader</c>
        /// </summary>
        internal ContentReader()
        {
            m_Reader = null;
            m_Types = new Dictionary<string, ConstructorInfo>();
            m_Elements = new Stack<ContentElement>(10);
        }

        #endregion

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

        public ContentElement ReadElement<T>(string name) where T : IXmlContent
        {
            // Ensure we're at an element
            if (!ReadToElement(name))
                return null; //default(T);

            // If there's no type declaration, assume we're dealing with a null object
            string typeName = m_Reader["xsi:type"];
            if (String.IsNullOrEmpty(typeName))
                return null; //default(T);

            // If we haven't previously encountered the type, look up a
            // default constructor
            if (!m_Types.ContainsKey(typeName))
            {
                Type t = FindType(typeName);
                if (t == null)
                    throw new Exception("Cannot create object with type: " + typeName);

                // Confirm that the type implements IXmlContent
                if (t.FindInterfaces(Module.FilterTypeName, "IXmlContent").Length == 0)
                    throw new Exception("IXmlContent not implemented by type: " + t.FullName);

                // Locate default constructor
                ConstructorInfo ci = t.GetConstructor(Type.EmptyTypes);
                if (ci == null)
                    throw new Exception("Cannot locate default constructor for type: " + t.FullName);

                m_Types.Add(typeName, ci);
            }

            // Create the instance
            ConstructorInfo c = m_Types[typeName];
            T result = (T)c.Invoke(new object[0]);
            ContentElement parent = this.CurrentElement;
            ContentElement resultElement = new ContentElement(parent, name, result);

            // Load the instance
            try
            {
                m_Elements.Push(resultElement);
                result.ReadContent(this);
            }

            finally
            {
                m_Elements.Pop();
            }

            return resultElement;
        }

        bool ReadToElement(string name)
        {
            return m_Reader.ReadToFollowing(name);
        }

        /// <summary>
        /// The element that is currently being loaded.
        /// </summary>
        internal ContentElement CurrentElement
        {
            get
            {
                if (m_Elements.Count == 0)
                    return null;
                else
                    return m_Elements.Peek();
            }
        }
    }
}
