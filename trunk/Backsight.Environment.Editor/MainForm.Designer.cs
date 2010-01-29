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

namespace Backsight.Environment.Editor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.fileOpenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileSaveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.fileExportMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileImportMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.fileExitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editNewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editUpdateMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editDeleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.helpAboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.newToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.updateToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.deleteToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.domainsPage = new System.Windows.Forms.TabPage();
            this.entityTypesPage = new System.Windows.Forms.TabPage();
            this.fontsPage = new System.Windows.Forms.TabPage();
            this.idGroupsPage = new System.Windows.Forms.TabPage();
            this.layersPage = new System.Windows.Forms.TabPage();
            this.propertiesPage = new System.Windows.Forms.TabPage();
            this.tablesPage = new System.Windows.Forms.TabPage();
            this.templatesPage = new System.Windows.Forms.TabPage();
            this.themesPage = new System.Windows.Forms.TabPage();
            this.zonesPage = new System.Windows.Forms.TabPage();
            this.helpProvider = new System.Windows.Forms.HelpProvider();
            this.tipProvider = new System.Windows.Forms.ToolTip(this.components);
            this.menuStrip.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenu,
            this.editToolStripMenuItem,
            this.helpMenu});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.menuStrip.Size = new System.Drawing.Size(706, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileMenu
            // 
            this.fileMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileOpenMenuItem,
            this.fileSaveMenuItem,
            this.toolStripSeparator2,
            this.fileExportMenuItem,
            this.fileImportMenuItem,
            this.toolStripSeparator1,
            this.fileExitMenuItem});
            this.fileMenu.Name = "fileMenu";
            this.fileMenu.Size = new System.Drawing.Size(35, 20);
            this.fileMenu.Text = "&File";
            // 
            // fileOpenMenuItem
            // 
            this.fileOpenMenuItem.Name = "fileOpenMenuItem";
            this.fileOpenMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.fileOpenMenuItem.Size = new System.Drawing.Size(136, 22);
            this.fileOpenMenuItem.Text = "&Open";
            this.fileOpenMenuItem.Click += new System.EventHandler(this.fileOpenMenuItem_Click);
            // 
            // fileSaveMenuItem
            // 
            this.fileSaveMenuItem.Name = "fileSaveMenuItem";
            this.fileSaveMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.fileSaveMenuItem.Size = new System.Drawing.Size(136, 22);
            this.fileSaveMenuItem.Text = "&Save";
            this.fileSaveMenuItem.Click += new System.EventHandler(this.fileSaveMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(133, 6);
            // 
            // fileExportMenuItem
            // 
            this.fileExportMenuItem.Name = "fileExportMenuItem";
            this.fileExportMenuItem.Size = new System.Drawing.Size(136, 22);
            this.fileExportMenuItem.Text = "Export...";
            this.fileExportMenuItem.Click += new System.EventHandler(this.fileExportMenuItem_Click);
            // 
            // fileImportMenuItem
            // 
            this.fileImportMenuItem.Name = "fileImportMenuItem";
            this.fileImportMenuItem.Size = new System.Drawing.Size(136, 22);
            this.fileImportMenuItem.Text = "Import...";
            this.fileImportMenuItem.Click += new System.EventHandler(this.fileImportMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(133, 6);
            // 
            // fileExitMenuItem
            // 
            this.fileExitMenuItem.Name = "fileExitMenuItem";
            this.fileExitMenuItem.Size = new System.Drawing.Size(136, 22);
            this.fileExitMenuItem.Text = "E&xit";
            this.fileExitMenuItem.Click += new System.EventHandler(this.fileExitMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editNewMenuItem,
            this.editUpdateMenuItem,
            this.editDeleteMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // editNewMenuItem
            // 
            this.editNewMenuItem.Name = "editNewMenuItem";
            this.editNewMenuItem.Size = new System.Drawing.Size(118, 22);
            this.editNewMenuItem.Text = "&New...";
            this.editNewMenuItem.Click += new System.EventHandler(this.newButton_Click);
            // 
            // editUpdateMenuItem
            // 
            this.editUpdateMenuItem.Name = "editUpdateMenuItem";
            this.editUpdateMenuItem.Size = new System.Drawing.Size(118, 22);
            this.editUpdateMenuItem.Text = "&Update...";
            this.editUpdateMenuItem.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // editDeleteMenuItem
            // 
            this.editDeleteMenuItem.Name = "editDeleteMenuItem";
            this.editDeleteMenuItem.Size = new System.Drawing.Size(118, 22);
            this.editDeleteMenuItem.Text = "&Delete";
            this.editDeleteMenuItem.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // helpMenu
            // 
            this.helpMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.helpMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpAboutMenuItem});
            this.helpMenu.Name = "helpMenu";
            this.helpMenu.Size = new System.Drawing.Size(41, 20);
            this.helpMenu.Text = "&Help";
            // 
            // helpAboutMenuItem
            // 
            this.helpAboutMenuItem.Name = "helpAboutMenuItem";
            this.helpAboutMenuItem.Size = new System.Drawing.Size(212, 22);
            this.helpAboutMenuItem.Text = "&About the Environment Editor";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.toolStrip);
            this.splitContainer1.Panel1MinSize = 16;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl);
            this.helpProvider.SetHelpKeyword(this.splitContainer1.Panel2, "");
            this.helpProvider.SetShowHelp(this.splitContainer1.Panel2, true);
            this.splitContainer1.Size = new System.Drawing.Size(706, 438);
            this.splitContainer1.SplitterDistance = 20;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 10;
            // 
            // toolStrip
            // 
            this.toolStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripButton,
            this.updateToolStripButton,
            this.deleteToolStripButton});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip.Size = new System.Drawing.Size(706, 25);
            this.toolStrip.TabIndex = 10;
            this.toolStrip.Text = "toolStrip1";
            // 
            // newToolStripButton
            // 
            this.newToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.newToolStripButton.Image = global::Backsight.Environment.Editor.Properties.Resources.New;
            this.newToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.newToolStripButton.Name = "newToolStripButton";
            this.newToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.newToolStripButton.Text = "Create new item";
            this.newToolStripButton.Click += new System.EventHandler(this.newButton_Click);
            // 
            // updateToolStripButton
            // 
            this.updateToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.updateToolStripButton.Image = global::Backsight.Environment.Editor.Properties.Resources.Update;
            this.updateToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.updateToolStripButton.Name = "updateToolStripButton";
            this.updateToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.updateToolStripButton.Text = "Update selected item";
            this.updateToolStripButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // deleteToolStripButton
            // 
            this.deleteToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("deleteToolStripButton.Image")));
            this.deleteToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteToolStripButton.Name = "deleteToolStripButton";
            this.deleteToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.deleteToolStripButton.Text = "Delete selected item";
            this.deleteToolStripButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // tabControl
            // 
            this.tabControl.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tabControl.Controls.Add(this.domainsPage);
            this.tabControl.Controls.Add(this.entityTypesPage);
            this.tabControl.Controls.Add(this.fontsPage);
            this.tabControl.Controls.Add(this.idGroupsPage);
            this.tabControl.Controls.Add(this.layersPage);
            this.tabControl.Controls.Add(this.propertiesPage);
            this.tabControl.Controls.Add(this.tablesPage);
            this.tabControl.Controls.Add(this.templatesPage);
            this.tabControl.Controls.Add(this.themesPage);
            this.tabControl.Controls.Add(this.zonesPage);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Margin = new System.Windows.Forms.Padding(0);
            this.tabControl.Multiline = true;
            this.tabControl.Name = "tabControl";
            this.tabControl.Padding = new System.Drawing.Point(0, 0);
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(706, 417);
            this.tabControl.TabIndex = 2;
            this.tabControl.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl_Selected);
            // 
            // domainsPage
            // 
            this.domainsPage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.domainsPage.Location = new System.Drawing.Point(4, 4);
            this.domainsPage.Margin = new System.Windows.Forms.Padding(0);
            this.domainsPage.Name = "domainsPage";
            this.domainsPage.Size = new System.Drawing.Size(698, 388);
            this.domainsPage.TabIndex = 0;
            this.domainsPage.Text = "Domains";
            this.domainsPage.UseVisualStyleBackColor = true;
            // 
            // entityTypesPage
            // 
            this.entityTypesPage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.entityTypesPage.Location = new System.Drawing.Point(4, 4);
            this.entityTypesPage.Margin = new System.Windows.Forms.Padding(0);
            this.entityTypesPage.Name = "entityTypesPage";
            this.entityTypesPage.Size = new System.Drawing.Size(698, 388);
            this.entityTypesPage.TabIndex = 1;
            this.entityTypesPage.Text = "Entity Types";
            this.entityTypesPage.UseVisualStyleBackColor = true;
            // 
            // fontsPage
            // 
            this.fontsPage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fontsPage.Location = new System.Drawing.Point(4, 4);
            this.fontsPage.Margin = new System.Windows.Forms.Padding(0);
            this.fontsPage.Name = "fontsPage";
            this.fontsPage.Size = new System.Drawing.Size(698, 388);
            this.fontsPage.TabIndex = 2;
            this.fontsPage.Text = "Fonts";
            this.fontsPage.UseVisualStyleBackColor = true;
            // 
            // idGroupsPage
            // 
            this.idGroupsPage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.idGroupsPage.Location = new System.Drawing.Point(4, 4);
            this.idGroupsPage.Margin = new System.Windows.Forms.Padding(0);
            this.idGroupsPage.Name = "idGroupsPage";
            this.idGroupsPage.Size = new System.Drawing.Size(698, 388);
            this.idGroupsPage.TabIndex = 4;
            this.idGroupsPage.Text = "ID Groups";
            this.idGroupsPage.UseVisualStyleBackColor = true;
            // 
            // layersPage
            // 
            this.layersPage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.layersPage.Location = new System.Drawing.Point(4, 4);
            this.layersPage.Margin = new System.Windows.Forms.Padding(0);
            this.layersPage.Name = "layersPage";
            this.layersPage.Size = new System.Drawing.Size(698, 388);
            this.layersPage.TabIndex = 6;
            this.layersPage.Text = "Layers";
            this.layersPage.UseVisualStyleBackColor = true;
            // 
            // propertiesPage
            // 
            this.propertiesPage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.propertiesPage.Location = new System.Drawing.Point(4, 4);
            this.propertiesPage.Margin = new System.Windows.Forms.Padding(0);
            this.propertiesPage.Name = "propertiesPage";
            this.propertiesPage.Size = new System.Drawing.Size(698, 388);
            this.propertiesPage.TabIndex = 9;
            this.propertiesPage.Text = "Properties";
            this.propertiesPage.UseVisualStyleBackColor = true;
            // 
            // tablesPage
            // 
            this.tablesPage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tablesPage.Location = new System.Drawing.Point(4, 4);
            this.tablesPage.Margin = new System.Windows.Forms.Padding(0);
            this.tablesPage.Name = "tablesPage";
            this.tablesPage.Size = new System.Drawing.Size(698, 388);
            this.tablesPage.TabIndex = 5;
            this.tablesPage.Text = "Tables";
            this.tablesPage.UseVisualStyleBackColor = true;
            // 
            // templatesPage
            // 
            this.templatesPage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.templatesPage.Location = new System.Drawing.Point(4, 4);
            this.templatesPage.Margin = new System.Windows.Forms.Padding(0);
            this.templatesPage.Name = "templatesPage";
            this.templatesPage.Size = new System.Drawing.Size(698, 388);
            this.templatesPage.TabIndex = 8;
            this.templatesPage.Text = "Templates";
            this.templatesPage.UseVisualStyleBackColor = true;
            // 
            // themesPage
            // 
            this.themesPage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.themesPage.Location = new System.Drawing.Point(4, 4);
            this.themesPage.Margin = new System.Windows.Forms.Padding(0);
            this.themesPage.Name = "themesPage";
            this.themesPage.Size = new System.Drawing.Size(698, 388);
            this.themesPage.TabIndex = 7;
            this.themesPage.Text = "Themes";
            this.themesPage.UseVisualStyleBackColor = true;
            // 
            // zonesPage
            // 
            this.zonesPage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.zonesPage.Location = new System.Drawing.Point(4, 4);
            this.zonesPage.Margin = new System.Windows.Forms.Padding(0);
            this.zonesPage.Name = "zonesPage";
            this.zonesPage.Size = new System.Drawing.Size(698, 388);
            this.zonesPage.TabIndex = 3;
            this.zonesPage.Text = "Zones";
            this.zonesPage.UseVisualStyleBackColor = true;
            // 
            // helpProvider
            // 
            this.helpProvider.HelpNamespace = "C:\\Users\\sstanton\\Code\\Files\\Backsight.chm";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(706, 462);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip);
            this.helpProvider.SetHelpKeyword(this, "EnvMainForm");
            this.helpProvider.SetHelpNavigator(this, System.Windows.Forms.HelpNavigator.KeywordIndex);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.helpProvider.SetShowHelp(this, true);
            this.Text = "Environment Editor";
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileMenu;
        private System.Windows.Forms.ToolStripMenuItem fileOpenMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpMenu;
        private System.Windows.Forms.ToolStripMenuItem helpAboutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileSaveMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem fileExitMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem fileExportMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileImportMenuItem;
        private System.Windows.Forms.HelpProvider helpProvider;
        private System.Windows.Forms.ToolTip tipProvider;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage domainsPage;
        private System.Windows.Forms.TabPage entityTypesPage;
        private System.Windows.Forms.TabPage fontsPage;
        private System.Windows.Forms.TabPage idGroupsPage;
        private System.Windows.Forms.TabPage layersPage;
        private System.Windows.Forms.TabPage tablesPage;
        private System.Windows.Forms.TabPage templatesPage;
        private System.Windows.Forms.TabPage themesPage;
        private System.Windows.Forms.TabPage zonesPage;
        private System.Windows.Forms.TabPage propertiesPage;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editNewMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editUpdateMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editDeleteMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton newToolStripButton;
        private System.Windows.Forms.ToolStripButton deleteToolStripButton;
        private System.Windows.Forms.ToolStripButton updateToolStripButton;
        //private EntityListControl entityListControl;
    }
}

