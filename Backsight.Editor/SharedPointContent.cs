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
    /// Serializable version of a <see cref="PointFeature"/> that shares
    /// geometry with another point.
    /// </summary>
    class SharedPointContent : Content
    {
        #region Class data

        /// <summary>
        /// The content object that corresponds to this serializable instance.
        /// </summary>
        PointFeature m_Point;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor (for serialization)
        /// </summary>
        public SharedPointContent()
        {
        }

        internal SharedPointContent(PointFeature p)
        {
            m_Point = p;
        }

        #endregion

        /// <summary>
        /// Writes the attributes of this class.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        /// <remarks>Implements IXmlContent</remarks>
        public override void WriteAttributes(XmlContentWriter writer)
        {
            m_Point.WriteFeatureAttributes(writer);
            writer.WriteFeatureReference("FirstPoint", m_Point.Node.FirstPoint);
        }

        /// <summary>
        /// Defines the attributes of this content
        /// </summary>
        /// <param name="reader">The reading tool</param>
        /// <remarks>Implements IXmlContent</remarks>
        public override void ReadAttributes(XmlContentReader reader)
        {
            m_Point = new PointFeature();
            m_Point.ReadFeatureAttributes(reader);

            PointFeature p = reader.ReadFeatureByReference<PointFeature>("FirstPoint");
            m_Point.Node = p.Node;
            p.Node.AttachPoint(m_Point);
        }

        /// <summary>
        /// Performs the reverse of <see cref="IContent.GetXmlContent"/> by obtaining
        /// an instance of the content object that corresponds to this serializable content.
        /// </summary>
        /// <returns>An object that corresponds to the original content prior
        /// to serialization.</returns>
        /// <remarks>Implements IXmlContent</remarks>
        public override Content GetContent()
        {
            return m_Point;
        }
    }
}
