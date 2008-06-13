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
    /// <written by="Steve Stanton" on="13-JUN-2008" />
    /// <summary>
    /// An ID that has been imported from some alien data source.
    /// </summary>
    class ForeignId : FeatureId
    {
        #region Class data

        /// <summary>
        /// The foreign key used to identify a feature
        /// </summary>
        readonly string m_Key;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ForeignId"/> class.
        /// </summary>
        /// <param name="key">The foreign key used to identify a feature</param>
        internal ForeignId(string key)
        {
            m_Key = key;
        }

        #endregion

        /// <summary>
        /// The user-perceived ID value
        /// </summary>
        internal override string FormattedKey
        {
            get { return m_Key; }
        }
    }
}
