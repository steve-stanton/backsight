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

namespace Backsight.Geometry
{
    /// <summary>
    /// Base class for geometry, providing facility for reading and writing in XML.
    /// </summary>
    abstract public class BaseGeometry
    {
        /// <summary>
        /// Writes this object to XML with the specified name, preceded by an <c>xsi:type</c>
        /// declaration that provides the element type.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        /// <param name="name">The name for the XML element</param>
        public void WriteElement(XmlWriter writer, string name)
        {
            writer.WriteStartElement(name);
            writer.WriteAttributeString("xsi", "type", null, "ced:"+GetType().Name);
            WriteContent(writer);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the content of this class. This is called by <see cref="WriteElement"/>
        /// after the class type (xsi:type) has been written, and after any attributes
        /// and elements that are part of the base <see cref="BaseGeometry"/> class.
        /// This implementation does nothing. Derived classes should override.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        abstract public void WriteContent(XmlWriter writer);
    }
}
