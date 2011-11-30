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

namespace Backsight.Editor.FileStore
{
    /// <summary>
    /// A user that corresponds to anyone logged on to the localhost.
    /// </summary>
    class AnyLocalUser : IUser
    {
        #region Static

        /// <summary>
        /// The singleton instance of this class.
        /// </summary>
        static IUser s_Instance = new AnyLocalUser();

        /// <summary>
        /// The singleton instance of this class.
        /// </summary>
        internal static IUser Instance
        {
            get { return s_Instance; }
        }

        #endregion

        #region Class data

        // None

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AnyLocalUser"/> class.
        /// </summary>
        AnyLocalUser()
        {
        }

        #endregion

        /// <summary>
        /// The internal ID for the user
        /// </summary>
        public uint UserId
        {
            get { return 1; }
        }

        /// <summary>
        /// The user-perceived name for the user
        /// </summary>
        public string Name
        {
            get { return "Local user"; }
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
