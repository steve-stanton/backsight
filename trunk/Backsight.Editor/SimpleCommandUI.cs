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

using Backsight.Forms;
using Backsight.Editor.Forms;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="10-SEP-2007" />
    /// <summary>
    /// Implementation of a simple user interface. This provides do-nothing implementations
    /// for all abstract methods defined in the <see cref="CommandUI"/> base class, with the
    /// exception of the <see cref="CommandUI.Run"/> method.
    /// <para/>
    /// The advantage of extending from this class is that you don't need to clutter up the
    /// derived class with any implementations that do nothing. However, you should take
    /// special care to review the do-nothing overrides defined here, to ensure that your
    /// derived class really does implement those methods that it needs to.
    /// </summary>
    abstract class SimpleCommandUI : CommandUI
    {
        #region Class data

        // none

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>SimpleCommandUI</c> that has no user dialog.
        /// </summary>
        /// <param name="action">The action that initiated this command</param>
        protected SimpleCommandUI(IUserAction action)
            : base(null, action)
        {
        }

        /// <summary>
        /// Creates a new <c>SimpleCommandUI</c> that has a user dialog.
        /// </summary>
        /// <param name="cc">The container for any dialog controls</param>
        /// <param name="action">The action that initiated this command</param>
        protected SimpleCommandUI(IControlContainer cc, IUserAction action)
            : base(cc, action)
        {
        }

        /// <summary>
        /// Creates new <c>SimpleCommandUI</c> that isn't an update.
        /// </summary>
        /// <param name="cc">The container that may be used to display any sort of
        /// user dialog (null if no dialog is involved)</param>
        /// <param name="cmdId">The item used to invoke the command.</param>
        /// <param name="update">The object (if any) that was selected for update</param>
        /// <param name="recall">An operation that is being recalled (null if this is an update).</param>
        protected SimpleCommandUI(IControlContainer cc, IUserAction cmdId, ISpatialObject update, Operation recall)
            : base(cc, cmdId, update, recall)
        {
        }

        /// <summary>
        /// Creates new <c>SimpleCommandUI</c> for use during an editing update. This doesn't refer to
        /// the UpdateUI itself, it refers to a command that is the subject of the update.
        /// </summary>
        /// <param name="cmdId">The item used to invoke the command.</param>
        /// <param name="updcmd">The update command (not null) that is controlling this command.</param>
        protected SimpleCommandUI(IControlContainer cc, IUserAction cmdId, UpdateUI updcmd)
            : base(cc, cmdId, updcmd)
        {
        }

        #endregion

        internal override void Paint(PointFeature point)
        {
        }

        internal override void MouseMove(IPosition p)
        {
        }

        internal override void LButtonDown(IPosition p)
        {
        }

        internal override void LButtonUp(IPosition p)
        {
        }

        internal override void LButtonDblClick(IPosition p)
        {
        }

        internal override bool RButtonDown(IPosition p)
        {
            return false;
        }

        /// <summary>
        /// Aborts this command by calling <see cref="CommandUI.AbortCommand"/>.
        /// </summary>
        /// <param name="wnd">The currently active control (not used)</param>
        internal override void DialAbort(Control wnd)
        {
            AbortCommand();
        }

        internal override bool DialFinish(Control wnd)
        {
            return false;
        }

        internal override void OnSelectPoint(PointFeature point)
        {
        }

        internal override void OnSelectLine(LineFeature line)
        {
        }

        internal override bool Dispatch(int id)
        {
            return false;
        }
    }
}
