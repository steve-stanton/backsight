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

namespace Backsight.Forms
{
    partial class MagnifyForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.scaleLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxZoomInMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxZoomOutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxDrawHereMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ctxCancelMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.magnifyMapControl = new Backsight.Forms.MagnifyMapControl();
            this.ctxScaleMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.scaleLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 104);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(131, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // scaleLabel
            // 
            this.scaleLabel.Name = "scaleLabel";
            this.scaleLabel.Size = new System.Drawing.Size(27, 17);
            this.scaleLabel.Text = "1:1";
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxScaleMenuItem,
            this.ctxZoomInMenuItem,
            this.ctxZoomOutMenuItem,
            this.ctxDrawHereMenuItem,
            this.toolStripSeparator1,
            this.ctxCancelMenuItem});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.ShowImageMargin = false;
            this.contextMenu.Size = new System.Drawing.Size(128, 142);
            // 
            // ctxZoomInMenuItem
            // 
            this.ctxZoomInMenuItem.Name = "ctxZoomInMenuItem";
            this.ctxZoomInMenuItem.Size = new System.Drawing.Size(127, 22);
            this.ctxZoomInMenuItem.Text = "Zoom In";
            this.ctxZoomInMenuItem.Click += new System.EventHandler(this.ctxZoomInMenuItem_Click);
            // 
            // ctxZoomOutMenuItem
            // 
            this.ctxZoomOutMenuItem.Name = "ctxZoomOutMenuItem";
            this.ctxZoomOutMenuItem.Size = new System.Drawing.Size(127, 22);
            this.ctxZoomOutMenuItem.Text = "Zoom Out";
            this.ctxZoomOutMenuItem.Click += new System.EventHandler(this.ctxZoomOutMenuItem_Click);
            // 
            // ctxDrawHereMenuItem
            // 
            this.ctxDrawHereMenuItem.Name = "ctxDrawHereMenuItem";
            this.ctxDrawHereMenuItem.Size = new System.Drawing.Size(127, 22);
            this.ctxDrawHereMenuItem.Text = "Draw Here";
            this.ctxDrawHereMenuItem.Click += new System.EventHandler(this.ctxDrawHereMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(124, 6);
            // 
            // ctxCancelMenuItem
            // 
            this.ctxCancelMenuItem.Name = "ctxCancelMenuItem";
            this.ctxCancelMenuItem.Size = new System.Drawing.Size(127, 22);
            this.ctxCancelMenuItem.Text = "Cancel";
            this.ctxCancelMenuItem.Click += new System.EventHandler(this.ctxCancelMenuItem_Click);
            // 
            // magnifyMapControl
            // 
            this.magnifyMapControl.AutoScroll = true;
            this.magnifyMapControl.BackColor = System.Drawing.Color.White;
            this.magnifyMapControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.magnifyMapControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.magnifyMapControl.Location = new System.Drawing.Point(0, 0);
            this.magnifyMapControl.Margin = new System.Windows.Forms.Padding(0);
            this.magnifyMapControl.Name = "magnifyMapControl";
            this.magnifyMapControl.Size = new System.Drawing.Size(131, 126);
            this.magnifyMapControl.TabIndex = 0;
            // 
            // ctxScaleMenuItem
            // 
            this.ctxScaleMenuItem.Name = "ctxScaleMenuItem";
            this.ctxScaleMenuItem.Size = new System.Drawing.Size(127, 22);
            this.ctxScaleMenuItem.Text = "Scale...";
            this.ctxScaleMenuItem.Click += new System.EventHandler(this.ctxScaleMenuItem_Click);
            // 
            // MagnifyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(131, 126);
            this.ContextMenuStrip = this.contextMenu;
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.magnifyMapControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MagnifyForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "MagnifyForm";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.MagnifyForm_Shown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MagnifyForm_MouseMove);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MagnifyMapControl magnifyMapControl;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel scaleLabel;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem ctxZoomInMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ctxZoomOutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ctxDrawHereMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem ctxCancelMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ctxScaleMenuItem;

    }
}