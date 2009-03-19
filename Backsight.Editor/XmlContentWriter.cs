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
using System.Xml;
using System.Text;
using System.Xml.Schema;
using System.Collections.Generic;

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
        /// Converts an editing operation into XML
        /// </summary>
        /// <param name="name">The name to assign to main content element</param>
        /// <param name="indent">Should the XML be indented or not?</param>
        /// <param name="edit">The edit to convert into XML</param>
        /// <returns>The XML that corresponds to the supplied edit</returns>
        internal static string GetXml(string name, bool indent, Operation edit)
        {
            StringBuilder sb = new StringBuilder(1000);
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.ConformanceLevel = ConformanceLevel.Fragment;
            xws.Indent = indent;

            using (XmlWriter writer = XmlWriter.Create(sb, xws))
            {
                new XmlContentWriter(writer).WriteEdit(edit);
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
        /// Creates a new <c>XmlContentWriter</c> that wraps the supplied writing tool.
        /// </summary>
        /// <param name="writer">The object that actually does the writing.</param>
        XmlContentWriter(XmlWriter writer)
        {
            m_Writer = writer;
        }

        #endregion

        /// <summary>
        /// Writes out a top-level element with the specified name. In addition to
        /// the usual stuff, this will write out a namespace for the element (which
        /// is required if the XML is destined for storage in a SqlServer database).
        /// </summary>
        /// <param name="edit">The content to write (not null)</param>
        /// <exception cref="ArgumentNullException">If the supplied content is null</exception>
        void WriteEdit(Operation edit)
        {
            if (edit==null)
                throw new ArgumentNullException();

            // The edit is enclosed by a top-level element that has no attributes - the only reason for
            // this is that the top-level element needs to also specify the XmlSchema.InstanceNamespace.

            // Should encoding be written too?
            m_Writer.WriteProcessingInstruction("xml", "version=\"1.0\"");
            m_Writer.WriteStartElement("Edit", "Backsight");
            m_Writer.WriteAttributeString("xmlns", "xsi", null, XmlSchema.InstanceNamespace);

            WriteElement("Operation", edit);

            m_Writer.WriteEndElement();
        }

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
        //void WriteTopElement(string name, IXmlContent content)
        //{
        //    if (content==null)
        //        throw new ArgumentNullException();

        //    // Should encoding be written too?
        //    m_Writer.WriteProcessingInstruction("xml", "version=\"1.0\"");

        //    // The initial element needs a namespace (for SqlServer)
        //    m_Writer.WriteStartElement(name, "Backsight");

        //    // Write declaration that allows specification of class type (if you don't do
        //    // this, an attempt to write out "xsi:type" will only give you "type").
        //    m_Writer.WriteAttributeString("xmlns", "xsi", null, XmlSchema.InstanceNamespace);

        //    WriteElementContent(content);
        //}

        /// <summary>
        /// Writes out an element with the specified name
        /// </summary>
        /// <param name="name">The name for the XML element.</param>
        /// <param name="content">The content to write. If a null value is supplied, an empty
        /// element will be written.</param>
        public void WriteElement(string name, IXmlContent content)
        {
            if (content==null)
                m_Writer.WriteElementString(name, null);
            else
            {
                m_Writer.WriteStartElement(name);

                if (content is IXmlAlternateContent)
                {
                    IXmlAlternateContent alt = (content as IXmlAlternateContent);
                    WriteElementContent(alt.GetAlternate());
                }
                else
                {
                    WriteElementContent(content);
                }
            }
        }

        void WriteElementContent(IXmlContent content)
        {
            string typeName = content.XmlTypeName;
            if (!String.IsNullOrEmpty(typeName))
                m_Writer.WriteAttributeString("xsi", "type", null, typeName);

            content.WriteAttributes(this);
            content.WriteChildElements(this);
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
        /// Writes out a reference to a previously existing spatial feature (as an attribute)
        /// </summary>
        /// <param name="name">The local name of the attribute</param>
        /// <param name="feature">The feature that's referenced</param>
        internal void WriteFeatureReference(string name, Feature feature)
        {
            if (feature!=null)
                m_Writer.WriteAttributeString(name, feature.DataId);
        }

        /// <summary>
        /// Writes out a reference to a previously existing spatial feature (as an attribute)
        /// </summary>
        /// <param name="name">The local name of the attribute</param>
        /// <param name="feature">The feature that's referenced</param>
        internal void WriteFeatureReferenceAsElement(string name, Feature feature)
        {
            m_Writer.WriteStartElement(name);
            m_Writer.WriteString(feature.DataId);
            m_Writer.WriteEndElement();
        }

        /// <summary>
        /// Writes out the supplied <c>Boolean</c> as an attribute
        /// </summary>
        /// <param name="name">The local name of the attribute</param>
        /// <param name="value">The value of the attribute</param>
        public void WriteBool(string name, bool value)
        {
            // There's no need to write out anything if the value is
            // false, since that's what you'll get back if the attribute
            // is undefined. Assumes that all boolean attributes are
            // defined as optional.

            //if (value == true)
            //{
                m_Writer.WriteStartAttribute(name);
                m_Writer.WriteValue(value);
                m_Writer.WriteEndAttribute();
            //}
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
        /// <param name="itemName">The element name for individual elements in the array</param>
        /// <param name="data">The content to write out</param>
        public void WriteArray(string itemName, IXmlContent[] data)
        {
            foreach (IXmlContent xc in data)
                WriteElement(itemName, xc);
        }

        /// <summary>
        /// Writes an array element for the supplied content
        /// </summary>
        /// <param name="itemName">The element name for individual elements in the array</param>
        /// <param name="data">The content to write out</param>
        internal void WriteFeatureReferenceArray(string itemName, Feature[] data)
        {
            foreach(Feature f in data)
                m_Writer.WriteElementString(itemName, f.DataId);
        }

        /// <summary>
        /// Writes an array of <see cref="FeatureData"/> objects constructed from
        /// an array of spatial features.
        /// </summary>
        /// <param name="arrayName">The name for the element representing the complete array</param>
        /// <param name="itemName">The element name for individual elements in the array</param>
        /// <param name="data">The features that will be written as an array of <c>FeatureData</c></param>
        internal void WriteFeatureDataArray(string itemName, Feature[] data)
        {
            foreach (Feature f in data)
                WriteElement(itemName, new FeatureData(f));
        }

        /// <summary>
        /// Writes out information for a point feature with geometry that will be
        /// re-calculated on deserialization.
        /// </summary>
        /// <param name="elementName">The name of the element</param>
        /// <param name="point">The point to write out</param>
        internal void WriteCalculatedPoint(string elementName, PointFeature point)
        {
            WriteElement(elementName, new CalculatedPoint(point));
        }

        /// <summary>
        /// Writes out the supplied <c>Double</c> as an angle, expressed
        /// in short D-M-S format (abbreviated where possible).
        /// </summary>
        /// <param name="name">The local name of the attribute</param>
        /// <param name="value">The value of the angle, in radians</param>
        public void WriteAngle(string name, double value)
        {
            WriteString(name, RadianValue.AsShortString(value));
        }
    }
}
