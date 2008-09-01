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

namespace Backsight.Editor
{
    /// <summary>
    /// Methods that must be implemented by classes that are utilized in
    /// conjunction with <see cref="XmlContentWriter"/> and
    /// <see cref="XmlContentReader"/>.
    /// </summary>
    public interface IXmlContent
    {
        /// <summary>
        /// Writes the content of this class. This is called by
        /// <see cref="XmlContentWriter.WriteElement"/>
        /// after the element name and class type (xsi:type) have been written.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        void WriteContent(XmlContentWriter writer);

        /// <summary>
        /// Loads the content of this class. This is called by
        /// <see cref="XmlContentReader.ReadElement"/>
        /// if the content object has a default constructor.
        /// <para/>
        /// In situations where a class contains readonly members (i.e. members that
        /// are not expected to change after instantiation), you must provide a
        /// constructor that accepts an <see cref="XmlContentReader"/>. This more
        /// specialized constructor will do the sort of stuff that would normally
        /// be done in an implementation of <c>ReadContent</c>. In that situation,
        /// you must obviously still implement a <c>ReadContent</c> method, but it
        /// would be a good idea to throw an exception on an attempt to load the
        /// object that way.
        /// </summary>
        /// <param name="reader">The reading tool</param>
        void ReadContent(XmlContentReader reader);

        //ContentElement GetContent(string name);
        void WriteContent(ContentWriter writer);
        void ReadContent(ContentReader reader);
    }
}
