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
using System.Windows.Forms;

namespace Backsight.Forms
{
	/// <written by="Steve Stanton" on="14-SEP-2006" />
    /// <summary>UI elements that are associated with an instance of <c>IUserAction</c></summary>
    class UserActionSupport
    {
        #region Class data

        /// <summary>
        /// The UI elements relating to the action
        /// </summary>
        private readonly ToolStripItem[] m_Items;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates new <c>UserActionSupport</c> that relates to the supplied array
        /// of UI elements.
        /// </summary>
        /// <param name="items">The items associated with an instance of <c>UserAction</c></param>
        /// <exception cref="ArgumentNullException">If the supplied array is null, empty, or any of the
        /// elements are null.</exception>
        internal UserActionSupport(ToolStripItem[] items)
        {
            if (items==null || items.Length==0)
                throw new ArgumentNullException();

            foreach (ToolStripItem item in items)
            {
                if (item==null)
                    throw new ArgumentNullException();
            }

            m_Items = items;
        }

        #endregion

        /// <summary>
        /// The text associated with the first element of the array returned by
        /// the <see cref="Items"/> property.
        /// </summary>
        internal string Title
        {
            get { return m_Items[0].Text; }
        }

        /// <summary>
        /// Does the supplied object match any of the UI elements associated with a user action.
        /// </summary>
        /// <param name="o">The user interface element that has been activated</param>
        /// <returns>True if the supplied object matches any of the elements associated with
        /// a user action</returns>
        internal bool IsMatch(object o)
        {
            foreach (ToolStripItem item in m_Items)
            {
                if (Object.ReferenceEquals(item, o))
                    return true;
            }

            return false;
        }

        internal bool Enabled
        {
            get { return m_Items[0].Enabled; }

            set
            {
                foreach (ToolStripItem item in m_Items)
                    item.Enabled = value;
            }
        }

        internal void SetHandler(EventHandler handler)
        {
            foreach (ToolStripItem item in m_Items)
                item.Click += handler;
        }
    }
}
