// <remarks>
// Copyright 2009 - Steve Stanton. This file is part of Backsight
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
using System.Text;

namespace Backsight.Editor
{
    class ContentWriter
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
        internal static string TargetNamespace
        {
            get { return s_TargetNamespace; }
            set { s_TargetNamespace = (value==null ? String.Empty : value); }
        }

        /// <summary>
        /// Converts an editing operation into XML
        /// </summary>
        /// <param name="indent">Should the XML be indented or not?</param>
        /// <param name="edit">The edit to convert into XML</param>
        /// <returns>The XML that corresponds to the supplied edit</returns>
        /*
        internal static string GetXml(bool indent, Edit edit)
        {
            StringBuilder sb = new StringBuilder(1000);
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.ConformanceLevel = ConformanceLevel.Fragment;
            xws.Indent = indent;

            using (XmlWriter writer = XmlWriter.Create(sb, xws))
            {
                new ContentWriter(writer, edit).Write();
            }

            return sb.ToString();
        }
        */

        #endregion

        #region Class data

        /// <summary>
        /// The object that actually does the writing.
        /// </summary>
        readonly XmlWriter m_Writer;

        /// <summary>
        /// The edit that is currently being written
        /// </summary>
        //readonly Edit m_Edit;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>ContentWriter</c> that wraps the supplied
        /// writing tool.
        /// </summary>
        /// <param name="writer">The object that actually does the writing.</param>
        ContentWriter(XmlWriter writer)
        {
            m_Writer = writer;
        }

        #endregion

        /*
        void Write(EditingActionId edit)
        {
            // The edit is enclosed by a top-level element that has no attributes - the only reason for
            // this is that the top-level element needs to also specify the XmlSchema.InstanceNamespace.

            m_Writer.WriteProcessingInstruction("xml", "version=\"1.0\"");
            m_Writer.WriteStartElement("Data", "Backsight");
            m_Writer.WriteAttributeString("xmlns", "xsi", null, System.Xml.Schema.XmlSchema.InstanceNamespace);

            WriteElement("Edit", edit);

            m_Writer.WriteEndElement();
        }
        */
        internal void WriteElement(string name, IContentElement ce)
        {
            m_Writer.WriteStartElement(name, "Backsight");
            m_Writer.WriteAttributeString("xsi", "type", null, ce.GetType().Name);
            ce.WriteAttributes(this);
            ce.WriteChildElements(this);
            m_Writer.WriteEndElement();
        }

        /// <summary>
        /// Writes out the supplied <c>Int32</c> as an attribute
        /// </summary>
        /// <param name="name">The local name of the attribute</param>
        /// <param name="value">The value of the attribute</param>
        internal void WriteInt(string name, int value)
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
        internal void WriteUnsignedInt(string name, uint value)
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
        internal void WriteLong(string name, long value)
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
        internal void WriteString(string name, string value)
        {
            m_Writer.WriteAttributeString(name, value);
        }

        /// <summary>
        /// Writes an array element for the supplied content
        /// </summary>
        /// <param name="itemName">The element name for individual elements in the array</param>
        /// <param name="data">The content to write out</param>
        internal void WriteStringArray(string itemName, string[] data)
        {
            m_Writer.WriteStartElement(itemName+"Array");
            WriteUnsignedInt("Length", (uint)data.Length);

            foreach (string s in data)
                m_Writer.WriteElementString(itemName, s);

            m_Writer.WriteEndElement();
        }

        /// <summary>
        /// Writes an array element for the supplied content
        /// </summary>
        /// <param name="itemName">The element name for individual elements in the array</param>
        /// <param name="items">The content to write out</param>
        internal void WriteArray(string itemName, IContentAttribute[] items)
        {
            m_Writer.WriteStartElement(itemName+"Array");
            WriteUnsignedInt("Length", (uint)items.Length);

            foreach (IContentAttribute item in items)
                m_Writer.WriteElementString(itemName, item.AttributeString);

            m_Writer.WriteEndElement();
        }

        /*
        internal void WriteIdArray(string itemName, Id[] items)
        {
            WriteArray(itemName, items);
        }
        */

        /// <summary>
        /// Writes out a reference to a previously existing spatial feature
        /// (as an attribute)
        /// </summary>
        /// <param name="name">The local name of the attribute</param>
        /// <param name="feature">The feature that's referenced</param>
        internal void WriteFeatureReference(string name, Feature feature)
        {
            if (feature != null)
                m_Writer.WriteAttributeString(name, feature.DataId);
        }
    }
}
