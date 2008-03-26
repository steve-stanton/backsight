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

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="25-MAR-2008"/>
    /// <summary>
    /// Information for defining a single span in a connection path. A span
    /// is part of a <see cref="Leg"/>.
    /// </summary>
    [Serializable]
    class SpanData
    {
        #region Class data

        /// <summary>
        /// The observed distances for the span. May be null when dealing
        /// with a cul-de-sac that was specified with center point and central angle).
        /// </summary>
        Distance m_Distance;

        /// <summary>
        /// The feature created for the span. This can
        /// be a <c>LineFeature</c>, <c>PointFeature</c>, or <c>null</c>.
        /// </summary>
        Feature m_Feature;

        /// <summary>
        /// Flag bits relating to the span.
        /// </summary>
        LegItemFlag m_Switches;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates new <c>SpanData</c> with everything set to null.
        /// </summary>
        internal SpanData()
        {
            m_Distance = null;
            m_Feature = null;
            m_Switches = LegItemFlag.Null;
        }

        #endregion

        /// <summary>
        /// The observed distances for the span. May be null when dealing
        /// with a cul-de-sac that was specified with center point and central angle).
        /// </summary>
        internal Distance ObservedDistance
        {
            get { return m_Distance; }
            set { m_Distance = value; }
        }

        /// <summary>
        /// The feature created for the span. This can
        /// be a <c>LineFeature</c>, <c>PointFeature</c>, or <c>null</c>.
        /// </summary>
        internal Feature CreatedFeature
        {
            get { return m_Feature; }
            set { m_Feature = value; }
        }

        /// <summary>
        /// Flag bits relating to the span.
        /// </summary>
        internal LegItemFlag Flags
        {
            get { return m_Switches; }
            set { m_Switches = value; }
        }
    }
}
