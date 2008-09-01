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
using System.Collections.Generic;
using System.Xml;

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
        readonly XmlReader m_Reader;

        /// <summary>
        /// The elements that are being read
        /// </summary>
        readonly Stack<ContentElement> m_Elements;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>ContentReader</c>
        /// </summary>
        internal ContentReader(XmlReader reader)
        {
            m_Reader = reader;
            m_Elements = new Stack<ContentElement>(10);
        }

        #endregion

        internal ContentElement Load()
        {
            return ReadElement<Operation>("Edit");
        }

        public ContentElement ReadElement<T>(string name) where T : IXmlContent
        {
            return null;
            /*
            // Ensure we're at an element
            if (!ReadToElement(name))
                return default(T);

            // If there's no type declaration, assume we're dealing with a null object
            string typeName = m_Reader["xsi:type"];
            if (String.IsNullOrEmpty(typeName))
                return default(T);
            //throw new Exception("Content does not contain xsi:type attribute");

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

            // Load the instance
            try
            {
                m_Elements.Push(result);
                result.ReadContent(this);
            }

            finally
            {
                m_Elements.Pop();
            }

            // If we've just read a spatial feature, remember it
            AddFeature(result as Feature);

            // Read the next node (if any), in case this element has an EndElement node. If it
            // doesn't we should be all ready to start reading the next element.
            //m_Reader.Read();

            return result;
             */
        }
    }
}
