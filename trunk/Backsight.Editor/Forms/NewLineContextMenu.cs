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
using System.Windows.Forms;

using Backsight.Forms;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" on="05-JAN-2008" />
    /// <summary>
    /// Context menu for the <see cref="NewLineUI"/>
    /// </summary>
    class NewLineContextMenu : ContextMenuStrip
    {
        #region Class data

        private ToolStripMenuItem ctxSpecifyId;
        private ToolStripMenuItem ctxCancel;
        private ToolStripSeparator toolStripSeparator1;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>NewLineContextMenu</c>
        /// </summary>
        /// <param name="ui">The user interface displaying this context menu</param>
        internal NewLineContextMenu(NewLineUI ui)
        {
            InitializeComponent();

            new UserAction(ctxSpecifyId, ui.SpecifyId);
            new UserAction(ctxCancel, ui.Cancel);
        }

        #endregion

        private void InitializeComponent()
        {
            this.ctxSpecifyId = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxCancel = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.SuspendLayout();
            // 
            // ctxSpecifyId
            // 
            this.ctxSpecifyId.Name = "ctxSpecifyId";
            this.ctxSpecifyId.Size = new System.Drawing.Size(107, 22);
            this.ctxSpecifyId.Text = "Specify ID...";
            // 
            // ctxCancel
            // 
            this.ctxCancel.Name = "ctxCancel";
            this.ctxCancel.Size = new System.Drawing.Size(107, 22);
            this.ctxCancel.Text = "Cancel";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(104, 6);
            // 
            // NewLineContextMenu
            // 
            this.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxSpecifyId,
            this.toolStripSeparator1,
            this.ctxCancel});
            this.ShowImageMargin = false;
            this.Size = new System.Drawing.Size(108, 54);
            this.ResumeLayout(false);
        }
    }
}
