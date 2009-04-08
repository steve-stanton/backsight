// <remarks>
// Copyright 2007 - Steve Stanton. This file is part of Backsight
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
using System.Collections.Generic;

namespace Backsight.Forms
{
	/// <written by="Steve Stanton" on="14-SEP-2006" />
    /// <summary>
    /// A list of user actions (association with a <c>List</c> of <c>IUserAction</c>)
    /// </summary>
    public class UserActionList
    {
        #region Class data

        /// <summary>
        /// The items in the list
        /// </summary>
        private readonly List<IUserAction> m_Items;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>UserActionList</c> with nothing in it.
        /// </summary>
        public UserActionList()
        {
            m_Items = new List<IUserAction>();
        }

        #endregion

        /// <summary>
        /// Adds the supplied action to this list.
        /// </summary>
        /// <param name="action">The action to append (not null).</param>
        /// <exception cref="ArgumentNullException">If a null action was supplied</exception>
        public void Add(IUserAction action)
        {
            if (action==null)
                throw new ArgumentNullException();

            m_Items.Add(action);
        }

        /// <summary>
        /// Ensures all actions have the right UI state (meaning they're enabled if the user
        /// is currently allowed to initiate them).
        /// </summary>
        public void Update()
        {
            foreach (IUserAction ua in m_Items)
                ua.Update();
        }

        /// <summary>
        /// The actions that have been added to this list
        /// </summary>
        public IUserAction[] Actions
        {
            get { return m_Items.ToArray(); }
        }
    }
}
