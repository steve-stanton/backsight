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
using System.Windows.Forms;

namespace Backsight.Forms
{
	/// <written by="Steve Stanton" on="14-SEP-2006" />
    /// <summary>
    /// Some sort of action that the user can initiate. This provides a simple way to group
    /// a menuitem and an associated toolbar button so that their UI state (enabled or disabled)
    /// will always be presented consistently.
    /// 
    /// This is meant to provide functionality that is comparable to the technique that
    /// was formerly available in old versions of Visual Studio (where multiple UI elements
    /// could be mapped to a single command ID, which could then be associated with specific
    /// COMMAND and UPDATE_COMMAND_UI handlers). The implementation here provides a bare-bones
    /// replacement.
    /// </summary>
    public class UserAction : IUserAction
    {
        /// <summary>
        /// Method that will be called to determine whether the action is currently allowed.
        /// This will typically be called during application idle time.
        /// </summary>
        /// <returns></returns>
        public delegate bool IsActionEnabled();

        /// <summary>
        /// The method to call whenever an action is initiated by the user
        /// </summary>
        /// <param name="sender">The action being performed</param>
        public delegate void DoAction (IUserAction sender);

        #region Class data

        /// <summary>
        /// The UI elements associated with this action.
        /// </summary>
        private readonly UserActionSupport m_Elements;

        /// <summary>
        /// The delegate to call to determine whether the action is currently enabled (a null value
        /// means the action will always be enabled).
        /// </summary>
        private readonly IsActionEnabled m_IsActionEnabled;

        /// <summary>
        /// The delegate for handling the action (not null).
        /// </summary>
        private readonly DoAction m_DoAction;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a <c>UserAction</c> that is invoked by one UI item that is
        /// always enabled.
        /// </summary>
        /// <param name="item">The associated UI element (not null)</param>
        /// <param name="doAction">Delegate that should be called to perform the action (not null)</param>
        public UserAction(ToolStripItem item, DoAction doAction)
            : this(new ToolStripItem[] { item }, null, doAction)
        {
        }

        /// <summary>
        /// Creates a new <c>UserAction</c>
        /// </summary>
        /// <param name="items">The associated UI elements (at least one of them)</param>
        /// <param name="isActionEnabled">Delegate that can be used to determine if the action should be enabled (null means the
        /// action will always be enabled)</param>
        /// <param name="doAction">Delegate that should be called to perform the action (not null)</param>
        /// <exception cref="ArgumentNullException">If doAction is null, or the associated UI elements are empty
        /// or null</exception>
        public UserAction(ToolStripItem[] items, IsActionEnabled isActionEnabled, DoAction doAction)
        {
            if (doAction==null)
                throw new ArgumentNullException();

            m_Elements = new UserActionSupport(items);
            m_IsActionEnabled = isActionEnabled;
            m_DoAction = doAction;

            // Wire the supplied UI elements to a suitable handler
            m_Elements.SetHandler(Do);
        }

        #endregion

        public void Update()
        {
            m_Elements.Enabled = (m_IsActionEnabled==null ? true : m_IsActionEnabled());
        }

        private void Do(object sender, EventArgs e)
        {
            // I've never had occasion to use the EventArgs, so don't bother passing them on (keeps
            // the code a bit tidier). If you ever have a real need for them, it would probably be
            // best to modify this class so that the EventArgs could be attached to a UserAction.
            m_DoAction(this);
        }

        public string Title { get { return m_Elements.Title; } }
    }
}
