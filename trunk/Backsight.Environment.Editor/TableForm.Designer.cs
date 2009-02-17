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
            this.tablesPage = new Gui.Wizard.WizardPage();
            this.tableList = new System.Windows.Forms.ListBox();
            this.columnsPage = new Gui.Wizard.WizardPage();
            this.excludeDomainTablesCheckBox = new System.Windows.Forms.CheckBox();
            this.excludeAlreadyAddedCheckBox = new System.Windows.Forms.CheckBox();
            this.columnsGrid = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.idColumnComboBox = new System.Windows.Forms.ComboBox();
            this.dgcColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcDataType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcDomain = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.wizard.SuspendLayout();
            this.tablesPage.SuspendLayout();
            this.columnsPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.columnsGrid)).BeginInit();
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
            this.wizard.Size = new System.Drawing.Size(752, 496);
            this.wizard.TabIndex = 24;
            this.wizard.CloseFromCancel += new System.ComponentModel.CancelEventHandler(this.wizard_CloseFromCancel);
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
            this.tablesPage.Size = new System.Drawing.Size(752, 448);
            this.tablesPage.TabIndex = 1;
            this.tablesPage.CloseFromNext += new Gui.Wizard.PageEventHandler(this.tablesPage_CloseFromNext);
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
            this.tableList.Size = new System.Drawing.Size(345, 308);
            this.tableList.Sorted = true;
            this.tableList.TabIndex = 24;
            // 
            // columnsPage
            // 
            this.columnsPage.Controls.Add(this.idColumnComboBox);
            this.columnsPage.Controls.Add(this.label2);
            this.columnsPage.Controls.Add(this.columnsGrid);
            this.columnsPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.columnsPage.IsFinishPage = true;
            this.columnsPage.Location = new System.Drawing.Point(0, 0);
            this.columnsPage.Name = "columnsPage";
            this.columnsPage.Size = new System.Drawing.Size(752, 448);
            this.columnsPage.TabIndex = 2;
            this.columnsPage.CloseFromNext += new Gui.Wizard.PageEventHandler(this.columnsPage_CloseFromNext);
            this.columnsPage.ShowFromNext += new System.EventHandler(this.columnsPage_ShowFromNext);
            // 
            // excludeDomainTablesCheckBox
            // 
            this.excludeDomainTablesCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.excludeDomainTablesCheckBox.AutoSize = true;
            this.excludeDomainTablesCheckBox.Checked = true;
            this.excludeDomainTablesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.excludeDomainTablesCheckBox.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.excludeDomainTablesCheckBox.Location = new System.Drawing.Point(36, 381);
            this.excludeDomainTablesCheckBox.Name = "excludeDomainTablesCheckBox";
            this.excludeDomainTablesCheckBox.Size = new System.Drawing.Size(132, 17);
            this.excludeDomainTablesCheckBox.TabIndex = 25;
            this.excludeDomainTablesCheckBox.Text = "Exclude domain tables";
            this.excludeDomainTablesCheckBox.UseVisualStyleBackColor = true;
            this.excludeDomainTablesCheckBox.CheckedChanged += new System.EventHandler(this.excludeDomainTablesCheckBox_CheckedChanged);
            // 
            // excludeAlreadyAddedCheckBox
            // 
            this.excludeAlreadyAddedCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.excludeAlreadyAddedCheckBox.AutoSize = true;
            this.excludeAlreadyAddedCheckBox.Checked = true;
            this.excludeAlreadyAddedCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.excludeAlreadyAddedCheckBox.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.excludeAlreadyAddedCheckBox.Location = new System.Drawing.Point(36, 404);
            this.excludeAlreadyAddedCheckBox.Name = "excludeAlreadyAddedCheckBox";
            this.excludeAlreadyAddedCheckBox.Size = new System.Drawing.Size(180, 17);
            this.excludeAlreadyAddedCheckBox.TabIndex = 26;
            this.excludeAlreadyAddedCheckBox.Text = "Exclude previously added tables";
            this.excludeAlreadyAddedCheckBox.UseVisualStyleBackColor = true;
            this.excludeAlreadyAddedCheckBox.CheckedChanged += new System.EventHandler(this.excludeAlreadyAddedCheckBox_CheckedChanged);
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
            this.columnsGrid.Location = new System.Drawing.Point(22, 32);
            this.columnsGrid.MultiSelect = false;
            this.columnsGrid.Name = "columnsGrid";
            this.columnsGrid.RowHeadersVisible = false;
            this.columnsGrid.Size = new System.Drawing.Size(559, 336);
            this.columnsGrid.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(24, 390);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Feature ID column";
            // 
            // idColumnComboBox
            // 
            this.idColumnComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.idColumnComboBox.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.idColumnComboBox.FormattingEnabled = true;
            this.idColumnComboBox.Location = new System.Drawing.Point(142, 387);
            this.idColumnComboBox.Name = "idColumnComboBox";
            this.idColumnComboBox.Size = new System.Drawing.Size(225, 24);
            this.idColumnComboBox.TabIndex = 2;
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
            // 
            // TableForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(752, 496);
            this.Controls.Add(this.wizard);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "TableForm";
            this.Text = "Database Table";
            this.Load += new System.EventHandler(this.TableForm_Load);
            this.wizard.ResumeLayout(false);
            this.tablesPage.ResumeLayout(false);
            this.tablesPage.PerformLayout();
            this.columnsPage.ResumeLayout(false);
            this.columnsPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.columnsGrid)).EndInit();
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
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcColumnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcDataType;
        private System.Windows.Forms.DataGridViewComboBoxColumn dgcDomain;
    }
}