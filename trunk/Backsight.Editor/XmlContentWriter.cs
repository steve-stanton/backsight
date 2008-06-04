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
using System.Text;
using System.Xml.Schema;

namespace Backsight.Editor
{
    /// <summary>
    /// Controls serialization of data to XML. All elements written using this
    /// class will be qualified with a type name. While this is a bit verbose,
    /// it should hopefully make it easier to evolve the XML schema over time.
    /// </summary>
    public class XmlContentWriter
    {
        #region Static

        /// <summary>
        /// The target namespace that should be used when writing out the
        /// top-level element. Not null.
        /// </summary>
        static string s_TargetNamespace = "Backsight";

        /// <summary>
        /// The target namespace that should be used when writing out the
        /// top-level element. Not null.
        /// </summary>
        public static string TargetNamespace
        {
            get { return s_TargetNamespace; }
            set { s_TargetNamespace = (value==null ? String.Empty : value); }
        }

        public static string GetXml(string name, IXmlContent content)
        {
            StringBuilder sb = new StringBuilder(1000);
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.ConformanceLevel = ConformanceLevel.Fragment;

            using (XmlWriter writer = XmlWriter.Create(sb, xws))
            {
                new XmlContentWriter(writer).WriteTopElement(name, content);
            }
            return sb.ToString();
        }

        #endregion

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
        /// Writes out a top-level element with the specified name. In addition to
        /// the usual stuff, this will write out a namespace for the element (which
        /// is required if the XML is destined for storage in a SqlServer database).
        /// The namespace used is obtained through the static <see cref="TargetNamespace"/>
        /// property.
        /// </summary>
        /// <param name="content">The content to write (not null)</param>
        /// <param name="name">The name for the XML element</param>
        /// <exception cref="ArgumentNullException">If the supplied content is null</exception>
        public void WriteTopElement(string name, IXmlContent content)
        {
            if (content==null)
                throw new ArgumentNullException();

            // Should encoding be written too?
            m_Writer.WriteProcessingInstruction("xml", "version=\"1.0\"");

            // The initial element needs a namespace (for SqlServer)
            m_Writer.WriteStartElement(name, s_TargetNamespace);

            // Write declaration that allows specification of class type (if you don't do
            // this, an attempt to write out "xsi:type" will only give you "type").
            m_Writer.WriteAttributeString("xmlns", "xsi", null, XmlSchema.InstanceNamespace);

            WriteElementContent(content);
        }

        /// <summary>
        /// Writes out an element with the specified name
        /// </summary>
        /// <param name="content">The content to write</param>
        /// <param name="name">The name for the XML element. If a null value
        /// is supplied, nothing will be written.</param>
        public void WriteElement(string name, IXmlContent content)
        {
            if (content==null)
                m_Writer.WriteElementString(name, null);
            else
            {
                m_Writer.WriteStartElement(name);
                WriteElementContent(content);
            }
        }

        void WriteElementContent(IXmlContent content)
        {
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
        /// Writes out an internal ID
        /// </summary>
        /// <param name="name">The local name of the attribute</param>
        /// <param name="value">The value of the attribute</param>
        internal void WriteId(string name, InternalIdValue value)
        {
            m_Writer.WriteAttributeString(name, value.ToString());
        }

        /// <summary>
        /// Writes out a reference to a previously existing spatial feature
        /// </summary>
        /// <param name="name">The local name of the attribute</param>
        /// <param name="feature">The feature that's referenced</param>
        internal void WriteFeatureReference(string name, Feature feature)
        {
            if (feature!=null)
                m_Writer.WriteAttributeString(name, feature.DataId);
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
        /// Writes out the supplied <c>Double</c> as an attribute
        /// </summary>
        /// <param name="name">The local name of the attribute</param>
        /// <param name="value">The value of the attribute</param>
        public void WriteDouble(string name, double value)
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
            WriteUnsignedInt("Capacity", (uint)data.Length);

            foreach (IXmlContent xc in data)
                WriteElement(itemName, xc);

            m_Writer.WriteEndElement();
        }

        /// <summary>
        /// Writes an array element for the supplied content
        /// </summary>
        /// <param name="arrayName">The name for the element representing the complete array</param>
        /// <param name="itemName">The element name for individual elements in the array</param>
        /// <param name="data">The content to write out</param>
        public void WriteArray(string arrayName, string itemName, string[] data)
        {
            m_Writer.WriteStartElement(arrayName);
            WriteUnsignedInt("Capacity", (uint)data.Length);

            foreach (string s in data)
                m_Writer.WriteElementString(itemName, s);

            m_Writer.WriteEndElement();
        }

        /// <summary>
        /// Writes an array element for the supplied content
        /// </summary>
        /// <param name="arrayName">The name for the element representing the complete array</param>
        /// <param name="itemName">The element name for individual elements in the array</param>
        /// <param name="data">The content to write out</param>
        internal void WriteArray(string arrayName, string itemName, InternalIdValue[] data)
        {
            m_Writer.WriteStartElement(arrayName);
            WriteUnsignedInt("Capacity", (uint)data.Length);

            foreach (InternalIdValue id in data)
                m_Writer.WriteElementString(itemName, id.ToString());

            m_Writer.WriteEndElement();
        }
    }
}
