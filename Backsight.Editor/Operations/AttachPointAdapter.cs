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

using Backsight.Xml;

namespace Backsight.Editor.Operations
{
    /// <summary>
    /// Implementation for <see cref="IAttachPoint"/>, for use in edit serialization.
    /// </summary>
    class AttachPointAdapter : IAttachPoint
    {
        #region Class data

        /// <summary>
        /// The edit of interest
        /// </summary>
        readonly AttachPointOperation m_Edit;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>AttachPointAdapter</c> that refers to the specified edit.
        /// </summary>
        /// <param name="edit">The edit of interest</param>
        internal AttachPointAdapter(AttachPointOperation edit)
        {
            m_Edit = edit;
        }

        #endregion

        #region IAttachPoint Members

        /// <summary>
        /// The line the point should appear on 
        /// </summary>
        public Guid Line
        {
            get
            {
                LineFeature line = m_Edit.Line;
                return Guid.NewGuid(); // for now
            }
        }

        /// <summary>
        /// The position ratio of the attached point. A point coincident with the start
        /// of the line is a value of 0. A point at the end of the line is a value of
        /// 1 billion  (1,000,000,000).
        /// </summary>
        public uint PositionRatio
        {
            get { return m_Edit.PositionRatio; }
        }

        /// <summary>
        /// The point that was created (without any geometry)
        /// </summary>
        public FeatureData Point
        {
            get
            {
                PointFeature p = m_Edit.NewPoint;
                return new FeatureData(Guid.NewGuid(), p.EntityType.Id, p.FormattedKey); // NewGuid for now
            }
        }

        #endregion
    }
}
