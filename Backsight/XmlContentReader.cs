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
    /// Reads back XML data that was previously created using the
    /// <see cref="XmlContentWriter"/> class.
    /// </summary>
    public class XmlContentReader : XmlBase
    {
        #region Class data

        /// <summary>
        /// The object that actually does the reading.
        /// </summary>
        readonly XmlReader m_Reader;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>XmlContentReader</c> that wraps the supplied
        /// reading tool.
        /// </summary>
        /// <param name="reader">The object that actually does the reading.</param>
        public XmlContentReader(XmlReader reader)
        {
            m_Reader = reader;
        }

        #endregion

        /// <summary>
        /// Loads the next element (if any) from the XML content
        /// </summary>
        /// <returns>The next element (null if there's nothing further)</returns>
        public IXmlContent ReadNextElement()
        {
            return null;
        }
    }
}
