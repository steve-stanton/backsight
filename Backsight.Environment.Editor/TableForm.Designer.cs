namespace Backsight.Environment.Editor
{
    partial class TableForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.wizard = new Gui.Wizard.Wizard();
            this.columnsPage = new Gui.Wizard.WizardPage();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.columnsGrid = new System.Windows.Forms.DataGridView();
            this.dgcColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcDataType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcDomain = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.idColumnComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.clearDomainLinkLabel = new System.Windows.Forms.LinkLabel();
            this.domainsListBox = new System.Windows.Forms.ListBox();
            this.setDomainLinkLabel = new System.Windows.Forms.LinkLabel();
            this.tablesPage = new Gui.Wizard.WizardPage();
            this.excludeAlreadyAddedCheckBox = new System.Windows.Forms.CheckBox();
            this.excludeDomainTablesCheckBox = new System.Windows.Forms.CheckBox();
            this.tableList = new System.Windows.Forms.ListBox();
            this.wizard.SuspendLayout();
            this.columnsPage.SuspendLayout();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.columnsGrid)).BeginInit();
            this.tablesPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(25, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(376, 16);
            this.label1.TabIndex = 23;
            this.label1.Text = "Select the table containing attributes of spatial features";
            // 
            // wizard
            // 
            this.wizard.Controls.Add(this.columnsPage);
            this.wizard.Controls.Add(this.tablesPage);
            this.wizard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizard.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.wizard.Location = new System.Drawing.Point(0, 0);
            this.wizard.Name = "wizard";
            this.wizard.Pages.AddRange(new Gui.Wizard.WizardPage[] {
            this.tablesPage,
            this.columnsPage});
            this.wizard.Size = new System.Drawing.Size(761, 484);
            this.wizard.TabIndex = 24;
            this.wizard.CloseFromCancel += new System.ComponentModel.CancelEventHandler(this.wizard_CloseFromCancel);
            // 
            // columnsPage
            // 
            this.columnsPage.Controls.Add(this.splitContainer);
            this.columnsPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.columnsPage.IsFinishPage = true;
            this.columnsPage.Location = new System.Drawing.Point(0, 0);
            this.columnsPage.Name = "columnsPage";
            this.columnsPage.Size = new System.Drawing.Size(761, 436);
            this.columnsPage.TabIndex = 2;
            this.columnsPage.CloseFromNext += new Gui.Wizard.PageEventHandler(this.columnsPage_CloseFromNext);
            this.columnsPage.ShowFromNext += new System.EventHandler(this.columnsPage_ShowFromNext);
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.columnsGrid);
            this.splitContainer.Panel1.Controls.Add(this.idColumnComboBox);
            this.splitContainer.Panel1.Controls.Add(this.label2);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.label3);
            this.splitContainer.Panel2.Controls.Add(this.clearDomainLinkLabel);
            this.splitContainer.Panel2.Controls.Add(this.domainsListBox);
            this.splitContainer.Panel2.Controls.Add(this.setDomainLinkLabel);
            this.splitContainer.Size = new System.Drawing.Size(761, 436);
            this.splitContainer.SplitterDistance = 528;
            this.splitContainer.TabIndex = 9;
            // 
            // columnsGrid
            // 
            this.columnsGrid.AllowUserToAddRows = false;
            this.columnsGrid.AllowUserToDeleteRows = false;
            this.columnsGrid.AllowUserToResizeRows = false;
            this.columnsGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.columnsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.columnsGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgcColumnName,
            this.dgcDataType,
            this.dgcDomain});
            this.columnsGrid.Location = new System.Drawing.Point(19, 39);
            this.columnsGrid.MultiSelect = false;
            this.columnsGrid.Name = "columnsGrid";
            this.columnsGrid.RowHeadersVisible = false;
            this.columnsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.columnsGrid.Size = new System.Drawing.Size(492, 325);
            this.columnsGrid.TabIndex = 0;
            this.columnsGrid.TabStop = false;
            // 
            // dgcColumnName
            // 
            this.dgcColumnName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgcColumnName.HeaderText = "Column name";
            this.dgcColumnName.Name = "dgcColumnName";
            this.dgcColumnName.ReadOnly = true;
            // 
            // dgcDataType
            // 
            this.dgcDataType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgcDataType.HeaderText = "Data type";
            this.dgcDataType.Name = "dgcDataType";
            this.dgcDataType.ReadOnly = true;
            // 
            // dgcDomain
            // 
            this.dgcDomain.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgcDomain.HeaderText = "Domain";
            this.dgcDomain.Name = "dgcDomain";
            this.dgcDomain.ReadOnly = true;
            this.dgcDomain.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgcDomain.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // idColumnComboBox
            // 
            this.idColumnComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.idColumnComboBox.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.idColumnComboBox.FormattingEnabled = true;
            this.idColumnComboBox.Location = new System.Drawing.Point(135, 381);
            this.idColumnComboBox.Name = "idColumnComboBox";
            this.idColumnComboBox.Size = new System.Drawing.Size(243, 24);
            this.idColumnComboBox.TabIndex = 2;
            this.idColumnComboBox.TabStop = false;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(16, 384);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Feature ID column";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Available domains";
            // 
            // clearDomainLinkLabel
            // 
            this.clearDomainLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.clearDomainLinkLabel.AutoSize = true;
            this.clearDomainLinkLabel.Location = new System.Drawing.Point(21, 349);
            this.clearDomainLinkLabel.Name = "clearDomainLinkLabel";
            this.clearDomainLinkLabel.Size = new System.Drawing.Size(70, 13);
            this.clearDomainLinkLabel.TabIndex = 8;
            this.clearDomainLinkLabel.TabStop = true;
            this.clearDomainLinkLabel.Text = "Clear Domain";
            this.clearDomainLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.clearDomainLinkLabel_LinkClicked);
            // 
            // domainsListBox
            // 
            this.domainsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.domainsListBox.FormattingEnabled = true;
            this.domainsListBox.Location = new System.Drawing.Point(20, 71);
            this.domainsListBox.Name = "domainsListBox";
            this.domainsListBox.Size = new System.Drawing.Size(187, 251);
            this.domainsListBox.TabIndex = 3;
            this.domainsListBox.DoubleClick += new System.EventHandler(this.domainsListBox_DoubleClick);
            // 
            // setDomainLinkLabel
            // 
            this.setDomainLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.setDomainLinkLabel.AutoSize = true;
            this.setDomainLinkLabel.Location = new System.Drawing.Point(21, 331);
            this.setDomainLinkLabel.Name = "setDomainLinkLabel";
            this.setDomainLinkLabel.Size = new System.Drawing.Size(61, 13);
            this.setDomainLinkLabel.TabIndex = 7;
            this.setDomainLinkLabel.TabStop = true;
            this.setDomainLinkLabel.Text = "Set Domain";
            this.setDomainLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.setDomainLinkLabel_LinkClicked);
            // 
            // tablesPage
            // 
            this.tablesPage.Controls.Add(this.excludeAlreadyAddedCheckBox);
            this.tablesPage.Controls.Add(this.excludeDomainTablesCheckBox);
            this.tablesPage.Controls.Add(this.tableList);
            this.tablesPage.Controls.Add(this.label1);
            this.tablesPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tablesPage.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tablesPage.IsFinishPage = false;
            this.tablesPage.Location = new System.Drawing.Point(0, 0);
            this.tablesPage.Name = "tablesPage";
            this.tablesPage.Size = new System.Drawing.Size(761, 436);
            this.tablesPage.TabIndex = 1;
            this.tablesPage.CloseFromNext += new Gui.Wizard.PageEventHandler(this.tablesPage_CloseFromNext);
            // 
            // excludeAlreadyAddedCheckBox
            // 
            this.excludeAlreadyAddedCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.excludeAlreadyAddedCheckBox.AutoSize = true;
            this.excludeAlreadyAddedCheckBox.Checked = true;
            this.excludeAlreadyAddedCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.excludeAlreadyAddedCheckBox.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.excludeAlreadyAddedCheckBox.Location = new System.Drawing.Point(36, 392);
            this.excludeAlreadyAddedCheckBox.Name = "excludeAlreadyAddedCheckBox";
            this.excludeAlreadyAddedCheckBox.Size = new System.Drawing.Size(180, 17);
            this.excludeAlreadyAddedCheckBox.TabIndex = 26;
            this.excludeAlreadyAddedCheckBox.Text = "Exclude previously added tables";
            this.excludeAlreadyAddedCheckBox.UseVisualStyleBackColor = true;
            this.excludeAlreadyAddedCheckBox.CheckedChanged += new System.EventHandler(this.excludeAlreadyAddedCheckBox_CheckedChanged);
            // 
            // excludeDomainTablesCheckBox
            // 
            this.excludeDomainTablesCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.excludeDomainTablesCheckBox.AutoSize = true;
            this.excludeDomainTablesCheckBox.Checked = true;
            this.excludeDomainTablesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.excludeDomainTablesCheckBox.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.excludeDomainTablesCheckBox.Location = new System.Drawing.Point(36, 369);
            this.excludeDomainTablesCheckBox.Name = "excludeDomainTablesCheckBox";
            this.excludeDomainTablesCheckBox.Size = new System.Drawing.Size(132, 17);
            this.excludeDomainTablesCheckBox.TabIndex = 25;
            this.excludeDomainTablesCheckBox.Text = "Exclude domain tables";
            this.excludeDomainTablesCheckBox.UseVisualStyleBackColor = true;
            this.excludeDomainTablesCheckBox.CheckedChanged += new System.EventHandler(this.excludeDomainTablesCheckBox_CheckedChanged);
            // 
            // tableList
            // 
            this.tableList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableList.FormattingEnabled = true;
            this.tableList.ItemHeight = 16;
            this.tableList.Location = new System.Drawing.Point(27, 53);
            this.tableList.Name = "tableList";
            this.tableList.Size = new System.Drawing.Size(354, 292);
            this.tableList.Sorted = true;
            this.tableList.TabIndex = 24;
            // 
            // TableForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(761, 484);
            this.Controls.Add(this.wizard);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "TableForm";
            this.Text = "Database Table";
            this.Load += new System.EventHandler(this.TableForm_Load);
            this.wizard.ResumeLayout(false);
            this.columnsPage.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.columnsGrid)).EndInit();
            this.tablesPage.ResumeLayout(false);
            this.tablesPage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private Gui.Wizard.Wizard wizard;
        private Gui.Wizard.WizardPage tablesPage;
        private System.Windows.Forms.ListBox tableList;
        private Gui.Wizard.WizardPage columnsPage;
        private System.Windows.Forms.CheckBox excludeAlreadyAddedCheckBox;
        private System.Windows.Forms.CheckBox excludeDomainTablesCheckBox;
        private System.Windows.Forms.ComboBox idColumnComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView columnsGrid;
        private System.Windows.Forms.ListBox domainsListBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcColumnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcDataType;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcDomain;
        private System.Windows.Forms.LinkLabel clearDomainLinkLabel;
        private System.Windows.Forms.LinkLabel setDomainLinkLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.SplitContainer splitContainer;
    }
}