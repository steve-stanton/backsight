namespace CadastralViewer
{
    partial class MainForm
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
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileOpenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.fileExitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewAutoSelect = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuViewOverview = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewZoomIn = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewZoomOut = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewZoomRectangle = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewDrawScale = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewMagnify = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewNewCenter = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewPan = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewPrevious = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewNext = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuViewStatusBar = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.mapScaleLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.positionLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.infoLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.mapControl = new Backsight.Forms.MapControl();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxViewOverview = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxViewZoomIn = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxViewZoomOut = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxViewZoomRectangle = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxViewDrawScale = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxViewMagnify = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxViewNewCenter = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxViewPan = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxViewPrevious = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxViewNext = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuItem,
            this.viewMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(708, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileMenuItem
            // 
            this.fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileOpenMenuItem,
            this.toolStripSeparator1,
            this.fileExitMenuItem});
            this.fileMenuItem.Name = "fileMenuItem";
            this.fileMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileMenuItem.Text = "&File";
            // 
            // fileOpenMenuItem
            // 
            this.fileOpenMenuItem.Name = "fileOpenMenuItem";
            this.fileOpenMenuItem.Size = new System.Drawing.Size(109, 22);
            this.fileOpenMenuItem.Text = "&Open...";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(106, 6);
            // 
            // fileExitMenuItem
            // 
            this.fileExitMenuItem.Name = "fileExitMenuItem";
            this.fileExitMenuItem.Size = new System.Drawing.Size(109, 22);
            this.fileExitMenuItem.Text = "E&xit";
            // 
            // viewMenuItem
            // 
            this.viewMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuViewAutoSelect,
            this.toolStripSeparator2,
            this.mnuViewOverview,
            this.mnuViewZoomIn,
            this.mnuViewZoomOut,
            this.mnuViewZoomRectangle,
            this.mnuViewDrawScale,
            this.mnuViewMagnify,
            this.mnuViewNewCenter,
            this.mnuViewPan,
            this.mnuViewPrevious,
            this.mnuViewNext,
            this.toolStripSeparator3,
            this.mnuViewStatusBar});
            this.viewMenuItem.Name = "viewMenuItem";
            this.viewMenuItem.Size = new System.Drawing.Size(42, 20);
            this.viewMenuItem.Text = "&View";
            // 
            // mnuViewAutoSelect
            // 
            this.mnuViewAutoSelect.Name = "mnuViewAutoSelect";
            this.mnuViewAutoSelect.Size = new System.Drawing.Size(158, 22);
            this.mnuViewAutoSelect.Text = "&Auto-select";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(155, 6);
            // 
            // mnuViewOverview
            // 
            this.mnuViewOverview.Name = "mnuViewOverview";
            this.mnuViewOverview.Size = new System.Drawing.Size(158, 22);
            this.mnuViewOverview.Text = "Over&view";
            // 
            // mnuViewZoomIn
            // 
            this.mnuViewZoomIn.Name = "mnuViewZoomIn";
            this.mnuViewZoomIn.Size = new System.Drawing.Size(158, 22);
            this.mnuViewZoomIn.Text = "Zoom &In";
            // 
            // mnuViewZoomOut
            // 
            this.mnuViewZoomOut.Name = "mnuViewZoomOut";
            this.mnuViewZoomOut.Size = new System.Drawing.Size(158, 22);
            this.mnuViewZoomOut.Text = "Zoom &Out";
            // 
            // mnuViewZoomRectangle
            // 
            this.mnuViewZoomRectangle.Name = "mnuViewZoomRectangle";
            this.mnuViewZoomRectangle.Size = new System.Drawing.Size(158, 22);
            this.mnuViewZoomRectangle.Text = "Zoom &Rectangle";
            // 
            // mnuViewDrawScale
            // 
            this.mnuViewDrawScale.Name = "mnuViewDrawScale";
            this.mnuViewDrawScale.Size = new System.Drawing.Size(158, 22);
            this.mnuViewDrawScale.Text = "Draw Scal&e...";
            // 
            // mnuViewMagnify
            // 
            this.mnuViewMagnify.Name = "mnuViewMagnify";
            this.mnuViewMagnify.Size = new System.Drawing.Size(158, 22);
            this.mnuViewMagnify.Text = "&Magnify";
            // 
            // mnuViewNewCenter
            // 
            this.mnuViewNewCenter.Name = "mnuViewNewCenter";
            this.mnuViewNewCenter.Size = new System.Drawing.Size(158, 22);
            this.mnuViewNewCenter.Text = "New &Centre";
            // 
            // mnuViewPan
            // 
            this.mnuViewPan.Name = "mnuViewPan";
            this.mnuViewPan.Size = new System.Drawing.Size(158, 22);
            this.mnuViewPan.Text = "Pa&n";
            // 
            // mnuViewPrevious
            // 
            this.mnuViewPrevious.Name = "mnuViewPrevious";
            this.mnuViewPrevious.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Left)));
            this.mnuViewPrevious.Size = new System.Drawing.Size(158, 22);
            this.mnuViewPrevious.Text = "Previo&us";
            // 
            // mnuViewNext
            // 
            this.mnuViewNext.Name = "mnuViewNext";
            this.mnuViewNext.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Right)));
            this.mnuViewNext.Size = new System.Drawing.Size(158, 22);
            this.mnuViewNext.Text = "Ne&xt";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(155, 6);
            // 
            // mnuViewStatusBar
            // 
            this.mnuViewStatusBar.Name = "mnuViewStatusBar";
            this.mnuViewStatusBar.Size = new System.Drawing.Size(158, 22);
            this.mnuViewStatusBar.Text = "&Status Bar";
            this.mnuViewStatusBar.ToolTipText = "Show or hide the status bar";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mapScaleLabel,
            this.positionLabel,
            this.infoLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 476);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(708, 22);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "statusStrip1";
            // 
            // mapScaleLabel
            // 
            this.mapScaleLabel.Name = "mapScaleLabel";
            this.mapScaleLabel.Size = new System.Drawing.Size(22, 17);
            this.mapScaleLabel.Text = "1:1";
            // 
            // positionLabel
            // 
            this.positionLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.positionLabel.Name = "positionLabel";
            this.positionLabel.Size = new System.Drawing.Size(28, 17);
            this.positionLabel.Text = "X,Y";
            // 
            // infoLabel
            // 
            this.infoLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(160, 17);
            this.infoLabel.Text = "Click on a map feature to list info";
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 24);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.mapControl);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.propertyGrid);
            this.splitContainer.Size = new System.Drawing.Size(708, 452);
            this.splitContainer.SplitterDistance = 538;
            this.splitContainer.TabIndex = 3;
            // 
            // mapControl
            // 
            this.mapControl.AutoScroll = true;
            this.mapControl.BackColor = System.Drawing.Color.White;
            this.mapControl.Center = null;
            this.mapControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapControl.Location = new System.Drawing.Point(0, 0);
            this.mapControl.MapScale = double.NaN;
            this.mapControl.Name = "mapControl";
            this.mapControl.Size = new System.Drawing.Size(538, 452);
            this.mapControl.TabIndex = 1;
            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.HelpVisible = false;
            this.propertyGrid.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.propertyGrid.Size = new System.Drawing.Size(166, 452);
            this.propertyGrid.TabIndex = 1;
            this.propertyGrid.ToolbarVisible = false;
            this.propertyGrid.ViewBackColor = System.Drawing.SystemColors.Info;
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxViewOverview,
            this.ctxViewZoomIn,
            this.ctxViewZoomOut,
            this.ctxViewZoomRectangle,
            this.ctxViewDrawScale,
            this.ctxViewMagnify,
            this.ctxViewNewCenter,
            this.ctxViewPan,
            this.ctxViewPrevious,
            this.ctxViewNext});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.ShowImageMargin = false;
            this.contextMenu.Size = new System.Drawing.Size(129, 224);
            // 
            // ctxViewOverview
            // 
            this.ctxViewOverview.Name = "ctxViewOverview";
            this.ctxViewOverview.Size = new System.Drawing.Size(128, 22);
            this.ctxViewOverview.Text = "Overview";
            // 
            // ctxViewZoomIn
            // 
            this.ctxViewZoomIn.Name = "ctxViewZoomIn";
            this.ctxViewZoomIn.Size = new System.Drawing.Size(128, 22);
            this.ctxViewZoomIn.Text = "Zoom In";
            // 
            // ctxViewZoomOut
            // 
            this.ctxViewZoomOut.Name = "ctxViewZoomOut";
            this.ctxViewZoomOut.Size = new System.Drawing.Size(128, 22);
            this.ctxViewZoomOut.Text = "Zoom Out";
            // 
            // ctxViewZoomRectangle
            // 
            this.ctxViewZoomRectangle.Name = "ctxViewZoomRectangle";
            this.ctxViewZoomRectangle.Size = new System.Drawing.Size(128, 22);
            this.ctxViewZoomRectangle.Text = "Zoom Rectangle";
            // 
            // ctxViewDrawScale
            // 
            this.ctxViewDrawScale.Name = "ctxViewDrawScale";
            this.ctxViewDrawScale.Size = new System.Drawing.Size(128, 22);
            this.ctxViewDrawScale.Text = "Draw Scale...";
            // 
            // ctxViewMagnify
            // 
            this.ctxViewMagnify.Name = "ctxViewMagnify";
            this.ctxViewMagnify.Size = new System.Drawing.Size(128, 22);
            this.ctxViewMagnify.Text = "Magnify";
            // 
            // ctxViewNewCenter
            // 
            this.ctxViewNewCenter.Name = "ctxViewNewCenter";
            this.ctxViewNewCenter.Size = new System.Drawing.Size(128, 22);
            this.ctxViewNewCenter.Text = "New Center";
            // 
            // ctxViewPan
            // 
            this.ctxViewPan.Name = "ctxViewPan";
            this.ctxViewPan.Size = new System.Drawing.Size(128, 22);
            this.ctxViewPan.Text = "Pan";
            // 
            // ctxViewPrevious
            // 
            this.ctxViewPrevious.Name = "ctxViewPrevious";
            this.ctxViewPrevious.Size = new System.Drawing.Size(128, 22);
            this.ctxViewPrevious.Text = "Previous";
            // 
            // ctxViewNext
            // 
            this.ctxViewNext.Name = "ctxViewNext";
            this.ctxViewNext.Size = new System.Drawing.Size(128, 22);
            this.ctxViewNext.Text = "Next";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(708, 498);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.Text = "Cadastral Viewer";
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileExitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileOpenMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel mapScaleLabel;
        private System.Windows.Forms.ToolStripStatusLabel positionLabel;
        private System.Windows.Forms.ToolStripStatusLabel infoLabel;
        private System.Windows.Forms.SplitContainer splitContainer;
        private Backsight.Forms.MapControl mapControl;
        private System.Windows.Forms.ToolStripMenuItem viewMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuViewAutoSelect;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem mnuViewOverview;
        private System.Windows.Forms.ToolStripMenuItem mnuViewZoomIn;
        private System.Windows.Forms.ToolStripMenuItem mnuViewZoomOut;
        private System.Windows.Forms.ToolStripMenuItem mnuViewZoomRectangle;
        private System.Windows.Forms.ToolStripMenuItem mnuViewDrawScale;
        private System.Windows.Forms.ToolStripMenuItem mnuViewMagnify;
        private System.Windows.Forms.ToolStripMenuItem mnuViewNewCenter;
        private System.Windows.Forms.ToolStripMenuItem mnuViewPan;
        private System.Windows.Forms.ToolStripMenuItem mnuViewPrevious;
        private System.Windows.Forms.ToolStripMenuItem mnuViewNext;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem mnuViewStatusBar;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem ctxViewOverview;
        private System.Windows.Forms.ToolStripMenuItem ctxViewZoomIn;
        private System.Windows.Forms.ToolStripMenuItem ctxViewZoomOut;
        private System.Windows.Forms.ToolStripMenuItem ctxViewZoomRectangle;
        private System.Windows.Forms.ToolStripMenuItem ctxViewDrawScale;
        private System.Windows.Forms.ToolStripMenuItem ctxViewMagnify;
        private System.Windows.Forms.ToolStripMenuItem ctxViewNewCenter;
        private System.Windows.Forms.ToolStripMenuItem ctxViewPan;
        private System.Windows.Forms.ToolStripMenuItem ctxViewPrevious;
        private System.Windows.Forms.ToolStripMenuItem ctxViewNext;
    }
}

