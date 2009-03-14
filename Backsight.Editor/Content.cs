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

namespace Backsight.Editor
{
    /// <summary>
    /// Something that will be serialized to the database
    /// </summary>
    abstract public class Content : IXmlContent
    {
        /// <summary>
        /// Obtains the object that should be used to serialize the content
        /// when it is being written to the database.
        /// </summary>
        /// <returns>The object that should be used to serialize the content</returns>
        internal virtual IXmlContent GetXmlContent()
        {
            return this;
        }

        /// <summary>
        /// The string that will be used as the xsi:type for this content (may
        /// be blank if an xsi:type is not needed). This implementation just returns
        /// the class name of this instance, concatenated with "Type".
        /// <para/>
        /// Derived classes may override. It is quite possible that a derived class
        /// will return various values, depending on the exact nature of the derived
        /// content.
        /// </summary>
        /// <remarks>Implements IXmlContent</remarks>
        public virtual string XmlTypeName
        {
            get { return GetType().Name + "Type"; }
        }

        /// <summary>
        /// Performs the reverse of <see cref="GetXmlContent"/> by obtaining
        /// an instance of the content object that corresponds to this serializable content.
        /// </summary>
        /// <returns>An object that corresponds to the original content prior
        /// to serialization.</returns>
        /// <remarks>Implements IXmlContent</remarks>
        public virtual Content GetContent()
        {
            return this;
        }

        /// <summary>
        /// Writes the attributes of this class.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        /// <remarks>Implements IXmlContent</remarks>
        public virtual void WriteAttributes(XmlContentWriter writer)
        {
        }

        /// <summary>
        /// Writes any child elements of this class. This will be called after
        /// all attributes have been written via <see cref="WriteAttributes"/>.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        /// <remarks>Implements IXmlContent</remarks>
        public virtual void WriteChildElements(XmlContentWriter writer)
        {
        }

        /// <summary>
        /// Defines the attributes of this content
        /// </summary>
        /// <param name="reader">The reading tool</param>
        /// <remarks>Implements IXmlContent</remarks>
        public virtual void ReadAttributes(XmlContentReader reader)
        {
        }

        /// <summary>
        /// Defines any child content related to this instance. This will be called after
        /// all attributes have been defined via <see cref="ReadAttributes"/>.
        /// </summary>
        /// <param name="reader">The reading tool</param>
        /// <remarks>Implements IXmlContent</remarks>
        public virtual void ReadChildElements(XmlContentReader reader)
        {
        }

    }
}
