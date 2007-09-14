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
    partial class MapControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.hScrollBar = new System.Windows.Forms.HScrollBar();
            this.vScrollBar = new System.Windows.Forms.VScrollBar();
            this.mapPanel = new System.Windows.Forms.Panel();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.Controls.Add(this.hScrollBar, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.vScrollBar, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.mapPanel, 0, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(291, 295);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // hScrollBar
            // 
            this.hScrollBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hScrollBar.Location = new System.Drawing.Point(0, 275);
            this.hScrollBar.Name = "hScrollBar";
            this.hScrollBar.Size = new System.Drawing.Size(271, 20);
            this.hScrollBar.TabIndex = 0;
            this.hScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar_Scroll);
            // 
            // vScrollBar
            // 
            this.vScrollBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vScrollBar.Location = new System.Drawing.Point(271, 0);
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.Size = new System.Drawing.Size(20, 275);
            this.vScrollBar.TabIndex = 1;
            this.vScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar_Scroll);
            // 
            // mapPanel
            // 
            this.mapPanel.BackColor = System.Drawing.Color.AntiqueWhite;
            this.mapPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapPanel.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.mapPanel.Location = new System.Drawing.Point(0, 0);
            this.mapPanel.Margin = new System.Windows.Forms.Padding(0);
            this.mapPanel.Name = "mapPanel";
            this.mapPanel.Size = new System.Drawing.Size(271, 275);
            this.mapPanel.TabIndex = 2;
            this.mapPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mapPanel_MouseDown);
            this.mapPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mapPanel_MouseMove);
            this.mapPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.mapPanel_Paint);
            this.mapPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.mapPanel_MouseUp);
            // 
            // MapControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "MapControl";
            this.Size = new System.Drawing.Size(291, 295);
            this.VisibleChanged += new System.EventHandler(this.MapControl_VisibleChanged);
            this.Load += new System.EventHandler(this.MapControl_Load);
            this.Leave += new System.EventHandler(this.MapControl_Leave);
            this.Resize += new System.EventHandler(this.MapControl_Resize);
            this.tableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.HScrollBar hScrollBar;
        private System.Windows.Forms.VScrollBar vScrollBar;
        private System.Windows.Forms.Panel mapPanel;


    }
}
