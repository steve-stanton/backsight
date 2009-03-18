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
using System.Reflection;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace Backsight.Editor.Xml
{
    /// <summary>
    /// Methods for validating XML content
    /// </summary>
    public static class Content
    {
        /// <summary>
        /// Validates a string representing an XML fragment that should conform to the
        /// XML schema defined by <c>Edits.xsd</c>.
        /// </summary>
        /// <param name="s">The XML to validate</param>
        /// <exception cref="XmlSchemaException">If the schema cannot be loaded from the assembly
        /// holding this class, or the supplied XML is not valid.</exception>
        public static void Validate(string s)
        {
            Validate(s, GetSchema());
        }

        /// <summary>
        /// Obtains the XML schema for Backsight content
        /// </summary>
        /// <returns>The schema defined by <c>Edits.xsd</c></returns>
        /// <exception cref="XmlSchemaException">If the schema cannot be loaded from the assembly
        /// holding this class</exception>
        static XmlSchema GetSchema()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            using (Stream fs = a.GetManifestResourceStream("Backsight.Editor.Xml.Edits.xsd"))
            {
                return XmlSchema.Read(fs, null);
            }
        }

        /// <summary>
        /// Validates a string representing an XML fragment using the supplied schema
        /// </summary>
        /// <param name="s">The XML to validate</param>
        /// <param name="schema">The schema the XML should conform to</param>
        /// <exception cref="XmlSchemaException">If the supplied XML is not valid.</exception>
        static void Validate(string s, XmlSchema schema)
        {
            XmlReaderSettings xrs = new XmlReaderSettings();
            xrs.ConformanceLevel = ConformanceLevel.Fragment;
            xrs.ValidationType = ValidationType.Schema;
            xrs.Schemas.Add(schema);

            using (StringReader sr = new StringReader(s))
            {
                XmlReader reader = XmlReader.Create(sr, xrs);
                while (reader.Read()) { }
            }
        }
    }
}
