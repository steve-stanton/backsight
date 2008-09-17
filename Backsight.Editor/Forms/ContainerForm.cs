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

using Backsight.Forms;

namespace Backsight.Editor.Forms
{
	/// <written by="Steve Stanton" on="30-MAY-2007" />
    /// <summary>
    /// A form for holding some sort of dialog (user control).
    /// </summary>
    public partial class ContainerForm : Form, IActionContainer
    {
        #region Class data

        /// <summary>
        /// The action the container is required for.
        /// </summary>
        private readonly IUserAction m_Action;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a container for some sort of user action.
        /// </summary>
        /// <param name="action">The action the container is required for.</param>
        public ContainerForm(IUserAction action)
        {
            InitializeComponent();
            m_Action = action;

            // The action title may contain a mnemonic
            Text = m_Action.Title.Replace("&", String.Empty);
        }

        #endregion

        #region IControlContainer Members

        /// <summary>
        /// Modeless display of the specified control.
        /// </summary>
        /// <param name="c">The control to display.</param>
        /// <exception cref="ArgumentException">If a control has already been
        /// displayed in this container (and not cleared via a call to <c>Clear</c>)</exception>
        public void Display(Control c)
        {
            if (Controls.Count!=0)
                throw new ArgumentException("Container already contains something");

            ClientSize = c.Size;
            Controls.Add(c);
            Show();
        }

        public void Clear()
        {
            Hide();
            Controls.Clear();
        }

        #endregion

        #region IActionContainer Members

        public IUserAction Action
        {
            get { return m_Action; }
        }

        #endregion
    }
}
