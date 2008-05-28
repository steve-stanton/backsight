/// <remarks>
/// Copyright 2007 - Steve Stanton. This file is part of Backsight
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

using Backsight.Geometry;


namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="" />
    /// <summary>
    /// 
    /// </summary>
    class MoveTextOperation : Operation
    {
        #region Class data

        /// <summary>
        /// The feature that was moved.
        /// </summary>
        TextFeature m_Text;

        /// <summary>
        /// Where the text used to be.
        /// </summary>
        PointGeometry m_OldPosition;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// Creates a new <c>MoveTextOperation</c>
        /// </summary>
        internal MoveTextOperation(TextFeature text, PointGeometry oldPosition)
        {
            m_Text = text;
            m_OldPosition = oldPosition;
        }

        #endregion

        /// <summary>
        /// The feature that was moved.
        /// </summary>
        internal TextFeature MovedText
        {
            get { return m_Text; }
        }

        /// <summary>
        /// Where the text used to be.
        /// </summary>
        internal IPointGeometry OldPosition
        {
            get { return m_OldPosition; }
        }

        public override string Name
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        internal override Distance GetDistance(LineFeature line)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal override Feature[] Features
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        internal override EditingActionId EditId
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        internal override bool Undo()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal override bool Rollforward()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void AddReferences()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Writes the content of this class. This is called by
        /// <see cref="XmlContentWriter.WriteElement"/>
        /// after the element name and class type (xsi:type) have been written.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteContent(XmlContentWriter writer)
        {
        }
    }
}
