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
using System.Collections.Generic;

namespace Backsight.Editor
{
    /// <summary>
    /// Controls serialization of data to XML. All elements written using this
    /// class will be qualified with a type name. While this is a bit verbose,
    /// it should hopefully make it easier to evolve the XML schema over time.
    /// </summary>
    public class ContentWriter
    {
        #region Class data

        /// <summary>
        /// The edit that is currently being written
        /// </summary>
        Operation m_CurrentEdit;

        /// <summary>
        /// The elements that are being written
        /// </summary>
        readonly Stack<ContentElement> m_Elements;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>ContentWriter</c>
        /// </summary>
        /// <param name="edit">The edit to convert</param>
        internal ContentWriter()
        {
            m_CurrentEdit = null;
            m_Elements = new Stack<ContentElement>(10);
        }

        #endregion

        /// <summary>
        /// Convert an editing operation into a content object (in advance of
        /// export to XML)
        /// </summary>
        /// <param name="edit">The editing operation to convert</param>
        /// <returns>The corresponding content object</returns>
        internal ContentElement Convert(Operation edit)
        {
            if (edit == null)
                throw new ArgumentNullException();

            if (m_Elements.Count > 0)
                throw new InvalidOperationException();

            m_CurrentEdit = edit;
            return AddChild("Edit", edit);
        }

        /// <summary>
        /// Converts some sort of data into a content element
        /// </summary>
        /// <param name="name">The name to associate with the data</param>
        /// <param name="data">The data to convert</param>
        /// <returns>The content that corresponds to the supplied data</returns>
        internal ContentElement AddChild(string name, IXmlContent data)
        {
            try
            {
                ContentElement parent = this.CurrentElement;
                ContentElement result = new ContentElement(parent, name, data.GetType());
                m_Elements.Push(result);
                data.WriteContent(this);
                return result;
            }

            finally
            {
                m_Elements.Pop();
            }
        }

        /// <summary>
        /// Converts an array of data into an array of content elements. The array is a
        /// child of the current content element.
        /// </summary>
        /// <typeparam name="T">The data type</typeparam>
        /// <param name="itemName">The name to associate with the results that represent each
        /// array item. The name of the array is defined as a concatenation of the item name
        /// + 'Array'</param>
        /// <param name="data">The data to convert</param>
        /// <returns>The content that corresponds to the array</returns>
        internal ContentElement AddChildArray<T>(string itemName, T[] data) where T : IXmlContent
        {
            try
            {
                ContentElement parent = this.CurrentElement;
                ContentElement result = new ContentElement(parent, itemName+"Array", null);
                m_Elements.Push(result);

                foreach (T item in data)                    
                    AddChild(itemName, item);

                return result;
            }

            finally
            {
                m_Elements.Pop();
            }
        }

        /// <summary>
        /// The edit that is currently being written
        /// </summary>
        internal Operation CurrentEdit
        {
            get { return m_CurrentEdit; }
        }

        /// <summary>
        /// The element that is currently being loaded. This is the element that
        /// should be appended to by implementations of <see cref="IXmlContent.WriteContent"/>
        /// </summary>
        internal ContentElement CurrentElement
        {
            get
            {
                if (m_Elements.Count == 0)
                    return null;
                else
                    return m_Elements.Peek();
            }
        }
    }
}
