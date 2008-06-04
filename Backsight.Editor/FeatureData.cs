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
    /// Data class for a feature, excluding the geometry (you use this class
    /// if the geometry will be calculated on-the-fly).
    /// </summary>
    class FeatureData : IXmlContent
    {
        #region Class data

        /// <summary>
        /// The 1-based creation sequence of the feature within the creating edit. A value
        /// of 0 means the sequence still needs to be defined.
        /// </summary>
        uint m_CreationSequence;

        /// <summary>
        /// The ID of the entity type for the feature
        /// </summary>
        int m_EntityId;

        /// <summary>
        /// Any user-defined key for the feature (may be null)
        /// </summary>
        string m_Key;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor (for serialization)
        /// </summary>
        public FeatureData()
        {
            m_CreationSequence = 0;
            m_EntityId = 0;
            m_Key = null;
        }

        /// <summary>
        /// Creates a new <c>FeatureData</c> that's initialized with the information
        /// in the supplied feature.
        /// </summary>
        /// <param name="f">The feature of interest</param>
        internal FeatureData(Feature f)
        {
            if (f==null)
            {
                m_CreationSequence = 0;
                m_EntityId = 0;
                m_Key = null;
            }
            else
            {
                m_CreationSequence = f.CreatorSequence;
                m_EntityId = f.EntityType.Id;
                m_Key = f.FormattedKey;
            }
        }

        #endregion

        #region IXmlContent Members

        /// <summary>
        /// Writes the content of this class. This is called by
        /// <see cref="XmlContentWriter.WriteElement"/>
        /// after the element name and class type (xsi:type) have been written.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public void WriteContent(XmlContentWriter writer)
        {
            // Don't write out any attributes if we had a null Feature
            if (m_CreationSequence>0)
            {
                writer.WriteUnsignedInt("CreationSequence", m_CreationSequence);
                writer.WriteInt("EntityId", m_EntityId);
                writer.WriteString("Key", m_Key);
            }
        }

        public void ReadContent(XmlContentReader reader)
        {
            if (reader.HasAttribute("CreationSequence"))
            {
                m_CreationSequence = reader.ReadUnsignedInt("CreationSequence");
                m_EntityId = reader.ReadInt("EntityId");
                m_Key = reader.ReadString("Key");
            }
        }

        #endregion
    }
}
