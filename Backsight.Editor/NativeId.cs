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
    class NativeId : FeatureId
    {
        #region Static

        /// <summary>
        /// Gets the check digit for a numeric key. 
        /// </summary>
        /// <param name="num">The numeric key</param>
        /// <returns></returns>
        internal static uint GetCheckDigit(uint num)
        {
            uint val = num;
            uint total;			// The total for one iteration

            for (; val>9; val=total)
            {
                for (total=0; val!=0; val /= 10)
                {
                    total += (val % 10);
                }
            }

            return val;
        }

        #endregion

        #region Class data

        /// <summary>
        /// The associated ID group
        /// </summary>
        readonly IdGroup m_Group;

        /// <summary>
        /// The undecorated ID value (excluding any prefix or suffix or check digit).
        /// </summary>
        readonly uint m_Key;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeId"/> class.
        /// </summary>
        /// <param name="key">The raw ID value that identifies a feature</param>
        internal NativeId(IdGroup group, uint key)
        {
            m_Group = group;
            m_Key = key;
        }

        #endregion

        /// <summary>
        /// The user-perceived ID value
        /// </summary>
        internal override string FormattedKey
        {
            get { return m_Group.FormatId(m_Key); }
        }

        /// <summary>
        /// The undecorated ID value (excluding any prefix or suffix or check digit).
        /// </summary>
        internal uint RawId
        {
            get { return m_Key; }
        }
    }
}
