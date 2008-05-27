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

namespace Backsight
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
        /// <see cref="XmlContentReader.ReadNextElement"/>
        /// after the content object has been instantiated through it's default
        /// constructor.
        /// </summary>
        /// <param name="reader">The reading tool</param>
        //void ReadContent(XmlContentReader reader);
    }
}
