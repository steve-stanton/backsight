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

using Backsight.Environment;
using System;

namespace Backsight.Editor
{
    /// <summary>
    /// A representation of <see cref="RowTextGeometry"/> that is used during
    /// database serialization.
    /// </summary>
    class RowTextContent : TextGeometry
    {
        #region Class data

        /// <summary>
        /// The Backsight ID for the database table
        /// </summary>
        int m_TableId;

        /// <summary>
        /// The ID of the formatting template.
        /// </summary>
        int m_TemplateId;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RowTextContent"/> class.
        /// </summary>
        /// <param name="tableId">The Backsight ID for the database table</param>
        /// <param name="template">The formatting template</param>
        /// <param name="row">The row that contains the information to format</param>
        /// <param name="template">How to form the text string out of the data in the row</param>
        /// <param name="pos">Position of the text's reference point (always the top left corner of the string).</param>
        /// <param name="font">The text style (defines the type-face and the height of the text).</param>
        /// <param name="height">The height of the text, in meters on the ground.</param>
        /// <param name="width">The total width of the text, in meters on the ground.</param>
        /// <param name="rotation">Clockwise rotation from horizontal</param>
        [Obsolete]
        internal RowTextContent(int tableId, ITemplate template,
                                 PointGeometry pos, IFont font, double height, double width, float rotation)
            : base(pos, font, height, width, rotation)
        {
            m_TableId = tableId;
            m_TemplateId = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RowTextContent"/> class.
        /// </summary>
        /// <param name="copy">The copy.</param>
        internal RowTextContent(RowTextGeometry copy)
            : base(copy)
        {
            m_TableId = copy.Row.Table.Id;
            m_TemplateId = copy.Template.Id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RowTextContent"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal RowTextContent(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
            m_TableId = editDeserializer.ReadInt32(DataField.Table);
            m_TemplateId = editDeserializer.ReadInt32(DataField.Template);
        }

        #endregion

        /// <summary>
        /// Override returns a null spatial extent. The extent will be defined
        /// only after instances of <c>RowTextContent</c> have been associated
        /// with database attributes as part of the deserialization logic that
        /// occurs during application startup (at that time, the geometry will be
        /// replaced with fully defined instances of <see cref="RowTextGeometry"/>).
        /// </summary>
        public override IWindow Extent
        {
            // Return something (cover situation where RowTextContent could not be completely
            // deserialized because the database is no longer accessible).
            //get { return null; }
            get { return base.Extent; }
        }

        /// <summary>
        /// The Backsight ID for the database table
        /// </summary>
        internal int TableId
        {
            get { return m_TableId; }
        }

        /// <summary>
        /// The ID of the formatting template.
        /// </summary>
        internal int TemplateId
        {
            get { return m_TemplateId; }
        }

        /// <summary>
        /// The text string represented by this geometry is "NoData" (always).
        /// Instances of <c>RowTextContent</c> should exist only for a short
        /// period during deserialization from the database (however, if database
        /// rows have been deleted unexpectedly, the content object may continue
        /// to exist).
        /// </summary>
        public override string Text
        {
            get { return "NoData"; }
        }

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public override void WriteData(EditSerializer editSerializer)
        {
            base.WriteData(editSerializer);

            editSerializer.WriteInt32(DataField.Table, m_TableId);
            editSerializer.WriteInt32(DataField.Template, m_TemplateId);
        }
    }
}
