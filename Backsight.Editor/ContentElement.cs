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
    class ContentElement
    {
        string m_Name;
        ContentElement m_Parent;
        XmlWriter m_Writer;
        List<ContentAttribute> m_Attributes;
        List<ContentElement> m_ChildNodes;

        ContentElement(string name, Type t, XmlWriter writer)
        {
            m_Parent = null;
            m_Writer = writer;
            m_Attributes = new List<ContentAttribute>();
            m_ChildNodes = null;
        }

        ContentElement(string name, ContentElement parent)
        {
            m_Parent = parent;
            m_Writer = parent.m_Writer;
            m_ChildNodes = null;
        }

        ContentElement AddChild(string name)
        {
            ContentElement result = new ContentElement(name, this);
            if (m_ChildNodes==null)
                m_ChildNodes = new List<ContentElement>(1);

            m_ChildNodes.Add(result);
            return result;
        }

        void Flush(XmlWriter writer)
        {
            // Output any attributes
            foreach (ContentAttribute a in m_Attributes)
                writer.WriteAttributeString(a.Name, a.Value);

            // Flush each child
            if (m_ChildNodes!=null)
            {
                foreach (ContentElement e in m_ChildNodes)
                {
                    e.Flush(writer);
                }
            }

        }
    }
}
