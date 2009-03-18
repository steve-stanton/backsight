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
using Backsight.Environment;

namespace Backsight.Editor
{
    /// <summary>
    /// Data class for a feature, excluding the geometry (you use this class
    /// if the geometry will be calculated on-the-fly).
    /// </summary>
    class FeatureData : Content
    {
        #region Class data

        /// <summary>
        /// The 1-based creation sequence of the feature within the creating edit. A value
        /// of 0 means the sequence still needs to be defined.
        /// </summary>
        uint m_CreationSequence;

        /// <summary>
        /// The entity type for the feature
        /// </summary>
        IEntity m_Entity;

        /// <summary>
        /// Any user-defined key for the feature (may be null)
        /// </summary>
        FeatureId m_Id;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>FeatureData</c> that's initialized with the information
        /// in the supplied feature.
        /// </summary>
        /// <param name="f">The feature of interest (may be null)</param>
        internal FeatureData(Feature f)
        {
            if (f==null)
            {
                m_CreationSequence = 0;
                m_Entity = null;
                m_Id = null;
            }
            else
            {
                m_CreationSequence = f.CreatorSequence;
                m_Entity = f.EntityType;
                m_Id = f.Id;
            }
        }

        /// <summary>
        /// Creates a new <c>FeatureData</c> with the supplied parameters.
        /// </summary>
        /// <param name="creationSequence">The 1-based creation sequence of the feature within the creating edit.</param>
        /// <param name="e">The entity type for the feature</param>
        /// <param name="id">Any user-defined key for the feature (may be null)</param>
        internal FeatureData(uint creationSequence, IEntity e, FeatureId id)
        {
            m_CreationSequence = creationSequence;
            m_Entity = e;
            m_Id = id;
        }

        #endregion

        #region IXmlContent Members

        /// <summary>
        /// Writes the attributes of this class.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteAttributes(XmlContentWriter writer)
        {
            if (m_CreationSequence != 0)
            {
                writer.WriteUnsignedInt("Item", m_CreationSequence);
                writer.WriteInt("EntityId", m_Entity.Id);

                if (m_Id != null)
                    m_Id.Write("Key", writer);
            }
        }

        /// <summary>
        /// Writes any child elements of this class. This will be called after
        /// all attributes have been written via <see cref="WriteAttributes"/>.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteChildElements(XmlContentWriter writer)
        {
        }

        /// <summary>
        /// Defines the attributes of this content
        /// </summary>
        /// <param name="reader">The reading tool</param>
        public override void ReadAttributes(XmlContentReader reader)
        {
            if (reader.HasAttribute("Item"))
            {
                m_CreationSequence = reader.ReadUnsignedInt("Item");
                int entityId = reader.ReadInt("EntityId");
                m_Entity = EnvironmentContainer.FindEntityById(entityId);
                m_Id = FeatureId.Read("Key", reader);
            }
        }

        #endregion

        /// <summary>
        /// Does this represent an "empty" (null) feature?
        /// </summary>
        internal bool IsEmpty
        {
            get { return (m_CreationSequence==0); }
        }

        /// <summary>
        /// The 1-based creation sequence of the feature within the creating edit. A value
        /// of 0 means the sequence still needs to be defined.
        /// </summary>
        internal uint CreationSequence
        {
            get { return m_CreationSequence; }
        }

        /// <summary>
        /// The entity type for the feature
        /// </summary>
        internal IEntity EntityType
        {
            get { return m_Entity; }
        }

        /// <summary>
        /// Any user-defined key for the feature (may be null)
        /// </summary>
        internal FeatureId Id
        {
            get { return m_Id; }
        }
    }
}
