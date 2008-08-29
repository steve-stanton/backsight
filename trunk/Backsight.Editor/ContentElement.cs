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
using System.Xml;

namespace Backsight.Editor
{
    /// <summary>
    /// An XML element
    /// </summary>
    public class ContentElement
    {
        #region Class data

        /// <summary>
        /// The name of the element
        /// </summary>
        string m_Name;

        /// <summary>
        /// The type of element (a class name). Null if the element represents
        /// an array.
        /// </summary>
        string m_Type;

        /// <summary>
        /// The attributes of this element. The key is the attribute name,
        /// the value is the er... value.
        /// </summary>
        Dictionary<string, IConvertible> m_Attributes;

        /// <summary>
        /// Any child elements
        /// </summary>
        List<ContentElement> m_ChildNodes;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>ContentElement</c> with no attributes and
        /// no child nodes.
        /// </summary>
        /// <param name="name">The name of the element</param>
        /// <param name="t">The type of data the element represents (null if this
        /// element represents an array header)</param>
        internal ContentElement(string name, Type t)
        {
            m_Name = name;
            m_Type = (t==null ? null : t.Name);
            m_Attributes = new Dictionary<string, IConvertible>();
            m_ChildNodes = null;
        }

        #endregion

        /// <summary>
        /// Adds an element that is contained by this one.
        /// </summary>
        /// <typeparam name="T">The data type of the child</typeparam>
        /// <param name="name">The name of the child</param>
        /// <param name="element"></param>
        internal void AddChild<T>(string name, T data) where T : IXmlContent
        {
            ContentElement element = data.GetContent(name);
            AddChild(element);
        }

        /// <summary>
        /// Adds a child element to this one
        /// </summary>
        /// <param name="element">The content to append</param>
        void AddChild(ContentElement element)
        {
            if (m_ChildNodes == null)
                m_ChildNodes = new List<ContentElement>(1);

            m_ChildNodes.Add(element);
        }

        /// <summary>
        /// Adds an array of elements as a child of this one
        /// </summary>
        /// <typeparam name="T">The data type</typeparam>
        /// <param name="arrayName"></param>
        /// <param name="itemName"></param>
        /// <param name="array"></param>
        internal void AddChildArray<T>(string arrayName, string itemName, T[] data) where T : IXmlContent
        {
            ContentElement array = new ContentElement(arrayName, null);

            foreach (T item in data)
                array.AddChild(itemName, item);

            AddChild(array);
        }

        /// <summary>
        /// Adds an attribute to this element
        /// </summary>
        /// <typeparam name="T">The data type of the attribute</typeparam>
        /// <param name="name">The name of the attribute</param>
        /// <param name="value">The value of the attribute</param>
        /// <exception cref="ArgumentException">If the specified attribute
        /// has already been recorded for this element</exception>
        internal void AddAttribute<T>(string name, T value) where T : IConvertible
        {
            //m_Attributes.Add(name, value.ToString());
            m_Attributes.Add(name, value);
        }

        /// <summary>
        /// Gets a specific attribute of this element
        /// </summary>
        /// <typeparam name="T">The data type of the attribute</typeparam>
        /// <param name="name">The name of the attribute</param>
        /// <returns>The value of the attribute (if the attribute is not present, you
        /// get the <c>default</c> value for the specified data type).</returns>
        internal T GetAttribute<T>(string name) where T : IConvertible
        {
            IConvertible c;
            if (m_Attributes.TryGetValue(name, out c))
                return (T)c; //return (T)Convert.ChangeType(c, typeof(T));
            else
                return default(T);
        }

        /// <summary>
        /// Checks whether a specific attribute is present.
        /// </summary>
        /// <param name="name">The name of the attribute to look for</param>
        /// <returns>True if the attribute is recorded for this element, false if not present</returns>
        internal bool HasAttribute(string name)
        {
            return m_Attributes.ContainsKey(name);
        }
    }
}
