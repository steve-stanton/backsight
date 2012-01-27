// <remarks>
// Copyright 2011 - Steve Stanton. This file is part of Backsight
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
    [Obsolete("Use plain username")]
    class User
    {
        #region Class data

        /// <summary>
        /// The login name of the user.
        /// </summary>
        readonly string m_LoginName;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        internal User(string loginName)
        {
            m_LoginName = loginName;
        }

        #endregion

        /// <summary>
        /// The internal ID for the user
        /// </summary>
        public uint UserId
        {
            get { return 0; }
        }

        /// <summary>
        /// The user-perceived name for the user
        /// </summary>
        public string Name
        {
            get { return m_LoginName; }
        }

        /// <summary>
        /// Provides a string for representing this object
        /// </summary>
        /// <returns>The <see cref="Name"/> property</returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
