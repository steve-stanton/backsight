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

namespace Backsight
{
    /// <summary>
    /// Controls serialization of data to XML. All elements written using this
    /// class will be qualified with a type name. While this is a bit verbose,
    /// it should hopefully make it easier to evolve the XML schema over time.
    /// </summary>
    public class XmlContentWriter : XmlBase
    {
        #region Class data

        /// <summary>
        /// The object that actually does the writing.
        /// </summary>
        readonly XmlWriter m_Writer;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>XmlContentWriter</c> that wraps the supplied
        /// writing tool.
        /// </summary>
        /// <param name="writer">The object that actually does the writing.</param>
        public XmlContentWriter(XmlWriter writer)
        {
            m_Writer = writer;
        }

        #endregion

        /// <summary>
        /// Writes out an element with the specified name
        /// </summary>
        /// <param name="content">The content to write</param>
        /// <param name="name">The name for the XML element</param>
        public void WriteElement(IXmlContent content, string name)
        {
            m_Writer.WriteStartElement(name);
            m_Writer.WriteAttributeString("xsi", "type", null, content.GetType().Name);
            content.WriteContent(this);
            m_Writer.WriteEndElement();
        }

        /// <summary>
        /// Writes out the supplied <c>Int32</c> as an attribute
        /// </summary>
        /// <param name="name">The local name of the attribute</param>
        /// <param name="value">The value of the attribute</param>
        public void WriteInt(string name, int value)
        {
            m_Writer.WriteStartAttribute(name);
            m_Writer.WriteValue(value);
            m_Writer.WriteEndAttribute();
        }

        /// <summary>
        /// Writes out the supplied <c>UInt32</c> as an attribute
        /// </summary>
        /// <param name="name">The local name of the attribute</param>
        /// <param name="value">The value of the attribute</param>
        public void WriteUnsignedInt(string name, uint value)
        {
            m_Writer.WriteStartAttribute(name);
            m_Writer.WriteValue(value);
            m_Writer.WriteEndAttribute();
        }

        /// <summary>
        /// Writes out the supplied <c>Int64</c> as an attribute
        /// </summary>
        /// <param name="name">The local name of the attribute</param>
        /// <param name="value">The value of the attribute</param>
        public void WriteLong(string name, long value)
        {
            m_Writer.WriteStartAttribute(name);
            m_Writer.WriteValue(value);
            m_Writer.WriteEndAttribute();
        }

        /// <summary>
        /// Writes out the supplied string as an attribute
        /// </summary>
        /// <param name="name">The local name of the attribute</param>
        /// <param name="value">The value of the attribute</param>
        public void WriteString(string name, string value)
        {
            m_Writer.WriteAttributeString(name, value);
        }

        /// <summary>
        /// Writes out the supplied <c>Boolean</c> as an attribute
        /// </summary>
        /// <param name="name">The local name of the attribute</param>
        /// <param name="value">The value of the attribute</param>
        public void WriteBool(string name, bool value)
        {
            m_Writer.WriteStartAttribute(name);
            m_Writer.WriteValue(value);
            m_Writer.WriteEndAttribute();
        }

        /// <summary>
        /// Writes an array element for the supplied content
        /// </summary>
        /// <param name="arrayName">The name for the element representing the complete array</param>
        /// <param name="itemName">The element name for individual elements in the array</param>
        /// <param name="data">The content to write out</param>
        public void WriteArray(string arrayName, string itemName, IXmlContent[] data)
        {
            m_Writer.WriteStartElement(arrayName);

            foreach (IXmlContent xc in data)
                WriteElement(xc, itemName);

            m_Writer.WriteEndElement();
        }
    }
}
